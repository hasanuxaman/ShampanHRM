using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.HRM
{
    public class EmployeeAssetRepo
    {
        public List<EmployeeAssetVM> SelectAll(int Id = 0, string employeeId = null)
        {
            try
            {
                return new EmployeeAssetDAL().SelectAll(Id, employeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] Insert(EmployeeAssetVM vm)
        {
            try
            {
                return new EmployeeAssetDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EmployeeAssetVM vm)
        {
            try
            {
                return new EmployeeAssetDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(EmployeeAssetVM vm, string[] ids)
        {
            try
            {
                return new EmployeeAssetDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            try
            {
                return new EmployeeAssetDAL().Report(CodeF, CodeT, DepartmentId, SectionId, ProjectId, DesignationId, dtpFrom, dtpTo, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable Report(EmployeeAssetVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeAssetDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
