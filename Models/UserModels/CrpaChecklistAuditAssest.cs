using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("CRPA_ChecklistAuditAssest")]
public partial class CrpaChecklistAuditAssest
{
    [Column("CRAD_PKID")]
    public int? CradPkid { get; set; }

    [Column("CRAD_CAuditID")]
    public int? CradCauditId { get; set; }

    [Column("CRAD_LOCATIONID")]
    public int? CradLocationid { get; set; }

    [Column("CRAD_SECTIONID")]
    public int? CradSectionid { get; set; }

    [Column("CRAD_SUBSECTIONID")]
    public int? CradSubsectionid { get; set; }

    [Column("CRAD_PROCESSID")]
    public int? CradProcessid { get; set; }

    [Column("CRAD_SUBPROCESSID")]
    public int? CradSubprocessid { get; set; }

    [Column("CRAD_FINDINGS")]
    public int? CradFindings { get; set; }

    [Column("CRAD_SCORE_STANDARD")]
    public int? CradScoreStandard { get; set; }

    [Column("CRAD_SCORE_RESULT")]
    public int? CradScoreResult { get; set; }

    [Column("CRAD_COMMENTS")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CradComments { get; set; }

    [Column("CRAD_DATE", TypeName = "datetime")]
    public DateTime? CradDate { get; set; }

    [Column("CRAD_YEARID")]
    public int? CradYearid { get; set; }

    [Column("CRAD_CREATEDBY")]
    public int? CradCreatedby { get; set; }

    [Column("CRAD_CREATEDON", TypeName = "datetime")]
    public DateTime? CradCreatedon { get; set; }

    [Column("CRAD_UPDATEDBY")]
    public int? CradUpdatedby { get; set; }

    [Column("CRAD_UPDATEDON", TypeName = "datetime")]
    public DateTime? CradUpdatedon { get; set; }

    [Column("CRAD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CradIpaddress { get; set; }

    [Column("CRAD_CompID")]
    public int? CradCompId { get; set; }
}
