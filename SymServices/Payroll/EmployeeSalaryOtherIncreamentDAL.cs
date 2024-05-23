using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
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
    public class EmployeeSalaryOtherIncreamentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        public DataTable SelectAll_For_Increment(EmployeeInfoVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            EmployeeInfoDAL _EmployeeInfoDAL = new EmployeeInfoDAL();
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement

                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    vm.MultipleOther3 = vm.MultipleOther3.Trim(',');
                    vm.Other3List = vm.MultipleOther3.Split(',').ToList();
                }

                string[] cFields = { "ve.IsActive", "ve.IsArchive", "ve.Code>", "ve.Code<", "ve.DesignationId", "ve.DepartmentId", "j.Other1", "j.Other2", "ve.ProjectId", "ve.SectionId" };
                string[] cValues = { "1", "0", vm.CodeF, vm.CodeT, vm.DesignationId, vm.DepartmentId, vm.Other1, vm.Other2, vm.ProjectId, vm.SectionId };

  

                dt = _EmployeeInfoDAL.SelectAll_DT(vm, cFields, cValues, currConn, transaction);

                string[] MandatoryFields = { "Increment Date", "Basic", "HouseRent", "Medical", "Conveyance", "Other", "Gross" };

                List<string> MandatoryFieldList = MandatoryFields.OfType<string>().ToList();

                foreach (string item in MandatoryFieldList)
                {
                    DataColumn dc = new DataColumn(item, typeof(string));
                    dc.DefaultValue = "0";
                    dt.Columns.Add(dc);
                }


                #endregion
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #region Catch and Finall
            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion
            #region Finally
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            return dt;
        }

        public string[] Insert_Increament(EmployeeSalaryStructureVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion

            #region Try
            try
            {
                #region Validation
                #endregion Validation

                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Save

                if (vm != null)
                {
                    DataSet ds = new DataSet();
                    ds = Ordinary.ExcelToDataSet(vm.file);

                    DataTable dt = new DataTable();

                    if (ds == null || ds.Tables.Count == 0)
                    {
                        retResults[1] = "Excel File has no Data!";
                        throw new ArgumentNullException("", retResults[1]);
                    }

                    dt = ds.Tables[0];

                    EmployeeStructureDAL _EmployeeStructureDAL = new EmployeeStructureDAL();

                    #region Checkpoint

                    #region Datatable Validations

                    string[] MandatoryFields = { "EmployeeId", "Code", "EmpName", "Increment Date", "Basic", "HouseRent", "Medical", "Conveyance", "Gross" };

                    List<string> MandatoryFieldList = MandatoryFields.OfType<string>().ToList();

                    bool FieldExist = false;
                    DataColumnCollection columns = dt.Columns;
                    foreach (var item in MandatoryFields)
                    {
                        FieldExist = false;
                        if (columns.Contains(item))
                        {
                            FieldExist = true;
                        }
                        if (FieldExist == false)
                        {
                            retResults[1] = item + " is a Mandatory Field!";
                            throw new ArgumentNullException("", retResults[1]);
                        }
                    }
                    #endregion

                    #endregion

                    #region Declarations

                    decimal Gross = 0;
                    decimal Basic = 0;
                    decimal HouseRent = 0;
                    decimal Medical = 0;
                    decimal Conveyance = 0;

                    #endregion


                    foreach (DataRow dr in dt.Rows)
                    {
                        EmployeeSalaryStructureVM insertVM = new EmployeeSalaryStructureVM();
                        #region Value Assign

                        insertVM.EmployeeId = dr["EmployeeId"].ToString();
                        insertVM.IncrementDate = dr["Increment Date"].ToString();

                        insertVM.EmpCode = dr["Code"].ToString();
                        insertVM.EmpName = dr["EmpName"].ToString();

                        insertVM.CreatedAt = vm.CreatedAt;
                        insertVM.CreatedBy = vm.CreatedBy;
                        insertVM.CreatedFrom = vm.CreatedFrom;
                        insertVM.BranchId = vm.BranchId;
                        #endregion

                        #region Checkpoint

                        if (string.IsNullOrWhiteSpace(insertVM.EmployeeId))
                        {
                            retResults[1] = "EmployeeId Can't be Blank or Null!" + insertVM.EmpCode + "-" + insertVM.EmpName;
                            throw new ArgumentNullException("", retResults[1]);
                        }

                        if (string.IsNullOrWhiteSpace(insertVM.IncrementDate))
                        {
                            retResults[1] = "Increment Date Can't be Blank or Null!" + insertVM.EmpCode + "-" + insertVM.EmpName;
                            throw new ArgumentNullException("", retResults[1]);
                        }
                        else
                        {
                            bool DateOkay = Ordinary.IsDate(insertVM.IncrementDate);

                            if (!DateOkay)
                            {
                                retResults[1] = "Increment Date Format Incorrect! " + insertVM.IncrementDate + " " + insertVM.EmpCode + "-" + insertVM.EmpName;
                                throw new ArgumentNullException("", retResults[1]);
                            }
                        }

                        insertVM.IncrementDate = Convert.ToDateTime(insertVM.IncrementDate).ToString("dd-MMM-yyyy");


                        #endregion

                        List<EmployeeSalaryStructureVM> VMs = new List<EmployeeSalaryStructureVM>();

                        VMs = _EmployeeStructureDAL.SelectEmployeeSalaryStructureDetailAll(insertVM.EmployeeId, currConn, transaction);

                        if (VMs == null || VMs.Count == 0)
                        {
                            retResults[1] = "This Employee has no Salary Structure Assigned! " + insertVM.EmpCode + "-" + insertVM.EmpName;
                            throw new ArgumentNullException("", retResults[1]);
                        }

                        #region Gross Set

                        Gross = 0;
                        Basic = 0;
                        HouseRent = 0;
                        Medical = 0;
                        Conveyance = 0;



                        Basic = Convert.ToDecimal(dr["Basic"]);
                        HouseRent = Convert.ToDecimal(dr["HouseRent"]);
                        Medical = Convert.ToDecimal(dr["Medical"]);
                        Conveyance = Convert.ToDecimal(dr["Conveyance"]);

                        Gross = Basic + HouseRent + Medical + Conveyance;
                        dr["Gross"] = Gross;


                        #endregion


                        foreach (var item in VMs)
                        {
                            item.IncrementValue = Convert.ToInt32(dr[item.SalaryType]);
                        }




                        retResults = InsertEmployeeSalaryStructure(VMs, insertVM, currConn, transaction);

                        if (retResults[0] == "Fail")
                        {
                            throw new ArgumentNullException("", retResults[1]);
                        }


                    }

                    #region Comments


                    //////VMs.FirstOrDefault(x => x.SalaryType == "Basic").IncrementValue = Convert.ToInt32(dr["Basic"]);
                    //////VMs.FirstOrDefault(x => x.SalaryType == "HouseRent").IncrementValue = Convert.ToInt32(dr["HouseRent"]);
                    //////VMs.FirstOrDefault(x => x.SalaryType == "Medical").IncrementValue = Convert.ToInt32(dr["Medical"]);
                    //////VMs.FirstOrDefault(x => x.SalaryType == "Conveyance").IncrementValue = Convert.ToInt32(dr["Conveyance"]);
                    //////VMs.FirstOrDefault(x => x.SalaryType == "Other").IncrementValue = Convert.ToInt32(dr["Other"]);
                    //////VMs.FirstOrDefault(x => x.SalaryType == "Gross").IncrementValue = Convert.ToInt32(dr["Gross"]);

                    //////foreach (DataColumn column in dt.Columns)
                    //////{
                    //////    string[] results = Array.FindAll(arr, s => s.Equals(column.ColumnName));

                    //////    if (results == null || results.Length == 0)
                    //////    {
                    //////        continue;
                    //////    }

                    //////    ////if (column.ColumnName == "EmployeeId" || column.ColumnName == "Increment Date" || column.ColumnName == "Employee Name"
                    //////    ////    || column.ColumnName == "Code")
                    //////    ////{
                    //////    ////    continue;
                    //////    ////}


                    //////    EmployeeSalaryStructureVM varVM = new EmployeeSalaryStructureVM();
                    //////    varVM = employeeSalaryStructureVMs.FirstOrDefault(x => x.SalaryType == column.ColumnName);


                    //////    if (varVM != null)
                    //////    {
                    //////        varVM.IncrementValue = string.IsNullOrEmpty(dr[column.ColumnName].ToString()) ? 0 : Convert.ToDecimal(dr[column.ColumnName]);
                    //////        //////varVM.IncrementDate = string.IsNullOrEmpty(row["Increment Date"].ToString()) ? null : row["Increment Date"].ToString();
                    //////        VMs.Add(varVM);

                    //////    }


                    ////}

                    #endregion


                }
                else
                {
                    retResults[1] = "This EmployeeSalaryStructure already used!";
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        ////==================InsertEmployeeSalaryStructure =================
        public string[] InsertEmployeeSalaryIncrementMatrix(EmployeeSalaryStructureVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save


                if (true)
                {
                    List<EmployeeInfoVM> evms = new List<EmployeeInfoVM>();
                    evms =new  EmployeeInfoDAL().SelectAll(null, new[] { "IsActive", "IsArchive" }, new[] { "1", "0" },currConn,transaction);
                    EmployeeSalaryStructureVM essVm = new EmployeeSalaryStructureVM();
                    List<EmployeeSalaryStructureVM> essVms = new List<EmployeeSalaryStructureVM>();

                    foreach (EmployeeInfoVM evm in evms)
                    {
                        if (evm.EmployeeId == "1_9")
                        {
                            
                        }
                        essVms = new List<EmployeeSalaryStructureVM>();
                        essVms = new EmployeeStructureDAL().SelectEmployeeSalaryStructureDetailAll(evm.Id,currConn,transaction);

                        foreach (EmployeeSalaryStructureVM essvm in essVms)
                        {
                            SalaryStructureMatrixVM ssmMV = new SalaryStructureMatrixDAL().StructureMatrix(evm.GradeId, evm.StepSL, vm.CurrentYear
                                , essvm.SalaryType,currConn,transaction);
                            if (essvm.SalaryType=="Basic" || essvm.SalaryType=="HouseRent" || essvm.SalaryType=="Medical" || essvm.SalaryType=="Conveyance" )
                            {
                                essvm.IncrementValue = ssmMV.CurrentAmount == 0 ? 0 : ssmMV.CurrentAmount - essvm.TotalValue;
                                
                            }
                            //essvm.TotalValue =ssmMV.CurrentAmount == 0 ?essvm.TotalValue: ssmMV.CurrentAmount;
                        }

                        //find StructureValue from Salary Matrix;

                        essVm = new EmployeeSalaryStructureVM();
                        essVm = vm;
                        essVm.EmployeeId = evm.Id;
                        essVm.BranchId = evm.BranchId.ToString();

                        retResults= InsertEmployeeSalaryStructure(essVms, vm, currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            retResults[1] = "EmployeeSalaryIncrementMatrix Fail.";
                            throw new ArgumentNullException(retResults[1], "");
                        }

                    }
                   


                }
                else
                {
                    retResults[1] = "This EmployeeSalaryStructure already used!";
                    throw new ArgumentNullException("Please Input EmployeeSalaryStructure Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public string[] InsertEmployeeSalaryStructure(List<EmployeeSalaryStructureVM> VMs, EmployeeSalaryStructureVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save


                if (vm != null)
                {

                    //EmployeeInfoVM evms = new EmployeeInfoVM();
                    //evms = new EmployeeInfoDAL().SelectAll(null, new[] { "EmployeeId" }, new[] { vm.EmployeeId }, currConn, transaction).FirstOrDefault();
 

                    #region Increment Date Check
                    string[] cFields = { "EmployeeId", "IsCurrent" };
                    string[] cValues = { vm.EmployeeId, "1" };

                    EmployeeSalaryStructureVM lastVM = new EmployeeSalaryStructureVM();
                    lastVM = new EmployeeStructureDAL().SelectAll("", cFields, cValues, currConn, transaction).FirstOrDefault();
                    //if (lastVM != null && Convert.ToDateTime(vm.IncrementDate) <= Convert.ToDateTime(lastVM.IncrementDate))
                    //{
                    //    retResults[1] = "Increment Date " + vm.IncrementDate + " can't be earlier than Last Increment Date " + lastVM.IncrementDate+" "+ vm.EmpCode + "-" + vm.EmpName;
                    //    throw new ArgumentNullException(retResults[1], "");
                    //}

                    #endregion

                    #region Update EmployeeSalaryStructure & EmployeeSalaryStructureDetail IsCurrent=0
                    sqlText = " ";
                    sqlText += @"

update EmployeePromotion set GradeId=@GradeId , StepId=(select Id from EnumSalaryStep where SL=@StepSL)
from EnumSalaryStep
where EmployeeId=@EmployeeId

UPDATE EmployeeSalaryStructure set IsCurrent = 0
WHERE EmployeeId = @EmployeeId

UPDATE EmployeeSalaryStructureDetail set IsCurrent = 0
WHERE EmployeeId = @EmployeeId


";
                    SqlCommand cmdIsCurrent = new SqlCommand(sqlText, currConn, transaction);
                    cmdIsCurrent.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdIsCurrent.Parameters.AddWithValue("@GradeId", VMs[0].GradeId);
                    cmdIsCurrent.Parameters.AddWithValue("@StepSL", VMs[0].StepSL);
                    var exeResIsCurrent = cmdIsCurrent.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeResIsCurrent);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EmployeeSalaryStructure!", "");
                    }
                    #endregion Update EmployeeSalaryStructure & EmployeeSalaryStructureDetail IsCurrent=0

                    #region Assign TotalValue, SalaryStructureId
                    foreach (var item in VMs)
                    {
                        if (item.SalaryType != "Gross")
                        {
                            vm.TotalValue += item.IncrementValue;
                            //break;
                        }
                    }


                    EmployeeStructureDAL _employeeStructureDAL = new EmployeeStructureDAL();
                    string[] conditionFields = { "EmployeeId" };
                    string[] conditionValues = { vm.EmployeeId };
                    vm.SalaryStructureId = _employeeStructureDAL.SelectAll("", conditionFields, conditionValues, currConn, transaction).FirstOrDefault().SalaryStructureId;
                    #endregion Assign TotalValue, SalaryStructureId


                    vm.Id = _cDal.NextId("EmployeeSalaryStructure", currConn, transaction);
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeSalaryStructure(
Id,EmployeeId,SalaryStructureId,TotalValue,IncrementDate,BranchId,IsCurrent

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id,@EmployeeId,@SalaryStructureId,@TotalValue,@IncrementDate,@BranchId,@IsCurrent
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    //SalaryStructureId
                    //TotalValue
                    //IncrementDate
                    //BranchId
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@SalaryStructureId", vm.SalaryStructureId);//To Find
                    cmdInsert.Parameters.AddWithValue("@TotalValue", vm.TotalValue);
                    cmdInsert.Parameters.AddWithValue("@IncrementDate", Ordinary.DateToString(vm.IncrementDate));
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EmployeeSalaryStructure.", "");
                    }
                    #endregion SqlExecution

                    #region insert Details from Master into Detail Table
                    if (VMs != null && VMs.Count > 0)
                    {
                        foreach (var empSalaryStructureVM in VMs)
                        {
                            EmployeeSalaryStructureVM dVM = new EmployeeSalaryStructureVM();
                            dVM = empSalaryStructureVM;
                            string EmployeeSalaryStructureId = vm.Id.ToString();

                            dVM.BranchId = vm.BranchId;
                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            dVM.IncrementDate = vm.IncrementDate;
                            //if (dVM.IncrementValue>0)
                            //{
                                retResults = InsertEmployeeSalaryStructureDetail(dVM, EmployeeSalaryStructureId, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            //}
                           
                        }
                    }
                    #endregion insert Details from Master into Detail Table

                    #region Update Gross & Basic in EmployeeJob
                    sqlText = " ";
                    sqlText += @"
UPDATE EmployeeJob set GrossSalary = GB.Gross, BasicSalary = GB.Basic
From ( 
SELECT sum(esd.Gross) Gross, sum(esd.[Basic]) [Basic]
From (
SELECT case when SalaryType = 'Gross' then Amount else 0 end as Gross 
,case when SalaryType = 'Basic' then Amount else 0 end as [Basic] 
from EmployeeSalaryStructureDetail
Where EmployeeId=@EmployeeId
and SalaryType in ('Gross', 'Basic')
) as esd
) as GB

WHERE EmployeeId = @EmployeeId";
                    SqlCommand cmdEmployeeJob = new SqlCommand(sqlText, currConn, transaction);
                    cmdEmployeeJob.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    exeRes = cmdEmployeeJob.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EmployeeJob.", "");
                    }
                    #endregion Update Gross & Basic in EmployeeJob
                }
                else
                {
                    retResults[1] = "This EmployeeSalaryStructure already used!";
                    throw new ArgumentNullException("Please Input EmployeeSalaryStructure Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        //==================Insert =================
        public string[] InsertEmployeeSalaryStructureDetail(EmployeeSalaryStructureVM vm, string EmployeeSalaryStructureId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
                #endregion Validation
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Save


                vm.Id = _cDal.NextId("EmployeeSalaryStructureDetail", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EmployeeSalaryStructureDetail(
Id,EmployeeSalaryStructureId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,IsEarning,Portion,PortionSalaryType,Amount,IncrementDate,IsCurrent
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id,@EmployeeSalaryStructureId,@SalaryTypeId,@SalaryType,@EmployeeId,@IsFixed,@IsEarning,@Portion,@PortionSalaryType,@Amount,@IncrementDate,@IsCurrent
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", EmployeeSalaryStructureId);
                    cmdInsert.Parameters.AddWithValue("@SalaryTypeId", vm.SalaryTypeId);
                    cmdInsert.Parameters.AddWithValue("@SalaryType", vm.SalaryType);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", vm.IsFixed);
                    cmdInsert.Parameters.AddWithValue("@IsEarning", vm.IsEarning);
                    if (vm.IsFixed)
                    {
                        cmdInsert.Parameters.AddWithValue("@Portion", vm.IncrementValue);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@Portion", vm.Portion);
                    }
                    cmdInsert.Parameters.AddWithValue("@PortionSalaryType", vm.PortionSalaryType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Amount", vm.IncrementValue);

                    cmdInsert.Parameters.AddWithValue("@IncrementDate", Ordinary.DateToString(vm.IncrementDate));
                    cmdInsert.Parameters.AddWithValue("@IsCurrent", true);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update BankBranchs.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This BankBranch already used!";
                    throw new ArgumentNullException("Please Input BankBranch Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }


    }
}
