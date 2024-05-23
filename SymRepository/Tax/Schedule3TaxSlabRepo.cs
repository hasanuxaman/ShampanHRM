using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymRepository.Tax
{
    public class Schedule3TaxSlabRepo
    {
                public List<Schedule3TaxSlabVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new Schedule3TaxSlabDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule3TaxSlabVM vm)
        {
            try
            {
                return new Schedule3TaxSlabDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule3TaxSlabVM vm)
        {
            try
            {
                return new Schedule3TaxSlabDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(Schedule3TaxSlabVM vm, string[] ids)
        {
            try
            {
                return new Schedule3TaxSlabDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
