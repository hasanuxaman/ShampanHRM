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
   public class SalaryOverTimeRepo
   {
    SalaryOverTimeDAL _dal = new SalaryOverTimeDAL();
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
        public string[] SalaryOverTimeSingleAddorUpdate(SalaryOverTimeDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryOverTimeSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryOverTimeVM> SelectAll(int BranchId,int? fid=null)
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
        public List<SalaryOverTimeDetailVM> SelectAllSalaryOverTimeDetails(string salaryOverTimeId)
        {
            try
            {
                return _dal.SelectAllSalaryOverTimeDetails(salaryOverTimeId, null, false);
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
        public string[] SalaryOverTimeDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryOverTimeDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryOverTimeDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryOverTimeDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryOverTimeDetailVM GetByIdSalaryOverTimeDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryOverTimeDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryOverTimeSingleEdit(SalaryOverTimeDetailVM vm)
        {
            try
            {
                return _dal.SalaryOverTimeSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPeriodName(string SalaryOverTimeId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryOverTimeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
