using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class FollowUpDetailByTelephone
    {
        public int FollowUpMainId { get; set; }
        public int FollowUpTypeId { get; set; }
        public int FollowUpById { get; set; }
        //public long FollowUpDetailId { get; set; }
        public long NPAManagementId { get; set; }
        [Required]
        [Display(Name = "Contact Date")]
        public string ContactDate { get; set; }
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
        public bool EnableControls { get; set; }
    }
}