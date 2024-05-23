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
    public class EmployeeSchedule2TaxSlabDetailYearlyRepo
    {
        public List<EmployeeSchedule2TaxSlabDetailVM> DropDown()
        {
            try
            {
                return new EmployeeSchedule2TaxSlabDetailYearlyDAL().DropDown();
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
                return new EmployeeSchedule2TaxSlabDetailYearlyDAL().SelectAll(Id, Schedule1Id, conditionFields, conditionValues);
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
                return new EmployeeSchedule2TaxSlabDetailYearlyDAL().Insert(vm);
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
                return new EmployeeSchedule2TaxSlabDetailYearlyDAL().Delete(vm, ids);
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
                return new EmployeeSchedule2TaxSlabDetailYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
