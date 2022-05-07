using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicTicketingAPI.Models
{
    public class CategoryAndPricing
    {
        public long CategoryURI { get; set; }
        public string CategoryName { get; set; }
        public Boolean CategoryIsActive { get; set; }
        public string CategoryDescription { get; set; }
        public long SubCategoryURI { get; set; }
        public string SubCategoryName { get; set; }
        public Boolean SubCategoryIsActive { get; set; }
        public string SubCategoryDescription { get; set; }
        public long PricingURI { get; set; }
        public string EffectiveFromDate { get; set; }
        public string EffectiveToDate { get; set; }
        public string PricingType { get; set; }
        public decimal Amount { get; set; }
        public string DataAvailable { get; set; }
    }
}