using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class LeaveTypeRepo
    {
        LeaveTypeDAL _dal = new LeaveTypeDAL();
        #region Methods
        public List<LeaveTypeVM> DropDown()
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

        public List<LeaveTypeVM> AutocompleteForSalary(string term)
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
        public List<LeaveTypeVM> SelectAll()
        {
            try
            {
                return new LeaveTypeDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public LeaveTypeVM SelectById(string Id)
        {
            try
            {
                return new LeaveTypeDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(LeaveTypeVM vm)
        {
            try
            {
                return new LeaveTypeDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(LeaveTypeVM vm)
        {
            try
            {
                return new LeaveTypeDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public LeaveTypeVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new LeaveTypeDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(LeaveTypeVM vm, string[] ids)
        {
            try
            {
                return new LeaveTypeDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
