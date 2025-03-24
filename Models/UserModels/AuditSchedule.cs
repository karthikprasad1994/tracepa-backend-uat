using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Schedule")]
public partial class AuditSchedule
{
    [Column("AUD_ID")]
    public int? AudId { get; set; }

    [Column("AUD_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AudCode { get; set; }

    [Column("AUD_YearID")]
    public int? AudYearId { get; set; }

    [Column("AUD_MonthID")]
    public int? AudMonthId { get; set; }

    [Column("AUD_SectionID")]
    public int? AudSectionId { get; set; }

    [Column("AUD_KitchenID")]
    public int? AudKitchenId { get; set; }

    [Column("AUD_AuditorIDs")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AudAuditorIds { get; set; }

    [Column("AUD_FromDate", TypeName = "datetime")]
    public DateTime? AudFromDate { get; set; }

    [Column("AUD_ToDate", TypeName = "datetime")]
    public DateTime? AudToDate { get; set; }

    [Column("AUD_Intmail")]
    public int? AudIntmail { get; set; }

    [Column("AUD_Firstmail")]
    public int? AudFirstmail { get; set; }

    [Column("AUD_SecondMail")]
    public int? AudSecondMail { get; set; }

    [Column("AUD_CrBy")]
    public int? AudCrBy { get; set; }

    [Column("AUD_CrOn", TypeName = "datetime")]
    public DateTime? AudCrOn { get; set; }

    [Column("AUD_UpdatedBy")]
    public int? AudUpdatedBy { get; set; }

    [Column("AUD_UpdateOn", TypeName = "datetime")]
    public DateTime? AudUpdateOn { get; set; }

    [Column("AUD_ApprovedBy")]
    public int? AudApprovedBy { get; set; }

    [Column("AUD_ApprovedOn", TypeName = "datetime")]
    public DateTime? AudApprovedOn { get; set; }

    [Column("AUD_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AudStatus { get; set; }

    [Column("AUD_CompID")]
    public int? AudCompId { get; set; }

    [Column("AUD_Operation")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AudOperation { get; set; }

    [Column("AUD_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AudIpaddress { get; set; }
}
