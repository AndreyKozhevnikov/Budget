using Budget.Classes;
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
using System.Windows.Shapes;

namespace Budget.Views {
    /// <summary>
    /// Interaction logic for WebOrdersWindow.xaml
    /// </summary>
    public partial class WebOrdersWindow : Window {
        public WebOrdersWindow() {
            InitializeComponent();
            this.Loaded += WebOrdersWindow_Loaded;
        }

        private void WebOrdersWindow_Loaded(object sender, RoutedEventArgs e) {
            grid1.SelectAll();
        }

        public List<WebOrder> FinalList { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e) {
            FinalList = grid1.SelectedItems.Cast<WebOrder>().ToList();
            this.DialogResult = true;
            this.Close();
        }
    }
}
