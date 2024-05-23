using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymRepository.Tax
{
    public class Schedule3TaxSlabDetailRepo
    {
        public List<Schedule3TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule3TaxSlabId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new Schedule3TaxSlabDetailDAL().SelectAll(Id, Schedule3TaxSlabId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule3TaxSlabDetailVM vm)
        {
            try
            {
                return new Schedule3TaxSlabDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
