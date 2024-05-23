using SymOrdinary;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymServices.Attendance
{
    public class EarningDeductionStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EarningDeductionStructureVM> SelectByMultiCondition(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string sqlText = "";
            List<EarningDeductionStructureVM> VMs = new List<EarningDeductionStructureVM>();
            EarningDeductionStructureVM vm;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
SELECT
Id
,Name
,DaysCountFrom
,FirstSlotAbsentFrom  
,IsNull(FirstSlotAbsentDays, 0) FirstSlotAbsentDays
,SecondSlotAbsentFrom
,IsNull(SecondSlotAbsentDays, 0) SecondSlotAbsentDays
,NPAbsentFrom
,LWPFrom

,IsNull(IsMonthlyLateInDeduct, 0) IsMonthlyLateInDeduct   
,IsNull(IsMonthlyLateInHourlyCount, 0) IsMonthlyLateInHourlyCount
,IsNull(MonthlyLateInCountDays, 0) MonthlyLateInCountDays
,IsNull(LateInAbsentDays, 0) LateInAbsentDays
,IsNull(IsMonthlyEarlyOutDeduct, 0) IsMonthlyEarlyOutDeduct
,IsNull(IsMonthlyEarlyOutHourlyCount, 0) IsMonthlyEarlyOutHourlyCount
,IsNull(MonthlyEarlyOutCountDays, 0) MonthlyEarlyOutCountDays
,IsNull(EarlyOutAbsentDays, 0) EarlyOutAbsentDays

,IsNull(WeeklyOTRate, 0)       WeeklyOTRate
,IsNull(GovtOTRate, 0)         GovtOTRate
,IsNull(FestivalOTRate, 0)     FestivalOTRate
,IsNull(SpecialOTRate, 0)      SpecialOTRate

,IsNull(DayRateCountFrom, 0)   DayRateCountFrom
,IsNull(HourRateCountFrom, 0)   HourRateCountFrom
,IsNull(DayRateDivisionFactor, 0)   DayRateDivisionFactor
,IsNull(HourRateDivisionFactor, 0)   HourRateDivisionFactor

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EarningDeductionStructure
Where 1=1 AND IsArchive=0
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
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

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
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EarningDeductionStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.DaysCountFrom = dr["DaysCountFrom"].ToString();
                    vm.FirstSlotAbsentFrom = dr["FirstSlotAbsentFrom"].ToString();
                    vm.FirstSlotAbsentDays = Convert.ToInt32(dr["FirstSlotAbsentDays"]);
                    vm.SecondSlotAbsentFrom = dr["SecondSlotAbsentFrom"].ToString();
                    vm.SecondSlotAbsentDays = Convert.ToInt32(dr["SecondSlotAbsentDays"]);
                    vm.NPAbsentFrom = dr["NPAbsentFrom"].ToString();
                    vm.LWPFrom = dr["LWPFrom"].ToString();

                    vm.IsMonthlyLateInDeduct = Convert.ToBoolean(dr["IsMonthlyLateInDeduct"]);
                    vm.IsMonthlyLateInHourlyCount = Convert.ToBoolean(dr["IsMonthlyLateInHourlyCount"]);
                    vm.MonthlyLateInCountDays = Convert.ToInt32(dr["MonthlyLateInCountDays"]);
                    vm.LateInAbsentDays = Convert.ToInt32(dr["LateInAbsentDays"]);
                    vm.IsMonthlyEarlyOutDeduct = Convert.ToBoolean(dr["IsMonthlyEarlyOutDeduct"]);
                    vm.IsMonthlyEarlyOutHourlyCount = Convert.ToBoolean(dr["IsMonthlyEarlyOutHourlyCount"]);
                    vm.MonthlyEarlyOutCountDays = Convert.ToInt32(dr["MonthlyEarlyOutCountDays"]);
                    vm.EarlyOutAbsentDays = Convert.ToInt32(dr["EarlyOutAbsentDays"]);

                    vm.WeeklyOTRate = Convert.ToDecimal(dr["WeeklyOTRate"]);
                    vm.GovtOTRate = Convert.ToDecimal(dr["GovtOTRate"]);
                    vm.FestivalOTRate = Convert.ToDecimal(dr["FestivalOTRate"]);
                    vm.SpecialOTRate = Convert.ToDecimal(dr["SpecialOTRate"]);
                    vm.DayRateCountFrom = dr["DayRateCountFrom"].ToString();
                    vm.HourRateCountFrom = dr["HourRateCountFrom"].ToString();
                    vm.DayRateDivisionFactor = Convert.ToInt32(dr["DayRateDivisionFactor"]);
                    vm.HourRateDivisionFactor = Convert.ToInt32(dr["HourRateDivisionFactor"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( dr["CreatedAt"].ToString()))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))));
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(dr["LastUpdateAt"].ToString()))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))));
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
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
            return VMs;
        }

        //==================SelectAll=================
        public List<EarningDeductionStructureVM> SelectAll(int Id = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string sqlText = "";
            List<EarningDeductionStructureVM> VMs = new List<EarningDeductionStructureVM>();
            EarningDeductionStructureVM vm;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
SELECT
Id
,Name
,DaysCountFrom
,FirstSlotAbsentFrom  
,IsNull(FirstSlotAbsentDays, 0) FirstSlotAbsentDays
,SecondSlotAbsentFrom
,IsNull(SecondSlotAbsentDays, 0) SecondSlotAbsentDays
,NPAbsentFrom
,LWPFrom

,IsNull(IsMonthlyLateInDeduct, 0) IsMonthlyLateInDeduct   
,IsNull(IsMonthlyLateInHourlyCount, 0) IsMonthlyLateInHourlyCount
,IsNull(MonthlyLateInCountDays, 0) MonthlyLateInCountDays
,IsNull(LateInAbsentDays, 0) LateInAbsentDays
,IsNull(IsMonthlyEarlyOutDeduct, 0) IsMonthlyEarlyOutDeduct
,IsNull(IsMonthlyEarlyOutHourlyCount, 0) IsMonthlyEarlyOutHourlyCount
,IsNull(MonthlyEarlyOutCountDays, 0) MonthlyEarlyOutCountDays
,IsNull(EarlyOutAbsentDays, 0) EarlyOutAbsentDays

,IsNull(WeeklyOTRate, 0)       WeeklyOTRate
,IsNull(GovtOTRate, 0)         GovtOTRate
,IsNull(FestivalOTRate, 0)     FestivalOTRate
,IsNull(SpecialOTRate, 0)      SpecialOTRate

,IsNull(DayRateCountFrom, 0)        DayRateCountFrom
,IsNull(HourRateCountFrom, 0)       HourRateCountFrom
,IsNull(DayRateDivisionFactor, 0)    DayRateDivisionFactor
,IsNull(HourRateDivisionFactor, 0)  HourRateDivisionFactor


,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EarningDeductionStructure
Where 1=1 AND IsArchive=0
";
                //DayRateCountFrom
                //HourRateCountFrom
                //DayRateDivisionFactor
                //HourRateDivisionFactor



                if (Id > 0)
                {
                    sqlText += " AND Id=@Id";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EarningDeductionStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.DaysCountFrom = dr["DaysCountFrom"].ToString();
                    vm.FirstSlotAbsentFrom = dr["FirstSlotAbsentFrom"].ToString();
                    vm.FirstSlotAbsentDays = Convert.ToInt32(dr["FirstSlotAbsentDays"]);
                    vm.SecondSlotAbsentFrom = dr["SecondSlotAbsentFrom"].ToString();
                    vm.SecondSlotAbsentDays = Convert.ToInt32(dr["SecondSlotAbsentDays"]);
                    vm.NPAbsentFrom = dr["NPAbsentFrom"].ToString();
                    vm.LWPFrom = dr["LWPFrom"].ToString();

                    vm.IsMonthlyLateInDeduct = Convert.ToBoolean(dr["IsMonthlyLateInDeduct"]);
                    vm.IsMonthlyLateInHourlyCount = Convert.ToBoolean(dr["IsMonthlyLateInHourlyCount"]);
                    vm.MonthlyLateInCountDays = Convert.ToInt32(dr["MonthlyLateInCountDays"]);
                    vm.LateInAbsentDays = Convert.ToInt32(dr["LateInAbsentDays"]);
                    vm.IsMonthlyEarlyOutDeduct = Convert.ToBoolean(dr["IsMonthlyEarlyOutDeduct"]);
                    vm.IsMonthlyEarlyOutHourlyCount = Convert.ToBoolean(dr["IsMonthlyEarlyOutHourlyCount"]);
                    vm.MonthlyEarlyOutCountDays = Convert.ToInt32(dr["MonthlyEarlyOutCountDays"]);
                    vm.EarlyOutAbsentDays = Convert.ToInt32(dr["EarlyOutAbsentDays"]);


                    vm.WeeklyOTRate = Convert.ToDecimal(dr["WeeklyOTRate"]);
                    vm.GovtOTRate = Convert.ToDecimal(dr["GovtOTRate"]);
                    vm.FestivalOTRate = Convert.ToDecimal(dr["FestivalOTRate"]);
                    vm.SpecialOTRate = Convert.ToDecimal(dr["SpecialOTRate"]);

                    vm.DayRateCountFrom = dr["DayRateCountFrom"].ToString();
                    vm.HourRateCountFrom = dr["HourRateCountFrom"].ToString();
                    vm.DayRateDivisionFactor = Convert.ToInt32(dr["DayRateDivisionFactor"]);
                    vm.HourRateDivisionFactor = Convert.ToInt32(dr["HourRateDivisionFactor"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( Ordinary.StringToDate( dr["CreatedAt"].ToString()))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))));
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(dr["LastUpdateAt"].ToString()))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))));
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
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
            return VMs;
        }
        //==================Insert =================
        public string[] Insert(EarningDeductionStructureVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            int transResult = 0;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(DeductionStructureVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
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
                #region Exist
                bool isExist = false;
                isExist = _cDal.ExistCheck("EarningDeductionStructure", "Name", vm.Name.Trim(), currConn, transaction);
                if (isExist)
                {
                    retResults[1] = "This Name :" + vm.Name.Trim() + " already used!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                #endregion Exist
                #region Save
                vm.Id = _cDal.NextId("EarningDeductionStructure", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO EarningDeductionStructure(
Id
,Name
,DaysCountFrom
,FirstSlotAbsentFrom
,FirstSlotAbsentDays
,SecondSlotAbsentFrom
,SecondSlotAbsentDays
,NPAbsentFrom
,LWPFrom

,IsMonthlyLateInDeduct
,IsMonthlyLateInHourlyCount
,MonthlyLateInCountDays
,LateInAbsentDays
,IsMonthlyEarlyOutDeduct
,IsMonthlyEarlyOutHourlyCount
,MonthlyEarlyOutCountDays
,EarlyOutAbsentDays


,WeeklyOTRate
,GovtOTRate
,FestivalOTRate
,SpecialOTRate

,DayRateCountFrom
,HourRateCountFrom
,DayRateDivisionFactor
,HourRateDivisionFactor

,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
@Id
,@Name
,@DaysCountFrom
,@FirstSlotAbsentFrom
,@FirstSlotAbsentDays
,@SecondSlotAbsentFrom
,@SecondSlotAbsentDays
,@NPAbsentFrom
,@LWPFrom

,@IsMonthlyLateInDeduct
,@IsMonthlyLateInHourlyCount
,@MonthlyLateInCountDays
,@LateInAbsentDays
,@IsMonthlyEarlyOutDeduct
,@IsMonthlyEarlyOutHourlyCount
,@MonthlyEarlyOutCountDays
,@EarlyOutAbsentDays


,@WeeklyOTRate
,@GovtOTRate
,@FestivalOTRate
,@SpecialOTRate

,@DayRateCountFrom
,@HourRateCountFrom
,@DayRateDivisionFactor
,@HourRateDivisionFactor

,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdInsert.Parameters.AddWithValue("@DaysCountFrom", vm.DaysCountFrom ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FirstSlotAbsentFrom", vm.FirstSlotAbsentFrom ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@FirstSlotAbsentDays", vm.FirstSlotAbsentDays);
                    cmdInsert.Parameters.AddWithValue("@SecondSlotAbsentFrom", vm.SecondSlotAbsentFrom ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SecondSlotAbsentDays", vm.SecondSlotAbsentDays);
                    cmdInsert.Parameters.AddWithValue("@NPAbsentFrom", vm.NPAbsentFrom ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LWPFrom", vm.LWPFrom ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@IsMonthlyLateInDeduct", vm.IsMonthlyLateInDeduct);
                    cmdInsert.Parameters.AddWithValue("@IsMonthlyLateInHourlyCount", vm.IsMonthlyLateInHourlyCount);
                    cmdInsert.Parameters.AddWithValue("@MonthlyLateInCountDays", vm.MonthlyLateInCountDays);
                    cmdInsert.Parameters.AddWithValue("@LateInAbsentDays", vm.LateInAbsentDays);
                    cmdInsert.Parameters.AddWithValue("@IsMonthlyEarlyOutDeduct", vm.IsMonthlyEarlyOutDeduct);
                    cmdInsert.Parameters.AddWithValue("@IsMonthlyEarlyOutHourlyCount", vm.IsMonthlyEarlyOutHourlyCount);
                    cmdInsert.Parameters.AddWithValue("@MonthlyEarlyOutCountDays", vm.MonthlyEarlyOutCountDays);
                    cmdInsert.Parameters.AddWithValue("@EarlyOutAbsentDays", vm.EarlyOutAbsentDays);


                    cmdInsert.Parameters.AddWithValue("@WeeklyOTRate", vm.WeeklyOTRate);
                    cmdInsert.Parameters.AddWithValue("@GovtOTRate", vm.GovtOTRate);
                    cmdInsert.Parameters.AddWithValue("@FestivalOTRate", vm.FestivalOTRate);
                    cmdInsert.Parameters.AddWithValue("@SpecialOTRate", vm.SpecialOTRate);
                    cmdInsert.Parameters.AddWithValue("@DayRateCountFrom", vm.DayRateCountFrom);
                    cmdInsert.Parameters.AddWithValue("@HourRateCountFrom", vm.HourRateCountFrom);
                    cmdInsert.Parameters.AddWithValue("@DayRateDivisionFactor", vm.DayRateDivisionFactor);
                    cmdInsert.Parameters.AddWithValue("@HourRateDivisionFactor", vm.HourRateDivisionFactor);
                    

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exec = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exec);
                    if (transResult <= 0)
                    {
                        retResults[1] = "Unexpected Error - Deduction Structure Saving!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }
                else
                {
                    retResults[1] = "Please Input Deduction Structure!";
                    throw new ArgumentNullException(retResults[1], "");
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
        //==================Update =================
        public string[] Update(EarningDeductionStructureVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction
                #region Exist
                #endregion Exist
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE EarningDeductionStructure SET";
                    sqlText += " Name=@Name";
                    sqlText += " ,DaysCountFrom=@DaysCountFrom";
                    sqlText += " ,FirstSlotAbsentFrom=@FirstSlotAbsentFrom";
                    sqlText += " ,FirstSlotAbsentDays=@FirstSlotAbsentDays";
                    sqlText += " ,SecondSlotAbsentFrom=@SecondSlotAbsentFrom";
                    sqlText += " ,SecondSlotAbsentDays=@SecondSlotAbsentDays";
                    sqlText += " ,NPAbsentFrom=@NPAbsentFrom";
                    sqlText += " ,LWPFrom=@LWPFrom";

                    sqlText += " ,IsMonthlyLateInDeduct=@IsMonthlyLateInDeduct";
                    sqlText += " ,IsMonthlyLateInHourlyCount=@IsMonthlyLateInHourlyCount";
                    sqlText += " ,MonthlyLateInCountDays=@MonthlyLateInCountDays";
                    sqlText += " ,LateInAbsentDays=@LateInAbsentDays";
                    sqlText += " ,IsMonthlyEarlyOutDeduct=@IsMonthlyEarlyOutDeduct";
                    sqlText += " ,IsMonthlyEarlyOutHourlyCount=@IsMonthlyEarlyOutHourlyCount";
                    sqlText += " ,MonthlyEarlyOutCountDays=@MonthlyEarlyOutCountDays";
                    sqlText += " ,EarlyOutAbsentDays=@EarlyOutAbsentDays";

                    sqlText += " ,WeeklyOTRate=@WeeklyOTRate";
                    sqlText += " ,GovtOTRate=@GovtOTRate";
                    sqlText += " ,FestivalOTRate=@FestivalOTRate";
                    sqlText += " ,SpecialOTRate=@SpecialOTRate";
                    sqlText += " ,DayRateCountFrom=@DayRateCountFrom";
                    sqlText += " ,HourRateCountFrom=@HourRateCountFrom";
                    sqlText += " ,DayRateDivisionFactor=@DayRateDivisionFactor";
                    sqlText += " ,HourRateDivisionFactor=@HourRateDivisionFactor";

                    
                    
                    
                    
                    sqlText += " ,Remarks=@Remarks";
                    sqlText += " ,IsActive=@IsActive";
                    sqlText += " ,IsArchive=@IsArchive";
                    sqlText += " ,LastUpdateBy=@LastUpdateBy";
                    sqlText += " ,LastUpdateAt=@LastUpdateAt";
                    sqlText += " ,LastUpdateFrom=@LastUpdateFrom";

                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@DaysCountFrom", vm.DaysCountFrom ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FirstSlotAbsentFrom", vm.FirstSlotAbsentFrom ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@FirstSlotAbsentDays", vm.FirstSlotAbsentDays);
                    cmdUpdate.Parameters.AddWithValue("@SecondSlotAbsentFrom", vm.SecondSlotAbsentFrom ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SecondSlotAbsentDays", vm.SecondSlotAbsentDays);
                    cmdUpdate.Parameters.AddWithValue("@NPAbsentFrom", vm.NPAbsentFrom ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LWPFrom", vm.LWPFrom ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsMonthlyLateInDeduct", vm.IsMonthlyLateInDeduct);
                    cmdUpdate.Parameters.AddWithValue("@IsMonthlyLateInHourlyCount", vm.IsMonthlyLateInHourlyCount);
                    cmdUpdate.Parameters.AddWithValue("@MonthlyLateInCountDays", vm.MonthlyLateInCountDays);
                    cmdUpdate.Parameters.AddWithValue("@LateInAbsentDays", vm.LateInAbsentDays);
                    cmdUpdate.Parameters.AddWithValue("@IsMonthlyEarlyOutDeduct", vm.IsMonthlyEarlyOutDeduct);
                    cmdUpdate.Parameters.AddWithValue("@IsMonthlyEarlyOutHourlyCount", vm.IsMonthlyEarlyOutHourlyCount);
                    cmdUpdate.Parameters.AddWithValue("@MonthlyEarlyOutCountDays", vm.MonthlyEarlyOutCountDays);
                    cmdUpdate.Parameters.AddWithValue("@EarlyOutAbsentDays", vm.EarlyOutAbsentDays);



                    cmdUpdate.Parameters.AddWithValue("@WeeklyOTRate", vm.WeeklyOTRate);
                    cmdUpdate.Parameters.AddWithValue("@GovtOTRate", vm.GovtOTRate);
                    cmdUpdate.Parameters.AddWithValue("@FestivalOTRate", vm.FestivalOTRate);
                    cmdUpdate.Parameters.AddWithValue("@SpecialOTRate", vm.SpecialOTRate);
                    cmdUpdate.Parameters.AddWithValue("@DayRateCountFrom", vm.DayRateCountFrom);
                    cmdUpdate.Parameters.AddWithValue("@HourRateCountFrom", vm.HourRateCountFrom);
                    cmdUpdate.Parameters.AddWithValue("@DayRateDivisionFactor", vm.DayRateDivisionFactor);
                    cmdUpdate.Parameters.AddWithValue("@HourRateDivisionFactor", vm.HourRateDivisionFactor);
                    
                    
                    
                    
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exec = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exec);
                    if (transResult <= 0)
                    {
                        retResults[1] = "Unexpected Error - Deduction Structure Updating!";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", DeductionStructureVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    retResults[1] = "Please Input - Deduction Structure!";
                    throw new ArgumentNullException(retResults[1], "");
                }
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
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
        //==================Delete =================
        public string[] Delete(EarningDeductionStructureVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete Attendance Structure"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EarningDeductionStructure set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Time Policy Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException(" Delete", "Could not found any item.");
                }
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
                    retResults[1] = "Unexpected error to delete Time Policy Information.";
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
        //==================DropDown =================
        public List<EarningDeductionStructureVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EarningDeductionStructureVM> VMs = new List<EarningDeductionStructureVM>();
            EarningDeductionStructureVM vm;
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
                sqlText = @"SELECT
Id,
Name
   FROM EarningDeductionStructure
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EarningDeductionStructureVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
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
        #endregion
    }
}
