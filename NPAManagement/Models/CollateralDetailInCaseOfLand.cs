using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfLand
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Province")]
        public string ProvinceId { get; set; }
        [Required]
        [Display(Name = "Zone")]
        public string ZoneId { get; set; }
        [Required]
        [Display(Name = "District")]
        public string DistrictId { get; set; }
        [Required]
        [Display(Name = "VDC/Municipality")]
        public string VDCMun { get; set; }
        //public string VDCMunId { get; set; }
        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }
        [Required]
        [Display(Name = "Ward No.")]
        public string WardNumber { get; set; }
        [Required]
        [Display(Name = "Plot No.")]
        public string PlotNumber { get; set; }
        [Required]
        [Display(Name = "Area Type")]
        public int? AreaTypeId { get; set; }
        [Required]
        [Display(Name = "Area")]
        public string Area { get; set; }
        [Required]
        [Display(Name = "Property Owner Name")]
        public string PropertyOwner { get; set; }
        [Required]
        [Display(Name = "Relationship with Borrower")]
        public string RelationWithBorrower { get; set; }
        [Required]
        [Display(Name = "First Valuator's Name")]
        public string FirstValuatorName { get; set; }
        [Required]
        [Display(Name = "First Valuation Date")]
        public string FirstValuationDate { get; set; }
        [Required]
        [Display(Name = "FMV of Property as per first valuation report")]
        public decimal? FirstFMVOfProperty { get; set; }
        [Required]
        [Display(Name = "First Valuator Exists in SBL Currently")]
        public string FirstValuatorExistsInSBLCurrentlyYN { get; set; }
        [Required]
        [Display(Name = "Latest Valuator's Name")]
        public string LatestValuatorName { get; set; }
        [Required]
        [Display(Name = "Latest Valuation Date")]
        public string LatestValuationDate { get; set; }
        [Required]
        [Display(Name = "Latest Valuator Exists in SBL Currently")]
        public string LatestValuatorExistsInSBLCurrentlyYN { get; set; }
        [Required]
        [Display(Name = "FMV of Property as per latest valuation report")]
        public decimal? FMVOfPropertyAsPerLatestValuationReport { get; set; }
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