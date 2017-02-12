using DevExpress.Data;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Budget {
    

    public interface ITableViewExportToExcelService {
        void ExportToExcel(string path);
    }

    public class TableViewExportToExcelService : ServiceBase, ITableViewExportToExcelService {


        public TableView ExportTableView {
            get { return (TableView)GetValue(ExportTableViewProperty); }
            set { SetValue(ExportTableViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExportTableView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExportTableViewProperty =
            DependencyProperty.Register("ExportTableView", typeof(TableView), typeof(TableViewExportToExcelService), new PropertyMetadata(null));



        public void ExportToExcel(string path) {
            if (OrderViewModel.IsTestMode)
                return;
            GridControl gc = ExportTableView.DataControl as GridControl;
            gc.Columns["DateOrder"].SortOrder = ColumnSortOrder.Descending;
            gc.Columns["DateOrder"].SortIndex = 0;
            try {
                ExportTableView.ExportToXlsx(path);
            }
            catch {
                MessageBox.Show("Export error");
            }
        }
    }


    public interface ISetFocusOnValueTextEdit {
        void SetFocus();
    }
    public class SetFocusOnValueTextEditService : ServiceBase, ISetFocusOnValueTextEdit {

        protected override void OnAttached() {
            base.OnAttached();
            targeTextEdit = this.AssociatedObject as TextEdit;
        }

        TextEdit targeTextEdit;


      
        public void SetFocus() {
            targeTextEdit.Focus();
        }
    }
}
