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
   public class SalaryConvenceRepo
   {
    SalaryConvenceDAL _dal = new SalaryConvenceDAL();
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
        public string[] SalaryConvenceSingleAddorUpdate(SalaryConvenceDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryConvenceSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryConvenceVM> SelectAll(int BranchId,int? fid=null)
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
        public List<SalaryConvenceDetailVM> SelectAllSalaryConvenceDetails(string salaryConvenceId)
        {
            try
            {
                return _dal.SelectAllSalaryConvenceDetails(salaryConvenceId, null, false);
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
        public string[] SalaryConvenceDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryConvenceDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryConvenceDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryConvenceDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryConvenceDetailVM GetByIdSalaryConvenceDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryConvenceDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryConvenceSingleEdit(SalaryConvenceDetailVM vm)
        {
            try
            {
                return _dal.SalaryConvenceSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPeriodName(string SalaryConvenceId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryConvenceId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
