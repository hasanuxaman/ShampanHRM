using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.HRM
{
    public class EmployeePromotionRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<EmployeePromotionVM> SelectAll()
        {
            try
            {
                return new EmployeePromotionDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CheckPromotionDate(string employeeId, string date)
        {
            try
            {
                return new EmployeePromotionDAL().CheckPromotionDate(employeeId, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAllByEmployee=================
        public List<EmployeePromotionVM> SelectAllByEmployee(string employeeId)
        {
            try
            {
                return new EmployeePromotionDAL().SelectAllByEmployee(employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeePromotionVM SelectById(int Id)
        {
            try
            {
                return new EmployeePromotionDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByEmployee=================
        public EmployeePromotionVM SelectByEmployeeCurrent(string Id)
        {
            try
            {
                return new EmployeePromotionDAL().SelectByEmployeeCurrent(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeePromotionVM employeePromosionVM)
        {
            try
            {
                return new EmployeePromotionDAL().Insert(employeePromosionVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeePromotionVM employeePromosionVM)
        {
            try
            {
                return new EmployeePromotionDAL().Update(employeePromosionVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public EmployeePromotionVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new EmployeePromotionDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeePromotionVM employeePromosionVM, string[] ids)
        {
            try
            {
                return new EmployeePromotionDAL().Delete(employeePromosionVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAllForReport=================
        public List<EmployeePromotionVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeePromotionDAL().SelectAllForReport(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report(EmployeePromotionVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeePromotionDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
