using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Payroll
{
    public class SalaryLoanRepo
   {
    SalaryLoanDAL _dal = new SalaryLoanDAL();
        #region Methods
        //==================Insert =================
        public string[] AddOrUpdate(int FiscalYearDetailsId, string ProjectId, string DepartmentId, string SectionId, FiscalYearVM vm)
        {
            try
            {
                return _dal.AddOrUpdate(FiscalYearDetailsId, ProjectId, DepartmentId, SectionId,  vm,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string[] SalaryLoanSingleAddorUpdate(SalaryLoanDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryLoanSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<SalaryLoanDetailVM> SelectAll(int? fid = 0)
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

        public List<SalaryLoanDetailVM> GetPeriodname()
        {
            try
            {
                return new SalaryLoanDAL().GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public EmployeeBonusVM SelectById(string employeeBonusId)
        //{
        //    try
        //    {
        //        return _dal.SelectById(employeeBonusId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        
        public List<SalaryLoanDetailVM> SelectAllSalaryLoanDetails(string salaryLoanId)
        {
            try
            {
                return _dal.SelectAllSalaryLoanDetails(salaryLoanId, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public SalaryLoanDetailVM GetByIdSalaryLoanDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryLoanDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        
        public List<SalaryLoanDetailVM> SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId =0)
        {
            try
            {
                return _dal.SelectByIdandFiscalyearDetail( empId,  FiscalYearDetailId );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string[] SalaryLoanSingleEdit(SalaryLoanDetailVM vm)
        //{
        //    try
        //    {
        //        return _dal.SalaryLoanSingleEdit(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        
        public string GetPeriodName(string SalaryLoanId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryLoanId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //==================Delete =================
        public string[] Delete(SalaryLoanVM vm, string[] Ids)
        {
            try
            {
                return new SalaryLoanDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<SalaryLoanDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
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
         public decimal GetChildAllowanceAmount(string empid,int fid ) {
             try
             {
                 return _dal.GetChildAllowanceAmount( empid, fid);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }

        #endregion      
   }
}

