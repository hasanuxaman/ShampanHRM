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
    public class EMJobDAL
    {
         #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<EMJobVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EMJobVM> VMs = new List<EMJobVM>();
            EMJobVM vm;
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
,es.Designation
,es.Department
,es.Vacancy
,es.RequiruitmentDate
,es.JobDescription
,e.FullName
,e.PresentDistrictId
,e.JobCategoryId
,e.Website

 from EMJobs es 
left outer join InfoEmployers e on es.EmployerId=e.Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EMJobVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Designation  = dr["Designation"].ToString();
                    vm.Department  = dr["Department"].ToString();
                    vm.Vacancy  = Convert.ToInt32(dr["Vacancy"].ToString());
                    vm.RequiruitmentDate  = Ordinary.StringToDate(dr["RequiruitmentDate"].ToString());
                    vm.JobDescription  = dr["JobDescription"].ToString();
                    vm.FullName  = dr["FullName"].ToString();
                    vm.PresentDistrictId  = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.JobCategoryId  = Convert.ToInt32(dr["JobCategoryId"]);
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
