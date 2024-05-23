using SymServices.GF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.GF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.GF
{
    public class EmployeeSettlementRepo
    {
        public List<EmployeeSettlementVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new EmployeeSettlementDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeSettlementVM> SelectLeftEmployeeList(string EmployeeId = "", string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSettlementDAL().SelectLeftEmployeeList(EmployeeId, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeSettlementVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSettlementDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EmployeeSettlementVM vm)
        {
            try
            {
                return new EmployeeSettlementDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EmployeeSettlementVM vm)
        {
            try
            {
                return new EmployeeSettlementDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(EmployeeSettlementVM vm, string[] ids)
        {
            try
            {
                return new EmployeeSettlementDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(EmployeeSettlementVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSettlementDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
