using SymOrdinary;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.HRM
{
   public class HomeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
       public List<EmployeeInfoVM> TotalEmployeeGender() {
           {
               #region Variables
               SqlConnection currConn = null;
               string sqlText = "";
               List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
               EmployeeInfoVM VM;
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
select distinct isnull(Gender,'Not Assign')Gender,Count(Id) Person  from ViewEmployeeInformation where IsActive=1
--and Gender is not null 
group by Gender
";
                   SqlCommand objComm = new SqlCommand();
                   objComm.Connection = currConn;
                   objComm.CommandText = sqlText;
                   objComm.CommandType = CommandType.Text;
                   SqlDataReader dr;
                   dr = objComm.ExecuteReader();
                   while (dr.Read())
                   {
                       VM = new EmployeeInfoVM();
                       VM.Gender = dr["Gender"].ToString();
                       VM.Person = Convert.ToInt32(dr["Person"].ToString());
                       VMs.Add(VM);
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
       public DataSet TotalEmployeeSectionGender()
       {
           {
               #region Variables
               SqlConnection currConn = null;
               string sqlText = "";             
               DataSet ds = new DataSet();
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
select distinct isnull(Gender,'N/A') Gender from ViewEmployeeInformation where IsActive=1  order by Gender
select distinct isnull(Section,'N/A') Section from ViewEmployeeInformation where IsActive=1  order by Section
select distinct section,isnull(Gender,'N/A') Gender,Count(Id) Person  from ViewEmployeeInformation where IsActive=1
 group by section,Gender  
 order by Gender";
                   SqlCommand objComm = new SqlCommand();
                   objComm.Connection = currConn;
                   objComm.CommandText = sqlText;
                   objComm.CommandType = CommandType.Text;
                   SqlDataAdapter da = new SqlDataAdapter(objComm);
                   da.Fill(ds);
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
               return ds;
           }
       }
       public decimal TotalEmployeeSectionGender(string section, string gender)
       {
           {
               #region Variables
               SqlConnection currConn = null;
               string sqlText = "";
               decimal result = 0;
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
select distinct section, Gender,Count(Id) Person  from ViewEmployeeInformation where IsActive=1
and Section=@Section and Gender=@Gender
 group by section,Gender  
";
                   SqlCommand objComm = new SqlCommand();
                   objComm.Connection = currConn;
                   objComm.CommandText = sqlText;
                   objComm.Parameters.AddWithValue("@Section", section);
                   objComm.Parameters.AddWithValue("@Gender", gender);
                   objComm.CommandType = CommandType.Text;
                   SqlDataReader dr;
                   dr = objComm.ExecuteReader();
                   while (dr.Read())
                   {
                      
                            result  =Convert.ToDecimal( dr["Person"].ToString());

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

               return result;
           }
       }

       public DataSet TotalEmployeeJoinDate(string FromDate, string ToDate)
       {
           {
               #region Variables
               SqlConnection currConn = null;
               string sqlText = "";
               DataSet ds = new DataSet();
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
                   sqlText = @"select Employeeid, substring(JoinDate,0,5) JoinYear,
substring(JoinDate,5,2) Months,substring(JoinDate,7,2) [Date], * from ViewEmployeeInformation  w
where substring(JoinDate,7,2) between @FromDate and @ToDate";
                   SqlCommand objComm = new SqlCommand();
                   objComm.Connection = currConn;
                   objComm.CommandText = sqlText;
                   objComm.CommandType = CommandType.Text;
                   objComm.Parameters.AddWithValue("@FromDate", FromDate);
                   objComm.Parameters.AddWithValue("@ToDate", ToDate);
                   SqlDataAdapter da = new SqlDataAdapter(objComm);
                   da.Fill(ds);
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
               return ds;
           }
       }
    }
}
