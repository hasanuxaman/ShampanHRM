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
    public class AttendanceDailyRepo
    {
        public List<AttendanceDailyVM> SelectAll()
        {
            try
            {
                return new AttendanceDailyDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttendanceDailyVM SelectById(string Id)
        {
            try
            {
                return new AttendanceDailyDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(AttendanceDailyVM vm)
        {
            try
            {
                return new AttendanceDailyDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Update(AttendanceDailyVM vm)
        {
            try
            {
                return new AttendanceDailyDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(AttendanceDailyVM vm, string[] ids)
        {
            try
            {
                return new AttendanceDailyDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttendanceDailyVM> DropDOwn()
        {
            try
            {
                return new AttendanceDailyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         public List<AttendanceDailyVM> SelectAllForReport(string MulitpleSelection=null,string Filter = null, string AttnStatus = null, string Summary=null, string AttendanceDate=null)
        {
            try
            {
                return new AttendanceDailyDAL().SelectAllForReport(MulitpleSelection, Filter,  AttnStatus , Summary, AttendanceDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
