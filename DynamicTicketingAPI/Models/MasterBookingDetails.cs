using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicTicketingAPI.Models
{
    public class MasterBookingDetails
    {
        public long AssociatedBooking { get; set; }
        public long AssociatedPricing { get; set; }
        public long AssociatedCategory { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public long AssociatedSubCategory { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryDescription { get; set; }
        public long SpecialDay { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}