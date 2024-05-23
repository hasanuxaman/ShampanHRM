using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
    public class EmployeePermanentAddressRepo
    {
        #region Methods
        //public EmployeePermanentAddressVM SelectPresentAddress(string EmployeeId = "")
        //{
        //    try
        //    {
        //        return new EmployeePermanentAddressDAL().SelectPresentAddress(EmployeeId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //==================SelectAll=================
        public List<EmployeePermanentAddressVM> SelectAll()
        {
            try
            {
                return new EmployeePermanentAddressDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeePermanentAddressVM SelectByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeePermanentAddressDAL().SelectByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePermanentAddressVM employeePermanentAddressVM)
        {
            try
            {
                return new EmployeePermanentAddressDAL().Insert(employeePermanentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePermanentAddressVM employeePermanentAddressVM)
        {
            try
            {
                return new EmployeePermanentAddressDAL().Update(employeePermanentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeePermanentAddressVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePermanentAddressDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePermanentAddressVM employeePermanentAddressVM)
        {
            try
            {
                return new EmployeePermanentAddressDAL().Delete(employeePermanentAddressVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeePermanentAddressVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeePermanentAddressDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
