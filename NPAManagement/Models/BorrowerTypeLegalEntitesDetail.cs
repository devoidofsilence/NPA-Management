using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class BorrowerTypeLegalEntitiesDetail
    {
        public int NPAManagementId { get; set; }
        public int BorrowerTypeId { get; set; }
        [Required]
        [Display(Name = "Registration No.")]
        public string LegalEntityRegistrationNumber { get; set; }
        [Required]
        [Display(Name = "Registration Date")]
        public string LegalEntityRegistrationDate { get; set; }
        [Required]
        [Display(Name = "Registered Office")]
        public string LegalEntityRegisteredOffice { get; set; }
    }
}