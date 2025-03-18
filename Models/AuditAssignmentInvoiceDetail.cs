using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentInvoiceDetail
{
    public int? AaidId { get; set; }

    public int? AaidAaiId { get; set; }

    public int? AaidAasId { get; set; }

    public string? AaidDesc { get; set; }

    public int? AaidHsnsac { get; set; }

    public int? AaidQuantity { get; set; }

    public decimal? AaidPricePerUnit { get; set; }

    public int? AaidCrBy { get; set; }

    public DateTime? AaidCrOn { get; set; }

    public string? AaidIpaddress { get; set; }

    public int? AaidCompId { get; set; }

    public int? AaidIsTaxable { get; set; }
}
