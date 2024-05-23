using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SymRepository.Enum
{
    public class EnumSalaryTypeRepo
    {
        EnumSalaryTypeDAL _salaryTypeDAL = new EnumSalaryTypeDAL();
        #region Method

        //==================SelectAll Salary Type=================
        public List<EnumSalaryTypeVM> SelectAll(int BranchId)
        {
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            try
            {
                salaryTypeVMs = _salaryTypeDAL.SelectAll(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return salaryTypeVMs;

        }
        public List<EnumSalaryTypeVM> SelectAllOthers(int BranchId=0)
        {
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            try
            {
                salaryTypeVMs = _salaryTypeDAL.SelectAllOthers(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return salaryTypeVMs;

        }

        public List<EnumSalaryTypeVM> SelectAllPrinciple(int BranchId=0, string Id=null)
        {
            List<EnumSalaryTypeVM> salaryTypeVMs = new List<EnumSalaryTypeVM>();
            try
            {
                salaryTypeVMs = _salaryTypeDAL.SelectAllPrinciple(BranchId, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return salaryTypeVMs;

        }


        //==================Select Salary Type=================
        public EnumSalaryTypeVM SelectById(string Id)
        {
            return _salaryTypeDAL.SelectById( Id);
        }

        //==================Insert Salary Type=================
        public string[] Insert(EnumSalaryTypeVM salaryTypeVM )
        {
            string[] results = new string[6];
            try
            {
                results = _salaryTypeDAL.Insert(salaryTypeVM, null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return results;
            #endregion


        }

        //==================Update Salary Type=================
        public string[] Update(EnumSalaryTypeVM salaryTypeVM)
        {
            string[] results = new string[6];
            try
            {
                results = _salaryTypeDAL.Update(salaryTypeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        public string[] EditPrinciple(string Id, string GLAccountCode)
        {
            string[] results = new string[6];
            try
            {
                results = _salaryTypeDAL.EditPrinciple( Id,  GLAccountCode, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        //==================Delete =================
        public string[] Delete(EnumSalaryTypeVM vm, string[] ids)
        {
            try
            {
                return _salaryTypeDAL.Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumSalaryTypeVM> DropDown(int BranchId, string ET)
        {
            try
            {
                return _salaryTypeDAL.DropDown(BranchId, ET);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumSalaryTypeVM> DropDownSalaryPortion(int BranchId)
        {
            try
            {
                return _salaryTypeDAL.DropDownSalaryPortion(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
