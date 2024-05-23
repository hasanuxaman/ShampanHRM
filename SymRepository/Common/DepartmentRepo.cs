using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;

namespace SymRepository.Common
{
   public class DepartmentRepo
    {
       DepartmentDAL _dal = new DepartmentDAL();
       #region Methods
       public List<DepartmentVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       public List<DropDownVM> DropDownByProject(string projectId)
       {
           try
           {
               return _dal.DropDownByProject(projectId);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DepartmentVM> AutocompleteForSalary(string term)
        {
            try
            {
                return _dal.AutocompleteForSalary(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       //==================SelectAll=================
        public List<DepartmentVM> SelectAll()
        {
            try
            {
                return new DepartmentDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public DepartmentVM SelectById(string Id)
        {
            try
            {
                return new DepartmentDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(DepartmentVM vm)
        {
            try
            {
                return new DepartmentDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(DepartmentVM vm)
        {
            try
            {
                return new DepartmentDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public DepartmentVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new DepartmentDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(DepartmentVM vm, string[] ids)
        {
            try
            {
                return new DepartmentDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
