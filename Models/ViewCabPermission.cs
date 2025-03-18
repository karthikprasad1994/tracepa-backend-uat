using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ViewCabPermission
{
    public int? CbnId { get; set; }

    public string? CbnName { get; set; }

    public int? CbnParent { get; set; }

    public string? CbnNote { get; set; }

    public int? CbnUserId { get; set; }

    public int? CbnDepartment { get; set; }

    public int? CbnSubCabCount { get; set; }

    public int? CbnFolderCount { get; set; }

    public int? CbnCreatedBy { get; set; }

    public DateTime? CbnCreatedOn { get; set; }

    public int? CbnUpdatedBy { get; set; }

    public DateTime? CbnUpdatedOn { get; set; }

    public int? CbnApprovedBy { get; set; }

    public DateTime? CbnApprovedOn { get; set; }

    public int? CbnDeletedBy { get; set; }

    public DateTime? CbnDeletedOn { get; set; }

    public int? CbnRecalledBy { get; set; }

    public DateTime? CbnRecalledOn { get; set; }

    public string? CbnStatus { get; set; }

    public string? CbnDelFlag { get; set; }

    public int? CbnCompId { get; set; }

    public int? CbpId { get; set; }

    public string? CbpPermissionType { get; set; }

    public int? CbpCabinet { get; set; }

    public int? CbpUser { get; set; }

    public int? CbpDepartment { get; set; }

    public int? CbpView { get; set; }

    public int? CbpCreate { get; set; }

    public int? CbpModify { get; set; }

    public int? CbpDelete { get; set; }

    public int? CbpSearch { get; set; }

    public int? CbpIndex { get; set; }

    public int? CbpOther { get; set; }

    public int? CbpCreateFolder { get; set; }
}
