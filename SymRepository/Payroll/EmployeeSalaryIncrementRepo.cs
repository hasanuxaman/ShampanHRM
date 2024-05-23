using SymServices.Payroll;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
  public  class EmployeeSalaryIncrementRepo
    {
      EmployeeSalaryIncrementDAL _dal = new EmployeeSalaryIncrementDAL();
      #region Methods
      //==================Insert =================
      public string[] InsertIncreament(List<string> empids,int BranchId, string IncreamentDate, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string GB, string FR, decimal Amount, string LastUpdateAt, string LastUpdateBy, string LastUpdateFrom)
      {
          try
          {
              return _dal.InsertIncreament( empids, BranchId,IncreamentDate, ProjectId, DepartmentId, SectionId, DesignationId,CodeF,CodeT,GB,FR, Amount, LastUpdateAt, LastUpdateBy, LastUpdateFrom,null,null);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }
        ////==================Update =================

        //public string[] Update(BonusProcessVM vm)
        //{
        //    try
        //    {
        //        return new BonusProcessDAL().Update(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////==================Delete =================
        //public string[] Delete(BonusProcessVM vm, string[] Ids)
        //{
        //    try
        //    {
        //        return new BonusProcessDAL().Delete(vm, Ids, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public string[] BonusProcess(BonusProcessVM vm)
        //{
        //    try
        //    {
        //        return _dal.BonusProcess(vm, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public List<BonusProcessVM> SelectAllForReport(string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
        //   , string Orderby)
        //{
        //    try
        //    {
        //        return new BonusProcessDAL().SelectAllForReport(BonusNameId, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, Orderby);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

      public DataTable EmployeeIncrement(EmployeeInfoVM vm)
      {
          try
          {
              return _dal.EmployeeIncrement(vm);
          }
          catch (Exception ex)
          {
              throw ex;
          }
      }
        #endregion
      
    }
}
