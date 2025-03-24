using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_Annotation_Details")]
public partial class EdtAnnotationDetail
{
    [Column("EAD_PKID")]
    public int? EadPkid { get; set; }

    [Column("EAD_DocumentID")]
    public int? EadDocumentId { get; set; }

    [Column("EAD_FileID")]
    public int? EadFileId { get; set; }

    [Column("EAD_OriginalName")]
    [StringLength(200)]
    [Unicode(false)]
    public string? EadOriginalName { get; set; }

    [Column("EAD_EXT")]
    [StringLength(20)]
    [Unicode(false)]
    public string? EadExt { get; set; }

    [Column("EAD_OLE")]
    [MaxLength(1)]
    public byte[]? EadOle { get; set; }

    [Column("EAD_SIZE")]
    public long? EadSize { get; set; }

    [Column("EAD_CreatedBy")]
    public int? EadCreatedBy { get; set; }

    [Column("EAD_CreatedOn", TypeName = "datetime")]
    public DateTime? EadCreatedOn { get; set; }

    [Column("EAD_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? EadIpaddress { get; set; }

    [Column("EAD_CompID")]
    public int? EadCompId { get; set; }
}
