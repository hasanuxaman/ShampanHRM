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
    public class AttendanceMigrationRepo
    {

        public string[] Insert(AttendanceMigrationVM vm)
        {
            try
            {
                return new AttendanceMigrationDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SelectToInsert(AttendanceMigrationVM vm)
        {
            try
            {
                List<AttendanceMigrationVM> VMs = new List<AttendanceMigrationVM>();
                VMs.Add(vm);
                return new AttendanceMigrationDAL().SelectToInsert(VMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] DeleteProcess(AttendanceMigrationVM vm)
        {
            try
            {
                return new AttendanceMigrationDAL().DeleteProcess(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] SelectFromDownloadData(AttendanceMigrationVM vm)
        {
            try
            {
                return new AttendanceMigrationDAL().SelectFromDownloadDataMultiDate(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Update(AttendanceMigrationVM vm)
        {
            try
            {
                return new AttendanceMigrationDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
