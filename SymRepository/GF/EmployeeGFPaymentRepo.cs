using SymServices.GF;
using SymServices.PF;
using SymViewModel.GF;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.GF
{
    public class EmployeeGFPaymentRepo
    {

        public List<EmployeeGFPaymentVM> SelectAll(string empid = null)
        {
            try
            {
                return new GFEmployeePaymentDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeGFPaymentVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFEmployeePaymentDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeGFPaymentVM SelectById(string Id, string empId="")
        {
            try
            {
                return new GFEmployeePaymentDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeGFPaymentVM SelectByIdAll(string Id, string empId = "")
        {

            try
            {
                return new GFEmployeePaymentDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeGFPaymentVM vm)
        {
            try
            {
                return new GFEmployeePaymentDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeGFPaymentVM vm)
        {
            try
            {
                return new GFEmployeePaymentDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeeGFPaymentVM vm)
        {
            try
            {
                return new GFEmployeePaymentDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeeGFPaymentVM vm, string[] Ids)
        {
            try
            {
                return new GFEmployeePaymentDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeeGFPaymentVM vm, string Filepath, string FileName)
        {
            return new GFEmployeePaymentDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeeGFPaymentVM vm, string Filepath, string FileName)
        {
            return new GFEmployeePaymentDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new GFEmployeePaymentDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
