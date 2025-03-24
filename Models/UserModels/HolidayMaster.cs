using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Holiday_Master")]
public partial class HolidayMaster
{
    [Column("Hol_YearId")]
    public int? HolYearId { get; set; }

    [Column("Hol_Date", TypeName = "datetime")]
    public DateTime? HolDate { get; set; }

    [Column("Hol_Remarks")]
    [StringLength(500)]
    [Unicode(false)]
    public string? HolRemarks { get; set; }

    [Column("Hol_Createdby")]
    public int? HolCreatedby { get; set; }

    [Column("Hol_CreatedOn", TypeName = "datetime")]
    public DateTime? HolCreatedOn { get; set; }

    [Column("Hol_UpdatedBy")]
    public int? HolUpdatedBy { get; set; }

    [Column("Hol_UpdatedOn", TypeName = "datetime")]
    public DateTime? HolUpdatedOn { get; set; }

    [Column("Hol_ApprovedBy")]
    public int? HolApprovedBy { get; set; }

    [Column("Hol_ApprovedOn", TypeName = "datetime")]
    public DateTime? HolApprovedOn { get; set; }

    [Column("Hol_Status")]
    [StringLength(3)]
    [Unicode(false)]
    public string? HolStatus { get; set; }

    [Column("Hol_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? HolDelFlag { get; set; }

    [Column("Hol_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? HolIpaddress { get; set; }

    [Column("Hol_CompID")]
    public int? HolCompId { get; set; }
}
