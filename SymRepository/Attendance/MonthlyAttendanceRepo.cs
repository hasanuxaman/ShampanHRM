using SymServices.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class MonthlyAttendanceRepo
    {
        public List<MonthlyAttendanceVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new MonthlyAttendanceDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(List<MonthlyAttendanceVM> VMs, MonthlyAttendanceVM paramVM)
        {
            try
            {
                return new MonthlyAttendanceDAL().Update(VMs, paramVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SelectToInsert(MonthlyAttendanceVM vm)
        {
            try
            {
                return new MonthlyAttendanceDAL().MonthlyAttendanceProcessRegular(vm);
                //return new MonthlyAttendanceDAL().SelectToInsert(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(MonthlyAttendanceVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new MonthlyAttendanceDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
