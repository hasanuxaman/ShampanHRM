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
    public class EmployeeGFForfeitureRepo
    {

        public List<EmployeeGFForfeitureVM> SelectAll(string empid = null)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeGFForfeitureVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public EmployeeGFForfeitureVM SelectById(string Id, string empId="")
        {
            try
            {
                return new GFEmployeeForfeitureDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeGFForfeitureVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new GFEmployeeForfeitureDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeGFForfeitureVM vm)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeGFForfeitureVM vm)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeeGFForfeitureVM vm)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeeGFForfeitureVM vm, string[] Ids)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeeGFForfeitureVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeForfeitureDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeeGFForfeitureVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeForfeitureDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new GFEmployeeForfeitureDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
