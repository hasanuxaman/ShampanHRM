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
    public class TAX108ARepo
    {

        public List<TAX108AVM> SelectAll(string empid = null)
        {
            try
            {
                return new TAX108ADAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TAX108AVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TAX108ADAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TAX108AVM SelectById(string Id, string empId = "")
        {
            try
            {
                return new TAX108ADAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(TAX108AVM vm)
        {
            try
            {
                return new TAX108ADAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(TAX108AVM vm)
        {
            try
            {
                return new TAX108ADAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(TAX108AVM vm)
        {
            try
            {
                return new TAX108ADAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(TAX108AVM vm, string[] Ids)
        {
            try
            {
                return new TAX108ADAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(TAX108AVM vm, string Filepath, string FileName)
        {
            return new TAX108ADAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormGFOpening(TAX108AVM vm, string Filepath, string FileName)
        {
            return new TAX108ADAL().ExportExcelFileFormTAX108A(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new TAX108ADAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
