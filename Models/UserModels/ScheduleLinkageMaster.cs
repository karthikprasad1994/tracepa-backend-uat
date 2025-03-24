using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Schedule_Linkage_Master")]
public partial class ScheduleLinkageMaster
{
    [Column("SLM_ID")]
    public int? SlmId { get; set; }

    [Column("SLM_Head")]
    public int? SlmHead { get; set; }

    [Column("SLM_GroupID")]
    public int? SlmGroupId { get; set; }

    [Column("SLM_SubGroupID")]
    public int? SlmSubGroupId { get; set; }

    [Column("SLM_GLId")]
    public int? SlmGlid { get; set; }

    [Column("SLM_GLLedger")]
    [Unicode(false)]
    public string? SlmGlledger { get; set; }

    [Column("SLM_CreatedBy")]
    public int? SlmCreatedBy { get; set; }

    [Column("SLM_CreatedOn", TypeName = "datetime")]
    public DateTime? SlmCreatedOn { get; set; }

    [Column("SLM_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SlmStatus { get; set; }

    [Column("SLM_YearID")]
    public int? SlmYearId { get; set; }

    [Column("SLM_CompID")]
    public int? SlmCompId { get; set; }

    [Column("SLM_NoteNo")]
    public int? SlmNoteNo { get; set; }

    [Column("SLM_DeletedBy")]
    public int? SlmDeletedBy { get; set; }

    [Column("SLM_DeletedOn", TypeName = "datetime")]
    public DateTime? SlmDeletedOn { get; set; }

    [Column("SLM_UpdatedBy")]
    public int? SlmUpdatedBy { get; set; }

    [Column("SLM_UpdatedOn", TypeName = "datetime")]
    public DateTime? SlmUpdatedOn { get; set; }

    [Column("SLM_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SlmOperation { get; set; }

    [Column("SLM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SlmIpaddress { get; set; }

    [Column("SLM_OrgID")]
    public int? SlmOrgId { get; set; }

    [Column("SLM_CustID")]
    public int? SlmCustId { get; set; }
}
