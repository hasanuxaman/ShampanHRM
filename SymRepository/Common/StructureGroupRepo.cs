using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class StructureGroupRepo
    {
       StructureGroupDAL _dal = new StructureGroupDAL();
       #region Methods
       public List<StructureGroupVM> DropDown()
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
        public List<StructureGroupVM> SelectAll()
        {
            try
            {
                return new StructureGroupDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public StructureGroupVM SelectById(string Id)
        {
            try
            {
                return new StructureGroupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(StructureGroupVM dVM)
        {
            try
            {
                return new StructureGroupDAL().Insert(dVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(StructureGroupVM vm)
        {
            try
            {
                return new StructureGroupDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public StructureGroupVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new StructureGroupDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(StructureGroupVM vm, string[] ids)
        {
            try
            {
                return new StructureGroupDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
