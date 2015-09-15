using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Budget {
    public static class MsSqlConnector //библиотека соединения с базой данных MsSql
     {
        public static string MsServerName; //база
        public static string MsUser; //пользователь
        public static string MsPass; //пароль
        public static string MsDataBase; // каталог базы
        public static bool MsIntegratedSecurity; //встроенная аутентификация
        static SqlConnection connSql;//коннекшн
        static SqlCommand commSql;//комманд
        static bool State; //состояние
        public static string Open() //открытие подключения
        {
            string res = "ok"; //результат
            if (!State) //если еще не были подключены
            {
                try //подключаемся
                {
                    //  string sConn = string.Format ( @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", MsServerName, MsDataBase, MsUser, MsPass ); // база и пароль к ней тут
                    //  string sConn = string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=True", MsServerName, MsDataBase, MsUser, MsPass); // база и пароль к ней тут
                    string sConn = "";
                    //if (MsIntegratedSecurity)
                    //    sConn = string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=True", MsServerName, MsDataBase, MsUser, MsPass); // база и пароль к ней тут
                    //else
                        sConn = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", MsServerName, MsDataBase, MsUser, MsPass); // база и пароль к ней тут

                    connSql = new SqlConnection(sConn);
                    //connSql = new SqlConnection ( @"Data Source=SERVER-AVAYA;Initial Catalog=" + MsCatalog + ";User ID=sa;Password=gfypth" ); // база и пароль к ней тут
                    connSql.Open(); // открытие
                    commSql = connSql.CreateCommand(); //создание команды
                    State = true;
                } catch {
                    res = "connect error"; //если вдруг что пошло не так
                }
            }
            return res;
        }
        static public int MakeNonQuery(string query)//запрос без результата (возвращает количество изменныех строк)
        {
            int change = 0;
            if (Open() == "ok") {


                commSql.CommandText = query;
                change = commSql.ExecuteNonQuery();
                Close();
            }
            return change;

        }
        static public object ExecuteScalar(string query) //запрос с одним результатом
        {
            Open();
            commSql.CommandText = query;
            object temp = commSql.ExecuteScalar();
            Close();
            return temp;

        }
        static public DataTable GetTable(string query) // запрос, возвращает таблицу с результатом
        {
            Open();
            DataTable table = new DataTable();
            commSql.CommandText = query;
            SqlDataAdapter adapter = new SqlDataAdapter(commSql);
            adapter.Fill(table);
            Close();
            return table;

        }
        public static void Close() //метод закрытия подключения
        {
            connSql.Close();
            State = false;
        }
        public static void GetSettingsFromTxt(string data)// метод получения настроек из строки
        {
            XElement xmlSettings = XElement.Parse(data);
            MsServerName = xmlSettings.Element("MsSqlSettings").Element("MsServerName").Value;
            MsUser = xmlSettings.Element("MsSqlSettings").Element("MsUser").Value;
            MsPass = xmlSettings.Element("MsSqlSettings").Element("MsPass").Value;
          //  MsDataBase = xmlSettings.Element("MsSqlSettings").Element("MsDataBase").Value;
          //  MsIntegratedSecurity = bool.Parse(xmlSettings.Element("MsSqlSettings").Element("MsIntegratedSecurity").Value);

        }
        public static void GetSettingsFromFile(string path) //метод получения настроек из файла
        {
            StreamReader sr = new StreamReader(path);
            string st = sr.ReadToEnd();
            sr.Close();
            GetSettingsFromTxt(st);
        }
    }
}
