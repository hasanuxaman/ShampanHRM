using SymServices.Attendance;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class AttendanceStructureRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<AttendanceStructureVM> SelectAll()
        {
            try
            {
                return new AttendanceStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public AttendanceStructureVM SelectById(string Id)
        {
            try
            {
                return new AttendanceStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AttendanceStructureVM SelectByEmployee(string Id,string PunchDate=null)
        {
            try
            {
                return new AttendanceStructureDAL().SelectByEmployee(Id,PunchDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(AttendanceStructureVM vm)
        {
            try
            {
                return new AttendanceStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(AttendanceStructureVM vm)
        {
            try
            {
                return new AttendanceStructureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(AttendanceStructureVM bankVM, string[] ids)
        {
            try
            {
                return new AttendanceStructureDAL().Delete(bankVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttendanceStructureVM> DropDown()
        {
            try
            {
                return new AttendanceStructureDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}
