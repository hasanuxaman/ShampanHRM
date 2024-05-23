using SymServices.Attendance;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class EarningDeductionStructureRepo
    {
        public List<EarningDeductionStructureVM> SelectAll(int Id = 0)
        {
            try
            {
                return new EarningDeductionStructureDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string[] Insert(EarningDeductionStructureVM vm)
        {
            try
            {
                return new EarningDeductionStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EarningDeductionStructureVM vm)
        {
            try
            {
                return new EarningDeductionStructureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EarningDeductionStructureVM vm, string[] ids)
        {
            try
            {
                return new EarningDeductionStructureDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EarningDeductionStructureVM> DropDown()
        {
            try
            {
                return new EarningDeductionStructureDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
