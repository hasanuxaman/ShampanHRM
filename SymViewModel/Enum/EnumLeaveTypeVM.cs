﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Enum
{
   public class EnumLeaveTypeVM
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public string Remarks { get; set; }
       [Display(Name = "Active")]
       public bool IsActive { get; set; }
       [Display(Name = "Without Pay")]
       public bool IsWithoutPay { get; set; }       
       public bool IsArchive { get; set; }
       public string CreatedBy { get; set; }
       public string CreatedAt { get; set; }
       public string CreatedFrom { get; set; }
       public string LastUpdateBy { get; set; }
       public string LastUpdateAt { get; set; }
       public string LastUpdateFrom { get; set; }
    }
}
