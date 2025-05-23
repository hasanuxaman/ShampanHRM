﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class EmployeeAssetVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public int AssetId { get; set; }
        [Required]
        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }
        [Display(Name="File Name")]
        public string FileName { get; set; }
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
        public List<string> EmployeeIdList { get; set; }

    }
}
