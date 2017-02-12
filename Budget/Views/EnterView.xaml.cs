using DevExpress.Data.Filtering;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private void GridControl_Loaded_1(object sender, RoutedEventArgs e) {
            GridControl gc = sender as GridControl;
            (gc.View as TableView).MoveLastRow();

        }

        private void LayoutGroup_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Tab) {
                Debug.Print(e.Key.ToString());
                LayoutGroup lg = sender as LayoutGroup;
                //  var l = lg.GetChildren(true).Where(x => x.GetType() == typeof(LayoutItem)).Select(y => (y as LayoutItem).Content).ToList();
                //   var l2 = lg.GetChildren(true,true,false).ToList();
                // remove LayoutTreeHelper and use GetChildre
                var l = LayoutTreeHelper.GetVisualChildren(lg).Where(x => x is BaseEdit).Cast<BaseEdit>().ToList();
                var f = l.Where(x => x.IsKeyboardFocusWithin == true).First(); ;

                var ind = l.IndexOf(f);

                int newInd = 0;
                if (ind > 0)
                    newInd = ind - 1;

                l[newInd].Focus();

                e.Handled = true;
            }
        }

        private void GridControl_CopyingToClipboard(object sender, CopyingToClipboardEventArgs e) {
            Clipboard.Clear();
            List<string> lstString = new List<string>();
            foreach (int rh in e.RowHandles) {
                GridControl gc = (e.Source as TableView).DataControl as GridControl;
                MyOrder ord = gc.GetRow(rh) as MyOrder;
                string tmpString = string.Format("{0}\t{1}\t{2}", ord.DateOrder, ord.Value, ord.Description);
                lstString.Add(tmpString);
            }
            string st = string.Join("\r\n", lstString);
            Clipboard.SetText(st);
            e.Handled = true;
        }
    }
}
