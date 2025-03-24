using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("acc_coa_settings")]
public partial class AccCoaSetting
{
    [Column("ACS_Id")]
    public int AcsId { get; set; }

    [Column("ACS_AccHead")]
    public int? AcsAccHead { get; set; }

    [Column("ACS_AccHeadPrefix")]
    [StringLength(5)]
    [Unicode(false)]
    public string? AcsAccHeadPrefix { get; set; }

    [Column("ACS_Group")]
    public int? AcsGroup { get; set; }

    [Column("ACS_SubGroup")]
    public int? AcsSubGroup { get; set; }

    [Column("ACS_GL")]
    public int? AcsGl { get; set; }

    [Column("ACS_SubGL")]
    public int? AcsSubGl { get; set; }

    [Column("ACS_CompId")]
    public int? AcsCompId { get; set; }

    [Column("ACS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AcsIpaddress { get; set; }

    [Column("ACS_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? AcsOperation { get; set; }

    [Column("ACS_CreatedBy")]
    public int? AcsCreatedBy { get; set; }

    [Column("ACS_CreatedOn", TypeName = "datetime")]
    public DateTime? AcsCreatedOn { get; set; }

    [Column("ACS_UpdatedBy")]
    public int? AcsUpdatedBy { get; set; }

    [Column("ACS_UpdatedOn", TypeName = "datetime")]
    public DateTime? AcsUpdatedOn { get; set; }
}
