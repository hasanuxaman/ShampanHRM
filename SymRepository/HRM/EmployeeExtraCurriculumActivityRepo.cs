using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeeExtraCurriculumActivityRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeeExtraCurriculumActivityVM> SelectAll()
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeExtraCurriculumActivityVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeExtraCurriculumActivityVM SelectById(int Id)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().Insert(employeeExtraCurriculumActivityVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().Update(employeeExtraCurriculumActivityVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        //public EmployeeExtraCurriculumActivityVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        //{

        //    try
        //    {
        //        return new EmployeeExtraCurriculumActivityDAL().Select(query, Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //==================Delete =================
        public string[] Delete(EmployeeExtraCurriculumActivityVM employeeExtraCurriculumActivityVM, string[] ids)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().Delete(employeeExtraCurriculumActivityVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeeExtraCurriculumActivityVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeExtraCurriculumActivityDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
