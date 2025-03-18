using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtPageLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public decimal? PgeBasename { get; set; }

    public decimal? PgeCabinet { get; set; }

    public decimal? PgeFolder { get; set; }

    public decimal? PgeDocumentType { get; set; }

    public string? PgeTitle { get; set; }

    public DateTime? PgeDate { get; set; }

    public decimal? PgeDetailsId { get; set; }

    public string? PgeObject { get; set; }

    public decimal? PgePageno { get; set; }

    public string? PgeExt { get; set; }

    public string? PgeKeyWord { get; set; }

    public string? PgeOcrtext { get; set; }

    public decimal? PgeSize { get; set; }

    public decimal? PgeCurrentVer { get; set; }

    public string? PgeStatus { get; set; }

    public decimal? PgeSubCabinet { get; set; }

    public int? PgeQcUsrGrpId { get; set; }

    public string? PgeFtpstatus { get; set; }

    public string? PgeBatchName { get; set; }

    public string? PgeOrignalFileName { get; set; }

    public int? PgeBatchId { get; set; }

    public int? PgeOcrdelFlag { get; set; }

    public int? PgeCompId { get; set; }

    public string? PgeDelflag { get; set; }

    public int? PgeCreatedBy { get; set; }

    public DateTime? PgeCreatedOn { get; set; }

    public int? PgeUpdatedBy { get; set; }

    public DateTime? PgeUpdatedOn { get; set; }
}
