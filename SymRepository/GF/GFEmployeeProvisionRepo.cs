using SymServices.GF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.GF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymViewModel.PF;


namespace SymRepository.GF
{
    public class GFEmployeeProvisionRepo
    {
        public List<GFEmployeeProvisionVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GFEmployeeProvisionDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GFEmployeeProvisionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFEmployeeProvisionDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Process(GFEmployeeProvisionVM vm)
        {
            try
            {
                return new GFEmployeeProvisionDAL().Process(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new GFEmployeeProvisionDAL().SelectFiscalPeriodHeader(conditionFields,conditionValues);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostHeader(GFHeaderVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new GFEmployeeProvisionDAL().PostHeader(vm);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GFEmployeeProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return  new GFEmployeeProvisionDAL().GFEmployeeProvisionsReport(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {

                return new GFEmployeeProvisionDAL().GFEmployersProvisionsReport(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
