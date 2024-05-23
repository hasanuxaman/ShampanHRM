using SymOrdinary;
using SymServices.Common;
using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
    public class EmployeeSalaryIncrementDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        EmployeeInfoDAL empdal = new EmployeeInfoDAL();
        #endregion

        //==================Insert =================
    
        public string[] InsertIncreament(List<string> empids,int BranchId,string IncreamentDate, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string GB, string FR, decimal Amount,string LastUpdateAt,string LastUpdateBy,string LastUpdateFrom, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBonusProcess"; //Method Name
            DataSet dsOld1 = new DataSet();
            DataSet dsOld2 = new DataSet();
            DataSet dsOld3 = new DataSet();
            DataSet dsOld4 = new DataSet(); 

            string stepName = "";
            string SalaryStructureId = "";
            decimal GrossSalary = 0;
            decimal SalaryAmount = 0;
            decimal MBasic = 0;
            decimal MMedical = 0;
            decimal MConveyance = 0;
            decimal Portion = 0;
            int EmployeeSalaryStructureNewId = 0;
            string EmployeeJoiningDate = "";
            string EmployeeJobId = "0";
            bool IsFixed = false;
            decimal BasicSalary = 0;
           EmployeeInfoVM empvm = new EmployeeInfoVM();
            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try
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

                //GetEmployeeInfo
                foreach (var a in empids) {
                    if (a == null)
                        break;
                    empvm = empdal.SelectByIdAll(a);
               
                    SettingDAL _setDAL = new SettingDAL();
                    if (GB == "True")
                    {
                        MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
                        sc.Language = "VBScript";

                        var basicCalc = _setDAL.settingValue("Salary", "BasicCalc", currConn, transaction);
                        if (FR == "True")
                        {

                            basicCalc = basicCalc.Replace("vGross", (empvm.GrossSalary + empvm.GrossSalary*Amount/100).ToString());
                        }
                        else {
                            basicCalc = basicCalc.Replace("vGross", (empvm.GrossSalary + Amount).ToString());
                        }
                   
                        var res = sc.Eval(basicCalc);
                        BasicSalary = Convert.ToDecimal(res);
                    }
                    else
                    {
                        if (FR == "True")
                        {

                            BasicSalary = empvm.BasicSalary + empvm.BasicSalary * Amount / 100;
                        }
                        else
                        {
                            BasicSalary = empvm.BasicSalary + Amount;
                        }
                       
                    }
                    DataTable dteJob = new DataTable();
                    DataTable dtePromossion = new DataTable();
                    DataTable dtJobMatrix = new DataTable();
                    DataSet dsSS = new DataSet();
                    #region Job Check
                    sqlText = "";
                    sqlText += @" SELECT *  From EmployeeJob 
                                 where  EmployeeId=@EmployeeId  ";
                    SqlCommand objCommejob = new SqlCommand();
                    objCommejob.Connection = currConn;
                    objCommejob.CommandText = sqlText;
                    objCommejob.CommandType = CommandType.Text;
                    objCommejob.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                    objCommejob.Transaction = transaction;
                    SqlDataAdapter daeJob = new SqlDataAdapter(objCommejob);
                    daeJob.Fill(dteJob);
                    if (dteJob == null || dteJob.Rows.Count <= 0)
                    {
                        retResults[1] = empvm.EmpName + " Have not Assign In JOB, \nPlease Update the JOB First";
                        throw new ArgumentNullException(empvm.EmpName + "This Employee Have In Assign the JOB, \nPlease Update the JOB First", "");
                    }
                    else
                    {
                        EmployeeJoiningDate = dteJob.Rows[0]["JoinDate"].ToString();
                        EmployeeJobId = dteJob.Rows[0]["Id"].ToString();
                    }
                    #endregion Job Check

                    #region Matrix Check
                    stepName = "step" + empvm.StepSL + "Amount";
                    sqlText = "";
                    sqlText += @" select max(Basic)Basic,max(Medical)Medical,max(Conveyance)Conveyance from ( ";

                    sqlText += @" select  " + stepName + "  'Basic',0 'Medical',0 'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='basic'";
                    sqlText += @" union all select 0 'Basic',  " + stepName + "   'Medical',0 'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='medical'";
                    sqlText += @" union all select 0 'Basic',0 'Medical',  " + stepName + "   'Conveyance' from SalaryStructureMatrix where GradeId=@GradeId and salaryTypeName='conveyance') as a ";

                    SqlCommand objCommMatrix = new SqlCommand();
                    objCommMatrix.Connection = currConn;
                    objCommMatrix.CommandText = sqlText;
                    objCommMatrix.CommandType = CommandType.Text;
                    objCommMatrix.Parameters.AddWithValue("@GradeId", empvm.GradeId);
                    objCommMatrix.Transaction = transaction;

                    SqlDataAdapter dajobMatrix = new SqlDataAdapter(objCommMatrix);
                    dajobMatrix.Fill(dtJobMatrix);

                    if (dtJobMatrix == null || dtJobMatrix.Rows.Count <= 0)
                    {
                        retResults[1] = empvm.EmpName + " Have not Assign In  Matrix, \nPlease Update the Matrix First";
                        throw new ArgumentNullException(empvm.EmpName + " Have In Assign the Matrix, \nPlease Update the Matrix First", "");
                    }
                    else
                    {
                        MBasic = Convert.ToDecimal(dtJobMatrix.Rows[0]["Basic"]);
                        MMedical = Convert.ToDecimal(dtJobMatrix.Rows[0]["Medical"]);
                        MConveyance = Convert.ToDecimal(dtJobMatrix.Rows[0]["Conveyance"]);
                    }
                    #endregion Matrix Check
                    #region Check Exists
                    sqlText = @"Select count(BranchId) from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId";
                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                    cmd2.Transaction = transaction;
                    var exeRes = cmd2.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);

                    sqlText = @"Select SalaryStructureId from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId";
                    SqlCommand cmd3 = new SqlCommand(sqlText, currConn);

                    cmd3.Parameters.AddWithValue("@EmployeeId", empvm.Id);
                    cmd3.Transaction = transaction;
                   
                    SqlDataReader dr;
                    dr = cmd3.ExecuteReader();
                    while (dr.Read())
                    {

                        SalaryStructureId = dr["SalaryStructureId"].ToString();
                    }
                    dr.Close();


                    #region SG Update

                    sqlText = "Select count(Id) from EmployeeStructureGroup where EmployeeId=@EmployeeId";
                    SqlCommand cmdExists = new SqlCommand(sqlText, currConn);
                    cmdExists.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                    cmdExists.Transaction = transaction;

                    exeRes = cmdExists.ExecuteScalar();
                    int exists = Convert.ToInt32(exeRes);
                    if (exists <= 0)
                    {
                        retResults[1] = "Please Save/Update Employee Job Information";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Save/Update Employee Job Information", "");
                    }


                    #endregion SG Update

                    if (count > 1)
                    {
                        retResults[1] = "This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment";
                        throw new ArgumentNullException("This Employee Already Assign the Salary Structure and can't Update\nYou can Give Increment", "");
                    }
                    else if (count == 1)
                    {
                        #region Update

                        #region SG Update



                        #endregion SG Update

                        #region OldData1

                        dsOld1 = new DataSet();
                        sqlText = "";
                        sqlText += @" 
                select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId)
                and salaryType not in('Gross','Basic')  and IsFixed=0 and IsEarning=1   
                ";

                        SqlCommand objComOld1 = new SqlCommand();
                        objComOld1.Connection = currConn;
                        objComOld1.CommandText = sqlText;
                        objComOld1.CommandType = CommandType.Text;
                        objComOld1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        objComOld1.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        objComOld1.Transaction = transaction;

                        SqlDataAdapter daOld1 = new SqlDataAdapter(objComOld1);
                        daOld1.Fill(dsOld1);
                        #endregion OldData1
                        #region OldData2

                        dsOld2 = new DataSet();
                        sqlText = "";
                        sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId) 
                    and salaryType not in('Gross','Basic')  and IsFixed=1 and IsEarning=1   
                ";

                        SqlCommand objComOld2 = new SqlCommand();
                        objComOld2.Connection = currConn;
                        objComOld2.CommandText = sqlText;
                        objComOld2.CommandType = CommandType.Text;
                        objComOld2.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        objComOld2.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        objComOld2.Transaction = transaction;

                        SqlDataAdapter daOld2 = new SqlDataAdapter(objComOld2);
                        daOld2.Fill(dsOld2);
                        #endregion OldData2
                        #region OldData3

                        dsOld3 = new DataSet();
                        sqlText = "";
                        sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId) 
                    and salaryType not in('Gross','Basic')  and IsFixed=0 and IsEarning=0   
                ";

                        SqlCommand objComOld3 = new SqlCommand();
                        objComOld3.Connection = currConn;
                        objComOld3.CommandText = sqlText;
                        objComOld3.CommandType = CommandType.Text;
                        objComOld3.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        objComOld3.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        objComOld3.Transaction = transaction;

                        SqlDataAdapter daOld3 = new SqlDataAdapter(objComOld3);
                        daOld3.Fill(dsOld3);
                        #endregion OldData3
                        #region OldData4

                        dsOld4 = new DataSet();
                        sqlText = "";
                        sqlText += @" 
                    select * from EmployeeSalaryStructureDetail where employeeid=@EmployeeId and salarytypeid 
                    not in ( select salarytypeid from SalaryStructureDetail where SalaryStructureId=@SalaryStructureId)
                    and salaryType not in('Gross','Basic')  and IsFixed=1 and IsEarning=0   
                ";

                        SqlCommand objComOld4 = new SqlCommand();
                        objComOld4.Connection = currConn;
                        objComOld4.CommandText = sqlText;
                        objComOld4.CommandType = CommandType.Text;
                        objComOld4.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        objComOld4.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        objComOld4.Transaction = transaction;

                        SqlDataAdapter daOld4 = new SqlDataAdapter(objComOld4);
                        daOld4.Fill(dsOld4);
                        #endregion OldData

                        #region Delete EmployeeSalaryStructureDetail
                        sqlText = "  ";
                        sqlText += @" delete from EmployeeSalaryStructureDetail 
                                where EmployeeId=@EmployeeId  ";

                        SqlCommand cmdsgD = new SqlCommand(sqlText, currConn);

                        cmdsgD.Parameters.AddWithValue("@EmployeeId",empvm.Id);

                        cmdsgD.Transaction = transaction;
                        cmdsgD.ExecuteNonQuery();
                        #endregion Delete EmployeeSalaryStructureDetail

                        #region Delete EmployeeSalaryStructure
                        sqlText = "  ";
                        sqlText += @" delete from EmployeeSalaryStructure 
                                where EmployeeId=@EmployeeId  ";

                        SqlCommand cmdsgH = new SqlCommand(sqlText, currConn);

                        cmdsgH.Parameters.AddWithValue("@EmployeeId",empvm.Id);

                        cmdsgH.Transaction = transaction;
                        cmdsgH.ExecuteNonQuery();
                        #endregion Delete EmployeeSalaryStructure

                        #endregion Update
                    }

                    #region  dsGB
                    DataSet dsGB = new DataSet();

                    sqlText = "";
                    sqlText += @" 
                            SELECT *  From EnumSalaryType where  TypeName in('Gross') and  BranchId=@BranchId
                            SELECT *  From EnumSalaryType where  TypeName in('Basic') and  BranchId=@BranchId
";

                    SqlCommand objCommGB = new SqlCommand();
                    objCommGB.Connection = currConn;
                    objCommGB.CommandText = sqlText;
                    objCommGB.CommandType = CommandType.Text;
                    objCommGB.Parameters.AddWithValue("@BranchId", BranchId);
                    objCommGB.Transaction = transaction;

                    SqlDataAdapter daGB = new SqlDataAdapter(objCommGB);
                    daGB.Fill(dsGB);
                    #endregion  dsGB

                    #region Insert


                    #region EmployeeSalaryStructure

                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructure ";
                    SqlCommand cmdEmployeeSalaryStructureId = new SqlCommand(sqlText, currConn);
                    cmdEmployeeSalaryStructureId.Transaction = transaction;
                    exeRes = cmdEmployeeSalaryStructureId.ExecuteScalar();
                    EmployeeSalaryStructureNewId = Convert.ToInt32(exeRes);
                    EmployeeSalaryStructureNewId++;
                    sqlText = "";
                    sqlText += @" SELECT *  From SalaryStructure where  Id=@Id ";

                    SqlCommand objss = new SqlCommand();
                    objss.Connection = currConn;
                    objss.CommandText = sqlText;
                    objss.CommandType = CommandType.Text;
                    objss.Parameters.AddWithValue("@Id", SalaryStructureId);
                    objss.Transaction = transaction;

                    SqlDataAdapter daSS = new SqlDataAdapter(objss);
                    daSS.Fill(dsSS);

                    foreach (var empstitem in dsSS.Tables[0].Rows)
                    {
                        sqlText = "  ";
                        sqlText += @" INSERT INTO EmployeeSalaryStructure(
                                    Id,EmployeeId,SalaryStructureId,TotalValue,IncrementDate,BranchId,Remarks,IsActive,IsArchive
                                    ,CreatedBy,CreatedAt,CreatedFrom
                                    )VALUES (
                                    @Id,@EmployeeId,@SalaryStructureId,@TotalValue,@IncrementDate,@BranchId,@Remarks,@IsActive
                                    ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                                    ) ";

                        SqlCommand cmdInsESSD = new SqlCommand(sqlText, currConn);

                        cmdInsESSD.Parameters.AddWithValue("@Id", EmployeeSalaryStructureNewId);
                        cmdInsESSD.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsESSD.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                        cmdInsESSD.Parameters.AddWithValue("@TotalValue", BasicSalary);
                        cmdInsESSD.Parameters.AddWithValue("@IncrementDate", IncreamentDate);
                        cmdInsESSD.Parameters.AddWithValue("@BranchId", BranchId);
                        cmdInsESSD.Parameters.AddWithValue("@Remarks", Convert.DBNull);
                        cmdInsESSD.Parameters.AddWithValue("@IsActive", true);
                        cmdInsESSD.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsESSD.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsESSD.Transaction = transaction;
                        cmdInsESSD.ExecuteNonQuery();
                        EmployeeSalaryStructureNewId++;
                    }
                    #endregion EmployeeSalaryStructure
                    #region Save
                    #region sql statement
                    sqlText = "Select isnull(max(convert(int, Id)),0) from EmployeeSalaryStructureDetail ";
                    SqlCommand cmdId = new SqlCommand(sqlText, currConn);
                    cmdId.Transaction = transaction;
                    exeRes = cmdId.ExecuteScalar();
                    int NewId = Convert.ToInt32(exeRes);

                    DataSet ds = new DataSet();

                    sqlText = "";
                    sqlText += @" 
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=0 and IsEarning=1 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=1 and IsEarning=1 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=0 and IsEarning=0 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Other') and IsFixed=1 and IsEarning=0 and  SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Medical') and   SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('Conveyance') and   SalaryStructureId=@SalaryStructureId
                SELECT *  From SalaryStructureDetail where  SalaryType in('HouseRent') and   SalaryStructureId=@SalaryStructureId
                ";

                    SqlCommand objCommssd1 = new SqlCommand();
                    objCommssd1.Connection = currConn;
                    objCommssd1.CommandText = sqlText;
                    objCommssd1.CommandType = CommandType.Text;
                    objCommssd1.Parameters.AddWithValue("@SalaryStructureId", SalaryStructureId);
                    objCommssd1.Transaction = transaction;

                    SqlDataAdapter da = new SqlDataAdapter(objCommssd1);
                    da.Fill(ds);


                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeSalaryStructureDetail(
                                            Id,EmployeeSalaryStructureId,SalaryTypeId,SalaryType,EmployeeId,IsFixed,IsEarning,Portion,PortionSalaryType,Amount,Remarks,IsActive,IsArchive
                                            ,CreatedBy,CreatedAt,CreatedFrom 
                                            )VALUES (
                                            @Id,@EmployeeSalaryStructureId,@SalaryTypeId,@SalaryType,@EmployeeId,@IsFixed,@IsEarning,@Portion,@PortionSalaryType,@Amount,@Remarks,@IsActive
                                            ,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom ) 
                                            ";
                    #endregion  sql statement
                    GrossSalary = BasicSalary;
                    #region drNotFixedEarning
                    foreach (DataRow drNotFixedEarning in ds.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        GrossSalary = GrossSalary + SalaryAmount;

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drNotFixedEarning

                    #region drFixedEarning
                    foreach (DataRow drFixedEarning in ds.Tables[1].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drFixedEarning["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(Portion);
                        GrossSalary = GrossSalary + SalaryAmount;


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drFixedEarning

                    #region drNotFixedDeduction
                    foreach (DataRow drNotFixedDeduction in ds.Tables[2].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drNotFixedDeduction["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedDeduction["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedDeduction["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedDeduction["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drNotFixedDeduction

                    #region drFixedDeduction
                    foreach (DataRow drFixedDeduction in ds.Tables[3].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = Convert.ToDecimal(drFixedDeduction["Portion"].ToString());
                        SalaryAmount = Math.Ceiling(Portion);


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedDeduction["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedDeduction["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drFixedDeduction["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drFixedDeduction

                    #region drMedical
                    foreach (DataRow drNotFixedEarning in ds.Tables[4].Rows)//Medical
                    {
                        Portion = 0;
                        NewId++;

                        IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                        Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                        SalaryAmount = Portion;
                        if (BasicSalary != MBasic)
                        {
                            if (!IsFixed)
                            {
                                SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                            }
                        }
                        else
                        {
                            if (MMedical > 0)
                            {
                                SalaryAmount = MMedical;
                            }
                            else
                            {
                                if (!IsFixed)
                                {
                                    SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                                }
                            }
                        }


                        GrossSalary = GrossSalary + SalaryAmount;

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drMedical

                    #region drConveyance
                    foreach (DataRow drNotFixedEarning in ds.Tables[5].Rows)//Conveyance
                    {
                        Portion = 0;
                        NewId++;

                        IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                        Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                        SalaryAmount = Portion;
                        if (BasicSalary != MBasic)
                        {
                            if (!IsFixed)
                            {
                                SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                            }
                        }
                        else
                        {
                            if (MConveyance > 0)
                            {
                                SalaryAmount = MConveyance;
                            }
                            else
                            {
                                if (!IsFixed)
                                {
                                    SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                                }
                            }
                        }



                        GrossSalary = GrossSalary + SalaryAmount;

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drConveyance

                    #region drConveyance
                    foreach (DataRow drNotFixedEarning in ds.Tables[6].Rows)//HR
                    {
                        Portion = 0;
                        NewId++;

                        IsFixed = Convert.ToBoolean(drNotFixedEarning["IsFixed"].ToString());
                        Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                        SalaryAmount = Portion;

                        if (!IsFixed)
                        {
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                        }


                        GrossSalary = GrossSalary + SalaryAmount;

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drConveyance

                    #region OldDataInstert
                    if (count == 1)
                    {
                        #region drNotFixedEarning
                        foreach (DataRow drNotFixedEarning in dsOld1.Tables[0].Rows)
                        {
                            Portion = 0;
                            NewId++;
                            Portion = Convert.ToDecimal(drNotFixedEarning["Portion"].ToString());
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);
                            GrossSalary = GrossSalary + SalaryAmount;

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                            cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                            cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedEarning["SalaryTypeId"].ToString());
                            cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedEarning["SalaryType"].ToString());
                            cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                            cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedEarning["Portion"].ToString());
                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                            cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                            cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                            cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                        }
                        #endregion drNotFixedEarning

                        #region drFixedEarning
                        foreach (DataRow drFixedEarning in dsOld2.Tables[0].Rows)
                        {
                            Portion = 0;
                            NewId++;
                            Portion = Convert.ToDecimal(drFixedEarning["Portion"].ToString());
                            SalaryAmount = Math.Ceiling(Portion);
                            GrossSalary = GrossSalary + SalaryAmount;


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                            cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                            cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedEarning["SalaryTypeId"].ToString());
                            cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedEarning["SalaryType"].ToString());
                            cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                            cmdInsert.Parameters.AddWithValue("@Portion", drFixedEarning["Portion"].ToString());
                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedEarning["PortionSalaryType"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                            cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                            cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                            cmdInsert.Parameters.AddWithValue("@Remarks", drFixedEarning["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                        }
                        #endregion drFixedEarning

                        #region drNotFixedDeduction
                        foreach (DataRow drNotFixedDeduction in dsOld3.Tables[0].Rows)
                        {
                            Portion = 0;
                            NewId++;
                            Portion = Convert.ToDecimal(drNotFixedDeduction["Portion"].ToString());
                            SalaryAmount = Math.Ceiling(BasicSalary * Portion / 100);


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                            cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                            cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drNotFixedDeduction["SalaryTypeId"].ToString());
                            cmdInsert.Parameters.AddWithValue("@SalaryType", drNotFixedDeduction["SalaryType"].ToString());
                            cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                            cmdInsert.Parameters.AddWithValue("@Portion", drNotFixedDeduction["Portion"].ToString());
                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drNotFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                            cmdInsert.Parameters.AddWithValue("@IsFixed", false);
                            cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                            cmdInsert.Parameters.AddWithValue("@Remarks", drNotFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                        }
                        #endregion drNotFixedDeduction

                        #region drFixedDeduction
                        foreach (DataRow drFixedDeduction in dsOld4.Tables[0].Rows)
                        {
                            Portion = 0;
                            NewId++;
                            Portion = Convert.ToDecimal(drFixedDeduction["Portion"].ToString());
                            SalaryAmount = Math.Ceiling(Portion);


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                            cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                            cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drFixedDeduction["SalaryTypeId"].ToString());
                            cmdInsert.Parameters.AddWithValue("@SalaryType", drFixedDeduction["SalaryType"].ToString());
                            cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                            cmdInsert.Parameters.AddWithValue("@Portion", drFixedDeduction["Portion"].ToString());
                            cmdInsert.Parameters.AddWithValue("@PortionSalaryType", drFixedDeduction["PortionSalaryType"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                            cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                            cmdInsert.Parameters.AddWithValue("@IsEarning", false);
                            cmdInsert.Parameters.AddWithValue("@Remarks", drFixedDeduction["Remarks"].ToString() ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                        }
                        #endregion drFixedDeduction
                    }
                    #endregion OldDataInstert




                    #region drGross
                    foreach (DataRow drGross in dsGB.Tables[0].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        if (GB == "True")
                        {
                            GrossSalary = GrossSalary;
                        }


                        Portion = GrossSalary;
                        SalaryAmount = Math.Ceiling(Portion);

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                        cmdInsert.Parameters.AddWithValue("@Id", (NewId));
                        cmdInsert.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdInsert.Parameters.AddWithValue("@SalaryTypeId", drGross["Id"].ToString());
                        cmdInsert.Parameters.AddWithValue("@SalaryType", "Gross");
                        cmdInsert.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdInsert.Parameters.AddWithValue("@Portion", Portion);
                        cmdInsert.Parameters.AddWithValue("@PortionSalaryType", "Gross");
                        cmdInsert.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdInsert.Parameters.AddWithValue("@IsFixed", true);
                        cmdInsert.Parameters.AddWithValue("@IsEarning", true);
                        cmdInsert.Parameters.AddWithValue("@Remarks", drGross["Remarks"].ToString() ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();
                    }
                    #endregion drGross
                    #region drBasic
                    foreach (DataRow drBasic in dsGB.Tables[1].Rows)
                    {
                        Portion = 0;
                        NewId++;
                        Portion = BasicSalary;
                        SalaryAmount = Math.Ceiling(Portion);

                        SqlCommand cmdBasic = new SqlCommand(sqlText, currConn);
                        cmdBasic.Parameters.AddWithValue("@Id", (NewId));
                        cmdBasic.Parameters.AddWithValue("@EmployeeSalaryStructureId", (EmployeeSalaryStructureNewId));
                        cmdBasic.Parameters.AddWithValue("@SalaryTypeId", drBasic["Id"].ToString());
                        cmdBasic.Parameters.AddWithValue("@SalaryType", "Basic");
                        cmdBasic.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                        cmdBasic.Parameters.AddWithValue("@Portion", Portion);
                        cmdBasic.Parameters.AddWithValue("@PortionSalaryType", "Basic");
                        cmdBasic.Parameters.AddWithValue("@Amount", SalaryAmount);
                        cmdBasic.Parameters.AddWithValue("@IsFixed", true);
                        cmdBasic.Parameters.AddWithValue("@IsEarning", true);
                        cmdBasic.Parameters.AddWithValue("@Remarks", drBasic["Remarks"].ToString() ?? Convert.DBNull);
                        cmdBasic.Parameters.AddWithValue("@IsActive", true);
                        cmdBasic.Parameters.AddWithValue("@IsArchive", false);
                        cmdBasic.Parameters.AddWithValue("@CreatedBy", LastUpdateBy);
                        cmdBasic.Parameters.AddWithValue("@CreatedAt", LastUpdateAt);
                        cmdBasic.Parameters.AddWithValue("@CreatedFrom", LastUpdateFrom);
                        cmdBasic.Transaction = transaction;
                        cmdBasic.ExecuteNonQuery();
                    }
                    #endregion drBasic


                    #endregion Save
                    #endregion Insert
                   

                    #endregion Check Exists


                    sqlText = "";

                    if (!Convert.ToBoolean(GB) == true)
                    {
                        sqlText = "";
                        sqlText = @" 
update EmployeeSalaryStructureDetail set Amount=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where EmployeeSalaryStructureDetail.EmployeeId=@EmployeeId
 and SalaryType in('gross') 
 update employeejob set GrossSalary=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType not in('gross') and EmployeeId=@EmployeeId ) earn
 where employeejob.EmployeeId=@EmployeeId
 update employeejob set BasicSalary=earn.amount
 from(
 select sum(amount)amount from EmployeeSalaryStructureDetail
 where 1=1 
 and IsEarning=1 and SalaryType  in('basic') and EmployeeId=@EmployeeId ) earn
 where employeejob.EmployeeId=@EmployeeId

 ";
                    }
                    else
                    {
                        sqlText = "";
                        sqlText = @" 
 update employeejob set GrossSalary=@GrossSalary
 where EmployeeId=@EmployeeId
 update employeejob set BasicSalary=@BasicSalary
 where EmployeeId=@EmployeeId
";
                    }

                    SqlCommand cmdUpdateEarn = new SqlCommand(sqlText, currConn);
                    cmdUpdateEarn.Parameters.AddWithValue("@EmployeeId",empvm.Id);
                    if (GB == "True")
                    {
                        cmdUpdateEarn.Parameters.AddWithValue("@BasicSalary", BasicSalary);
                        cmdUpdateEarn.Parameters.AddWithValue("@GrossSalary", GrossSalary);

                    }
                }
            }
            #endregion
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

        public DataTable EmployeeIncrement(EmployeeInfoVM vm,  SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            string[] retResults = new string[6];
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

                #region SqlText

                sqlText = @"
 
select * into #IncrementPrevious from   ( 
select distinct   p.EmployeeId,p.SalaryTypeId, max(p.IncrementDate)IncrementDate ,sum(Amount)Amount 
from EmployeeSalaryStructureDetail p ,
(select distinct employeeId, max(IncrementDate)IncrementDate   from EmployeeSalaryStructureDetail 
group by employeeId) l
where p.EmployeeId=l.EmployeeId and p.IncrementDate<l.IncrementDate
group by p.employeeId,p.SalaryTypeId
  ) as a
 select * into #IncrementCurrent from   ( 
 select   p.EmployeeId,p.SalaryTypeId, max(p.IncrementDate)IncrementDate ,sum(Amount)Amount  from EmployeeSalaryStructureDetail p ,
(select distinct employeeId, max(IncrementDate)IncrementDate
from EmployeeSalaryStructureDetail 
group by employeeId,SalaryTypeId) l
where p.EmployeeId=l.EmployeeId and p.IncrementDate=l.IncrementDate
group by p.employeeId,p.SalaryTypeId
) as  b  

select e.Code,e.EmpName,e.Designation,e.JoinDate
,C.IncrementDate IncrementDate
,st.Name SalaryHead,p.Amount PreviousAmount
 ,c.Amount IncrementAmount ,p.Amount+c.Amount CurrentAmount
  from #IncrementPrevious p
left outer join  #IncrementCurrent c on p.EmployeeId=c.EmployeeId and p.SalaryTypeId=c.SalaryTypeId
left outer join  ViewEmployeeInformation e on p.EmployeeId=e.EmployeeId
left outer join  EnumSalaryType st on p.SalaryTypeId=st.Id
where 1=1 
and st.Name in('Basic','grossX','House Rent','Medical','Transport Allowance' )
 
";

               
                #endregion SqlText

                #region SqlExecution

                if (vm.EmployeeId!=null)
                {
                    sqlText += @" and p.EmployeeId=@EmployeeId";
                }
                sqlText += @" order by case when st.Name='Basic' then 1
		       when st.Name='House Rent' then 2
		       when st.Name='Medical' then 3
		       when st.Name='Transport Allowance' then 4
		       when st.Name='gross' then 5
			   else 99 end  ";

                sqlText += @"  drop table #IncrementPrevious
                               drop table #IncrementCurrent ";
                 
                //sqlText += @" order by GroupTypeSL,   GroupSL, GroupName,COASL,AccountName, RowSL ";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;


                if (vm.EmployeeId != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmployeeId" ,vm.EmployeeId);
                }

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");
                dt = Ordinary.DtColumnStringToDate(dt, "IncrementDate");

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

       
    }
}
