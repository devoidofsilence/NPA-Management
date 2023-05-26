using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class LoanSettlementDetail
    {
        public int LoanSettlementDetailId { get; set; }
        public long NPAmanagementId { get; set; }
        public int FollowUpDetailId { get; set; }
        [Required]
        [Display(Name="Date of Settlement")]
        public string DateOfSettlement { get; set; }
        [Required]
        [Display(Name = "Amount settled")]
        public decimal? AmountSettled { get; set; }
        [Required]
        [Display(Name = "Remaining Amount")]
        public decimal? RemainingAmount { get; set; }
        [Required]
        [Display(Name = "Next action to be taken")]
        public string NextActionToBeTaken { get; set; }
        [Required]
        [Display(Name = "Due date of next action")]
        public string DueDateOfNextAction { get; set; }
    }
}