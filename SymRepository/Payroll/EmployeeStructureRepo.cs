using SymServices.Payroll;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using SymViewModel.PF;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class EmployeeStructureRepo
    {
        EmployeeStructureDAL _dal = new EmployeeStructureDAL();
        #region Methods

        public DataTable ExcelData(EmployeeStructureGroupVM vm)
        {
            try
            {
                return _dal.ExcelData(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportExcelFile(EmployeeStructureGroupVM vm)
        {
            try
            {
                return _dal.ImportExcelFile(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<EmployeeSalaryStructureVM> SelectAll(string Id = "", string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeStructureDAL().SelectAll(Id, conditionFields, conditionValues);

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<EmployeeSalaryStructureVM> SelectAllStructure(string Id = "", string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeStructureDAL().SelectAllStructure(Id, conditionFields, conditionValues);

            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<EmployeeSalaryStructureVM> SelectEmployeeSalaryStructureDetailAll(string EmployeeId)
        {
            try
            {
                return _dal.SelectEmployeeSalaryStructureDetailAll(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EmployeeSalaryStructureVM> SelectEmployeeSalaryStructureAll(string Id, string EmployeeSalaryStructureId = "")
        {
            try
            {
                return _dal.SelectEmployeeSalaryStructureAll(Id, EmployeeSalaryStructureId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeSalaryStructureDetailVM SelectEmployeeSalaryStructureDetail(string Id)
        {
            try
            {
                return _dal.SelectEmployeeSalaryStructureDetail(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertDetailNew(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.InsertDetailNew(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] UpdateDetailNew(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.UpdateDetailNew(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteDetailNew(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.DeleteDetailNew(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Use Later
        public string[] InsertDetail(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.InsertDetail(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] UpdateDetail(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.UpdateDetail(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] DeleteDetail(EmployeeSalaryStructureDetailVM vm)
        {
            try
            {
                return _dal.DeleteDetail(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Use Later


        public List<EmployeeLoanVM> SelectEmployeeLoanStructureAll(string Id)
        {
            try
            {
                return _dal.SelectEmployeeLoanStructureAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeePFVM> SelectEmployeePFStructureAll(string Id)
        {
            try
            {
                return _dal.SelectEmployeePFtructureAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeTaxVM> SelectEmployeeTAXtructureAll(string Id)
        {
            try
            {
                return _dal.SelectEmployeeTAXtructureAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeBonusVM> SelectEmployeeBonustructureAll(string Id)
        {
            try
            {
                return _dal.SelectEmployeeBonustructureAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(SalaryStructureVM vm)
        {
            try
            {
                return _dal.Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Methods
    }
}
