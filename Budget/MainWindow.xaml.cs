using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using System.Windows.Threading;
using System.Reflection;

namespace Budget {
    public partial class MainWindow : DXWindow {
        public MainWindow() {
            InitializeComponent();
            ViewModel = new OrderViewModel();
            DataContext = ViewModel;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            var st = this.Title + " - " + v;
#if DEBUG
           st  = st + " Debug mode";
#endif
            this.Title = st;
        }

        OrderViewModel ViewModel;

    }
    public class ObservableListConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // return value;
            List<object> lst = new List<object>();
            ObservableCollection<int> obcol = value as ObservableCollection<int>;
            foreach (int tg in obcol)
                lst.Add(tg);
            return lst;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            List<object> lst = value as List<object>;
            ObservableCollection<int> obcol = new ObservableCollection<int>();
            foreach (object ob in lst) {
                int val = (int)ob;
                obcol.Add(val);
            }
            return obcol;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

}
