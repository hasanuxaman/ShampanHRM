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
    public class InfoAgentDAL
    {
         #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<InfoAgentVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<InfoAgentVM> VMs = new List<InfoAgentVM>();
            InfoAgentVM vm;
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
,FullName
,PhotoName
,PresentDistrictId
,MobileBankType
,MobileBankNo
,MobileNo
,Email
,CompanyName
,Gender
,DOB

from InfoAgents
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
                    vm = new InfoAgentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.PresentDistrictId = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.MobileBankType = dr["MobileBankType"].ToString();
                    vm.MobileBankNo = dr["MobileBankNo"].ToString();
                    vm.MobileNo = dr["MobileNo"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.CompanyName = dr["CompanyName"].ToString();
                    vm.Gender = dr["Gender"].ToString();
                    vm.DOB = Ordinary.StringToDate(dr["DOB"].ToString());
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
