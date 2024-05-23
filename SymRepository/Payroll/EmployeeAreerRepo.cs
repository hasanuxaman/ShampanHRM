using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class EmployeeAreerRepo
    {
        EmployeeAreerDAL _dal = new EmployeeAreerDAL();
        #region Methods
      
        //==================SelectAll=================
        public List<EmployeeAreerVM> SelectAll(string empid=null, int? fid = null)
        {
            try
            {
                return new EmployeeAreerDAL().SelectAll(empid, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeAreerVM SelectById(string Id)
        {
            try
            {
                return new EmployeeAreerDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeAreerVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0")
        {
            try
            {
                return new EmployeeAreerDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeAreerVM vm)
        {
            try
            {
                return new EmployeeAreerDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeAreerVM vm)
        {
            try
            {
                return new EmployeeAreerDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeAreerVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeAreerDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
