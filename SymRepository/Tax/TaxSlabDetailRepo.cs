using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class TaxSlabDetailRepo
    {
        public List<TaxSlabDetailVM> SelectAll(int Id = 0, int TaxSlabId = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new TaxSlabDetailDAL().SelectAll(Id, TaxSlabId,  conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(TaxSlabDetailVM vm)
        {
            try
            {
                return new TaxSlabDetailDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
