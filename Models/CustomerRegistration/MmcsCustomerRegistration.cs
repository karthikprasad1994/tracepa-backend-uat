using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsCustomerRegistration
{
    [Key]
    public int? McrId { get; set; }

    public int? McrMpId { get; set; }

    public string? McrCustomerName { get; set; }

    public string? McrCustomerCode { get; set; }

    public string? McrCustomerEmail { get; set; }

    public string? McrCustomerTelephoneNo { get; set; }

    public string? McrContactPersonName { get; set; }

    public string? McrContactPersonPhoneNo { get; set; }

    public string? McrContactPersonEmail { get; set; }

    public string? McrGstno { get; set; }

    public int? McrNumberOfUsers { get; set; }

    public string? McrAddress { get; set; }

    public string? McrCity { get; set; }

    public string? McrState { get; set; }

    public int? McrBillingFrequency { get; set; }

    public DateOnly? McrFromDate { get; set; }

    public DateOnly? McrToDate { get; set; }

    public DateOnly? McrCreatedDate { get; set; }

    public string? McrStatus { get; set; }

    public string? McrProductKey { get; set; }

    public string? McrTstatus { get; set; }

    public int? McrRnwlFee { get; set; }

    public int? McrDataSize { get; set; }

    public int? McrNumberOfCustomers { get; set; }

    [JsonConstructor]
    public MmcsCustomerRegistration() { }

    public void SetCustomerCodeAndProductKey(string mcrCustomerCode, string mcrProductKey)
    {
        McrCustomerCode = mcrCustomerCode;
        McrProductKey = mcrProductKey;
    }
}
