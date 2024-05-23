using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
    public class EmployeeProfessionalDegreeRepo
    {
        #region
        //==================SelectAll=================
        public List<EmployeeProfessionalDegreeVM> SelectAll()
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<EmployeeProfessionalDegreeVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().SelectAllByEmployeeId(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        //==================SelectByID=================
        public EmployeeProfessionalDegreeVM SelectById(int Id)
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM)
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().Insert(employeeProfessionalDegreeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Update =================
        public string[] Update(EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM)
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().Update(employeeProfessionalDegreeVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeProfessionalDegreeVM employeeProfessionalDegreeVM, string[] ids)
        {
            try
            {
                return new EmployeeProfessionalDegreeDAL().Delete(employeeProfessionalDegreeVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
