using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicTicketingAPI.Models
{
    public class ScanSummary
    {
        public long TicketScanCount { get; set; }
        public long CategoryURI { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }
    }
}