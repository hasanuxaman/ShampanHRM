using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SymAPIRepository
{
    public class LeaveRepo
    {
        public List<EmployeeInfoVM> DropDownCodeName()
        {
            try
            {
                return new EmployeeDAL().DropDownCodeName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Insert(employeeLeaveVM, null, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertSchedule(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().InsertSchedule(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     
        public List<EmployeeLeaveBalanceVM> EmployeeLeaveBalance(string employeeId, string leaveyear)
        {
            try
            {
                return new EmployeeLeaveDAL().EmployeeLeaveBalance(employeeId, leaveyear);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<EmployeeLeaveStructureVM> DropDown(string employeeId, int year)
        {
            try
            {
                return new EmployeeLeaveStructureDAL().DropDown(employeeId, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLeaveVM> SelectAll(string Name)
        {
            try
            {               
                if (Name=="admin")
                {
                    return new EmployeeLeaveDAL().SelectAll();
                }
                else
                {
                    return new EmployeeLeaveDAL().SelectAll(Name, "");
                }
              //  return new EmployeeLeaveDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<EmployeeLeaveVM> SelectAllByDate(string code, string DateFrom, string DateTo)
        {
            try
            {
                if (code == "admin")
                {
                    return new EmployeeLeaveDAL().SelectAllByDateRange(DateFrom, DateTo);
                }
                else
                {
                    return new EmployeeLeaveDAL().SelectAllByDate(code, DateFrom, DateTo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
        public List<EmployeeLeaveVM> SelectAll(string code, string status)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAll(code, status);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string[] Approve(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Approve(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //For pdf report
        public Stream EmployeeLeaveStatementNew(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
   , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType, string EmployeeId, string LId = "0")
        {
            List<EmployeeLeaveBalanceVM> getData = new List<EmployeeLeaveBalanceVM>();

            try
            {
                getData = new EmployeeInfoDAL().EmployeeLeaveStatementNew(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId, LId);


                #region Report Call
                ReportDocument doc = new ReportDocument();

                string ReportHead = "Leave Application From";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveStatementNew.rpt";

                doc.Load(rptLocation);

                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["UpToDate"].Text = "'" + DateTime.Now.ToString("dd/MMM/yyyy") + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = doc.DataDefinition.FormulaFields;
                FormulaField(doc, crFormulaF, "LeaveSignatureLocation", AppDomain.CurrentDomain.BaseDirectory + "File\\SignatureFile\\DefaultSignature.jpg");

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
        public List<EmployeeLeaveVM> SelectAllForSupervisor(string SupervisorId)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAllForSupervisor(SupervisorId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Stream EmployeeLeaveList(string CodeF, string CodeT, string DepartmentId, string SectionId
             , string ProjectId, string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType
             , string EmployeeId, string Gender_E, string Religion, string GradeId)
        {
            try
            {
                List<EmployeeLeaveBalanceVM> getData = new List<EmployeeLeaveBalanceVM>();

                getData = new EmployeeInfoDAL().EmployeeLeaveList(CodeF, CodeT, DepartmentId, SectionId, ProjectId
            , DesignationId, dtpFrom, dtpTo, leaveyear, LeaveType, EmployeeId, Gender_E, Religion, GradeId);

                #region Report Call
                ReportDocument doc = new ReportDocument();

                string ReportHead = "Employee Leave Register";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\HRM\rptEmployeeLeaveList.rpt";

                doc.Load(rptLocation);

                doc.Database.Tables["SymWebUI_Areas_HRM_Report_EmployeeLeaveBalanceVM_Proxy"].SetDataSource(getData);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";             
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

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


        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue)
        {
            try
            {
                FormulaFieldDefinition fs;
                fs = crFormulaF[fieldName];
                objrpt.DataDefinition.FormulaFields[fieldName].Text = "'" + fieldValue + "'";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }    

    }
}
