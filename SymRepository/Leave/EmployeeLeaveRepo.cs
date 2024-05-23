using SymServices.HRM;
using SymServices.Leave;
using SymViewModel.HRM;
using SymViewModel.Leave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Leave
{
    public class EmployeeLeaveRepo
    {
        //#region Methods
        //==================SelectAll=================
        public List<EmployeeLeaveVM> SelectAll()
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<EmployeeLeaveVM> SelectAllFromSchedule()
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAllFromSchedule();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        //==================SelectAllByEmployee=================
        public List<EmployeeLeaveVM> SelectAllForSupervisor(string SupervisorId)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAllForSupervisor(SupervisorId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAllByEmployee=================
        public List<EmployeeLeaveVM> SelectAll(string code, string status)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAll(code, status);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<EmployeeLeaveVM> SelectAllfromSchedule(string code, string status)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAllfromSchedule(code, status);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        public List<EmployeeLeaveVM> SelectScheduleByEmployeeId(string Id)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectScheduleByEmployeeId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          public List<EmployeeLeaveVM> SelectByEmployeeId(string Id)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectByEmployeeId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeLeaveStructureVM> SelectAllOpening(string empcode, string vyear)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectAllOpening(empcode, vyear);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public DataTable SelectAllOpeningDt(string vyear)
        //{
        //    try
        //    {
        //        return new EmployeeLeaveDAL().SelectAllOpeningDt(vyear);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        public List<EmployeeLeaveBalanceVM> EmployeeLeaveBalance(string employeeId, string leaveyear)
        {
            try
            {
                return new EmployeeLeaveDAL().EmployeeLeaveBalance(employeeId, leaveyear);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public EmployeeLeaveVM SelectById(int Id)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeLeaveVM SelectScheduleById(int Id)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectScheduleById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        //==================SelectByEMPID=================
        public EmployeeLeaveVM SelectByEMPId(string empId)
        {
            try
            {
                return new EmployeeLeaveDAL().SelectByEMPId(empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Insert(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertSchedule(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().InsertSchedule(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string[] Approve(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Approve(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] LeaveSchedule(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().LeaveSchedule(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Update(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateLeaveBalance(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().UpdateLeaveBalance(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().Delete(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] DeleteArchive(EmployeeLeaveVM employeeLeaveVM)
        {
            try
            {
                return new EmployeeLeaveDAL().DeleteArchive(employeeLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ExportExcelFile(string Filepath, string FileName, string vyear)
        {
            return new EmployeeLeaveDAL().ExportExcelFile(Filepath, FileName, vyear);
        }

        public bool ImportExcelFile(string FileName, EmployeeLeaveVM vm)
        {
            return new EmployeeLeaveDAL().ImportExcelFile(FileName, vm);
        }
        //#endregion

        public string[] UpdateLeaveStructureBalance(EmployeeLeaveStructureVM vm)
        {
            try
            {
                return new EmployeeLeaveDAL().UpdateLeaveStructureBalance(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(EmployeeLeaveVM vm)
        {
            try
            {
                return new EmployeeLeaveDAL().Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     

    }
}
