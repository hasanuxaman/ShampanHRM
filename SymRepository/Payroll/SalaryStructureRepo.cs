using SymServices.Enum;
using SymServices.HRM;
using SymServices.Payroll;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;

namespace SymRepository.Payroll
{
    public class SalaryStructureRepo
    {
        SalaryStructureDAL _salaryStructureDAL = new SalaryStructureDAL();
        #region Method

        //==================SelectAll  Salary Structure=================
        public List<SalaryStructureVM> SelectAll()
        {
            List<SalaryStructureVM> salaryStructureVMs = new List<SalaryStructureVM>();
            try
            {
                salaryStructureVMs = _salaryStructureDAL.SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return salaryStructureVMs;

        }

        //==================Select Salary Structure=================
        public SalaryStructureVM SelectById(string Id)
        {
            return _salaryStructureDAL.SelectById( Id);
        }

        //==================Insert Salary Structure=================
        public string[] Insert(SalaryStructureVM vm)
        {
            string[] results = new string[6];
            try
            {
                results = _salaryStructureDAL.Insert(vm, null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return results;
            #endregion


        }

        //==================Update Salary Structure=================
        public string[] Update(SalaryStructureVM vm)
        {
            string[] results = new string[6];
            try
            {
                results = _salaryStructureDAL.Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }

        //==================Delete =================
        public string[] Delete(SalaryStructureVM vm, string[] ids)
        {
            try
            {
                return new SalaryStructureDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public List<SalaryStructureVM> DropDown()
        {
            try
            {
                return _salaryStructureDAL.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
