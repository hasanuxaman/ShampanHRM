using SymServices.Tax;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Tax
{
    public class SalaryTaxRepo
    {
       SalaryTaxDAL _dal = new SalaryTaxDAL();
        #region Methods
        //==================Insert =================
     
        public string[]SalaryTaxSingleAddorUpdate(SalaryTaxDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryTaxSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SalaryTaxSingleAdd(EmployeeInfoVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryTaxSingleAdd(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryTaxVM> GetPeriodNameDistrinct()
        {
            try
            {
                return _dal.GetPeriodNameDistrinct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public List<SalaryTaxVM> SelectAll(int BranchId, int? fid=null)
        {
            try
            {
                return _dal.SelectAll(BranchId,fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryTaxDetailVM> SelectAllSalaryTaxDetails(int fid)
        {
            try
            {
                return _dal.SelectAllSalaryTaxDetails(fid, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[]SalaryTaxDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryTaxDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[]SalaryTaxDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryTaxDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SalaryTaxDetailVM GetByIdSalaryTaxDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryTaxDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         public SalaryTaxDetailVM GetByEmpIdandFdidSalaryTaxDetails(string empid , int fid)
        {
            try
            {
                return _dal.GetByEmpIdandFdidSalaryTaxDetails(empid,fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     
        public string[] SalaryTaxSingleEdit(SalaryTaxDetailVM vm)
        {
            try
            {
                return _dal.SalaryTaxSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPeriodName(string SalaryTaxId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryTaxId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM vm, int branchId, int FYDId = 0)
        {
            try
            {
                return new SalaryTaxDAL().ImportExcelFile(fullPath, fileName, vm, branchId, null, null, FYDId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
          {
            return _dal.ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
          }
          public List<SalaryTaxDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
          {
            try
            {
                return _dal.SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

          public ResultVM DownloadExcel(ParameterVM paramVM)
          {
              try
              {
                  return _dal.DownloadExcel(paramVM);

              }
              catch (Exception ex)
              {
                  throw ex;
              }

          }

        #endregion
    }
}
