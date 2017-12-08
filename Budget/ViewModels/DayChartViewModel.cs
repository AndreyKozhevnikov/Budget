
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Budget {
    public class DayChartViewModel : MyBindableBase {
        OrderViewModel ParentViewModel;
        const int idealComPerDay = 1500;
        const int idealEatPerDay = SummaryViewModel.NORMEATTOONEDAY;
        public DayChartViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            MakeMonthsList();
           
        }

        private void MakeMonthsList() {
            MonthsList = new List<CustomComboBoxItem>();
            var allDates = ParentViewModel.Orders.Select(x => x.DateOrder).Distinct();
            MonthsList = allDates.Select(x => new { mnt = new DateTime(x.Year, x.Month, 1) }).GroupBy(x => x.mnt).Select(y => new CustomComboBoxItem() { DisplayValue = y.Key.ToString("MMM yyyy"), EditValue = y.Key }).ToList();
            SelectedMonth = (DateTime)MonthsList.Last().EditValue;
        }
        public DateTime _selectedMonth;
        public DateTime SelectedMonth {
            get {
                return _selectedMonth;
            }
            set {
                _selectedMonth = value;
                MakeChart();
                RaisePropertyChanged("SelectedMonth");
            }
        }
        List<CustomComboBoxItem> monthsList;
        public List<CustomComboBoxItem> MonthsList {
            get {
                return monthsList;
            }
            set {
                monthsList = value;
                RaisePropertyChanged("MonthsList");
            }
        }
        public bool IsFullMonth { get; set; }
        public List<DaySummaryData> DaySummaryCollection { get; set; }
        ICommand _updateMonthsCommand;

        void MakeChart() {
           
            var targetDate = SelectedMonth;
            var fullOrderList = ParentViewModel.Orders.Where(x => x.DateOrder >= targetDate);
            var groupList = fullOrderList.GroupBy(x => x.DateOrder).Select(g => new { dt = g.Key, all = g.Sum(x => x.Value), eat = g.Where(t => t.ParentTag == 1).Sum(m => m.Value) }).OrderBy(x => x.dt).ToList();
            DateTime finishDate = targetDate.AddMonths(1).AddDays(-1);
            if (!IsFullMonth&&finishDate>DateTime.Today) {
                finishDate = DateTime.Today;
            }
            var dates = Enumerable.Range(0, finishDate.Subtract(targetDate).Days + 1).Select(offset => targetDate.AddDays(offset)).ToList();
            DaySummaryCollection = new List<DaySummaryData>();
            int allSum = 0;
            int idealCom = 0;
            int allEat = 0;
            int idealEat = 0;
            foreach (var date in dates) {
                var linqDate = groupList.Where(x => x.dt == date).FirstOrDefault();
                if (linqDate != null) {
                    allSum = allSum + linqDate.all;
                    allEat = allEat + linqDate.eat;
                }
                idealCom = idealCom + idealComPerDay;
                idealEat = idealEat + idealEatPerDay;
                var daySummary = new DaySummaryData(date, allSum, idealCom, allEat, idealEat);
                DaySummaryCollection.Add(daySummary);
            }



            RaisePropertyChanged("DaySummaryCollection");
        }
        public ICommand UpdateMonthsCommand {
            get {
                if (_updateMonthsCommand == null)
                    _updateMonthsCommand = new DelegateCommand(MakeMonthsList);
                return _updateMonthsCommand;
            }
        }
    }

    public class DaySummaryData {


        public DaySummaryData(DateTime date, int allsum, int idealCom, int allEat, int idealEat) {
            this.Day = date;
            this.SumCommon = allsum;
            this.IdealCommon = idealCom;
            this.SumEat = allEat;
            this.IdealEat = idealEat;

        }

        public DateTime Day { get; set; }
        public int SumCommon { get; set; }
        public int SumEat { get; set; }
        public int IdealCommon { get; set; }
        public int IdealEat { get; set; }
    }
}
