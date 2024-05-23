using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class BonusProcessRepo
    {
        BonusProcessDAL _dal = new BonusProcessDAL();
        #region Methods

        public List<BonusProcessVM> SelectAll()
        {
            try
            {
                return new BonusProcessDAL().SelectAll();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<BonusProcessVM> SelectBonusDetailAll()
        {
            try
            {
                return new BonusProcessDAL().SelectBonusDetailAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public BonusProcessVM SelectById(string Id)
        {
            try
            {
                return new BonusProcessDAL().SelectAll(Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(BonusProcessVM vm)
        {
            try
            {
                return new BonusProcessDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(BonusProcessVM vm)
        {
            try
            {
                return new BonusProcessDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(BonusProcessVM vm, string[] Ids)
        {
            try
            {
                return new BonusProcessDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] BonusProcess(BonusProcessVM vm)
        {
            try
            {
                return _dal.BonusProcess(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BonusProcessVM> Report(string BonusNameId, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
           , string Orderby,string SheetName="")
        {
            try
            {
                return new BonusProcessDAL().Report(BonusNameId, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, Orderby, SheetName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<BonusProcessVM> BonusReportSummary(string BonusNameId, string ProjectId, string SectionId)
        {
            try
            {
                return new BonusProcessDAL().BonusReportSummary(BonusNameId, ProjectId, SectionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable Report(BonusProcessVM vm)
        {
            try
            {
                return _dal.Report(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExcelReport(BonusProcessVM vm)
        {
            try
            {
                return _dal.ExcelReport(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable ExcelData(BonusProcessVM vm)
        {
            try
            {
                return _dal.ExcelData(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] ImportExcelFile(BonusProcessVM vm)
        {
            try
            {
                return _dal.ImportExcelFile(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ResultVM DownloadExcel(BonusProcessVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                
                return _dal.DownloadExcel(vm, conditionFields, conditionValues);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion
    }
}
