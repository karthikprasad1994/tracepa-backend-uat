using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("QAA_ChecksMatrix")]
public partial class QaaChecksMatrix
{
    [Column("QAM_PKID")]
    public int? QamPkid { get; set; }

    [Column("QAM_QAPKID")]
    public int? QamQapkid { get; set; }

    [Column("QAM_YearID")]
    public int? QamYearId { get; set; }

    [Column("QAM_CustID")]
    public int? QamCustId { get; set; }

    [Column("QAM_FunctionID")]
    public int? QamFunctionId { get; set; }

    [Column("QAM_SubFunctionID")]
    public int? QamSubFunctionId { get; set; }

    [Column("QAM_ProcessID")]
    public int? QamProcessId { get; set; }

    [Column("QAM_SubProcessID")]
    public int? QamSubProcessId { get; set; }

    [Column("QAM_RiskID")]
    public int? QamRiskId { get; set; }

    [Column("QAM_ControlID")]
    public int? QamControlId { get; set; }

    [Column("QAM_ChecksID")]
    public int? QamChecksId { get; set; }

    [Column("QAM_MMMID")]
    public int? QamMmmid { get; set; }

    [Column("QAM_Status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? QamStatus { get; set; }

    [Column("QAM_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? QamIpaddress { get; set; }

    [Column("QAM_CompID")]
    public int? QamCompId { get; set; }
}
