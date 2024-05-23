using SymServices.PF;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Payroll
{
    public class SalaryPFRepo
    {
        SalaryPFDAL _dal = new SalaryPFDAL();
        #region Methods
        //==================Insert =================
        public string[] SalaryPFSingleAddorUpdate(SalaryPFDetailVM vm, int branchId)
        {
            try
            {
                return _dal.SalaryPFSingleAddorUpdate(vm,branchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryPFVM> GetPeriodNameDistrinct()
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
        public List<SalaryPFVM> SelectAll(int BranchId,int? fid=null)
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
        //public EmployeeBonusVM SelectById(string employeeBonusId)
        //{
        //    try
        //    {
        //        return _dal.SelectById(employeeBonusId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<SalaryPFDetailVM> SelectAllSalaryPFDetails(int FId)
        {
            try
            {
                return _dal.SelectAllSalaryPFDetails(FId, null, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public EmployeeBonusDetailVM SelectByIdBonusDetail(int bonusDetailId)
        //{
        //    try
        //    {
        //        return _dal.SelectByIdBonusDetail(bonusDetailId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public string[] SingleBonusUpdate(EmployeeBonusDetailVM vm)
        //{
        //    try
        //    {
        //        return _dal.SingleBonusUpdate(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public string[] SingleBonusAdd(EmployeeBonusDetailVM vm)
        //{
        //    try
        //    {
        //        return _dal.SingleBonusAdd(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public string[] SalaryPFDetailsDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryPFDetailsDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryPFDelete(string[] Ids)
        {
            try
            {
                return _dal.SalaryPFDelete(Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryPFDetailVM GetByIdSalaryPFDetails(int Id)
        {
            try
            {
                return _dal.GetByIdSalaryPFDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryPFDetailVM GetByIdSalaryPFbyempidandfidDetail(string empid, int fid)
        {
            try
            {
                return _dal.GetByIdSalaryPFbyempidandfidDetail(empid, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] SalaryPFSingleEdit(SalaryPFDetailVM vm)
        {
            try
            {
                return _dal.SalaryPFSingleEdit(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPeriodName(string SalaryPFId)
        {
            try
            {
                return _dal.GetPeriodName(SalaryPFId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportExcelFile(string fileName, ShampanIdentityVM vm, int branchId, int FYDId = 0)
        {
            try
            {
                return new SalaryPFDAL().ImportExcelFile(fileName, vm, branchId, null, null, FYDId);
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
        public List<SalaryPFDetailVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string Orderby)
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

        #endregion
    }
}
