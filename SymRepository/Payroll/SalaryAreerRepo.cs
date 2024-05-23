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
   public class SalaryAreerRepo
   {
    SalaryAreerDAL _dal = new SalaryAreerDAL();
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
        public string[] SalaryAreerSingleAddorUpdate(SalaryAreerDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryAreerSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryAreerVM> SelectAll(int BranchId,int? fid=null)
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
        public List<SalaryAreerDetailVM> SelectAllSalaryAreerDetails(string salaryAreerId)
        {
            try
            {
                return _dal.SelectAllSalaryAreerDetails(salaryAreerId, null, false);
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
        public string[] SalaryAreerDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryAreerDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryAreerDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryAreerDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryAreerDetailVM GetByIdSalaryAreerDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryAreerDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryAreerSingleEdit(SalaryAreerDetailVM vm)
        {
            try
            {
                return _dal.SalaryAreerSingleEdit(vm);
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
        #endregion
    }
}
