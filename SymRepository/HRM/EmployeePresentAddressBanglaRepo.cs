using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeePresentAddressBanglaRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeePresentAddressBanglaVM> SelectAll()
        {
            try
            {
                return new EmployeePresentAddressBanglaDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeePresentAddressBanglaVM SelectByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeePresentAddressBanglaDAL().SelectByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM)
        {
            try
            {
                return new EmployeePresentAddressBanglaDAL().Insert(employeePresentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM)
        {
            try
            {
                return new EmployeePresentAddressBanglaDAL().Update(employeePresentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeePresentAddressBanglaVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePresentAddressBanglaDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePresentAddressBanglaVM employeePresentAddressBanglaVM)
        {
            try
            {
                return new EmployeePresentAddressBanglaDAL().Delete(employeePresentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
