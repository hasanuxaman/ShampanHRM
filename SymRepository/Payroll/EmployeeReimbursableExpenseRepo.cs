using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class EmployeeReimbursableExpenseRepo
    {
        EmployeeReimbursableExpenseDAL _dal = new EmployeeReimbursableExpenseDAL();
        #region Methods
      
        //==================SelectAll=================
        public List<EmployeeReimbursableExpenseVM> SelectAll(string empID=null, int? fid = null)
        {
            try
            {
                return new EmployeeReimbursableExpenseDAL().SelectAll(empID,fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeReimbursableExpenseVM SelectById(string Id)
        {
            try
            {
                return new EmployeeReimbursableExpenseDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeReimbursableExpenseVM vm)
        {
            try
            {
                return new EmployeeReimbursableExpenseDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeReimbursableExpenseVM vm)
        {
            try
            {
                return new EmployeeReimbursableExpenseDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeReimbursableExpenseVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeReimbursableExpenseDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
