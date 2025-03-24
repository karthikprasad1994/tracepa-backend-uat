using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_RISK_Library_Log")]
public partial class MstRiskLibraryLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(30)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("MRL_PKID")]
    public int? MrlPkid { get; set; }

    [Column("MRL_RiskName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MrlRiskName { get; set; }

    [Column("nMRL_RiskName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? NMrlRiskName { get; set; }

    [Column("MRL_RiskDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? MrlRiskDesc { get; set; }

    [Column("nMRL_RiskDesc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NMrlRiskDesc { get; set; }

    [Column("MRL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MrlCode { get; set; }

    [Column("nMRL_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NMrlCode { get; set; }

    [Column("MRL_RiskTypeID")]
    public int? MrlRiskTypeId { get; set; }

    [Column("nMRL_RiskTypeID")]
    public int? NMrlRiskTypeId { get; set; }

    [Column("MRL_IsKey")]
    public int? MrlIsKey { get; set; }

    [Column("nMRL_IsKey")]
    public int? NMrlIsKey { get; set; }

    [Column("MRL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MrlIpaddress { get; set; }

    [Column("MRL_CompID")]
    public int? MrlCompId { get; set; }
}
