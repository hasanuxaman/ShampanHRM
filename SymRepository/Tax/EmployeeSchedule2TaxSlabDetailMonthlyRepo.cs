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
    public class EmployeeSchedule2TaxSlabDetailMonthlyRepo
    {
        public List<EmployeeSchedule2TaxSlabDetailVM> DropDown()
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeSchedule2TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule1Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailMonthlyDAL().SelectAll(Id, Schedule1Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EmployeeSchedule2TaxSlabDetailVM vm)
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EmployeeSchedule2TaxSlabDetailVM vm, string[] ids)
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailMonthlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EmployeeSchedule2TaxSlabDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    

    }
}
