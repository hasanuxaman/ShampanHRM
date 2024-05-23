using SymServices.Common;
using SymServices.Tax;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class EmployeeTaxRepo
    {
        EmployeeTaxDAL _dal = new EmployeeTaxDAL();
        #region Methods
        public List<EmployeeTaxVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeTaxVM> SelectAll()
        {
            try
            {
                return new EmployeeTaxDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeTaxVM SelectById(string Id)
        {
            try
            {
                return new EmployeeTaxDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(EmployeeTaxVM vm)
        {
            try
            {
                return new EmployeeTaxDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeTaxVM vm)
        {
            try
            {
                return new EmployeeTaxDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeTaxVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeTaxDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm)
        {
            try
            {
                return _dal.ImportExcelFile(fullPath, fileName, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(EmployeeTaxVM vm, string[] conFields = null, string[] conValues = null)
        {
            try
            {
                return _dal.ExportExcelFile(vm, conFields, conValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        


        #endregion
    }
}
