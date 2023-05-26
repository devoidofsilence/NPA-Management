using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralDetailInCaseOfGold
    {
        public int CollateralDetailMainId { get; set; }
        public int CollateralTypeId { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }
        [Required]
        [Display(Name = "Measurement Unit")]
        public string MeasurementUnitId { get; set; }
        [Required]
        [Display(Name = "Gold Type")]
        public string GoldTypeId { get; set; }
        [Required]
        [Display(Name = "Value of Gold")]
        public decimal? ValueOfGold { get; set; }
        [Required]
        [Display(Name = "Name of Gold Tester")]
        public string NameOfGoldTester { get; set; }
    }
}