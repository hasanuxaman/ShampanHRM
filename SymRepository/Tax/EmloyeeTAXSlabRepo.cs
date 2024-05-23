using SymViewModel.Tax;
using SymServices.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class EmloyeeTAXSlabRepo
    {
        public List<EmloyeeTAXSlabVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, string DojFromDate = "",
            string DojToDate = "")
        {
            try
            {
                return new EmloyeeTAXSlabDAL().SelectAll(conditionFields, conditionValues, null,null,DojFromDate, DojToDate);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] Insert(List<EmloyeeTAXSlabVM> VMs, EmloyeeTAXSlabVM vm)
        {
            try
            {
                return new EmloyeeTAXSlabDAL().Insert(VMs, vm);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
