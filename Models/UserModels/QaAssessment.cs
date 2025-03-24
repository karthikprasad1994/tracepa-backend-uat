using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("QA_Assessment")]
public partial class QaAssessment
{
    [Column("QA_PKID")]
    public int? QaPkid { get; set; }

    [Column("QA_Code")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QaCode { get; set; }

    [Column("QA_FinancialYear")]
    public int? QaFinancialYear { get; set; }

    [Column("QA_CUSTID")]
    public int? QaCustid { get; set; }

    [Column("QA_FUNID")]
    public int? QaFunid { get; set; }

    [Column("QA_StartDate", TypeName = "datetime")]
    public DateTime? QaStartDate { get; set; }

    [Column("QA_EndDate", TypeName = "datetime")]
    public DateTime? QaEndDate { get; set; }

    [Column("QA_AUDITORTEAM")]
    [StringLength(100)]
    [Unicode(false)]
    public string? QaAuditorteam { get; set; }

    [Column("QA_AUDITTITLE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? QaAudittitle { get; set; }

    [Column("QA_Delflag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? QaDelflag { get; set; }

    [Column("QA_CrBy")]
    public int? QaCrBy { get; set; }

    [Column("QA_CrOn", TypeName = "datetime")]
    public DateTime? QaCrOn { get; set; }

    [Column("QA_UpdatedBy")]
    public int? QaUpdatedBy { get; set; }

    [Column("QA_UpdatedOn", TypeName = "datetime")]
    public DateTime? QaUpdatedOn { get; set; }

    [Column("QA_SavedBy")]
    public int? QaSavedBy { get; set; }

    [Column("QA_SavedOn", TypeName = "datetime")]
    public DateTime? QaSavedOn { get; set; }

    [Column("QA_AUDStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QaAudstatus { get; set; }

    [Column("QA_WPStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QaWpstatus { get; set; }

    [Column("QA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? QaIpaddress { get; set; }

    [Column("QA_CompID")]
    public int? QaCompId { get; set; }
}
