using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUsrOrGrpPermissionDgo
{
    public int? SgpId { get; set; }

    public int? SgpModId { get; set; }

    public string? SgpLevelGroup { get; set; }

    public int? SgpLevelGroupId { get; set; }

    public int? SgpView { get; set; }

    public int? SgpSaveOrUpdate { get; set; }

    public int? SgpActiveOrDeactive { get; set; }

    public int? SgpReport { get; set; }

    public int? SgpCreatedBy { get; set; }

    public DateTime? SgpCreatedOn { get; set; }

    public int? SgpApprovedBy { get; set; }

    public DateTime? SgpApprovedOn { get; set; }

    public int? SgpUpdatedBy { get; set; }

    public DateTime? SgpUpdatedOn { get; set; }

    public string? SgpStatus { get; set; }

    public string? SgpDelFlag { get; set; }

    public int? SgpCompId { get; set; }

    public int? SgpDownload { get; set; }

    public int? SgpAnnotaion { get; set; }
}
