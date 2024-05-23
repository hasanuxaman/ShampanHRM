using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymServices.Attendance;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymAPIRepository
{
    public class AttendanceRepo
    {
        public Stream AttendanceReport(EmployeeInfoVM vm, string[] conFields, string[] conValues)
        {
            try
            {
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                vm = new EmployeeInfoVM();
                string ReportHead = "";
                string rptLocation = "";              

                table = new DailyAttendanceProcessDAL().Report(vm, conFields, conValues);
                          

                #region Report Call
                ReportDocument doc = new ReportDocument();
                ReportHead = "There are no data to Preview for Attendance Daily (" + vm.AttnStatus + ")";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Attendance Daily List (" + vm.AttnStatus + ")";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtDailyAttendanceProcess";           
               
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSingle.rpt"; 

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullOT"].Text = "'" + vm.FullOT + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;

                #endregion


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                doc.Close();
                return stream;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Stream AttendanceSummeryReport(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                vm = new EmployeeInfoVM();
                vm.DateFrom = conValues[2];
                vm.DateTo = conValues[3];
                string thirdValue = conValues[2];

                string ReportHead = "";
                string rptLocation = "";

                table = new DailyAttendanceProcessDAL().ReportAttendanceSummery(vm, conFields, conValues);


                #region Report Call
                ReportDocument doc = new ReportDocument();
                ReportHead = "There are no data to Preview for Attendance Daily (" + vm.AttnStatus + ")";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Attendance Daily Summery (From " + vm.DateFrom + " to " + vm.DateTo + ")";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtDailyAttendanceProcess";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Attendance\\rptDailyAttendanceSummery.rpt";

                CompanyDAL cdal = new CompanyDAL(); 
                CompanyVM cvm = cdal.SelectAll().FirstOrDefault();

                string Logo = new AppSettingsReader().GetValue("Logo", typeof(string)).ToString();

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Logo;
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["FullOT"].Text = "'" + vm.FullOT + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;

                #endregion


                Stream stream = doc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                doc.Close();
                return stream;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
