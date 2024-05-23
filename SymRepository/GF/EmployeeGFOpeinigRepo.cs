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
    public class EmployeeGFOpeinigRepo
    {

        public List<EmployeeGFOpeinigVM> SelectAll(string empid = null)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeGFOpeinigVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeGFOpeinigVM SelectById(string Id, string empId="")
        {
            try
            {
                return new GFEmployeeOpeinigDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeGFOpeinigVM vm)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeGFOpeinigVM vm)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeeGFOpeinigVM vm)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeeGFOpeinigVM vm, string[] Ids)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeeGFOpeinigVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeOpeinigDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormGFOpening(EmployeeGFOpeinigVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeOpeinigDAL().ExportExcelFileFormGFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new GFEmployeeOpeinigDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
