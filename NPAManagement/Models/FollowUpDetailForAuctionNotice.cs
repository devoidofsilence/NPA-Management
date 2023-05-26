using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class FollowUpDetailForAuctionNotice
    {
        public int FollowUpMainId { get; set; }
        public int FollowUpTypeId { get; set; }
        public int FollowUpById { get; set; }
        //public long FollowUpDetailId { get; set; }
        public long NPAManagementId { get; set; }
        #region 15 Days Auction Notice
        [Display(Name = "Auction Notice Type")]
        public int? AuctionNoticeTypeId { get; set; }
        [Required]
        [Display(Name = "Date of Notice")]
        public string DateOfAuctionNotice { get; set; }
        [Required]
        [Display(Name = "Newspaper name")]
        public string AuctionNewspaperName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Response of Borrower")]
        public string AuctionResponseOfBorrower { get; set; }
        [Required]
        [Display(Name = "Next action to be taken")]
        public string AuctionNextActionToBeTaken { get; set; }
        [Required]
        [Display(Name = "Due date of next action")]
        public string AuctionDueDateOfNextAction { get; set; }
        [Required]
        [Display(Name = "Remarks")]
        public string AuctionRemarks { get; set; }
        [Required]
        [Display(Name = "Date of Revaluation")]
        public string DateOfRevaluationForAuction { get; set; }
        [Required]
        [Display(Name = "FMV of Collateral")]
        public decimal? RevaluatedFMVOfCollateralForAuction { get; set; }
        [Display(Name = "Scan copy of action taken(*jpg and pdf only, max size 1 MB)")]
        public string ScanCopyOfActionTakenPath { get; set; }
        public string ScanCopyOfActionTakenFileType { get; set; }
        public string ScanCopyOfActionTakenFile { get; set; }
        public bool EnableControls { get; set; }
        #endregion
    }
}