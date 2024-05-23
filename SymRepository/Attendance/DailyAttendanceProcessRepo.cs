using SymServices.Attendance;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class DailyAttendanceProcessRepo
    {
        public List<AttendanceDailyNewVM> SelectAll(int Id = 0, AttendanceDailyNewVM vm = null, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return new DailyAttendanceProcessDAL().SelectAll(Id, vm, conFields, conValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
               return new DailyAttendanceProcessDAL().Report(vm, conFields, conValues);            
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable ReportAttendanceSummery(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return new DailyAttendanceProcessDAL().ReportAttendanceSummery(vm, conFields, conValues);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public string[] Update(List<AttendanceDailyNewVM> VMs){
        //    try
        //    {
        //        return new DailyAttendanceProcessDAL().Update(VMs);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public string[] UpdateAttendanceMigration(List<AttendanceDailyNewVM> VMs, AttendanceDailyNewVM vm)
        {
            try
            {
                return new DailyAttendanceProcessDAL().UpdateAttendanceMigration(VMs, vm);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public string[] Process1(string fid, ShampanIdentityVM auditvm, string EmployeeId = "0_0")
        {
            try
            {
                return new DailyAttendanceProcessDAL().Process1(fid, auditvm, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Process2(string fid, ShampanIdentityVM auditvm,  string EmployeeId = "0_0")
        {
            try
            {
                return new DailyAttendanceProcessDAL().Process2(fid, auditvm, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Process3(string fid, ShampanIdentityVM auditvm, string EmployeeId = "0_0")
        {
            {
                try
                {
                    return new DailyAttendanceProcessDAL().Process3(fid, auditvm, EmployeeId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public string[] CalendarProcess(string fid)
        {
            try
            {
                return new DailyAttendanceProcessDAL().CalendarProcess(fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MonthlyAttendanceDownload(EmployeeInfoVM vm)
        {
            try
            {
                return new DailyAttendanceProcessDAL().MonthlyAttendanceDownload(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
