using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Content_Management_Master_log")]
public partial class ContentManagementMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("CMM_ID")]
    public int? CmmId { get; set; }

    [Column("CMM_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CmmCode { get; set; }

    [Column("nCMM_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NCmmCode { get; set; }

    [Column("CMM_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmmDesc { get; set; }

    [Column("nCMM_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? NCmmDesc { get; set; }

    [Column("CMM_Category")]
    [StringLength(3)]
    [Unicode(false)]
    public string? CmmCategory { get; set; }

    [Column("nCMM_Category")]
    [StringLength(3)]
    [Unicode(false)]
    public string? NCmmCategory { get; set; }

    [Column("CMM_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CmmRemarks { get; set; }

    [Column("nCMM_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NCmmRemarks { get; set; }

    [Column("CMM_KeyComponent")]
    public int? CmmKeyComponent { get; set; }

    [Column("nCMM_KeyComponent")]
    public int? NCmmKeyComponent { get; set; }

    [Column("CMM_Module")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmmModule { get; set; }

    [Column("nCMM_Module")]
    [StringLength(1)]
    [Unicode(false)]
    public string? NCmmModule { get; set; }

    [Column("CMM_RiskCategory")]
    public int? CmmRiskCategory { get; set; }

    [Column("nCMM_RiskCategory")]
    public int? NCmmRiskCategory { get; set; }

    [Column("CMM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmmIpaddress { get; set; }

    [Column("CMM_CompID")]
    public int? CmmCompId { get; set; }
}
