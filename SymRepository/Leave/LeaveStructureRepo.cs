using SymServices.HRM;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Leave
{
   public class LeaveStructureRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<LeaveStructureVM> SelectAll()
        {
            try
            {
                return new LeaveStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<LeaveStructureVM> DropDown()
        {
            try
            {
                return new LeaveStructureDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ////==================SelectAllByEmployee=================
        //public List<LeaveStructureVM> SelectAllByEmployee(string EmployeeId)
        //{
        //    try
        //    {
        //        return new LeaveStructureDAL().SelectAllByEmployee(employeeId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        //==================SelectByID=================
        public LeaveStructureVM SelectById(int Id)
        {
            try
            {
                return new LeaveStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(LeaveStructureVM LeaveStructureVM)
        {
            try
            {
                return new LeaveStructureDAL().Insert(LeaveStructureVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(LeaveStructureVM LeaveStructureVM)
        {
            try
            {
                return new LeaveStructureDAL().Update(LeaveStructureVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public LeaveStructureVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new LeaveStructureDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(LeaveStructureVM LeaveStructureVM, string[] ids)
        {
            try
            {
                return new LeaveStructureDAL().Delete(LeaveStructureVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
