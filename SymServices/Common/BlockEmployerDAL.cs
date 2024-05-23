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
    public class BlockEmployerDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        //==================SelectAll=================
        public List<BlockEmployerVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BlockEmployerVM> VMs = new List<BlockEmployerVM>();
            BlockEmployerVM vm;
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
select * from (
select 
 be.Id
,e.FullName
,e.LogoName
,e.PresentDistrictId
,e.JobCategoryId
,e.ContactPersonName
,e.ContactPersonDesignation
,e.ContactPersonMobile
,e.ContactPersonEmail
,e.Website 
,'Y' BlockStatus
 from BlockEmployers be 
 left outer join InfoEmployers e on be.EmployerId=e.id

 union all
 select  
 e.id Id
,e.FullName
,e.LogoName
,e.PresentDistrictId
,e.JobCategoryId
,e.ContactPersonName
,e.ContactPersonDesignation
,e.ContactPersonMobile
,e.ContactPersonEmail
,e.Website 
,'N' BlockStatus

 from InfoEmployers e
 where id not in(
 select distinct EmployerId
 from BlockEmployers
 )
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
                    vm = new BlockEmployerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.LogoName = dr["LogoName"].ToString();
                    vm.PresentDistrictId = dr["PresentDistrictId"].ToString();
                    vm.JobCategoryId = Convert.ToInt32(dr["JobCategoryId"]);
                    vm.ContactPersonName = dr["ContactPersonName"].ToString();
                    vm.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vm.ContactPersonMobile = dr["ContactPersonMobile"].ToString();
                    vm.ContactPersonEmail = dr["ContactPersonEmail"].ToString();
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
