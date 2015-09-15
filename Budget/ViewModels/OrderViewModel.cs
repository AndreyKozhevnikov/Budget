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
    public class OrderViewModel : MyBindableBase {
        public static bool IsTestMode = false;
        public static BudgetEntities generalEntity;
        public static string DropboxPath;

        ICommand _onChangeTabCommand;

  

        public OrderViewModel() {
          //  string st = "test string";
            
            ConnectToDataBase();
         
            UpdateOrders(); //+

            PieChartVM = new PieChartViewModel(this);
            SummaryVM = new SummaryViewModel(this);
            EnterVM = new EnterViewModel(this);
        }

     

        public PieChartViewModel PieChartVM { get; set; }
        public SummaryViewModel SummaryVM { get; set; }
        public EnterViewModel EnterVM { get; set; }

        public ICommand OnChangeTabCommand {
            get {
                if (_onChangeTabCommand == null)
                    _onChangeTabCommand = new DelegateCommand<object>(OnChangeTab);
                return _onChangeTabCommand; }
           
        }
   
      
       
        public ObservableCollection<MyOrder> Orders { get; set; }

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
            if (machineName == "KOZHEVNIKOV-W8") {
                generalEntity = new BudgetEntities("BudgetEntitiesWork");
                DropboxPath = @"d:\dropbox\";
            } else {
                generalEntity = new BudgetEntities("BudgetEntitiesHome");
                DropboxPath = @"f:\dropbox\";
            }
            if (IsTestMode) {
                if (machineName == "KOZHEVNIKOV-W8")
                    generalEntity = new BudgetEntities("BudgetEntitiesWork");
                else
                    generalEntity = new BudgetEntities("BudgetEntitiesHomeTest");
            }
        }
    }

    public static class FocusExtension {
        public static bool GetIsFocused(DependencyObject obj) {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value) {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
             "IsFocused", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));


        private static void OnIsFocusedPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e) {
            var uie = (UIElement)d;
            uie.Focus(); // Don't care about false values.
        }
    }
}
