using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_folder_rights")]
public partial class EdtFolderRight
{
    [Column("FER_FolderID", TypeName = "numeric(5, 0)")]
    public decimal? FerFolderId { get; set; }

    [Column("FER_DoctypeID", TypeName = "numeric(5, 0)")]
    public decimal? FerDoctypeId { get; set; }

    [Column("FER_UserID", TypeName = "numeric(4, 0)")]
    public decimal? FerUserId { get; set; }

    [Column("FER_Index")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FerIndex { get; set; }

    [Column("FER_Search")]
    [StringLength(1)]
    [Unicode(false)]
    public string? FerSearch { get; set; }
}
