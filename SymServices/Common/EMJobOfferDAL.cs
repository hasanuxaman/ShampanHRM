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
    public class EMJobOfferDAL
    {
          #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<EMJobOfferVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EMJobOfferVM> VMs = new List<EMJobOfferVM>();
            EMJobOfferVM vm;
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
 o.Id
,o.JobSeekerId
,o.EmployerId
,o.JobId
,o.OfferType
,j.FullName
,j.PhotoName
,j.PresentDistrictId
,j.Gender
,j.DOB
,j.JobCategoryId
,j.Mobile
,j.Email
,ej.Designation
,ej.Department
,ej.Vacancy
,ej.RequiruitmentDate

 from EMJobOffers o
 left outer join InfoJobSeekers j on o.JobSeekerId=j.Id
 left outer join EMJobs ej on o.JobId=ej.Id
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EMJobOfferVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.JobSeekerId = Convert.ToInt32(dr["JobSeekerId"]);
                    vm.EmployerId = Convert.ToInt32(dr["EmployerId"]);
                    vm.JobId = Convert.ToInt32(dr["JobId"]);
                    vm.OfferType = dr["OfferType"].ToString();
                    vm.FullName = dr["FullName"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.PresentDistrictId = Convert.ToInt32(dr["PresentDistrictId"]);
                    vm.Gender = dr["Gender"].ToString();
                    vm.DOB = Ordinary.StringToDate(dr["DOB"].ToString());
                    vm.JobCategoryId = Convert.ToInt32(dr["JobCategoryId"]);
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Vacancy = dr["Vacancy"].ToString();
                    vm.RequiruitmentDate = Ordinary.StringToDate(dr["RequiruitmentDate"].ToString());
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
