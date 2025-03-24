using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_SubHeadingNoteDesc")]
public partial class AccSubHeadingNoteDesc
{
    [Column("ASHN_ID")]
    public int? AshnId { get; set; }

    [Column("ASHN_SubHeadingId")]
    public int? AshnSubHeadingId { get; set; }

    [Column("ASHN_CustomerId")]
    public int? AshnCustomerId { get; set; }

    [Column("ASHN_Description")]
    [Unicode(false)]
    public string? AshnDescription { get; set; }

    [Column("ASHN_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AshnDelFlag { get; set; }

    [Column("ASHN_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AshnStatus { get; set; }

    [Column("ASHN_CreatedBy")]
    public int? AshnCreatedBy { get; set; }

    [Column("ASHN_CreatedOn", TypeName = "datetime")]
    public DateTime? AshnCreatedOn { get; set; }

    [Column("ASHN_UpdatedBy")]
    public int? AshnUpdatedBy { get; set; }

    [Column("ASHN_UpdatedOn", TypeName = "datetime")]
    public DateTime? AshnUpdatedOn { get; set; }

    [Column("ASHN_ApprovedBy")]
    public int? AshnApprovedBy { get; set; }

    [Column("ASHN_ApprovedOn", TypeName = "datetime")]
    public DateTime? AshnApprovedOn { get; set; }

    [Column("ASHN_CompID")]
    public int? AshnCompId { get; set; }

    [Column("ASHN_YearID")]
    public int? AshnYearId { get; set; }

    [Column("ASHN_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AshnIpaddress { get; set; }

    [Column("ASHN_Operation")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AshnOperation { get; set; }
}
