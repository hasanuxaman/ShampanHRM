using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SymRepository.Leave;
using SymRepository.Attendance;
using SymRepository.Payroll;
using SymOrdinary;
using System.Threading;
using SymRepository.PF;
using SymRepository.Tax;
using SymRepository.GF;
using SymServices.Enum;

namespace SymWebUI.Areas.Config.Controllers
{
    public class DropDownController : Controller
    {

        #region GF

        public JsonResult GFPolicy()
        {
            return Json(new SelectList(new GFPolicyRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        #endregion GF

        #region TAX
        public JsonResult TAX_TaxSlab()
        {
            return Json(new SelectList(new TaxSlabRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        #endregion TAX

        #region PF - Module
        //public JsonResult PF_WithdrawDebitHead(string WithdrawTypeId = "0")
        //{
        //    return Json(new SelectList(new AccountRepo().DropDown_Account_WithdrawDebitHead(WithdrawTypeId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult PF_Account(string AccountType = "")
        //{
        //    return Json(new SelectList(new AccountRepo().DropDown(AccountType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}




        public JsonResult PF_COA(string TransType = "PF")
        {
            return Json(new SelectList(new COARepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_COAGroup(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COATypeOfReport(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().COATypeOfReportDropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_COAGroupType(string TransType = "PF")
        {
            return Json(new SelectList(new COAGroupRepo().COAGroupTypeDropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_JournalTypeDropDown()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult PF_EnumJournalEntryType()

        public JsonResult PF_JournalTransactionTypeDropDown()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTransactionTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult PF_COAType()
        {
            return Json(new SelectList(new COARepo().COATypeDropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_EnumJournalEntryType()
        {
            return Json(new SelectList(new EnumJournalTypeRepo().JournalTransactionTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PF_InvestmentName(string TransType = "PF")
        {
            return Json(new SelectList(new InvestmentNameRepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_TransactionMedia()
        {
            return Json(new SelectList(new TransactionMediaRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_TransactionMediaName()
        {
            return Json(new SelectList(new TransactionMediaRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult PF_WithdrawType()
        //{
        //    return Json(new SelectList(new WithdrawTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult PF_Investment(string InvestmentTypeId)
        //{
        //    return Json(new SelectList(new InvestmentRepo().DropDown(InvestmentTypeId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult PF_EnumInvestmentType()
        {
            return Json(new SelectList(new EnumInvestmentTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_BankBranchName(string TransType = "PF")
        {
            return Json(new SelectList(new BankBranchRepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PF_BankName(string TransType = "PF")
        {
            return Json(new SelectList(new BankNameRepo().DropDown(TransType), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult PF_COA()
        //{
        //    return Json(new SelectList(new SymRepository.PF.COARepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        #endregion PF

        #region HRM - Payroll

        public JsonResult ReportTypeList()
        {
            return Json(new SelectList(new EnumRepo().ReportTypesList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReportTypesList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new MultipleSalaryStructureRepo().SalaryTypeList(), "SalaryTypeId", "SalaryTypeName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BonusStatus()
        {
            return Json(new SelectList(new EnumRepo().BonusStatusList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AccountNature()
        {
            return Json(new SelectList(new EnumRepo().GetAccountNatureList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }



        public JsonResult HoldStatus()
        {
            return Json(new SelectList(new EnumRepo().HoldStatusList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatabaseTable(string InvestmentTypeId)
        {
            return Json(new SelectList(new DatabaseTableRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StructureType()
        {
            return Json(new SelectList(new EnumRepo().GetStructureTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostStatus()
        {
            return Json(new SelectList(new EnumRepo().GetPostStatusList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DropDowDaysOfWeek()
        {
            return Json(new SelectList(new EnumRepo().DaysOfWeekList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Force()
        {
            return Json(new SelectList(new EnumRepo().ForceList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EMPType(string Type = "")
        {
            return Json(new SelectList(new OtherInfoRepo().EMPTypeList(Type), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Retirement()
        {
            return Json(new SelectList(new EnumRepo().RetirementList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmpCategory()
        {
            return Json(new SelectList(new OtherInfoRepo().EmpCategoryList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult NoChildren()
        {
            return Json(new SelectList(new EnumRepo().NoChildrenList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Import()
        {
            return Json(new SelectList(new EnumRepo().ImportList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Feet()
        {
            return Json(new SelectList(new EnumRepo().FeetList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult HeightInch()
        {
            return Json(new SelectList(new EnumRepo().HeightInchList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPreEmpRef()
        {
            return Json(new SelectList(new PreEmployementInformationRepo().DropDownRef(), "Name", "Code"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult OtherInfo(string infoType = "")
        {
            return Json(new SelectList(new OtherInfoRepo().DropDown(infoType), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownReport(string ReportType)
        {
            return Json(new SelectList(new EnumReportRepo().DropDown(ReportType), "ReportId", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ColumnList(string tableName = "")
        {
            return Json(new SelectList(new EnumColumnListRepo().DropDown(tableName), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropDownYear()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownYear(Convert.ToInt32(identity.BranchId)), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownReportType()
        {
            // Define static report types
            List<SelectListItem> staticReportTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "BS", Text = "Balance Sheet" },
                new SelectListItem { Value = "IS", Text = "Income Statement" },
                new SelectListItem { Value = " ", Text = "Trial Balance" }
                // Add more items as needed
            };

            // Create a SelectList from the static list
            SelectList reportTypes = new SelectList(staticReportTypes, "Value", "Text");

            // Return the SelectList as JSON result with support for HTTP GET requests
            return Json(reportTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult VoucherType()
        {
            return Json(new SelectList(new SymRepository.Enum.EnumVoucherTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByYear()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYear(Convert.ToInt32(identity.BranchId), DateTime.Now.Year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriod()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriod(int FiscalYearId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId), FiscalYearId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByFYear(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYear(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByYearLockPayroll(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYearLockPayroll(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodByYearLockPayroll_All(int year)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodByYearLockPayroll_All(Convert.ToInt32(identity.BranchId), year), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownPeriodNext(int currentId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriodNext(Convert.ToInt32(identity.BranchId), currentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FiscalPeriod()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.Common.FiscalYearRepo().DropDownPeriod(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoanType()
        {
            return Json(new SelectList(new EnumLoanTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LetterName()
        {
            return Json(new SelectList(new EnumRepo().GetLetterNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryHeadType()
        {
            return Json(new SelectList(new EnumRepo().GetSalaryHeadTypeList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AbsentDeductFromList()
        {
            return Json(new SelectList(new EnumRepo().AbsentDeductFromList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DaysCountList()
        {
            return Json(new SelectList(new EnumRepo().DaysCountList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttnStatusName()
        {
            return Json(new SelectList(new EnumRepo().GetAttnStatusNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult HoliDayTypeName()
        {
            return Json(new SelectList(new EnumRepo().GetHoliDayTypeNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalarySheetName()
        {
            return Json(new SelectList(new EnumRepo().GetSalarySheetNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoanInterestPolicyList()
        {
            return Json(new SelectList(new EnumRepo().GetLoanInterestPolicyList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryProcessName()
        {
            return Json(new SelectList(new EnumRepo().GetSalaryProcessNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EarningRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetEarningRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeductionRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetDeductionRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PFTaxRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetPFTaxRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GroupList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new GroupRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TaxStructureList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new TaxStructureRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PFStructureList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new PFStructureRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EarningDeductionStructureList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new EarningDeductionStructureRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BonusStructureList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new BonusStructureRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BonusTypeList()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new BonusNameRepo().DropDown(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProjectAllocationList()
        {
            return Json(new SelectList(new ProjectAllocationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryStructureList()
        {
            return Json(new SelectList(new SalaryStructureRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CurrentYearPartList()
        {
            return Json(new SelectList(new SalaryStructureMatrixRepo().CurrentYearPartDropDown(), "CurrentYearPart", "CurrentYearPart"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeCode()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDown(), "Code", "Code"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MultiEmployeeCodeName()
        {
            return Json(new MultiSelectList(new EmployeeInfoRepo().DropDownCodeName(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeCodeName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeCodeNameAll()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameAll(), "Id", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeAll()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameAll(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeCodeNameId()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(), "Id", "EmpName"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult EmployeeIdName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(), "Id", "EmpName"), JsonRequestBehavior.AllowGet);
        }



        public JsonResult EmployeeCodeNameConfirmed()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeNameConfirmed(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExEmployeeCodeName()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownExCodeName(), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeCodeNext(string currentCode)
        {
            string vcurrentCode = "0_0";

            if (currentCode != "0_0" && currentCode != "0" && currentCode != "" && currentCode != "null" && currentCode != null)
                vcurrentCode = currentCode;
            return Json(new SelectList(new EmployeeInfoRepo().DropDown(vcurrentCode), "Code", "EmpName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropDownAllEmployee()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownAllEmployee(), "Id", "EmpInfo"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DropDownEmployeeId()
        {
            return Json(new SelectList(new EmployeeInfoRepo().DropDownCodeName(), "Id", "EmpName"), JsonRequestBehavior.AllowGet);
        }




        public JsonResult SalaryType(string ET)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new EnumSalaryTypeRepo().DropDown(Convert.ToInt32(identity.BranchId), ET), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnumOrderBy(string Module)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new EnumOderByRepo().DropDown(Module), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SalaryPortion()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new EnumSalaryTypeRepo().DropDownSalaryPortion(Convert.ToInt32(identity.BranchId)), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SalaryTypeBasic()
        //{
        //    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    return Json(new SelectList(new EnumSalaryTypeRepo().DropDown(Convert.ToInt32(identity.BranchId), true), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult StructureGroup()
        {
            return Json(new SelectList(new StructureGroupRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeaveStructure()
        {
            return Json(new SelectList(new LeaveStructureRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttendanceStructure()
        {
            return Json(new SelectList(new AttendanceStructureRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Result()
        {
            return Json(new SelectList(new EnumResultRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GradeName()
        {
            return Json(new SelectList(new GradeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Grade()
        {
            return Json(new SelectList(new GradeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DesignationGroup()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Company()
        {
            return Json(new SelectList(new SymRepository.Common.CompanyRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DepartmentName()
        {
            return Json(new SelectList(new DepartmentRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Department()
        {
            return Json(new SelectList(new DepartmentRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Project()
        {
            return Json(new SelectList(new ProjectRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectByDepartment(string departmentId, string sectionId)
        {
            string vDepartmentId = "0_0";
            string vSectionId = "0_0";

            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;


            if (sectionId != "0_0" && sectionId != "0" && sectionId != "" && sectionId != "null" && sectionId != null)
                vSectionId = sectionId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId, vSectionId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProjectByDepartmentId(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DerparmentByProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(new SelectList(new DepartmentRepo().DropDownByProject(projectId), "value", "text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Section()
        {
            return Json(new SelectList(new SectionRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SectionByDepartment(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;

            return Json(new SelectList(new SectionRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesignationName()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Designation()
        {
            return Json(new SelectList(new DesignationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmpLeaveType(string employeeId, int year)
        {
            return Json(new SelectList(new EmployeeLeaveStructureRepo().DropDown(employeeId, year), "LeaveType_E", "LeaveType_E"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmpLeaveType(int year)
        {
            return Json(new SelectList(new EmployeeLeaveStructureRepo().DropDown(year), "LeaveType_E", "LeaveType_E"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult BloodGroup()
        {
            return Json(new SelectList(new EnumBloodGroupRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GLAccount()
        {
            return Json(new SelectList(new SymRepository.Payroll.GLAccountRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Country()
        {
            return Json(new SelectList(new EnumCountryRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CountryEN(bool? isContact = null)
        {
            return Json(new SelectList(new EnumCountryRepo().DropDown(isContact), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Division(string country)
        {
            return Json(new SelectList(new EnumDivisionRepo().DropDown(country), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult District(string division)
        {
            return Json(new SelectList(new EnumDistrictRepo().DropDown(division), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmpJobType(string division)
        {
            return Json(new SelectList(new EnumDistrictRepo().DropDown(division), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CountryA(string term)
        {
            return Json(new EnumCountryRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Degree()
        {
            return Json(new SelectList(new EnumDegreeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProfessionalDegrees()
        {
            return Json(new SelectList(new EnumProfessionalDegreeRepo().DropDown(), "ProfessionalDegrees", "ProfessionalDegrees"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Name()
        {
            return Json(new SelectList(new EnumEMPTypeDAL().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmploymentStatus()
        {
            return Json(new SelectList(new EnumEmploymentStatusRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmploymentType()
        {
            return Json(new SelectList(new EnumEmploymentTypeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Gender()
        {
            return Json(new SelectList(new EnumGenderRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Step()
        {
            return Json(new SelectList(new EnumSalaryStepRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StepSL()
        {
            return Json(new SelectList(new EnumSalaryStepRepo().DropDown(), "SL", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FixedRate()
        {
            return Json(new SelectList(new EnumSalaryPortionRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Language()
        {
            return Json(new SelectList(new EnumLanguageRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeaveApproveStatus()
        {
            return Json(new SelectList(new EnumLeaveApproveStatusRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeaveTypeByEmployee(string EmployeeId)
        {
            return Json(new SelectList(new EnumLeaveTypeRepo().DropDown(EmployeeId), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeaveType()
        {
            return Json(new SelectList(new EnumLeaveTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeftType()
        {
            return Json(new SelectList(new EnumLeftTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MeritalStatus()
        {
            return Json(new SelectList(new EnumMeritalStatusRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Religion()
        {
            return Json(new SelectList(new EnumReligionRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SalaryPayMode()
        {
            return Json(new SelectList(new EnumSalaryPayModeRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Salutation()
        {
            return Json(new SelectList(new EnumSalutationRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TrainingStatus()
        {
            return Json(new SelectList(new EnumTrainingStatusRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult TravelType()
        {
            return Json(new SelectList(new EnumTravelTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Year()
        {
            return Json(new SelectList(new EnumYearRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ImmigrationType()
        {
            return Json(new SelectList(new EnumImmigrationTypeRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        //Skill
        public JsonResult LanguageCompetency()
        {
            return Json(new SelectList(new EnumLanguageCompetencyRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LanguageFluency()
        {
            return Json(new SelectList(new EnumLanguageFluencyRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Nationality()
        {
            return Json(new SelectList(new EnumNationalityRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TrainingPlace()
        {
            return Json(new SelectList(new EnumTrainingPlaceRepo().DropDown(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Bank()
        {
            return Json(new SelectList(new BankRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Asset()
        {
            return Json(new SelectList(new AssetRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult UserGroup()
        {
            return Json(new SelectList(new SymRepository.Common.UserGroupRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ProjectAllocation()
        {
            return Json(new SelectList(new ProjectAllocationRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult ContactPerson()
        //{
        //    return Json(new SelectList(new ContactPersonRepo().DropDownForSelect(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult EarningTypeDropDown()
        {
            return Json(new SelectList(new EarningDeductionTypeRepo().EarningTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeductionTypeDropDown()
        {
            return Json(new SelectList(new EarningDeductionTypeRepo().DeductionTypeDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ModuleDropDown()
        {
            return Json(new SelectList(new SymRepository.Common.SymUserRoleRepo().DropDownModule(), "symArea", "symArea"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MenuDropDown()
        {
            return Json(new SelectList(new SymRepository.Common.SymUserRoleRepo().DropDownMenu(), "symController", "symController"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesignationGroupDropDown()
        {
            return Json(new SelectList(new SymRepository.HRM.DesignationRepo().DesignationGroupDropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult FileDelete(string filepath, int table, string field, string id)
        {
            string tableName = "";
            if (table == 1)
            {
                tableName = "EmployeePersonalDetail";
            }
            else if (table == 2)
            {
                tableName = "EmployeeEmergencyContact";
            }
            else if (table == 3)
            {
                tableName = "EmployeeJobHistory";
            }
            else if (table == 4)
            {
                tableName = "EmployeeLeftInformation";
            }
            else if (table == 5)
            {
                tableName = "EmployeePromotion";
            }
            else if (table == 6)
            {
                tableName = "EmployeeTransfer";

            }
            else if (table == 7)
            {
                tableName = "EmployeeEducation";

            }
            else if (table == 8)
            {
                tableName = "EmployeeExtraCurriculumActivities";

            }
            else if (table == 9)
            {
                tableName = "EmployeeLanguage";

            }
            else if (table == 10)
            {
                tableName = "EmployeeImmigration";

            }
            else if (table == 11)
            {
                tableName = "EmployeeTraining";

            }
            else if (table == 12)
            {
                tableName = "EmployeeTravel";

            }
            else if (table == 13)
            {
                tableName = "EmployeeDependent";

            }
            else if (table == 14)
            {
                tableName = "EmployeeNominee";

            }
            else if (table == 15)
            {
                tableName = "EmployeeFile";

            }
            else if (table == 501)
            {
                tableName = "Clients";
            }
            else if (table == 601)
            {
                tableName = "GLUsers";
            }

            bool returnval = new CommonRepo().FileDelete(tableName, field, id);
            if (returnval)
            {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "" + filepath;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return Json(returnval, JsonRequestBehavior.AllowGet);
        }




        //---------- User Dropdown---------
        public JsonResult User()
        {
            return Json(new SelectList(new UserInformationRepo().DropDown(), "Id", "FullName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser(string deptId)
        {
            return Json(new SelectList(new UserInformationRepo().GetUser(deptId), "Id", "FullName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFeedbackValue(string periodName, string feedBackYear)
        {
            return Json(new SelectList(new Appraisal360UserFeedbackRepo().GetFeedbackValue(periodName, feedBackYear), "Id", "Value"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Question()
        {
            return Json(new SelectList(new AppraisalQuestionsRepo().DropDown(), "Id", "Question"), JsonRequestBehavior.AllowGet);
            //return Json(new SelectList(new DepartmentRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        //====== User By Department ===

        public JsonResult UserByDepartmentId(string departmentId)
        {
            string vDepartmentId = "0_0";
            if (departmentId != "0_0" && departmentId != "0" && departmentId != "" && departmentId != "null" && departmentId != null)
                vDepartmentId = departmentId;
            return Json(new SelectList(new ProjectRepo().DropDownByDepartment(vDepartmentId), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


    }
}
