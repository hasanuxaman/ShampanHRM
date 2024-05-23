using SymOrdinary;
using SymServices.Attendance;
using SymServices.Common;
using SymServices.Enum;
using SymServices.HRM;
using SymServices.PF;
using SymServices.Tax;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

//////////using System.Web;
//////using System.Web.Mvc;
namespace SymServices.Payroll
{
    public class SalaryProcessDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion

        public string[] SalaryPreProcessNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string EmployeeIdF, string EmployeeIdT, FiscalYearVM fiscalYearVM, string processName, string CompanyName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string[] retResults = new string[6];
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            EmployeeOtherEarningDAL _EmployeeOtherEarningDAL = new EmployeeOtherEarningDAL();

            #endregion

            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPool();
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

                #region Checkpoint
                FiscalYearDAL _fyDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();

                #region Previous Fiscal Period Status
                fydVM = new FiscalYearDetailVM();

                fydVM = _fyDAL.SelectAll_PreviousFiscalPeriod(Convert.ToInt32(FiscalYearDetailId), currConn, transaction).FirstOrDefault();

                if (fydVM != null)
                {
                    retResults[1] = "Previous Fiscal Period: " + fydVM.PeriodName + " must be Locked!";
                    throw new ArgumentNullException("", retResults[1]);
                }


                #endregion

                #region Current Fiscal Period Status
                fydVM = new FiscalYearDetailVM();

                fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(FiscalYearDetailId), null, null, currConn, transaction).FirstOrDefault();

                if (fydVM.PeriodLock)
                {
                    retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                    throw new ArgumentNullException("", retResults[1]);
                }
                if (CompanyName.ToUpper() == "BOLLORE")
                {
                    #region Current PayRoll Lock Status
                    if (fydVM.PayrollLock)
                    {
                        retResults[1] = "This PayRoll Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                        throw new ArgumentNullException("", retResults[1]);
                    }
                    #endregion
                }

                #endregion

                #endregion

                #region Fiscal Year
                string PeriodEnd = "";
                string PeriodStart = "";
                string sqlText = "";
                sqlText = @"select * from FiscalYearDetail
                            where id=@FiscalYearDetailsId";
                SqlCommand cmdfy = new SqlCommand(sqlText, currConn, transaction);
                cmdfy.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailId);
                using (SqlDataReader dr = cmdfy.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PeriodEnd = dr["PeriodEnd"].ToString();
                        PeriodStart = dr["PeriodStart"].ToString();
                    }
                    dr.Close();
                }

                retResults = DeleteLeftEmployee(PeriodStart, PeriodEnd, FiscalYearDetailId.ToString(), currConn, transaction);
                if (retResults[0].Trim().ToUpper() == "FAIL")
                {
                    throw new ArgumentException(retResults[1], "");
                }


                #endregion Fiscal Year

                #region Business


                #region ATTENDANCE

                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "ATTENDANCE")
                {
                    MonthlyAttendanceVM vVM = new MonthlyAttendanceVM();
                    MonthlyAttendanceDAL _dal = new MonthlyAttendanceDAL();

                    vVM.FiscalYearDetailId = FiscalYearDetailId;
                    vVM.CreatedFrom = fiscalYearVM.CreatedFrom;
                    vVM.CreatedAt = fiscalYearVM.CreatedAt;
                    vVM.CreatedBy = fiscalYearVM.CreatedBy;

                    retResults = _dal.MonthlyAttendanceProcessRegular(vVM, currConn, transaction);

                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                }
                #endregion ATTENDANCE

                #region _salaryEarning
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "SALARY EARNING")
                {
                    SalaryEarningDAL _salaryEarning = new SalaryEarningDAL();
                    retResults = _salaryEarning.InsertSalaryEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryEarning.InsertSalaryEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                  , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryEarning.InsertSalaryEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                  , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                    retResults = _salaryEarning.InsertSalaryEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                  , EmployeeIdF, EmployeeIdT, "archive", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                #endregion _salaryEarning
                if (CompanyName.ToLower() == "tib")
                {
                    #region SALARY ARREAR
                    if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "SALARY ARREAR")
                    {

                        ShampanIdentityVM auditvm = new ShampanIdentityVM();

                        auditvm.LastUpdateAt = fiscalYearVM.CreatedAt;
                        auditvm.LastUpdateBy = fiscalYearVM.CreatedBy;
                        auditvm.LastUpdateFrom = fiscalYearVM.CreatedFrom;
                        auditvm.CreatedAt = fiscalYearVM.CreatedAt;
                        auditvm.CreatedBy = fiscalYearVM.CreatedBy;
                        auditvm.CreatedFrom = fiscalYearVM.CreatedFrom;

                        string SQLText1 = @"select distinct EmployeeId,IncrementDate,ArrearFiscalYearDetailId
                    from EmployeeSalaryStructure where IsArrear=1 and ArrearFiscalYearDetailId=@ArrearFiscalYearDetailId";
                        SqlCommand objComm = new SqlCommand(SQLText1, currConn, transaction);
                        SqlDataAdapter da = new SqlDataAdapter(objComm);

                        da.SelectCommand.Parameters.AddWithValue("@ArrearFiscalYearDetailId", FiscalYearDetailId);
                        DataTable dt1 = new DataTable();
                        da.Fill(dt1);

                        foreach (DataRow row in dt1.Rows)
                        {
                            decimal BasicArear = 0;
                            decimal MedicalArear = 0;
                            decimal HouseRentArear = 0;
                            decimal ConveyanceArear = 0;

                            string EmployeeId = row["EmployeeId"].ToString();
                            string IncrementDay = row["IncrementDate"].ToString();
                            string SalaryProcessStartDate = new FiscalYearDAL().FYPeriodDetail(FiscalYearDetailId, "", "", currConn, transaction).PeriodStart;
                            int IncrementFiscalYearDetailId = new FiscalYearDAL().FYPeriodDetail(0, "", IncrementDay, currConn, transaction).Id;
                            //DateTime IncrementDateTemp ="01"+ Convert.ToDateTime(Ordinary.StringToDate(IncrementDay)).ToString("MMyyyy");

                            DataTable dtArearFiscalYear = new FiscalYearDAL().FYPeriodDetailDt(0, new[] { "PeriodStart>", "PeriodStart<" }, new[] { Convert.ToDateTime(Ordinary.StringToDate(IncrementDay)).ToString("yyyyMM") + "01", SalaryProcessStartDate }, currConn, transaction);
                            dtArearFiscalYear.Rows.RemoveAt(dtArearFiscalYear.Rows.Count - 1);
                            int MonthC = 0;

                            EmployeeStructureGroupVM CurrentEmployeeStructureGroup = new EmployeeStructureGroupDAL().SelectByEmployee(EmployeeId, currConn, transaction);

                            string CurrentGradeId = CurrentEmployeeStructureGroup.GradeId;
                            string CurrentStepId = CurrentEmployeeStructureGroup.StepId;

                            foreach (DataRow item in dtArearFiscalYear.Rows)
                            {

                                decimal IncrementBasic = 0;
                                decimal IncrementHouseRent = 0;
                                decimal IncrementMedical = 0;
                                decimal IncrementConveyance = 0;


                                decimal CurrentBasic = 0;
                                decimal CurrentHouseRent = 0;
                                decimal CurrentMedical = 0;
                                decimal CurrentConveyance = 0;

                                decimal DiffBasic = 0;
                                decimal DiffHouseRent = 0;
                                decimal DiffMedical = 0;
                                decimal DiffConveyance = 0;

                                string FYID = item["Id"].ToString();
                                string FY = Convert.ToDateTime(Ordinary.StringToDate(item["PeriodStart"].ToString())).ToString("yyyy");





                                string SalaryInfoByGradeStep = new SalaryStructureMatrixDAL().BasicAmount(CurrentGradeId, CurrentStepId, FY, "");
                                string[] SalaryInfoByGradeStepArray = SalaryInfoByGradeStep.Split('~');

                                IncrementBasic = Convert.ToDecimal(SalaryInfoByGradeStepArray[0]);
                                IncrementHouseRent = Convert.ToDecimal(SalaryInfoByGradeStepArray[1]);
                                IncrementMedical = Convert.ToDecimal(SalaryInfoByGradeStepArray[2]);
                                IncrementConveyance = Convert.ToDecimal(SalaryInfoByGradeStepArray[3]);

                                #region SQLText2

                                string SQLText2 = @"
                                    select distinct FiscalYearDetailId,EmployeeId ,sum(Basic)Basic,sum(Medical)Medical,sum(HouseRent)HouseRent,sum(Conveyance)Conveyance from (
                                    select distinct FiscalYearDetailId,EmployeeId 
                                    ,case when SalaryType in('Basic') then Amount else 0 end  'Basic'
                                    ,case when SalaryType in('Medical') then Amount else 0 end  'Medical'
                                    ,case when SalaryType in('HouseRent') then Amount else 0 end  'HouseRent'
                                    ,case when SalaryType in('Conveyance') then Amount else 0 end  'Conveyance'
                                    from SalaryEarningDetail where EmployeeId=@EmployeeId and FiscalYearDetailId= @IncrementFiscalYearDetailId
                                    ) as a
                                    group by FiscalYearDetailId,EmployeeId

                                    ";
                                #endregion SQLText2


                                objComm = new SqlCommand(SQLText2, currConn, transaction);
                                SqlDataAdapter da2 = new SqlDataAdapter(objComm);

                                da2.SelectCommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                da2.SelectCommand.Parameters.AddWithValue("@IncrementFiscalYearDetailId", FYID);
                                DataSet ds = new DataSet();
                                da2.Fill(ds);
                                DataTable SalaryDt = ds.Tables[0].Copy();
                                foreach (DataRow SalaryR in SalaryDt.Rows)
                                {
                                    CurrentBasic = Convert.ToDecimal(SalaryR["Basic"]);
                                    CurrentHouseRent = Convert.ToDecimal(SalaryR["HouseRent"]);
                                    CurrentMedical = Convert.ToDecimal(SalaryR["Medical"]);
                                    CurrentConveyance = Convert.ToDecimal(SalaryR["Conveyance"]);
                                }

                                DiffBasic = IncrementBasic - CurrentBasic;
                                DiffHouseRent = IncrementHouseRent - CurrentHouseRent;
                                DiffMedical = IncrementMedical - CurrentMedical;
                                DiffConveyance = IncrementConveyance - CurrentConveyance;


                                string PayrollProceeDay = new FiscalYearDAL().FYPeriodDetail(FiscalYearDetailId, "", IncrementDay, currConn, transaction).PeriodStart;

                                DateTime IncrementDate = Convert.ToDateTime(Ordinary.StringToDate(IncrementDay));
                                DateTime PayrollProceeDate = Convert.ToDateTime(Ordinary.StringToDate(PayrollProceeDay)).AddDays(-1);

                                //int month = ((PayrollProceeDate.Year - IncrementDate.Year) * 12) + PayrollProceeDate.Month - IncrementDate.Month;




                                if (MonthC == 0)
                                {
                                    int IncrementDayInMonth = DateTime.DaysInMonth(Convert.ToInt32(IncrementDate.ToString("yyyy")), Convert.ToInt32(IncrementDate.ToString("MM")));
                                    int incDay = Convert.ToInt32(IncrementDate.ToString("dd"));
                                    int restIncDay = IncrementDayInMonth - incDay + 1;

                                    if (restIncDay > 0)
                                    {
                                        BasicArear = BasicArear + (DiffBasic > 0 ? (DiffBasic / IncrementDayInMonth * restIncDay) : 0);
                                        MedicalArear = MedicalArear + (DiffMedical > 0 ? (DiffMedical / IncrementDayInMonth * restIncDay) : 0);
                                        HouseRentArear = HouseRentArear + (DiffHouseRent > 0 ? (DiffHouseRent / IncrementDayInMonth * restIncDay) : 0);
                                        ConveyanceArear = ConveyanceArear + (DiffConveyance > 0 ? (DiffConveyance / IncrementDayInMonth * restIncDay) : 0);
                                    }
                                    else
                                    {
                                        BasicArear = BasicArear + (DiffBasic > 0 ? DiffBasic : 0);
                                        MedicalArear = MedicalArear + (DiffMedical > 0 ? DiffMedical : 0);
                                        HouseRentArear = HouseRentArear + (DiffHouseRent > 0 ? DiffHouseRent : 0);
                                        ConveyanceArear = ConveyanceArear + (DiffConveyance > 0 ? DiffConveyance : 0);
                                    }
                                }
                                else
                                {
                                    BasicArear = BasicArear + (DiffBasic > 0 ? DiffBasic : 0);
                                    MedicalArear = MedicalArear + (DiffMedical > 0 ? DiffMedical : 0);
                                    HouseRentArear = HouseRentArear + (DiffHouseRent > 0 ? DiffHouseRent : 0);
                                    ConveyanceArear = ConveyanceArear + (DiffConveyance > 0 ? DiffConveyance : 0);

                                }



                                //var tt = new SalaryEmployeeDAL().SelectAll(0, new[] { "se.EmployeeId", "se.FiscalYearDetailId" }, new[] { EmployeeId, FYID }, currConn, transaction);
                                //string GradeId = tt.FirstOrDefault().GradeId;
                                //string StepId = tt.FirstOrDefault().StepId;




                                MonthC++;
                            }
                            #region EmployeeOtherEarningVM

                            EmployeeOtherEarningVM vm = new EmployeeOtherEarningVM();
                            vm.LastUpdateAt = auditvm.CreatedAt;
                            vm.LastUpdateBy = auditvm.CreatedBy;
                            vm.LastUpdateFrom = auditvm.CreatedFrom;
                            vm.CreatedAt = auditvm.CreatedAt;
                            vm.CreatedBy = auditvm.CreatedBy;
                            vm.CreatedFrom = auditvm.CreatedFrom;
                            vm.EmployeeId = EmployeeId;
                            vm.FiscalYearDetailId = FiscalYearDetailId;
                            vm.EarningDate = PeriodStart;
                            vm.OTHrsSpecial = 0;
                            vm.OTHrs = 0;
                            vm.SalaryMonthId = FiscalYearDetailId.ToString();

                            // insert 
                            if (BasicArear != 0)
                            {
                                vm.EarningAmount = BasicArear;
                                vm.EarningTypeId = 23;

                                retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            }
                            // insert 
                            if (HouseRentArear != 0)
                            {
                                vm.EarningAmount = HouseRentArear;
                                vm.EarningTypeId = 24;

                                retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            }
                            // insert 
                            if (MedicalArear != 0)
                            {
                                vm.EarningAmount = MedicalArear;
                                vm.EarningTypeId = 25;

                                retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            }
                            // insert 
                            if (ConveyanceArear != 0)
                            {
                                vm.EarningAmount = ConveyanceArear;
                                vm.EarningTypeId = 26;

                                retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            }
                            // insert 

                            if (BasicArear != 0)
                            {
                                vm.EarningAmount = -1 * (BasicArear * 10 / 100);
                                vm.EarningTypeId = 20;

                                retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            }
                            #endregion EmployeeOtherEarningVM
                            //                            #region Comments
                            //                            if (FiscalYearDetailId != IncrementFiscalYearDetailId)
                            //                            {
                            //                                #region SQLText2

                            //                                string SQLText2 = @"
                            //select distinct FiscalYearDetailId,EmployeeId ,sum(Basic)Basic,sum(Medical)Medical,sum(HouseRent)HouseRent,sum(Conveyance)Conveyance from (
                            //select distinct FiscalYearDetailId,EmployeeId 
                            //,case when SalaryType in('Basic') then Amount else 0 end  'Basic'
                            //,case when SalaryType in('Medical') then Amount else 0 end  'Medical'
                            //,case when SalaryType in('HouseRent') then Amount else 0 end  'HouseRent'
                            //,case when SalaryType in('Conveyance') then Amount else 0 end  'Conveyance'
                            //from SalaryEarningDetail where EmployeeId=@EmployeeId and FiscalYearDetailId= @IncrementFiscalYearDetailId
                            //) as a
                            //group by FiscalYearDetailId,EmployeeId
                            //
                            //select distinct FiscalYearDetailId,EmployeeId ,sum(Basic)Basic,sum(Medical)Medical,sum(HouseRent)HouseRent,sum(Conveyance)Conveyance from (
                            //select distinct FiscalYearDetailId,EmployeeId 
                            //,case when SalaryType in('Basic') then Amount else 0 end  'Basic'
                            //,case when SalaryType in('Medical') then Amount else 0 end  'Medical'
                            //,case when SalaryType in('HouseRent') then Amount else 0 end  'HouseRent'
                            //,case when SalaryType in('Conveyance') then Amount else 0 end  'Conveyance'
                            //from SalaryEarningDetail where EmployeeId=@EmployeeId and FiscalYearDetailId= @FiscalYearDetailId
                            //) as a
                            //group by FiscalYearDetailId,EmployeeId
                            //
                            //";
                            //                                #endregion SQLText2


                            //                                objComm = new SqlCommand(SQLText2, currConn, transaction);
                            //                                SqlDataAdapter da2 = new SqlDataAdapter(objComm);

                            //                                da2.SelectCommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            //                                da2.SelectCommand.Parameters.AddWithValue("@IncrementFiscalYearDetailId", IncrementFiscalYearDetailId);
                            //                                da2.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                            //                                DataSet ds = new DataSet();
                            //                                da2.Fill(ds);
                            //                                DataTable IncrementSalary = ds.Tables[0].Copy();
                            //                                DataTable CurrentSalary = ds.Tables[1].Copy();
                            //                                decimal IncrementBasic = 0;
                            //                                decimal IncrementHouseRent = 0;
                            //                                decimal IncrementMedical = 0;
                            //                                decimal IncrementConveyance = 0;


                            //                                decimal CurrentBasic = 0;
                            //                                decimal CurrentHouseRent = 0;
                            //                                decimal CurrentMedical = 0;
                            //                                decimal CurrentConveyance = 0;

                            //                                decimal DiffBasic = 0;
                            //                                decimal DiffHouseRent = 0;
                            //                                decimal DiffMedical = 0;
                            //                                decimal DiffConveyance = 0;

                            //                                foreach (DataRow IncrementR in IncrementSalary.Rows)
                            //                                {
                            //                                    IncrementBasic = Convert.ToDecimal(IncrementR["Basic"]);
                            //                                    IncrementHouseRent = Convert.ToDecimal(IncrementR["HouseRent"]);
                            //                                    IncrementMedical = Convert.ToDecimal(IncrementR["Medical"]);
                            //                                    IncrementConveyance = Convert.ToDecimal(IncrementR["Conveyance"]);
                            //                                }
                            //                                foreach (DataRow CurrentSalaryR in CurrentSalary.Rows)
                            //                                {
                            //                                    CurrentBasic = Convert.ToDecimal(CurrentSalaryR["Basic"]);
                            //                                    CurrentHouseRent = Convert.ToDecimal(CurrentSalaryR["HouseRent"]);
                            //                                    CurrentMedical = Convert.ToDecimal(CurrentSalaryR["Medical"]);
                            //                                    CurrentConveyance = Convert.ToDecimal(CurrentSalaryR["Conveyance"]);
                            //                                }
                            //                                DiffBasic = CurrentBasic - IncrementBasic;
                            //                                DiffHouseRent = CurrentHouseRent - IncrementHouseRent;
                            //                                DiffMedical = CurrentMedical - IncrementMedical;
                            //                                DiffConveyance = CurrentConveyance - IncrementConveyance;

                            //                                string PayrollProceeDay = new FiscalYearDAL().FYPeriodDetail(FiscalYearDetailId, "", IncrementDay, currConn, transaction).PeriodStart;

                            //                                DateTime IncrementDate = Convert.ToDateTime(Ordinary.StringToDate(IncrementDay));
                            //                                DateTime PayrollProceeDate = Convert.ToDateTime(Ordinary.StringToDate(PayrollProceeDay)).AddDays(-1);
                            //                                int month = ((PayrollProceeDate.Year - IncrementDate.Year) * 12) + PayrollProceeDate.Month - IncrementDate.Month;

                            //                                int IncrementDayInMonth = DateTime.DaysInMonth(Convert.ToInt32(IncrementDate.ToString("yyyy")), Convert.ToInt32(IncrementDate.ToString("MM")));
                            //                                int incDay = Convert.ToInt32(IncrementDate.ToString("dd"));
                            //                                int restIncDay = IncrementDayInMonth - incDay + 1;



                            //                                #region Get SalaryMonthId

                            //                                sqlText = @"
                            //select top 1 * from FiscalYearDetail where id<@FiscalYearDetailsId order by id desc
                            //
                            //";
                            //                                objComm = new SqlCommand(sqlText, currConn, transaction);
                            //                                objComm.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailId);
                            //                                string salaryId = "";
                            //                                using (SqlDataReader dr = objComm.ExecuteReader())
                            //                                {
                            //                                    while (dr.Read())
                            //                                    {
                            //                                        salaryId = dr["Id"].ToString();
                            //                                    }
                            //                                    dr.Close();
                            //                                }

                            //                                #endregion
                            //                                #region EmployeeOtherEarningVM

                            //                                EmployeeOtherEarningVM vm = new EmployeeOtherEarningVM();
                            //                                vm.LastUpdateAt = auditvm.CreatedAt;
                            //                                vm.LastUpdateBy = auditvm.CreatedBy;
                            //                                vm.LastUpdateFrom = auditvm.CreatedFrom;
                            //                                vm.CreatedAt = auditvm.CreatedAt;
                            //                                vm.CreatedBy = auditvm.CreatedBy;
                            //                                vm.CreatedFrom = auditvm.CreatedFrom;
                            //                                vm.EmployeeId = EmployeeId;
                            //                                vm.FiscalYearDetailId = FiscalYearDetailId;
                            //                                vm.EarningDate = PeriodStart;
                            //                                vm.OTHrsSpecial = 0;
                            //                                vm.OTHrs = 0;
                            //                                vm.SalaryMonthId = FiscalYearDetailId.ToString();

                            //                                decimal BasicArear = DiffBasic>0 ?(DiffBasic * month) + (DiffBasic / IncrementDayInMonth * restIncDay):0;
                            //                                if (BasicArear != 0)
                            //                                {
                            //                                    vm.EarningAmount = BasicArear;
                            //                                    vm.EarningTypeId = 23;

                            //                                    retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            //                                }
                            //                                decimal HouseRentArear = DiffHouseRent > 0 ? (DiffHouseRent * month) + (DiffHouseRent / IncrementDayInMonth * restIncDay) : 0;
                            //                                if (HouseRentArear != 0)
                            //                                {
                            //                                    vm.EarningAmount = HouseRentArear;
                            //                                    vm.EarningTypeId = 24;

                            //                                    retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            //                                }
                            //                                decimal MedicalArear = DiffMedical > 0 ? (DiffMedical * month) + (DiffMedical / IncrementDayInMonth * restIncDay) : 0;
                            //                                if (MedicalArear!=0)
                            //                                {
                            //                                    vm.EarningAmount = MedicalArear;
                            //                                    vm.EarningTypeId = 25;

                            //                                    retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            //                                }
                            //                                decimal ConveyanceArear = DiffConveyance > 0 ? (DiffConveyance * month) + (DiffConveyance / IncrementDayInMonth * restIncDay) : 0;
                            //                                if (ConveyanceArear != 0)
                            //                                {
                            //                                    vm.EarningAmount = ConveyanceArear;
                            //                                    vm.EarningTypeId = 26;

                            //                                    retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            //                                }
                            //                                decimal PFArear = -1 * (BasicArear * 10 / 100);
                            //                                if (PFArear != 0)
                            //                                {
                            //                                    vm.EarningAmount = PFArear;
                            //                                    vm.EarningTypeId = 20;

                            //                                    retResults = _EmployeeOtherEarningDAL.Insert(vm, currConn, transaction);

                            //                                }
                            //                                #endregion EmployeeOtherEarningVM


                            //                            }
                            //                            #endregion Comments

                            //row["Fiscal Period"] = fname;
                        }



                        //retResults = new EmployeeOtherEarningDAL().SaveFixesOtSalaryProcess(FiscalYearDetailId, currConn, transaction, auditvm);
                    }
                    #endregion SALARY ARREAR

                    #region FIXED OT

                    if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "FIXED OT")
                    {

                        ShampanIdentityVM auditvm = new ShampanIdentityVM();

                        auditvm.LastUpdateAt = fiscalYearVM.CreatedAt;
                        auditvm.LastUpdateBy = fiscalYearVM.CreatedBy;
                        auditvm.LastUpdateFrom = fiscalYearVM.CreatedFrom;
                        auditvm.CreatedAt = fiscalYearVM.CreatedAt;
                        auditvm.CreatedBy = fiscalYearVM.CreatedBy;
                        auditvm.CreatedFrom = fiscalYearVM.CreatedFrom;

                        retResults = new EmployeeOtherEarningDAL().SaveFixesOtSalaryProcess(FiscalYearDetailId, currConn, transaction, auditvm);
                    }
                    #endregion FIXED OT
                }

                #region _salaryOtherEarning
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "OTHER EARNING")
                {
                    SalaryOtherEarningDAL _salaryOtherEarning = new SalaryOtherEarningDAL();
                    retResults = _salaryOtherEarning.InsertSalaryOtherEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryOtherEarning.InsertSalaryOtherEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryOtherEarning.InsertSalaryOtherEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                    retResults = _salaryOtherEarning.InsertSalaryOtherEarningNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "archive", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                #endregion _salaryOtherEarning

                #region _salaryOtherDeduction
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "OTHER DEDUCTION")
                {
                    SalaryOtherDeductionDAL _salaryOtherDeduction = new SalaryOtherDeductionDAL();
                    retResults = _salaryOtherDeduction.InsertSalaryOtherDeductionNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryOtherDeduction.InsertSalaryOtherDeductionNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryOtherDeduction.InsertSalaryOtherDeductionNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                #endregion _salaryOtherDeduction


                #region _salaryLoan
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "LOAN")
                {
                    SalaryLoanDAL _salaryLoan = new SalaryLoanDAL();
                    retResults = _salaryLoan.InsertSalaryLoanNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryLoan.InsertSalaryLoanNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryLoan.InsertSalaryLoanNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                #endregion _salaryLoan

                #region _salaryPF
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "PF")
                {
                    SalaryPFDAL _salaryPF = new SalaryPFDAL();
                    retResults = _salaryPF.InsertSalaryPFNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                    , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryPF.InsertSalaryPFNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                    , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryPF.InsertSalaryPFNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                    retResults = _salaryPF.InsertSalaryPFNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "archive", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                #endregion _salaryPF

                #region _salaryTAX
                SalaryTaxDAL _salaryTax = new SalaryTaxDAL();
                if (string.IsNullOrWhiteSpace(processName) || processName.ToUpper() == "TAX")
                {

                    retResults = _salaryTax.InsertSalaryTaxNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "new", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryTax.InsertSalaryTaxNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "regular", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                    retResults = _salaryTax.InsertSalaryTaxNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "left", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                    retResults = _salaryTax.InsertSalaryTaxNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                   , EmployeeIdF, EmployeeIdT, "archive", fiscalYearVM, CompanyName, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }
                string TaxValueFromEmployee = new SettingDAL().settingValue("Tax", "ValueFromEmployee");
                if (TaxValueFromEmployee.ToLower() == "y")
                {
                    retResults = _salaryTax.UpsateSalaryTaxNew(FiscalYearDetailId, currConn, transaction);
                }

                #endregion _salaryTAX

                #region EmployeeStatus
                if (string.IsNullOrWhiteSpace(processName) || processName == "EMPLOYEE STATUS")
                {

                    retResults = UpdateEmployeeStatus(FiscalYearDetailId.ToString(), currConn, transaction);

                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException(retResults[1], "");
                    }
                }
                #endregion EmployeeStatus

                #region EMPLOYEE DEPENDENT

                if (string.IsNullOrWhiteSpace(processName) || processName == "EMPLOYEE DEPENDENT")
                {

                    SalaryEarningDAL _salaryEarning = new SalaryEarningDAL();
                    retResults = _salaryEarning.InsertSalaryDependent(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                        , EmployeeIdF, EmployeeIdT, "new", CompanyName, fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");

                    }
                    retResults = _salaryEarning.InsertSalaryDependent(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                        , EmployeeIdF, EmployeeIdT, "regular", CompanyName, fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");

                    }

                    retResults = _salaryEarning.InsertSalaryDependent(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                        , EmployeeIdF, EmployeeIdT, "left", CompanyName, fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }

                    retResults = _salaryEarning.InsertSalaryDependent(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                        , EmployeeIdF, EmployeeIdT, "archive", CompanyName, fiscalYearVM, currConn, transaction);
                    if (retResults[0].Trim().ToUpper() == "FAIL")
                    {
                        throw new ArgumentException("", "");
                    }
                }

                #endregion EMPLOYEE DEPENDENT

                #region SalaryEmployee

                SalaryEmployeeVM varSalaryEmployeeVM = new SalaryEmployeeVM();
                varSalaryEmployeeVM.FiscalYearDetailId = FiscalYearDetailId;
                varSalaryEmployeeVM.CreatedBy = fiscalYearVM.CreatedBy;
                varSalaryEmployeeVM.CreatedAt = fiscalYearVM.CreatedAt;
                varSalaryEmployeeVM.CreatedFrom = fiscalYearVM.CreatedFrom;

                retResults = new SalaryEmployeeDAL().InsertDetail(varSalaryEmployeeVM, currConn, transaction);

                if (retResults[0].Trim().ToUpper() == "FAIL")
                {
                    throw new ArgumentException(retResults[1], "");
                }

                #endregion

                #region Update CurrentEmployeeInfo

                retResults = UpdateCurrentEmployee(FiscalYearDetailId.ToString(), currConn, transaction);


                if (CompanyName.ToLower() == "tib")
                {

                }


                if (retResults[0].Trim().ToUpper() == "FAIL")
                {
                    throw new ArgumentException(retResults[1], "");
                }

                #endregion

                #endregion Business

                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Salary Preprocess Successfully.";
                retResults[2] = "Id";
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

        public List<SalaryPFDetailVM> EmployeeSalaryPF(string employeeID, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string sqlText = "";
            List<SalaryPFDetailVM> VMs = new List<SalaryPFDetailVM>();
            SalaryPFDetailVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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
                sqlText = @" select  p.id,p.PFValue,p.BasicSalary,p.GrossSalary ,p.Remarks,d.PeriodName
from SalaryPFDetail p
left join FiscalYearDetail d on d.Id=p.FiscalYearDetailId 
where p.EmployeeId=@empId
";
                #endregion
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@empId", employeeID);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryPFDetailVM();
                        vm.Id = Convert.ToInt32(dr["id"]);
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.PFValue = Convert.ToDecimal(dr["PFValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalPeriod = dr["PeriodName"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) { transaction.Rollback(); }
                return VMs;
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
            return VMs;
        }

        public List<SalaryTaxDetailVM> EmployeeSalaryTax(string employeeID, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string sqlText = "";
            List<SalaryTaxDetailVM> VMs = new List<SalaryTaxDetailVM>();
            SalaryTaxDetailVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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
                sqlText = @" select  s.Id,s.TaxValue,s.BasicSalary,s.GrossSalary ,s.Remarks,d.PeriodName
from SalaryTaxDetail s
left join FiscalYearDetail d on d.Id=s.FiscalYearDetailId 
where s.EmployeeId=@empId
";
                #endregion
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@empId", employeeID);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryTaxDetailVM();
                        vm.Id = Convert.ToInt32(dr["Id"]);
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.TaxValue = Convert.ToDecimal(dr["TaxValue"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalPeriod = dr["PeriodName"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return VMs;
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
            return VMs;
        }

        public List<EmployeeInfoVM> EmployeeSalaryEarning(string employeeID, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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
                sqlText = @" 
select 
sed.SalaryType,sed.Amount,sed.Remarks,d.PeriodName
from EmployeeInfo e
right join SalaryEarningDetail sed on  sed.EmployeeId=e.Id
left join FiscalYearDetail d on d.Id=sed.FiscalYearDetailId 
where  e.Id=@empoyeeId ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@empoyeeId", employeeID);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalPeriod = dr["PeriodName"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return VMs;
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
            return VMs;
        }

        public List<EmployeeInfoVM> EmployeeSalaryLoan(string employeeID, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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
                sqlText = @" 
select s.LoanAmount,s.Remarks,d.PeriodName
from SalaryLoanDetail s
left join FiscalYearDetail d on d.Id=s.FiscalYearDetailId
where  s.EmployeeId=@empoyeeId ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@empoyeeId", employeeID);
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.Amount = Convert.ToDecimal(dr["LoanAmount"]);
                        vm.Remarks = dr["Remarks"].ToString();
                        vm.FiscalPeriod = dr["PeriodName"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return VMs;
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
            return VMs;
        }

        public string[] DeleteProcess(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string fid, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Process Delete"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBank"); }
                #endregion open connection and transaction
                EmployeeInfoDAL _empDAL = new EmployeeInfoDAL();
                List<EmployeeInfoVM> vms = new List<EmployeeInfoVM>();

                #region CheckPoing

                #region Current Fiscal Period Status
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                FiscalYearDAL _fyDAL = new FiscalYearDAL();

                fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(fid), null, null, currConn, transaction).FirstOrDefault();

                if (fydVM.PeriodLock)
                {
                    retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                    throw new ArgumentNullException("", retResults[1]);
                }


                #endregion

                #endregion

                #region Update Settings
                sqlText = "";
                sqlText += " delete [dbo].[SalaryEarningDetail] where  FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryLoanDetail] where  FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryOtherDeduction] where  FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryOtherEarning] where   FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryPFDetail] where   FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryPFDetailEmployeer] where   FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[SalaryTaxDetail] where   FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += " delete [dbo].[MonthlyAttendance] where   FiscalYearDetailId=@FiscalYearDetailId";

                sqlText += " delete [dbo].[SalaryEmployee] where   FiscalYearDetailId=@FiscalYearDetailId";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                cmdUpdate.Transaction = transaction;
                int exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit
                if (transResult <= 0)
                {
                    //throw new ArgumentNullException("Process Delete Delete", fid + " could not Delete.");
                }
                #endregion Commit
                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete Bank Information.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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
            return retResults;
        }

        public DataSet ChildAllowanceDetail(string FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
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
                sqlText = @"select * from
(select sal.employeeid,sal.Amount SalaryChildAllowance,isnull(getAllowance.ChildAllowance,0)ChildAllowance,sal.FiscalYearDetailId from ViewSalaryPreCalculation sal
left outer join (select ed.EmployeeId,ed.Child,Allowance.Allowance,Allow.Allow
,case when ed.Child>Allow.Allow then CONVERT(decimal(18,2), Allowance.Allowance)*CONVERT(decimal(18,2), Allow.Allow)
else CONVERT(decimal(18,2),Allowance.Allowance)*CONVERT(decimal(18,2),ed.Child )
end  ChildAllowance
 from(select distinct ed.employeeid,count(ed.id)Child  from EmployeeDependent ed
, (select SettingValue Age from setting where SettingGroup='Dependent' and SettingName='Age')Age 
where DATEDIFF(MONTH, DateofBirth,getdate())<=Age.Age*12
group by employeeid) ed,
(select SettingValue Allowance from setting where SettingGroup='Dependent' and SettingName='Allowance'
)Allowance,(
select SettingValue Allow from setting where SettingGroup='Dependent' and SettingName='Allow'
)Allow) getAllowance on sal.EmployeeId=getAllowance.EmployeeId
where sal.SalaryHead='ChildAllowance'
and sal.Amount>0  ";
                if (FiscalYearDetailId != "0_0")
                    sqlText += @" and sal.FiscalYearDetailId=@FiscalYearDetailId";
                sqlText += @" union 
select ed.EmployeeId,0 SalaryChildAllowance,case when ed.Child>Allow.Allow then CONVERT(decimal(18,2), Allowance.Allowance)*CONVERT(decimal(18,2), Allow.Allow)
else CONVERT(decimal(18,2),Allowance.Allowance)*CONVERT(decimal(18,2),ed.Child )
end  ChildAllowance,0 FiscalYearDetailId
 from(select distinct ed.employeeid,count(ed.id)Child  from EmployeeDependent ed
, (select SettingValue Age from setting where SettingGroup='Dependent' and SettingName='Age')Age 
where DATEDIFF(MONTH, DateofBirth,getdate())<=Age.Age*12
group by employeeid) ed,
(select SettingValue Allowance from setting where SettingGroup='Dependent' and SettingName='Allowance'
)Allowance,(select SettingValue Allow from setting where SettingGroup='Dependent' and SettingName='Allow'
)Allow
) as a
left outer join ViewEmployeeInformation e on e.EmployeeId=a.EmployeeId
left outer join FiscalYearDetail f on f.Id=a.FiscalYearDetailId
Where 1=1 ";
                if (ProjectId != "0_0")
                    sqlText += " and e.ProjectId=@ProjectId";
                if (DepartmentId != "0_0")
                    sqlText += " and e.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0")
                    sqlText += " and e.SectionId=@SectionId ";
                if (DesignationId != "0_0")
                    sqlText += " and e.DesignationId=@DesignationId ";
                if (CodeF != "0_0")
                    sqlText += " and e.Code>= @CodeF";
                if (CodeT != "0_0")
                    sqlText += " and e.Code<= @CodeT";
                //if (Orderby == "DCG")
                //    sqlText += " order by deName, e.code, g.sl";
                //else if (Orderby == "DDC")
                //    sqlText += " order by d.Name, ej.JoinDate, ei.code";
                //else if (Orderby == "DGC")
                //    sqlText += " order by d.Name, g.sl, ei.code";
                //else if (Orderby == "DGDC")
                //    sqlText += " order by d.Name, g.sl, ej.JoinDate, ei.code";
                //else if (Orderby == "CODE")
                //    sqlText += ", ei.code";
                #endregion
                SqlCommand objComm = new SqlCommand();
                if (FiscalYearDetailId != "0_0")
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                if (ProjectId != "0_0")
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (DepartmentId != "0_0")
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (SectionId != "0_0")
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                if (DesignationId != "0_0")
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                if (CodeF != "0_0")
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                if (CodeT != "0_0")
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);



                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(ds);
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

        #region Backup Methods

        public List<SalarySheetVM> SalaryPreCalculationBackup(string FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Name, string dojFrom, string dojTo)
        {
            return null;
            #region Comments

            //////            #region Variables
            //////            SqlConnection currConn = null;
            //////            string sqlText = "";
            //////            List<SalarySheetVM> VMs = new List<SalarySheetVM>();
            //////            SalarySheetVM vm = new SalarySheetVM();
            //////            #endregion
            //////            try
            //////            {
            //////                #region open connection and transaction
            //////                currConn = _dbsqlConnection.GetConnection();
            //////                if (currConn.State != ConnectionState.Open)
            //////                {
            //////                    currConn.Open();
            //////                }
            //////                #endregion open connection and transaction
            //////                #region sql statement
            //////                sqlText = @"
            //////                            select PeriodName,Code,EmpName,Designation,JoinDate,Project,Department,Section
            //////                            ,SalaryHead,Amount
            //////                            ,ProjectId,DepartmentId,SectionId,DesignationId,FiscalYearDetailId,IsEarning
            //////                            from ViewSalaryPreCalculation
            //////                            where  1=1
            //////                            ";
            //////                sqlText += " and FiscalYearDetailId=@FiscalYearDetailsId";
            //////                if (ProjectId != "0_0")
            //////                    sqlText += " and ProjectId=@ProjectId";
            //////                if (DepartmentId != "0_0")
            //////                    sqlText += " and DepartmentId=@DepartmentId";
            //////                if (SectionId != "0_0")
            //////                    sqlText += " and SectionId=@SectionId";
            //////                if (DesignationId != "0_0")
            //////                    sqlText += " and DesignationId=@DesignationId";
            //////                if (!string.IsNullOrWhiteSpace(Name))
            //////                    sqlText += " and EmpName=@Name";
            //////                if (!string.IsNullOrWhiteSpace(CodeF))
            //////                    sqlText += " and Code>=@CodeF";
            //////                if (!string.IsNullOrWhiteSpace(CodeT))
            //////                    sqlText += " and Code<=@CodeT";
            //////                if (!string.IsNullOrWhiteSpace(dojFrom))
            //////                    sqlText += " and JoinDate>=@dojFrom";
            //////                if (!string.IsNullOrWhiteSpace(dojTo))
            //////                    sqlText += " and JoinDate<=@dojTo";
            //////                SqlCommand objComm = new SqlCommand();
            //////                objComm.Connection = currConn;
            //////                objComm.CommandText = sqlText;
            //////                objComm.CommandType = CommandType.Text;
            //////                objComm.Parameters.AddWithValue("@FiscalYearDetailsId", FiscalYearDetailsId);
            //////                if (ProjectId != "0_0" || !string.IsNullOrWhiteSpace(ProjectId) || ProjectId.Trim() != "")
            //////                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
            //////                if (DepartmentId != "0_0" || !string.IsNullOrWhiteSpace(DepartmentId))
            //////                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
            //////                if (SectionId != "0_0" || !string.IsNullOrWhiteSpace(SectionId))
            //////                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
            //////                if (DesignationId != "0_0" || !string.IsNullOrWhiteSpace(DesignationId))
            //////                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
            //////                if (!string.IsNullOrWhiteSpace(Name))
            //////                    objComm.Parameters.AddWithValue("@Name", Name);
            //////                if (!string.IsNullOrWhiteSpace(CodeF))
            //////                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
            //////                if (!string.IsNullOrWhiteSpace(CodeT))
            //////                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
            //////                if (!string.IsNullOrWhiteSpace(dojFrom))
            //////                    objComm.Parameters.AddWithValue("@dojFrom", dojFrom);
            //////                if (!string.IsNullOrWhiteSpace(dojTo))
            //////                    objComm.Parameters.AddWithValue("@dojTo", dojTo);
            //////                SqlDataReader dr;
            //////                dr = objComm.ExecuteReader();
            //////                while (dr.Read())
            //////                {
            //////                    vm = new SalarySheetVM();
            //////                    vm.PeriodName = dr["PeriodName"].ToString();
            //////                    vm.Code = dr["Code"].ToString();
            //////                    vm.EmpName = dr["EmpName"].ToString();
            //////                    vm.Designation = dr["Designation"].ToString();
            //////                    vm.JoinDate = Convert.ToDateTime(Ordinary.StringToDate(dr["JoinDate"].ToString()));
            //////                    vm.Project = dr["Project"].ToString();
            //////                    vm.Department = dr["Department"].ToString();
            //////                    vm.Section = dr["Section"].ToString();
            //////                    vm.SalaryHead = dr["SalaryHead"].ToString();
            //////                    vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
            //////                    vm.ProjectId = dr["ProjectId"].ToString();
            //////                    vm.DepartmentId = dr["DepartmentId"].ToString();
            //////                    vm.SectionId = dr["SectionId"].ToString();
            //////                    vm.DesignationId = dr["DesignationId"].ToString();
            //////                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
            //////                    vm.IsEarning = Convert.ToInt32(dr["IsEarning"].ToString());
            //////                    VMs.Add(vm);
            //////                }
            //////                dr.Close();
            //////                #endregion
            //////            }
            //////            #region catch
            //////            catch (SqlException sqlex)
            //////            {
            //////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //////            }
            //////            catch (Exception ex)
            //////            {
            //////                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            //////            }
            //////            #endregion
            //////            #region finally
            //////            finally
            //////            {
            //////                if (currConn != null)
            //////                {
            //////                    if (currConn.State == ConnectionState.Open)
            //////                    {
            //////                        currConn.Close();
            //////                    }
            //////                }
            //////            }
            //////            #endregion
            //////            return VMs;
            #endregion

        }
        #endregion

        public DataTable KazalMainTopGroupOT(SalarySheetVM vm)
        {
            DataTable dt = new DataTable();
            try
            {

                EmployeeMonthlyOvertimeDAL _dalEmployeeMonthlyOvertime = new EmployeeMonthlyOvertimeDAL();
                EmployeeMonthlyOvertimeVM vmEmployeeMonthlyOvertime = new EmployeeMonthlyOvertimeVM();

                vmEmployeeMonthlyOvertime.FiscalYearDetailId = Convert.ToInt32(vm.FiscalYearDetailId);
                vmEmployeeMonthlyOvertime.OrderBy = vm.Orderby;

                vmEmployeeMonthlyOvertime.ProjectIdList = vm.ProjectIdList;
                vmEmployeeMonthlyOvertime.Other3List = vm.Other3List;

                vmEmployeeMonthlyOvertime.DBName = vm.DBName;

                string[] conditionFields = { };//// { "ve.DepartmentId", "ve.ProjectId", "ve.SectionId" };
                string[] conditionValues = { };////  { departmentId, projectId, sectionId };


                dt = _dalEmployeeMonthlyOvertime.Report(vmEmployeeMonthlyOvertime, conditionFields, conditionValues);


                dt.Columns["OTAmount"].ColumnName = "NetSalary";

                {
                    var newSort = (from row in dt.AsEnumerable()
                                   group row by new
                                   {
                                       Project = row.Field<string>("Project")
                                       ,
                                       PeriodName = row.Field<string>("PeriodName")

                                   } into grp
                                   select new
                                   {
                                       Project = grp.Key.Project
                                       ,
                                       PeriodName = grp.Key.PeriodName
                                       ,
                                       NoOfEmployee = grp.Count()
                                       ,
                                       ////Gross = grp.Sum(r => r.Field<Decimal>("Gross"))
                                       ////,
                                       NetSalaryCash = grp.Sum(r => r.Field<Decimal>("NetSalary"))
                                        ,
                                       NetSalary = grp.Sum(r => r.Field<Decimal>("NetSalary"))


                                   }).ToList();
                    dt = Ordinary.ToDataTable(newSort);


                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Project"] = dr["Project"].ToString() + " OT";
                    }


                }



                return dt;
            }
            catch (Exception)
            {

                return dt;
            }
        }

        public DataTable SalarySheetMainTop(SalarySheetVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();


                //////////string PeriodName = dt.Rows[0]["PeriodName"].ToString();
                //////////string filename = "SalarySheet" + "-" + PeriodName;

                if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol" || CompanyName.ToLower() == "ssl")
                {
                    #region Kazal
                    #region Notes
                    ////////----------------Last Update - 08 November 2018--------------
                    ////////ReportId	    ReportName
                    ////////SalarySheet1	Full Salary --------------(Not Ready) 
                    ////////SalarySheet2	Bank Pay    --------------(Not Ready)	
                    ////////SalarySheet3	Cash Pay    --------------(Not Ready)	
                    ////////SalarySheet4	Bank Sheet  --------------(Ready)
                    #endregion

                    #region Hard Code


                    string MainTopGroup = "";

                    #endregion
                    vm.DBName = "KajolBrothersHRM";
                    #region KajolBrothersHRM

                    {
                        string MulitpleProjectId = "1_1,1_2"; //Corporate Office;;;Bangla Bazar
                        vm.ProjectIdList = MulitpleProjectId.Split(',').ToList();

                        #region Kazal Brothers (Salary)


                        dt = SalarySheet(vm);

                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["Project"] = dr["Project"] + " Salary";
                        }



                        dt.Columns.Add("MainTopGroup");

                        #endregion
                        #region Kazal Brothers (OT)

                        DataTable dtEmployeeMonthlyOvertime = new DataTable();

                        dtEmployeeMonthlyOvertime = KazalMainTopGroupOT(vm);

                        dt.Merge(dtEmployeeMonthlyOvertime);

                        #endregion

                        vm.ProjectIdList = null;
                        MainTopGroup = "Kazal Brothers";
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["MainTopGroup"] = MainTopGroup;
                        }
                    }
                    {
                        string MulitpleOther3 = "Chattogram and Sylhet,Dhaka 1,Dhaka 2,Rajshahi,Khulna,Barishal,Jashore,Rangpur"; //Chattogram and Sylhet,Dhaka 1,Dhaka 2,Rajshahi,Khulna,Barishal,Jashore,Rangpur
                        vm.Other3List = MulitpleOther3.Split(',').ToList();

                        #region Kazal Brothers (Salary - Marketing)

                        MainTopGroup = "Marketing";

                        vm.MainTopGroup = MainTopGroup;

                        DataTable dtKazalBrothers_Salary_Marketing = new DataTable();

                        dtKazalBrothers_Salary_Marketing = SalarySheet(vm);

                        dtKazalBrothers_Salary_Marketing.Columns.Add("MainTopGroup");
                        foreach (DataRow dr in dtKazalBrothers_Salary_Marketing.Rows)
                        {
                            dr["MainTopGroup"] = MainTopGroup;
                        }

                        dt.Merge(dtKazalBrothers_Salary_Marketing);

                        #endregion
                        vm.Other3List = null;

                        vm.MainTopGroup = "";
                    }
                    #endregion

                    vm.DBName = "AnupamPrintersHRM";
                    DataTable dtAnupamPrinters_Salary = new DataTable();

                    #region AnupamPrintersHRM
                    {
                        //////string MulitpleProjectId = "1_2"; //Matuail
                        //////vm.ProjectIdList = MulitpleProjectId.Split(',').ToList();

                        #region Anupam Printers (Salary)

                        dtAnupamPrinters_Salary = SalarySheet(vm);

                        foreach (DataRow dr in dtAnupamPrinters_Salary.Rows)
                        {
                            dr["Project"] = "Anupam Printers" + " Salary";
                        }




                        dtAnupamPrinters_Salary.Columns.Add("MainTopGroup");

                        #endregion
                        #region Anupam Printers (OT)

                        DataTable dtEmployeeMonthlyOvertime = new DataTable();

                        dtEmployeeMonthlyOvertime = KazalMainTopGroupOT(vm);

                        foreach (DataRow dr in dtEmployeeMonthlyOvertime.Rows)
                        {
                            dr["Project"] = "Anupam Printers" + " OT";
                        }

                        dtAnupamPrinters_Salary.Merge(dtEmployeeMonthlyOvertime);

                        #endregion

                        vm.ProjectIdList = null;
                        MainTopGroup = "Anupam Printers";
                        foreach (DataRow dr in dtAnupamPrinters_Salary.Rows)
                        {
                            dr["MainTopGroup"] = MainTopGroup;
                        }
                    }

                    #endregion

                    dt.Merge(dtAnupamPrinters_Salary);


                    #endregion
                }

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
            return dt;
        }

        public DataTable SalarySheet(SalarySheetVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                ds = SalaryPreCalculationNew(vm);

                dt = ds.Tables[0];
                if (CompanyName == "brac"
                    || CompanyName.ToLower() == "tib"
                    )
                {
                    #region Brac
                    #region Notes
                    ////////----------------Last Update - 25 February 2019--------------
                    ////////ReportId	    ReportName
                    ////////SalarySheet1	Full Salary Sheet
                    ////////SalarySheet2	Bank Pay
                    ////////SalarySheet3	Salary Summary
                    ////////SalarySheet5	Pay Slip
                    ////////SalarySheet6	pay Slip (Email)
                    ////////SalarySheet7	Salary Certificate
                    ////////SalarySheet8	Full Salary Sheet (Decimal)
                    #endregion
                    #region Brac Excel Salary Sheet (Full Salary Sheet)
                    #region Column Selection

                    string[] shortColumnName = {           
  "FiscalYearDetailId"                                  //01
, "EmployeeId"                                          //02
, "Code"                                                //03
, "EmpName"                                             //04
, "PermanentStatus"                                     //05
, "JoinDate"                                            //06
, "BankName"                                            //07
, "Department"                                          //08
, "Other1"                                              //09
, "Section"                                             //10
, "Project"                                             //11
, "BankAccountNo"                                       //12

, "Basic"                                               //21
, "HouseRent"                                           //22
, "Medical"                                             //23
, "Conveyance"                                          //24
, "Gross"                                               //25
, "Arrear"                                              //26
, "ReimbursableExpense"                                 //27
, "PreEmploymentCheckUp"                                //28
, "OtherAllowanceMonthly"                               //29
, "MobileBill"                                          //30
, "TAX"                                                 //31
, "AdvanceDeduction"                                    //32
, "PFEmployee"                                          //33
, "TotalLoan"                                           //34
, "OtherDeductionMonthly"                               //35
, "DOM"                                                 //36
, "PayDays"                                             //37
, "NPDays"                                              //38
, "SalaryDeduction"                                     //40
, "TransportBill"                                       //41
, "LeaveWOPayOD"                                          //42
, "OtherDeductionAmount"                                          //43

                                               };
                    Type[] columnTypes = {                  
  typeof(String)                                        //01
, typeof(String)                                        //02
, typeof(String)                                        //03
, typeof(String)                                        //04
, typeof(String)                                        //05
, typeof(String)                                        //06
, typeof(String)                                        //07
, typeof(String)                                        //08
, typeof(String)                                        //09
, typeof(String)                                        //10
, typeof(String)                                        //11
, typeof(string)                                        //12

, typeof(decimal)                                       //21
, typeof(decimal)                                       //22
, typeof(decimal)                                       //23
, typeof(decimal)                                       //24
, typeof(decimal)                                       //25
, typeof(decimal)                                       //26
, typeof(decimal)                                       //27
, typeof(decimal)                                       //28
, typeof(decimal)                                       //29
, typeof(decimal)                                       //30
, typeof(decimal)                                       //31
, typeof(decimal)                                       //32
, typeof(decimal)                                       //33
, typeof(decimal)                                       //34
, typeof(decimal)                                       //35
, typeof(int)                                           //36
, typeof(decimal)                                       //37
, typeof(int)                                           //38
, typeof(decimal)                                       //40
, typeof(decimal)                                       //41
, typeof(decimal)                                       //42
, typeof(decimal)                                       //43
                                         };


                    dt = Ordinary.DtSetColumnsOrder(dt, shortColumnName);
                    dt = Ordinary.DtSelectedColumn(dt, shortColumnName, columnTypes);

                    ////////if (Session["LabelOther1"].ToString() != "")
                    ////////{
                    ////////    dt = Ordinary.DtColumnNameChange(dt, "Other1", Session["LabelOther1"].ToString());
                    ////////}

                    #endregion
                    dt.Columns.Add("OtherDeduction", typeof(decimal));
                    dt.Columns.Add("TotalEarning", typeof(decimal));
                    dt.Columns.Add("TotalDeduction", typeof(decimal));
                    dt.Columns.Add("NetSalary", typeof(decimal));
                    #region Declarations
                    decimal vBasic = 0;
                    decimal vHouseRent = 0;
                    decimal vMedical = 0;
                    decimal vConveyance = 0;
                    decimal vGross = 0;
                    decimal vDOM = 0;
                    decimal vPayDays = 0;
                    decimal vArrear = 0;

                    decimal vReimbursableExpense = 0;


                    decimal vPreEmploymentCheckUp = 0;
                    decimal vAdvanceDeduction = 0;
                    decimal vTAX = 0;
                    decimal vPFEmployee = 0;
                    decimal vMobileBill = 0;
                    decimal vTotalLoan = 0;
                    decimal vTotalEarning = 0;
                    decimal vTotalDeduction = 0;
                    decimal vNPDays = 0;

                    decimal vOtherAllowanceMonthly = 0;
                    decimal vOtherDeductionMonthly = 0;
                    decimal vSalaryDeduction = 0;
                    decimal vTransportBill = 0;
                    decimal vLeaveWOPayOD = 0;

                    decimal vOtherDeductionAmount = 0;

                    decimal vTotalOtherDeduction = 0;


                    #endregion

                    #region Settings
                    SettingDAL _settingDAL = new SettingDAL();
                    int decimalPlace = Convert.ToInt32(_settingDAL.settingValue("SalarySheet", "DecimalPlace"));
                    #endregion

                    int i = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        #region Variables
                        vBasic = 0;
                        vHouseRent = 0;
                        vMedical = 0;
                        vConveyance = 0;
                        vGross = 0;
                        vDOM = 0;
                        vPayDays = 0;
                        vArrear = 0;
                        vReimbursableExpense = 0;

                        vPreEmploymentCheckUp = 0;
                        vAdvanceDeduction = 0;
                        vTAX = 0;
                        vPFEmployee = 0;
                        vMobileBill = 0;
                        vTotalLoan = 0;
                        vTotalEarning = 0;
                        vTotalDeduction = 0;
                        vNPDays = 0;

                        vOtherAllowanceMonthly = 0;
                        vOtherDeductionMonthly = 0;
                        vSalaryDeduction = 0;
                        vTransportBill = 0;
                        vOtherDeductionAmount = 0;
                        vLeaveWOPayOD = 0;
                        vTotalOtherDeduction = 0;


                        #endregion
                        #region Value Assign

                        vBasic = Convert.ToDecimal(dt.Rows[i]["Basic"]);
                        vHouseRent = Convert.ToDecimal(dt.Rows[i]["HouseRent"]);
                        vMedical = Convert.ToDecimal(dt.Rows[i]["Medical"]);
                        vConveyance = Convert.ToDecimal(dt.Rows[i]["Conveyance"]);
                        vGross = Convert.ToDecimal(dt.Rows[i]["Gross"]);
                        vDOM = Convert.ToDecimal(dt.Rows[i]["DOM"]);
                        vPayDays = Convert.ToDecimal(dt.Rows[i]["PayDays"]);
                        vArrear = Convert.ToDecimal(dt.Rows[i]["Arrear"]);
                        vReimbursableExpense = Convert.ToDecimal(dt.Rows[i]["ReimbursableExpense"]);

                        vPreEmploymentCheckUp = Convert.ToDecimal(dt.Rows[i]["PreEmploymentCheckUp"]);
                        vAdvanceDeduction = Convert.ToDecimal(dt.Rows[i]["AdvanceDeduction"]);
                        vTAX = Convert.ToDecimal(dt.Rows[i]["TAX"]);
                        vPFEmployee = Convert.ToDecimal(dt.Rows[i]["PFEmployee"]);
                        vMobileBill = Convert.ToDecimal(dt.Rows[i]["MobileBill"]);
                        vTotalLoan = Convert.ToDecimal(dt.Rows[i]["TotalLoan"]);
                        vNPDays = Convert.ToDecimal(dt.Rows[i]["NPDays"]);

                        vOtherAllowanceMonthly = Convert.ToDecimal(dt.Rows[i]["OtherAllowanceMonthly"]);
                        vOtherDeductionMonthly = Convert.ToDecimal(dt.Rows[i]["OtherDeductionMonthly"]);

                        vSalaryDeduction = Convert.ToDecimal(dt.Rows[i]["SalaryDeduction"]);
                        vTransportBill = Convert.ToDecimal(dt.Rows[i]["TransportBill"]);

                        vLeaveWOPayOD = Convert.ToDecimal(dt.Rows[i]["LeaveWOPayOD"]);

                        vOtherDeductionAmount = Convert.ToDecimal(dt.Rows[i]["OtherDeductionAmount"]);

                        #endregion
                        #region Calculation
                        if (vNPDays > 0)
                        {
                            vGross = vGross / vDOM * vPayDays;
                            dt.Rows[i]["Basic"] = vBasic / vDOM * vPayDays;
                            dt.Rows[i]["HouseRent"] = vHouseRent / vDOM * vPayDays;
                            dt.Rows[i]["Medical"] = vMedical / vDOM * vPayDays;
                            dt.Rows[i]["Conveyance"] = vConveyance / vDOM * vPayDays;
                            dt.Rows[i]["Gross"] = vGross;

                        }

                        vTotalOtherDeduction = vOtherDeductionAmount + vSalaryDeduction + vTransportBill + vOtherDeductionMonthly + vLeaveWOPayOD;

                        vTotalEarning = vGross + vArrear + vReimbursableExpense + vPreEmploymentCheckUp + vOtherAllowanceMonthly;
                        vTotalDeduction = vAdvanceDeduction + vTAX + vPFEmployee + vMobileBill + vTotalLoan + vTotalOtherDeduction;

                        dt.Rows[i]["OtherDeduction"] = vTotalOtherDeduction;
                        dt.Rows[i]["TotalEarning"] = vTotalEarning;
                        dt.Rows[i]["TotalDeduction"] = vTotalDeduction;
                        dt.Rows[i]["NetSalary"] = vTotalEarning - vTotalDeduction;

                        #endregion
                        i++;
                    }

                    #region Make Reday Excel Sheet Data

                    if (vm.SheetName == "SalarySheet1" || vm.SheetName == "SalarySheet8")////Full Salary Sheet ------
                    {

                        string[] removeColumnName = { "FiscalYearDetailId", "EmployeeId", "DOM", "PayDays", "NPDays", "BankAccountNo", "SalaryDeduction", "TransportBill", "OtherDeductionMonthly", "LeaveWOPayOD", "Department", "PermanentStatus", "JoinDate", "BankName", "Project" };
                        dt = Ordinary.DtDeleteColumns(dt, removeColumnName);
                    }
                    else if (vm.SheetName == "SalarySheet2") //--------Bank Pay
                    {
                        {
                            string[] shortColumnNameNew = { "EmpName", "BankAccountNo", "NetSalary" };
                            Type[] columnTypesNew = { typeof(String), typeof(String), typeof(decimal) };

                            dt = Ordinary.DtSetColumnsOrder(dt, shortColumnNameNew);
                            dt = Ordinary.DtSelectedColumn(dt, shortColumnNameNew, columnTypesNew);

                        }
                    }
                    else if (vm.SheetName == "SalarySheet3") //--------Salary Summary
                    {
                        {
                            var newSort = (from row in dt.AsEnumerable()
                                           group row by new
                                           {
                                               Department = row.Field<string>("Department")
                                               ,
                                               Section = row.Field<string>("Section")
                                           } into grp
                                           select new
                                           {
                                               Department = grp.Key.Department,
                                               Section = grp.Key.Section
                                               ,
                                               Gross = grp.Sum(r => r.Field<Decimal>("Gross"))
                                               ,
                                               Arrear = grp.Sum(r => r.Field<Decimal>("Arrear"))
                                               ,
                                               ReimbursableExpense = grp.Sum(r => r.Field<Decimal>("ReimbursableExpense"))
                                               ,
                                               PreEmploymentCheckUp = grp.Sum(r => r.Field<Decimal>("PreEmploymentCheckUp"))
                                               ,
                                               OtherAllowanceMonthly = grp.Sum(r => r.Field<Decimal>("OtherAllowanceMonthly"))
                                               ,
                                               TotalEarning = grp.Sum(r => r.Field<Decimal>("TotalEarning"))
                                               ,
                                               MobileBill = grp.Sum(r => r.Field<Decimal>("MobileBill"))
                                               ,
                                               TAX = grp.Sum(r => r.Field<Decimal>("TAX"))
                                               ,
                                               AdvanceDeduction = grp.Sum(r => r.Field<Decimal>("AdvanceDeduction"))
                                               ,
                                               PFEmployee = grp.Sum(r => r.Field<Decimal>("PFEmployee"))
                                               ,
                                               TotalLoan = grp.Sum(r => r.Field<Decimal>("TotalLoan"))
                                               ,
                                               OtherDeduction = grp.Sum(r => r.Field<Decimal>("OtherDeduction"))
                                               ,
                                               TotalDeduction = grp.Sum(r => r.Field<Decimal>("TotalDeduction"))
                                               ,
                                               NetSalary = grp.Sum(r => r.Field<Decimal>("NetSalary"))


                                           }).ToList();

                            dt = new DataTable();
                            dt = Ordinary.ToDataTable(newSort);
                        }
                    }
                    else
                    {
                        ////////dt = new DataTable();
                        ////////Session["result"] = "Fail" + "~" + "This Sheet is Not Available in Excel";
                        ////////return Redirect("SalarySheet");
                    }

                    #endregion
                    #region Column Name Chagne
                    ////dt = Ordinary.DtColumnNameChange(dt, "EmpName", "Name");
                    ////dt = Ordinary.DtColumnNameChange(dt, "Other1", "Designation");
                    ////dt = Ordinary.DtColumnNameChange(dt, "PreEmploymentCheckUp", "Pre-Employment Check Up");
                    ////dt = Ordinary.DtColumnNameChange(dt, "MobileBill", "Excess Mobile Bill");



                    #region Change Column Name

                    //////string[] DtcolumnName = new string[dt.Columns.Count];
                    //////int j = 0;
                    //////foreach (DataColumn column in dt.Columns)
                    //////{
                    //////    DtcolumnName[j] = column.ColumnName;
                    //////    j++;
                    //////}

                    //////for (int k = 0; k < DtcolumnName.Length; k++)
                    //////{
                    //////    dt = Ordinary.DtColumnNameChange(dt, DtcolumnName[k], Ordinary.AddSpacesToSentence(DtcolumnName[k]));
                    //////}

                    ////dt = Ordinary.DtColumnNameChange(dt, "Code", "BESL ID");
                    ////dt = Ordinary.DtColumnNameChange(dt, "Section", "Cost Center");
                    ////dt = Ordinary.DtColumnNameChange(dt, "PFEmployee", "PF By Employee");

                    #endregion




                    #endregion


                    #region Sl
                    dt.Columns.Add("Sl", typeof(int)).SetOrdinal(0);
                    {
                        int Sl = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["Sl"] = Sl;
                            Sl++;
                        }
                    }
                    #endregion
                    #endregion
                    #endregion
                }
                else if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol" || CompanyName.ToLower() == "ssl")
                {
                    #region Kazal
                    #region Notes
                    ////////----------------Last Update - 08 November 2018--------------
                    ////////ReportId	    ReportName
                    ////////SalarySheet1	Full Salary --------------(Not Ready) 
                    ////////SalarySheet2	Bank Pay    --------------(Not Ready)	
                    ////////SalarySheet3	Cash Pay    --------------(Not Ready)	
                    ////////SalarySheet4	Bank Sheet  --------------(Ready)
                    #endregion

                    //dt = dtSalarySheetKazol(dt);

                    #region Make Reday Excel Sheet Data
                    string singleSheetName = "";
                    if (vm.SheetName == "SalarySheet1" || vm.SheetName == "SalarySheet7" || vm.SheetName == "SalarySheet8" || vm.SheetName == "SalarySheet5" || vm.SheetName == "SalarySheet6") //--------Full Salary //Summary Salary//Summary Salary Marketing
                    {
                        singleSheetName = "SalarySheet1";
                    }

                    if (singleSheetName == "SalarySheet1") //--------Full Salary //Summary Salary//Summary Salary Marketing
                    {
                        {
                            //////DataTable dtTemp = new DataTable();
                            //////dtTemp = dt;

                            //////dt = new DataTable();
                            //////dt = dtSalarySheetKazol(dtTemp);

                            //////dt.Clear();
                            //////dt.Merge(dt);

                        }
                    }

                    else if (vm.SheetName == "SalarySheet4" || vm.SheetName == "SalarySheet2") //--------Bank Sheet
                    {
                        {

                            DataTable dtTemp = new DataTable();
                            dtTemp = dt;

                            EnumerableRowCollection<DataRow> dtBankSheet = dtTemp.AsEnumerable().Where(r => r.Field<decimal>("NetSalaryBank") >= 0);

                            if (dtBankSheet == null || dtBankSheet.Count() == 0)
                            {
                                dt = new DataTable();

                                ////////Session["result"] = "Fail" + "~" + "No Employee For Bank Sheet";
                                ////////return Redirect("SalarySheet");
                            }

                            dt = dtBankSheet.CopyToDataTable();

                            string[] shortColumnNameNew = { "Code", "EmpName", "BankAccountNo", "Designation", "NetSalaryBank" };
                            Type[] columnTypesNew = { typeof(String), typeof(String), typeof(String), typeof(String), typeof(decimal) };

                            dt = Ordinary.DtSetColumnsOrder(dt, shortColumnNameNew);
                            dt = Ordinary.DtSelectedColumn(dt, shortColumnNameNew, columnTypesNew);



                            ////DataTable dtTemp = new DataTable();
                            //////dtTemp = dt;

                            //////dt = new DataTable();
                            //////dt = dtSalarySheetKazol(dt);

                            //////var dtBankSheet = dt.AsEnumerable().Where(r => r.Field<decimal>("NetSalaryBank") > 0);

                            ////////if (dtBankSheet == null || dtBankSheet.Count() == 0)
                            ////////{
                            ////////    Session["result"] = "Fail" + "~" + "No Employee For Bank Sheet";
                            ////////    return Redirect("SalarySheet");
                            ////////}

                            dt.Clear();
                            dt.Merge(dtBankSheet.CopyToDataTable());


                        }
                    }
                    else if (vm.SheetName == "SalarySheet") // Cash Pay
                    {
                        {
                            //////DataTable dtTemp = new DataTable();
                            //////dtTemp = dt;

                            //////dt = new DataTable();
                            //////dt = dtSalarySheetKazol(dtTemp);

                            EnumerableRowCollection<DataRow> dtCashPay = dt.AsEnumerable().Where(r => r.Field<decimal>("CashPayGross") > 0);

                            if (dtCashPay == null || dtCashPay.Count() == 0)
                            {
                                dt = new DataTable();
                                //////Session["result"] = "Fail" + "~" + "No Employee For Cash Pay";
                                //////return Redirect("SalarySheet");
                            }
                            else
                            {
                                DataTable dtCashPayNew = new DataTable();
                                dtCashPayNew = dtCashPay.CopyToDataTable();

                                dt.Clear();
                                dt.Merge(dtCashPayNew);
                            }

                        }
                    }

                    else if (vm.SheetName == "SalarySheet100") //--------Main Top
                    {
                        #region SalarySheet100--------Main Top

                        #region MainTopGroup - Marketing

                        if (vm.MainTopGroup == "Marketing")
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                string Other3 = dr["Other3"].ToString();
                                if (Other3 == "Dhaka 1" || Other3 == "Dhaka 2")
                                {
                                    dr["Project"] = "Dhaka";
                                }
                                else if (Other3 == "Chattogram and Sylhet")
                                {
                                    dr["Project"] = "Chattogram";
                                }
                                else if (Other3 == "Khulna" || Other3 == "Jashore" || Other3 == "Barishal")
                                {
                                    dr["Project"] = "Khulna";
                                }
                                else if (Other3 == "Rajshahi" || Other3 == "Rangpur")
                                {
                                    dr["Project"] = "Rajshahi";
                                }
                            }
                        }
                        #endregion
                        {
                            var newSort = (from row in dt.AsEnumerable()
                                           group row by new
                                           {
                                               Project = row.Field<string>("Project")
                                               ,
                                               PeriodName = row.Field<string>("PeriodName")

                                           } into grp
                                           select new
                                           {
                                               Project = grp.Key.Project
                                               ,
                                               PeriodName = grp.Key.PeriodName
                                               ,
                                               NoOfEmployee = grp.Count()
                                               ,
                                               Gross = grp.Sum(r => r.Field<Decimal>("Gross"))
                                               ,
                                               AbsentDeduction = grp.Sum(r => r.Field<Decimal>("AbsentDeduction"))
                                               ,
                                               LeaveWOPay = grp.Sum(r => r.Field<Decimal>("LeaveWOPay"))
                                               ,
                                               AdvanceDeduction = grp.Sum(r => r.Field<Decimal>("AdvanceDeduction"))
                                               ,
                                               TotalLoan = grp.Sum(r => r.Field<Decimal>("TotalLoan"))
                                               ,
                                               Tax = grp.Sum(r => r.Field<Decimal>("Tax"))
                                               ,
                                               OtherDeduction = grp.Sum(r => r.Field<Decimal>("OtherDeduction"))
                                               ,
                                               OtherEarning = grp.Sum(r => r.Field<Decimal>("OtherEarning"))
                                               ,
                                               NetSalaryBank = grp.Sum(r => r.Field<Decimal>("NetSalaryBank"))
                                               ,
                                               NetSalaryCash = grp.Sum(r => r.Field<Decimal>("NetSalaryCash"))


                                               ,
                                               NetSalary = grp.Sum(r => r.Field<Decimal>("NetSalary"))


                                           }).ToList();
                            dt = Ordinary.ToDataTable(newSort);
                        }
                        #endregion
                    }
                    else
                    {
                        // dt = new DataTable();
                        ////////Session["result"] = "Fail" + "~" + "This Sheet is Not Available in Excel";
                        ////////return Redirect("SalarySheet");
                    }

                    #endregion
                    #endregion
                }
            }
            #endregion

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

            return dt;
        }



        //////public DataSet SalaryPreCalculationNew(string FiscalYearDetailId, string ProjectId, string DepartmentId
        //////    , string SectionId, string DesignationId, string CodeF, string CodeT
        //////    , string Orderby, string other1, string other2, string other3, string BankId
        //////    , List<string> ProjectIdList = null
        //////    , List<string> Other3List = null, string DBName = "", string HoldStatus = ""
        //////    )

        public DataSet SalaryPreCalculationNew(SalarySheetVM vm)
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

                if (!string.IsNullOrWhiteSpace(vm.DBName))
                {
                    currConn.ChangeDatabase(vm.DBName);
                }
                #endregion open connection and transaction

                if (vm.FiscalYearDetailIdTo == 0)
                {
                    vm.FiscalYearDetailIdTo = vm.FiscalYearDetailId;
                }

                #region sql statement

                #region SqlText

                #region Primary SqlText


                sqlText = @"
--------declare @FiscalYearDetailId as int
--------set @FiscalYearDetailId = 1047
--------
--------declare @FiscalYearDetailIdTo as int
--------set @FiscalYearDetailIdTo = 1047


declare @vDOM as varchar(10);
set @vDOM = 'DOM';

select @vDOM = SettingValue From Setting Where SettingGroup = 'DOM' and SettingName = 'DOM'
";

                sqlText += @" 
                SELECT * INTO #TempTable  FROM
                  (
                ";

                sqlText += @"
SELECT 
ISNULL(se.IsHold,0)																		AS IsHold
,case when ISNULL(se.IsHold,0) = 1 then 'Hold' else 'Not Hold' end						AS HoldStatus
, ISNULL(desig.PriorityLevel,0)															AS PriorityLevel
,isnull( sal.LeaveWOPay,0)																AS LeaveWOPayOD
,isnull(ej.Other1,'NA')																	AS Other1
,isnull(ej.Other2,'NA')																	AS Other2
,isnull(ej.Other3,'NA')																	AS Other3
,isnull(ej.Other4,'NA')																	AS Other4
,isnull(ej.Other5,'NA')																	AS Other5
,case 																					------------------------------
when @vDOM = 'DOM' 																		------------------------------
then ISNULL(CONVERT(int, fyd.PeriodEnd)- convert(int,fyd.PeriodStart)+1,0)				------------------------------
when  @vDOM != 'DOM' then @vDOM end														AS DOM
,ISNULL(bk.Name,'NA')																	AS BankName
,ISNULL(bk.Code,'NA')																	AS BankCode
,ISNULL(ej.BankAccountNo,'NA')															AS BankAccountNo
,isnull(ej.BankPayAmount,0)																AS BankPayAmount
,case when isnull(ej.IsPermanent, 0) = 1 then 'Permanent' else 'Not Permanent' end		AS PermanentStatus
, Convert(VARCHAR(50),Convert(Datetime, ISNULL(NULLIF (ej.JoinDate, N''), N'19000101')),101) AS JoinDate
,0																						AS Bonus
,0																						AS SL
,0																						AS CL
,0																						AS EL
,0																						AS COL
,0																						AS ML
,0																						AS PL
,0																						AS STL
,0																						AS HoliDays
,0																						AS workingDays
,0																						AS EarningDays
,isnull(ma.LWPDay,0)																	AS LWP
,isnull(ma.LWPAmount,0)																	AS LeaveWOPay_ma
,isnull(ma.AbsentDay,0)																	AS AbsentDays
,isnull(ma.AbsentAmount,0)																AS AbsentAmount
,isnull(ma.NPAmount,0)																	AS NPAmount
,isnull(ma.NPDay,0)																		AS NPDays
--,ISNULL(CONVERT(int, fyd.PeriodEnd)- convert(int,fyd.PeriodStart)+1,0) 				
---(isnull(ma.NPDay,0)+isnull(ma.LWPDay,0)+isnull(ma.AbsentDay,0))						AS PayDays
,(isnull(ma.DOM,0)-isnull(ma.LWPDay,0)-isnull(ma.AbsentDay,0)
-isnull(ma.LateAbsentDay,0))															AS PayDays
,isnull(ma.LateAbsentDay,0) 															AS LateDay
,isnull(ma.LateAmount,0)																AS LateAmount
,pad.ProjectAllocationId																AS ProjectAllocationId
,pad.Portion1																			AS Portion1
,pad.Portion2																			AS Portion2
,pad.Portion3																			AS Portion3
,pad.Portion4																			AS Portion4
,epd.Email																				AS EmpEmail
, PeriodName																			AS PeriodName
, ei.Code																				AS Code
, ei.Salutation_E																		AS Salutation_E
, ei.MiddleName																			AS MiddleName
, ei.LastName																			AS LastName
, ISNULL(LTRIM(RTRIM(ei.Salutation_E)), N'') 											------------------------------
+ ' ' + ISNULL(LTRIM(RTRIM(ei.MiddleName)), N'') 										------------------------------
+ ' ' + ISNULL(LTRIM(RTRIM(ei.LastName)), N'')											AS EmpName
, ISNULL(NULLIF (ej.ProbationEnd, N''), N'19000101') AS ProbationEnd					------------------------------
, ISNULL(NULLIF (ej.DateOfPermanent, N''), N'19000101')									AS DateOfPermanent 
,ISNULL(NULLIF (ej.LeftDate, N''), N'19000101')											AS LeftDate
,desig.Name																				AS Designation
,p.Name																					AS Project
,d.Name																					AS Department
,s.name																					AS Section
,ltrim(rtrim(g.Name))  +'-'+ltrim(rtrim(stp.SL))										AS Grade
, sal.*																					------------------------------
,[Basic]+HouseRent+Medical+Conveyance+ChildAllowance+Arrear+OverTime+Entertainment		AS GrossPayForJournalCampe
,[Basic]+HouseRent+Medical+Conveyance+ChildAllowance+Arrear+OverTime+Entertainment		AS NetSalaryBank
,(																						------------------------------
isnull(TransportBill,0)																	------------------------------
+isnull(CarLoan,0)+isnull(CarLoanInterest,0)+isnull(EducationLoan,0)					------------------------------
+isnull(EducationLoanInterest,0)														------------------------------
+isnull(PFEmployee,0)+isnull(HouseLoan,0)+isnull(HouseLoanInterest,0)					------------------------------
+isnull(HouseRentDeduction,0)															------------------------------
+isnull(TAX,0)																			------------------------------
+isnull(LeaveWOPay,0)																	------------------------------
+isnull(Loan,0)+isnull(LoanInterest,0)+isnull(MedicalLoan,0)+							------------------------------
+isnull(MedicalLoanInterest,0)+isnull(OtherLoan,0)+isnull(OtherLoanInterest,0)			------------------------------
+isnull(PFLoan,0)+isnull(PFLoanInterest,0)+isnull(SalaryDeduction,0)					------------------------------
+isnull(TravelLoan,0)+isnull(TravelLoanInterest,0)										------------------------------
)																						AS TotalDeductionForJournalCampe
,(																						------------------------------
([Basic]+HouseRent+Medical+Conveyance+ChildAllowance+Arrear+OverTime+Entertainment)		------------------------------
-(isnull(TransportBill,0)																------------------------------
+isnull(CarLoan,0)+isnull(CarLoanInterest,0)+isnull(EducationLoan,0)					------------------------------
+isnull(EducationLoanInterest,0)														------------------------------
+isnull(PFEmployee,0)+isnull(HouseLoan,0)+isnull(HouseLoanInterest,0)					------------------------------
+isnull(HouseRentDeduction,0)+isnull(TAX,0)												------------------------------
+isnull(LeaveWOPay,0)																	------------------------------
+isnull(Loan,0)+isnull(LoanInterest,0)+isnull(MedicalLoan,0)+							------------------------------
+isnull(MedicalLoanInterest,0)+isnull(OtherLoan,0)										------------------------------
+isnull(OtherLoanInterest,0)															------------------------------
+isnull(PFLoan,0)+isnull(PFLoanInterest,0)+isnull(SalaryDeduction,0)					------------------------------
+isnull(TravelLoan,0)+isnull(TravelLoanInterest,0))										------------------------------
)																						AS NetPayForJournalCampe
,isnull(PFEmployee,0)+isnull(PFEmployer,0)+isnull(PFLoan,0)								------------------------------
+isnull(PFLoanInterest,0)	AS PFForJournalCampe										------------------------------
,TAX																					AS TAXForJournalCampe
,AdvanceDeduction																		AS AdvanceDeductionForJournalCampe
,LeaveWOPay																				AS LeaveWOPayForJournalCampe
,(																						------------------------------
isnull(CarLoan,0)+isnull(CarLoanInterest,0)												------------------------------
+isnull(EducationLoan,0)+isnull(EducationLoanInterest,0)								------------------------------
+isnull(HouseLoan,0)+isnull(HouseLoanInterest,0)										------------------------------
+isnull(Loan,0)+isnull(LoanInterest,0)													------------------------------
+isnull(MedicalLoan,0)++isnull(MedicalLoanInterest,0)									------------------------------
+isnull(OtherLoan,0)+isnull(OtherLoanInterest,0)										------------------------------
+isnull(PFLoan,0)+isnull(PFLoanInterest,0)												------------------------------
+isnull(TravelLoan,0)+isnull(TravelLoanInterest,0)										------------------------------
+isnull(SalaryLoan,0)+isnull(SalaryLoanInterest,0)										------------------------------
)																						AS TotalLoan
,TransportBill																			AS TransportBillForJournalCampe
FROM(																					------------------------------
SELECT 																					------------------------------
EmployeeId																				AS EmployeeId
,isnull(OtherEarning,0)	+isnull(LeaveEncash,0)										    AS OtherEarning
,isnull(OtherDeduction,0) +isnull(ConveyanceD,0)+isnull(Lunch,0)+isnull(AbsentOrLate,0)	AS OtherDeductionAmount
,isnull(OtherAllowanceMonthly,0)														AS OtherAllowanceMonthly
,isnull(OtherDeductionMonthly,0)														AS OtherDeductionMonthly
,FiscalYearDetailId																		AS FiscalYearDetailId
,ProjectId																				AS ProjectId
,DepartmentId																			AS DepartmentId
,SectionId																				AS SectionId
,DesignationId																			AS DesignationId
,EmployeeStatus																			AS EmployeeStatus
,GradeId																				AS GradeId
,isnull( Arrear,0)																		AS Arrear
,isnull([Basic],0)																		AS [Basic]
,isnull([Basic],0)																		AS [BasicSalary]
,isnull(CarLoan,0)																		AS CarLoan
,isnull(CarLoanInterest,0)																AS CarLoanInterest
,isnull(ChildAllowance,0)																AS ChildAllowance
,isnull(Conveyance,0)																	AS Conveyance
,isnull(Conveyance,0)																	AS ConveyanceAllowance
,isnull(EducationLoan,0)																AS EducationLoan
,isnull(EducationLoanInterest,0)														AS EducationLoanInterest
,isnull(Entertainment,0)																AS Entertainment
,isnull(Gross,0)																		AS Gross
,isnull(HouseLoan,0)																	AS HouseLoan
,isnull(HouseLoanInterest,0)															AS HouseLoanInterest
,isnull(HouseRent,0)																	AS HouseRent
,isnull(HouseRentDeduction,0)															AS HouseRentDeduction
,isnull(Loan,0)																			AS Loan
,isnull(LoanInterest,0)																	AS LoanInterest
,isnull(Medical,0)																		AS Medical
,isnull(MedicalLoan,0)																	AS MedicalLoan
,isnull(MedicalLoanInterest,0)															AS MedicalLoanInterest
,isnull(MobileBill,0)																	AS MobileBill
,isnull(OtherLoan,0)																	AS OtherLoan
,isnull(OtherLoanInterest,0)															AS OtherLoanInterest
,isnull(OverTime,0)																		AS OverTime
,isnull(PFEmployee,0)																	AS PFEmployee
,isnull(PFEmployer,0)																	AS PFEmployer
,isnull(PFLoan,0)																		AS PFLoan
,isnull(PFLoanInterest,0)																AS PFLoanInterest
,isnull(ReimbursableExpense,0)															AS ReimbursableExpense
,isnull(PreEmploymentCheckUp,0)															AS PreEmploymentCheckUp
,isnull(LeaveWOPay,0)																	AS LeaveWOPay
,isnull(AbsentDeduction,0)																AS AbsentDeduction
,isnull(AdvanceDeduction,0)																AS AdvanceDeduction
,isnull(LeaveEncash,0)																	AS LeaveEncash
,isnull(SalaryDeduction,0)																AS SalaryDeduction
,isnull(TAX,0)																			AS TAX
,isnull(Tiffin,0)																		AS Tiffin
,isnull(TransportBill,0)																AS TransportBill
,isnull(TravelLoan,0)																	AS TravelLoan
,isnull(TravelLoanInterest,0)															AS TravelLoanInterest
,isnull(SalaryLoan,0)																	AS SalaryLoan
,isnull(SalaryLoanInterest,0)															AS SalaryLoanInterest
,isnull(Basic_B,0)																		AS Basic_B
,isnull(Medical_B,0)																	AS Medical_B
,isnull(Conveyance_B,0)																	AS Conveyance_B
,isnull(House_Rent_B,0)																	AS House_Rent_B
,isnull(Punishment,0)																	AS Punishment

from (select EmployeeId
,FiscalYearDetailId 
,ProjectId
,DepartmentId
,SectionId
,DesignationId 
,EmployeeStatus
,GradeId
, isnull(amount,0)amount
, SalaryHead
from ViewSalaryPreCalculation) x pivot 
(sum(amount)for SalaryHead in (
OtherEarning
,OtherDeduction
,OtherAllowanceMonthly
,OtherDeductionMonthly
,Arrear
,[Basic]
,CarLoan
,CarLoanInterest
,ChildAllowance
,Conveyance
,EducationLoan
,EducationLoanInterest
,Entertainment
,Gross
,HouseLoan
,HouseLoanInterest
,HouseRent
,HouseRentDeduction
,Loan
,LoanInterest
,Medical
,MedicalLoan
,MedicalLoanInterest
,MobileBill
,OtherLoan
,OtherLoanInterest
,OverTime
,PFEmployee
,PFEmployer
,PFLoan
,PFLoanInterest
,SalaryLoan
,SalaryLoanInterest
,ReimbursableExpense
,PreEmploymentCheckUp
,LeaveWOPay
,AbsentDeduction
,AbsentOrLate 
,LeaveEncash
,SalaryDeduction
,AdvanceDeduction
,TAX,Tiffin,ConveyanceD,Lunch
,TransportBill
,TravelLoan
,TravelLoanInterest
,Basic_B
,Medical_B
,Conveyance_B
,House_Rent_B
,Punishment
)
) p ) as Sal
left outer join 
dbo.Project AS p ON Sal.ProjectId = p.Id LEFT OUTER JOIN
dbo.Department AS d ON Sal.DepartmentId = d.Id LEFT OUTER JOIN
dbo.Section AS s ON Sal.SectionId = s.Id LEFT OUTER JOIN
dbo.Designation AS desig ON Sal.DesignationId = desig.Id LEFT OUTER JOIN
dbo.EmployeeJob AS ej ON Sal.EmployeeId = ej.EmployeeId LEFT OUTER JOIN
dbo.FiscalYearDetail AS fyd ON Sal.FiscalYearDetailId = fyd.Id LEFT OUTER JOIN
EmployeeInfo AS ei  ON Sal.EmployeeId = ei.Id LEFT OUTER JOIN
dbo.EmployeeTransfer AS et ON ei.Id = et.EmployeeId AND et.IsCurrent = 1 LEFT OUTER JOIN
dbo.EmployeePromotion AS ep ON ei.Id = ep.EmployeeId AND ep.IsCurrent = 1 LEFT OUTER JOIN
dbo.Grade AS g ON Sal.GradeId = g.Id LEFT OUTER JOIN
dbo.EnumSalaryStep AS stp ON ep.StepId = stp.Id LEFT OUTER JOIN
dbo.Branch AS b ON ei.BranchId = b.Id LEFT OUTER JOIN
dbo.EmployeeStructureGroup AS esg ON Sal.EmployeeId = esg.EmployeeId LEFT OUTER JOIN
dbo.EmployeePersonalDetail AS epd ON Sal.EmployeeId = epd.EmployeeId left outer join 
ProjectAllocationDetail pad on esg.ProjectAllocationId=pad.ProjectAllocationId LEFT OUTER JOIN
bank AS bk  ON ej.BankInfo = bk.Id
 LEFT OUTER JOIN MonthlyAttendance AS ma  ON Sal.EmployeeId = ma.EmployeeId and  Sal.FiscalYearDetailId = ma.FiscalYearDetailId
 LEFT OUTER JOIN SalaryEmployee se ON sal.EmployeeId = se.EmployeeId and  sal.FiscalYearDetailId = se.FiscalYearDetailId



where 1=1 
and pad.HeadName='Gross' 
 and (sal.FiscalYearDetailId between @FiscalYearDetailId and @FiscalYearDetailIdTo)

";
                #endregion

                #endregion

                #region Conditions

                if (!string.IsNullOrWhiteSpace(vm.ProjectId))
                    sqlText += " and sal.ProjectId=@ProjectId";
                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND sal.ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }

                if (vm.Other3List != null && vm.Other3List.Count > 0 && !string.IsNullOrWhiteSpace(vm.Other3List.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND ej.other3 IN(";
                    foreach (string item in vm.Other3List)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }


                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    sqlText += " and sal.DepartmentId=@DepartmentId ";
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    sqlText += " and sal.SectionId=@SectionId ";
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    sqlText += " and sal.DesignationId=@DesignationId ";
                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    sqlText += " and ei.Code>= @CodeF";
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    sqlText += " and ei.Code<= @CodeT";
                if (!string.IsNullOrWhiteSpace(vm.Other1))
                    sqlText += " and ej.other1=@other1 ";
                if (!string.IsNullOrWhiteSpace(vm.Other2))
                    sqlText += " and ej.other2=@other2 ";

                if (!string.IsNullOrWhiteSpace(vm.BankId))
                    sqlText += " and ej.BankInfo=@BankId ";

                if (!string.IsNullOrWhiteSpace(vm.HoldStatus))
                {
                    if (vm.HoldStatus.ToLower() == "hold")
                    {
                        sqlText += " AND se.IsHold=1 ";
                    }
                    else
                    {
                        sqlText += " AND se.IsHold=0 ";
                    }
                }
                #endregion

                sqlText += @" 
                 ) as  TempSalary
                ";
                if (!vm.IsMultipleSalary)
                {
                    #region Select for Single Month
                    sqlText += @" 
                 SELECT * FROM #TempTable as newSal
                ";
                    #endregion
                }
                else
                {
                    #region Select for Multiple Month
                    sqlText += @" 
 SELECT * from(
                  SELECT newSal.*,
 0 IsHold
,'Not Hold' HoldStatus
, ISNULL(desig.PriorityLevel,0) PriorityLevel
,ve.Other1
,ve.Other2
,ve.Other3
,ve.Other4
,ve.Other5
,ej.BankInfo BankName
,bk.Code BankCode
,ej.BankAccountNo
,ej.BankPayAmount
,case when isnull(ej.IsPermanent, 0) = 1 then 'Permanent' else 'Not Permanent' end as PermanentStatus
,ve.JoinDate
,ve.ProjectAllocationId
,epd.Email EmpEmail
,'Month' PeriodName
,ve.Code
,ve.Salutation_E
,ve.MiddleName
,ve.LastName
,ve.EmpName
,ve.ProbationEnd
,ve.DateOfPermanent
,ve.LeftDate
,ve.Designation
,ve.Project
,ve.Department
,ve.Section
,ve.Grade
,@FiscalYearDetailId FiscalYearDetailId
,ve.ProjectId
,ve.DepartmentId
,ve.SectionId
,ve.DesignationId
,sed.EmployeeStatus
,ve.GradeId

  FROM
 (
SELECT 
EmployeeId
,SUM(OtherDeductionAmount				) OtherDeductionAmount					
,SUM(LeaveWOPayOD						) LeaveWOPayOD
,SUM(DOM								) DOM
,SUM(Bonus								) Bonus
,SUM(SL									) SL
,SUM(CL									) CL
,SUM(EL									) EL
,SUM(COL								) COL
,SUM(ML									) ML
,SUM(PL									) PL
,SUM(STL								) STL
,SUM(HoliDays							) HoliDays
,SUM(workingDays						) workingDays
,SUM(EarningDays						) EarningDays
,SUM(LWP								) LWP
,SUM(LeaveWOPay_ma						) LeaveWOPay_ma
,SUM(AbsentDays							) AbsentDays
,SUM(AbsentDeduction_ma					) AbsentDeduction_ma
,SUM(NPAmount							) NPAmount
,SUM(NPDays								) NPDays
,SUM(PayDays							) PayDays
,SUM(LateDeduction						) LateDeduction
,SUM(Portion1							) Portion1
,SUM(Portion2							) Portion2
,SUM(Portion3							) Portion3
,SUM(Portion4							) Portion4
,SUM(OtherAllowanceMonthly				) OtherAllowanceMonthly
,SUM(OtherDeductionMonthly				) OtherDeductionMonthly
,SUM(Arrear								) Arrear
,SUM(Basic								) Basic
,SUM(CarLoan							) CarLoan
,SUM(CarLoanInterest					) CarLoanInterest
,SUM(ChildAllowance						) ChildAllowance
,SUM(Conveyance							) Conveyance
,SUM(EducationLoan						) EducationLoan
,SUM(EducationLoanInterest				) EducationLoanInterest
,SUM(Entertainment						) Entertainment
,SUM(Gross								) Gross
,SUM(HouseLoan							) HouseLoan
,SUM(HouseLoanInterest					) HouseLoanInterest
,SUM(HouseRent							) HouseRent
,SUM(HouseRentDeduction					) HouseRentDeduction
,SUM(Loan								) Loan
,SUM(LoanInterest						) LoanInterest
,SUM(Medical							) Medical
,SUM(MedicalLoan						) MedicalLoan
,SUM(MedicalLoanInterest				) MedicalLoanInterest
,SUM(MobileBill							) MobileBill
,SUM(OtherLoan							) OtherLoan
,SUM(OtherLoanInterest					) OtherLoanInterest
,SUM(OverTime							) OverTime
,SUM(PFEmployee							) PFEmployee
,SUM(PFEmployer							) PFEmployer
,SUM(PFLoan								) PFLoan
,SUM(PFLoanInterest						) PFLoanInterest
,SUM(ReimbursableExpense				) ReimbursableExpense
,SUM(PreEmploymentCheckUp				) PreEmploymentCheckUp
,SUM(LeaveWOPay							) LeaveWOPay
,SUM(AbsentDeduction					) AbsentDeduction
,SUM(AdvanceDeduction					) AdvanceDeduction
,SUM(LeaveEncash						) LeaveEncash
,SUM(SalaryDeduction					) SalaryDeduction
,SUM(TAX								) TAX
,SUM(Tiffin								) Tiffin
,SUM(TransportBill						) TransportBill
,SUM(TravelLoan							) TravelLoan
,SUM(TravelLoanInterest					) TravelLoanInterest
,SUM(SalaryLoan							) SalaryLoan
,SUM(SalaryLoanInterest					) SalaryLoanInterest
,SUM(Basic_B							) Basic_B
,SUM(Medical_B							) Medical_B
,SUM(Conveyance_B						) Conveyance_B
,SUM(House_Rent_B						) House_Rent_B
,SUM(Punishment							) Punishment
,SUM(GrossPayForJournalCampe			) GrossPayForJournalCampe
,SUM(TotalDeductionForJournalCampe		) TotalDeductionForJournalCampe
,SUM(NetPayForJournalCampe				) NetPayForJournalCampe
,SUM(PFForJournalCampe					) PFForJournalCampe
,SUM(TAXForJournalCampe					) TAXForJournalCampe
,SUM(AdvanceDeductionForJournalCampe	) AdvanceDeductionForJournalCampe
,SUM(LeaveWOPayForJournalCampe			) LeaveWOPayForJournalCampe
,SUM(TotalLoan							) TotalLoan
,SUM(TransportBillForJournalCampe		) TransportBillForJournalCampe

FROM #TempTable

GROUP BY 						
EmployeeId											
)
 as newSal
 LEFT OUTER JOIN ViewEmployeeInformation AS ve  ON newSal.EmployeeId = ve.EmployeeId
 LEFT OUTER JOIN dbo.EmployeeJob AS ej ON newSal.EmployeeId = ej.EmployeeId
 LEFT OUTER JOIN dbo.EmployeeStructureGroup AS esg ON newSal.EmployeeId = esg.EmployeeId
 LEFT OUTER JOIN dbo.EmployeePersonalDetail AS epd ON newSal.EmployeeId = epd.EmployeeId 
 LEFT OUTER JOIN dbo.Designation AS desig ON ve.DesignationId = desig.Id

LEFT OUTER JOIN bank AS bk  ON ej.BankInfo = bk.Id
---- LEFT OUTER JOIN MonthlyAttendance AS ma  ON newSal.EmployeeId = ma.EmployeeId and  newSal.FiscalYearDetailId = ma.FiscalYearDetailId
LEFT OUTER JOIN 
(
 SELECT  
                                          main.EmployeeId,  main.EmployeeStatus 
                                                  
                          FROM            dbo.SalaryEarningDetail main
                          WHERE       1=1 and main.FiscalYearDetailId = @FiscalYearDetailIdTo
						  Group By main.EmployeeId, main.EmployeeStatus
						  ) as  sed ON newSal.EmployeeId = sed.EmployeeId
) as newSal
                ";
                    #endregion
                }

                #region Order By

                if (!string.IsNullOrWhiteSpace(vm.Orderby))
                {

                    if (vm.Orderby == "DCG")
                        sqlText += " order by newSal.Department, newSal.code";
                    else if (vm.Orderby == "DDC")
                        sqlText += " order by newSal.Department, newSal.JoinDate, newSal.code";
                    else if (vm.Orderby == "DGC")
                        sqlText += " order by newSal.Department, newSal.code";
                    else if (vm.Orderby == "DGDC")
                        sqlText += " order by  newSal.Department, newSal.JoinDate, newSal.code";
                    else if (vm.Orderby == "CODE")
                        sqlText += " order by newSal.code";
                    else if (vm.Orderby.ToLower() == "designation")
                        sqlText += " order by ISNULL(newSal.PriorityLevel,0), newSal.code";
                }
                #endregion

                #region Drop Table

                sqlText += @" 
                 DROP TABLE #TempTable
                ";
                #endregion

                #endregion

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                #region Parameters

                //////if (!string.IsNullOrWhiteSpace(vm.FiscalYearDetailId))
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);

                if (!string.IsNullOrWhiteSpace(vm.ProjectId) && vm.ProjectId != "0")
                    objComm.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    objComm.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    objComm.Parameters.AddWithValue("@SectionId", vm.SectionId);
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    objComm.Parameters.AddWithValue("@DesignationId", vm.DesignationId);

                if (!string.IsNullOrWhiteSpace(vm.Other1))
                    objComm.Parameters.AddWithValue("@other1", vm.Other1);

                if (!string.IsNullOrWhiteSpace(vm.Other2))
                    objComm.Parameters.AddWithValue("@other2", vm.Other2);

                //////if (!string.IsNullOrWhiteSpace(other3))
                //////    objComm.Parameters.AddWithValue("@other3", other3);

                if (!string.IsNullOrWhiteSpace(vm.BankId))
                    objComm.Parameters.AddWithValue("@BankId", vm.BankId);


                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    objComm.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    objComm.Parameters.AddWithValue("@CodeT", vm.CodeTo);

                //////objComm.Connection = currConn;
                //////objComm.CommandText = sqlText;
                //////objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(ds);
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

        public DataTable TIBHRMSalary(SalarySheetVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (!string.IsNullOrWhiteSpace(vm.DBName))
                {
                    currConn.ChangeDatabase(vm.DBName);
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                #region Primary SqlText

                sqlText = @"Select

VS.Code
,VS.EmpName
,VS.EmployeeId
,Convert(VARCHAR(110),Convert(Datetime, ISNULL(NULLIF (VS.JoinDate, N''), N'19000101')),107) AS JoinDate
,VS.Project
,VS.Designation
,VS.TIN
,e.Gender 
,VT.PFLoan
,(ISNULL(e.CorporateContactLimit,0))TelephoneAllowance
,Convert(VARCHAR(110),Convert(Datetime, ISNULL(NULLIF (e.ProjectEndDate, N''), N'19000101')),107) AS ProjectEndDate
,SUM (CASE  WHEN SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) GrossSalary
,SUM (CASE  WHEN SalaryHead in('Basic') then Amount else 0 end) BasicSalary
,SUM (CASE  WHEN SalaryHead in('TAX') then Amount else 0 end) TAXSalary
,SUM (CASE  WHEN SalaryHead in('PFEmployee') THEN Amount ELSE 0 END) AS PFEmployee
,SUM (CASE  WHEN SalaryHead in('PFEmployer') THEN Amount ELSE 0 END) AS PFEmployer
,SUM (CASE  WHEN SalaryHead in('STAFFWELFARE') THEN Amount ELSE 0 END) AS STAFFWELFARE
,SUM (CASE  WHEN SalaryHead in('HouseRent') THEN Amount ELSE 0 END) AS HouseRent
,SUM (CASE  WHEN SalaryHead in('Medical') THEN Amount ELSE 0 END) AS Medical
,SUM (CASE  WHEN SalaryHead in('TransportAllowance') THEN Amount ELSE 0 END) AS ConveyanceAllowance
,SUM (CASE  WHEN SalaryHead in('Other (Bonus') THEN Amount ELSE 0 END) AS Bonus
,SUM (CASE  WHEN SalaryHead in('Rest&Recartion') THEN Amount ELSE 0 END) AS RestLeaveAllowance
,SUM (CASE  WHEN SalaryHead in('Charge') then Amount else 0 end)ChargeAllowance 
,SUM (CASE  WHEN SalaryHead in('Othere(OT)') then Amount else 0 end)[Othere(OT)]
,SUM (CASE  WHEN SalaryHead in('ChildAllowance') then Amount else 0 end)ChildAllowance 
,SUM (CASE  WHEN SalaryHead in('HARDSHIP') then Amount else 0 end)HARDSHIP 
,SUM (CASE  WHEN SalaryHead in('LeaveEncash') then Amount else 0 end)LeaveEncashment 
,SUM (CASE  WHEN SalaryHead in('PFLoan') then Amount else 0 end)PFLoan
,SUM (CASE  WHEN SalaryHead in('Transfer') then Amount else 0 end)TransferAllowance  
,SUM(ISNULL(e.CorporateContactLimit,0))TelephoneAllowance
from ViewSalaryPreCalculation VS 
left outer join ViewEmployeeInformation e on e.EmployeeId=VS.EmployeeId
Left Outer Join View_TIBSalary VT on VT.Code=VS.Code

where 1=1
 and (VS.FiscalYearDetailId between @FiscalYearDetailId and @FiscalYearDetailIdTo)
";
                #endregion

                #region Conditions

                if (!string.IsNullOrWhiteSpace(vm.ProjectId))
                    sqlText += " and VS.ProjectId=@ProjectId";
                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND VS.ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }




                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    sqlText += " and VS.DepartmentId=@DepartmentId ";
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    sqlText += " and VS.SectionId=@SectionId ";
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    sqlText += " and VS.DesignationId=@DesignationId ";
                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    sqlText += " and VS.Code>= @CodeF";
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    sqlText += " and VS.Code<= @CodeT";

                #endregion

                #endregion

                #endregion

                sqlText += @" Group by VS.Code,VS.EmpName,VS.EmployeeId,VS.JoinDate,VS.Project,VS.Designation,VS.TIN,e.Gender,e.ProjectEndDate,e.CorporateContactLimit,VT.PFLoan ";

                if (!string.IsNullOrWhiteSpace(vm.Orderby))
                {

                    if (vm.Orderby == "DCG")
                        sqlText += " order by VS.Department, VS.code";
                    else if (vm.Orderby == "DDC")
                        sqlText += " order by VS.Department, VS.JoinDate, VS.code";
                    else if (vm.Orderby == "DGC")
                        sqlText += " order by VS.Department, VS.code";
                    else if (vm.Orderby == "DGDC")
                        sqlText += " order by  VS.Department, VS.JoinDate, VS.code";
                    else if (vm.Orderby == "CODE")
                        sqlText += " order by VS.code";
                    else if (vm.Orderby.ToLower() == "designation")
                        sqlText += " order by ISNULL(VS.PriorityLevel,0), VS.code";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                #region Parameters

                //////if (!string.IsNullOrWhiteSpace(vm.FiscalYearDetailId))
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);

                if (!string.IsNullOrWhiteSpace(vm.ProjectId) && vm.ProjectId != "0")
                    objComm.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    objComm.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    objComm.Parameters.AddWithValue("@SectionId", vm.SectionId);
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    objComm.Parameters.AddWithValue("@DesignationId", vm.DesignationId);

                //if (!string.IsNullOrWhiteSpace(vm.Other1))
                //    objComm.Parameters.AddWithValue("@other1", vm.Other1);

                //if (!string.IsNullOrWhiteSpace(vm.Other2))
                //    objComm.Parameters.AddWithValue("@other2", vm.Other2);

                ////////if (!string.IsNullOrWhiteSpace(other3))
                ////////    objComm.Parameters.AddWithValue("@other3", other3);

                //if (!string.IsNullOrWhiteSpace(vm.BankId))
                //    objComm.Parameters.AddWithValue("@BankId", vm.BankId);


                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    objComm.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    objComm.Parameters.AddWithValue("@CodeT", vm.CodeTo);

                //////objComm.Connection = currConn;
                //////objComm.CommandText = sqlText;
                //////objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dt);
                #endregion
                dt = Ordinary.DtValueRound(dt, new[] { "TAXSalary" });

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
            return dt;
        }

        #region Backup Methods
        //////28 May 2019 ---Khalid
        public DataSet SalaryPreCalculationNewBackup(SalarySheetVM vm)
        {
            return null;


        }
        #endregion

        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId
            , string SectionId, string DesignationId, string CodeF, string CodeT, string fid
            , List<string> ProjectIdList = null, string other1 = "", string other2 = "", string other3 = "", string Orderby = "", string bankId = "")
        {
            string[] results = new string[6];
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            FiscalYearDAL fdal = new FiscalYearDAL();
            string fname = fdal.FYPeriodDetail(Convert.ToInt32(fid), null, null).FirstOrDefault().PeriodName;
            try
            {
                #region DataRead From DB
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                int j = 2;
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
select --isnull(CONVERT(int, fyd.PeriodEnd)- convert(int,fyd.PeriodStart)+1,0) DOM, 
  ISNULL(LTRIM(RTRIM(ei.Salutation_E)), N'') 
+ ' ' + ISNULL(LTRIM(RTRIM(ei.MiddleName)), N'') + ' ' + ISNULL(LTRIM(RTRIM(ei.LastName)), N'') AS EmpName
,desig.Name Designation,
 ei.Code [PF No/Emp No],
[Basic]
 ,HouseRent[House Rent]
 ,Medical[Med Allow]
 ,Conveyance[Conv.]
 ,ChildAllowance[Child Allow.]
,0 Bonus
,OverTime[OVERTime]
,Arrear
,PFEmployer [Employers Cont to P.F]
,Sal.Gross
,Sal.Gross -PFEmployer [Total Salary ]
,TAX [Income Tax]
,TransportBill [TranspBill/LWP]
,sal.MobileBill
,OtherLoan+OtherLoanInterest+SalaryLoan+SalaryLoanInterest [Loan/Adv.]
,sal.PreEmploymentCheckUp
,(Sal.AbsentDeduction+Sal.AdvanceDeduction+sal.TAX+ Sal.CarLoan+Sal.CarLoanInterest+Sal.EducationLoan+Sal.EducationLoanInterest+
Sal.HouseLoan+Sal.HouseLoanInterest+
Sal.LeaveWOPay+Sal.Loan+Sal.LoanInterest
+Sal.OtherLoan+Sal.OtherLoanInterest+Sal.SalaryLoan+Sal.SalaryLoanInterest+
Sal.MedicalLoan+Sal.MedicalLoanInterest+Sal.PFLoan+Sal.PFLoanInterest
+Sal.SalaryDeduction+Sal.TravelLoan+Sal.TravelLoanInterest)+PFEmployer [Total Deduct]

,(ej.GrossSalary-(Sal.AbsentDeduction+Sal.AdvanceDeduction+sal.TAX+ Sal.CarLoan+Sal.CarLoanInterest+Sal.EducationLoan+Sal.EducationLoanInterest+
Sal.HouseLoan+Sal.HouseLoanInterest+
Sal.LeaveWOPay+Sal.Loan+Sal.LoanInterest
+Sal.OtherLoan+Sal.OtherLoanInterest+Sal.SalaryLoan+Sal.SalaryLoanInterest+
Sal.MedicalLoan+Sal.MedicalLoanInterest+Sal.PFLoan+Sal.PFLoanInterest
+Sal.SalaryDeduction+Sal.TravelLoan+Sal.TravelLoanInterest+PFEmployer)) [Net Salary]
,ej.GrossSalary
,p.Name Project,d.name Department,s.Name Section
,sal.FiscalYearDetailId
,sal.EmployeeStatus

,pad.Portion1 [(%) of salary]
,((Sal.Gross -PFEmployer)* pad.Portion1) /100 [Protyasha mount in BDT]
,pad.Portion2 [(%) of salary]
,((Sal.Gross -PFEmployer)* pad.Portion2) /100 [Ongiker amount in BDT]
,PFEmployer [Employers Cont to P.F]
,PFLoan+PFLoanInterest [PF Loan & Interest]




, sal.* from(
SELECT  EmployeeId
,FiscalYearDetailId
,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeStatus,GradeId
,isnull(MobileBill,0)MobileBill,isnull(OtherLoan,0)OtherLoan,isnull(OtherLoanInterest,0)OtherLoanInterest
,isnull( Arrear,0) Arrear,isnull([Basic],0)[Basic],isnull(CarLoan,0)CarLoan,isnull(CarLoanInterest,0)CarLoanInterest
,isnull(ChildAllowance,0)ChildAllowance,isnull(Conveyance,0)Conveyance,isnull(EducationLoan,0)EducationLoan
,isnull(EducationLoanInterest,0)EducationLoanInterest,isnull(Entertainment,0)Entertainment,isnull(Gross,0)Gross
,isnull(HouseLoan,0)HouseLoan,isnull(HouseLoanInterest,0)HouseLoanInterest,isnull(HouseRent,0)HouseRent
,isnull(HouseRentDeduction,0)HouseRentDeduction,isnull(Loan,0)Loan
,isnull(LoanInterest,0)LoanInterest
,isnull(Medical,0)Medical,isnull(MedicalLoan,0)MedicalLoan,isnull(MedicalLoanInterest,0)MedicalLoanInterest

,isnull(OverTime,0)OverTime,isnull(PFEmployee,0)PFEmployee,isnull(PFEmployer,0)PFEmployer,isnull(PFLoan,0)PFLoan
,isnull(PFLoanInterest,0)PFLoanInterest
,isnull(ReimbursableExpense,0)ReimbursableExpense
,isnull(PreEmploymentCheckUp,0)PreEmploymentCheckUp
,isnull(LeaveWOPay,0)LeaveWOPay
,isnull(AbsentDeduction,0)AbsentDeduction
,isnull(AdvanceDeduction,0)AdvanceDeduction
,isnull(LeaveEncash,0)LeaveEncash
,isnull(SalaryDeduction,0)SalaryDeduction
,isnull(TAX,0)TAX,isnull(Tiffin,0)Tiffin
,isnull(TransportBill,0)TransportBill
,isnull(TravelLoan,0)TravelLoan
,isnull(TravelLoanInterest,0)TravelLoanInterest
,isnull(SalaryLoan,0)SalaryLoan
,isnull(SalaryLoanInterest,0)SalaryLoanInterest
from 
(select EmployeeId,FiscalYearDetailId ,ProjectId,DepartmentId,SectionId,DesignationId ,EmployeeStatus,GradeId
, isnull(amount,0)amount, SalaryHead
from ViewSalaryPreCalculation) x pivot 
(sum(amount) for SalaryHead in (Arrear,[Basic],CarLoan,CarLoanInterest,ChildAllowance,Conveyance,EducationLoan,EducationLoanInterest,Entertainment
,Gross,HouseLoan,HouseLoanInterest,HouseRent,HouseRentDeduction
,Loan,LoanInterest,Medical,MedicalLoan,MedicalLoanInterest,MobileBill,OtherLoan,OtherLoanInterest,OverTime
,PFEmployee,PFEmployer,PFLoan,PFLoanInterest,SalaryLoan,SalaryLoanInterest
,ReimbursableExpense
,PreEmploymentCheckUp
,LeaveWOPay
,AbsentDeduction
,LeaveEncash
,SalaryDeduction
,AdvanceDeduction,TAX,Tiffin,TransportBill,TravelLoan,TravelLoanInterest)
) p ) as Sal
left outer join 
dbo.Project AS p ON Sal.ProjectId = p.Id LEFT OUTER JOIN
dbo.Department AS d ON Sal.DepartmentId = d.Id LEFT OUTER JOIN
dbo.Section AS s ON Sal.SectionId = s.Id LEFT OUTER JOIN
dbo.Designation AS desig ON Sal.DesignationId = desig.Id LEFT OUTER JOIN
dbo.EmployeeJob AS ej ON Sal.EmployeeId = ej.EmployeeId LEFT OUTER JOIN
dbo.FiscalYearDetail AS fyd ON Sal.FiscalYearDetailId = fyd.Id LEFT OUTER JOIN
EmployeeInfo AS ei  ON Sal.EmployeeId = ei.Id LEFT OUTER JOIN
dbo.EmployeeTransfer AS et ON ei.Id = et.EmployeeId AND et.IsCurrent = 1 LEFT OUTER JOIN
dbo.EmployeePromotion AS ep ON ei.Id = ep.EmployeeId AND ep.IsCurrent = 1 LEFT OUTER JOIN
dbo.Grade AS g ON Sal.GradeId = g.Id LEFT OUTER JOIN
dbo.EnumSalaryStep AS stp ON ep.StepId = stp.Id LEFT OUTER JOIN
dbo.Branch AS b ON ei.BranchId = b.Id LEFT OUTER JOIN
dbo.EmployeeStructureGroup AS esg ON Sal.EmployeeId = esg.EmployeeId
left outer join ProjectAllocationDetail pad on esg.ProjectAllocationId=pad.ProjectAllocationId
where 1=1 
and pad.HeadName='Gross'";
                if (fid != "0_0" && fid != "0" && fid != "" && fid != "null" && fid != null)
                    sqlText += " and sal.FiscalYearDetailId=@FiscalYearDetailId";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    sqlText += " and sal.ProjectId=@ProjectId";
                if (ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(ProjectIdList.FirstOrDefault()))
                {
                    sqlText += "  and sal.ProjectId in(";
                    for (int i = 0; i < ProjectIdList.Count; i++)
                    {
                        sqlText += "'" + ProjectIdList[i] + "'";
                        if (i < ProjectIdList.Count - 1)
                            sqlText += ", ";
                    }
                    sqlText += ")";
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += " and sal.DepartmentId=@DepartmentId ";
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    sqlText += " and sal.SectionId=@SectionId ";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += " and sal.DesignationId=@DesignationId ";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += " and ei.Code>= @CodeF";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += " and ei.Code<= @CodeT";

                if (!string.IsNullOrWhiteSpace(other1))
                    sqlText += " and ej.other1=@other1 ";
                if (!string.IsNullOrWhiteSpace(other2))
                    sqlText += " and ej.other2=@other2 ";
                if (!string.IsNullOrWhiteSpace(other3))
                    sqlText += " and ej.other3=@other3 ";

                if (!string.IsNullOrWhiteSpace(bankId))
                    sqlText += " and ej.BankInfo=@BankId ";


                if (Orderby == "DCG")
                    sqlText += " order by d.Name, ei.code, g.sl";
                else if (Orderby == "DDC")
                    sqlText += " order by d.Name, ej.JoinDate, ei.code";
                else if (Orderby == "DGC")
                    sqlText += " order by d.Name, g.sl, ei.code";
                else if (Orderby == "DGDC")
                    sqlText += " order by d.Name, g.sl, ej.JoinDate, ei.code";
                else if (Orderby == "CODE")
                    sqlText += " order by ei.code";


                #endregion
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrWhiteSpace(other1))
                    objComm.Parameters.AddWithValue("@other1", other1);

                if (!string.IsNullOrWhiteSpace(other2))
                    objComm.Parameters.AddWithValue("@other2", other2);

                if (!string.IsNullOrWhiteSpace(other3))
                    objComm.Parameters.AddWithValue("@other3", other3);



                if (fid != "0_0" && fid != "0" && fid != "" && fid != "null" && fid != null)
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", fid);
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);
                //dt.Columns.Add("Fiscal Period");
                //dt.Columns.Add("Type");
                //foreach (DataRow row in dt.Rows)
                //{
                //    row["Fiscal Period"] = fname;
                //    row["FYDId"] = FiscalYearDetailId;
                //}
                //bool tt = WriteDataTableToExcel(dt, "DataSheet", Filepath + FileName,fname);
                //if (tt == false)
                //{
                //    results[0] = "Fail";
                //    results[1] = "Data Download UnSuccessfully.";
                //}
                //{
                //    results[0] = "Success";
                //    results[1] = "Data Download Successfully.";
                //}
                #endregion
            }
            catch (Exception ex)
            {
                results[4] = ex.Message.ToString();
            }
            return dt;
        }

        public static bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string fname)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                excelworkBook = excel.Workbooks.Add(Type.Missing);
                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;
                int j = 3;
                int rowcount = j;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1000, 1]];
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 3], excelSheet.Cells[1000, 1]];
                excelCellrange.NumberFormat = "@";
                //excelCellrange = excelSheet.Range[excelSheet.Cells[1, 4], excelSheet.Cells[1000, 1000]];
                //excelCellrange.NumberFormat = "#,###,###";
                excelSheet.get_Range("A1", "Y2").Merge(false);
                excelCellrange = excelSheet.get_Range("A1", "Y2");
                excelCellrange.FormulaR1C1 = "Salary Sheet (For the month Of " + fname + ")";
                excelCellrange.Font.Size = 20;
                excelCellrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excelSheet.get_Range("A1", "Y2").Merge(false);
                foreach (DataRow datarow in dataTable.Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == j + 1)
                        {
                            excelSheet.Cells[j, i] = dataTable.Columns[i - 1].ColumnName;
                            excelSheet.Cells[j, i].Font.Bold = true;
                        }
                        if (dataTable.Columns[i - 1].DataType == Type.GetType("System.Decimal"))
                        {
                            excelSheet.Cells[rowcount, i].Numberformat = "#,###,###";
                        }
                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                        if (dataTable.Columns[i - 1].ColumnName == "Net Salary")
                        {
                            break;
                        }
                    }
                }
                excelCellrange = excelSheet.Range[excelSheet.Cells[j, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, dataTable.Columns.Count]];
                excelCellrange.Font.Bold = true;
                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }
        }

        public DataSet PaySlipPreCalculation(string FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Name, string dojFrom, string dojTo)
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
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);
SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.SalaryHead) 
from(select distinct SalaryHead from ViewSalaryPreCalculation 
union select distinct  NameTrim from EnumSalaryType 
union select distinct  replace(name,' ','')SalaryHead from EnumLoanType 
union select distinct  replace(name,' ','') +'Interest' SalaryHead from EnumLoanType 
) as c  FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
set @query = 'SELECT PeriodName
,Code
,EmpName
,Designation
,JoinDate
,Project
,Department
,Section
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearDetailId
, ' + @cols + ' from 
            (
                select PeriodName
,Code
,EmpName
,Designation
,JoinDate
,Project
,Department
,Section
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,FiscalYearDetailId
,isnull(amount,0)amount
,SalaryHead
from ViewSalaryPreCalculation
           ) x
            pivot 
            (
                 sum(amount)
                for SalaryHead in (' + @cols + ')
            ) p '
execute(@query)                            
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(ds);
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

        public List<SalaryInformationVM> SalaryInfomation(string employeeID = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string sqlText = "";
            List<SalaryInformationVM> VMs = new List<SalaryInformationVM>();
            SalaryInformationVM vm;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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
                sqlText = @" select td.sl, td.EmployeeId,v.Code, v.EmpName,v.Designation, v.Department,
 v.DepartmentId,v.Section,v.SectionId,v.Project,v.ProjectId,v.JoinDate,td.Type, f.PeriodName, f.Id FiscalYearDetailId,
td.Name SalaryType ,td.Amount from (  
select case when  sed.isearning=1 then '4' else '5' end as [SL], sed.EmployeeId,sed.Amount Amount, 'Earning' [Type],sed.FiscalYearDetailId,est.Name
from SalaryEarningDetail sed left outer join EnumSalaryType est on est.Id=sed.SalaryTypeId
union all
select '1', p.EmployeeId,p.PFValue Amount,'PF ' [Type],p.FiscalYearDetailId,'PF Employee'
from SalaryPFDetail p
union all
select '2', s.EmployeeId,s.TaxValue Amount, 'TAX ' [Type],s.FiscalYearDetailId,'TAX Employee'
from SalaryTaxDetail s
union all 
select '3' ,se.EmployeeId , se.AreerAmount,'Areer ' [Type],se.FiscalYearDetailId,'Areer Employee'
from SalaryAreerDetail se
union all
select '5' ,e.EmployeeId, e.DeductionAmount Amount,'Deduction' [Type],e.FiscalYearDetailId,'Employee Deduction'
from SalaryDeductionDetail e
union all
select '6' , s.EmployeeId, s.LoanAmount Amount,'Loan ' [Type],s.FiscalYearDetailId ,'Employee Loan'
from SalaryLoanDetail s
) as td  left outer join   ViewEmployeeInformation v on td.EmployeeId =v.Id
left outer join FiscalYearDetail f on td.FiscalYearDetailId=f.Id
where 1=1
order by td.SL
";
                //if (employeeID != null)
                //{
                //    if (employeeID != "0_0")
                //    {
                //        sqlText += " and v.Id=@empId";
                //    }
                //}
                //sqlText += "  order by td.SL";
                #endregion
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                //objComm.Parameters.AddWithValue("@empId", employeeID);
                //if (employeeID != "0_0" || !string.IsNullOrWhiteSpace(employeeID) || employeeID.Trim() != "")
                //{
                //    objComm.Parameters.AddWithValue("@empId", employeeID);
                //}
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new SalaryInformationVM();
                        vm.SLNo = dr["sl"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.Department = dr["Department"].ToString();
                        vm.DepartmentId = dr["DepartmentId"].ToString();
                        vm.Designation = dr["Designation"].ToString();
                        vm.Project = dr["Project"].ToString();
                        vm.ProjectId = dr["ProjectId"].ToString();
                        vm.Section = dr["Section"].ToString();
                        vm.SectionId = dr["SectionId"].ToString();
                        vm.JoinDate = Ordinary.StringToDate(dr["joindate"].ToString());
                        vm.Type = dr["Type"].ToString();
                        vm.PeriodName = dr["PeriodName"].ToString();
                        vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                        vm.SalaryType = dr["SalaryType"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return VMs;
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
            return VMs;
        }

        public string[] DeleteLeftEmployee(string PeriodStart, string PeriodEnd, string FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBank"); }
                #endregion open connection and transaction
                #region Update Settings
                sqlText = "";
                sqlText += @" delete [dbo].[SalaryEarningDetail] 
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryLoanDetail]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryOtherDeduction]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryOtherEarning] 
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryPFDetail]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryPFDetailEmployeer]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[SalaryTaxDetail]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";
                sqlText += @" delete [dbo].[MonthlyAttendance]
where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";

                //////                sqlText += @" delete [dbo].[SalaryEmployee]
                //////where EmployeeId in(select EmployeeId from EmployeeLeftInformation where LeftDate<@PeriodStart) and FiscalYearDetailId =@FiscalYearDetailId";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdUpdate.Parameters.AddWithValue("@PeriodStart", PeriodStart);
                int exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #region Commit
                if (transResult <= 0)
                {
                    //throw new ArgumentNullException("Process Delete Delete", fid + " could not Delete.");
                }
                #endregion Commit
                #endregion Update Settings
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Left Employee Delete Successfully.";
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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
            return retResults;
        }

        //==================UpdateEmployeeStatus =================
        public string[] UpdateEmployeeStatus(string FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                #region Save

                #region sqlText
                sqlText = "  ";
                sqlText = "";

                sqlText = @"
update  SalaryPFDetail set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryPFDetail.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryPFDetail.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId

update  SalaryPFDetailEmployeer set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryPFDetailEmployeer.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryPFDetailEmployeer.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId

 update  SalaryTaxDetail set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryTaxDetail.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryTaxDetail.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId

 update  SalaryOtherEarning set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryOtherEarning.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryOtherEarning.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId

update  SalaryOtherDeduction set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryOtherDeduction.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryOtherDeduction.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId

 update  SalaryLoanDetail set EmployeeStatus=SalaryEarningDetail.EmployeeStatus
from SalaryEarningDetail
where SalaryEarningDetail.FiscalYearDetailId=SalaryLoanDetail.FiscalYearDetailId 
and SalaryEarningDetail.EmployeeId=SalaryLoanDetail.EmployeeId
and SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId
";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                int exec = cmdUpdate.ExecuteNonQuery();

                transResult = Convert.ToInt32(exec);
                //if (transResult <= 0)
                //{
                //    retResults[1] = "Unexpected error to Update EmployeeStatus!";
                //    throw new ArgumentNullException(retResults[1], "");
                //}


                #endregion sqlExecution

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
                retResults[2] = "0";
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

        public string[] UpdateCurrentEmployee(string FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                #region Save

                #region sqlText
                sqlText = "  ";
                sqlText = "";

                sqlText = @"
------declare @FiscalYearDetailId varchar(100)='1049'
declare @PeriodEnd varchar(100)

select @PeriodEnd=PeriodEnd from FiscalYearDetail where id=@FiscalYearDetailId
 

select * into #TransferTemp from(
select EmployeeTransfer.EmployeeId,ProjectId,DepartmentId,SectionId from EmployeeTransfer
inner join (
select distinct EmployeeId,MAX(TransferDate)TransferDate from EmployeeTransfer
where TransferDate <=@PeriodEnd and IsActive=1
group by EmployeeId
) as Trn1 on EmployeeTransfer.EmployeeId=Trn1.EmployeeId 
and EmployeeTransfer.TransferDate=Trn1.TransferDate  and IsActive= 1

) as a


select * into #PromotionTemp from(
select EmployeePromotion.EmployeeId,DesignationId,GradeId from EmployeePromotion
inner join (
select distinct EmployeeId,MAX(PromotionDate)PromotionDate from EmployeePromotion
where PromotionDate <=@PeriodEnd  and IsActive=1
group by EmployeeId
) as Trn1 on EmployeePromotion.EmployeeId=Trn1.EmployeeId 
and EmployeePromotion.PromotionDate=Trn1.PromotionDate  and IsActive=1
) as a





update SalaryEarningDetail set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryEarningDetail.FiscalYearDetailId=@FiscalYearDetailId and SalaryEarningDetail.EmployeeId=t.EmployeeId


update SalaryLoanDetail set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryLoanDetail.FiscalYearDetailId=@FiscalYearDetailId and SalaryLoanDetail.EmployeeId=t.EmployeeId






update SalaryOtherDeduction set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryOtherDeduction.FiscalYearDetailId=@FiscalYearDetailId and SalaryOtherDeduction.EmployeeId=t.EmployeeId



update SalaryOtherEarning set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryOtherEarning.FiscalYearDetailId=@FiscalYearDetailId and SalaryOtherEarning.EmployeeId=t.EmployeeId




update SalaryPFDetail set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryPFDetail.FiscalYearDetailId=@FiscalYearDetailId and SalaryPFDetail.EmployeeId=t.EmployeeId


update SalaryTaxDetail set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryTaxDetail.FiscalYearDetailId=@FiscalYearDetailId and SalaryTaxDetail.EmployeeId=t.EmployeeId


update SalaryPFDetailEmployeer set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryPFDetailEmployeer.FiscalYearDetailId=@FiscalYearDetailId and SalaryPFDetailEmployeer.EmployeeId=t.EmployeeId





update SalaryEmployee set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId
,GradeId=p.GradeId
from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where SalaryEmployee.FiscalYearDetailId=@FiscalYearDetailId and SalaryEmployee.EmployeeId=t.EmployeeId




update MonthlyAttendance set 
ProjectId=t.ProjectId
,DepartmentId=t.DepartmentId
,SectionId=t.SectionId
,DesignationId=p.DesignationId

from #TransferTemp  t 
left outer join #PromotionTemp p on t.EmployeeId=p.EmployeeId
where MonthlyAttendance.FiscalYearDetailId=@FiscalYearDetailId and MonthlyAttendance.EmployeeId=t.EmployeeId




drop table #TransferTemp
drop table #PromotionTemp




";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                int exec = cmdUpdate.ExecuteNonQuery();

                transResult = Convert.ToInt32(exec);


                #endregion sqlExecution

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
                retResults[2] = "0";
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


        public DataTable dtSalarySheetKazol(DataTable dt)
        {
            try
            {
                ////string[] shortColumnName = { "FiscalYearDetailId", "EmployeeId", "Code", "EmpName", "Department", "Basic", "HouseRent", "Medical", "Conveyance", "Gross", "Arrear", "ReimbursableExpense", "PreEmploymentCheckUp", "OtherAllowanceMonthly", "MobileBill", "TAX", "AdvanceDeduction", "PFEmployee", "TotalLoan", "OtherDeductionMonthly", "DOM", "PayDays", "NPDays", "BankAccountNo", "Section", "SalaryDeduction", "TransportBill" };
                ////Type[] columnTypes = { typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(int), typeof(decimal), typeof(int), typeof(string), typeof(string), typeof(decimal), typeof(decimal) };
                #region Select Columns

                string[] shortColumnName = { "FiscalYearDetailId", "PeriodName", "EmployeeId", "Code", "EmpName", "Department", "BankAccountNo"
                                                    , "Designation", "Project", "Other3"//01-19
                                                    
                                                    , "Basic"                                                                       //20     typeof(decimal)
                                                    , "HouseRent"                                                                   //21     typeof(decimal)
                                                    , "Medical"                                                                     //22     typeof(decimal)
                                                    , "Conveyance"                                                                  //23     typeof(decimal)
                                                    , "Gross"                                                                       //24     typeof(decimal)
                                                    , "Arrear"                                                                      //25     typeof(decimal)
                                                    , "LeaveEncash"                                                                 //26     typeof(decimal)
                                                    , "ReimbursableExpense"                                                         //27     typeof(decimal)
                                                    , "AbsentDeduction_ma"                                                             //28     typeof(decimal)
                                                    , "LeaveWOPay_ma"                                                                  //29     typeof(decimal)
                                                    , "AdvanceDeduction"                                                            //30     typeof(decimal)
                                                    , "TotalLoan"                                                                   //31     typeof(decimal)
                                                    , "Tax"                                                                         //32     typeof(decimal)
                                                    , "SalaryDeduction"                                                             //33     typeof(decimal)
                                                    , "TransportBill"                                                               //34     typeof(decimal)
                                                    , "LateDeduction"                                                               //35     typeof(decimal)
                                                    , "Punishment"                                                                  //36     typeof(decimal)
                                                    , "BankPayAmount"                                                               //37     typeof(decimal)
                                                    , "PayDays"                                                                     //38     typeof(decimal)
                                                    , "NPAmount"                                                                    //39     typeof(decimal)
                                                 };

                Type[] columnTypes = { typeof(int), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String)
                                             , typeof(String), typeof(String), typeof(String)//01-19

                                             , typeof(decimal)                                                                           //20   
                                             , typeof(decimal)                                                                           //21   
                                             , typeof(decimal)                                                                           //22   
                                             , typeof(decimal)                                                                           //23   
                                             , typeof(decimal)                                                                           //24   
                                             , typeof(decimal)                                                                           //25   
                                             , typeof(decimal)                                                                           //26   
                                             , typeof(decimal)                                                                           //27   
                                             , typeof(decimal)                                                                           //28   
                                             , typeof(decimal)                                                                           //29   
                                             , typeof(decimal)                                                                           //30   
                                             , typeof(decimal)                                                                           //31   
                                             , typeof(decimal)                                                                           //32   
                                             , typeof(decimal)                                                                           //33   
                                             , typeof(decimal)                                                                           //34   
                                             , typeof(decimal)                                                                           //35   
                                             , typeof(decimal)                                                                           //36   
                                             , typeof(decimal)                                                                           //37   
                                             , typeof(decimal)                                                                           //38   
                                             , typeof(decimal)                                                                           //39   
                                            

                                         };


                dt = Ordinary.DtSetColumnsOrder(dt, shortColumnName);
                dt = Ordinary.DtSelectedColumn(dt, shortColumnName, columnTypes);

                ////////dt.Columns.Add("OtherDeduction", typeof(decimal));
                ////////dt.Columns.Add("TotalEarning", typeof(decimal));
                ////////dt.Columns.Add("TotalDeduction", typeof(decimal));
                dt.Columns.Add("NetSalaryBank", typeof(decimal));
                dt.Columns.Add("NetSalaryCash", typeof(decimal));


                dt.Columns.Add("BankSheetGross", typeof(decimal));
                dt.Columns.Add("CashPayGross", typeof(decimal));

                dt.Columns.Add("OtherEarning", typeof(decimal));
                dt.Columns.Add("TotalDeduction", typeof(decimal));
                dt.Columns.Add("OtherDeduction", typeof(decimal));
                dt.Columns.Add("AllEarning", typeof(decimal));
                dt.Columns.Add("AllDeduction", typeof(decimal));
                dt.Columns.Add("BankSheetDeduction", typeof(decimal));

                dt.Columns.Add("NetSalary", typeof(decimal));


                dt = Ordinary.DtColumnNameChange(dt, "LeaveWOPay_ma", "LeaveWOPay");
                dt = Ordinary.DtColumnNameChange(dt, "AbsentDeduction_ma", "AbsentDeduction");



                #endregion
                #region Declarations
                decimal vGross = 0;
                decimal vArrear = 0;
                decimal vLeaveEncash = 0;
                decimal vReimbursableExpense = 0;
                decimal vAbsentDeduction = 0;
                decimal vLeaveWOPay = 0;
                decimal vAdvanceDeduction = 0;
                decimal vTotalLoan = 0;
                decimal vTax = 0;
                decimal vSalaryDeduction = 0;
                decimal vTransportBill = 0;
                decimal vLateDeduction = 0;
                decimal vPunishment = 0;
                decimal vBankPayAmount = 0;
                decimal vNPAmount = 0;

                decimal vBankSheetGross = 0;
                decimal vCashPayGross = 0;
                decimal vOtherEarning = 0;
                decimal vTotalDeduction = 0;
                decimal vOtherDeduction = 0;
                decimal vAllEarning = 0;
                decimal vAllDeduction = 0;
                decimal vBankSheetDeduction = 0;
                decimal vNetSalaryBank = 0;
                decimal vNetSalaryCash = 0;

                decimal vNetSalary = 0;


                #endregion

                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    #region Variables
                    vGross = 0;
                    vArrear = 0;
                    vLeaveEncash = 0;
                    vReimbursableExpense = 0;
                    vAbsentDeduction = 0;
                    vLeaveWOPay = 0;
                    vAdvanceDeduction = 0;
                    vTotalLoan = 0;
                    vTax = 0;
                    vSalaryDeduction = 0;
                    vTransportBill = 0;
                    vLateDeduction = 0;
                    vPunishment = 0;
                    vBankPayAmount = 0;
                    vNPAmount = 0;


                    vBankSheetGross = 0;
                    vCashPayGross = 0;
                    vOtherEarning = 0;
                    vTotalDeduction = 0;
                    vOtherDeduction = 0;
                    vAllEarning = 0;
                    vAllDeduction = 0;
                    vBankSheetDeduction = 0;
                    vNetSalaryBank = 0;
                    vNetSalaryCash = 0;

                    vNetSalary = 0;


                    #endregion
                    #region Value Assign

                    vGross = Math.Round(Convert.ToDecimal(dt.Rows[i]["Gross"]));
                    vArrear = Math.Round(Convert.ToDecimal(dt.Rows[i]["Arrear"]));
                    vLeaveEncash = Math.Round(Convert.ToDecimal(dt.Rows[i]["LeaveEncash"]));
                    vReimbursableExpense = Math.Round(Convert.ToDecimal(dt.Rows[i]["ReimbursableExpense"]));
                    vAbsentDeduction = Convert.ToDecimal(dt.Rows[i]["AbsentDeduction"]);
                    vLeaveWOPay = Math.Round(Convert.ToDecimal(dt.Rows[i]["LeaveWOPay"]));
                    vAdvanceDeduction = Math.Round(Convert.ToDecimal(dt.Rows[i]["AdvanceDeduction"]));
                    vTotalLoan = Math.Round(Convert.ToDecimal(dt.Rows[i]["TotalLoan"]));
                    vTax = Math.Round(Convert.ToDecimal(dt.Rows[i]["Tax"]));
                    vSalaryDeduction = Math.Round(Convert.ToDecimal(dt.Rows[i]["SalaryDeduction"]));
                    vTransportBill = Math.Round(Convert.ToDecimal(dt.Rows[i]["TransportBill"]));
                    vLateDeduction = Math.Round(Convert.ToDecimal(dt.Rows[i]["LateDeduction"]));
                    vPunishment = Math.Round(Convert.ToDecimal(dt.Rows[i]["Punishment"]));
                    vBankPayAmount = Math.Round(Convert.ToDecimal(dt.Rows[i]["BankPayAmount"]));
                    vNPAmount = Convert.ToDecimal(dt.Rows[i]["NPAmount"]);

                    vAbsentDeduction = Math.Round(vAbsentDeduction + vNPAmount);

                    #region Round Figure
                    //////vAbsentDeduction = vAbsentDeduction;
                    //////vLeaveWOPay = Math.Round(vLeaveWOPay);
                    //////vAdvanceDeduction = Math.Round(vAdvanceDeduction);
                    //////vTotalLoan = Math.Round(vTotalLoan);
                    //////vTax = Math.Round(vTax);


                    //////vSalaryDeduction = Math.Round(vSalaryDeduction);
                    //////vTransportBill = Math.Round(vTransportBill);
                    //////vLateDeduction = Math.Round(vLateDeduction);
                    //////vPunishment = Math.Round(vPunishment);
                    //////vPunishment = Math.Round(vPunishment);

                    #endregion

                    dt.Rows[i]["Gross"] = vGross;
                    dt.Rows[i]["Arrear"] = vArrear;
                    dt.Rows[i]["LeaveEncash"] = vLeaveEncash;
                    dt.Rows[i]["ReimbursableExpense"] = vReimbursableExpense;
                    dt.Rows[i]["AbsentDeduction"] = vAbsentDeduction;
                    dt.Rows[i]["LeaveWOPay"] = vLeaveWOPay;
                    dt.Rows[i]["AdvanceDeduction"] = vAdvanceDeduction;
                    dt.Rows[i]["TotalLoan"] = vTotalLoan;
                    dt.Rows[i]["Tax"] = vTax;
                    dt.Rows[i]["SalaryDeduction"] = vSalaryDeduction;
                    dt.Rows[i]["TransportBill"] = vTransportBill;
                    dt.Rows[i]["LateDeduction"] = vLateDeduction;
                    dt.Rows[i]["Punishment"] = vPunishment;
                    dt.Rows[i]["BankPayAmount"] = vBankPayAmount;
                    dt.Rows[i]["NPAmount"] = vNPAmount;



                    ////////dt.Rows[i]["AbsentDeduction"] = vAbsentDeduction;


                    #endregion
                    #region Calculation

                    vBankSheetGross = vGross - vBankPayAmount; //Note vBankSheetGross = vCashPayGross
                    vCashPayGross = vBankSheetGross;

                    vOtherEarning = vArrear + vLeaveEncash + vReimbursableExpense;
                    vOtherDeduction = vSalaryDeduction + vTransportBill + vLateDeduction + vPunishment;

                    vTotalDeduction = vAbsentDeduction + vLeaveWOPay + vAdvanceDeduction + vTotalLoan + vTax;



                    vAllEarning = vBankSheetGross + vOtherEarning;
                    vAllDeduction = vTotalDeduction + vOtherDeduction;

                    if ((vAllEarning - vAllDeduction) < 0)
                    {
                        vBankSheetDeduction = vAllDeduction - vAllEarning;
                    }

                    vNetSalaryBank = vBankPayAmount - vBankSheetDeduction;
                    vNetSalaryBank = vNetSalaryBank > 0 ? vNetSalaryBank : 0;



                    vNetSalaryCash = vAllEarning - vAllDeduction;
                    vNetSalaryCash = vNetSalaryCash > 0 ? vNetSalaryCash : 0;


                    vNetSalary = vGross - vTotalDeduction + vOtherEarning - vOtherDeduction;
                    vNetSalary = vNetSalary > 0 ? vNetSalary : 0;



                    //////vNetSalary = Math.Round(vNetSalary);
                    //////vNetSalaryBank = Math.Round(vNetSalaryBank);
                    //////vNetSalaryCash = Math.Round(vNetSalaryCash);



                    dt.Rows[i]["NetSalary"] = vNetSalary;


                    dt.Rows[i]["NetSalaryBank"] = vNetSalaryBank;
                    dt.Rows[i]["NetSalaryCash"] = vNetSalaryCash;

                    dt.Rows[i]["BankSheetGross"] = vBankSheetGross;
                    dt.Rows[i]["CashPayGross"] = vCashPayGross;

                    dt.Rows[i]["OtherEarning"] = vOtherEarning;
                    dt.Rows[i]["TotalDeduction"] = vTotalDeduction;
                    dt.Rows[i]["OtherDeduction"] = vOtherDeduction;
                    dt.Rows[i]["AllEarning"] = vAllEarning;
                    dt.Rows[i]["AllDeduction"] = vAllDeduction;
                    dt.Rows[i]["BankSheetDeduction"] = vBankSheetDeduction;


                    #endregion

                    i++;
                }

                return dt;
            }
            catch (Exception)
            {

                return dt;
            }
        }

        public string[] TempSalaryProcess(SalarySheetVM vm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region TempSalaryProcess
                DataSet dataSet = new DataSet();
                string hrmDB = _dbsqlConnection.HRMDB; //_settingDal.settingValue("Database", "HRMDB").Trim()_settingDal.settingValue("Database", "HRMDB").Trim();
                vm.PeriodId = new FiscalYearDAL().FYPeriodDetail(vm.FiscalYearDetailId, currConn, transaction).FirstOrDefault().PeriodId;
                SqlCommand command = new SqlCommand("", currConn, transaction);

                sqlText = @"";
                sqlText += @" delete from  " + hrmDB + ".[dbo].TempSalary  ";
                sqlText += @" insert into  " + hrmDB + ".[dbo].TempSalary(EmployeeId,FiscalYearDetailId,ProjectId,DepartmentId,SectionId,DesignationId)";
                sqlText += @" Select s.EmployeeId,FiscalYearDetailId,s.ProjectId,s.DepartmentId,s.SectionId,s.DesignationId from  " + hrmDB + ".[dbo].SalaryEmployee s";
                sqlText += @" Left Outer Join  " + hrmDB + ".[dbo].EmployeeJob ej on ej.EmployeeId=s.EmployeeId";
                sqlText += @" where 1=1  AND s.IsHold=0  and s.FiscalYearDetailId=@FiscalYearDetailId and ej.EmployeeId Not in(Select EmployeeId from " + hrmDB + ".[dbo].EmployeeJob ";
                sqlText += @" where ej.JoinDate<'" + vm.PeriodId + "20' and  ej.JoinDate>'" + vm.PeriodId + "26')";
                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND s.ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }
                if (!string.IsNullOrWhiteSpace(vm.HoldStatus))
                {
                    if (vm.HoldStatus.ToLower() == "hold")
                    {
                        sqlText += " AND s.IsHold=1 ";
                    }
                    else
                    {
                        sqlText += " AND s.IsHold=0 ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(vm.ProjectId))
                    sqlText += " and s.ProjectId=@ProjectId";
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    sqlText += " and s.DepartmentId=@DepartmentId ";
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    sqlText += " and s.SectionId=@SectionId ";
                if (!string.IsNullOrWhiteSpace(vm.FiscalYearDetailId.ToString()) && vm.FiscalYearDetailId.ToString() != "0")
                    sqlText += " and s.FiscalYearDetailId=@FiscalYearDetailId ";
                //if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                //    sqlText += " and s.Code>= @CodeF";
                //if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                //    sqlText += " and s.Code<= @CodeT";


                command = new SqlCommand("", currConn, transaction);
                command.CommandText = sqlText;
                if (!string.IsNullOrWhiteSpace(vm.ProjectId) && vm.ProjectId != "0")
                    command.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    command.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    command.Parameters.AddWithValue("@SectionId", vm.SectionId);
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    command.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                if (!string.IsNullOrWhiteSpace(vm.FiscalYearDetailId.ToString()) && vm.FiscalYearDetailId.ToString() != "0")
                    command.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                //command.Parameters.AddWithValue("@FiscalYearDetailId",vm.FiscalYearDetailId);

                command.ExecuteNonQuery();

                command = new SqlCommand("", currConn, transaction);

                sqlText = @" ";

                sqlText += @" select distinct isnull(sl,9999),FieldGroup from  " + hrmDB + ".[dbo].SalaryHeadMappings where IsActive=1 and  isnull(groupType,'') not in('Sum') order by isnull(sl,9999) ";
                sqlText += @" select * from  " + hrmDB + ".[dbo].SalaryHeadMappings  where  IsActive=1 and isnull(groupType,'') not in('Sum') order by isnull(sl,9999) ";

                sqlText += @" select distinct isnull(sl,9999),FieldGroup from  " + hrmDB + ".[dbo].SalaryHeadMappings where  IsActive=1 and isnull(groupType,'')  in('Sum') order by isnull(sl,9999) ";
                sqlText += @" select * from  " + hrmDB + ".[dbo].SalaryHeadMappings  where  IsActive=1 and groupType  in('Sum') order by isnull(sl,9999) ";


                command.CommandText = sqlText;
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                adapter.Fill(dataSet);
                foreach (DataRow drFieldGroupNotSum in dataSet.Tables[0].Rows)
                {
                    string FieldGroup = "";
                    string FieldName = "";
                    FieldGroup = Convert.ToString(drFieldGroupNotSum["FieldGroup"]);
                    DataRow[] DetailRawsOthers = dataSet.Tables[1].Select("FieldGroup='" + FieldGroup + "'");

                    foreach (DataRow drFieldName in DetailRawsOthers)
                    {
                        FieldName = FieldName + "'" + drFieldName["FieldName"].ToString() + "',";
                    }
                    FieldName = FieldName.Trim().Substring(0, FieldName.Length - 1);
                    sqlText = @"";

                    sqlText += @" update  " + hrmDB + ".[dbo].TempSalary set [vFieldGroup] =a.Amount ";
                    sqlText += @" from( ";
                    sqlText += @" select distinct EmployeeId,FiscalYearDetailId,sum(isnull(Amount,0))Amount from  " + hrmDB + ".[dbo].ViewSalaryPreCalculation s  ";
                    sqlText += @" where  FiscalYearDetailId=@FiscalYearDetailId ";
                    sqlText += @" and s.SalaryHead in(vFieldName) ";
                    sqlText += @" group by EmployeeId,FiscalYearDetailId) ";
                    sqlText += @"  as a ";
                    sqlText += @"  where  " + hrmDB + ".[dbo].TempSalary.EmployeeId=a.EmployeeId and  " + hrmDB + ".[dbo].TempSalary.FiscalYearDetailId=a.FiscalYearDetailId";
                    sqlText = sqlText.Replace("vFieldGroup", FieldGroup);
                    sqlText = sqlText.Replace("vFieldName", FieldName);
                    command = new SqlCommand("", currConn, transaction);
                    command.CommandText = sqlText;
                    command.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    command.ExecuteNonQuery();

                }
                foreach (DataRow drFieldGroupSum in dataSet.Tables[2].Rows)
                {
                    string FieldGroup = "";
                    string FieldName = "";
                    FieldGroup = Convert.ToString(drFieldGroupSum["FieldGroup"]);
                    DataRow[] DetailRawsOthers = dataSet.Tables[3].Select("FieldGroup='" + FieldGroup + "'");

                    foreach (DataRow drFieldName in DetailRawsOthers)
                    {
                        FieldName = drFieldName["FieldName"].ToString();
                        //FieldName = 
                    }
                    FieldName = FieldName.Replace("+", "],0)+isnull([").Replace("-", "],0)-isnull([").Replace("*", "],0)*isnull([").Replace("/", "],0)/isnull([");
                    FieldName = "isnull([" + FieldName.Trim() + "],0)";
                    sqlText = @" ";

                    sqlText += @" update  " + hrmDB + ".[dbo].TempSalary set [vFieldGroup] =a.Amount ";
                    sqlText += @" from( ";
                    sqlText += @" select distinct EmployeeId,FiscalYearDetailId,(vFieldName)Amount from  " + hrmDB + ".[dbo].TempSalary s  ";
                    sqlText += @" where FiscalYearDetailId=@FiscalYearDetailId) ";
                    sqlText += @"  as a ";
                    sqlText += @"  where  " + hrmDB + ".[dbo].TempSalary.EmployeeId=a.EmployeeId and  " + hrmDB + ".[dbo].TempSalary.FiscalYearDetailId=a.FiscalYearDetailId";
                    sqlText = sqlText.Replace("vFieldGroup", FieldGroup);
                    sqlText = sqlText.Replace("vFieldName", FieldName);
                    command = new SqlCommand("", currConn, transaction);
                    command.CommandText = sqlText;
                    command.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                    command.ExecuteNonQuery();

                }
                #endregion TempSalaryProcess
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
                retResults[2] = "0";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("SaleSheetDAL", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

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

        private string FiscalYearDAL()
        {
            throw new NotImplementedException();
        }
        public DataSet SalarySheet_TIB(string[] conditionFields = null, string[] conditionValues = null, string ReportName = "SalarySheet1"
           , string vFiscalYearDetailId = "0", SalarySheetVM vm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction
                #region TempSalaryProcess
                vm.PeriodId = new FiscalYearDAL().FYPeriodDetail(vm.FiscalYearDetailId, currConn, transaction).FirstOrDefault().PeriodId;
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                #endregion TempSalaryProcess
                string[] retResults = TempSalaryProcess(vm, currConn, transaction);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("T SalaryProcess Fail for Current month  ", "");

                }
                #region sql statement
                //for G4s e.CarAllowance,                 
                sqlText += @"
 select s.*
 from View_TIBSalary s 
where 1=1
 and (s.FiscalYearDetailId = @FiscalYearDetailId) ";
                string cField = "";

                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND s.ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }
                if (!string.IsNullOrWhiteSpace(vm.HoldStatus))
                {
                    if (vm.HoldStatus.ToLower() == "hold")
                    {
                        sqlText += " AND s.IsHold=1 ";
                    }
                    else
                    {
                        sqlText += " AND s.IsHold=0 ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(vm.ProjectId))
                    sqlText += " and s.ProjectId=@ProjectId";
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    sqlText += " and s.DepartmentId=@DepartmentId ";
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    sqlText += " and s.SectionId=@SectionId ";
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    sqlText += " and s.DesignationId=@DesignationId ";
                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    sqlText += " and s.Code>= @CodeF";
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    sqlText += " and s.Code<= @CodeT";
                if (!string.IsNullOrWhiteSpace(vm.EmploymentType_E))
                    sqlText += " and e.EmploymentTypeId= @EmploymentType_E";
                //if (!string.IsNullOrWhiteSpace(vm.PeriodId) && vm.PeriodId != "0")
                //    sqlText += " and s.PeriodId=@PeriodId ";
                //if (!string.IsNullOrWhiteSpace(vm.BankId))
                //    sqlText += " and ej.BankInfo=@BankId ";


                if (ReportName.ToLower() == "salarysheet9")
                {
                    sqlText += " AND s.BankName not in ('Standard Chartered Bank') ";
                    if (CompanyName.ToLower() == "tib")
                    {
                        sqlText += "   AND (s.JoinDate <= @PeriodId + '20' OR s.JoinDate > @PeriodId + '25')";
                    }
                }
                else if (ReportName.ToLower() == "salarysheet10")
                {
                    sqlText += " AND s.BankName ='Standard Chartered Bank' ";
                    if (CompanyName.ToLower() == "tib")
                    {
                        sqlText += "   AND (s.JoinDate <= @PeriodId + '20' OR s.JoinDate > @PeriodId + '25')";
                    }

                }
                else if (ReportName.ToLower() == "salarysheet1")
                {
                    if (CompanyName.ToLower() == "tib")
                    {
                        sqlText += "   AND (s.JoinDate <= @PeriodId + '20' OR s.JoinDate > @PeriodId + '25')";
                    }

                }
                else if (ReportName.ToLower() == "salarysheet16")
                {
                    sqlText += "  and   s.JoinDate>@PeriodId+'20' and  s.JoinDate<@PeriodId+'26' ";

                }

                #region Order By
                if (ReportName.ToLower() != "salarysheet2" && ReportName.ToLower() != "salarysheet3")
                {
                    if (!string.IsNullOrWhiteSpace(vm.Orderby))
                    {
                        if (vm.Orderby == "DCG")
                            sqlText += " order by s.Department, s.code";
                        else if (vm.Orderby == "DDC")
                            sqlText += " order by s.Department, s.JoinDate, s.code";
                        else if (vm.Orderby == "DGC")
                            sqlText += " order by s.Department, s.code";
                        else if (vm.Orderby == "DGDC")
                            sqlText += " order by  s.Department, s.JoinDate, s.code";
                        else if (vm.Orderby == "CODE")
                            if (CompanyName == "G4S")
                            {
                                sqlText += " order by [S.N.], s.G4SID, s.Section,s.Expr1, s.OrderNo,s.NetPayment";
                            }
                            else
                            {
                                sqlText += " order by  s.Section,s.Expr1, s.OrderNo,s.NetPayment";
                            }
                    }
                }
                #endregion
                if (ReportName.ToLower() == "salarysheet2")//Salary Summary(Designation)
                {
                    sqlText += @" ) as a group by Designation,FiscalYearDetailId Order by Designation";
                }
                else if (ReportName.ToLower() == "salarysheet3")//Salary Summary(Section)
                {
                    sqlText += @" ) as a group by Section,FiscalYearDetailId  Order by Section";
                }
                sqlText += @" select  distinct Code, Employeeid,HeadType, SalaryHead,Amount  from ViewSalaryPreCalculation a
where  FiscalYearDetailId=@FiscalYearDetailId
and SalaryHead in(select FieldName from SalaryHeadMappings where FieldGroup='OtherAdjustment')
order by a.HeadType, a.SalaryHead";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                // da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);

                if (!string.IsNullOrWhiteSpace(vm.ProjectId) && vm.ProjectId != "0")
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                if (!string.IsNullOrWhiteSpace(vm.DepartmentId) && vm.DepartmentId != "0")
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                if (!string.IsNullOrWhiteSpace(vm.SectionId) && vm.SectionId != "0")
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", vm.SectionId);
                if (!string.IsNullOrWhiteSpace(vm.DesignationId) && vm.DesignationId != "0")
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", vm.DesignationId);

                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeTo);
                if (!string.IsNullOrWhiteSpace(vm.EmploymentType_E))
                    da.SelectCommand.Parameters.AddWithValue("@EmploymentType_E", vm.EmploymentType_E);
                if (!string.IsNullOrWhiteSpace(vm.PeriodId) && vm.PeriodId != "0")
                    da.SelectCommand.Parameters.AddWithValue("@PeriodId", vm.PeriodId);
                da.Fill(ds);
                ////dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

                #region Value Round

                if (vm.FiscalYearDetailId > 1080)
                {
                    string[] columnNames ={"Basic","HouseRent","Medical","TransportAllowance","Gross","TAX","TransportBill","Stamp","PFEmployer"
                        ,"PFLoan","STAFFWELFARE","DeductionTotal","NetSalary","Vehicle(Adj)","Other(Bonus)"
                        ,"LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment"
                        ,"NetPayment"};
                    DataTable dt1 = Ordinary.DtValueRound(ds.Tables[0], columnNames);
                    dt1 = Ordinary.DtColumnStringToDate(dt1, "JoinDate");
                    dt1 = Ordinary.DtColumnStringToDate(dt1, "LeftDate");
                    DataTable dt2 = ds.Tables[1];

                    //DataTable dt1Copy = dt1.Copy();
                    //DataTable dt2Copy = dt2.Copy();

                    ds = new DataSet();
                    ds.Tables.Add(dt1.Copy());
                    ds.Tables.Add(dt2.Copy());
                }


                #endregion

            }
            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("SaleSheetDAL", this.GetType().Name, sqlex.Message + Environment.NewLine + sqlex.StackTrace);

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("SaleSheetDAL", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

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
            return ds;
        }

        public DataSet SalarySheet_TIB_Excel(SalarySheetVM vm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    string[] retResults = TempSalaryProcess(vm, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("T SalaryProcess Fail for Current month  ", "");

                    }
                }

                string getRegularEmployee = @"


select
distinct 
FiscalYearDetailId
,ProjectId
,SectionId
,DesignationId
,sum(isnull(Basic,0))					[Basic]
,sum(isnull(HouseRent					,0))HouseRent
,sum(isnull(Medical					,0))Medical
,sum(isnull(TransportAllowance			,0))TransportAllowance
,sum(isnull(Gross						,0))Gross
,sum(isnull(TAX						,0))TAX
,sum(isnull(TransportBill				,0))TransportBill
,sum(isnull(Stamp						,0))Stamp
,sum(isnull(PFEmployer					,0))PFEmployer
,sum(isnull(PFLoan						,0))PFLoan
,sum(isnull(STAFFWELFARE				,0))STAFFWELFARE
,sum(isnull(DeductionTotal				,0))DeductionTotal
,sum(isnull(NetSalary					,0))NetSalary
,sum(isnull([Othere(OT)]				,0))[Othere(OT)]
,sum(isnull([Vehicle(Adj)]				,0))[Vehicle(Adj)]
,sum(isnull([Other(Bonus)]				,0))[Other(Bonus)]
,sum(isnull(LeaveWOPay					,0))LeaveWOPay
,sum(isnull(GP							,0))GP
,sum(isnull(Travel						,0))Travel
,sum(isnull(ChildAllowance				,0))ChildAllowance
,sum(isnull([MOBILE(Allowance)]		,0))[MOBILE(Allowance)]
,sum(isnull(OtherAdjustment			,0))OtherAdjustment
,sum(isnull(TotalAdjustment			,0))TotalAdjustment
,sum(isnull(NetPayment					,0))NetPayment
from TempSalary 
 where DesignationId  not in( select DesignationId from SectionDesignationRatio)
group by FiscalYearDetailId,ProjectId,SectionId,DesignationId";

                string getSpecialEmployee = @"

select
distinct 
FiscalYearDetailId
,ProjectId
,SectionId
,DesignationId
,sum(isnull(Basic,0))					[Basic]
,sum(isnull(HouseRent					,0))HouseRent
,sum(isnull(Medical					,0))Medical
,sum(isnull(TransportAllowance			,0))TransportAllowance
,sum(isnull(Gross						,0))Gross
,sum(isnull(TAX						,0))TAX
,sum(isnull(TransportBill				,0))TransportBill
,sum(isnull(Stamp						,0))Stamp
,sum(isnull(PFEmployer					,0))PFEmployer
,sum(isnull(PFLoan						,0))PFLoan
,sum(isnull(STAFFWELFARE				,0))STAFFWELFARE
,sum(isnull(DeductionTotal				,0))DeductionTotal
,sum(isnull(NetSalary					,0))NetSalary
,sum(isnull([Othere(OT)]				,0))[Othere(OT)]
,sum(isnull([Vehicle(Adj)]				,0))[Vehicle(Adj)]
,sum(isnull([Other(Bonus)]				,0))[Other(Bonus)]
,sum(isnull(LeaveWOPay					,0))LeaveWOPay
,sum(isnull(GP							,0))GP
,sum(isnull(Travel						,0))Travel
,sum(isnull(ChildAllowance				,0))ChildAllowance
,sum(isnull([MOBILE(Allowance)]		,0))[MOBILE(Allowance)]
,sum(isnull(OtherAdjustment			,0))OtherAdjustment
,sum(isnull(TotalAdjustment			,0))TotalAdjustment
,sum(isnull(NetPayment					,0))NetPayment
from TempSalary 
 where DesignationId  in( select DesignationId from SectionDesignationRatio)
group by FiscalYearDetailId,ProjectId,SectionId,DesignationId ";




                string getEmployeeRatio = " select   * from SectionDesignationRatio";
                string getAlltempData = @"select  

isnull(Basic,0)					[Basic]
,isnull(HouseRent					,0)HouseRent
,isnull(Medical					,0)Medical
,isnull(TransportAllowance			,0)TransportAllowance
,isnull(Gross						,0)Gross
,isnull(TAX						,0)TAX
,isnull(TransportBill				,0)TransportBill
,isnull(Stamp						,0)Stamp
,isnull(PFEmployer					,0)PFEmployer
,isnull(PFLoan						,0)PFLoan
,isnull(STAFFWELFARE				,0)STAFFWELFARE
,isnull(DeductionTotal				,0)DeductionTotal
,isnull(NetSalary					,0)NetSalary
,isnull([Othere(OT)]				,0)[Othere(OT)]
,isnull([Vehicle(Adj)]				,0)[Vehicle(Adj)]
,isnull([Other(Bonus)]				,0)[Other(Bonus)]
,isnull(LeaveWOPay					,0)LeaveWOPay
,isnull(GP							,0)GP
,isnull(Travel						,0)Travel
,isnull(ChildAllowance				,0)ChildAllowance
,isnull([MOBILE(Allowance)]		,0)[MOBILE(Allowance)]
,isnull(OtherAdjustment			,0)OtherAdjustment
,isnull(TotalAdjustment			,0)TotalAdjustment
,isnull(NetPayment					,0)NetPayment
      ,[ProjectId]
      ,[DepartmentId]
      ,[SectionId]
      ,[DesignationId]
	  ,[Id]
      ,[FiscalYearDetailId]
      ,[EmployeeId]
from TempSalary ts";

                string finalQuery = getRegularEmployee + getSpecialEmployee + getEmployeeRatio;
                string deleteTempSalary = "delete from TempSalary";
                CommonDAL commonDal = new CommonDAL();

                SqlCommand cmd = new SqlCommand(getAlltempData, currConn,
                    transaction);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dsEmployees = new DataSet();
                adapter.Fill(dsEmployees);

                if (vm != null && vm.FiscalYearDetailId > 1080)
                {
                    string[] columnNames ={"Basic","HouseRent","Medical","TransportAllowance","Gross","TAX","TransportBill","Stamp","PFEmployer"
                        ,"PFLoan","STAFFWELFARE","DeductionTotal","NetSalary","Othere(OT)","Vehicle(Adj)","Other(Bonus)"
                        ,"LeaveWOPay","GP","Travel","ChildAllowance","MOBILE(Allowance)","OtherAdjustment","TotalAdjustment"
                        ,"NetPayment"};
                    DataTable dt1 = Ordinary.DtValueRound(dsEmployees.Tables[0], columnNames);

                    //DataTable dt1Copy = dt1.Copy();
                    //DataTable dt2Copy = dt2.Copy();

                    dsEmployees = new DataSet();
                    dsEmployees.Tables.Add(dt1.Copy());
                }

                cmd.CommandText = deleteTempSalary;
                cmd.ExecuteNonQuery();
                string[] result = commonDal.BulkInsert("TempSalary", dsEmployees.Tables[0], currConn, transaction);


                cmd.CommandText = finalQuery;
                dsEmployees = new DataSet();
                adapter.Fill(dsEmployees);

                cmd.CommandText = deleteTempSalary;
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("TempSalary", dsEmployees.Tables[0], currConn, transaction);


                DataTable dtFinalRatio = new DataTable();
                int rowsCount = dsEmployees.Tables[2].Rows.Count;

                for (int index = 0; index < rowsCount; index++)
                {
                    DataRow dataRow = dsEmployees.Tables[2].Rows[index];

                    DataRow[] rows = dsEmployees.Tables[1].Select("DesignationId='" +
                                                         dataRow["DesignationId"] + "'");

                    if (rows.Any())
                    {
                        DataTable dtTemp = rows.CopyToDataTable();

                        for (int i = 0; i < dtTemp.Rows.Count; i++)
                        {
                            DataRow dtTempRow = dtTemp.Rows[i];

                            dtTempRow["Basic"] = Convert.ToDecimal(dtTempRow["Basic"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["HouseRent"] = Convert.ToDecimal(dtTempRow["HouseRent"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Medical"] = Convert.ToDecimal(dtTempRow["Medical"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TransportAllowance"] = Convert.ToDecimal(dtTempRow["TransportAllowance"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Gross"] = Convert.ToDecimal(dtTempRow["Gross"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TAX"] = Convert.ToDecimal(dtTempRow["TAX"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TransportBill"] = Convert.ToDecimal(dtTempRow["TransportBill"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["PFEmployer"] = Convert.ToDecimal(dtTempRow["PFEmployer"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["STAFFWELFARE"] = Convert.ToDecimal(dtTempRow["STAFFWELFARE"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["DeductionTotal"] = Convert.ToDecimal(dtTempRow["DeductionTotal"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["NetSalary"] = Convert.ToDecimal(dtTempRow["NetSalary"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Othere(OT)"] = Convert.ToDecimal(dtTempRow["Othere(OT)"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Vehicle(Adj)"] = Convert.ToDecimal(dtTempRow["Vehicle(Adj)"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Other(Bonus)"] = Convert.ToDecimal(dtTempRow["Other(Bonus)"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["LeaveWOPay"] = Convert.ToDecimal(dtTempRow["LeaveWOPay"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["GP"] = Convert.ToDecimal(dtTempRow["GP"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Travel"] = Convert.ToDecimal(dtTempRow["Travel"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["ChildAllowance"] = Convert.ToDecimal(dtTempRow["ChildAllowance"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["MOBILE(Allowance)"] = Convert.ToDecimal(dtTempRow["MOBILE(Allowance)"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["OtherAdjustment"] = Convert.ToDecimal(dtTempRow["OtherAdjustment"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["TotalAdjustment"] = Convert.ToDecimal(dtTempRow["TotalAdjustment"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["NetPayment"] = Convert.ToDecimal(dtTempRow["NetPayment"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);
                            dtTempRow["Stamp"] = Convert.ToDecimal(dtTempRow["Stamp"]) * (Convert.ToDecimal(dataRow["Ratio"]) / 100);

                            dtTempRow["SectionId"] = dataRow["SectionId"];

                        }

                        dtFinalRatio.Merge(dtTemp);
                    }


                }

                result = commonDal.BulkInsert("TempSalary", dtFinalRatio, currConn, transaction);

                sqlText = @"
select distinct 
fs.PeriodName
,p.Id ProjectId
,p.Name Project
,st.Id SectionId
,st.Name Section
,st.OrderNo SectionOrderNo
,dgg.Id DesignationId
,dgg.Name Designation
,dgg.Serial DesignationOrderNo
,sum(isnull(Basic,0))					[Basic]
,sum(isnull(HouseRent					,0))HouseRent
,sum(isnull(Medical					,0))Medical
,sum(isnull(TransportAllowance			,0))TransportAllowance
,sum(isnull(Gross						,0))Gross
,sum(isnull(TAX						,0))TAX
,sum(isnull(TransportBill				,0))TransportBill
,sum(isnull(Stamp						,0))Stamp
,sum(isnull(PFEmployer					,0))PFEmployer
,sum(isnull(PFLoan						,0))PFLoan
,sum(isnull(STAFFWELFARE				,0))STAFFWELFARE
,sum(isnull(DeductionTotal				,0))DeductionTotal
,sum(isnull(NetSalary					,0))NetSalary
,sum(isnull([Othere(OT)]				,0))[Othere(OT)]
,sum(isnull([Vehicle(Adj)]				,0))[Vehicle(Adj)]
,sum(isnull([Other(Bonus)]				,0))[Other(Bonus)]
,sum(isnull(LeaveWOPay					,0))LeaveWOPay
,sum(isnull(GP							,0))GP
,sum(isnull(Travel						,0))Travel
,sum(isnull(ChildAllowance				,0))ChildAllowance
,sum(isnull([MOBILE(Allowance)]		,0))[MOBILE(Allowance)]
,sum(isnull(OtherAdjustment			,0))OtherAdjustment
,sum(isnull(TotalAdjustment			,0))TotalAdjustment
,sum(isnull(NetPayment					,0))NetPayment
from TempSalary ts left outer join  Section st on ts.SectionId = st.Id
left outer join Project p on ts.ProjectId = p.Id 
left outer join FiscalYearDetail fs on ts.FiscalYearDetailId = fs.Id
left outer join Designation dg on ts.DesignationId = dg.Id
left outer join DesignationGroup dgg on dg.DesignationGroupId = dgg.Id
  Group by 
 fs.PeriodName
,p.Id
,p.Name
,st.Id
,st.Name
,st.OrderNo
,dgg.Id
,dgg.Name
,dgg.Serial


";

                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND p.id IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }
                //if (!string.IsNullOrWhiteSpace(vm.HoldStatus))
                //{
                //    if (vm.HoldStatus.ToLower() == "hold")
                //    {
                //        sqlText += " AND s.IsHold=1 ";
                //    }
                //    else
                //    {
                //        sqlText += " AND s.IsHold=0 ";
                //    }
                //}

                //if (vm.PeriodId !=null)
                //{
                //    sqlText += "  and    e.JoinDate>@PeriodId+'20' and  e.JoinDate<@PeriodId+'26' ";

                //}

                cmd.CommandText = sqlText;
                adapter.SelectCommand = cmd;
                dsEmployees = new DataSet();
                adapter.Fill(dsEmployees);




                #region MyRegion
                //                DataTable dtTemp1 = dsEmployees.Tables[0];

                //                string[] columnDelete = { "PeriodName", "Project", "Section", "SectionOrderNo", "Designation", "DesignationOrderNo" };
                //                dtTemp1 = Ordinary.DtDeleteColumns(dtTemp1, columnDelete);
                //                cmd.CommandText = deleteTempSalary;
                //                cmd.ExecuteNonQuery();
                //                result = commonDal.BulkInsert("TempSalary", dtTemp1, currConn, transaction);

                //                string sqlSalarySageAccountHeadMaping = "select * from SalarySageAccountHeadMaping";

                //                SqlCommand cmdSsahm = new SqlCommand(sqlSalarySageAccountHeadMaping, currConn, transaction);
                //                SqlDataAdapter adapterSsahm = new SqlDataAdapter(cmdSsahm);
                //                DataSet dsSsahm = new DataSet();
                //                adapterSsahm.Fill(dsSsahm);
                //                foreach (DataRow item in dsSsahm.Tables[0].Rows)
                //                {
                //                    string SectionId = item["SectionId"].ToString();
                //                    string DesignationGroupId = item["DesignationGroupId"].ToString();
                //                    string AccountHead = item["AccountHead"].ToString();
                //                    string SalaryHead = item["SalaryHead"].ToString();
                //                    string IsCredit = item["IsCredit"].ToString();


                //                    string sqlSelectText = @"select sum("+ SalaryHead +@") SalaryValue from TempSalary where  
                //                                        SectionId='" + SectionId + "' and DesignationId='"+DesignationGroupId+"'";

                //                      cmdSsahm = new SqlCommand(sqlSelectText, currConn, transaction);
                //                      adapterSsahm = new SqlDataAdapter(cmdSsahm);
                //                      DataTable dtTemp2 = new DataTable();
                //                      adapterSsahm.Fill(dtTemp2);

                //                    //AccountHead
                //                    //DRAmount
                //                    //CrAmount



                //                }

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }


                return dsEmployees;
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

        }


        public ResultVM PostToSage(SalarySheetVM vm = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            CommonDAL commonDal = new CommonDAL();
            SettingDAL settingDal = new SettingDAL();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction


                DataSet dsEmployees = SalarySheet_TIB_Excel(vm, currConn, transaction);


                string deleteTempSalary = "delete from TempSalary";

                SqlCommand cmd = new SqlCommand("", currConn, transaction);
                DataTable dtTemp1 = dsEmployees.Tables[0];

                string[] columnDelete = { "PeriodName", "Project", "Section", "SectionOrderNo", "Designation", "DesignationOrderNo" };
                dtTemp1 = Ordinary.DtDeleteColumns(dtTemp1, columnDelete);
                cmd.CommandText = deleteTempSalary;
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("TempSalary", dtTemp1, currConn, transaction);

                string sqlSalarySageAccountHeadMaping = "select * from SalarySageAccountHeadMaping";

                SqlCommand cmdSsahm = new SqlCommand(sqlSalarySageAccountHeadMaping, currConn, transaction);
                SqlDataAdapter adapterSsahm = new SqlDataAdapter(cmdSsahm);
                DataSet dsSsahm = new DataSet();
                adapterSsahm.Fill(dsSsahm);

                DataTable dtJournal = new DataTable();

                dtJournal.Columns.Add("SectionId");
                dtJournal.Columns.Add("DesignationGroupId");
                dtJournal.Columns.Add("AccountHead");
                dtJournal.Columns.Add("SalaryHead");
                dtJournal.Columns.Add("IsCredit");
                dtJournal.Columns.Add("Amount");

                int i = 0;
                //decimal sum = 0;
                foreach (DataRow item in dsSsahm.Tables[0].Rows)
                {
                    try
                    {
                        string SectionId = item["SectionId"].ToString();
                        string DesignationGroupId = item["DesignationGroupId"].ToString();
                        string AccountHead = item["AccountHead"].ToString();
                        string SalaryHead = item["SalaryHead"].ToString();
                        string IsCredit = item["IsCredit"].ToString();


                        string sqlSelectText = @"select sum(" + SalaryHead + @") SalaryValue from TempSalary where  
                                        SectionId='" + SectionId;

                        if (!string.IsNullOrEmpty(DesignationGroupId))
                        {
                            sqlSelectText += "' and DesignationId='" + DesignationGroupId + "'";
                        }

                        cmdSsahm = new SqlCommand(sqlSelectText, currConn, transaction);
                        adapterSsahm = new SqlDataAdapter(cmdSsahm);
                        DataTable dtTemp2 = new DataTable();
                        adapterSsahm.Fill(dtTemp2);


                        if (string.IsNullOrEmpty(dtTemp2.Rows[0][0].ToString()))
                            continue;

                        dtJournal.Rows.Add(dtJournal.NewRow());

                        dtJournal.Rows[i]["SectionId"] = SectionId;
                        dtJournal.Rows[i]["DesignationGroupId"] = DesignationGroupId;
                        dtJournal.Rows[i]["AccountHead"] = AccountHead;
                        dtJournal.Rows[i]["SalaryHead"] = SalaryHead;
                        dtJournal.Rows[i]["IsCredit"] = IsCredit;
                        dtJournal.Rows[i]["Amount"] = dtTemp2.Rows[0][0];

                        i++;

                        //sum += Convert.ToDecimal(dtTemp2.Rows[0][0]);
                    }
                    catch (Exception)
                    {
                    }

                }

                JournalHeaders journalHeaders = new JournalHeaders
                {
                    BatchDescription = "Salary Entry",
                    BatchNo = "000035",
                    Currency = settingDal.settingValue("Sage", "Currency", currConn, transaction),
                    DocumentDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                    PostingDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                    FiscalPeriod = DateTime.Now.ToString("MM"),
                    FiscalYear = DateTime.Now.ToString("yyyy"),
                    Password = settingDal.settingValue("Sage", "Password", currConn, transaction),
                    Username = settingDal.settingValue("Sage", "Username", currConn, transaction),
                    SourceType = settingDal.settingValue("Sage", "SourceType", currConn, transaction)
                };


                foreach (DataRow dtJournalRow in dtJournal.Rows)
                {
                    journalHeaders.GlDetails.Add(new JournalDetailsModel()
                    {
                        AccountNo = dtJournalRow["AccountHead"].ToString(),
                        Amount = dtJournalRow["IsCredit"] == "1" ? Convert.ToDecimal(dtJournalRow["Amount"]) * -1 : Convert.ToDecimal(dtJournalRow["Amount"]),
                        LineComment = "-",
                        LineDescription = "-",

                    });
                }


                HttpRequestHelper httpRequestHelper = new HttpRequestHelper();
                AuthModel authModel = httpRequestHelper.GetAuthentication(new CredentialModel { UserName = "admin", Password = "admin", ApiKey = ConfigurationManager.AppSettings["apiKey"] });
                string sageJson = httpRequestHelper.PostData("/api/GLJournal/CreateGLEntry", authModel, JsonConvert.SerializeObject(journalHeaders));
                RootModel<Result> root = JsonConvert.DeserializeObject<RootModel<Result>>(sageJson);


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }


                ResultVM resultVm = new ResultVM();

                resultVm.Status = root.StatusCode.StartsWith("4") ? "Fail" : "Success";
                resultVm.Message = root.StatusCode.StartsWith("4") ? "Something gone wrong" : "Data has been sent to sage";

                return resultVm;

            }
            #region catch

            catch (Exception ex)
            {
                throw ex;
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

        }



        public DataSet SalarySheet_TIB_SummeryOther(SalarySheetVM vm = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    string[] retResults = TempSalaryProcess(vm, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("T SalaryProcess Fail for Current month  ", "");

                    }
                }
                sqlText = @"

select distinct Project, Section,SectionOrder,DesignationGroup.Name Designation,DesignationGroup.Serial DesignationOrder,SalaryHead,sum(Amount)Amount 
from ViewSalaryPreCalculation left outer join DesignationGroup
on ViewSalaryPreCalculation.designationGroupId = DesignationGroup.ID

where 1=1
and  SalaryHead in(
select FieldName from SalaryHeadMappings where FieldGroup='OtherAdjustment')


";

                //and FiscalYearDetailId='1078'



                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }
                sqlText += @" group by Project, Section,SectionOrder,DesignationGroup.Name,DesignationGroup.Serial,SalaryHead
having sum(Amount)<>0
order by Project, SectionOrder,DesignationOrder,SalaryHead
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(ds);



                if (Vtransaction == null)
                {
                    transaction.Commit();
                }


                return ds;
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

        }
        public DataSet SalarySheet_TIB_SummeryOtherDownload(SalarySheetVM vm = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, DataTable DtResult = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Report"); }

                #endregion open connection and transaction
                #region Bulk Insert
                #region CreateTemp

                string CreateTemp = @"create table #Temp
                             (
                                  Project nvarchar(100)
                                  ,Section nvarchar(100)
                                  ,SectionOrder int
                             	 , Designation  nvarchar(100)
                             	 , DesignationOrder  int
                             	 , SalaryHead  nvarchar(100)
                                 , Amount decimal(18, 2)
                             )";
                SqlCommand cmd = new SqlCommand(CreateTemp, currConn, transaction);
                cmd.ExecuteNonQuery();
                CommonDAL commonDal = new CommonDAL();
                string[] result = commonDal.BulkInsert("#Temp", DtResult, currConn, transaction);
                #endregion

                #endregion

                sqlText = @"
 
SELECT * FROM (
  SELECT
   distinct Project, Section,SectionOrder,Designation,DesignationOrder,SalaryHead,isnull(Amount,0)Amount
  FROM #Temp
  where 1=1

";
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                if (vm.ProjectIdList != null && vm.ProjectIdList.Count > 0 && !string.IsNullOrWhiteSpace(vm.ProjectIdList.FirstOrDefault()))
                {
                    string sqlTextCondition = "";
                    sqlText += "  AND ProjectId IN(";
                    foreach (string item in vm.ProjectIdList)
                    {
                        sqlTextCondition += "'" + item + "',";
                    }
                    sqlTextCondition = sqlTextCondition.Trim(',');
                    sqlText += sqlTextCondition + ")";
                }
                sqlText += @" ) StudentResults
PIVOT (
  sum(Amount)
  FOR SalaryHead
  IN (
[AbsentDeduction]
,[Adj.forSalaryProgramme]
,[Arrear]
,[AssetsLostAdj.]
,[BasicAdj]
,[CPFAdj]
,[FestivalAllowanceAdj]
,[HARDSHIP]
,[HardshipAllowanceAdj]
,[HouserentAdj]
,[LeaveEncash]
,[MedialAllow.Adj]
,[Other(Deduction)]
,[Other(Earning)]
,[Other(PF)]
,[Other(Salary)]
,[Other(StaffWF)]
,[PayableAdj.InsurancePremium]
,[PreEmploymentCheckUp]
,[Punishment]
,[ReimbursableExpense]
,[SalaryAdj]
,[TaxConsultantFee]
,[TaxDues]
,[Welfare/EnjoyedExcessEL/ELAdjustment]
  )
) AS PivotTable

where (
[AbsentDeduction]							<>0
or [Adj.forSalaryProgramme]					<>0
or [Arrear]									<>0
or [AssetsLostAdj.]							<>0
or [BasicAdj]								<>0
or [CPFAdj]									<>0
or isnull([FestivalAllowanceAdj],0)					<>0
or [HARDSHIP]								<>0
or [HardshipAllowanceAdj]					<>0
or [HouserentAdj]							<>0
or [LeaveEncash]							<>0
or [MedialAllow.Adj]						<>0
or [Other(Deduction)]						<>0
or [Other(Earning)]							<>0
or [Other(PF)]								<>0
or [Other(Salary)]							<>0
or [Other(StaffWF)]							<>0
or [PayableAdj.InsurancePremium]			<>0
or [PreEmploymentCheckUp]					<>0
or [Punishment]								<>0
or [ReimbursableExpense]					<>0
or [SalaryAdj]								<>0
or [TaxConsultantFee]						<>0
or [TaxDues]								<>0
or [Welfare/EnjoyedExcessEL/ELAdjustment]	<>0
)

DROP TABLE #Temp
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }


                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(ds);



                if (Vtransaction == null)
                {
                    transaction.Commit();
                }


                return ds;
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

        }


        public DataTable DataExportForSunTemplate(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;


                #region sql statement

                sqlText = @"

DECLARE @PeriodName AS varchar(100)
DECLARE @PeriodYear AS varchar(100)

SELECT @PeriodName=UPPER(LEFT(PeriodName, 3)), @PeriodYear=[Year]
FROM FiscalYearDetail
WHERE Id =@FiscalYearDetailId

SELECT
	CASE 
	WHEN upvt.[Transaction Reference] = 'BASIC SALARY' THEN '641110'
	WHEN upvt.[Transaction Reference] = 'HOUSE RENT ALLOWANCE' THEN '641120'
	WHEN upvt.[Transaction Reference] = 'SPECIAL ALLOWANCE' THEN '641475'
	WHEN upvt.[Transaction Reference] = 'PROVIDENT FUND-EMPLYR CONT' THEN '645300'
	WHEN upvt.[Transaction Reference] = 'FESTIVAL BONUS PROVISION EXP' THEN '641100'
	WHEN upvt.[Transaction Reference] = 'ANNUAL LEAVE PROVISION EXP' THEN '641150'
	WHEN upvt.[Transaction Reference] = 'GRATUITY FUND PROVISION EXP' THEN '645310'
	WHEN upvt.[Transaction Reference] = 'MEDICAL EXP PROVISION EXP' THEN '645110'
	WHEN upvt.[Transaction Reference] = 'TDS FROM SALARY' THEN '442820'
	WHEN upvt.[Transaction Reference] = 'SALARY PAYABLE' THEN 'WD00007'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR PROVIDENT FUND' THEN '437000'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR FESTIVAL BONUS' THEN '428600'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR ANNUAL LEAVE' THEN '428200'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR GRATUITY' THEN '437010'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR MEDICAL RMBRSMT' THEN '428620'
	
	
	END as  [Account code]

    ,'SALARY '+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName AS Description
    ,upvt.[Transaction Reference]
   ,e.Code [Extra Reference]
   ,''[Transaction Date]
    ,'BDT'[Currency Code]
	 , upvt.[Transaction Amount]
   ,upvt.[Transaction Amount][Base Amount]
  ,s.Branch [BRANCHE T1]
  ,s.Department [ACTIVITY T2]
  ,e.TPNCode[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,e.DCode [EMPLOYEE CODE T8]
  ,''[T9]
  
FROM View_TIBSalary s
LEFT OUTER JOIN ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
--LEFT OUTER JOIN GLAccount GLA ON upvt.[Transaction Reference] = GLA.[GLAccountType]
CROSS APPLY (
    VALUES
        ('BASIC SALARY', s.Basic*-1),
        ('HOUSE RENT ALLOWANCE', s.HouseRent*-1),
        ('SPECIAL ALLOWANCE', s.TransportAllowance*-1),
        ('PROVIDENT FUND-EMPLYR CONT', s.PFEmployer*-1),
        ('FESTIVAL BONUS PROVISION EXP', (s.Basic*25/100)*-1),
        ('ANNUAL LEAVE PROVISION EXP', ((s.Gross/30 * 15) / 12)*-1),
        ('GRATUITY FUND PROVISION EXP', (s.Basic/ 12*-1)),
        ('MEDICAL EXP PROVISION EXP', CASE
                                          WHEN s.Grade ='C3' OR s.Grade ='C2' OR s.Grade ='C1' OR s.Grade ='A1' OR s.Grade ='A2' OR s.Grade ='A3' OR s.Grade ='B1-M3' OR s.Grade ='B1-M1' THEN
                                              CASE WHEN s.Basic<50000 THEN (s.Basic/12)*-1 ELSE (50000/12)* -1 END 
                                          ELSE ((10000) / 12) * -1
                                      END),
        ('TDS FROM SALARY', s.TAX),
        ('SALARY PAYABLE', s.Gross - s.PFEmployer),
        ('PROVISION FOR PROVIDENT FUND', s.PFEmployer * 2),
        ('PROVISION FOR FESTIVAL BONUS', (s.Basic*25/100)),
        ('PROVISION FOR ANNUAL LEAVE', (s.Gross/30 * 15) / 12),
        ('PROVISION FOR GRATUITY', s.Basic/ 12),
        ('PROVISION FOR MEDICAL RMBRSMT', CASE
                                              WHEN s.Grade ='C3' OR s.Grade ='C2' OR s.Grade ='C1' OR s.Grade ='A1' OR s.Grade ='A2' OR s.Grade ='A3' OR s.Grade ='B1-M3' OR s.Grade ='B1-M1' THEN
                                                  CASE WHEN s.Basic<50000 THEN (s.Basic/12) ELSE (50000/12) END
                                              ELSE ((10000) / 12) 
                                          END)
) AS upvt([Transaction Reference], [Transaction Amount])
WHERE s.FiscalYearDetailId =@FiscalYearDetailId
    AND s.IsHold = 0
    --AND s.Code = '11450'
";

                #region More Conditions
                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                {
                    sqlText += "  and s.Code>= @CodeF";
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                {
                    sqlText += " and s.Code<= @CodeT";
                }


                #endregion

                sqlText += "    ORDER BY upvt.[Transaction Reference],s.Code";



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeTo);
                }
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                da.Fill(dt);




                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable DataExportForSunTemplatePayment(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;


                #region sql statement

                sqlText = @"

DECLARE @PeriodName AS varchar(100)
DECLARE @PeriodYear AS varchar(100)

SELECT @PeriodName=UPPER(LEFT(PeriodName, 3)), @PeriodYear=[Year]
FROM FiscalYearDetail
WHERE Id =@FiscalYearDetailId;

with cat as(
SELECT
	CASE 
	WHEN upvt.[Transaction Reference] = 'BASIC SALARY' THEN '641110'
	WHEN upvt.[Transaction Reference] = 'HOUSE RENT ALLOWANCE' THEN '641120'
	WHEN upvt.[Transaction Reference] = 'SPECIAL ALLOWANCE' THEN '641475'
	WHEN upvt.[Transaction Reference] = 'PROVIDENT FUND-EMPLYR CONT' THEN '645300'
	WHEN upvt.[Transaction Reference] = 'FESTIVAL BONUS PROVISION EXP' THEN '641100'
	WHEN upvt.[Transaction Reference] = 'ANNUAL LEAVE PROVISION EXP' THEN '641150'
	WHEN upvt.[Transaction Reference] = 'GRATUITY FUND PROVISION EXP' THEN '645310'
	WHEN upvt.[Transaction Reference] = 'MEDICAL EXP PROVISION EXP' THEN '645110'
	WHEN upvt.[Transaction Reference] = 'TDS FROM SALARY' THEN '442820'
	WHEN upvt.[Transaction Reference] = 'SALARY PAYABLE' THEN 'WD00007'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR PROVIDENT FUND' THEN '437000'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR FESTIVAL BONUS' THEN '428600'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR ANNUAL LEAVE' THEN '428200'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR GRATUITY' THEN '437010'
	WHEN upvt.[Transaction Reference] = 'PROVISION FOR MEDICAL RMBRSMT' THEN '428620'
	
	
	END as  [Account code]

    ,'SALARY '+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName AS Description
    ,upvt.[Transaction Reference]
   ,e.Code [Extra Reference]
   ,''[Transaction Date]
    ,'BDT'[Currency Code]
	 , upvt.[Transaction Amount]
   ,upvt.[Transaction Amount][Base Amount]
  ,s.Branch [BRANCHE T1]
  ,s.Department [ACTIVITY T2]
  ,e.TPNCode[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,e.DCode [EMPLOYEE CODE T8]
  ,''[T9]
  
FROM View_TIBSalary s
LEFT OUTER JOIN ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
--LEFT OUTER JOIN GLAccount GLA ON upvt.[Transaction Reference] = GLA.[GLAccountType]
CROSS APPLY (
    VALUES
        ('BASIC SALARY', s.Basic*-1),
        ('HOUSE RENT ALLOWANCE', s.HouseRent*-1),
        ('SPECIAL ALLOWANCE', s.TransportAllowance*-1),
        ('PROVIDENT FUND-EMPLYR CONT', s.PFEmployer*-1),
        ('FESTIVAL BONUS PROVISION EXP', (s.Basic*25/100)*-1),
        ('ANNUAL LEAVE PROVISION EXP', ((s.Gross/30 * 15) / 12)*-1),
        ('GRATUITY FUND PROVISION EXP', (s.Basic/ 12*-1)),
        ('MEDICAL EXP PROVISION EXP', CASE
                                          WHEN s.Grade ='C3' OR s.Grade ='C2' OR s.Grade ='C1' OR s.Grade ='A1' OR s.Grade ='A2' OR s.Grade ='A3' OR s.Grade ='B1-M3' OR s.Grade ='B1-M1' THEN
                                              CASE WHEN s.Basic<50000 THEN (s.Basic/12)*-1 ELSE (50000/12)* -1 END 
                                          ELSE ((10000) / 12) * -1
                                      END),
        ('TDS FROM SALARY', s.TAX),
        ('SALARY PAYABLE', s.Gross - s.PFEmployer),
        ('PROVISION FOR PROVIDENT FUND', s.PFEmployer * 2),
        ('PROVISION FOR FESTIVAL BONUS', (s.Basic*25/100)),
        ('PROVISION FOR ANNUAL LEAVE', (s.Gross/30 * 15) / 12),
        ('PROVISION FOR GRATUITY', s.Basic/ 12),
        ('PROVISION FOR MEDICAL RMBRSMT', CASE
                                              WHEN s.Grade ='C3' OR s.Grade ='C2' OR s.Grade ='C1' OR s.Grade ='A1' OR s.Grade ='A2' OR s.Grade ='A3' OR s.Grade ='B1-M3' OR s.Grade ='B1-M1' THEN
                                                  CASE WHEN s.Basic<50000 THEN (s.Basic/12) ELSE (50000/12) END
                                              ELSE ((10000) / 12) 
                                          END)
) AS upvt([Transaction Reference], [Transaction Amount])
WHERE s.FiscalYearDetailId =@FiscalYearDetailId and upvt.[Transaction Reference] = 'SALARY PAYABLE'
    AND s.IsHold = 0
	) Select * from cat 

	UNION ALL

	Select  '512153C' [Account code]
    ,'LOCAL BUS TRAVEL & TRNSPT ALLOWANCE PAY - ' Description
    ,'xx'[Transaction Reference]
   ,''[Extra Reference]
   ,''[Transaction Date]
    ,''[Currency Code]
	 ,SUM([Transaction Amount])
   ,SUM([Transaction Amount])[Base Amount]
  ,''[BRANCHE T1]
  ,''[ACTIVITY T2]
  ,'D00999'[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,'D999'[EMPLOYEE CODE T8]
  ,''[T9] from cat
";

                #region More Conditions
                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                {
                    sqlText += "  and s.Code>= @CodeF";
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                {
                    sqlText += " and s.Code<= @CodeT";
                }


                #endregion

                sqlText += "  ORDER BY [Transaction Reference]";



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeTo);
                }
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                da.Fill(dt);




                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable DataExportForSunCarTransport(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;


                #region sql statement

                sqlText = @"

DECLARE @PeriodName AS varchar(100)
DECLARE @PeriodYear AS varchar(100)

SELECT @PeriodName=UPPER(LEFT(PeriodName, 3)), @PeriodYear=[Year]
FROM FiscalYearDetail
WHERE Id =@FiscalYearDetailId

SELECT
	CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN '624700'
	WHEN upvt.[Transaction Reference] = 'Car Allowance' THEN '625000'	
	
	END as  [Account code]

    , 
	CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LBT-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName ELSE 'TA-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName END AS Description
    , CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LOCAL BUS TRVL-'+@PeriodName+'-'+ @PeriodYear ELSE 'TRSPT ALWNC-'+@PeriodName+'-'+ @PeriodYear END AS [Transaction Reference]
   ,e.Code [Extra Reference]
   ,''[Transaction Date]
    ,'BDT'[Currency Code]
	 , upvt.[Transaction Amount]
   ,upvt.[Transaction Amount][Base Amount]
  ,s.Branch [BRANCHE T1]
  ,s.Department [ACTIVITY T2]
  ,e.TPNCode[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,e.DCode [EMPLOYEE CODE T8]
  ,''[T9]
  
FROM View_TIBSalary s
LEFT OUTER JOIN ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
--LEFT OUTER JOIN GLAccount GLA ON upvt.[Transaction Reference] = GLA.[GLAccountType]
CROSS APPLY (
    VALUES
        ('Transport Allowance', s.TransportAllowance*-1),
        ('Car Allowance', s.CarAllowance*-1)
       
       
) AS upvt([Transaction Reference], [Transaction Amount])
WHERE s.FiscalYearDetailId =@FiscalYearDetailId
    AND s.IsHold = 0
   

	Union All

	SELECT
	'W'+e.DCode  as  [Account code]

    , 
	CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LBT-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName ELSE 'TA-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName END AS Description
    , CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LOCAL BUS TRVL-'+@PeriodName+'-'+ @PeriodYear ELSE 'TRSPT ALWNC-'+@PeriodName+'-'+ @PeriodYear END AS [Transaction Reference]
   ,e.Code [Extra Reference]
   ,''[Transaction Date]
    ,'BDT'[Currency Code]
	 , upvt.[Transaction Amount]
   ,upvt.[Transaction Amount][Base Amount]
  ,s.Branch [BRANCHE T1]
  ,s.Department [ACTIVITY T2]
  ,e.TPNCode[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,e.DCode [EMPLOYEE CODE T8]
  ,''[T9]
  
FROM View_TIBSalary s
LEFT OUTER JOIN ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
--LEFT OUTER JOIN GLAccount GLA ON upvt.[Transaction Reference] = GLA.[GLAccountType]
CROSS APPLY (
    VALUES
        ('Transport Allowance', s.TransportAllowance*1),
        ('Car Allowance', s.CarAllowance*1)
       
       
) AS upvt([Transaction Reference], [Transaction Amount])
WHERE s.FiscalYearDetailId ='1102'
    AND s.IsHold = 0 Order by [Transaction Reference] 
";


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                da.Fill(dt);
                                
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable DataExportForSunCarTransportPayment(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;


                #region sql statement

                sqlText = @"
	DECLARE @PeriodName AS varchar(100)
DECLARE @PeriodYear AS varchar(100)

SELECT @PeriodName=UPPER(LEFT(PeriodName, 3)), @PeriodYear=[Year]
FROM FiscalYearDetail
WHERE Id ='1102';
with cat as(
	SELECT
	'W'+e.DCode  as  [Account code]

    , 
	CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LBT-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName ELSE 'TA-'+@PeriodName+'-'+ @PeriodYear+' -'+s.EmpName END AS Description
    , CASE 
	WHEN upvt.[Transaction Reference] = 'Transport Allowance' THEN 'LOCAL BUS TRVL-'+@PeriodName+'-'+ @PeriodYear ELSE 'TRSPT ALWNC-'+@PeriodName+'-'+ @PeriodYear END AS [Transaction Reference]
   ,e.Code [Extra Reference]
   ,''[Transaction Date]
    ,'BDT'[Currency Code]
	 , upvt.[Transaction Amount]
   ,upvt.[Transaction Amount][Base Amount]
  ,s.Branch [BRANCHE T1]
  ,s.Department [ACTIVITY T2]
  ,e.TPNCode[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,e.DCode [EMPLOYEE CODE T8]
  ,''[T9]
  
FROM View_TIBSalary s
LEFT OUTER JOIN ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
--LEFT OUTER JOIN GLAccount GLA ON upvt.[Transaction Reference] = GLA.[GLAccountType]
CROSS APPLY (
    VALUES
        ('Transport Allowance', s.TransportAllowance*1),
        ('Car Allowance', s.CarAllowance*1)
       
       
) AS upvt([Transaction Reference], [Transaction Amount])
WHERE s.FiscalYearDetailId ='1102'
    AND s.IsHold = 0

	)
	Select * from cat
	
	UNION ALL

	Select  '512153C' [Account code]
    ,'LOCAL BUS TRAVEL & TRNSPT ALLOWANCE PAY - ' +@PeriodName+'-'+ @PeriodYear Description
    ,'LOC BUS TRVL & TA PAY-'+@PeriodName+'-'+ @PeriodYear [Transaction Reference]
   ,''[Extra Reference]
   ,''[Transaction Date]
    ,''[Currency Code]
	 ,SUM([Transaction Amount])
   ,SUM([Transaction Amount])[Base Amount]
  ,''[BRANCHE T1]
  ,''[ACTIVITY T2]
  ,'D00999'[TPN T3]
  ,''[VAT TAX]
  ,''[T5]
  ,''[JOB FILE T6]
  ,''[CASH FLOW T7]
  ,'D999'[EMPLOYEE CODE T8]
  ,''[T9] from cat
";


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);

                da.Fill(dt);

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable TAX_108(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;


                #region sql statement
                if (vm.CompanyName.ToLower() == "tib" && vm.FiscalYear == 2022)
                {
                    sqlText = @"

---DECLARE @year AS VARCHAR(100)='2022'


select 
tempTable.EmpName	
,tempTable.Section	
,tempTable.Section1	
,tempTable.Designation
,tempTable.Project	
,tempTable.JoinDate	
,tempTable.LeftDate	
,tempTable.Code	
,tempTable.TIN	
,tempTable.Email	
,tempTable.SectionOrder
,tempTable.OrderNo	
,tempTable.orderNo1
,tempTable.employeeid
,tempTable.Gender	
,tempTable.Basic + 	isnull(efs.Basic,0) [Basic]
,tempTable.HouseRent  + isnull(efs.HouseRent,0) HouseRent
,tempTable.Medical  + isnull(efs.Medical,0) Medical
,tempTable.TransportAllowance + isnull(efs.Transport,0) TransportAllowance
,tempTable.Gross	+ 	isnull(efs.Basic,0)  + isnull(efs.HouseRent,0)+ isnull(efs.Medical,0)+ isnull(efs.Transport,0) [Gross]
,tempTable.PFEmployer + 	isnull(efs.PFEmployer,0)PFEmployer
,tempTable.ChildAllowance
,tempTable.HARDSHIP	
,tempTable.Othere_OT + isnull(efs.Overtime,0) Othere_OT
,tempTable.TransportBill
,tempTable.Bonus + isnull(efs.BonusAdjustment,0) Bonus
,tempTable.LeaveEncashment + isnull(efs.leave,0) LeaveEncashment
,tempTable.RecognizedPF	
,tempTable.RecognizedGF	
,tempTable.Principal	
,tempTable.Profit	
,tempTable.TotalPaid	
,tempTable.TAXDeduction
,tempTable.TotalExemptedAmount
,tempTable.RebateAmount from (
select 
CASE
    WHEN e.EmpName is null THEN em.EmpName
    WHEN e.EmpName = 'NA' THEN em.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Section is null THEN em.Section
    WHEN e.Section = 'NA' THEN em.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Gender is null THEN em.Gender
    WHEN e.Gender = 'NA' THEN em.Gender
    ELSE e.Gender
END Gender
,CASE
    WHEN e.Department is null THEN em.Department
    WHEN e.Department = 'NA' THEN em.Department
    ELSE e.Department
END Section1
,CASE
    WHEN e.Designation is null THEN em.Designation
    WHEN e.Designation = 'NA' THEN em.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Project is null THEN em.Project
    WHEN e.Project = 'NA' THEN em.Project
    ELSE e.Project
END Project
,CASE
    WHEN em.JoinDate is null THEN e.JoinDate
    WHEN em.JoinDate = 'NA' THEN e.JoinDate
    ELSE em.JoinDate
END JoinDate
,CASE
    WHEN e.LeftDate is null THEN em.LeftDate
    WHEN e.LeftDate = 'NA' THEN em.LeftDate
    ELSE e.LeftDate
END LeftDate
,CASE
    WHEN e.Code is null THEN em.Code
    WHEN e.Code = 'NA' THEN em.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.TIN is null THEN em.TIN
    WHEN e.TIN = 'NA' THEN em.TIN
    ELSE e.TIN
END TIN
,CASE
    WHEN e.Email is null THEN em.Email
    WHEN e.Email = 'NA' THEN em.Email
    ELSE e.Email
END Email
,CASE
    WHEN e.SectionOrder is null THEN em.SectionOrder
    ELSE e.SectionOrder
END SectionOrder --
, Sec.OrderNo --
,d.OrderNo orderNo1
,a.*
,isnull(Bonus.Bonus,0) Bonus
,isnull(EL.Amount,0) LeaveEncashment
,isnull(pf.PF,0) RecognizedPF
,isnull(GF.GF,0) RecognizedGF
,isnull(pf.Principal,0) Principal
,isnull(pf.Profit,0) Profit
,0 TotalPaid
,round(isnull(tax.TAXAmount,0),0) TAXDeduction
,isnull(T.TotalExemptedAmount,0)TotalExemptedAmount
,isnull(T.RebateAmount,0)RebateAmount
from (Select 
distinct employeeid
,sum(e.Basic -[Other(Deduction)])Basic
,sum(e.HouseRent)HouseRent
,sum(e.Medical )Medical
,sum(e.TransportAllowance)TransportAllowance
,sum(e.Gross -[Other(Deduction)])Gross
,sum(e.PFEmployer )PFEmployer
,sum(e.ChildAllowance )ChildAllowance
,sum(e.HARDSHIP )HARDSHIP
,sum(e.Othere_OT)Othere_OT
,sum(e.TransportBill)TransportBill

From (select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid

union all 

select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]

from HRMDB.dbo.ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from HRMDB.dbo.FiscalYearDetail
where year=@year)
group by employeeid
) as e

group by e.employeeid
) as a 
left outer join  ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join HRMDB.dbo. ViewEmployeeInformation em on a.EmployeeId=em.EmployeeId
left outer join (select distinct EmployeeId, sum(isnull(TotalExemptedAmount,0) )TotalExemptedAmount, sum(isnull(RebateAmount,0) )RebateAmount
from TAX_Pectra.dbo.Schedule1SalaryMonthlies
where 1=1
And TransactionType='YearlyTaX'
and Year=@year
group by EmployeeId)
T on a.EmployeeId=T.EmployeeId
left outer join Section Sec on e.SectionId=Sec.Id
left outer join Designation D on e.DesignationId=D.Id
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id

left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from   " + _dbsqlConnection.PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)) Principal
,sum(isnull(EmployeeProfit,0)+ isnull(EmployerProfit,0))Profit
 ,sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from   " + _dbsqlConnection.PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from TAX_Pectra.dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId

) as tempTable

left outer join EmployeeFinalSettlement efs

on tempTable.EmployeeId = efs.EmployeeId";

                    #region More Conditions
                    if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    {
                        sqlText += "  Where tempTable.Code>= @CodeF";
                    }
                    if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    {
                        sqlText += " and tempTable.Code<= @CodeT";
                    }


                    #endregion

                    sqlText += "    ORDER BY CASE WHEN OrderNo IS NULL THEN 99999 ELSE OrderNo END ,CASE WHEN OrderNo1 IS NULL THEN 99999 ELSE orderNo1 END ";

                }
                else if (vm.CompanyName.ToLower() == "tib")
                {
                    sqlText = @"

---DECLARE @year AS VARCHAR(100)='2022'


select 
tempTable.EmpName	
,tempTable.Section	
,tempTable.Section1	
,tempTable.Designation
,tempTable.Project	
,tempTable.JoinDate	
,tempTable.LeftDate	
,tempTable.Code	
,tempTable.TIN	
,tempTable.Email	
,tempTable.SectionOrder
,tempTable.OrderNo	
,tempTable.orderNo1
,tempTable.employeeid
,tempTable.Gender	
,tempTable.Basic + 	isnull(efs.Basic,0) [Basic]
,tempTable.HouseRent  + isnull(efs.HouseRent,0) HouseRent
,tempTable.Medical  + isnull(efs.Medical,0) Medical
,tempTable.TransportAllowance + isnull(efs.Transport,0) TransportAllowance
,tempTable.Gross	+ 	isnull(efs.Basic,0)  + isnull(efs.HouseRent,0)+ isnull(efs.Medical,0)+ isnull(efs.Transport,0) [Gross]
,tempTable.PFEmployer + 	isnull(efs.PFEmployer,0)PFEmployer
,tempTable.ChildAllowance
,tempTable.HARDSHIP	
,tempTable.Othere_OT + isnull(efs.Overtime,0) Othere_OT
,tempTable.TransportBill
,tempTable.Bonus +isnull(tempTable.[Other(Bonus)],0)+ isnull(efs.BonusAdjustment,0) Bonus
,tempTable.LeaveEncashment +isnull(tempTable.EL,0)+ isnull(efs.leave,0) LeaveEncashment
,tempTable.RecognizedPF	
,tempTable.RecognizedGF	
,tempTable.Principal	
,tempTable.Profit	
,tempTable.TotalPaid	
,tempTable.TAXDeduction
,tempTable.TotalExemptedAmount
,tempTable.RebateAmount from (
select 
CASE
    WHEN e.EmpName is null THEN em.EmpName
    WHEN e.EmpName = 'NA' THEN em.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Section is null THEN em.Section
    WHEN e.Section = 'NA' THEN em.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Gender is null THEN em.Gender
    WHEN e.Gender = 'NA' THEN em.Gender
    ELSE e.Gender
END Gender
,CASE
    WHEN e.Department is null THEN em.Department
    WHEN e.Department = 'NA' THEN em.Department
    ELSE e.Department
END Section1
,CASE
    WHEN e.Designation is null THEN em.Designation
    WHEN e.Designation = 'NA' THEN em.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Project is null THEN em.Project
    WHEN e.Project = 'NA' THEN em.Project
    ELSE e.Project
END Project
,CASE
    WHEN em.JoinDate is null THEN e.JoinDate
    WHEN em.JoinDate = 'NA' THEN e.JoinDate
    ELSE em.JoinDate
END JoinDate
,CASE
    WHEN e.LeftDate is null THEN em.LeftDate
    WHEN e.LeftDate = 'NA' THEN em.LeftDate
    ELSE e.LeftDate
END LeftDate
,CASE
    WHEN e.Code is null THEN em.Code
    WHEN e.Code = 'NA' THEN em.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.TIN is null THEN em.TIN
    WHEN e.TIN = 'NA' THEN em.TIN
    ELSE e.TIN
END TIN
,CASE
    WHEN e.Email is null THEN em.Email
    WHEN e.Email = 'NA' THEN em.Email
    ELSE e.Email
END Email
,CASE
    WHEN e.SectionOrder is null THEN em.SectionOrder
    ELSE e.SectionOrder
END SectionOrder --
, Sec.OrderNo --
,d.OrderNo orderNo1
,a.*
,isnull(Bonus.Bonus,0) Bonus
,isnull(EL.Amount,0) LeaveEncashment
,isnull(pf.PF,0) RecognizedPF
,isnull(GF.GF,0) RecognizedGF
,isnull(pf.Principal,0) Principal
,isnull(pf.Profit,0) Profit
,0 TotalPaid
,round(isnull(tax.TAXAmount,0),0) TAXDeduction
,isnull(T.TotalExemptedAmount,0)TotalExemptedAmount
,isnull(T.RebateAmount,0)RebateAmount
from (Select 
distinct employeeid
,sum(e.Basic -[Other(Deduction)])Basic
,sum(e.HouseRent)HouseRent
,sum(e.Medical )Medical
,sum(e.TransportAllowance)TransportAllowance
,sum(e.Gross -[Other(Deduction)])Gross
,sum(e.PFEmployer )PFEmployer
,sum(e.ChildAllowance )ChildAllowance
,sum(e.HARDSHIP )HARDSHIP
,sum(e.Othere_OT)Othere_OT
,sum(e.TransportBill)TransportBill
,sum(e.LeaveEncash)EL
,sum(e.[Other(Bonus)])[Other(Bonus)]

From (select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end +case when SalaryHead in('BasicAdj') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end+case when SalaryHead in('HouserentAdj') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end+case when SalaryHead in('MedialAllow.Adj') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end+case when SalaryHead in('TransportAdj') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance','BasicAdj','HouserentAdj','MedialAllow.Adj','TransportAdj') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end+case when SalaryHead in('CPFAdj') then (-1)*Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
,sum(case when SalaryHead in('LeaveEncash') then Amount else 0 end) LeaveEncash
,sum(case when SalaryHead in('Other(Bonus)') then Amount else 0 end) [Other(Bonus)]
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid

union all 

select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end +case when SalaryHead in('BasicAdj') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end+case when SalaryHead in('HouserentAdj') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end+case when SalaryHead in('MedialAllow.Adj') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end+case when SalaryHead in('TransportAdj') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance','BasicAdj','HouserentAdj','MedialAllow.Adj','TransportAdj') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
,sum(case when SalaryHead in('LeaveEncash') then Amount else 0 end) LeaveEncash
,sum(case when SalaryHead in('Other(Bonus)') then Amount else 0 end) [Other(Bonus)]

from HRMDB.dbo.ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from HRMDB.dbo.FiscalYearDetail
where year=@year)
group by employeeid
) as e

group by e.employeeid
) as a 
left outer join  ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join HRMDB.dbo. ViewEmployeeInformation em on a.EmployeeId=em.EmployeeId
left outer join (select distinct EmployeeId, sum(isnull(TotalExemptedAmount,0) )TotalExemptedAmount, sum(isnull(RebateAmount,0) )RebateAmount
from TAX_Pectra.dbo.Schedule1SalaryMonthlies
where 1=1
And TransactionType='YearlyTaX'
and Year=@year
group by EmployeeId)
T on a.EmployeeId=T.EmployeeId
left outer join Section Sec on e.SectionId=Sec.Id
left outer join Designation D on e.DesignationId=D.Id
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id

left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from   " + _dbsqlConnection.PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)) Principal
,sum(isnull(EmployeeProfit,0)+ isnull(EmployerProfit,0))Profit
 ,sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from   " + _dbsqlConnection.PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from TAX_Pectra.dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId

) as tempTable

left outer join EmployeeFinalSettlement efs

on tempTable.EmployeeId = efs.EmployeeId";

                    #region More Conditions
                    if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                    {
                        sqlText += "  Where tempTable.Code>= @CodeF";
                    }
                    if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                    {
                        sqlText += " and tempTable.Code<= @CodeT";
                    }


                    #endregion

                    sqlText += "    ORDER BY CASE WHEN OrderNo IS NULL THEN 99999 ELSE OrderNo END ,CASE WHEN OrderNo1 IS NULL THEN 99999 ELSE orderNo1 END ";

                }
                else
                {
                    sqlText = @"
--DECLARE @year AS VARCHAR(100)='2022'

select e.EmpName,e.Designation,e.Section,e.Department,e.Project,e.JoinDate,e.LeftDate,e.Code,e.TIN, e.SectionOrder,a.*
,isnull(Bonus.Bonus,0) Bonus
,isnull(EL.Amount,0) LeaveEncashment
,isnull(pf.PF,0) RecognizedPF
,isnull(GF.GF,0) RecognizedGF
,isnull(pf.Principal,0) Principal
,isnull(pf.Profit,0) Profit
,0 TotalPaid
,round(isnull(tax.TAXAmount,0),0) TAXDeduction
,T.TotalExemptedAmount
,T.RebateAmount
from (
select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end+case when SalaryHead in('Vehicle(Adj)') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid
) as a 
left outer join ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join (select distinct EmployeeId, sum(isnull(TotalExemptedAmount,0) )TotalExemptedAmount, sum(isnull(RebateAmount,0) )RebateAmount
from " + TAXDB + @".dbo.Schedule1SalaryMonthlies
where 1=1
And TransactionType='YearlyTaX'
and Year=@year
group by EmployeeId)
T on a.EmployeeId=T.EmployeeId
left outer join Section Sec on e.SectionId=Sec.Id
left outer join Designation D on e.DesignationId=D.Id
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from " + PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)) Principal
,sum(isnull(EmployeeProfit,0)+ isnull(EmployerProfit,0))Profit
 ,sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from " + PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from " + TAXDB + @".dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId
ORDER BY CASE WHEN Sec.OrderNo IS NULL THEN 99999 ELSE Sec.OrderNo END ,CASE WHEN d.OrderNo IS NULL THEN 99999 ELSE d.OrderNo END 



 

";
                }

                sqlText = sqlText.Replace("HRMDB", _dbsqlConnection.HRMDB);

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;
                da.SelectCommand.Parameters.AddWithValue("@year", vm.FiscalYear);

                if (!string.IsNullOrWhiteSpace(vm.CodeFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeFrom);
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeTo);
                }
                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");
                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable YearlyTAX(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string HRMDB = _dbsqlConnection.HRMDB;

                #region sql statement
                sqlText = @"

---DECLARE @year AS VARCHAR(100)='2022'

Select EmployeeId,ChallanNo,DepositDate,DepositAmount,f.PeriodName from " + TAXDB + @".dbo.TaxDeposits t
left outer join " + HRMDB + @".dbo.FiscalYearDetail  f on t.FiscalYearDetailId=f.Id
where t.Year=@Year

group by EmployeeId,ChallanNo,DepositDate,DepositAmount,f.PeriodName";


                sqlText += "    order by t.EmployeeId,t.DepositDate ";




                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@year", vm.FiscalYear);

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DepositDate");


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }


        public DataTable TAX_108A(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                sqlText = @"

select  e.EmpName,e.Designation,e.Section,e.Department,e.Project,e.JoinDate,e.LeftDate,e.Code,e.TIN
, a.SubmitDate
, a.SubmitYear
, a.SubmitSerialNo
from TAX108A a
left outer join ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
where 1=1
and a.SubmitYear=@SubmitYear
order by Dg.Serial
";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@SubmitYear", vm.FiscalYear);

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");
                dt = Ordinary.DtColumnStringToDate(dt, "LeftDate");
                dt = Ordinary.DtColumnStringToDate(dt, "SubmitDate");


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable TAX_108_WithOutTIN(SalarySheetVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                string TAXDB = _dbsqlConnection.TAXDB;
                string PFDB = _dbsqlConnection.PFDB;
                string HRMDB = _dbsqlConnection.HRMDB;


                #region sql statement
                if (vm.CompanyName.ToLower() == "tib" && vm.FiscalYear == 2022)
                {
                    sqlText = @"
create table #Table
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    EmpName Varchar(200), 
    employeeid Varchar(200), 
    TIN Varchar(200), 
    Total decimal(25, 9), 
   
)

create table #Temp
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    TIN Varchar(50), 
    Total decimal(25, 9), 
   
)

create table #Temp1
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    NumberWithOutTIN Varchar(50), 
    WithOutTINTotal decimal(25, 9), 
    Total decimal(25, 9), 
    Percentage decimal(25, 9), 
   
)

INSERT INTO #Table (EmpName,TIN,employeeid,Total)
select 

CASE
    WHEN e.EmpName is null THEN em.EmpName
    WHEN e.EmpName = 'NA' THEN em.EmpName
    ELSE e.EmpName
END EmpName
,
CASE
    WHEN e.TIN is null THEN em.TIN
    WHEN e.TIN = 'NA' THEN em.TIN
    ELSE e.TIN
END TIN
,a.employeeid
,(a.Gross+
+a.Othere_OT
+a.ChildAllowance
+a.PFEmployer
+a.HARDSHIP
+isnull(Bonus.Bonus,0) 
+isnull(EL.Amount,0)
--+isnull(pf.PF,0) 
--+isnull(GF.GF,0)
)Total

from (Select 
distinct employeeid
,sum(e.Basic-[Other(Deduction)] )Basic
,sum(e.HouseRent)HouseRent
,sum(e.Medical )Medical
,sum(e.TransportAllowance)TransportAllowance
,sum(e.Gross-[Other(Deduction)] )Gross
,sum(e.PFEmployer )PFEmployer
,sum(e.ChildAllowance )ChildAllowance
,sum(e.HARDSHIP )HARDSHIP
,sum(e.Othere_OT)Othere_OT
,sum(e.TransportBill)TransportBill

From (select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end+case when SalaryHead in('CPFAdj') then (-1)*Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid

union all 

select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
from  " + HRMDB + @".dbo.ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from   " + _dbsqlConnection.HRMDB + @" .dbo.FiscalYearDetail
where year=@year)
group by employeeid
) as e

group by e.employeeid
) as a 
left outer join  ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join  " + HRMDB + @".dbo. ViewEmployeeInformation em on a.EmployeeId=em.EmployeeId
left outer join (select distinct EmployeeId, sum(isnull(TotalExemptedAmount,0) )TotalExemptedAmount, sum(isnull(RebateAmount,0) )RebateAmount
from " + TAXDB + @".dbo.Schedule1SalaryMonthlies
where 1=1
And TransactionType='YearlyTaX'
and Year=@year
group by EmployeeId)
T on a.EmployeeId=T.EmployeeId
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from " + PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)) Principal
,sum(isnull(EmployeeProfit,0)+ isnull(EmployerProfit,0))Profit
 ,sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from " + PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from " + TAXDB + @".dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId
order by Dg.Serial

update #Table set Total=Total+ isnull(efs.basic,0)+ isnull(efs.HouseRent,0)+ isnull(efs.Medical,0)+ isnull(efs.Transport,0)+ isnull(efs.Leave,0)
+ isnull(efs.BonusAdjustment,0)+isnull(efs.Overtime,0)
from #Table
left outer join EmployeeFinalSettlement efs
on #Table.employeeid = efs.EmployeeId


INSERT INTO #Temp (TIN, Total)
select 

'NO' TIN 
,Total
from #Table
where TIN is  null or TIN=''or TIN='-'
 
union all 
select 

'Yes' TIN 
,Total
from #Table
where TIN is not null or TIN!=''or TIN!='-'


INSERT INTO #Temp1 (NumberWithOutTIN, WithOutTINTotal)
Select  Count(TIN)NumberWithOutTIN,sum (Total)Total from #Temp
where TIN ='NO'


Update #Temp1 set Total=  (Select  sum (Total)Total from #Table
)

Select NumberWithOutTIN,WithOutTINTotal,Total,(WithOutTINTotal/Total)*100 as [percent] from #Temp1

Drop Table #Table
Drop Table #Temp
Drop Table #Temp1

";
                }

                if (vm.CompanyName.ToLower() == "tib")
                {
                    sqlText = @"
create table #Table
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    EmpName Varchar(200), 
    employeeid Varchar(200), 
    TIN Varchar(200), 
    Total decimal(25, 9), 
   
)

create table #Temp
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    TIN Varchar(50), 
    Total decimal(25, 9), 
   
)

create table #Temp1
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    NumberWithOutTIN Varchar(50), 
    WithOutTINTotal decimal(25, 9), 
    Total decimal(25, 9), 
    Percentage decimal(25, 9), 
   
)

INSERT INTO #Table (EmpName,TIN,employeeid,Total)
select 

CASE
    WHEN e.EmpName is null THEN em.EmpName
    WHEN e.EmpName = 'NA' THEN em.EmpName
    ELSE e.EmpName
END EmpName
,
CASE
    WHEN e.TIN is null THEN em.TIN
    WHEN e.TIN = 'NA' THEN em.TIN
    ELSE e.TIN
END TIN
,a.employeeid
,(a.Gross+
+a.Othere_OT
+a.ChildAllowance
+a.PFEmployer
+a.HARDSHIP
+isnull(Bonus.Bonus,0)+ isnull(a.[Other(Bonus)],0)
+isnull(EL.Amount,0)+isnull(a.EL,0)
--+isnull(pf.PF,0) 
--+isnull(GF.GF,0)
)Total

from (Select 
distinct employeeid
,sum(e.Basic-[Other(Deduction)] )Basic
,sum(e.HouseRent)HouseRent
,sum(e.Medical )Medical
,sum(e.TransportAllowance)TransportAllowance
,sum(e.Gross-[Other(Deduction)] )Gross
,sum(e.PFEmployer )PFEmployer
,sum(e.ChildAllowance )ChildAllowance
,sum(e.HARDSHIP )HARDSHIP
,sum(e.Othere_OT)Othere_OT
,sum(e.TransportBill)TransportBill
,sum(e.LeaveEncash)EL
,sum(e.[Other(Bonus)])[Other(Bonus)]
From (select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end +case when SalaryHead in('BasicAdj') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end+case when SalaryHead in('HouserentAdj') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end+case when SalaryHead in('MedialAllow.Adj') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end+case when SalaryHead in('TransportAdj') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance','BasicAdj','HouserentAdj','MedialAllow.Adj','TransportAdj') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
,sum(case when SalaryHead in('LeaveEncash') then Amount else 0 end) LeaveEncash
,sum(case when SalaryHead in('Other(Bonus)') then Amount else 0 end) [Other(Bonus)]
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid

union all 

select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end +case when SalaryHead in('BasicAdj') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end+case when SalaryHead in('HouserentAdj') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end+case when SalaryHead in('MedialAllow.Adj') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end+case when SalaryHead in('TransportAdj') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance','BasicAdj','HouserentAdj','MedialAllow.Adj','TransportAdj') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
,sum(case when SalaryHead in('LeaveEncash') then Amount else 0 end) LeaveEncash
,sum(case when SalaryHead in('Other(Bonus)') then Amount else 0 end) [Other(Bonus)]
from  " + HRMDB + @".dbo.ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from  " + HRMDB + @".dbo.FiscalYearDetail
where year=@year)
group by employeeid
) as e

group by e.employeeid
) as a 
left outer join  ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join  " + HRMDB + @".dbo. ViewEmployeeInformation em on a.EmployeeId=em.EmployeeId
left outer join (select distinct EmployeeId, sum(isnull(TotalExemptedAmount,0) )TotalExemptedAmount, sum(isnull(RebateAmount,0) )RebateAmount
from " + TAXDB + @".dbo.Schedule1SalaryMonthlies
where 1=1
And TransactionType='YearlyTaX'
and Year=@year
group by EmployeeId)
T on a.EmployeeId=T.EmployeeId
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from " + PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)) Principal
,sum(isnull(EmployeeProfit,0)+ isnull(EmployerProfit,0))Profit
 ,sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from " + PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from " + TAXDB + @".dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId
order by Dg.Serial

update #Table set Total=Total+ isnull(efs.basic,0)+ isnull(efs.HouseRent,0)+ isnull(efs.Medical,0)+ isnull(efs.Transport,0)+ isnull(efs.Leave,0)
+ isnull(efs.BonusAdjustment,0)+isnull(efs.Overtime,0)
from #Table
left outer join EmployeeFinalSettlement efs
on #Table.employeeid = efs.EmployeeId


INSERT INTO #Temp (TIN, Total)
select 

'NO' TIN 
,Total
from #Table
where TIN is  null or TIN=''or TIN='-'
 
union all 
select 

'Yes' TIN 
,Total
from #Table
where TIN is not null or TIN!=''or TIN!='-'


INSERT INTO #Temp1 (NumberWithOutTIN, WithOutTINTotal)
Select  Count(TIN)NumberWithOutTIN,sum (Total)Total from #Temp
where TIN ='NO'


Update #Temp1 set Total=  (Select  sum (Total)Total from #Table
)

Select NumberWithOutTIN,WithOutTINTotal,Total,(WithOutTINTotal/Total)*100 as [percent] from #Temp1

Drop Table #Table
Drop Table #Temp
Drop Table #Temp1

";
                }
                else
                {
                    sqlText = @"

create table #Temp
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    TIN Varchar(50), 
    Total decimal(25, 9), 
   
)

create table #Temp1
(
   [ID] [int] IDENTITY(1,1) NOT NULL,
    NumberWithOutTIN Varchar(50), 
    WithOutTINTotal decimal(25, 9), 
    Total decimal(25, 9), 
    Percentage decimal(25, 9), 
   
)

INSERT INTO #Temp (TIN, Total)
select 

'NO' TIN 
,(a.Gross+
+a.Othere_OT
+a.ChildAllowance
+a.PFEmployer
+a.HARDSHIP
+isnull(Bonus.Bonus,0) 
+isnull(EL.Amount,0)
+isnull(pf.PF,0) 
+isnull(GF.GF,0))Total


from (
select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
,sum(case when SalaryHead in('Other(Deduction)') then Amount else 0 end) [Other(Deduction)]
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid
) as a 
left outer join ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from   " + _dbsqlConnection.PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from   " + _dbsqlConnection.PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from TAX_Pectra.dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId
where e.TIN is null or e.TIN=''or e.TIN='-' 

union all 
select 

'Yes' TIN 

,(a.Gross+
+a.Othere_OT
+a.ChildAllowance
+a.PFEmployer
+a.HARDSHIP
+isnull(Bonus.Bonus,0) 
+isnull(EL.Amount,0)
+isnull(pf.PF,0) 
+isnull(GF.GF,0))Total


from (
select distinct employeeid
,sum(case when SalaryHead in('Basic') then Amount else 0 end) Basic
,sum(case when SalaryHead in('HouseRent') then Amount else 0 end) HouseRent
,sum(case when SalaryHead in('Medical') then Amount else 0 end) Medical
,sum(case when SalaryHead in('TransportAllowance') then Amount else 0 end) TransportAllowance
,sum(case when SalaryHead in('Basic','HouseRent','Medical','TransportAllowance') then Amount else 0 end) Gross
,sum(case when SalaryHead in('PFEmployer') then Amount else 0 end) PFEmployer
,sum(case when SalaryHead in('ChildAllowance') then Amount else 0 end) ChildAllowance
,sum(case when SalaryHead in('HARDSHIP') then Amount else 0 end) HARDSHIP
,sum(case when SalaryHead in('Othere(OT)') then Amount else 0 end) Othere_OT
,sum(case when SalaryHead in('TransportBill') then Amount else 0 end) TransportBill
from ViewSalaryPreCalculation
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid
) as a 
left outer join ViewEmployeeInformation e on a.EmployeeId=e.EmployeeId
left outer join DesignationGroup Dg on e.DesignationGroupId=Dg.Id
left outer join(
select distinct employeeid, sum(isnull(EmployerContribution,0)+ isnull(EmployerProfit,0))GF
from   " + _dbsqlConnection.PFDB + @".dbo.GFEmployeePayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid
) GF on a.EmployeeId=gf.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(EmployeeContribution,0)
+ isnull(EmployerContribution,0)
+ isnull(EmployeeProfit,0)
+ isnull(EmployerProfit,0)
)PF
from   " + _dbsqlConnection.PFDB + @".dbo.EmployeePFPayment
where 1=1
and PaymentDate >= (
select min(PeriodStart) from FiscalYearDetail
where year=@year)
and PaymentDate <= (
select max(PeriodEnd) from FiscalYearDetail
where year=@year)
group by employeeid)
PF  on a.EmployeeId=PF.EmployeeId
left outer join (
select distinct employeeid, sum(isnull(Amount,0) )Bonus
from  SalaryBonusDetail
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by employeeid)
Bonus  on a.EmployeeId=Bonus.EmployeeId
left outer join (
select distinct EmployeeId, sum(isnull(DepositAmount,0) )TAXAmount
from TAX_Pectra.dbo.TaxDeposits
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId
)
TAX on a.EmployeeId=TAX.EmployeeId
left outer join(
select distinct EmployeeId, sum(isnull(DeductionAmount,0) )Amount
from  EmployeeEarningLeave
where 1=1
and FiscalYearDetailId in(
select distinct Id from FiscalYearDetail
where year=@year)
group by EmployeeId)
EL on a.EmployeeId=EL.EmployeeId
where e.TIN is not null or e.TIN!=''or e.TIN!='-'


INSERT INTO #Temp1 (NumberWithOutTIN, WithOutTINTotal)
Select  Count(TIN)NumberWithOutTIN,sum (Total)Total from #Temp
where TIN ='NO'


Update #Temp1 set Total=  (Select  sum (Total)Total from #Temp
)

Select NumberWithOutTIN,WithOutTINTotal,Total,(WithOutTINTotal/Total)*100 as [percent] from #Temp1

Drop Table #Temp
Drop Table #Temp1


";
                }


                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@year", vm.FiscalYear);

                da.Fill(dt);




                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable ChildAllowance(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
                #region sqlText
                sqlText = @"
select e.EmpName,e.Code,e.TIN,e.Designation,Project,Department,Section ,Other2 Area, Other3 Location,d.Name DependentName
,CEILING(CAST(DATEDIFF(MONTH,Convert(datetime,DateofBirth),@IssueDate) as decimal)/12) Age,DateOfBirth
,CASE WHEN IsDependentAllowance = 1 THEN 'Y' ELSE 'N' END IsDependentAllowance
,case when e.LeftDate not in ('19000101') and e.LeftDate<=@IssueDate then 'Left' else 'Active' end EmpStatus

from EmployeeDependent d
left outer join 
ViewEmployeeInformation e on d.EmployeeId=e.EmployeeId
where 1=1

";
                #endregion
                #region More Conditions
                if (!string.IsNullOrWhiteSpace(vm.CodeF))
                {
                    sqlText += " and e.Code>= @CodeF";
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeT))
                {
                    sqlText += " and e.Code<= @CodeT";
                }
                if (vm.DesignationList != null && vm.DesignationList.Count > 0)
                {
                    string MultipleDesignation = "";
                    foreach (string item in vm.DesignationList)
                    {
                        MultipleDesignation += "'" + item + "',";
                    }
                    MultipleDesignation = MultipleDesignation.Remove(MultipleDesignation.Length - 1);
                    sqlText += " AND e.DesignationId IN(" + MultipleDesignation + ")";
                }
                if (vm.DepartmentList != null && vm.DepartmentList.Count > 0)
                {
                    string MultipleDepartment = "";
                    foreach (string item in vm.DepartmentList)
                    {
                        MultipleDepartment += "'" + item + "',";
                    }
                    MultipleDepartment = MultipleDepartment.Remove(MultipleDepartment.Length - 1);
                    sqlText += " AND e.DepartmentId IN(" + MultipleDepartment + ")";
                }
                if (vm.SectionList != null && vm.SectionList.Count > 0)
                {
                    string MultipleSection = "";
                    foreach (string item in vm.SectionList)
                    {
                        MultipleSection += "'" + item + "',";
                    }
                    MultipleSection = MultipleSection.Remove(MultipleSection.Length - 1);
                    sqlText += " AND e.SectionId IN(" + MultipleSection + ")";
                }
                if (vm.ProjectList != null && vm.ProjectList.Count > 0)
                {
                    string MultipleProject = "";
                    foreach (string item in vm.ProjectList)
                    {
                        MultipleProject += "'" + item + "',";
                    }
                    MultipleProject = MultipleProject.Remove(MultipleProject.Length - 1);
                    sqlText += " AND e.ProjectId IN(" + MultipleProject + ")";
                }

                if (vm.Other2List != null && vm.Other2List.Count > 0)
                {
                    string MultipleOther2 = "";
                    foreach (string item in vm.Other2List)
                    {
                        MultipleOther2 += "'" + item + "',";
                    }
                    MultipleOther2 = MultipleOther2.Remove(MultipleOther2.Length - 1);
                    sqlText += " AND e.Other2 IN(" + MultipleOther2 + ")";
                }
                if (vm.Other3List != null && vm.Other3List.Count > 0)
                {
                    string MultipleOther3 = "";
                    foreach (string item in vm.Other3List)
                    {
                        MultipleOther3 += "'" + item + "',";
                    }
                    MultipleOther3 = MultipleOther3.Remove(MultipleOther3.Length - 1);
                    sqlText += " AND e.Other3 IN(" + MultipleOther3 + ")";
                }

                #endregion

                sqlText += "    order by e.Section,e.Code";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Parameters.AddWithValue("@IssueDate", Ordinary.DateToString(vm.IssueDate));
                if (!string.IsNullOrWhiteSpace(vm.CodeF))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", vm.CodeF);
                }
                if (!string.IsNullOrWhiteSpace(vm.CodeT))
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeT);

                }

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "DateOfBirth");

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
            return dt;
        }


    }
}
