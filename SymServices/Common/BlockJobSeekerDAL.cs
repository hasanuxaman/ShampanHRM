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
    public class BlockJobSeekerDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        //==================SelectAll=================
        public List<BlockJobSeekerVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BlockJobSeekerVM> VMs = new List<BlockJobSeekerVM>();
            BlockJobSeekerVM vm;
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
select * from(
select
 be.Id
,e.FullName
,e.PhotoName
,e.CVName
,e.PresentDistrictId
,e.Gender
,e.DOB
,e.JobCategoryId
,e.JobLevel
,e.JobNature
,e.CareerObjective
,e.LastSchool
,e.LastDegree
,e.EducationSubject
,e.LastCompany
,e.ExperianceYear
,e.Mobile
,e.Email
,e.Website
,'Y' BlockStatus

 from BlockJobSeekers be 
 left outer join InfoJobSeekers e on be.JobSeekerId=e.id

 union all

 select  e.id Id
,e.FullName
,e.PhotoName
,e.CVName
,e.PresentDistrictId
,e.Gender
,e.DOB
,e.JobCategoryId
,e.JobLevel
,e.JobNature
,e.CareerObjective
,e.LastSchool
,e.LastDegree
,e.EducationSubject
,e.LastCompany
,e.ExperianceYear
,e.Mobile
,e.Email
,e.Website
,'N' BlockStatus

 from InfoJobSeekers e
 ) as a
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BlockJobSeekerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.CVName = dr["CVName"].ToString();
                    vm.PresentDistrictId =  Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.Gender = dr["Gender"].ToString();
                    vm.DOB = Ordinary.StringToDate(dr["DOB"].ToString());
                    vm.JobCategoryId =  Convert.ToInt32(dr["JobCategoryId"]);
                    vm.JobLevel = dr["JobLevel"].ToString();
                    vm.JobNature = dr["JobNature"].ToString();
                    vm.CareerObjective = dr["CareerObjective"].ToString();
                    vm.LastSchool = dr["LastSchool"].ToString();
                    vm.LastDegree = dr["LastDegree"].ToString();
                    vm.EducationSubject = dr["EducationSubject"].ToString();
                    vm.LastCompany = dr["LastCompany"].ToString();
                    vm.ExperianceYear = dr["ExperianceYear"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Website = dr["Website"].ToString();
                    vm.BlockStatus = dr["BlockStatus"].ToString();
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
