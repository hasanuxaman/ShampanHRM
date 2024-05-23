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
    public class EmployeeSchedule3TaxSlabDetailYearlyRepo
    {
        
        public List<EmployeeSchedule3TaxSlabDetailVM> DropDown()
        {
            try
            {
                return new EmployeeSchedule3TaxSlabDetailYearlyDAL().DropDown();
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
                return new EmployeeSchedule3TaxSlabDetailYearlyDAL().SelectAll(Id, Schedule1Id, conditionFields, conditionValues);
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
                return new EmployeeSchedule3TaxSlabDetailYearlyDAL().Insert(vm);
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
                return new EmployeeSchedule3TaxSlabDetailYearlyDAL().Delete(vm, ids);
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
                return new EmployeeSchedule3TaxSlabDetailYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
