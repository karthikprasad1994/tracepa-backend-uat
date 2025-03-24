using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_SubHeadingLedgerDesc")]
public partial class AccSubHeadingLedgerDesc
{
    [Column("ASHL_ID")]
    public int? AshlId { get; set; }

    [Column("ASHL_SubHeadingId")]
    public int? AshlSubHeadingId { get; set; }

    [Column("ASHL_CustomerId")]
    public int? AshlCustomerId { get; set; }

    [Column("ASHL_Description")]
    [Unicode(false)]
    public string? AshlDescription { get; set; }

    [Column("ASHL_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AshlDelFlag { get; set; }

    [Column("ASHL_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AshlStatus { get; set; }

    [Column("ASHL_CreatedBy")]
    public int? AshlCreatedBy { get; set; }

    [Column("ASHL_CreatedOn", TypeName = "datetime")]
    public DateTime? AshlCreatedOn { get; set; }

    [Column("ASHL_UpdatedBy")]
    public int? AshlUpdatedBy { get; set; }

    [Column("ASHL_UpdatedOn", TypeName = "datetime")]
    public DateTime? AshlUpdatedOn { get; set; }

    [Column("ASHL_ApprovedBy")]
    public int? AshlApprovedBy { get; set; }

    [Column("ASHL_ApprovedOn", TypeName = "datetime")]
    public DateTime? AshlApprovedOn { get; set; }

    [Column("ASHL_CompID")]
    public int? AshlCompId { get; set; }

    [Column("ASHL_YearID")]
    public int? AshlYearId { get; set; }

    [Column("ASHL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AshlIpaddress { get; set; }

    [Column("ASHL_Operation")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AshlOperation { get; set; }

    [Column("ASHL_BranchId")]
    public int? AshlBranchId { get; set; }
}
