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
using System.Windows.Threading;

namespace Budget.Views {
    /// <summary>
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView : UserControl {
        public SummaryView() {
            InitializeComponent();
        }

        private void GridControl_ItemsSourceChanged_1(object sender, ItemsSourceChangedEventArgs e) {
            GridControl gc = sender as GridControl;
            TableView tv = gc.View as TableView;

            Dispatcher.BeginInvoke((Action)(() => {
                tv.TopRowIndex = gc.VisibleRowCount;
            }), DispatcherPriority.Input);


        }
    }
}
