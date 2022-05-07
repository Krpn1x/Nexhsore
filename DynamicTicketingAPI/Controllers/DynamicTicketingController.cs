using DynamicTicketingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;
using Newtonsoft.Json;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace DynamicTicketingAPI.Controllers
{
    public class DynamicTicketingController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/CategoryAndPricing/")]
        public IEnumerable<CategoryAndPricing> GetCategoryAndPricing([FromUri] string VisitDate, [FromUri] string Company, [FromUri] string Branch)
        {
            List<CategoryAndPricing> objPricingList = new List<CategoryAndPricing>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {

                DateTime dateValue;//mm/dd/yyyy
                dateValue = DateTime.Parse(VisitDate, CultureInfo.InvariantCulture);
                string DayofWeek = dateValue.ToString("dddd");
                DataSet dsPricing = new DataSet();
                DataSet dsAPIRequestLog = new DataSet();
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@VisitDate", Value = VisitDate });
                    objParamList.Add(new CommandParameter { Name = "@DayofWeek", Value = DayofWeek });
                    objParamList.Add(new CommandParameter { Name = "@CompanyName", Value = Company });
                    objParamList.Add(new CommandParameter { Name = "@BranchName", Value = Branch });
                    dsPricing = objDataDataAccess.ExecuteDataSet("BbpGetCategoryandPricing", objParamList);
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetCategoryAndPricing" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsPricing != null && dsPricing.Tables.Count > 0 && dsPricing.Tables[0].Rows.Count > 0)
                    {
                        objPricingList = dsPricing.Tables[0].AsEnumerable()
                        .Select(dataRow => new CategoryAndPricing
                        {
                            CategoryURI = dataRow.Field<long>("CategoryURI"),
                            CategoryName = dataRow.Field<string>("CategoryName"),
                            CategoryIsActive = dataRow.Field<Boolean>("CategoryIsActive"),
                            CategoryDescription = dataRow.Field<string>("CategoryDescription"),
                            SubCategoryURI = dataRow.Field<long>("SubCategoryURI"),
                            SubCategoryName = dataRow.Field<string>("SubCategoryName"),
                            SubCategoryIsActive = dataRow.Field<Boolean>("SubCategoryIsActive"),
                            SubCategoryDescription = dataRow.Field<string>("SubCategoryDescription"),
                            PricingURI = dataRow.Field<long>("PricingURI"),
                            EffectiveFromDate = dataRow.Field<string>("EffectiveFromDate"),
                            EffectiveToDate = dataRow.Field<string>("EffectiveToDate"),
                            PricingType = dataRow.Field<string>("PricingType"),
                            Amount = dataRow.Field<decimal>("Amount"),
                            DataAvailable = "Data Available"
                        }).ToList();
                    };
                    if (objPricingList != null && objPricingList.Count > 0)
                    {
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetCategoryAndPricing" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objPricingList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return objPricingList;
                    }
                    else
                    {
                        objPricingList = new List<CategoryAndPricing>()
                        {
                            new CategoryAndPricing() { DataAvailable = "No Data Available" }
                        };
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetCategoryAndPricing" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objPricingList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return objPricingList;
                    }

                }
            }
            catch (Exception ex)
            {
                objPricingList = new List<CategoryAndPricing>()
                {
                   new CategoryAndPricing() { DataAvailable = ex.Message }
                };
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetCategoryAndPricing" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objPricingList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                return objPricingList;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/dynamicticketing/SavingTicketDetails")]
        public IHttpActionResult SavingTicketDetails([FromBody] MasterBooking objMasterBooking)
        {
            ApiResponse objApiResponse = new ApiResponse();
            DataSet dsAPIResponseLog = new DataSet();
            DataSet dsAPIRequestLog = new DataSet();
            long requestid = 0;
            List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
            objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "SavingTicketDetails" });
            objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = JsonConvert.SerializeObject(objMasterBooking, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
            using (DataAccess objDataDataAccess = new DataAccess())
            {
                dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
            }
            if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
            {
                requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SavingTicketDetails" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(ModelState, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return BadRequest(ModelState);
                }
                else
                {
                    DataSet dsSavingTicketDetails = new DataSet();
                    decimal SumofTicketAmount = 0;
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        if (objMasterBooking.MasterBookingDetails != null)
                        {
                            foreach (MasterBookingDetails objMasterBookingDetails in objMasterBooking.MasterBookingDetails)
                            {
                                if (SumofTicketAmount > 0)
                                    SumofTicketAmount = objMasterBookingDetails.Amount + SumofTicketAmount;
                                else
                                    SumofTicketAmount = objMasterBookingDetails.Amount;
                            }
                            if(SumofTicketAmount == objMasterBooking.TicketAmount)
                            {
                                foreach (MasterBookingDetails objMasterBookingDetails in objMasterBooking.MasterBookingDetails)
                                {
                                    List<CommandParameter> objParamListBookingDetails = new List<CommandParameter>();
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@AssociatedPricing", Value = objMasterBookingDetails.AssociatedPricing });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@AssociatedCategory", Value = objMasterBookingDetails.AssociatedCategory });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@AssociatedSubCategory", Value = objMasterBookingDetails.AssociatedSubCategory });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@SpecialDay", Value = objMasterBooking.SpecialDay });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@Quantity", Value = objMasterBookingDetails.Quantity });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@Price", Value = objMasterBookingDetails.Price });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@Amount", Value = objMasterBookingDetails.Amount });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@CCNCode", Value = objMasterBooking.CCNCode });
                                    //objParamListBookingDetails.Add(new CommandParameter { Name = "@PayID", Value = objMasterBooking.PayID });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@OrderID", Value = objMasterBooking.OrderID });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitDate", pDbType = PrmType.DateTime, Value = objMasterBooking.VisitDate, Size = 50 });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@BookingDate", pDbType = PrmType.DateTime, Value = objMasterBooking.BookingDate, Size = 50 });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitorFName", Value = objMasterBooking.VisitorFName });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitorLName", Value = objMasterBooking.VisitorLName });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitorPhNo", Value = objMasterBooking.VisitorPhNo });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitorEmailId", Value = objMasterBooking.VisitorEmailId });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@VisitorCity", Value = objMasterBooking.VisitorCity });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@AgentID", Value = objMasterBooking.AgentID });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@PaymentMode", Value = objMasterBooking.PaymentMode });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@TicketAmount", Value = objMasterBooking.TicketAmount });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@ticketdateforSD", pDbType = PrmType.DateTime, Value = objMasterBooking.ticketdateforSD, Size = 50 });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@ticketDateStamp", pDbType = PrmType.DateTime, Value = objMasterBooking.ticketdateforSD, Size = 50 });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@ChoiceID", Value = objMasterBooking.ChoiceID });
                                    objParamListBookingDetails.Add(new CommandParameter { Name = "@TicketCategory", Value = objMasterBooking.TicketCategory });
                                    //objParamListBookingDetails.Add(new CommandParameter { Name = "@SumofTicketAmount", Value = SumofTicketAmount });
                                    dsSavingTicketDetails = objDataDataAccess.ExecuteDataSet("BbpSavingTicketDetails", objParamListBookingDetails);
                                    if (dsSavingTicketDetails != null && dsSavingTicketDetails.Tables.Count > 0 && dsSavingTicketDetails.Tables[0].Rows.Count > 0)
                                    {
                                        if (dsSavingTicketDetails.Tables[0].Rows[0]["Status"].ToString().ToLower().Trim() == "ccncode is already exists")
                                            objApiResponse.status = "CCNCode is already exists.";
                                        else if (dsSavingTicketDetails.Tables[0].Rows[0]["Status"].ToString().ToLower().Trim() == "invalid price")
                                            objApiResponse.status = "Invalid Price.";
                                        else
                                            objApiResponse.status = "Saved Successfully.";
                                    }
                                }
                            }
                            else
                            {
                                objApiResponse.status = "Amount mismatch.";
                            }
                        }
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SavingTicketDetails" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                }
                if (objApiResponse.status == "Saved Successfully.")
                    return Ok(objApiResponse);
                else
                    return BadRequest(objApiResponse.status);
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SavingTicketDetails" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/dynamicticketing/UpdatePaymentfromPSP")]
        public IHttpActionResult UpdatePaymentfromPSP([FromBody] MasterBooking objMasterBooking)
        {
            ApiResponse objApiResponse = new ApiResponse();
            DataSet dsAPIResponseLog = new DataSet();
            DataSet dsAPIRequestLog = new DataSet();
            long requestid = 0;
            List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
            objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "UpdatePaymentfromPSP" });
            objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = JsonConvert.SerializeObject(objMasterBooking, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
            using (DataAccess objDataDataAccess = new DataAccess())
            {
                dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
            }
            if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
            {
                requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
            }
            try
            {
                DataSet dsUpdatePayment = new DataSet();
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    if (objMasterBooking != null)
                    {
                        List<CommandParameter> objParamListUpdatePayment = new List<CommandParameter>();
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@PayID", Value = objMasterBooking.PayID });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@OrderID", Value = objMasterBooking.OrderID });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@PaidAmountFromPG", Value = objMasterBooking.PaidAmountFromPG });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@PaymentStatus", Value = objMasterBooking.PaymentStatus });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@amountchargedtovisitor", Value = objMasterBooking.amountchargedtovisitor });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@paymentgatewaycharges", Value = objMasterBooking.paymentgatewaycharges });
                        objParamListUpdatePayment.Add(new CommandParameter { Name = "@IsActive", Value = objMasterBooking.IsActive });
                        dsUpdatePayment = objDataDataAccess.ExecuteDataSet("BbpUpdatePaymentfromPSP", objParamListUpdatePayment);
                        if (dsUpdatePayment != null && dsUpdatePayment.Tables.Count > 0 && dsUpdatePayment.Tables[0].Rows.Count > 0)
                        {
                            if (dsUpdatePayment.Tables[0].Rows[0]["Status"] != null && dsUpdatePayment.Tables[0].Rows[0]["Status"].ToString().Length > 0)
                                objApiResponse.status = dsUpdatePayment.Tables[0].Rows[0]["Status"].ToString().Trim();
                        }
                    }
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "UpdatePaymentfromPSP" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                if (objApiResponse.status == "Updated Successfully.")
                    return Ok(objApiResponse);
                else
                    return BadRequest(objApiResponse.status);
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "UpdatePaymentfromPSP" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/GetReportData/")]
        public DataSet GetReportData([FromUri] string ReportType, [FromUri] string FromDate, [FromUri] string ToDate, [FromUri] string TypeofDate)
        {
            DataSet dsReportData = new DataSet();
            ApiResponse objApiResponse = new ApiResponse();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@ReportType", Value = ReportType });
                    objParamList.Add(new CommandParameter { Name = "@FromDate", Value = FromDate });
                    objParamList.Add(new CommandParameter { Name = "@ToDate", Value = ToDate });
                    objParamList.Add(new CommandParameter { Name = "@TypeofDate", Value = TypeofDate });
                    if (ReportType.ToLower().Trim() == "cagronline" || ReportType.ToLower().Trim() == "otr" || ReportType.ToLower().Trim() == "sotr")
                    {
                        dsReportData = objDataDataAccess.ExecuteDataSet("BbpGetOnlineReportData", objParamList);
                    }
                    else if (ReportType.ToLower().Trim() == "cagrcounter" || ReportType.ToLower().Trim() == "oftr"
                    || ReportType.ToLower().Trim() == "counterwisecollection" || ReportType.ToLower().Trim() == "adr"
                    || ReportType.ToLower().Trim() == "softr")
                    {
                        dsReportData = objDataDataAccess.ExecuteDataSet("BbpGetOfflineReportData", objParamList);
                    }
                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetReportData" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetReportData" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(dsReportData, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                return dsReportData;
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetReportData" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/GetTicketDetails/")]
        public IHttpActionResult GetTicketDetails([FromUri] string OrderID, [FromUri] string CCNNO)
        {
            DataSet dsTicketDetails = new DataSet();
            MasterBooking objMasterBooking = new MasterBooking();
            ApiResponse objApiResponse = new ApiResponse();
            List<MasterBookingDetails> objMasterBookingDetailsList = new List<MasterBookingDetails>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@OrderID", Value = OrderID });
                    objParamList.Add(new CommandParameter { Name = "@CCNNO", Value = CCNNO });
                    dsTicketDetails = objDataDataAccess.ExecuteDataSet("BbpGetTicketDetails", objParamList);
                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetTicketDetails" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsTicketDetails != null && dsTicketDetails.Tables.Count > 0 && dsTicketDetails.Tables[0].Rows.Count > 0)
                    {
                        Boolean isbookingobject = false;
                        for (int i = 0; i < dsTicketDetails.Tables[0].Rows.Count; i++)
                        {
                            if (!isbookingobject)
                            {
                                objMasterBooking.CCNCode = dsTicketDetails.Tables[0].Rows[i]["CCNCode"].ToString().Trim();
                                objMasterBooking.PayID = dsTicketDetails.Tables[0].Rows[i]["PayID"].ToString().Trim();
                                objMasterBooking.OrderID = dsTicketDetails.Tables[0].Rows[i]["OrderID"].ToString().Trim();
                                objMasterBooking.VisitDate = Convert.ToDateTime(dsTicketDetails.Tables[0].Rows[i]["VisitDate"].ToString().Trim());
                                objMasterBooking.BookingDate = Convert.ToDateTime(dsTicketDetails.Tables[0].Rows[i]["BookingDate"].ToString().Trim());
                                objMasterBooking.SpecialDay = Convert.ToInt64(dsTicketDetails.Tables[0].Rows[i]["SpecialDay"].ToString().Trim());
                                objMasterBooking.VisitorFName = dsTicketDetails.Tables[0].Rows[i]["VisitorFirstName"].ToString().Trim();
                                objMasterBooking.VisitorLName = dsTicketDetails.Tables[0].Rows[i]["VisitorLastName"].ToString().Trim();
                                objMasterBooking.VisitorEmailId = dsTicketDetails.Tables[0].Rows[i]["VisitorEmailId"].ToString().Trim();
                                objMasterBooking.VisitorCity = dsTicketDetails.Tables[0].Rows[i]["VisitorCity"].ToString().Trim();
                                objMasterBooking.VisitorPhNo = dsTicketDetails.Tables[0].Rows[i]["VisitorPhoneNumber"].ToString().Trim();
                                objMasterBooking.AgentID = Convert.ToInt32(dsTicketDetails.Tables[0].Rows[i]["AgentID"].ToString().Trim());
                                objMasterBooking.PaymentMode = dsTicketDetails.Tables[0].Rows[i]["PaymentMode"].ToString().Trim();
                                objMasterBooking.TicketAmount = Convert.ToDecimal(dsTicketDetails.Tables[0].Rows[i]["TicketAmount"].ToString().Trim());
                                objMasterBooking.PaidAmountFromPG = Convert.ToDecimal(dsTicketDetails.Tables[0].Rows[i]["PaidAmountfromPG"].ToString().Trim());
                                objMasterBooking.IsActive = Convert.ToBoolean(dsTicketDetails.Tables[0].Rows[i]["IsActive"].ToString().Trim());
                                objMasterBooking.ChoiceID = dsTicketDetails.Tables[0].Rows[i]["ChoiceID"].ToString().Trim();
                                objMasterBooking.TicketCategory = dsTicketDetails.Tables[0].Rows[i]["TicketCategory"].ToString().Trim();
                                isbookingobject = true;
                            }
                            MasterBookingDetails objMasterBookingDetails = new MasterBookingDetails();
                            objMasterBookingDetails.AssociatedPricing = Convert.ToInt64(dsTicketDetails.Tables[0].Rows[i]["AssociatedPricing"].ToString().Trim());
                            objMasterBookingDetails.AssociatedCategory = Convert.ToInt64(dsTicketDetails.Tables[0].Rows[i]["AssociatedCategory"].ToString().Trim());
                            objMasterBookingDetails.CategoryName = dsTicketDetails.Tables[0].Rows[i]["CategoryName"].ToString().Trim();
                            objMasterBookingDetails.CategoryDescription = dsTicketDetails.Tables[0].Rows[i]["CategoryDescription"].ToString().Trim();
                            objMasterBookingDetails.AssociatedSubCategory = Convert.ToInt64(dsTicketDetails.Tables[0].Rows[i]["AssociatedSubCategory"].ToString().Trim());
                            objMasterBookingDetails.SubCategoryName = dsTicketDetails.Tables[0].Rows[i]["SubCategoryName"].ToString().Trim();
                            objMasterBookingDetails.SubCategoryDescription = dsTicketDetails.Tables[0].Rows[i]["SubCategoryDescription"].ToString().Trim();
                            objMasterBookingDetails.Quantity = Convert.ToInt64(dsTicketDetails.Tables[0].Rows[i]["Quantity"].ToString().Trim());
                            objMasterBookingDetails.Price = Convert.ToDecimal(dsTicketDetails.Tables[0].Rows[i]["Price"].ToString().Trim());
                            objMasterBookingDetails.Amount = Convert.ToDecimal(dsTicketDetails.Tables[0].Rows[i]["Amount"].ToString().Trim());
                            objMasterBookingDetailsList.Add(objMasterBookingDetails);
                            objMasterBooking.MasterBookingDetails = objMasterBookingDetailsList;
                        }
                    }
                    else
                    {
                        objApiResponse.status = "CCNNO does not exist in active state.";
                    }
                }
                if (dsTicketDetails != null && dsTicketDetails.Tables.Count > 0 && dsTicketDetails.Tables[0].Rows.Count > 0)
                {
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetTicketDetails" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objMasterBooking, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return Ok(objMasterBooking);
                }
                else
                {
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetTicketDetails" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return BadRequest(objApiResponse.status);
                }
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetTicketDetails" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/SaveScanDetails/")]
        public IHttpActionResult SaveScanDetails([FromUri] string CCNNo, [FromUri] string ScannedDate, [FromUri] string UserID)
        {
            DataSet dsScanDetails = new DataSet();
            MasterBooking objMasterBooking = new MasterBooking();
            ApiResponse objApiResponse = new ApiResponse();
            List<MasterBookingDetails> objMasterBookingDetailsList = new List<MasterBookingDetails>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@CCNNO", Value = CCNNo });
                    objParamList.Add(new CommandParameter { Name = "@ScannedDate", Value = ScannedDate });
                    objParamList.Add(new CommandParameter { Name = "@UserID", Value = UserID });
                    dsScanDetails = objDataDataAccess.ExecuteDataSet("BbpSaveScanDetails", objParamList);
                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "SaveScanDetails" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsScanDetails != null && dsScanDetails.Tables.Count > 0 && dsScanDetails.Tables[0].Rows.Count > 0)
                    {
                        if (dsScanDetails.Tables[0].Columns.Contains("MessageStatus") && dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim() != null && dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim().Length > 0)
                            objApiResponse.status = dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim();
                        else
                        {

                            Boolean isbookingobject = false;
                            for (int i = 0; i < dsScanDetails.Tables[0].Rows.Count; i++)
                            {
                                if (!isbookingobject)
                                {
                                    objMasterBooking.CCNCode = dsScanDetails.Tables[0].Rows[i]["CCNCode"].ToString().Trim();
                                    objMasterBooking.PayID = dsScanDetails.Tables[0].Rows[i]["PayID"].ToString().Trim();
                                    objMasterBooking.OrderID = dsScanDetails.Tables[0].Rows[i]["OrderID"].ToString().Trim();
                                    objMasterBooking.VisitDate = Convert.ToDateTime(dsScanDetails.Tables[0].Rows[i]["VisitDate"].ToString().Trim());
                                    objMasterBooking.BookingDate = Convert.ToDateTime(dsScanDetails.Tables[0].Rows[i]["BookingDate"].ToString().Trim());
                                    objMasterBooking.SpecialDay = Convert.ToInt64(dsScanDetails.Tables[0].Rows[i]["SpecialDay"].ToString().Trim());
                                    objMasterBooking.VisitorFName = dsScanDetails.Tables[0].Rows[i]["VisitorFirstName"].ToString().Trim();
                                    objMasterBooking.VisitorLName = dsScanDetails.Tables[0].Rows[i]["VisitorLastName"].ToString().Trim();
                                    objMasterBooking.VisitorEmailId = dsScanDetails.Tables[0].Rows[i]["VisitorEmailId"].ToString().Trim();
                                    objMasterBooking.VisitorCity = dsScanDetails.Tables[0].Rows[i]["VisitorCity"].ToString().Trim();
                                    objMasterBooking.VisitorPhNo = dsScanDetails.Tables[0].Rows[i]["VisitorPhoneNumber"].ToString().Trim();
                                    objMasterBooking.AgentID = Convert.ToInt32(dsScanDetails.Tables[0].Rows[i]["AgentID"].ToString().Trim());
                                    objMasterBooking.PaymentMode = dsScanDetails.Tables[0].Rows[i]["PaymentMode"].ToString().Trim();
                                    objMasterBooking.TicketAmount = Convert.ToDecimal(dsScanDetails.Tables[0].Rows[i]["TicketAmount"].ToString().Trim());
                                    objMasterBooking.PaidAmountFromPG = Convert.ToDecimal(dsScanDetails.Tables[0].Rows[i]["PaidAmountfromPG"].ToString().Trim());
                                    objMasterBooking.IsActive = Convert.ToBoolean(dsScanDetails.Tables[0].Rows[i]["IsActive"].ToString().Trim());
                                    objMasterBooking.ChoiceID = dsScanDetails.Tables[0].Rows[i]["ChoiceID"].ToString().Trim();
                                    objMasterBooking.TicketCategory = dsScanDetails.Tables[0].Rows[i]["TicketCategory"].ToString().Trim();
                                    isbookingobject = true;
                                }
                                MasterBookingDetails objMasterBookingDetails = new MasterBookingDetails();
                                objMasterBookingDetails.AssociatedPricing = Convert.ToInt64(dsScanDetails.Tables[0].Rows[i]["AssociatedPricing"].ToString().Trim());
                                objMasterBookingDetails.AssociatedCategory = Convert.ToInt64(dsScanDetails.Tables[0].Rows[i]["AssociatedCategory"].ToString().Trim());
                                objMasterBookingDetails.CategoryName = dsScanDetails.Tables[0].Rows[i]["CategoryName"].ToString().Trim();
                                objMasterBookingDetails.CategoryDescription = dsScanDetails.Tables[0].Rows[i]["CategoryDescription"].ToString().Trim();
                                objMasterBookingDetails.AssociatedSubCategory = Convert.ToInt64(dsScanDetails.Tables[0].Rows[i]["AssociatedSubCategory"].ToString().Trim());
                                objMasterBookingDetails.SubCategoryName = dsScanDetails.Tables[0].Rows[i]["SubCategoryName"].ToString().Trim();
                                objMasterBookingDetails.SubCategoryDescription = dsScanDetails.Tables[0].Rows[i]["SubCategoryDescription"].ToString().Trim();
                                objMasterBookingDetails.Quantity = Convert.ToInt64(dsScanDetails.Tables[0].Rows[i]["Quantity"].ToString().Trim());
                                objMasterBookingDetails.Price = Convert.ToDecimal(dsScanDetails.Tables[0].Rows[i]["Price"].ToString().Trim());
                                objMasterBookingDetails.Amount = Convert.ToDecimal(dsScanDetails.Tables[0].Rows[i]["Amount"].ToString().Trim());
                                objMasterBookingDetailsList.Add(objMasterBookingDetails);
                                objMasterBooking.MasterBookingDetails = objMasterBookingDetailsList;
                            }
                        }
                    }
                }
                if (dsScanDetails != null && dsScanDetails.Tables.Count > 0
                    && dsScanDetails.Tables[0].Rows.Count > 0 && dsScanDetails.Tables[0].Columns.Contains("MessageStatus") && dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim() != null
                    && dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim().Length > 0)
                {
                    objApiResponse.status = dsScanDetails.Tables[0].Rows[0]["MessageStatus"].ToString().Trim();
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SaveScanDetails" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return BadRequest(objApiResponse.status);
                }
                else
                {
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SaveScanDetails" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objMasterBooking, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return Ok(objMasterBooking);
                }

            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "SaveScanDetails" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/RedactTicket/")]
        public IHttpActionResult RedactTicket([FromUri] string CCNNo, [FromUri] long AgentID, [FromUri] string VisitDate, [FromUri] long RequestedBy,
        [FromUri] char Redact)
        {
            DataSet dsRedactTicket = new DataSet();
            MasterBooking objMasterBooking = new MasterBooking();
            ApiResponse objApiResponse = new ApiResponse();
            List<MasterBookingDetails> objMasterBookingDetailsList = new List<MasterBookingDetails>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@CCNNO", Value = CCNNo });
                    objParamList.Add(new CommandParameter { Name = "@AgentID", Value = AgentID });
                    objParamList.Add(new CommandParameter { Name = "@VisitDate", Value = VisitDate });
                    objParamList.Add(new CommandParameter { Name = "@RequestedBy", Value = RequestedBy });
                    objParamList.Add(new CommandParameter { Name = "@Redact", Value = Redact });
                    dsRedactTicket = objDataDataAccess.ExecuteDataSet("BbpRedactTicket", objParamList);
                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "RedactTicket" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsRedactTicket != null && dsRedactTicket.Tables.Count > 0 && dsRedactTicket.Tables[0].Rows.Count > 0)
                    {
                        if (dsRedactTicket.Tables[0].Columns.Contains("MessageStatus") && dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim() != null && dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim().Length > 0)
                            objApiResponse.status = dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim();
                        else
                        {

                            Boolean isbookingobject = false;
                            for (int i = 0; i < dsRedactTicket.Tables[0].Rows.Count; i++)
                            {
                                if (!isbookingobject)
                                {
                                    objMasterBooking.CCNCode = dsRedactTicket.Tables[0].Rows[i]["CCNCode"].ToString().Trim();
                                    objMasterBooking.PayID = dsRedactTicket.Tables[0].Rows[i]["PayID"].ToString().Trim();
                                    objMasterBooking.OrderID = dsRedactTicket.Tables[0].Rows[i]["OrderID"].ToString().Trim();
                                    objMasterBooking.VisitDate = Convert.ToDateTime(dsRedactTicket.Tables[0].Rows[i]["VisitDate"].ToString().Trim());
                                    objMasterBooking.BookingDate = Convert.ToDateTime(dsRedactTicket.Tables[0].Rows[i]["BookingDate"].ToString().Trim());
                                    objMasterBooking.SpecialDay = Convert.ToInt64(dsRedactTicket.Tables[0].Rows[i]["SpecialDay"].ToString().Trim());
                                    objMasterBooking.VisitorFName = dsRedactTicket.Tables[0].Rows[i]["VisitorFirstName"].ToString().Trim();
                                    objMasterBooking.VisitorLName = dsRedactTicket.Tables[0].Rows[i]["VisitorLastName"].ToString().Trim();
                                    objMasterBooking.VisitorEmailId = dsRedactTicket.Tables[0].Rows[i]["VisitorEmailId"].ToString().Trim();
                                    objMasterBooking.VisitorCity = dsRedactTicket.Tables[0].Rows[i]["VisitorCity"].ToString().Trim();
                                    objMasterBooking.VisitorPhNo = dsRedactTicket.Tables[0].Rows[i]["VisitorPhoneNumber"].ToString().Trim();
                                    objMasterBooking.AgentID = Convert.ToInt32(dsRedactTicket.Tables[0].Rows[i]["AgentID"].ToString().Trim());
                                    objMasterBooking.PaymentMode = dsRedactTicket.Tables[0].Rows[i]["PaymentMode"].ToString().Trim();
                                    objMasterBooking.TicketAmount = Convert.ToDecimal(dsRedactTicket.Tables[0].Rows[i]["TicketAmount"].ToString().Trim());
                                    objMasterBooking.PaidAmountFromPG = Convert.ToDecimal(dsRedactTicket.Tables[0].Rows[i]["PaidAmountfromPG"].ToString().Trim());
                                    objMasterBooking.IsActive = Convert.ToBoolean(dsRedactTicket.Tables[0].Rows[i]["IsActive"].ToString().Trim());
                                    objMasterBooking.ChoiceID = dsRedactTicket.Tables[0].Rows[i]["ChoiceID"].ToString().Trim();
                                    objMasterBooking.TicketCategory = dsRedactTicket.Tables[0].Rows[i]["TicketCategory"].ToString().Trim();
                                    objMasterBooking.Status = dsRedactTicket.Tables[0].Rows[i]["Status"].ToString().Trim();
                                    isbookingobject = true;
                                }
                                MasterBookingDetails objMasterBookingDetails = new MasterBookingDetails();
                                objMasterBookingDetails.AssociatedPricing = Convert.ToInt64(dsRedactTicket.Tables[0].Rows[i]["AssociatedPricing"].ToString().Trim());
                                objMasterBookingDetails.AssociatedCategory = Convert.ToInt64(dsRedactTicket.Tables[0].Rows[i]["AssociatedCategory"].ToString().Trim());
                                objMasterBookingDetails.CategoryName = dsRedactTicket.Tables[0].Rows[i]["CategoryName"].ToString().Trim();
                                objMasterBookingDetails.CategoryDescription = dsRedactTicket.Tables[0].Rows[i]["CategoryDescription"].ToString().Trim();
                                objMasterBookingDetails.AssociatedSubCategory = Convert.ToInt64(dsRedactTicket.Tables[0].Rows[i]["AssociatedSubCategory"].ToString().Trim());
                                objMasterBookingDetails.SubCategoryName = dsRedactTicket.Tables[0].Rows[i]["SubCategoryName"].ToString().Trim();
                                objMasterBookingDetails.SubCategoryDescription = dsRedactTicket.Tables[0].Rows[i]["SubCategoryDescription"].ToString().Trim();
                                objMasterBookingDetails.Quantity = Convert.ToInt64(dsRedactTicket.Tables[0].Rows[i]["Quantity"].ToString().Trim());
                                objMasterBookingDetails.Price = Convert.ToDecimal(dsRedactTicket.Tables[0].Rows[i]["Price"].ToString().Trim());
                                objMasterBookingDetails.Amount = Convert.ToDecimal(dsRedactTicket.Tables[0].Rows[i]["Amount"].ToString().Trim());
                                objMasterBookingDetailsList.Add(objMasterBookingDetails);
                                objMasterBooking.MasterBookingDetails = objMasterBookingDetailsList;
                            }
                        }
                    }
                }
                if (dsRedactTicket != null && dsRedactTicket.Tables.Count > 0
                    && dsRedactTicket.Tables[0].Rows.Count > 0 && dsRedactTicket.Tables[0].Columns.Contains("MessageStatus") && dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim() != null
                    && dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim().Length > 0)
                {
                    objApiResponse.status = dsRedactTicket.Tables[0].Rows[0]["MessageStatus"].ToString().Trim();
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "RedactTicket" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return BadRequest(objApiResponse.status);
                }
                else
                {
                    List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "RedactTicket" });
                    objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objMasterBooking, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                    using (DataAccess objDataDataAccess = new DataAccess())
                    {
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                    }
                    return Ok(objMasterBooking);
                }
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message; 
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "RedactTicket" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, objApiResponse.status));
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/GetSpecialWorkingDay")]
        public IHttpActionResult GetSpecialWorkingDay()
        {
            DataSet dsSpecialWorkingDay = new DataSet();
            ApiResponse objApiResponse = new ApiResponse();
            List<SpecialWorkingDay> objSpecialWorkingDayList = new List<SpecialWorkingDay>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    dsSpecialWorkingDay = objDataDataAccess.ExecuteDataSet("GetSpecialWorkingDay");

                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetSpecialWorkingDay" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsSpecialWorkingDay != null && dsSpecialWorkingDay.Tables[0] != null)
                    {
                        objSpecialWorkingDayList=(from rw in dsSpecialWorkingDay.Tables[0].AsEnumerable()
                         select new SpecialWorkingDay()
                         {
                             Id = Convert.ToInt32(rw["Id"]),
                             SpecialDay = Convert.ToDateTime(rw["SpecialDay"]),
                             CreatedDate = Convert.ToDateTime(rw["CreatedDate"])
                         }).ToList();

                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetSpecialWorkingDay" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objSpecialWorkingDayList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return Ok(objSpecialWorkingDayList);
                    }
                    else
                    {
                        SpecialWorkingDay objSpecialWorkingDay = new SpecialWorkingDay();
                        objSpecialWorkingDay.Status = "No data available";
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetSpecialWorkingDay" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objSpecialWorkingDay, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return BadRequest(objSpecialWorkingDay.Status);
                    }
                }

            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetSpecialWorkingDay" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/GetRestrictedWorkingDay")]
        public IHttpActionResult GetRestrictedWorkingDay()
        {
            DataSet dsSpecialWorkingDay = new DataSet();
            ApiResponse objApiResponse = new ApiResponse();
            List<SpecialWorkingDay> objSpecialWorkingDayList = new List<SpecialWorkingDay>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    dsSpecialWorkingDay = objDataDataAccess.ExecuteDataSet("GetRestrictedWorkingDay");

                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetRestrictedWorkingDay" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsSpecialWorkingDay != null && dsSpecialWorkingDay.Tables[0] != null)
                    {
                        objSpecialWorkingDayList = (from rw in dsSpecialWorkingDay.Tables[0].AsEnumerable()
                                                    select new SpecialWorkingDay()
                                                    {
                                                        Id = Convert.ToInt32(rw["Id"]),
                                                        SpecialDay = Convert.ToDateTime(rw["RestrictedDay"]),
                                                        CreatedDate = Convert.ToDateTime(rw["CreatedDate"])
                                                    }).ToList();

                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetRestrictedWorkingDay" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objSpecialWorkingDayList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return Ok(objSpecialWorkingDayList);
                    }
                    else
                    {
                        SpecialWorkingDay objSpecialWorkingDay = new SpecialWorkingDay();
                        objSpecialWorkingDay.Status = "No data available";
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetRestrictedWorkingDay" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objSpecialWorkingDay, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return BadRequest(objSpecialWorkingDay.Status);
                    }
                }

            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetRestrictedWorkingDay" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dynamicticketing/GetScanSummary")]
        public IHttpActionResult GetScanSummary([FromUri] string ScannedDate)
        {
            DataSet dsScanSummary = new DataSet();
            ApiResponse objApiResponse = new ApiResponse();
            List<ScanSummary> objScanSummaryList = new List<ScanSummary>();
            DataSet dsAPIResponseLog = new DataSet();
            long requestid = 0;
            try
            {
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    List<CommandParameter> objParamList = new List<CommandParameter>();
                    objParamList.Add(new CommandParameter { Name = "@ScannedDate", Value = ScannedDate });
                    dsScanSummary = objDataDataAccess.ExecuteDataSet("BbpGetScanSummary", objParamList);

                    DataSet dsAPIRequestLog = new DataSet();
                    List<CommandParameter> objParamAPIRequestLog = new List<CommandParameter>();
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@APIName", Value = "GetScanSummary" });
                    objParamAPIRequestLog.Add(new CommandParameter { Name = "@RequestDetails", Value = "" });
                    dsAPIRequestLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIRequestLog", objParamAPIRequestLog);
                    if (dsAPIRequestLog != null && dsAPIRequestLog.Tables.Count > 0 && dsAPIRequestLog.Tables[0].Rows.Count > 0)
                    {
                        requestid = Convert.ToInt64(dsAPIRequestLog.Tables[0].Rows[0]["Id"].ToString().Trim());
                    }
                    if (dsScanSummary != null && dsScanSummary.Tables[0] != null && dsScanSummary.Tables[0].Rows.Count > 0)
                    {
                        objScanSummaryList = (from rw in dsScanSummary.Tables[0].AsEnumerable()
                                              select new ScanSummary()
                                              {
                                                  TicketScanCount = Convert.ToInt32(rw["TicketScanCount"]),
                                                  CategoryName = Convert.ToString(rw["CategoryName"]),
                                                  CategoryURI = Convert.ToInt32(rw["CategoryURI"])
                                              }).ToList();

                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetScanSummary" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objScanSummaryList, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return Ok(objScanSummaryList);
                    }
                    else
                    {
                        ScanSummary objScanSummary = new ScanSummary();
                        objScanSummary.Status = "No data available";
                        List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetScanSummary" });
                        objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objScanSummary, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                        dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                        return BadRequest(objScanSummary.Status);
                    }
                }
            }
            catch (Exception ex)
            {
                objApiResponse.status = ex.Message;
                List<CommandParameter> objParamAPIResponseLog = new List<CommandParameter>();
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@RequestId", Value = requestid });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@APIName", Value = "GetScanSummary" });
                objParamAPIResponseLog.Add(new CommandParameter { Name = "@ResponseDetails", Value = JsonConvert.SerializeObject(objApiResponse, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) });
                using (DataAccess objDataDataAccess = new DataAccess())
                {
                    dsAPIResponseLog = objDataDataAccess.ExecuteDataSet("BbpSavesAPIResponseLog", objParamAPIResponseLog);
                }
                return InternalServerError(ex);
            }
        }
    }
}
