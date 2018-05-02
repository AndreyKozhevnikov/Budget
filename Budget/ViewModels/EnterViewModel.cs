using Budget.Classes;
using Budget.Properties;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Budget {
    public partial class EnterViewModel : MyBindableBase, ISupportServices {
        string budgetWebPath;
        public EnterViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            UpdateOrders();
            UpdateTags(); //+
            CreateNewCurrentOrder();//te
            CurrentDate = DateTime.Today;
            budgetWebPath = @"https://budgetweb.herokuapp.com";
#if DEBUG
            budgetWebPath = @"http://localhost:3000";
#endif

            SupportDateTable = new UniqueDateKeeper(ParentViewModel.Orders.Select(x => x.DateOrder));
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
            if(st == "Up")
                CurrentDate += new TimeSpan(1, 0, 0, 0);
            if(st == "Down")
                CurrentDate -= new TimeSpan(1, 0, 0, 0);
        }

        void EnterOrder() {
            CurrentOrder.AddEntityInstanceToBase();
            ParentViewModel.Orders.Add(CurrentOrder);
            SupportDateTable.AddDate(CurrentOrder.DateOrder);
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

            var lst = ParentViewModel.Orders.Where(x => x.DateOrder > DateTime.Today.AddDays(-90)).Select(x => new { pTag = x.ParentTag, vl = x.Value }).ToList();
            var lst2 = lst.GroupBy(x => x.pTag).ToList();
            var lst3 = lst2.Select(x => new { pn = x.Key, vl = x.Count(), sm = x.Sum(y => y.vl) }).ToList();
            var lst4 = lst3.OrderByDescending(x => x.vl).ToList();
            var lst5 = lst4.Select(x => x.pn).ToList();
            MyIntInListPositionComparer<int> comp = new MyIntInListPositionComparer<int>(lst5);
            var v2 = v.OrderBy(x => x.Id, comp).ToList();

            AllTags = new ObservableCollection<Tag>(v2);

            foreach(Tag tg in AllTags) {
                var tmp = lst3.Where(x => x.pn == tg.Id).FirstOrDefault();
                if(tmp != null) {
                    tg.ComplexValue = string.Format("{0} - {1} - {2}", tg.TagName, tmp.vl, tmp.sm);
                } else {
                    tg.ComplexValue = string.Format("{0} - {1} - {2}", tg.TagName, 0, 0);
                }
            }


            RaisePropertyChanged("AllTags");
        }

        void SaveNotSavedOrdersInBaseMehtod() {
            //           var ch = OrderViewModel.generalEntity.ChangeTracker.Entries().Where(x => x.State == System.Data.Entity.EntityState.Modified
            //      || x.State == System.Data.Entity.EntityState.Added)
            //.Select(x => x.Entity).ToList();

            OrderViewModel.generalEntity.SaveChanges();
        }
        void ExportXLSX() {
            string path = OrderViewModel.DropboxPath + @"common\BudgetExport.xlsx";
            TableViewExportToExcelService.ExportToExcel(path);

        }
        private void ImportFromWeb() {
            List<WebOrder> listWebOrders = GetWebOrders();
            if(listWebOrders.Count == 0)
                return;
            var finalList = ShowWebListWindowService.ShowWindow(listWebOrders);
            if(finalList == null)
                return;
            List<Tuple<WebOrder, MyOrder>> listForUPdateWeb = new List<Tuple<WebOrder, MyOrder>>();
            foreach(WebOrder webOrder in finalList) {
                var localOrder = CreateLocalOrderFromWeb(webOrder);
                ParentViewModel.Orders.Add(localOrder);

                listForUPdateWeb.Add(new Tuple<WebOrder, MyOrder>(webOrder, localOrder));
            }
            OrderViewModel.generalEntity.SaveChanges();
            var notUpdatedList = listWebOrders.Except(finalList);
            foreach(var notUpdatedItem in notUpdatedList) {
                listForUPdateWeb.Add(new Tuple<WebOrder, MyOrder>(notUpdatedItem, null));
            }

            foreach(var tuple in listForUPdateWeb) {
                UpdateLocalIdForWebOrder(tuple.Item1, tuple.Item2);
            }


        }
        void UpdateLocalIdForWebOrder(WebOrder webOrder, MyOrder localOrder) {
            int idToInsertInWeb;
            if(localOrder != null) {
                idToInsertInWeb = localOrder.Id;
            } else {
                idToInsertInWeb = -1;
            }
            var webid = webOrder._id;
            using(var client = new WebClient()) {
                var values = new NameValueCollection();
                values["id"] = webid;
                values["localid"] = idToInsertInWeb.ToString();
                var response = client.UploadValues(budgetWebPath + "/catalog/update/order", values);
                var responseString = Encoding.Default.GetString(response);
            }
        }

        MyOrder CreateLocalOrderFromWeb(WebOrder webOrder) {
            var par = OrderViewModel.generalEntity.Orders.Create();
            var localOrder = new MyOrder() { parentOrderEntity = par };
            localOrder.DateOrder = webOrder.DateOrder;
            localOrder.Description = webOrder.Description;
            localOrder.Value = webOrder.Value;
            localOrder.ParentTag = webOrder.ParentTag.LocalId;
            localOrder.IsJourney = webOrder.IsJourney;
            localOrder.Tags = webOrder.Tags;
            localOrder.AddEntityInstanceToBase();
            localOrder.IsFromWeb = true;
            return localOrder;
        }

        List<WebOrder> GetWebOrders() {
            using(var webClient = new WebClient()) {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add(HttpRequestHeader.Cookie, "cookiename="+ Resources.settings_log + ";  cookie2=value2");
                var json = webClient.DownloadString(budgetWebPath + "/catalog/orders/export");
                List<WebOrder> webOrderList;
                try {
                    webOrderList = JsonConvert.DeserializeObject<List<WebOrder>>(json);
                }
                catch(Exception e) {
                    return new List<WebOrder>();
                }
                return webOrderList;
            }
        }
        private void PreviewKeyHandler(KeyEventArgs e) {
            if(e.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
                if(CurrentOrder.Value == 0)
                    return;
                EnterOrder();
            }
            if(e.Key == Key.Add) {
                CurrentDate = CurrentDate.AddDays(1);
                e.Handled = true;
            }
            if(e.Key == Key.Subtract) {
                CurrentDate = CurrentDate.AddDays(-1);
                e.Handled = true;
            }
        }
        private void ShowFilterPopup(FilterPopupEventArgs e) {

            if(e.Column.FieldName == "DateOrder") {

                var v1 = e.ComboBoxEdit.ItemsSource as List<object>;
                //ppp
                //List<CustomComboBoxItem> listItems = v1.Cast<CustomComboBoxItem>().ToList();

                //var listMonth1 = listItems.Where(x => x.EditValue is DateTime).ToList();
                //var listMonth2 = listMonth1.Select(x => new { dt = ((DateTime)x.EditValue) }).ToList();
                //var listMonth3 = listMonth2.Select(x => new { mnt = new DateTime(x.dt.Year, x.dt.Month,1) }).ToList();
                //var listMonth4 = listMonth3.GroupBy(x => x.mnt).Select(y => new { vl =(DateTime) y.Key }).ToList();
                //var listMonth5 = listMonth4.Select(x => new { Display = x.vl.ToString("MMM yyyy"), v1 = x.vl, v2 = x.vl.AddMonths(1) }).ToList();
                //var listMonth6 = listMonth5.Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = CriteriaOperator.Parse(string.Format("[DateOrder]>=#{0}# and [DateOrder]<#{1}#", x.v1, x.v2)) }).ToList();

                //   var listMonth = v1.Cast<ICustomItem>().Where(x => x.EditValue is DateTime).Select(x => new { dt = ((DateTime)x.EditValue) }).Select(x => new { mnt = new DateTime(x.dt.Year, x.dt.Month, 1) }).GroupBy(x => x.mnt).Select(y => new { vl = (DateTime)y.Key }).Select(x => new { Display = x.vl.ToString("MMM yyyy"), v1 = x.vl, v2 = x.vl.AddMonths(1) }).Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = CriteriaOperator.Parse(string.Format("[DateOrder]>=#{0}# and [DateOrder]<#{1}#", x.v1, x.v2)) }).ToList();
                var listMonth = v1.Cast<ICustomItem>().Where(x => x.EditValue is DateTime).Distinct().Select(x => (DateTime)x.EditValue).Select(x => new { mnt = new DateTime(x.Year, x.Month, 1) }).GroupBy(x => x.mnt).Select(x => new { Display = x.Key.ToString("MMM yyyy"), v1 = x.Key, v2 = x.Key.AddMonths(1) }).Select(x => new CustomComboBoxItem() { DisplayValue = x.Display, EditValue = CriteriaOperator.Parse(string.Format("[DateOrder]>=#{0}# and [DateOrder]<#{1}#", x.v1, x.v2)) }).ToList();
                e.ComboBoxEdit.ItemsSource = listMonth;
            }
            if(e.Column.FieldName == "ParentTag") {
                var l = AllTags.Select(x => new CustomComboBoxItem() { DisplayValue = x.TagName, EditValue = x.TagName }).ToList();
                e.ComboBoxEdit.ItemsSource = l;
            }
        }




    }


    public partial class EnterViewModel {
        ICommand _changeCurrentOrderDateCommand;
        ICommand _enterOrderCommand;
        ICommand _saveNotSavedTagsInBaseCommand;
        ICommand _exportXLSXCommand;
        ICommand _previewKeyHandlerCommand;
        ICommand _showFilterPopupCommand;
        ICommand _importFromWebCommand;

        DateTime _currentDate;
        MyOrder _currentOrder;
        MyOrder _focusedInOutLayGridOrder;
        Tag _currentTag;
        string _exportProperty;

        public OrderViewModel ParentViewModel { get; set; }
        public UniqueDateKeeper SupportDateTable { get; set; }
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
                if(_changeCurrentOrderDateCommand == null)
                    _changeCurrentOrderDateCommand = new DelegateCommand<string>(ChangeCurrentOrderDateMethod, true);
                return _changeCurrentOrderDateCommand;
            }
        }
        public ICommand EnterOrderCommand {
            get {
                if(_enterOrderCommand == null)
                    _enterOrderCommand = new DelegateCommand(EnterOrder);
                return _enterOrderCommand;
            }
        }

        public ICommand SaveNotSavedOrdersInBaseCommand {
            get {
                if(_saveNotSavedTagsInBaseCommand == null)
                    _saveNotSavedTagsInBaseCommand = new DelegateCommand(SaveNotSavedOrdersInBaseMehtod);
                return _saveNotSavedTagsInBaseCommand;
            }
        }

        public ICommand ExportXLSXCommand {
            get {
                if(_exportXLSXCommand == null)
                    _exportXLSXCommand = new DelegateCommand(ExportXLSX);
                return _exportXLSXCommand;
            }
        }

        public ICommand ImportFromWebCommand {
            get {
                if(_importFromWebCommand == null)
                    _importFromWebCommand = new DelegateCommand(ImportFromWeb);
                return _importFromWebCommand;
            }
        }



        public ICommand PreviewKeyHandlerCommand {
            get {
                if(_previewKeyHandlerCommand == null)
                    _previewKeyHandlerCommand = new DelegateCommand<KeyEventArgs>(PreviewKeyHandler);
                return _previewKeyHandlerCommand;
            }
        }
        public ICommand ShowFilterPopupCommand {
            get {
                if(_showFilterPopupCommand == null)
                    _showFilterPopupCommand = new DelegateCommand<FilterPopupEventArgs>(ShowFilterPopup);
                return _showFilterPopupCommand;
            }
        }


        IServiceContainer serviceContainer = null;
        protected IServiceContainer ServiceContainer {
            get {
                if(serviceContainer == null)
                    serviceContainer = new ServiceContainer(this);
                return serviceContainer;
            }
        }
        IServiceContainer ISupportServices.ServiceContainer { get { return ServiceContainer; } }

        ITableViewExportToExcelService TableViewExportToExcelService { get { return ServiceContainer.GetService<ITableViewExportToExcelService>(); } }
        ISetFocusOnValueTextEdit SetFocusOnValueTextEditService { get { return ServiceContainer.GetService<ISetFocusOnValueTextEdit>(); } }
        IShowWebListWindow ShowWebListWindowService { get { return ServiceContainer.GetService<IShowWebListWindow>(); } }

    }



}
