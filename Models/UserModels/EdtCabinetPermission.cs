using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_cabinet_Permission")]
public partial class EdtCabinetPermission
{
    [Column("CBP_ID")]
    public int? CbpId { get; set; }

    [Column("CBP_PermissionType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CbpPermissionType { get; set; }

    [Column("CBP_Cabinet")]
    public int? CbpCabinet { get; set; }

    [Column("CBP_User")]
    public int? CbpUser { get; set; }

    [Column("CBP_Department")]
    public int? CbpDepartment { get; set; }

    [Column("CBP_View")]
    public int? CbpView { get; set; }

    [Column("CBP_Create")]
    public int? CbpCreate { get; set; }

    [Column("CBP_Modify")]
    public int? CbpModify { get; set; }

    [Column("CBP_Delete")]
    public int? CbpDelete { get; set; }

    [Column("CBP_Search")]
    public int? CbpSearch { get; set; }

    [Column("CBP_Index")]
    public int? CbpIndex { get; set; }

    [Column("CBP_Other")]
    public int? CbpOther { get; set; }

    [Column("CBP_CreateFolder")]
    public int? CbpCreateFolder { get; set; }
}
