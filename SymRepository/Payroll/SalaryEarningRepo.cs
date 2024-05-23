using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class SalaryEarningRepo
    {
        SalaryEarningDAL _dal = new SalaryEarningDAL();
        #region Methods
        //==================Insert =================

        public string[] SalaryEarningSingleAddorUpdate(List<SalaryEarningDetailVM> vm)
        {
            try
            {
                return _dal.SalaryEarningSingleAddorUpdate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryEarningDetailVM> SelectAll(int? fid = null)
        {
            try
            {
                return _dal.SelectAll(fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryEarningDetailVM SelectById(string Id)
          {
            try
            {
                return _dal.SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
                //==================Distinct Employee Name =================
        public List<EmployeeInfoVM> GetEmployeebyfid(int? fid)
        {
            try
            {
                return _dal.GetEmployeebyfid( fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryEarningDetailVM> SelectByIdandFiscalyearDetail(string empId, int fid = 0)
        {
            try
            {
                return _dal.SelectByIdandFiscalyearDetail(empId, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryEarningDetailVM> GetPeriodname()
        {
            try
            {
                return _dal.GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> SelectAllSalaryEarningDetailsEmployeeAndPeriod(string salaryEarningId)
        {
            try
            {
                return _dal.SelectAllSalaryEarningDetailsEmployeeAndPeriod(salaryEarningId, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> SelectAllSalaryEarningDetails(string employeeId,int periodId)
        {
            try
            {
                return _dal.SelectAllSalaryEarningDetails(employeeId,periodId, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryEarningDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryEarningDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryEarningDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryEarningDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryEarningSingleEdit(SalaryEarningDetailVM vm)
        {
            try
            {
                return _dal.SalaryEarningSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPeriodName(string SalaryAreerId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryAreerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string fileName, SalaryEarningDetailVM vm)
        {
            try
            {
                return new SalaryEarningDAL().ImportExcelFile(fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ExportExcelFile(string Filepath, string FileName, int fid, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT)
        {
            return new SalaryEarningDAL().ExportExcelFile(Filepath, FileName, fid, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT);
        }
        public SalaryEarningDetailVM SalaryEarningBySalaryTypeSingle(string Id, string employeeId, string SalaryTypeId)

        {
            try
            {
                return _dal.SalaryEarningBySalaryTypeSingle(Id, employeeId, SalaryTypeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<SalaryEarningDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
        {
            try
            {
                return _dal.SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataSet SalarySheet(SalarySheetVM vm)
        {
            return _dal.SalarySheet(vm);
        }
        #endregion
    }
}
