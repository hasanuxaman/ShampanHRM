using SymServices.Payroll;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class EmployeeSalaryOtherIncreamentRepo
    {

        public string[] InsertEmployeeSalaryStructure(List<EmployeeSalaryStructureVM> VMs, EmployeeSalaryStructureVM vm)
        {
            try
            {
                return new EmployeeSalaryOtherIncreamentDAL().InsertEmployeeSalaryStructure(VMs, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

public string[] InsertEmployeeSalaryIncrementMatrix(EmployeeSalaryStructureVM vm)
        {
            try
            {
                return new EmployeeSalaryOtherIncreamentDAL().InsertEmployeeSalaryIncrementMatrix( vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert_Increament(EmployeeSalaryStructureVM vm)
        {
            try
            {
                return new EmployeeSalaryOtherIncreamentDAL().Insert_Increament(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SelectAll_For_Increment(EmployeeInfoVM vm)
        {
            try
            {
                return new EmployeeSalaryOtherIncreamentDAL().SelectAll_For_Increment(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
