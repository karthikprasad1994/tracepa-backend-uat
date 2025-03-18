using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditExcelUpload
{
    public int AeuId { get; set; }

    public string? AeuDescription { get; set; }

    public int? AeuCustId { get; set; }

    public decimal? AeuOdamount { get; set; }

    public decimal? AeuOcamount { get; set; }

    public decimal? AeuTrdamount { get; set; }

    public decimal? AeuTrcamount { get; set; }

    public decimal? AeuCdamount { get; set; }

    public decimal? AeuCcamount { get; set; }

    public string? AeuObservation { get; set; }

    public string? AeuReviewerObservation { get; set; }

    public string? AeuClientComments { get; set; }

    public string? AeuDelflg { get; set; }

    public DateTime? AeuCron { get; set; }

    public int? AeuCrby { get; set; }

    public int? AeuApprovedby { get; set; }

    public DateTime? AeuApprovedon { get; set; }

    public string? AeuStatus { get; set; }

    public int? AeuUpdatedby { get; set; }

    public DateTime? AeuUpdatedon { get; set; }

    public int? AeuDeletedby { get; set; }

    public DateTime? AeuDeletedon { get; set; }

    public int? AeuRecallby { get; set; }

    public DateTime? AeuRecallon { get; set; }

    public string? AeuIpaddress { get; set; }

    public int? AeuCompId { get; set; }

    public int? AeuYearid { get; set; }

    public int? AeuAuditId { get; set; }

    public int? AeuAuditTypeId { get; set; }

    public int? AeuAttachmentId { get; set; }
}
