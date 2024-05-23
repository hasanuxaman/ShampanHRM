using SymServices.Attendance;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymAPIRepository
{
    public class AlltendanceRepo
    {
        public string[] InsertManual(DownloadDataVM vm)
        {
            try
            {
                return new DownloadDataDAL().InsertManual(vm, null, null);
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
        public DataTable AttendanceBySupervisor(EmployeeInfoVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return new DailyAttendanceProcessDAL().ReportBySupervisor(vm, conFields, conValues);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable LateReason()
        {
            try
            {
                return new DailyAttendanceProcessDAL().LateReason();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
