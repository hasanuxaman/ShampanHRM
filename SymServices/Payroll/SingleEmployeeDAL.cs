using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
  public  class SingleEmployeeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        public List<SingleEmployeeSalaryStructureVM> SingleEmployeeEntry(string EmployeeId="", string FiscalYearDetailId="")
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SingleEmployeeSalaryStructureVM> VMs = new List<SingleEmployeeSalaryStructureVM>();
            SingleEmployeeSalaryStructureVM VM;
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

                #region sql Earning

                sqlText = @"
select  @FiscalYearDetailId FiscalYearDetailId, employeeid,est.IsEarning ,   est.Name, sum(Amount)Value
 from EmployeeSalaryStructureDetail essd left outer join EnumSalaryType est on essd.SalaryTypeId=est.id
where employeeid=@EmployeeId    
and est.TypeName NOT IN ( 'gross')
group by employeeid,est.IsEarning ,  est.Name
 ORDER BY IsEarning DESC,   Name
";
                SqlCommand objComm1 = new SqlCommand();
                objComm1.Connection = currConn;
                objComm1.CommandText = sqlText;
                objComm1.CommandType = CommandType.Text;
                objComm1.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm1.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                SqlDataReader dr1;
                dr1 = objComm1.ExecuteReader();
                if (dr1.HasRows)
                {
                    while (dr1.Read())
                    {
                        VM = new SingleEmployeeSalaryStructureVM();
                        VM.EmployeeId = dr1["EmployeeId"].ToString();
                        VM.FiscalYearDetailId = Convert.ToInt32(dr1["FiscalYearDetailId"]);
                        VM.Name = dr1["Name"].ToString();
                        VM.Value = Convert.ToDecimal(dr1["Value"]);
                        VM.IsEarning = Convert.ToBoolean(dr1["IsEarning"]);
                        VM.IsEditable = false;
                        VMs.Add(VM);
                    }
                }
                else
                {
                    VM = new SingleEmployeeSalaryStructureVM();
                    VM.EmployeeId = EmployeeId;
                    VM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    VM.Name = "Basic";
                    VM.Value = 0;
                    VM.IsEarning = true;
                    VM.IsEditable = false;
                    VMs.Add(VM);

                }
                dr1.Close();

                #endregion sql Earning

                #region sql PFEmloyer

                sqlText = @"
 select @FiscalYearDetailId FiscalYearDetailId, t.EmployeeId,1 IsEarning,   'PFEmloyer' Name
,case when t.IsFixed= 1 then t.PFValue 
when t.PortionSalaryType='basic' then ej.BasicSalary*t.PFValue / 100
when t.PortionSalaryType='gross' then ej.GrossSalary*t.PFValue / 100
end as Value
 from EmployeePF t left outer join EmployeeJob ej on t.EmployeeId=ej.EmployeeId 
 where t.EmployeeId=@EmployeeId
";
                SqlCommand objComm2 = new SqlCommand();
                objComm2.Connection = currConn;
                objComm2.CommandText = sqlText;
                objComm2.CommandType = CommandType.Text;
                objComm2.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm2.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                SqlDataReader dr2;
                dr2 = objComm2.ExecuteReader();
                if (dr2.HasRows)
                {
                    while (dr2.Read())
                    {
                        VM = new SingleEmployeeSalaryStructureVM();
                        VM.EmployeeId = dr2["EmployeeId"].ToString();
                        VM.FiscalYearDetailId = Convert.ToInt32(dr2["FiscalYearDetailId"]);
                        VM.Name = dr2["Name"].ToString();
                        VM.Value = Convert.ToDecimal(dr2["Value"]);
                        VM.IsEarning = Convert.ToBoolean(dr2["IsEarning"]);
                        VM.IsEditable = false;
                        VMs.Add(VM);
                    }
                }
                else
                {
                    VM = new SingleEmployeeSalaryStructureVM();
                    VM.EmployeeId = EmployeeId;
                    VM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    VM.Name = "PFEmloyer";
                    VM.Value = 0;
                    VM.IsEarning = true;
                    VM.IsEditable = false;
                    VMs.Add(VM);

                }
                dr2.Close();

                #endregion sql PFEmloyer

                #region sql PFEmloyee

                sqlText = @"
 select @FiscalYearDetailId FiscalYearDetailId, t.EmployeeId,0 IsEarning,   'PFEmloyee' Name
,case when t.IsFixed= 1 then t.PFValue 
when t.PortionSalaryType='basic' then ej.BasicSalary*t.PFValue / 100
when t.PortionSalaryType='gross' then ej.GrossSalary*t.PFValue / 100
end as Value
 from EmployeePF t left outer join EmployeeJob ej on t.EmployeeId=ej.EmployeeId 
 where t.EmployeeId=@EmployeeId
";
                SqlCommand objComm3 = new SqlCommand();
                objComm3.Connection = currConn;
                objComm3.CommandText = sqlText;
                objComm3.CommandType = CommandType.Text;
                objComm3.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm3.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                SqlDataReader dr3;
                dr3 = objComm3.ExecuteReader();
                if (dr3.HasRows)
                {
                    while (dr3.Read())
                    {
                        VM = new SingleEmployeeSalaryStructureVM();
                        VM.EmployeeId = dr3["EmployeeId"].ToString();
                        VM.FiscalYearDetailId = Convert.ToInt32(dr3["FiscalYearDetailId"]);
                        VM.Name = dr3["Name"].ToString();
                        VM.Value = Convert.ToDecimal(dr3["Value"]);
                        VM.IsEarning = Convert.ToBoolean(dr3["IsEarning"]);
                        VM.IsEditable = false;
                        VMs.Add(VM);
                    }
                }
                else
                {
                    VM = new SingleEmployeeSalaryStructureVM();
                    VM.EmployeeId = EmployeeId;
                    VM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    VM.Name = "PFEmloyee";
                    VM.Value = 0;
                    VM.IsEarning = false;
                    VM.IsEditable = false;
                    VMs.Add(VM);

                }
                dr3.Close();

                #endregion sql PFEmloyee

                #region sql TAX

                sqlText = @"
select @FiscalYearDetailId FiscalYearDetailId,t.EmployeeId,0 IsEarning,   'TAX' Name
,case when t.IsFixed= 1 then t.TaxValue 
when t.PortionSalaryType='basic' then ej.BasicSalary*t.TaxValue / 100
when t.PortionSalaryType='gross' then ej.GrossSalary*t.TaxValue / 100
end as Value
 from EmployeeTax t left outer join EmployeeJob ej on t.EmployeeId=ej.EmployeeId 
 where t.EmployeeId=@EmployeeId
";
                SqlCommand objComm4 = new SqlCommand();
                objComm4.Connection = currConn;
                objComm4.CommandText = sqlText;
                objComm4.CommandType = CommandType.Text;
                objComm4.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm4.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                SqlDataReader dr4;
                dr4 = objComm4.ExecuteReader();
                if (dr4.HasRows)
                {
                    while (dr4.Read())
                    {
                        VM = new SingleEmployeeSalaryStructureVM();
                        VM.EmployeeId = dr4["EmployeeId"].ToString();
                        VM.FiscalYearDetailId = Convert.ToInt32(dr4["FiscalYearDetailId"]);
                        VM.Name = dr4["Name"].ToString();
                        VM.Value = Convert.ToDecimal(dr4["Value"]);
                        VM.IsEarning = Convert.ToBoolean(dr4["IsEarning"]);
                        VM.IsEditable = false;
                        VMs.Add(VM);
                    }
                }
                else
                {
                    VM = new SingleEmployeeSalaryStructureVM();
                    VM.EmployeeId = EmployeeId;
                    VM.FiscalYearDetailId = Convert.ToInt32(FiscalYearDetailId);
                    VM.Name = "TAX";
                    VM.Value = 0;
                    VM.IsEarning = false;
                    VM.IsEditable = false;
                    VMs.Add(VM);

                }
                dr4.Close();

                #endregion sql TAX

                

                

                

                

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

        //public string[] Update(EmployeeInfoVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //{
        //    #region Variables

        //    string[] retResults = new string[6];
        //    retResults[0] = "Fail";//Success or Fail
        //    retResults[1] = "Fail";// Success or Fail Message
        //    retResults[2] = "0";
        //    retResults[3] = "sqlText"; //  SQL Query
        //    retResults[4] = "ex"; //catch ex
        //    retResults[5] = "Employee SalaryStructure Update"; //Method Name

        //    string sqlText = "";

        //    bool iSTransSuccess = false;

        //    #endregion
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {

        //        #region open connection and transaction
        //        #region New open connection and transaction
        //        if (VcurrConn != null)
        //        {
        //            currConn = VcurrConn;
        //        }

        //        if (Vtransaction != null)
        //        {
        //            transaction = Vtransaction;
        //        }

        //        #endregion New open connection and transaction

        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnection();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }

        //        if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSalaryStructure"); }

        //        #endregion open connection and transaction


        //        if (vm != null)
        //        {
        //            FiscalYearDAL fydal = new FiscalYearDAL();
        //            foreach (SingleEmployeeSalaryStructureVM item in vm.SingleEmployeeSalaryStructureVM)
        //            {
        //                string transDate = fydal.FiscalPeriodStartDate(item.FiscalYearDetailId, currConn, transaction);

        //                #region areer
        //                if (item.Name.ToLower() == "areer" && item.Value > 0)
        //                {
        //                    EmployeeAreerVM tempvm = new EmployeeAreerVM();

        //                    EmployeeAreerDAL dal = new EmployeeAreerDAL();
        //                    tempvm.AreerDate = transDate;
        //                    tempvm.EmployeeId = vm.Id;
        //                    tempvm.AreerAmount = item.Value;
        //                    tempvm.FiscalYearDetailId = item.FiscalYearDetailId;
        //                    tempvm.Remarks = item.Remarks;
        //                    tempvm.CreatedBy = vm.CreatedBy;
        //                    tempvm.CreatedAt = vm.CreatedAt;
        //                    tempvm.CreatedFrom = vm.CreatedFrom;
        //                  retResults=  dal.Insert(tempvm, currConn, transaction);
        //                  if (retResults[0].ToLower()=="fail")
        //                  {
        //                         retResults[1] = "Arear not Entred";
        //                         throw new ArgumentNullException("Arear not Entred", "Arear not Entred");
        //                  }

        //                }
        //                #endregion OverTime

        //                #region OverTime
        //                if (item.Name.ToLower() == "overtime" && item.Value > 0)
        //                {
        //                    EmployeeOverTimeVM tempvm = new EmployeeOverTimeVM();

        //                    EmployeeOverTimeDAL dal = new EmployeeOverTimeDAL();
        //                    tempvm.EmployeeId = vm.Id;
        //                    tempvm.OverTimeDate = transDate;
        //                    tempvm.OverTimeAmount = item.Value;
        //                    tempvm.FiscalYearDetailId = item.FiscalYearDetailId;
        //                    tempvm.Remarks = item.Remarks;
        //                    tempvm.CreatedBy = vm.CreatedBy;
        //                    tempvm.CreatedAt = vm.CreatedAt;
        //                    tempvm.CreatedFrom = vm.CreatedFrom;
        //                    retResults = dal.Insert(tempvm, currConn, transaction);
        //                    if (retResults[0].ToLower() == "fail")
        //                    {
        //                        retResults[1] = "OT not Entred";
        //                        throw new ArgumentNullException("OT not Entred", "OT not Entred");
        //                    }
        //                }
        //                #endregion OverTime

        //                #region ReimbursableExpense
        //                if (item.Name.ToLower() == "reimbursable expense" && item.Value > 0)
        //                {
        //                    EmployeeReimbursableExpenseVM tempvm = new EmployeeReimbursableExpenseVM();

        //                    EmployeeReimbursableExpenseDAL dal = new EmployeeReimbursableExpenseDAL();
        //                    tempvm.EmployeeId = vm.Id;
        //                    tempvm.ReimbursableExpenseDate = transDate;
        //                    tempvm.ReimbursableExpenseAmount = item.Value;
        //                    tempvm.FiscalYearDetailId = item.FiscalYearDetailId;
        //                    tempvm.Remarks = item.Remarks;
        //                    tempvm.CreatedBy = vm.CreatedBy;
        //                    tempvm.CreatedAt = vm.CreatedAt;
        //                    tempvm.CreatedFrom = vm.CreatedFrom;
        //                    retResults = dal.Insert(tempvm, currConn, transaction);
        //                    if (retResults[0].ToLower() == "fail")
        //                    {
        //                        retResults[1] = "ReimbursableExpense not Entred";
        //                        throw new ArgumentNullException("ReimbursableExpense not Entred", "ReimbursableExpense not Entred");
        //                    }
        //                }
        //                #endregion ReimbursableExpense

        //                #region deduction
        //                if (item.Name.ToLower() == "deduction" && item.Value > 0)
        //                {
        //                    EmployeeDeductionVM tempvm = new EmployeeDeductionVM();
        //                    EmployeeDeductionDAL dal = new EmployeeDeductionDAL();
        //                    tempvm.DeductionDate = transDate;
        //                    tempvm.EmployeeId = vm.Id;
        //                    tempvm.DeductionAmount = item.Value;
        //                    tempvm.FiscalYearDetailId = item.FiscalYearDetailId;
        //                    tempvm.Remarks = item.Remarks;
        //                    tempvm.CreatedBy = vm.CreatedBy;
        //                    tempvm.CreatedAt = vm.CreatedAt;
        //                    tempvm.CreatedFrom = vm.CreatedFrom;
        //                    retResults = dal.Insert(tempvm, currConn, transaction);
        //                    if (retResults[0].ToLower() == "fail")
        //                    {
        //                        retResults[1] = "deduction not Entred";
        //                        throw new ArgumentNullException("deduction not Entred", "deduction not Entred");
        //                    }
        //                }
        //                #endregion deduction

        //            }
        //            iSTransSuccess = true;
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("SalaryStructure Update", "Could not found any item.");
        //        }


        //        if (iSTransSuccess == true)
        //        {

        //            if (Vtransaction == null)
        //            {
        //                if (transaction != null)
        //                {
        //                    transaction.Commit();
        //                }
        //            }

        //            retResults[0] = "Success";
        //            retResults[1] = "Data Update Successfully.";

        //        }
        //        else
        //        {
        //            retResults[1] = "Unexpected error to update SalaryStructure.";
        //            throw new ArgumentNullException("", "");
        //        }

        //    }
        //    #region catch
        //    catch (Exception ex)
        //    {
        //        retResults[0] = "Fail";//Success or Fail
        //        retResults[4] = ex.Message; //catch ex
        //        if (Vtransaction == null) { transaction.Rollback(); }
        //        return retResults;
        //    }
        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {
        //                currConn.Close();
        //            }
        //        }
        //    }

        //    #endregion

        //    return retResults;
        //}

  }
}
