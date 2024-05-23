using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class EmployeeOverTimeRepo
    {
        EmployeeOverTimeDAL _dal = new EmployeeOverTimeDAL();
        #region Methods
      
        //==================SelectAll=================
        public List<EmployeeOverTimeVM> SelectAll(string empId=null, int? fid = null)
        {
            try
            {
                return new EmployeeOverTimeDAL().SelectAll(empId, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeOverTimeVM SelectById(string Id)
        {
            try
            {
                return new EmployeeOverTimeDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeOverTimeVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0")
        {
            try
            {
                return new EmployeeOverTimeDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeOverTimeVM vm)
        {
            try
            {
                return new EmployeeOverTimeDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeOverTimeVM vm)
        {
            try
            {
                return new EmployeeOverTimeDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeOverTimeVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeOverTimeDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
