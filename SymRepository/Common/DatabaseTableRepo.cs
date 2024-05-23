using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Common
{
    public class DatabaseTableRepo
    {
        
        public List<DatabaseTableVM> DropDown()
        {
            try
            {
                return new DatabaseTableDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable SelectAllData(DatabaseTableVM vm)
        {
            try
            {
                return new DatabaseTableDAL().SelectAllData(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(DatabaseTableVM vm)
        {
            try
            {
                return new DatabaseTableDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
