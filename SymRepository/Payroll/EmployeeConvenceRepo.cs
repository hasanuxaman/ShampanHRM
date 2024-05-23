using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class EmployeeConvenceRepo
    {
        EmployeeConvenceDAL _dal = new EmployeeConvenceDAL();
        #region Methods
      
        //==================SelectAll=================
        public List<EmployeeConvenceVM> SelectAll(string empID=null, int? fid = null)
        {
            try
            {
                return new EmployeeConvenceDAL().SelectAll(empID,fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeConvenceVM SelectById(string Id)
        {
            try
            {
                return new EmployeeConvenceDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeConvenceVM vm)
        {
            try
            {
                return new EmployeeConvenceDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeConvenceVM vm)
        {
            try
            {
                return new EmployeeConvenceDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeConvenceVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeConvenceDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
