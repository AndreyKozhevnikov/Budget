using DevExpress.Data;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Budget {
    public class ExportHelper {


        public static string GetExportProperty(DependencyObject obj) {
            return (string)obj.GetValue(ExportPropertyProperty);
        }

        public static void SetExportProperty(DependencyObject obj, string value) {
            obj.SetValue(ExportPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for ExportProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExportPropertyProperty =
            DependencyProperty.RegisterAttached("ExportProperty", typeof(string), typeof(ExportHelper), new PropertyMetadata(null, new PropertyChangedCallback(ExportChanged)));

        private static void ExportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue == null) return;
            if (OrderViewModel.IsTestMode)
                return;
            TableView tv = d as TableView;
            GridControl gc = tv.DataControl as GridControl;
            gc.Columns["DateOrder"].SortOrder = ColumnSortOrder.Descending;
            gc.Columns["DateOrder"].SortIndex = 0;//говнокод
            string path = e.NewValue.ToString();
            try {
                tv.ExportToXlsx(path);
            } catch {

            }
        }


    }
}
