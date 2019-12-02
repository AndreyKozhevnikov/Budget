using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Budget {
    public partial class SummaryViewModel : MyBindableBase {

        public SummaryViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            DateCollection = new ObservableCollection<DayData>();
        }
        public void CreateDateCollection() {
            DateCollection.Clear();
            DateTime? tmpDate = ParentViewModel.Orders.Min(x => x.DateOrder);
            List<MyOrder> Orders = null;
            if (IsLightOrders) {
                Orders = ParentViewModel.Orders.Where(x => x.ParentTag != 23 && x.ParentTag != 6 && x.ParentTag != 21 && x.ParentTag != 24).ToList();
            }
            else {
                Orders = ParentViewModel.Orders.ToList();
            }

            var sumAll = Orders.GroupBy(s => s.DateOrder).Select(g => new { mid = g.Key, mall = g.Sum(s => s.Value) }).ToList();
            var sumEat = Orders.Where(x => x.ParentTag == 1).GroupBy(s => s.DateOrder).Select(g => new { mid = g.Key, mall = g.Sum(s => s.Value) }).ToList();

            DateTime tmpDateFinish = DateTime.Today;
            var dates = Enumerable.Range(0, 1 + tmpDateFinish.Subtract(tmpDate.Value).Days).Select(offset => tmpDate.Value.AddDays(offset)).ToList();

            var summall = (from sAll in sumAll
                           join sEat in sumEat
                           on sAll.mid equals sEat.mid
                           into t
                           from rt in t.DefaultIfEmpty(new { mid = DateTime.Today, mall = 0 })

                           select new {
                               date = sAll.mid,
                               sumofall = sAll.mall,
                               sumofeat = rt.mall
                           })
                               .ToList();

            
            DateCollection = new ObservableCollection<DayData>((from pd in dates
                                                                join od in summall on
                    pd.Date equals od.date
                    into t
                                                                from rt in t.DefaultIfEmpty(new { date = DateTime.Today, sumofall = 0, sumofeat = 0 })
                                                                orderby pd.Date
                                                                select new DayData(pd.Date) {
                                                                    SumAll = rt.sumofall,
                                                                    SumOfEat = rt.sumofeat
                                                                }
                                   ).ToList());
            RaisePropertyChanged("DateCollection");
            CreateDateChartCollection();
            CalculateEatStatistic();
        }

        void CreateDateChartCollection() {
            DateTime tmpDate = DateCollection.Min(x => x.DayDate);
            var dStart = tmpDate.AddDays(tmpDate.Day * -1 + 1);
            var dFinish = DateTime.Now;
            dFinish = new DateTime(dFinish.Year, dFinish.Month, 1);
            var count = (dFinish.Year * 12 + dFinish.Month) - (dStart.Year * 12 + dStart.Month);
            var dates = Enumerable.Range(0, count + 1).Select(offset => dStart.AddMonths(offset)).ToList();
            var dateData = DateCollection.Select(x => new DayData() { DayDate = new DateTime(x.DayDate.Year, x.DayDate.Month, 1), SumAll = x.SumAll, SumOfEat = x.SumOfEat }).ToList();

            var dateDataToMonth = dateData.GroupBy(x => x.DayDate).Select(g => new DayData() { DayDate = g.Key, SumAll = g.Sum(s => s.SumAll), SumOfEat = g.Sum(s => s.SumOfEat) }).ToList();

            var v = (from pd in dates
                     join od in dateDataToMonth on
                     pd.Date equals od.DayDate
                     into t
                     from rt in t.DefaultIfEmpty(new DayData() { DayDate = pd.Date, SumAll = 0, SumOfEat = 0 })
                     select new DayData() { DayDate = pd.Date, SumAll = rt.SumAll, SumOfEat = rt.SumOfEat }).ToList();
            DateChartCollection = new ObservableCollection<DayData>(v);

            RaisePropertyChanged("DateChartCollection");
        }

        void CalculateEatStatistic() {
            int d = DateTime.Now.Day;
            int norm = d * NORMEATTOONEDAY;
            NormToThisMonth = string.Format("{0} ({1})", norm, d);
            DateTime curMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DayData currentDayData = DateChartCollection.Where(x => x.DayDate == curMonth).First();
            SpentThisMonth = currentDayData.SumOfEat;
            Balance = norm - SpentThisMonth;
        }
    }

    public partial class SummaryViewModel {
        public int NORMEATTOONEDAY = Properties.Settings.Default.NormEatToOneDay;

        bool isLightOrders;
        string _normToThisMonth;
        int _spentThisMonth;
        int _balance;

        ICommand _createDateCollectionCommand;

        public OrderViewModel ParentViewModel { get; set; }
        public ObservableCollection<DayData> DateCollection { get; set; }
        public ObservableCollection<DayData> DateChartCollection { get; set; }
        public bool IsLightOrders {
            get { return isLightOrders; }
            set { isLightOrders = value; }
        }
        public string NormToThisMonth {
            get { return _normToThisMonth; }
            set {
                _normToThisMonth = value;
                RaisePropertyChanged();
            }
        }
        public int SpentThisMonth {
            get { return _spentThisMonth; }
            set {
                _spentThisMonth = value;
                RaisePropertyChanged();
            }
        }
        public int Balance {
            get { return _balance; }
            set {
                _balance = value;
                RaisePropertyChanged();
            }
        }
        public ICommand CreateDateCollectionCommand {
            get {
                if (_createDateCollectionCommand == null)
                    _createDateCollectionCommand = new DelegateCommand(CreateDateCollection);
                return _createDateCollectionCommand;
            }
        }
    }
}
