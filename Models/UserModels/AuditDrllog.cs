using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_DRLLog")]
public partial class AuditDrllog
{
    [Column("ADRL_ID")]
    public int? AdrlId { get; set; }

    [Column("ADRL_YearID")]
    public int? AdrlYearId { get; set; }

    [Column("ADRL_AuditNo")]
    public int? AdrlAuditNo { get; set; }

    [Column("ADRL_FunID")]
    public int? AdrlFunId { get; set; }

    [Column("ADRL_CustID")]
    public int? AdrlCustId { get; set; }

    [Column("ADRL_RequestedListID")]
    public int? AdrlRequestedListId { get; set; }

    [Column("ADRL_RequestedTypeID")]
    public int? AdrlRequestedTypeId { get; set; }

    [Column("ADRL_RequestedOn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AdrlRequestedOn { get; set; }

    [Column("ADRL_EmailID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AdrlEmailId { get; set; }

    [Column("ADRL_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AdrlComments { get; set; }

    [Column("ADRL_ReceivedComments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AdrlReceivedComments { get; set; }

    [Column("ADRL_ReceivedOn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AdrlReceivedOn { get; set; }

    [Column("ADRL_LogStatus")]
    public int? AdrlLogStatus { get; set; }

    [Column("ADRL_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AdrlStatus { get; set; }

    [Column("ADRL_AttachID")]
    public int? AdrlAttachId { get; set; }

    [Column("ADRL_CrBy")]
    public int? AdrlCrBy { get; set; }

    [Column("ADRL_CrOn", TypeName = "datetime")]
    public DateTime? AdrlCrOn { get; set; }

    [Column("ADRL_UpdatedBy")]
    public int? AdrlUpdatedBy { get; set; }

    [Column("ADRL_UpdatedOn", TypeName = "datetime")]
    public DateTime? AdrlUpdatedOn { get; set; }

    [Column("ADRL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AdrlIpaddress { get; set; }

    [Column("ADRL_CompID")]
    public int? AdrlCompId { get; set; }

    [Column("ADRL_TimlinetoResOn")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AdrlTimlinetoResOn { get; set; }
}
