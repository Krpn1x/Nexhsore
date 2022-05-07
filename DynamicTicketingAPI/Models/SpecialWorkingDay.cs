using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicTicketingAPI.Models
{
    public class SpecialWorkingDay
    {
        public long Id { get; set; }
        public DateTime SpecialDay { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}