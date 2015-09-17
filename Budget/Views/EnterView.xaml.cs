using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Budget.Views {
    /// <summary>
    /// Interaction logic for EnterView.xaml
    /// </summary>
    public partial class EnterView : UserControl {
        public EnterView() {
            InitializeComponent();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            OrderViewModel.generalEntity.SaveChanges();
        }
        private void GridControl_Loaded_1(object sender, RoutedEventArgs e) {
            GridControl gc = sender as GridControl;
            (gc.View as TableView).MoveLastRow();

        }

        private void TableView_ShowFilterPopup(object sender, FilterPopupEventArgs e) {
            if (e.Column.FieldName != "DateOrder")
                return;

            var v1 = e.ComboBoxEdit.ItemsSource as List<object>;

            //List<CustomComboBoxItem> listItems = v1.Cast<CustomComboBoxItem>().ToList();

            //var listMonth1 = listItems.Where(x => x.EditValue is DateTime).ToList();
            //var listMonth2 = listMonth1.Select(x => new { dt = ((DateTime)x.EditValue) }).ToList();
            //var listMonth3 = listMonth2.Select(x => new { mnt = new DateTime(x.dt.Year, x.dt.Month,1) }).ToList();
            //var listMonth4 = listMonth3.GroupBy(x => x.mnt).Select(y => new { vl =(DateTime) y.Key }).ToList();
            //var listMonth5 = listMonth4.Select(x => new { Display = x.vl.ToString("MMM yyyy"), v1 = x.vl, v2 = x.vl.AddMonths(1) }).ToList();
            //var listMonth6 = listMonth5.Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = CriteriaOperator.Parse(string.Format("[DateOrder]>=#{0}# and [DateOrder]<#{1}#", x.v1, x.v2)) }).ToList();

            var listMonth = v1.Cast<CustomComboBoxItem>().Where(x => x.EditValue is DateTime).Select(x => new { dt = ((DateTime)x.EditValue) }).Select(x => new { mnt = new DateTime(x.dt.Year, x.dt.Month, 1) }).GroupBy(x => x.mnt).Select(y => new { vl = (DateTime)y.Key }).Select(x => new { Display = x.vl.ToString("MMM yyyy"), v1 = x.vl, v2 = x.vl.AddMonths(1) }).Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = CriteriaOperator.Parse(string.Format("[DateOrder]>=#{0}# and [DateOrder]<#{1}#", x.v1, x.v2)) }).ToList();
            e.ComboBoxEdit.ItemsSource = listMonth;

        }
    }
}
