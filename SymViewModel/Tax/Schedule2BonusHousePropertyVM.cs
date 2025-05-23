﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class Schedule2BonusHousePropertyVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string FiscalYearId { get; set; }
        public int Year { get; set; }
        public int FiscalYearDetailId { get; set; }
        public decimal Line1 { get; set; }
        public decimal Line2 { get; set; }
        public decimal Line3 { get; set; }
        public decimal Line4 { get; set; }
        public decimal Line5 { get; set; }
        public decimal Line6 { get; set; }
        public decimal Line7 { get; set; }
        public decimal Line8 { get; set; }
        public decimal Line9 { get; set; }
        public decimal Line10 { get; set; }
        public decimal Line11 { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string EmpName { get; set; }
        public string Code { get; set; }
        public string Designation  { get; set; }
        public string Department   { get; set; }
        public string Section      { get; set; }
        public string Project      { get; set; }

        public string PeriodName { get; set; }

        public string PeriodStart { get; set; }

        public string PeriodEnd { get; set; }

        public string JoinDate { get; set; }


         public decimal OtherTotal { get; set; }
         public decimal NetIncome { get; set; }

         public List<EmployeeSchedule2TaxSlabDetailVM> employeeSchedule2TaxSlabDetailVMs {get; set;}
         public int Schedule2TaxSlabId { get; set; }


         public decimal TotalTaxNotPayAmount { get; set; }
    }
}
