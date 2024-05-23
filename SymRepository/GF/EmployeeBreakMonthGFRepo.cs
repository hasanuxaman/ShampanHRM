using SymServices.GF;
using SymViewModel.GF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.GF
{
    public class EmployeeBreakMonthGFRepo
    {

        public List<EmployeeBreakMonthGFVM> SelectAll(string empid = null)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().SelectAll(empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeBreakMonthGFVM SelectById(string Id, string empId="")
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().SelectById(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeBreakMonthGFVM SelectByIdAll(string Id, string empId = "")
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().SelectByIdAll(Id, empId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeBreakMonthGFVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().SelectAllList(empid, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeBreakMonthGFVM vm)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Update(EmployeeBreakMonthGFVM vm)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(EmployeeBreakMonthGFVM vm)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MultiplePost(EmployeeBreakMonthGFVM vm, string[] Ids)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().MultiplePost(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ExportExcelFileFormEmployee(EmployeeBreakMonthGFVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeBreakMonthDAL().ExportExcelFileFormEmployee(vm, Filepath, FileName);
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeeBreakMonthGFVM vm, string Filepath, string FileName)
        {
            return new GFEmployeeBreakMonthDAL().ExportExcelFileFormPFOpening(vm, Filepath, FileName);
        }

        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new GFEmployeeBreakMonthDAL().ImportExcelFile(fullpath, fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
