using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class NPAManagementMain
    {
        [Display(Name = "NPA Mgmt. Id")]
        public int? NPAManagementId { get; set; }

        public string LoggedInUserType { get; set; }
        public bool ViewOnlyUser { get; set; }

        #region Basic Borrower Details
        [Required]
        [Display(Name = "CIF")]
        public string CIFNo { get; set; }
        [Required]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        [Required]
        [Display(Name = "Borrower's Name")]
        public string BorrowerName { get; set; }
        [Required]
        [Display(Name = "Borrower's Contact Number")]
        public string BorrowerContactNumber { get; set; }
        [Required]
        [Display(Name = "Borrower's Email Address")]
        public string BorrowerEmailAddress { get; set; }
        #endregion

        #region Borrower's Address
        [Required(ErrorMessage = "The Permanent Province is required.")]
        [Display(Name = "Province")]
        public string BorrowerPermanentAddressProvinceId { get; set; }
        [Required(ErrorMessage = "The Permanent Zone is required.")]
        [Display(Name = "Zone")]
        public string BorrowerPermanentAddressZoneId { get; set; }
        [Required(ErrorMessage = "The Permanent District is required.")]
        [Display(Name = "District")]
        public string BorrowerPermanentAddressDistrictId { get; set; }
        [Required(ErrorMessage = "The Permanent VDC/Municipality is required.")]
        [Display(Name = "VDC/Municipality")]
        public string BorrowerPermanentAddressVDCMun { get; set; }
        [Required(ErrorMessage = "The Permanent Tole(Street Name) is required.")]
        [Display(Name = "Tole(Street Name)")]
        public string BorrowerPermanentToleStreetName { get; set; }
        [Required]
        [Display(Name = "House Number")]
        public string BorrowerPermanentHouseNumber { get; set; }
        [Display(Name = "Same as permanent address")]
        public bool TempAddSameAsPerAdd { get; set; }
        [Required(ErrorMessage = "The Temporary Province is required.")]
        [Display(Name = "Province")]
        public string BorrowerTemporaryAddressProvinceId { get; set; }
        [Required(ErrorMessage = "The Temporary Zone is required.")]
        [Display(Name = "Zone")]
        public string BorrowerTemporaryAddressZoneId { get; set; }
        [Required(ErrorMessage = "The Temporary District is required.")]
        [Display(Name = "District")]
        public string BorrowerTemporaryAddressDistrictId { get; set; }
        [Required(ErrorMessage = "The Temporary VDC/Municipality is required.")]
        [Display(Name = "VDC/Municipality")]
        public string BorrowerTemporaryAddressVDCMun { get; set; }
        [Required(ErrorMessage = "The Temporary Tole(Street Name) is required.")]
        [Display(Name = "Tole(Street Name)")]
        public string BorrowerTemporaryToleStreetName { get; set; }
        [Required]
        [Display(Name = "House Number")]
        public string BorrowerTemporaryHouseNumber { get; set; }
        #endregion

        #region Borrower's Individual/Legal Details
        [Required]
        [Display(Name = "Borrower Type")]
        public int? BorrowerTypeId { get; set; }
        //Individual
        public string StringifiedBorrowerTypeIndividualLegalDetail { get; set; }
        #endregion

        //#region Individual Loan Account Details
        //public List<LoanAccountDetail> LoanAccountDetailsList { get; set; }
        //#endregion
        [Required]
        [Display(Name = "Current RO Name")]
        public string CurrentROName { get; set; }
        [Required]
        [Display(Name = "Branch Location(Loan Account)")]
        public string BranchLocationCode { get; set; }
        [Required]
        [Display(Name = "SAC RO Name")]
        public string SACROName { get; set; }
        [Required]
        [Display(Name = "File Transfer date to SAC")]
        public string FileTransferDateToSAC { get; set; }
        [Required]
        [Display(Name = "Loan status on the date of transfer")]
        public int? ProvisionStatusIdOnTheDateOfTransfer { get; set; }

        #region Blacklisting Details
        [Required]
        [Display(Name = "Date of request for Blacklisting")]
        public string DateOfRequestForBlacklisting { get; set; }
        [Required]
        [Display(Name = "Status of Request")]
        public string BlacklistStatusOfRequest { get; set; }
        [Required]
        [Display(Name = "Blacklist Date")]
        public string BlacklistDate { get; set; }
        [Required]
        [Display(Name = "Blacklist Number")]
        public string BlacklistNumber { get; set; }
        #endregion

        #region NBA
        [Required]
        [Display(Name = "Date of initiation for booking NBA")]
        public string DateOfInitiationForBookingNBA { get; set; }
        [Required]
        [Display(Name = "Status of Request")]
        public int? NBAStatusOfRequestId { get; set; }
        [Required]
        [Display(Name = "Date of Completion")]
        public string NBADateOfCompletion { get; set; }
        [Required]
        [Display(Name = "NBA Amount")]
        public decimal? NBAAmount { get; set; }
        [Required]
        [Display(Name = "Remarks")]
        public string NBARemarks { get; set; }
        #endregion

        #region DRT
        [Required]
        [Display(Name = "Date of file handed over to Legal for DRT")]
        public string DateOfFileHandedOverToLegalForDRT { get; set; }
        [Required]
        [Display(Name = "Date of filing a case in DRT")]
        public string DateOfFilingACaseInDRT { get; set; }
        [Required]
        [Display(Name = "Status of Request")]
        public int? DRTStatusOfRequestId { get; set; }
        [Required]
        [Display(Name = "Date of Completion")]
        public string DRTDateOfCompletion { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public decimal? DRTAmount { get; set; }
        [Required]
        [Display(Name = "Remarks")]
        public string DRTRemarks { get; set; }
        #endregion

        #region Write Off
        [Required]
        [Display(Name = "Date of initiation for Write off")]
        public string DateOfInitiationForWriteOff { get; set; }
        [Required]
        [Display(Name = "Status of Request")]
        public int? WriteOffStatusOfRequestId { get; set; }
        [Required]
        [Display(Name = "Date of Completion")]
        public string WriteOffDateOfCompletion { get; set; }
        [Required]
        [Display(Name = "Principal outstanding on write off date")]
        public decimal? PrincipalOnWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Interest outstanding on write off date")]
        public decimal? InterestOnWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Principal recovered from start date to loan write off date")]
        public decimal? PrincipalRecoveredFromStartDateToLoanWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Interest recovered from start date to loan write off date")]
        public decimal? InterestRecoveredFromStartDateToLoanWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Interest on Interest recovered from start date to loan write off date")]
        public decimal? InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Penal amount recovered from start date to loan write off date")]
        public decimal? PenalAmountRecoveredFromStartDateToLoanWriteOffDate { get; set; }
        [Required]
        [Display(Name = "Next action to be taken")]
        public string WriteOffNextActionToBeTaken { get; set; }
        [Required]
        [Display(Name = "Due date of next action")]
        public string WriteOffDueDateOfNextAction { get; set; }
        [Required]
        [Display(Name = "Remarks")]
        public string WriteOffRemarks { get; set; }
        #endregion

        public string EnteredBy { get; set; }
        public string EnteredOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }

        #region Stringified objects from dynamic views
        public string StringifiedListOfPrimaryCollateralDetailObjects { get; set; }
        public string StringifiedListOfSecondaryCollateralDetailObjects { get; set; }
        public string StringifiedListOfLoanAccountDetailObjects { get; set; }
        public string StringifiedListOfBranchFollowUpDetailObjects { get; set; }
        public string StringifiedListOfSACFollowUpDetailObjects { get; set; }
        #endregion
    }
}