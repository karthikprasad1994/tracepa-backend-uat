using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtCabinetPermission
{
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
