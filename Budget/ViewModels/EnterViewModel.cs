using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Budget {
    public class EnterViewModel : MyBindableBase {
        public EnterViewModel(OrderViewModel vm) {
            ParentViewModel = vm;
            UpdateOrders();
            MakeLists();//+
            UpdateTags(); //+

            CreateNewCurrentOrder();//te
            SetFocusOnTextEditValue();



            CurrentDate = DateTime.Today;

        }
        ICommand _changeCurrentOrderDateCommand;
        ICommand _enterOrderCommand;
        ICommand _saveTagCommand;
        ICommand _addTagCommand;
        ICommand _saveNotSavedTagsInBaseCommand;
        ICommand _exportXLSXCommand;
        ICommand _previewKeyHandlerCommand;

        DateTime _currentDate;
        MyOrder _currentOrder;
        MyOrder _focusedInOutLayGridOrder;
        bool _isValueTextEditFocused;
        AddTagWindow addtgwnd;
        Tag _currentTag;
        string _exportProperty;


        public OrderViewModel ParentViewModel { get; set; }
        public ObservableCollection<MyOrder> SelectedOrders { get; set; }
        public ObservableCollection<Source> Sources { get; set; }
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
        public int? SelectedItemsSumCard {
            get {
                return SelectedOrders.Where(x => x.Source == 3).Sum(x => x.Value);
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
        public bool IsValueTextEditFocused {
            get { return _isValueTextEditFocused; }
            set {
                _isValueTextEditFocused = value;
                RaisePropertyChanged();
            }
        }
        public Tag CurrentTag {
            get { return _currentTag; }
            set { _currentTag = value; }
        }

        public string ExportProperty {
            get { return _exportProperty; }
            set { _exportProperty = value;
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
        public ICommand SaveTagCommand {
            get {
                if (_saveTagCommand == null)
                    _saveTagCommand = new DelegateCommand(SaveTag);
                return _saveTagCommand;
            }
        }
        public ICommand AddTagCommand {
            get {
                if (_addTagCommand == null)
                    _addTagCommand = new DelegateCommand(AddTag);
                return _addTagCommand;
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

      
        void UpdateOrders() {
            SelectedOrders = new ObservableCollection<MyOrder>();
            SelectedOrders.CollectionChanged += SelectedOrders_CollectionChanged;
        }


        void SelectedOrders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged("SelectedItemsSumAll");
            RaisePropertyChanged("SelectedItemsSumEat");
            RaisePropertyChanged("SelectedItemsSumCard");
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
            CurrentOrder.Source = Sources[1].Id;
            CurrentOrder.ParentTag = AllTags[0].Id;
            CurrentOrder.Ignore = false;

        }

        void SetFocusOnTextEditValue() {
            IsValueTextEditFocused =  !IsValueTextEditFocused;
        }

        void AddTag() {
            addtgwnd = new AddTagWindow();
            addtgwnd.DataContext = this;
            CurrentTag = OrderViewModel.generalEntity.Tags.Create();
            addtgwnd.ShowDialog();
        }
        void SaveTag() {
            OrderViewModel.generalEntity.Tags.Add(CurrentTag);
            OrderViewModel.generalEntity.SaveChanges();
            AllTags.Add(CurrentTag);
            CurrentOrder.ParentTag = CurrentTag.Id;
            addtgwnd.Close();
        }

        void UpdateTags() {
            var v = OrderViewModel.generalEntity.Tags.ToList();
            AllTags = new ObservableCollection<Tag>(v);
            RaisePropertyChanged("AllTags");
        }
        void MakeLists() {

            var tmpSources = OrderViewModel.generalEntity.Sources.ToList();
            Sources = new ObservableCollection<Source>(tmpSources);
        }
        void SaveNotSavedOrdersInBaseMehtod() {
            OrderViewModel.generalEntity.SaveChanges();
        }
        void ExportXLSX() {
            string path =OrderViewModel.DropboxPath + @"common\BudgetExport.xlsx";
            ExportProperty = path;
            ExportProperty = null; //говнокод
         }

        private void PreviewKeyHandler(KeyEventArgs e) {
            if (e.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
                IsValueTextEditFocused = false;
                EnterOrder();
            }
        }

    }
}
