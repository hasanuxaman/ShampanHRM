using SymServices.Enum;
using SymServices.Payroll;
using SymViewModel.Enum;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class GLAccountRepo
    {
        GLAccountDAL _enumDAL = new GLAccountDAL();
        #region Method

        public List<GLAccountVM> DropDown()
        {
            try
            {
                return _enumDAL.DropDown();
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
                return _enumDAL.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GLAccountVM> SelectAll(string ProjectId = null)
        {
            try
            {
                return _enumDAL.SelectAll(ProjectId,null,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GLAccountVM SelectById(int Id)
        {
            try
            {
                return _enumDAL.SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GLAccountVM vm)
        {
            try
            {
                return _enumDAL.Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GLAccountVM vm)
        {
            try
            {
                return _enumDAL.Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(GLAccountVM vm, string[] ids)
        {
            try
            {
                return _enumDAL.Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
