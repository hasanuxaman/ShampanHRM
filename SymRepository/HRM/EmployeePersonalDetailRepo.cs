using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;

namespace SymRepository.HRM
{
    public class EmployeePersonalDetailRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeePersonalDetailVM> SelectAll()
        {
            try
            {
                return new EmployeePersonalDetailDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAllByEmployee=================
        public EmployeePersonalDetailVM SelectByEmployee(string employeeId)
        {
            try
            {
                return new EmployeePersonalDetailDAL().SelectByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeePersonalDetailVM SelectById(int Id)
        {
            try
            {
                return new EmployeePersonalDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePersonalDetailVM employeePersonalDetailVM)
        {
            try
            {
                return new EmployeePersonalDetailDAL().Insert(employeePersonalDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePersonalDetailVM employeePersonalDetailVM)
        {
            try
            {
                return new EmployeePersonalDetailDAL().Update(employeePersonalDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeePersonalDetailVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePersonalDetailDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePersonalDetailVM employeePersonalDetailVM)
        {
            try
            {
                return new EmployeePersonalDetailDAL().Delete(employeePersonalDetailVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
