using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymRepository.Tax
{
    public class Schedule2TaxSlabDetailRepo
    {
        public List<Schedule2TaxSlabDetailVM> SelectAll(int Id = 0, int Schedule2TaxSlabId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new Schedule2TaxSlabDetailDAL().SelectAll(Id, Schedule2TaxSlabId, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule2TaxSlabDetailVM vm)
        {
            try
            {
                return new Schedule2TaxSlabDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
