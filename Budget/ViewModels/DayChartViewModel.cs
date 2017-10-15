
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Budget {
    public class DayChartViewModel: MyBindableBase {
        OrderViewModel ParentViewModel;
        const int idealPerDay= 1500;
        public DayChartViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
        }
        public List<DaySummaryData> DaySummaryCollection { get; set; }
        ICommand _makeChartCommand;

        DateTime targetMonth;
        void MakeChart() {
            targetMonth = DateTime.Today;
            var targetDate = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var fullOrderList = ParentViewModel.Orders.Where(x => x.DateOrder >= targetDate);
            var groupList = fullOrderList.GroupBy(x => x.DateOrder).Select(g => new { dt = g.Key, all = g.Sum(x => x.Value) }).OrderBy(x=>x.dt).ToList();

            //  var finishDate = targetDate.AddMonths(1).AddDays(-1);
            var finishDate = DateTime.Today;
            var dates = Enumerable.Range(0, finishDate.Subtract(targetDate).Days+1).Select(offset => targetDate.AddDays(offset)).ToList();
            DaySummaryCollection = new List<DaySummaryData>();
            int allSum=0;
            int ideal = 0;
            foreach(var date in dates) {
                var linqDate = groupList.Where(x => x.dt == date).FirstOrDefault();
                if(linqDate != null) {
                    allSum = allSum + linqDate.all;
                }
                ideal = ideal + idealPerDay;
                var daySummary = new DaySummaryData(date, allSum,ideal);
                DaySummaryCollection.Add(daySummary);
            }

           
            
            RaisePropertyChanged("DaySummaryCollection");
        }
        public ICommand MakeChartCommand {
            get {
                if (_makeChartCommand == null)
                    _makeChartCommand = new DelegateCommand(MakeChart);
                return _makeChartCommand;
            }
        }
    }

    public class DaySummaryData {
     

        public DaySummaryData(DateTime date, int allsum,int ideal) {
            this.Day = date;
            this.SumCommon = allsum;
            this.IdealSum = ideal;
        }

        public DateTime Day { get; set; }
        public int SumCommon { get; set; }
        public int SumEat { get; set; }
        public int IdealSum { get; set; }
    }
}
