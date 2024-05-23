using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public class DBSQLConnection
    {
        public DBSQLConnection()
        {


        }
       //HRMDB
       //TAXDB
       //PFDB
       //GFDB

        public string HRMDB = new AppSettingsReader().GetValue("HRMDB", typeof(string)).ToString();
        public string TAXDB = new AppSettingsReader().GetValue("TAXDB", typeof(string)).ToString();
        public string PFDB = new AppSettingsReader().GetValue("PFDB", typeof(string)).ToString();
        //public string GFDB = new AppSettingsReader().GetValue("GFDB", typeof(string)).ToString();
        //public string SAGEGLDB = new AppSettingsReader().GetValue("SAGEGLDB", typeof(string)).ToString();
        public string SAGEDB = new AppSettingsReader().GetValue("SAGEDB", typeof(string)).ToString();
        //public string GDICCommissionBillDB = new AppSettingsReader().GetValue("GDICCommissionBillDB", typeof(string)).ToString();

     
        public SqlConnection GetConnectionNoPooling()
        {
            //tt++;
            //Debug.WriteLine(tt.ToString());
            //string ConnectionString = "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
            string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
                + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600; pooling=no;";

            //string ConnectionString = "Data Source=120;Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + DatabaseInfoVM.dbUserName
            //    + ";password=" + DatabaseInfoVM.dbPassword + ";connect Timeout=600;";
            // string conn = System.Configuration.ConfigurationSettings.AppSettings["VATDB"];
            //string conn = System.Configuration.ConfigurationManager.AppSettings["VATDB"].ToString();

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionNoPool()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsNoPool", typeof(string)).ToString();

            //SysDBInfoVM.SysdataSource = ".";
            //DatabaseInfoVM.DatabaseName = "NTDLDT";
            //SysDBInfoVM.SysUserName = "sa";
            //SysDBInfoVM.SysPassword = "S123456_";
            //string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName.Replace("[", "").Replace("]", "") + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";
            //  string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionNoPoolMachine()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsNoPoolMachine", typeof(string)).ToString();

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnection()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStrings", typeof(string)).ToString();

            //SysDBInfoVM.SysdataSource = ".";
            //DatabaseInfoVM.DatabaseName = "NTDLDT";
            //SysDBInfoVM.SysUserName = "sa";
            //SysDBInfoVM.SysPassword = "S123456_";
            //SysDBInfoVM.DatabaseName = new AppSettingsReader().GetValue("HRMDB", typeof(string)).ToString();
            //string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + SysDBInfoVM.DatabaseName.Replace("[", "").Replace("]", "") + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";
            ////  string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionAcc()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsAcc", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        public SqlConnection GetConnectionTax()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsTax", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        public SqlConnection GetConnectionPF()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsPF", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }


       //public SqlConnection GetConnectionGF()
       // {
       //     string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsGF", typeof(string)).ToString();
       //     SqlConnection conn = new SqlConnection(ConnectionString);
       //     return conn;
       // }
       public SqlConnection GetConnectionSageGL()
       {
           string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsSAGEGL", typeof(string)).ToString();
           SqlConnection conn = new SqlConnection(ConnectionString);
           return conn;
       }

       public SqlConnection GetConnectionSageGL_PC()
       {
           string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsSAGEGL_PC", typeof(string)).ToString();
           SqlConnection conn = new SqlConnection(ConnectionString);
           return conn;
       }


       public SqlConnection GetConnectionSageGL_BDE()
       {
           string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsSAGEGL_BDE", typeof(string)).ToString();
           SqlConnection conn = new SqlConnection(ConnectionString);
           return conn;
       }


       public SqlConnection GetConnectionGDICCommissionBill()
       {
           string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsGDICCommissionBill", typeof(string)).ToString();
           SqlConnection conn = new SqlConnection(ConnectionString);
           return conn;
       }


       
       public SqlConnection GetConnectionToDo()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsToDo", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        public SqlConnection GetConnectionSage()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsSAGE", typeof(string)).ToString();

            //SysDBInfoVM.SysdataSource = ".";
            //DatabaseInfoVM.DatabaseName = "NTDLDT";
            //SysDBInfoVM.SysUserName = "sa";
            //SysDBInfoVM.SysPassword = "S123456_";
            //string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName.Replace("[", "").Replace("]", "") + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";
            //  string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionNoTimeOut()
        {
            //tt++;
            //Debug.WriteLine(tt.ToString());
            //string ConnectionString = "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
            string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
                + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=60000; pooling=no;";

            //string ConnectionString = "Data Source=120;Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + DatabaseInfoVM.dbUserName
            //    + ";password=" + DatabaseInfoVM.dbPassword + ";connect Timeout=600;";
            // string conn = System.Configuration.ConfigurationSettings.AppSettings["VATDB"];
            //string conn = System.Configuration.ConfigurationManager.AppSettings["VATDB"].ToString();

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionSys()
        {
            string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";" +
                                      "Initial Catalog=SymphonyVATSys;" +
                //"Initial Catalog=" + SysDBInfoVM.SysDatabaseName + ";" +
                                      "user id=" + SysDBInfoVM.SysUserName + ";" +
                                      "password=" + SysDBInfoVM.SysPassword + ";" +
                                      "connect Timeout=60;";
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionMaster()
        {

            //string ConnectionString = "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
            //string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + SysDBInfoVM.SysDatabaseName + ";" +
            //                          "user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";

            string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=master; user id=" + SysDBInfoVM.SysUserName + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=60;";
            // string conn = System.Configuration.ConfigurationSettings.AppSettings["VATDB"];
            //string conn = System.Configuration.ConfigurationManager.AppSettings["VATDB"].ToString();

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
    }
}
