using SymServices.Common;
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
    public class EmployeeBonusRepo
    {
        EmployeeBonusDAL _dal = new EmployeeBonusDAL();
        #region Methods
        //==================Insert =================
        public string[] Insert(string bonusStructureId, EmployeeBonusDetailVM  vm)
        {
            try
            {
                return _dal.Insert(bonusStructureId,vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeBonusVM> SelectAll(int BranchId)
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
        public EmployeeBonusVM SelectById(string employeeBonusId)
        {
            try
            {
                return _dal.SelectById(employeeBonusId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeBonusDetailVM> SelectAllEmpBonusDetails(string employeeBonusId = null)
        {
            try
            {
                return _dal.SelectAllEmpBonusDetails( null, false, employeeBonusId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeBonusDetailVM SelectByIdBonusDetail(int bonusDetailId)
        {
            try
            {
                return _dal.SelectByIdBonusDetail(bonusDetailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SingleBonusUpdate(EmployeeBonusDetailVM vm)
        {
            try
            {
                return _dal.SingleBonusUpdate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SingleBonusAdd(EmployeeBonusDetailVM vm)
        {
            try
            {
                return _dal.SingleBonusAdd(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeBonusDetailsDelete(EmployeeBonusDetailVM vm, string[] Ids)
        {
            try
            {
                return _dal.EmployeeBonusDetailsDelete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeBonusDelete(EmployeeBonusVM vm, string[] Ids)
        {
            try
            {
                return _dal.EmployeeBonusDelete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
