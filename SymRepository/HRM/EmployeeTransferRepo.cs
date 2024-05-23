using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
    public class EmployeeTransferRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeTransferVM> SelectAll()
        {
            try
            {
                return new EmployeeTransferDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CheckTransferDate(string employeeId, string date)
        {
            try
            {
                return new EmployeeTransferDAL().CheckTransferDate(employeeId, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeTransferVM> SelectAllByEmployee(string EmployeeId)
        {
            try
            {
                return new EmployeeTransferDAL().SelectAllByEmployee(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeTransferVM SelectByEmployeeCurrent(string employeeId)
        {
            try
            {
                return new EmployeeTransferDAL().SelectByEmployeeCurrent(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeTransferVM SelectById(string Id)
        {
            try
            {
                return new EmployeeTransferDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeTransferVM employeeTransferVM)
        {
            try
            {
                return new EmployeeTransferDAL().Insert(employeeTransferVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeTransferVM employeeTransferVM)
        {
            try
            {
                return new EmployeeTransferDAL().Update(employeeTransferVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeeTransferVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeeTransferDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeTransferVM employeeTransferVM, string[] ids)
        {
            try
            {
                return new EmployeeTransferDAL().Delete(employeeTransferVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeTransferVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeTransferDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(EmployeeTransferVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeTransferDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
