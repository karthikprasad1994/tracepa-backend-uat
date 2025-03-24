using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_ScheduleObservations")]
public partial class StandardAuditScheduleObservation
{
    [Column("SSO_PKID")]
    public int? SsoPkid { get; set; }

    [Column("SSO_SA_ID")]
    public int? SsoSaId { get; set; }

    [Column("SSO_SAC_CheckPointID")]
    public int? SsoSacCheckPointId { get; set; }

    [Column("SSO_Observations")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SsoObservations { get; set; }

    [Column("SSO_CrBy")]
    public int? SsoCrBy { get; set; }

    [Column("SSO_CrOn", TypeName = "datetime")]
    public DateTime? SsoCrOn { get; set; }

    [Column("SSO_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SsoIpaddress { get; set; }

    [Column("SSO_CompID")]
    public int? SsoCompId { get; set; }
}
