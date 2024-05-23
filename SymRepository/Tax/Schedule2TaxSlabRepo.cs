using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymRepository.Tax
{
    public class Schedule2TaxSlabRepo
    {
        public List<Schedule2TaxSlabVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new Schedule2TaxSlabDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule2TaxSlabVM vm)
        {
            try
            {
                return new Schedule2TaxSlabDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule2TaxSlabVM vm)
        {
            try
            {
                return new Schedule2TaxSlabDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(Schedule2TaxSlabVM vm, string[] ids)
        {
            try
            {
                return new Schedule2TaxSlabDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
