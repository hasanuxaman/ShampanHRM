using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class SalaryEmployeeRepo
    {
         public List<SalaryEmployeeVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new SalaryEmployeeDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(List<SalaryEmployeeVM> VMs, SalaryEmployeeVM paramVM)
        {
            try
            {
                return new SalaryEmployeeDAL().Update(VMs, paramVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
