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
    public class MyOrder : MyBindableBase {
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
                if (value == 22) {
                    Ignore = true;
                }
                RaisePropertyChanged("ParentTag");
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

        public int Source {
            get {
                return parentOrderEntity.Source;
            }
            set {
                parentOrderEntity.Source = value;
                RaisePropertyChanged();
            }
        }
        public bool Ignore {
            get {
                return parentOrderEntity.Ignore;
            }
            set {
                parentOrderEntity.Ignore = value;
                RaisePropertyChanged();
            }
        }

        public DateTime MonthDateOrder { get; set; }
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
