using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class InfoEmployerDAL
    {
         #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<InfoEmployerVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<InfoEmployerVM> VMs = new List<InfoEmployerVM>();
            InfoEmployerVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT 
 Id
,AgentId
,FullName
,LogoName
,PresentDistrictId
,JobCategoryId
,ContactPersonName
,ContactPersonDesignation
,ContactPersonMobile
,ContactPersonEmail
,Website 

from InfoEmployers
    ORDER BY FullName
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new InfoEmployerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AgentId = Convert.ToInt32(dr["AgentId"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.LogoName = dr["LogoName"].ToString();
                    vm.PresentDistrictId = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.JobCategoryId = Convert.ToInt32(dr["JobCategoryId"]);
                    vm.ContactPersonName = dr["ContactPersonName"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonMobile = dr["ContactPersonMobile"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
                    vm.Website  = dr["Website"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }
    }
}
