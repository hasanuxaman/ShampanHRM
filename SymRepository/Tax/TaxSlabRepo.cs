using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class TaxSlabRepo
    {
        public List<TaxSlabVM> DropDown(string tType = "", int branchId = 0)
        {
            try
            {
                return new TaxSlabDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TaxSlabVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null)
        {
            try
            {
                return new TaxSlabDAL().SelectAll(Id, conditionField, conditionValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(TaxSlabVM vm)
        {
            try
            {
                return new TaxSlabDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(TaxSlabVM vm)
        {
            try
            {
                return new TaxSlabDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(TaxSlabVM vm, string[] ids)
        {
            try
            {
                return new TaxSlabDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
