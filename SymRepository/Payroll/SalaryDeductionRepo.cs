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
   public class SalaryDeductionRepo
   {
    SalaryDeductionDAL _dal = new SalaryDeductionDAL();
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
        public string[] SalaryDeductionSingleAddorUpdate(SalaryDeductionDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryDeductionSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryDeductionVM> SelectAll(int BranchId)
        {
            try
            {
                return _dal.SelectAll(BranchId);
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
        public List<SalaryDeductionDetailVM> SelectAllSalaryDeductionDetails(string salaryDeductionId)
        {
            try
            {
                return _dal.SelectAllSalaryDeductionDetails(salaryDeductionId, null, false);
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
        public string[] SalaryDeductionDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryDeductionDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryDeductionDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryDeductionDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryDeductionDetailVM GetByIdSalaryDeductionDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryDeductionDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryDeductionSingleEdit(SalaryDeductionDetailVM vm)
        {
            try
            {
                return _dal.SalaryDeductionSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPeriodName(string SalaryDeductionId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryDeductionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
