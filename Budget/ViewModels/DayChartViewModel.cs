
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
        const int idealComPerDay= 1500;
        const int idealEatPerDay = SummaryViewModel.NORMEATTOONEDAY;
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
            var groupList = fullOrderList.GroupBy(x => x.DateOrder).Select(g => new { dt = g.Key, all = g.Sum(x => x.Value),eat=g.Where(t=>t.ParentTag==1).Sum(m=>m.Value)}).OrderBy(x=>x.dt).ToList();
            
            //  var finishDate = targetDate.AddMonths(1).AddDays(-1);
            var finishDate = DateTime.Today;
            var dates = Enumerable.Range(0, finishDate.Subtract(targetDate).Days+1).Select(offset => targetDate.AddDays(offset)).ToList();
            DaySummaryCollection = new List<DaySummaryData>();
            int allSum=0;
            int idealCom = 0;
            int allEat = 0;
            int idealEat = 0;
            foreach(var date in dates) {
                var linqDate = groupList.Where(x => x.dt == date).FirstOrDefault();
                if(linqDate != null) {
                    allSum = allSum + linqDate.all;
                    allEat = allEat + linqDate.eat;
                }
                idealCom = idealCom + idealComPerDay;
                idealEat = idealEat + idealEatPerDay;
                var daySummary = new DaySummaryData(date, allSum,idealCom,allEat,idealEat);
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
     

        public DaySummaryData(DateTime date, int allsum,int idealCom, int allEat, int idealEat) {
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
