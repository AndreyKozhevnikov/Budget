using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    [DebuggerDisplay("TagName-{TagName} Id-{Id}")]
    public class MyTag2 {
        public Tag parentTagEntity;
        
        public MyTag2() { }
       


    

        public string TagName {
            get {
                return parentTagEntity.TagName;
            
            }
            set {
                parentTagEntity.TagName = value;
            
            }
        }
        public int Id {
            get {
                return parentTagEntity.Id;
            }
            set {
                parentTagEntity.Id = value;
            }
        }
       
        public void Save() {
            OrderViewModel.generalEntity.SaveChanges();
            //string query;
            //if (Id == 0) //если слово новое (нет айди)
            //{
            //    query = string.Format(" insert into Tags (TagName) values ('{0}'); select SCOPE_IDENTITY()", TagName); //вставляем новое слово
            //} else //обновляем существующее
            //{
            //    query = string.Format("update tags set TagName='{0}', where id={1}", TagName, Id);

            //}
            //Id = int.Parse(MsSqlConnector.ExecuteScalar(query).ToString());
        }
    }
}
