using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data;
using System.Diagnostics;

namespace Budget {
    [DebuggerDisplay("DateOrder-{DateOrder} Value-{Value}, ParentTag-{ParentTag}")]
    public class MyOrder : MyBindableBase , ILocalEntity {
        public Order parentOrderEntity;

        public MyOrder() {
            test = DateTime.Now.ToString();
        }

        string test;
        #region
        public int Id {
            get {
                return parentOrderEntity.Id;
            }
        }
        public DateTime DateOrder {
            get {
                return parentOrderEntity.DateOrder;
            }
            set {
                parentOrderEntity.DateOrder = value;
                RaisePropertyChanged("DateOrder");
            }
        }

        public int Value {
            get {
                return parentOrderEntity.Value;
            }
            set {
                parentOrderEntity.Value = value;
                RaisePropertyChanged("Value");
            }
        }
        public int? PaymentNumber {
            get {
                return parentOrderEntity.PaymentNumber;
            }
            set {
                parentOrderEntity.PaymentNumber = value;
                RaisePropertyChanged("PaymentNumber");
            }
        }
        public string Description {
            get {
                return parentOrderEntity.Description;
            }
            set {
                parentOrderEntity.Description = value;
                RaisePropertyChanged("Description");
            }
        }
        public int ParentTag {
            get {
                return parentOrderEntity.ParentTag;
            }
            set {
                parentOrderEntity.ParentTag = value;
                RaisePropertyChanged("ParentTag");
            }
        }
        public int? Place {
            get {
                return parentOrderEntity.Place;
            }
            set {
                parentOrderEntity.Place = value;
                RaisePropertyChanged("Place");
            }
        }
        public int? Object {
            get {
                return parentOrderEntity.Object;
            }
            set {
                parentOrderEntity.Object = value;
                RaisePropertyChanged("Object");
            }
        }

        public int? PaymentTypeId {
            get {
                return parentOrderEntity.PaymentTypeId;
            }
            set {
                parentOrderEntity.PaymentTypeId = value;
                RaisePropertyChanged("PaymentType");
            }
        }

        public string Tags {
            get {
                return parentOrderEntity.Tags;
            }
            set {
                parentOrderEntity.Tags = value;
                RaisePropertyChanged();
            }
        }

        public bool IsJourney {
            get {
                return parentOrderEntity.IsJourney;
            }
            set {
                parentOrderEntity.IsJourney = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFromWeb { get; set; }

        #endregion

























        //public MyOrder CreateEntityInstance() {
        //    parentOrderEntity = OrderViewModel.generalEntity.Orders.Create();
        //    return this;
        //  //  OrderViewModel.generalEntity.Orders.Add(parentOrderEntity);
        //}
        public void AddEntityInstanceToBase() {
            OrderViewModel.generalEntity.Orders.Add(parentOrderEntity);
        }




        public void Save() {
            OrderViewModel.generalEntity.SaveChanges();

            //IsSavedInBase = true;
        }

        public void GetPropertiesFromWebEntity(IWebEntity wEntity) {
            throw new NotImplementedException();
        }
    }

    //public class SimpleOrder {
    //    public int Id { get; set; }
    //    public int Value { get; set; }
    //    public DateTime DateOrder { get; set; }


    //}

    //public class SourceOrder {
    //    public SourceOrder(DataRow row) {
    //        Id = (int)row["Id"];
    //        Name = row["Name"].ToString();
    //    }
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    //public class TypeOrder {
    //    public TypeOrder(DataRow row) {
    //        Id = (int)row["Id"];
    //        Name = row["Name"].ToString();
    //    }
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
    //public class OwnerOrder {
    //    public OwnerOrder(DataRow row) {
    //        Id = (int)row["Id"];
    //        Name = row["Name"].ToString();
    //    }
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}





}
