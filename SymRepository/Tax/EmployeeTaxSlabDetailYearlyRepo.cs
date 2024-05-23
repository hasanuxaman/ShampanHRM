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
    public class EmployeeTaxSlabDetailYearlyRepo
    {
        public List<EmployeeTaxSlabDetailVM> DropDown()
        {
            try
            {
                return new EmployeeTaxSlabDetailYearlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeTaxSlabDetailVM> SelectAll(int Id = 0, int Schedule1Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeTaxSlabDetailYearlyDAL().SelectAll(Id, Schedule1Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EmployeeTaxSlabDetailVM vm)
        {
            try
            {
                return new EmployeeTaxSlabDetailYearlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EmployeeTaxSlabDetailVM vm, string[] ids)
        {
            try
            {
                return new EmployeeTaxSlabDetailYearlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EmployeeTaxSlabDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeTaxSlabDetailYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
