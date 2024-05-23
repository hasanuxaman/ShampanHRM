using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
  public class EnumForceRepo
    {
      EnumForceDAL _enumDAL = new EnumForceDAL();

      #region Method

      public List<EnumForceVM> DropDown()
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

      //public List<string> Autocomplete(string term)
      //{
      //    try
      //    {
      //        return _enumDAL.Autocomplete(term);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}
      //public List<EnumForceVM> SelectAll()
      //{
      //    try
      //    {
      //        return _enumDAL.SelectAll();
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}
      //public EnumForceVM SelectById(int Id)
      //{
      //    try
      //    {
      //        return _enumDAL.SelectById(Id);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}
      //public string[] Insert(EnumForceVM vm)
      //{
      //    try
      //    {
      //        //return _enumDAL.Insert(vm, null, null);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}
      //public string[] Update(EnumForceVM vm)
      //{
      //    try
      //    {
      //        //return _enumDAL.Update(vm, null, null);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}
      //public string[] Delete(EnumForceVM vm, string[] ids)
      //{
      //    try
      //    {
      //        //return _enumDAL.Delete(vm, ids, null, null);
      //    }
      //    catch (Exception ex)
      //    {
      //        throw ex;
      //    }
      //}

      #endregion
    }
}
