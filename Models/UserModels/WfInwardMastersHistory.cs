using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("WF_Inward_Masters_history")]
public partial class WfInwardMastersHistory
{
    [Column("WIMH_PKID")]
    public int? WimhPkid { get; set; }

    [Column("WIMH_InwardPKID")]
    public int? WimhInwardPkid { get; set; }

    [Column("WIMH_User")]
    public int? WimhUser { get; set; }

    [Column("WIMH_SentTOID")]
    public int? WimhSentToid { get; set; }

    [Column("WIMH_Datetime", TypeName = "datetime")]
    public DateTime? WimhDatetime { get; set; }

    [Column("WIMH_Remarks")]
    [Unicode(false)]
    public string? WimhRemarks { get; set; }

    [Column("WIMH_LineNo")]
    public int? WimhLineNo { get; set; }

    [Column("WIMH_Stage")]
    public int? WimhStage { get; set; }

    [Column("WIMH_CompID")]
    public int? WimhCompId { get; set; }

    [Column("WIMH_ReplyOrForward")]
    public int? WimhReplyOrForward { get; set; }
}
