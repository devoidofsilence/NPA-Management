using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class CollateralType
    {
        public int CollateralTypeId { get; set; }
        [Display(Name = "Collateral Type")]
        public string CollateralTypeName { get; set; }
        public bool IsSelected { get; set; }
    }
}