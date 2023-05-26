using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class FollowUpDetailFor35DaysCallNotice
    {
        public int FollowUpMainId { get; set; }
        public int FollowUpTypeId { get; set; }
        public int FollowUpById { get; set; }
        //public long FollowUpDetailId { get; set; }
        public long NPAManagementId { get; set; }
        [Required]
        [Display(Name = "Date of Notice")]
        public string DateOfNotice { get; set; }
        [Required]
        [Display(Name = "Newspaper name")]
        public string NewspaperName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Response of Borrower")]
        public string ResponseOfBorrower { get; set; }
        [Required]
        [Display(Name = "Next action to be taken")]
        public string NextActionToBeTaken { get; set; }
        [Required]
        [Display(Name = "Due date of next action")]
        public string DueDateOfNextAction { get; set; }
        [Required]
        [Display(Name = "Result")]
        public int? FollowUpResultId { get; set; }
        [Required]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Display(Name = "Scan copy of action taken(*jpg and pdf only, max size 1 MB)")]
        public string ScanCopyOfActionTakenPath { get; set; }
        public string ScanCopyOfActionTakenFileType { get; set; }
        public string ScanCopyOfActionTakenFile { get; set; }
        public bool EnableControls { get; set; }
    }
}