using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
   public class EnumProfessionalDegreeRepo
    {
       EnumProfessionalDegreeDAL _epdDAL = new EnumProfessionalDegreeDAL();
        #region Method
       public List<EnumProfessionalDegreeVM> DropDown()
       {
           try
           {
               return _epdDAL.DropDown();
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
               return _epdDAL.Autocomplete(term);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public List<EnumProfessionalDegreeVM> SelectAll()
       {
           try
           {
               return _epdDAL.SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public EnumProfessionalDegreeVM SelectById(int Id)
       {
           try
           {
               return _epdDAL.SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public string[] Insert(EnumProfessionalDegreeVM vm)
       {
           try
           {
               return _epdDAL.Insert(vm, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public string[] Update(EnumProfessionalDegreeVM vm)
       {
           try
           {
               return _epdDAL.Update(vm, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public string[] Delete(EnumProfessionalDegreeVM vm, string[] ids)
       {
           try
           {
               return _epdDAL.Delete(vm, ids, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

        #endregion
    }
}
