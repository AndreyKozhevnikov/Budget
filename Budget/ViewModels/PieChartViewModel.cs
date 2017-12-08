
using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Budget {
    public partial class PieChartViewModel : MyBindableBase {
        public PieChartViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            MakePieChartList();
            MakeFilterList();
        }

        public void CreateGroupCollection() {
            var v3 = SelectedGroups.Select(origTag => new { Id = origTag.Id, TagName = origTag.TagName, Orders = origTag.Orders.Where(ord => ord.DateOrder >= targetDateItem.StartDate && ord.DateOrder < targetDateItem.FinishDate) }).Select(y => new GroupData { ParentTagName = y.TagName, ParentTagId = y.Id, Orders = y.Orders.ToList() }).ToList();
            GroupCollection = new ObservableCollection<GroupData>(v3);
            RaisePropertyChanged("GroupCollection");
        }

        public void MakePieChartList() {
            var tmpGroups = OrderViewModel.generalEntity.Tags.ToList();
            GroupList = new ObservableCollection<Tag>(tmpGroups);
            var tmpSelectedGroups = GroupList.Where(x => x.Id != 21 && x.Id != 22).ToList(); //remove credit & capital 
            SelectedGroups = new ObservableCollection<Tag>(tmpSelectedGroups);
        }

        void CreateDateOrderCollection() {
            var count = (int)(targetDateItem.FinishDate - targetDateItem.StartDate).TotalDays;
            var orders = SelectedGroup.Orders;
            IEnumerable<DateTime> datesList;
            IEnumerable<IGrouping<DateTime, Order>> groupOrdersByDate;
            if(count < 32) {
                datesList = Enumerable.Range(0, count + 1).Select(offset => targetDateItem.StartDate.AddDays(offset));
                groupOrdersByDate = orders.GroupBy(x => x.DateOrder);

            } else {
                var countMonth = (targetDateItem.FinishDate.Year * 12 + targetDateItem.FinishDate.Month) - (targetDateItem.StartDate.Year * 12 + targetDateItem.StartDate.Month);
                datesList = Enumerable.Range(0, countMonth + 1).Select(offset => new DateTime(targetDateItem.StartDate.Year, targetDateItem.StartDate.Month, 1).AddMonths(offset));
                groupOrdersByDate = orders.GroupBy(x => new DateTime(x.DateOrder.Year, x.DateOrder.Month, 1));
            }
            var ordersDateWithSum = groupOrdersByDate.Select(g => new { dt = g.Key, ssum = g.Sum(s => s.Value) }).ToList();
            var orderDataList = new List<DayOrderData>();
            foreach(var d in datesList) {
                var dd = new DayOrderData(d);
                var mDt = ordersDateWithSum.Where(x => x.dt == d).FirstOrDefault();
                if(mDt != null)
                    dd.Value = mDt.ssum;
                orderDataList.Add(dd);
            }
            DateOrderCollection = new ObservableCollection<DayOrderData>(orderDataList);
            RaisePropertyChanged("DateOrderCollection");
        }
        void MakeFilterList() {
            var uniqOrders = ParentViewModel.Orders.Select(x => x.DateOrder).Distinct();
            var AvailableDates2 = uniqOrders.Select(x => new { mnt = new DateTime(x.Year, x.Month, 1) }).GroupBy(x => x.mnt).Select(y => new { vl = (DateTime)y.Key }).Select(x => new { Display = x.vl.ToString("MMM yyyy"), v1 = x.vl, v2 = x.vl.AddMonths(1) }).Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = new ChartDateBounds() { StartDate = x.v1, FinishDate = x.v2 } }).ToList();
            TargetDateItem = (ChartDateBounds)AvailableDates2.Last().EditValue; //last month

            var years = uniqOrders.Select(x => new { year = new DateTime(x.Year, 1, 1) }).GroupBy(x => x.year).Select(y => new { vl = (DateTime)y.Key }).Select(x => new { Display = x.vl.ToString("'Full' yyyy"), v1 = x.vl, v2 = x.vl.AddYears(1) }).Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = new ChartDateBounds() { StartDate = x.v1, FinishDate = x.v2 } }).ToList();
            AvailableDates2.AddRange(years);

            var allItem = new CustomComboBoxItem() { DisplayValue = "ALL", EditValue = new ChartDateBounds() { StartDate = uniqOrders.Min(), FinishDate = uniqOrders.Max() } };
            AvailableDates2.Add(allItem);
            AvailableDates = AvailableDates2;
        }
    }



    public partial class PieChartViewModel {
        GroupData _selectedGroup;
        ICommand _updateMonthsCommand;
        public OrderViewModel ParentViewModel { get; set; }
        public ObservableCollection<Tag> SelectedGroups { get; set; }
        public ObservableCollection<Tag> GroupList { get; set; }
        public ObservableCollection<GroupData> GroupCollection { get; set; }
        public ObservableCollection<DayOrderData> DateOrderCollection { get; set; }
        List<CustomComboBoxItem> availableDates;
        public List<CustomComboBoxItem> AvailableDates {
            get {
                return availableDates;
            }
            set {
                availableDates = value;
                RaisePropertyChanged("AvailableDates");
            }
        }
        ChartDateBounds targetDateItem;

        public ChartDateBounds TargetDateItem {
            get {
                return targetDateItem;
            }
            set {
                targetDateItem = value;
                CreateGroupCollection();
                RaisePropertyChanged("TargetDateItem");
            }
        }

        public GroupData SelectedGroup {
            get { return _selectedGroup; }
            set {
                _selectedGroup = value;
                if(_selectedGroup != null) {
                    CreateDateOrderCollection();
                }
                RaisePropertyChanged("SelectedGroup");
            }
        }

        public ICommand UpdateMonthsCommand {
            get {
                if(_updateMonthsCommand == null)
                    _updateMonthsCommand = new DelegateCommand(MakeFilterList);
                return _updateMonthsCommand;
            }

        }
        public ICommand ExportSummaryCommand {
            get {
                if(_exportSummaryCommand == null)
                    _exportSummaryCommand = new DelegateCommand(ExportSummary);
                return _exportSummaryCommand;
            }
        }


        ICommand _exportSummaryCommand;
        public void ExportSummary() {
            var sw = new StreamWriter("summary.txt");
            sw.WriteLine("Name, Average, Sum, Counnt");
            foreach(var sum in GroupCollection) {
                var st = sum.ToExportString();
                sw.WriteLine(st);
            }
            sw.Close();
        }
    }
}
