using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_EMailSent_Details")]
public partial class GraceEmailSentDetail
{
    [Column("EMD_ID")]
    public int? EmdId { get; set; }

    [Column("EMD_MstPKID")]
    public int? EmdMstPkid { get; set; }

    [Column("EMD_YearID")]
    public int? EmdYearId { get; set; }

    [Column("EMD_FormName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EmdFormName { get; set; }

    [Column("EMD_FromEmailID")]
    [Unicode(false)]
    public string? EmdFromEmailId { get; set; }

    [Column("EMD_ToEmailIDs")]
    [Unicode(false)]
    public string? EmdToEmailIds { get; set; }

    [Column("EMD_CCEmailIDs")]
    [Unicode(false)]
    public string? EmdCcemailIds { get; set; }

    [Column("EMD_Subject")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? EmdSubject { get; set; }

    [Column("EMD_Body")]
    [Unicode(false)]
    public string? EmdBody { get; set; }

    [Column("EMD_EMailStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? EmdEmailStatus { get; set; }

    [Column("EMD_SentUsrID")]
    public int? EmdSentUsrId { get; set; }

    [Column("EMD_SentOn", TypeName = "datetime")]
    public DateTime? EmdSentOn { get; set; }

    [Column("EMD_CreatedBy")]
    public int? EmdCreatedBy { get; set; }

    [Column("EMD_CreatedOn", TypeName = "datetime")]
    public DateTime? EmdCreatedOn { get; set; }

    [Column("EMD_AttachedPath")]
    [Unicode(false)]
    public string? EmdAttachedPath { get; set; }

    [Column("EMD_AttachedDocIDs")]
    [Unicode(false)]
    public string? EmdAttachedDocIds { get; set; }

    [Column("EMD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? EmdIpaddress { get; set; }

    [Column("EMD_CompID")]
    public int? EmdCompId { get; set; }
}
