using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadCustomerLegalTransfer
{
    public int CltId { get; set; }

    public string? CltType { get; set; }

    public int? CltCustomerId { get; set; }

    public string? CltName { get; set; }

    public string? CltAddress { get; set; }

    public string? CltEmail { get; set; }

    public string? CltStatus { get; set; }

    public int? CltCrBy { get; set; }

    public DateTime? CltCreatedOn { get; set; }

    public int? CltCompId { get; set; }
}
