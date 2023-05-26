using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfCorporateGuarantee
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        //
        [Required]
        [Display(Name = "Name of Firm")]
        public string NameOfFirm { get; set; }
        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        [Required]
        [Display(Name = "Firm's Address")]
        public string FirmAddress { get; set; }
        [Required]
        [Display(Name = "Net-worth of the firm(As per latest audit report)")]
        public decimal? NetworthOfTheFirm { get; set; }
        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
    }
}