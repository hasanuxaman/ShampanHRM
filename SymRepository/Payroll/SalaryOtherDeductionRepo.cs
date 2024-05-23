using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class SalaryOtherDeductionRepo
    {
        SalaryOtherDeductionDAL _dal = new SalaryOtherDeductionDAL();
        #region Methods


        //==================Get All Distinct FiscalPeriodName =================
        public List<SalaryOtherDeductionVM> GetPeriodname()
        {
            try
            {
                return new SalaryOtherDeductionDAL().GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAll=================
        public List<SalaryOtherDeductionVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {
            try
            {
                return new SalaryOtherDeductionDAL().SelectAll(empid, fid, DTId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public SalaryOtherDeductionVM SelectById(string Id)
        {
            try
            {
                return new SalaryOtherDeductionDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SalaryOtherDeductionVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0", string edType = "0")
        {
            try
            {
                return new SalaryOtherDeductionDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(SalaryOtherDeductionVM vm)
        {
            try
            {
                return new SalaryOtherDeductionDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Update =================


        //==================Delete =================
        public string[] Delete(SalaryOtherDeductionVM vm, string[] Ids)
        {
            try
            {
                return new SalaryOtherDeductionDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        //==================SelectAllForReport=================
        public List<SalaryOtherDeductionVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTId = 0,string Orderby=null)
        {
            try
            {
                return new SalaryOtherDeductionDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, DTId, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion
    }
}


