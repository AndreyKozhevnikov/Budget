using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;


namespace Budget {
    public partial class OrderViewModel : MyBindableBase {
        public OrderViewModel() {
            ConnectToDataBase();
            UpdateOrders(); //+
            PieChartVM = new PieChartViewModel(this);
            SummaryVM = new SummaryViewModel(this);
            EnterVM = new EnterViewModel(this);
        }
        void UpdateOrders() {
            var v = generalEntity.Orders.Select(x => new MyOrder() { parentOrderEntity = x }).ToList();
            Orders = new ObservableCollection<MyOrder>(v);
        }

        void OnChangeTab(object o) {
            var v = o as ValueChangedEventArgs<FrameworkElement>;
            if (v.NewValue is LayoutGroup) {
                LayoutGroup lg = v.NewValue as LayoutGroup;
                string nm = lg.Header.ToString();
                if (nm == "Summary") {
                    SummaryVM.CreateDateCollection();
                }
                if (nm == "Chart") {
                    PieChartVM.CreateGroupCollection();
                }
            }
        }


        private static void ConnectToDataBase() {

            string machineName = System.Environment.MachineName;
            if (machineName == "KOZHEVNIKOV-W10") {
                generalEntity = new BudgetEntities("BudgetEntitiesWork");
                DropboxPath = @"d:\dropbox\";
            }
            else {
                generalEntity = new BudgetEntities("BudgetEntitiesHome");
                DropboxPath = @"d:\dropbox\";
            }
#if DEBUG
            if (machineName == "KOZHEVNIKOV-W10")
                generalEntity = new BudgetEntities("BudgetEntitiesWork");
            else
                generalEntity = new BudgetEntities("BudgetEntitiesHomeTest");

#endif

        }
    }

    public partial class OrderViewModel {
        public static BudgetEntities generalEntity;
        public static string DropboxPath;

        ICommand _onChangeTabCommand;

        public PieChartViewModel PieChartVM { get; set; }
        public SummaryViewModel SummaryVM { get; set; }
        public EnterViewModel EnterVM { get; set; }
        public ObservableCollection<MyOrder> Orders { get; set; }

        public ICommand OnChangeTabCommand {
            get {
                if (_onChangeTabCommand == null)
                    _onChangeTabCommand = new DelegateCommand<object>(OnChangeTab);
                return _onChangeTabCommand;
            }

        }


    }
}
