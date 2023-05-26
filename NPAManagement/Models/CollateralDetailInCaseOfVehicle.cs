using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfVehicle
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Vehicle Type")]
        public int? VehicleTypeId { get; set; }
        [Required]
        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; }
        [Required]
        [Display(Name = "Engine Number")]
        public string EngineNumber { get; set; }
        [Required]
        [Display(Name = "Chassis Number")]
        public string ChassisNumber { get; set; }
        [Required]
        [Display(Name = "Vehicle Registration Number")]
        public string VehicleRegistrationNumber { get; set; }
        [Required]
        [Display(Name = "Vehicle Registration Date")]
        public string VehicleRegistrationDate { get; set; }
        [Required]
        [Display(Name = "Vehicle Make Year")]
        public string VehicleMakeYear { get; set; }
        [Required]
        [Display(Name = "Vehicle Registered Office")]
        public string VehicleRegisteredOffice { get; set; }
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
        public decimal? ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice { get; set; }
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