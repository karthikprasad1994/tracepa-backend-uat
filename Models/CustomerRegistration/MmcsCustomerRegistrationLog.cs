using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsCustomerRegistrationLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? McrId { get; set; }

    public int? McrMpId { get; set; }

    public int? NMcrMpId { get; set; }

    public string? McrCustomerName { get; set; }

    public string? NMcrCustomerName { get; set; }

    public string? McrCustomerCode { get; set; }

    public string? NMcrCustomerCode { get; set; }

    public string? McrCustomerEmail { get; set; }

    public string? NMcrCustomerEmail { get; set; }

    public string? McrCustomerTelephoneNo { get; set; }

    public string? NMcrCustomerTelephoneNo { get; set; }

    public string? McrContactPersonName { get; set; }

    public string? NMcrContactPersonName { get; set; }

    public string? McrContactPersonPhoneNo { get; set; }

    public string? NMcrContactPersonPhoneNo { get; set; }

    public string? McrContactPersonEmail { get; set; }

    public string? NMcrContactPersonEmail { get; set; }

    public string? McrGstno { get; set; }

    public string? NMcrGstno { get; set; }

    public int? McrNumberOfUsers { get; set; }

    public int? NMcrNumberOfUsers { get; set; }

    public string? McrAddress { get; set; }

    public string? NMcrAddress { get; set; }

    public string? McrCity { get; set; }

    public string? NMcrCity { get; set; }

    public string? McrState { get; set; }

    public string? NMcrState { get; set; }

    public int? McrBillingFrequency { get; set; }

    public int? NMcrBillingFrequency { get; set; }

    public DateTime? McrFromDate { get; set; }

    public DateTime? NMcrFromDate { get; set; }

    public DateTime? McrToDate { get; set; }

    public DateTime? NMcrToDate { get; set; }

    public string? McrProductKey { get; set; }

    public string? NMcrProductKey { get; set; }

    public int? McrDataSize { get; set; }

    public int? NMcrDataSize { get; set; }

    public int? McrNumberOfCustomers { get; set; }

    public int? NMcrNumberOfCustomers { get; set; }
}
