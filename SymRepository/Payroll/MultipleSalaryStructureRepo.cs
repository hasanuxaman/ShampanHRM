using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
  public class MultipleSalaryStructureRepo
    {
        #region Methods

        //==================SelectAll=================
        public List<MultipleSalaryStructureVM> SelectAll(string empid = null, int? fid = null)
        {
            try
            {
                return new MultipleSalaryStructureDAL().SelectAll(empid, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public MultipleSalaryStructureVM SelectById(string Id)
        {
            try
            {
                return new MultipleSalaryStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        //public string[] Insert(MultipleSalaryStructureVM vm)
        //{
        //    try
        //    {
        //        return new MultipleSalaryStructureDAL().Insert(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<MultipleSalaryStructureVM> SalaryTypeList()
        {
            try
            {
                return new MultipleSalaryStructureDAL().SalaryTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
