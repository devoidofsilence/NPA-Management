using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfPersonalGuarantee
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Name of Guarantor")]
        public string NameOfGuarantor { get; set; }
        [Required]
        [Display(Name = "Citizenship Number")]
        public string CitizenshipNumber { get; set; }
        [Required]
        [Display(Name = "Father's Name/Spouse Name")]
        public string FatherName { get; set; }
        [Required]
        [Display(Name = "Grandfather's Name/Father-In-Law's Name")]
        public string GrandfatherName { get; set; }
        [Required]
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        [Required]
        [Display(Name = "Current Address")]
        public string CurrentAddress { get; set; }
        [Required]
        [Display(Name = "Net-worth of Guarantor")]
        public decimal? NetworthOfGuarantor { get; set; }
        [Required]
        [Display(Name = "Relationship with Borrower")]
        public string RelationWithBorrower { get; set; }
        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Profession")]
        public string Profession { get; set; }
    }
}