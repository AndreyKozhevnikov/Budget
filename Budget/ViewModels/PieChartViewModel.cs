using DevExpress.Mvvm;
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
       public PieChartViewModel( OrderViewModel vm) {
           ParentViewModel = vm;
           MakePieChartList();
       }
        public void CreateGroupCollection() {
            var v3 = SelectedGroups.Select(y => new GroupData { ParentTagName = y.TagName, ParentTagId = y.Id, Value = y.Orders.Sum(x => x.Value),Count=y.Orders.Count(),MaxValue=y.Orders.Max(x=>x.Value),MinValue=y.Orders.Min(x=>x.Value),StartDate=y.Orders.Select(z=>z.DateOrder).Min(), FinishDate = y.Orders.Select(z => z.DateOrder).Max() }).ToList();
            GroupCollection = new ObservableCollection<GroupData>(v3);
            RaisePropertyChanged("GroupCollection");
        }
        public void CreateGroupsOrderCollection() {
            var id = SelectedGroup.ParentTagName;
            var v =OrderViewModel.generalEntity.Orders.Where(x => x.Tag.TagName == id).OrderBy(x => x.DateOrder).ToList();
            GroupsOrderCollection = new ObservableCollection<Order>(v);
            RaisePropertyChanged("GroupsOrderCollection");

        }
        public void MakePieChartList() {
            var tmpGroups =OrderViewModel.generalEntity.Tags.ToList();
            GroupList = new ObservableCollection<Tag>(tmpGroups);
            var tmpSelectedGroups = GroupList.Where(x => x.Id != 21 && x.Id != 22).ToList(); //remove credit & capital 
            //var utilitiesGroup = tmpSelectedGroups.Where(x => x.Id == 25).First();
            //utilitiesGroup.Orders = utilitiesGroup.Orders.Where(x => x.DateOrder >= new DateTime(2016, 1, 1)).ToList();
            SelectedGroups = new ObservableCollection<Tag>(tmpSelectedGroups);
        }

        void CreateDateOrderCollection() {
            DateTime tmpDate =ParentViewModel.Orders.Min(x => x.DateOrder);
            var dStart = tmpDate.AddDays(tmpDate.Day * -1 + 1);
            var dFinish = DateTime.Now;
            dFinish = new DateTime(dFinish.Year, dFinish.Month, 1);
            var count = (dFinish.Year * 12 + dFinish.Month) - (dStart.Year * 12 + dStart.Month);
            var dates = Enumerable.Range(0, count + 1).Select(offset => dStart.AddMonths(offset)).ToList();
            var orders =ParentViewModel.Orders.Where(x => x.ParentTag == SelectedGroup.ParentTagId);
            orders.Select(x => x.MonthDateOrder = new DateTime(x.DateOrder.Year, x.DateOrder.Month, 1)).ToList();

        
            var ordertoMonth = orders.GroupBy(x => x.MonthDateOrder).Select(g => new { dt = g.Key, ssum = g.Sum(s => s.Value) }).ToList();
            var v = (from pd in dates join od in ordertoMonth on
                                     pd.Date equals od.dt
                         into t
                     from rt in t.DefaultIfEmpty(new { dt = pd.Date, ssum = 0 })
                     select new DayOrderData(pd.Date) {
                         Value = rt.ssum
                     }
                         ).ToList();
            DateOrderCollection = new ObservableCollection<DayOrderData>(v);
            RaisePropertyChanged("DateOrderCollection");
        }
    }

   public partial class PieChartViewModel {
       GroupData _selectedGroup;
       ICommand _createGroupCollectionCommand;
       public OrderViewModel ParentViewModel { get; set; }
       public ObservableCollection<Tag> SelectedGroups { get; set; }
       public ObservableCollection<Tag> GroupList { get; set; }
       public ObservableCollection<GroupData> GroupCollection { get; set; }
       public ObservableCollection<Order> GroupsOrderCollection { get; set; }
       public ObservableCollection<DayOrderData> DateOrderCollection { get; set; }

       public GroupData SelectedGroup {
           get { return _selectedGroup; }
           set {
               _selectedGroup = value;
               if (_selectedGroup != null) {
                   CreateGroupsOrderCollection();
                   CreateDateOrderCollection();
               }
               RaisePropertyChanged("SelectedGroup");
           }
       }

       public ICommand CreateGroupCollectionCommand {
           get {
               if (_createGroupCollectionCommand == null)
                   _createGroupCollectionCommand = new DelegateCommand(CreateGroupCollection);
               return _createGroupCollectionCommand;
           }

       }
        public ICommand ExportSummaryCommand {
            get {
                if (_exportSummaryCommand == null)
                    _exportSummaryCommand = new DelegateCommand(ExportSummary);
                return _exportSummaryCommand;
            }
        }


        ICommand _exportSummaryCommand;
        public void ExportSummary() {
            var sw = new StreamWriter("summary.txt");
            sw.WriteLine("Name, Average, Sum, Counnt");
            foreach (var sum in GroupCollection) {
                var st = sum.ToExportString();
                sw.WriteLine(st);
            }
            sw.Close();
        }
    }
}
