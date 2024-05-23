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
    public class SMSSenderDAL
    {
                  #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<SMSSenderVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SMSSenderVM> VMs = new List<SMSSenderVM>();
            SMSSenderVM vm;
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
 es.Id
,es.SenderMobileNo
,es.RecepentMobileNo
,es.SMSBody
,e.FullName 
,e.PresentDistrictId
,e.JobCategoryId
,e.Website

 from SMSSenders es 
left outer join InfoJobSeekers e on es.RecepentId=e.Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SMSSenderVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.SenderMobileNo = dr["SenderMobileNo"].ToString();
                    vm.RecepentMobileNo = dr["RecepentMobileNo"].ToString();
                    vm.SMSBody = dr["SMSBody"].ToString();
                    vm.FullName  = dr["FullName"].ToString();
                    vm.PresentDistrictId = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.JobCategoryId = Convert.ToInt32(dr["JobCategoryId"]);
                    vm.Website = dr["Website"].ToString();
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
