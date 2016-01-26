using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Budget {
    public class EnterViewModel : MyBindableBase, ISupportServices {
        public EnterViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            UpdateOrders();
            UpdateTags(); //+

            CreateNewCurrentOrder();//te



            CurrentDate = DateTime.Today;

        }
        ICommand _changeCurrentOrderDateCommand;
        ICommand _enterOrderCommand;
        ICommand _saveNotSavedTagsInBaseCommand;
        ICommand _exportXLSXCommand;
        ICommand _previewKeyHandlerCommand;
        ICommand _showFilterPopupCommand;
        DateTime _currentDate;
        MyOrder _currentOrder;
        MyOrder _focusedInOutLayGridOrder;
        Tag _currentTag;
        string _exportProperty;


        public OrderViewModel ParentViewModel { get; set; }
        public ObservableCollection<MyOrder> SelectedOrders { get; set; }
        public static ObservableCollection<Tag> AllTags { get; set; }
        public int? SelectedItemsSumAll {
            get {
                return SelectedOrders.Sum(x => x.Value);
            }
        }
        public int? SelectedItemsSumEat {
            get {
                return SelectedOrders.Where(x => x.ParentTag == 1).Sum(x => x.Value);
            }
        }
       
        public DateTime CurrentDate {
            get { return _currentDate; }
            set {
                _currentDate = value;
                CurrentOrder.DateOrder = value;
                RaisePropertyChanged();

            }
        }
        public MyOrder CurrentOrder {
            get { return _currentOrder; }
            set {
                _currentOrder = value;
                RaisePropertyChanged();
            }
        }
        public MyOrder FocusedInOutLayGridOrder {
            get { return _focusedInOutLayGridOrder; }
            set {
                _focusedInOutLayGridOrder = value;
                RaisePropertyChanged();
            }
        }
        public Tag CurrentTag {
            get { return _currentTag; }
            set { _currentTag = value; }
        }

        public string ExportProperty {
            get { return _exportProperty; }
            set {
                _exportProperty = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ChangeCurrentOrderDate {
            get {
                if (_changeCurrentOrderDateCommand == null)
                    _changeCurrentOrderDateCommand = new DelegateCommand<string>(ChangeCurrentOrderDateMethod, true);
                return _changeCurrentOrderDateCommand;
            }
        }
        public ICommand EnterOrderCommand {
            get {
                if (_enterOrderCommand == null)
                    _enterOrderCommand = new DelegateCommand(EnterOrder);
                return _enterOrderCommand;
            }
        }
 
        public ICommand SaveNotSavedOrdersInBaseCommand {
            get {
                if (_saveNotSavedTagsInBaseCommand == null)
                    _saveNotSavedTagsInBaseCommand = new DelegateCommand(SaveNotSavedOrdersInBaseMehtod);
                return _saveNotSavedTagsInBaseCommand;
            }
        }

        public ICommand ExportXLSXCommand {
            get {
                if (_exportXLSXCommand == null)
                    _exportXLSXCommand = new DelegateCommand(ExportXLSX);
                return _exportXLSXCommand;
            }
        }

        public ICommand PreviewKeyHandlerCommand {
            get {
                if (_previewKeyHandlerCommand == null)
                    _previewKeyHandlerCommand = new DelegateCommand<KeyEventArgs>(PreviewKeyHandler);
                return _previewKeyHandlerCommand;
            }
        }
        public ICommand ShowFilterPopupCommand {
            get {
                if (_showFilterPopupCommand == null)
                    _showFilterPopupCommand = new DelegateCommand<FilterPopupEventArgs>(ShowFilterPopup);
                    return _showFilterPopupCommand;
            }
        }

     



        void UpdateOrders() {
            SelectedOrders = new ObservableCollection<MyOrder>();
            SelectedOrders.CollectionChanged += SelectedOrders_CollectionChanged;
        }


        void SelectedOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged("SelectedItemsSumAll");
            RaisePropertyChanged("SelectedItemsSumEat");
        }

        void ChangeCurrentOrderDateMethod(string st) {
            if (st == "Up")
                CurrentDate += new TimeSpan(1, 0, 0, 0);
            if (st == "Down")
                CurrentDate -= new TimeSpan(1, 0, 0, 0);
        }

        void EnterOrder() {
            CurrentOrder.AddEntityInstanceToBase();
            ParentViewModel.Orders.Add(CurrentOrder);
            FocusedInOutLayGridOrder = CurrentOrder;
            CreateNewCurrentOrder();
            SetFocusOnTextEditValue();
            OrderViewModel.generalEntity.SaveChanges();
        }

  

        void CreateNewCurrentOrder() {

            var par = OrderViewModel.generalEntity.Orders.Create();
            CurrentOrder = new MyOrder() { parentOrderEntity = par };
            CurrentOrder.DateOrder = CurrentDate;
            CurrentOrder.ParentTag = AllTags[0].Id;
            CurrentOrder.Ignore = false;

        }
        private void SetFocusOnTextEditValue() {
            SetFocusOnValueTextEditService.SetFocus();
        }


        void UpdateTags() {
            var v = OrderViewModel.generalEntity.Tags.ToList();

            var lst = ParentViewModel.Orders.Select(x => new { pTag = x.ParentTag, vl = x.Value }).ToList();
            var lst2 = lst.GroupBy(x => x.pTag).ToList();
            //var lst3=lst2.Select(x=>new{pn=x.Key,vl=x.Sum(y=>y.vl)}).ToList();
            var lst3 = lst2.Select(x => new { pn = x.Key, vl = x.Count() ,sm=x.Sum(y=>y.vl)}).ToList();
            var lst4 = lst3.OrderByDescending(x => x.vl).ToList();
            var lst5 = lst4.Select(x => x.pn).ToList();
            MyComparer<int> comp = new MyComparer<int>(lst5);
            var v2 = v.OrderBy(x => x.Id, comp).ToList();

            AllTags = new ObservableCollection<Tag>(v2);

            foreach (Tag tg in AllTags) {
                var tmp = lst3.Where(x => x.pn == tg.Id).First();
                tg.ComplexValue =string.Format("{0} - {1} - {2}",tg.TagName, tmp.vl,tmp.sm);
            }


            RaisePropertyChanged("AllTags");
        }
       
        void SaveNotSavedOrdersInBaseMehtod() {
            OrderViewModel.generalEntity.SaveChanges();
        }
        void ExportXLSX() {
            string path = OrderViewModel.DropboxPath + @"common\BudgetExport.xlsx";
            TableViewExportToExcelService.ExportToExcel(path);
        
        }

        private void PreviewKeyHandler(KeyEventArgs e) {
            if (e.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
                if (CurrentOrder.Value == 0)
                    return;
                EnterOrder();
            }
        }
        private void ShowFilterPopup(FilterPopupEventArgs e) {
           
            if (e.Column.FieldName == "DateOrder") {

                var v1 = e.ComboBoxEdit.ItemsSource as List<object>;
                //ppp
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
            if (e.Column.FieldName == "ParentTag") {
                var l = AllTags.Select(x => new CustomComboBoxItem() { DisplayValue = x.TagName, EditValue = x.TagName }).ToList();
                e.ComboBoxEdit.ItemsSource = l;
            }
        }



        IServiceContainer serviceContainer = null;
        protected IServiceContainer ServiceContainer {
            get {
                if (serviceContainer == null)
                    serviceContainer = new ServiceContainer(this);
                return serviceContainer;
            }
        }
        IServiceContainer ISupportServices.ServiceContainer { get { return ServiceContainer; } }

        ITableViewExportToExcelService TableViewExportToExcelService { get { return ServiceContainer.GetService<ITableViewExportToExcelService>(); } }
        ISetFocusOnValueTextEdit SetFocusOnValueTextEditService { get { return ServiceContainer.GetService<ISetFocusOnValueTextEdit>(); } }
    }

    class MyComparer<T> : IComparer<int> {
        public MyComparer(List<int> _innerList) {
            innerList = _innerList;
        }

        List<int> innerList;
        public int Compare(int x, int y) {
            var index1 = innerList.IndexOf(x);
            var index2 = innerList.IndexOf(y);
            var res = Comparer<int>.Default.Compare(index1, index2);

         //   Debug.Print(string.Format("{0} {1}  - {2} ", index1, index2, res));
            return res;
        }
    }

  
}
