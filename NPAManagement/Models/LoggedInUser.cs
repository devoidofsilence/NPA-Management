using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPAManagement.Models
{
    public class LoggedInUser
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserRole { get; set; }
        public bool UserActiveFlag { get; set; }
    }
}