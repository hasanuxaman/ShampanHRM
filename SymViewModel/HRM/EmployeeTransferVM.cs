using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SymViewModel.HRM
{
   public class EmployeeTransferVM
   {
       [Required]
       public string Id { get; set; }
       [Required]
       public string EmployeeId { get; set; }
       [Required]
       [Display(Name = "Branch")]
       public int BranchId { get; set; }

       [Required]
       [Display(Name = "Project")]
       public string ProjectId { get; set; }
       [Required]
       [Display(Name = "Department")]
       public string DepartmentId { get; set; }
       [Required]
       [Display(Name = "Section")]
       public string SectionId { get; set; }
       [Required]
       [Display(Name = "Transfer Date")]
       public string TransferDate { get; set; }
       [Required]
       [Display(Name = "Current")]
       public bool IsCurrent { get; set; }
       public string FileName { get; set; }
       public HttpPostedFileBase FileNames { get; set; }
       [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
       public string Remarks { get; set; }

       [Display(Name = "Active")]
       public bool IsActive { get; set; }
       public bool IsArchive { get; set; }
       public string CreatedBy { get; set; }
       public string CreatedAt { get; set; }
       public string CreatedFrom { get; set; }
       public string LastUpdateBy { get; set; }
       public string LastUpdateAt { get; set; }
       public string LastUpdateFrom { get; set; }

       [Display(Name = "Section")]
       public string SectionName { get; set; }
       [Display(Name = "Department")]
       public string DepartmentName { get; set; }
       [Display(Name = "Project")]
       public string ProjectName { get; set; }

       public List<string> EmployeeIdList { get; set; }

       public string Other1 { get; set; }
       public string Other2 { get; set; }
       public string Other3 { get; set; }
       public string Other4 { get; set; }
       public string Other5 { get; set; }
    }
}
