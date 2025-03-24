using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Uploaded_Shared_Documents")]
public partial class UploadedSharedDocument
{
    [Column("USD_PKID")]
    public int? UsdPkid { get; set; }

    [Column("USD_FName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsdFname { get; set; }

    [Column("USD_Ext")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UsdExt { get; set; }

    [Column("USD_Size")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? UsdSize { get; set; }

    [Column("USD_UploadedDocType")]
    public int? UsdUploadedDocType { get; set; }

    [Column("USD_CreatedBy")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsdCreatedBy { get; set; }

    [Column("USD_CreatedOn", TypeName = "datetime")]
    public DateTime? UsdCreatedOn { get; set; }

    [Column("USD_UpdatedBy")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsdUpdatedBy { get; set; }

    [Column("USD_UpdatedOn", TypeName = "datetime")]
    public DateTime? UsdUpdatedOn { get; set; }

    [Column("USD_DeletedBy")]
    public int? UsdDeletedBy { get; set; }

    [Column("USD_DeletedOn", TypeName = "datetime")]
    public DateTime? UsdDeletedOn { get; set; }

    [Column("USD_RecalledBy")]
    public int? UsdRecalledBy { get; set; }

    [Column("USD_RecalledOn", TypeName = "datetime")]
    public DateTime? UsdRecalledOn { get; set; }

    [Column("USD_SharedWith")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? UsdSharedWith { get; set; }

    [Column("USD_SharedOn", TypeName = "datetime")]
    public DateTime? UsdSharedOn { get; set; }

    [Column("USD_YearID")]
    public int? UsdYearId { get; set; }

    [Column("USD_Status")]
    [StringLength(10)]
    [Unicode(false)]
    public string? UsdStatus { get; set; }

    [Column("USD_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? UsdIpaddress { get; set; }

    [Column("USD_CompID")]
    public int? UsdCompId { get; set; }
}
