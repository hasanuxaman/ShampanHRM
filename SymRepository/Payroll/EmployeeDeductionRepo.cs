using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class EmployeeDeductionRepo
    {
        EmployeeDeductionDAL _dal = new EmployeeDeductionDAL();
        #region Methods
      
        //==================SelectAll=================
        public List<EmployeeDeductionVM> SelectAll(string empID=null, int? fid = null)
        {
            try
            {
                return new EmployeeDeductionDAL().SelectAll(empID, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeDeductionVM SelectById(string Id)
        {
            try
            {
                return new EmployeeDeductionDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeDeductionVM vm)
        {
            try
            {
                return new EmployeeDeductionDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeDeductionVM vm)
        {
            try
            {
                return new EmployeeDeductionDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeDeductionVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeDeductionDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
