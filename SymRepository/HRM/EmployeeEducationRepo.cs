using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeeEducationRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeEducationVM> SelectAll()
        {
            try
            {
                return new EmployeeEducationDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<EmployeeEducationVM> SelectAllByEmployeeId(string EmployeeId)
        {
            try
            {
                return new EmployeeEducationDAL().SelectAllByEmployeeId(EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeEducationVM SelectById(int Id)
        {
            try
            {
                return new EmployeeEducationDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeEducationVM empEducationVM)
        {
            try
            {
                return new EmployeeEducationDAL().Insert(empEducationVM,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeEducationVM educationVM)
        {
            try
            {
                return new EmployeeEducationDAL().Update(educationVM,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        //public EmployeeEducationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        //{

        //    try
        //    {
        //        return new EmployeeEducationDAL().Select(query,Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //==================Delete =================
        public string[] Delete(EmployeeEducationVM employeeEducationVM, string[] ids)
        {
            try
            {
                return new EmployeeEducationDAL().Delete(employeeEducationVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeEducationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeEducationDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
