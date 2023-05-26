using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfCurrentAsset
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Date of Latest Stock Inspection Report Collected By Branch")]
        public string DateOfLatestStockInspectionReportCollectedByBranch { get; set; }
        [Required]
        [Display(Name = "Working Capital as per Stock Report")]
        public decimal? WorkingCapitalAsPerStockReport { get; set; }
        [Required]
        [Display(Name = "Working Capital as per Stock Inspector")]
        public decimal? WorkingCapitalAsPerStockInspector { get; set; }
        [Required]
        [Display(Name = "Drawing Power")]
        public string DrawingPower { get; set; }
        [Required]
        [Display(Name = "Insurance Coverage Type")]
        public string InsuranceCoverageType { get; set; }
        [Required]
        [Display(Name = "Insurance Expiry Date")]
        public string InsuranceExpiryDate { get; set; }
        [Required]
        [Display(Name = "Insured Amount")]
        public decimal? InsuranceAmount { get; set; }
    }
}