using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class EmployeeSchedule3TaxSlabDetailMonthlyRepo
    {
                public List<EmployeeSchedule3TaxSlabDetailVM> DropDown()
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeSchedule3TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule1Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailMonthlyDAL().SelectAll(Id, Schedule1Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EmployeeSchedule3TaxSlabDetailVM vm)
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EmployeeSchedule3TaxSlabDetailVM vm, string[] ids)
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailMonthlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EmployeeSchedule3TaxSlabDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
    }
}
