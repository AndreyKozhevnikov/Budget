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
        private void GridControl_Loaded_1(object sender, RoutedEventArgs e) {
            GridControl gc = sender as GridControl;
            (gc.View as TableView).MoveLastRow();

        }
    }
}
