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
    public class EmployeeCompensatoryLeaveRepo
    {
        //#region Methods
        //==================SelectAll=================
        public List<EmployeeCompensatoryLeaveVM> SelectAll()
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAllByEmployee=================
        public List<EmployeeCompensatoryLeaveVM> SelectAllForSupervisor(string SupervisorId)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectAllForSupervisor(SupervisorId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAllByEmployee=================
        public List<EmployeeCompensatoryLeaveVM> SelectAll(string code, string status)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectAll(code, status);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<EmployeeCompensatoryLeaveVM> SelectByEmployeeId(string Id)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectByEmployeeId(Id);
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
        //        return new EmployeeCompensatoryLeaveDAL().SelectAllOpeningDt(vyear);
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
        public EmployeeCompensatoryLeaveVM SelectById(int Id)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByEMPID=================
        public EmployeeCompensatoryLeaveVM SelectByEMPId(string empId)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().SelectByEMPId(empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().Insert(EmployeeCompensatoryLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Approve(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().Approve(EmployeeCompensatoryLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().Update(EmployeeCompensatoryLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        //==================Delete =================
        public string[] Delete(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().Delete(EmployeeCompensatoryLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] DeleteArchive(EmployeeCompensatoryLeaveVM EmployeeCompensatoryLeaveVM)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().DeleteArchive(EmployeeCompensatoryLeaveVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        public DataTable Report(EmployeeCompensatoryLeaveVM vm)
        {
            try
            {
                return new EmployeeCompensatoryLeaveDAL().Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
