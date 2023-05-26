using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class LoanAccountDetail
    {
        public int LoanMainId { get; set; }
        public int NPAManagementId { get; set; }
        [Required]
        [Display(Name = "Loan Account Number")]
        public string LoanAccountNo { get; set; }
        [Required]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        [Required]
        [Display(Name = "Loan Type")]
        public int? LoanTypeId { get; set; }
        [Required]
        [Display(Name = "Loan Nature")]
        public int? LoanNatureId { get; set; }
        [Required]
        [Display(Name = "First Limit Initiated By")]
        public string FirstLimitInitiatedBy { get; set; }
        [Required]
        [Display(Name = "First Limit Recommended By(Final Recommender)")]
        public string FirstRecommendedBy { get; set; }
        [Required]
        [Display(Name = "First Limit Approved By(Final Approver)")]
        public string FirstApprovedBy { get; set; }
        [Required]
        [Display(Name = "Department Name(if applicable)")]
        public int? DepartmentId { get; set; }
        [Required]
        [Display(Name = "Province")]
        public string BranchProvince { get; set; }
        [Required]
        [Display(Name = "Reporting SBO")]
        public string ReportingSBO { get; set; }
        [Required]
        [Display(Name = "Nominee Account Number")]
        public string NomineeAccountNo { get; set; }
        [Required]
        [Display(Name = "First Approved Limit")]
        public decimal? InitialApprovedLimit { get; set; }
        [Required]
        [Display(Name = "First Limit Placement Date")]
        public string FirstLimitPlacementDate { get; set; }
        [Required]
        [Display(Name = "Latest Approved Limit")]
        public decimal? LatestApprovedLimit { get; set; }
        [Required]
        [Display(Name = "Limit Expiry Date")]
        public string LimitExpiryDate { get; set; }
        [Required]
        [Display(Name = "Current Status of Loan")]
        public int? LoanCurrentStatusId { get; set; }
        [Required]
        [Display(Name = "Settlement Date")]
        public string SettlementDate { get; set; }

        //#region Overdue when loan is booked under Doubtful loan
        //[Display(Name = "Overdue Principal")]
        //public string OverduePrincipalAsOnDate { get; set; }
        //[Display(Name = "Overdue Interest")]
        //public string OverdueInterestAsOnDate { get; set; }
        //[Display(Name = "Ad-hoc Charges")]
        //public string AdhocCharges { get; set; }
        //#endregion

        //#region Penal when loan is booked under Doubtful loan
        //[Display(Name = "Penal Interest on Interest")]
        //public string PenalInterestOnInterestAsOnDate { get; set; }
        //[Display(Name = "Penal Charges")]
        //public string PenalChargesAsOnDate { get; set; }
        //#endregion

        //[Display(Name = "Total Overdue amount")]
        //public string TotalOverdueWhileTransferToRecovery { get; set; }

        #region Outstanding Balance
        [Required]
        [Display(Name = "Loan Status")]
        public int? LoanStatusId { get; set; }
        [Required]
        [Display(Name = "Date")]
        public string OutStDate { get; set; }
        [Required]
        [Display(Name = "Principal")]
        public decimal? OutStPrincipal { get; set; }
        [Required]
        [Display(Name = "Interest")]
        public decimal? OutStInterest { get; set; }
        [Required]
        [Display(Name = "Adhoc Charges")]
        public decimal? OutStAdhocCharges { get; set; }
        [Required]
        [Display(Name = "Interest on Interest")]
        public decimal? OutStInterestOnInterest { get; set; }
        [Required]
        [Display(Name = "Penal Charges")]
        public decimal? OutStPenalCharges { get; set; }
        [Required]
        [Display(Name = "Total Amount")]
        public decimal? OutStTotalAmount { get; set; }
        #endregion

        #region Accrued Amount
        [Required]
        [Display(Name = "From Date")]
        public string AccruedAmtFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public string AccruedAmtToDate { get; set; }
        [Required]
        [Display(Name = "Principal")]
        public decimal? AccruedAmtPrincipal { get; set; }
        [Required]
        [Display(Name = "Interest")]
        public decimal? AccruedAmtInterest { get; set; }
        [Required]
        [Display(Name = "Adhoc Charges")]
        public decimal? AccruedAmtAdhocCharges { get; set; }
        [Required]
        [Display(Name = "Interest on Interest")]
        public decimal? AccruedAmtInterestOnInterest { get; set; }
        [Required]
        [Display(Name = "Penal Charges")]
        public decimal? AccruedAmtPenalCharges { get; set; }
        [Required]
        [Display(Name = "Total Amount")]
        public decimal? AccruedAmtTotalAmount { get; set; }
        #endregion

        #region Recovered Amount
        [Required]
        [Display(Name = "From Date")]
        public string RecoveredAmtFromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public string RecoveredAmtToDate { get; set; }
        [Required]
        [Display(Name = "Principal")]
        public decimal? RecoveredAmtPrincipal { get; set; }
        [Required]
        [Display(Name = "Interest")]
        public decimal? RecoveredAmtInterest { get; set; }
        [Required]
        [Display(Name = "Adhoc Charges")]
        public decimal? RecoveredAmtAdhocCharges { get; set; }
        [Required]
        [Display(Name = "Interest on Interest")]
        public decimal? RecoveredAmtInterestOnInterest { get; set; }
        [Required]
        [Display(Name = "Penal Charges")]
        public decimal? RecoveredAmtPenalCharges { get; set; }
        [Required]
        [Display(Name = "Total Amount")]
        public decimal? RecoveredAmtTotalAmount { get; set; }
        #endregion

        //#region Overdue as on date
        //[Display(Name = "Overdue Principal")]
        //public string OverduePrincipalAsOnDateWhenTransferToSAC { get; set; }
        //[Display(Name = "Overdue Interest")]
        //public string OverdueInterestAsOnDateWhenTransferToSAC { get; set; }
        //#endregion

        //#region Penal as on date
        //[Display(Name = "Penal Interest on Interest")]
        //public string PenalInterestOnInterestAsOnDateWhenTransferToSAC { get; set; }
        //[Display(Name = "Penal Charges")]
        //public string PenalChargesAsOnDateWhenTransferToSAC { get; set; }
        //#endregion

        //[Display(Name = "Total Overdue amount")]
        //public string TotalOverdueWhileTransferToRecoveryWhenTransferToSAC { get; set; }
        [Required]
        [Display(Name = "Provision %")]
        public int? ProvisionPercentageId { get; set; }
        [Required]
        [Display(Name = "Provision Date")]
        public string ProvisionDate { get; set; }
        [Required]
        [Display(Name = "Provision Amount")]
        public decimal? ProvisionAmount { get; set; }

        //#region Security Details & Valuation
        //public List<CollateralType> PrimaryCollateralTypes { get; set; }
        //public List<CollateralType> SecondaryCollateralTypes { get; set; }
        //public List<CollateralDetailInCaseOfLand> CollateralDetailInCaseOfLandList { get; set; }
        //public List<CollateralDetailInCaseOfBuilding> CollateralDetailInCaseOfBuildingList { get; set; }
        //public List<CollateralDetailInCaseOfVehicle> CollateralDetailInCaseOfVehicleList { get; set; }
        //public List<CollateralDetailInCaseOfPlantAndMachinery> CollateralDetailInCaseOfPlantAndMachineryList { get; set; }
        //public List<CollateralDetailInCaseOfCurrentAsset> CollateralDetailInCaseOfCurrentAssetList { get; set; }
        //public List<CollateralDetailInCaseOfPersonalGuarantee> CollateralDetailInCaseOfPersonalGuaranteeList { get; set; }
        //#endregion
    }
}