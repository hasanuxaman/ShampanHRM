using SymServices.Payroll;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class SingleEmployeeRepo
    {
       SingleEmployeeDAL _dAL = new SingleEmployeeDAL();
       public List<SingleEmployeeSalaryStructureVM> SingleEmployeeEntry(string EmployeeId, string FiscalYearDetailId)
       {
           return _dAL.SingleEmployeeEntry( EmployeeId,  FiscalYearDetailId);
       }

       //public string[] Update(EmployeeInfoVM VM)
       //{
       //    try
       //    {
       //        return  _dAL.Update(VM, null, null);
       //    }
       //    catch (Exception ex)
       //    {
       //        throw ex;
       //    }
       //}
        //===

    }
}
