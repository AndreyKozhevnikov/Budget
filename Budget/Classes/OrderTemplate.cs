using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {


   
    public class delMyOrderTemplate {
        public delMyOrderTemplate(MyOrder baseOrder) {
            ParentTag = baseOrder.ParentTag;
            Value = baseOrder.Value;
            Source = baseOrder.Source;
            Description = baseOrder.Description;

            string st = EnterViewModel.AllTags.Where(x => x.Id == ParentTag).First().TagName;
            st += " ";
            st += Value.ToString();
            Name = st;

        }
        public delMyOrderTemplate(DataRow row) {
            Id = (int)row["Id"];
            Name = row["Name"].ToString();
            ParentTag = (int)row["ParentTag"];
            Value = (int)row["Value"];
            Source = (int)row["Source"];
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentTag { get; set; }
        public int Value { get; set; }
        public int Source { get; set; }
        public string Description { get; set; }

        public void Save() {
            string query;
            if (Id == 0) //если слово новое (нет айди)
            {
                query = string.Format(" insert into OrderTemplates (Name,ParentTag,Value,Source,Description) values ('{0}','{1}','{2}','{3}',N'{4}'); select SCOPE_IDENTITY()", Name, ParentTag, Value, Source, Description); //вставляем новое слово
            } else //обновляем существующее
            {
                query = string.Format("update OrderTemplates set Name='{1}',ParentTag='{2}',Value='{3}',Source='{4}',Description=N'{5}' where id={0}", Id, Name, ParentTag, Value, Source, Description);

            }
            Id = int.Parse(MsSqlConnector.ExecuteScalar(query).ToString());
        }
    }
}
