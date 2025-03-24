using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_InvoiceDetails")]
public partial class AuditAssignmentInvoiceDetail
{
    [Column("AAID_ID")]
    public int? AaidId { get; set; }

    [Column("AAID_AAI_ID")]
    public int? AaidAaiId { get; set; }

    [Column("AAID_AAS_ID")]
    public int? AaidAasId { get; set; }

    [Column("AAID_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AaidDesc { get; set; }

    [Column("AAID_HSNSAC")]
    public int? AaidHsnsac { get; set; }

    [Column("AAID_Quantity")]
    public int? AaidQuantity { get; set; }

    [Column("AAID_PricePerUnit", TypeName = "decimal(10, 2)")]
    public decimal? AaidPricePerUnit { get; set; }

    [Column("AAID_CrBy")]
    public int? AaidCrBy { get; set; }

    [Column("AAID_CrOn", TypeName = "datetime")]
    public DateTime? AaidCrOn { get; set; }

    [Column("AAID_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AaidIpaddress { get; set; }

    [Column("AAID_CompID")]
    public int? AaidCompId { get; set; }

    [Column("AAID_IsTaxable")]
    public int? AaidIsTaxable { get; set; }
}
