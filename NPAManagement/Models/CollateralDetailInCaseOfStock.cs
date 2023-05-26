using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfStock
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Share Type")]
        public string ShareTypeId { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Listed in NEPSE")]
        public string ListedInNepseBoolId { get; set; }
        [Required]
        [Display(Name = "Pledged Units")]
        public decimal? PledgedUnits { get; set; }
        [Required]
        [Display(Name = "Unit Type")]
        public string ShareUnitTypeId { get; set; }
        [Required]
        [Display(Name = "Value of Share")]
        public decimal? ValueOfShare { get; set; }
    }
}