using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class SalaryReimbursableExpenseRepo
   {
    SalaryReimbursableExpenseDAL _dal = new SalaryReimbursableExpenseDAL();
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
        public string[] SalaryReimbursableExpenseSingleAddorUpdate(SalaryReimbursableExpenseDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryReimbursableExpenseSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryReimbursableExpenseVM> SelectAll(int BranchId,int? fid=null)
        {
            try
            {
                return _dal.SelectAll(BranchId,fid);
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
        public List<SalaryReimbursableExpenseDetailVM> SelectAllSalaryReimbursableExpenseDetails(string salaryReimbursableExpenseId)
        {
            try
            {
                return _dal.SelectAllSalaryReimbursableExpenseDetails(salaryReimbursableExpenseId, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public EmployeeBonusDetailVM SelectByIdBonusDetail(int bonusDetailId)
        //{
        //    try
        //    {
        //        return _dal.SelectByIdBonusDetail(bonusDetailId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public string[] SingleBonusUpdate(EmployeeBonusDetailVM vm)
        //{
        //    try
        //    {
        //        return _dal.SingleBonusUpdate(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public string[] SingleBonusAdd(EmployeeBonusDetailVM vm)
        //{
        //    try
        //    {
        //        return _dal.SingleBonusAdd(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public string[] SalaryReimbursableExpenseDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryReimbursableExpenseDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryReimbursableExpenseDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryReimbursableExpenseDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryReimbursableExpenseDetailVM GetByIdSalaryReimbursableExpenseDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryReimbursableExpenseDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryReimbursableExpenseSingleEdit(SalaryReimbursableExpenseDetailVM vm)
        {
            try
            {
                return _dal.SalaryReimbursableExpenseSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPeriodName(string SalaryReimbursableExpenseId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryReimbursableExpenseId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
