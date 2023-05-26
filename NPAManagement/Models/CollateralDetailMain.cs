using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailMain
    {
        public int CollateralDetailMainId { get; set; }
        public int NPAManagementId { get; set; }
        [Required]
        [Display(Name = "Collateral Type")]
        public int CollateralTypeId { get; set; }
        public string CollateralClass { get; set; }
        public string StringifiedCollateralDetail { get; set; }
    }
}