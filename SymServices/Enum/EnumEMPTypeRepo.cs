using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Enum
{
   public class EnumEMPTypeRepo
    {
       EnumEMPTypeDAL _emptype = new EnumEMPTypeDAL();
        #region Method
       public List<EnumEMPTypeVM> DropDown()
       {
           try
           {
               return _emptype.DropDown();
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
               return _emptype.Autocomplete(term);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public List<EnumEMPTypeVM> SelectAll()
       {
           try
           {
               return _emptype.SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public EnumEMPTypeVM SelectById(int Id)
       {
           try
           {
               return _emptype.SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public string[] Insert(EnumEMPTypeVM vm)
       {
           try
           {
               return _emptype.Insert(vm, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public string[] Update(EnumEMPTypeVM vm)
       {
           try
           {
               return _emptype.Update(vm, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       public string[] Delete(EnumEMPTypeVM vm, string[] ids)
       {
           try
           {
               return _emptype.Delete(vm, ids, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
        #endregion
    }
}
