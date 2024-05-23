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
    public class EmailSenderDAL
    {
                  #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
         //==================SelectAll=================
        public List<EmailSenderVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmailSenderVM> VMs = new List<EmailSenderVM>();
            EmailSenderVM vm;
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
SELECT * FROM (
select 
es.Id
,es.SenderEmail
,es.RecepentEmail
,es.EmailSubject
,es.IsAttachmentCV
,e.FullName RecipientName
,e.PresentDistrictId
,e.JobCategoryId
,e.Website

 from EmailSenders es 
left outer join InfoEmployers e on es.EmployerId=e.Id
where es.IsSendEmployer=0
union all
select 
es.Id
,es.SenderEmail
,es.RecepentEmail
,es.EmailSubject
,es.IsAttachmentCV
,e.FullName RecipientName
,e.PresentDistrictId
,e.JobCategoryId
,e.Website
 from EmailSenders es 
left outer join InfoJobSeekers e on es.JobSeekerId=e.Id
where es.IsSendEmployer=1
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
                    vm = new EmailSenderVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.SenderEmail = dr["SenderEmail"].ToString();
                    vm.RecepentEmail = dr["RecepentEmail"].ToString();
                    vm.EmailSubject = dr["EmailSubject"].ToString();
                    vm.IsAttachmentCV = Convert.ToBoolean(dr["IsAttachmentCV"].ToString());
                    vm.RecipientName = dr["RecipientName"].ToString();
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
