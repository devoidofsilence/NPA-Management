using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class BorrowerTypeIndividualDetail
    {
        public int NPAManagementId { get; set; }
        public int BorrowerTypeId { get; set; }
        [Required]
        [Display(Name = "Father's Name")]
        public string BorrowerFatherName { get; set; }
        [Required]
        [Display(Name = "Grandfather's Name")]
        public string BorrowerGrandfatherName { get; set; }
        [Required]
        [Display(Name = "Citizenship No.")]
        public string BorrowerCitizenshipNumber { get; set; }
        [Required]
        [Display(Name = "Spouse Name")]
        public string BorrowerSpouseName { get; set; }
        [Required]
        [Display(Name = "Son's Name")]
        public string BorrowerSonName { get; set; }
        [Required]
        [Display(Name = "Daughter's Name")]
        public string BorrowerDaughterName { get; set; }
    }
}