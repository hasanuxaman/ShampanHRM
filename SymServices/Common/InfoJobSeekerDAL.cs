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
    public class InfoJobSeekerDAL
    {
         #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<InfoJobSeekerVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<InfoJobSeekerVM> VMs = new List<InfoJobSeekerVM>();
            InfoJobSeekerVM vm;
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
,PhotoName
,CVName
,PresentDistrictId
,Gender
,DOB
,JobCategoryId
,JobLevel
,JobNature
,CareerObjective
,LastSchool
,LastDegree
,EducationSubject
,LastCompany
,ExperianceYear
,Mobile
,Email
,Speciality
,Website
from InfoJobSeekers
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
                    vm = new InfoJobSeekerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AgentId = Convert.ToInt32(dr["AgentId"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.CVName = dr["CVName"].ToString();
                    vm.PresentDistrictId = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.Gender = dr["Gender"].ToString();
                    vm.DOB = Ordinary.StringToDate(dr["DOB"].ToString());
                    vm.JobCategoryId = Convert.ToInt32(dr["JobCategoryId"]);
                    vm.JobLevel = dr["JobLevel"].ToString();
                    vm.JobNature = dr["JobNature"].ToString();
                    vm.CareerObjective = dr["CareerObjective"].ToString();
                    vm.LastSchool = dr["LastSchool"].ToString();
                    vm.LastDegree = dr["LastDegree"].ToString();
                    vm.EducationSubject = dr["EducationSubject"].ToString();
                    vm.LastCompany = dr["LastCompany"].ToString();
                    vm.ExperianceYear =dr["ExperianceYear"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Speciality = dr["Speciality"].ToString();
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
