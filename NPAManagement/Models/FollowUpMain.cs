using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class FollowUpMain
    {
        public int FollowUpMainId { get; set; }
        public int NPAManagementId { get; set; }
        [Required]
        [Display(Name = "Follow-Up Type")]
        public int FollowUpTypeId { get; set; }
        public int FollowUpById { get; set; }
        public string StringifiedFollowUpDetail { get; set; }
        public string LoggedInUserType { get; set; }
        public bool ViewOnlyUser { get; set; }
        public bool EnableEditingOnThisPanel { get; set; }
    }
}