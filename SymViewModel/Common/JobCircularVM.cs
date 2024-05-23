using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class JobCircularVM : BaseEntity
   {
       public string Id { get; set; }
       public int BranchId { get; set; }
       public string JobTitle	 { get; set; }
       public string DesignationId { get; set; }
       [Display(Name = "Designation Name")]
       public string DesignationName { get; set; }
       public string Deadline	 { get; set; }
       public string Expriance	 { get; set; }
       public string Description { get; set; }
    }
}