using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Loan;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
    public class EmployeeStructureGroupRepo
    {
        #region Methods

        //==================SelectAllByEmployee=================
        public EmployeeStructureGroupVM SelectByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeStructureGroupDAL().SelectByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================Insert =================
        public string[] Insert(EmployeeStructureGroupVM employeeJobVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().Insert(employeeJobVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeJobVM employeeJobVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().Update(employeeJobVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeSalaryStructureFromBasic(string EmployeeId, string SalaryStructureId, decimal salaryInput, string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeSalaryStructureFromBasic(EmployeeId, SalaryStructureId, salaryInput, gradeId, stepId,isGross,BankPayAmount, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeSalaryStructureTIB(string EmployeeId, string SalaryStructureId, decimal salaryInput,
            string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM,
            EmployeeStructureGroupVM vm)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeSalaryStructureTIB(EmployeeId, SalaryStructureId, salaryInput, gradeId, stepId, isGross, BankPayAmount, siVM, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeSalaryStructureG4S(string EmployeeId, string SalaryStructureId, decimal salaryInput,
            string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM,
            EmployeeStructureGroupVM vm)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeSalaryStructureG4S(EmployeeId, SalaryStructureId, salaryInput, gradeId, stepId, isGross, BankPayAmount, siVM, vm, null, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeSalaryStructureBollore(string EmployeeId, string SalaryStructureId, decimal salaryInput,
            string gradeId, string stepId, bool isGross, decimal BankPayAmount, ShampanIdentityVM siVM,
            EmployeeStructureGroupVM vm)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeSalaryStructureBollore(EmployeeId, SalaryStructureId, salaryInput, gradeId, stepId, isGross, BankPayAmount, siVM, vm, null, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public string[] EmployeeLeaveStructure(string EmployeeId, string LeaveStructureId, string LeaveYear, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeLeaveStructure(EmployeeId, LeaveStructureId, LeaveYear, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeOtherStructure(EmployeeStructureGroupVM vm, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeOtherStructure( vm, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeePFStructure(string EmployeeId, string PFStructureId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeePFStructure(EmployeeId, PFStructureId,  siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeTaxStructure(string EmployeeId, string TaxStructureId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeTaxStructure(EmployeeId, TaxStructureId,  siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeGroup(string EmployeeId, string EmployeeGroupId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeGroup(EmployeeId, EmployeeGroupId,  siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeTaxPortion(string EmployeeId, decimal TaxPortion, decimal EmpTaxValue, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeTaxPortion(EmployeeId, TaxPortion,EmpTaxValue, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeBonusTaxPortion(string EmployeeId, decimal BonusTaxPortion, decimal EmpBonusTaxValue, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeBonusTaxPortion(EmployeeId, BonusTaxPortion, EmpBonusTaxValue, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EmployeeFixedOT(string EmployeeId, decimal FixedOT, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeFixedOT(EmployeeId, FixedOT, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] EmployeeIsGFApplicable(string EmployeeId, bool IsGFApplicable, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EmployeeIsGFApplicable(EmployeeId, IsGFApplicable, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] BonusStructure(string EmployeeId, string BonusStructureId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().BonusStructure(EmployeeId, BonusStructureId, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ProjectAllocation(string EmployeeId, string BonusStructureId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().ProjectAllocation(EmployeeId, BonusStructureId, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] AttendanceRoster(AttendanceRosterVM vm, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().AttendanceRoster(vm, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EarningDeductionStructure(string EmployeeId, string EarningDeductionStructureId, ShampanIdentityVM siVM)
        {
            try
            {
                return new EmployeeStructureGroupDAL().EarningDeductionStructure(EmployeeId, EarningDeductionStructureId, siVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


       

        #endregion
    }
}
