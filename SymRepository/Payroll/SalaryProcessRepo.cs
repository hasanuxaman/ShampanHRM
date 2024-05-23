using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class SalaryProcessRepo
    {
        SalaryProcessDAL _salaryProcessDAL = new SalaryProcessDAL();

        public string[] SalaryPreProcessNew(int FiscalYearDetailId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
      , string EmployeeIdF, string EmployeeIdT, FiscalYearVM fiscalYearVM, string processName, string CompanyName)
        {
            return _salaryProcessDAL.SalaryPreProcessNew(FiscalYearDetailId, ProjectId, DepartmentId, SectionId, DesignationId
                 , EmployeeIdF, EmployeeIdT, fiscalYearVM, processName, CompanyName, null, null);
        }

        public List<SalaryPFDetailVM> EmployeeSalaryPF(string employeeID)
        {
            return _salaryProcessDAL.EmployeeSalaryPF(employeeID, null, null);
        }
        public List<SalaryTaxDetailVM> EmployeeSalaryTax(string employeeID)
        {
            return _salaryProcessDAL.EmployeeSalaryTax(employeeID, null, null);
        }
        public string[] DeleteProcess(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string fid)
        {
            return _salaryProcessDAL.DeleteProcess(ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, null, null);
        }
        public List<EmployeeInfoVM> EmployeeSalaryEarning(string employeeID)
        {
            return _salaryProcessDAL.EmployeeSalaryEarning(employeeID, null, null);
        }
        public List<EmployeeInfoVM> EmployeeSalaryLoan(string employeeID)
        {
            return _salaryProcessDAL.EmployeeSalaryLoan(employeeID, null, null);
        }

        public List<SalarySheetVM> SalaryPreCalculationBackup(string FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
             , string CodeF, string CodeT, string Name, string dojFrom, string dojTo)
        {
            return _salaryProcessDAL.SalaryPreCalculationBackup(FiscalYearDetailsId, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, Name, dojFrom, dojTo);
        }
        public DataSet ChildAllowanceDetail(string FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT)
        {
            return _salaryProcessDAL.ChildAllowanceDetail(FiscalYearDetailsId, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT);
        }

        public DataTable SalarySheetMainTop(SalarySheetVM vm)
        {
            return _salaryProcessDAL.SalarySheetMainTop(vm);
        }

        public DataTable SalarySheet(SalarySheetVM vm)
        {
            return _salaryProcessDAL.SalarySheet(vm);
        }

        public DataTable TIBHRMSalary(SalarySheetVM vm)
        {
            return _salaryProcessDAL.TIBHRMSalary(vm);
        }
        public DataSet SalarySheet_TIB(string[] conditionFields = null, string[] conditionValues = null, string ReportName = "SalarySheet1"
            , string vFiscalYearDetailId = "0",SalarySheetVM vm=null)
        {
            return _salaryProcessDAL.SalarySheet_TIB(conditionFields, conditionValues, ReportName, vFiscalYearDetailId, vm);
        }      
        public DataSet SalarySheet_TIB_Excel(SalarySheetVM vm=null)
        {
            return _salaryProcessDAL.SalarySheet_TIB_Excel( vm);
        }

        public ResultVM PostToSage(SalarySheetVM vm=null)
        {
            return _salaryProcessDAL.PostToSage( vm);
        }


        public DataSet SalarySheet_TIB_SummeryOther(SalarySheetVM vm = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            return _salaryProcessDAL.SalarySheet_TIB_SummeryOther( vm, conditionFields, conditionValues);
        }
        public DataSet SalarySheet_TIB_SummeryOtherDownload(SalarySheetVM vm = null, string[] conditionFields = null, string[] conditionValues = null,DataTable DtResult=null)
        {
            return _salaryProcessDAL.SalarySheet_TIB_SummeryOtherDownload(vm, conditionFields, conditionValues, null,null,DtResult);
        }
        public DataSet SalaryPreCalculationNew(SalarySheetVM vm)
        {
            return _salaryProcessDAL.SalaryPreCalculationNew(vm);
        }



        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, string fid, List<string> ProjectIdList = null, string other1 = "", string other2 = "", string other3 = "", string Orderby = "", string bankId = "")
        {
            return _salaryProcessDAL.ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId
                , SectionId, DesignationId, CodeF, CodeT, fid, ProjectIdList, other1, other2, other3, Orderby, bankId);
        }
        //public DataSet PaySlipPreCalculation(string FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, string DesignationId
        //     , string CodeF, string CodeT, string dojFrom, string dojTo)
        //{
        //    return _salaryProcessDAL.SalaryPreCalculation(FiscalYearDetailsId, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, dojFrom, dojTo);
        //}

        public List<SalaryInformationVM> SalaryInfomation(string employeeID = null)
        {
            return _salaryProcessDAL.SalaryInfomation(employeeID, null, null);
        }

        public DataTable TAX_108(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.TAX_108(vm);
        }
        public DataTable YearlyTAX(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.YearlyTAX(vm);
        }
        public DataTable TAX_108_WithOutTIN(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.TAX_108_WithOutTIN(vm);
        }
        public DataTable TAX_108A(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.TAX_108A(vm);
        }
        public DataTable ChildAllowance(EmployeeInfoVM vm = null)
        {
            return _salaryProcessDAL.ChildAllowance(vm);
        }
        public DataTable DataExportForSunTemplate(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.DataExportForSunTemplate(vm);
        }
        public DataTable DataExportForSunTemplatePayment(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.DataExportForSunTemplatePayment(vm);
        }
        public DataTable DataExportForSunCarTransport(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.DataExportForSunCarTransport(vm);
        }
        public DataTable DataExportForSunCarTransportPayment(SalarySheetVM vm = null)
        {
            return _salaryProcessDAL.DataExportForSunCarTransportPayment(vm);
        }
    }
}
