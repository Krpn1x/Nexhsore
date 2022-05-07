using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DynamicTicketingAPI.CustomValidation;

namespace DynamicTicketingAPI.Models
{
    public class MasterBooking
    {
        [Required(ErrorMessage = "CCNCode is required")]
        public string CCNCode { get; set; }
        ///[RequiredIf("AgentID", 100, ErrorMessage = "PayID is required")]
        public string PayID { get; set; }
        [RequiredIf("AgentID", 100, ErrorMessage = "OrderID is required")]
        public string OrderID { get; set; }
        [Required(ErrorMessage = "VisitDate is required")]
        public DateTime VisitDate { get; set; }
        [Required(ErrorMessage = "BookingDate is required")]
        public DateTime BookingDate { get; set; }
        [Required(ErrorMessage = "SpecialDay is required")]
        public long SpecialDay { get; set; }
        [RequiredIf("AgentID", 100, ErrorMessage = "VisitorFName is required")]
        public string VisitorFName { get; set; }
        [RequiredIf("AgentID", 100, ErrorMessage = "VisitorLName is required")]
        public string VisitorLName { get; set; }
        [Required(ErrorMessage = "VisitorPhNo is required")]
        public string VisitorPhNo { get; set; }
        [RequiredIf("AgentID", 100, ErrorMessage = "VisitorEmailId is required")]
        public string VisitorEmailId { get; set; }
        [RequiredIf("AgentID", 100, ErrorMessage = "VisitorCity is required")]
        public string VisitorCity { get; set; }
        [Required(ErrorMessage = "AgentID is required")]
        public long AgentID { get; set; }
        [Required(ErrorMessage = "PaymentMode is required")]
        public string PaymentMode { get; set; }
        [Required(ErrorMessage = "TicketAmount is required")]
        public decimal TicketAmount{ get; set; }
        //[RequiredIf("AgentID", 100, ErrorMessage = "PaidAmountFromPG is required")]
        public decimal PaidAmountFromPG { get; set; }
        public string PaymentStatus { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime ticketdateforSD { get; set; }
        public DateTime ticketDateStamp { get; set; }
        public string ChoiceID { get; set; }
        public string TicketCategory { get; set; }
        public string Status { get; set; }
        public decimal amountchargedtovisitor { get; set; }
        public decimal paymentgatewaycharges { get; set; }
        public List<MasterBookingDetails> MasterBookingDetails { get; set; }

    }
}