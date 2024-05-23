using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeePermanentAddressBanglaRepo
    {
        #region Methods
        //==================SelectAll=================
       public List<EmployeePermanentAddressBanglaVM> SelectAll()
        {
            try
            {
                return new EmployeePermanentAddressBanglaDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
       public EmployeePermanentAddressBanglaVM SelectByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeePermanentAddressBanglaDAL().SelectByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
       public string[] Insert(EmployeePermanentAddressBanglaVM employeePermanentAddressBanglaVM)
        {
            try
            {
                return new EmployeePermanentAddressBanglaDAL().Insert(employeePermanentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
       public string[] Update(EmployeePermanentAddressBanglaVM employeePermanentAddressBanglaVM)
        {
            try
            {
                return new EmployeePermanentAddressBanglaDAL().Update(employeePermanentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
       public EmployeePermanentAddressBanglaVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePermanentAddressBanglaDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
       public string[] Delete(EmployeePermanentAddressBanglaVM employeePermanentAddressBanglaVM)
        {
            try
            {
                return new EmployeePermanentAddressBanglaDAL().Delete(employeePermanentAddressBanglaVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
