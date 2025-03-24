using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_Folder_Permission")]
public partial class EdtFolderPermission
{
    [Column("EFP_ID", TypeName = "numeric(10, 0)")]
    public decimal? EfpId { get; set; }

    [Column("EFP_PTYPE")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EfpPtype { get; set; }

    [Column("EFP_GRPID")]
    public short EfpGrpid { get; set; }

    [Column("EFP_USRID")]
    public short EfpUsrid { get; set; }

    [Column("EFP_INDEX")]
    public byte? EfpIndex { get; set; }

    [Column("EFP_SEARCH")]
    public byte? EfpSearch { get; set; }

    [Column("EFP_MOD_FOLDER")]
    public byte? EfpModFolder { get; set; }

    [Column("EFP_MOD_DOC")]
    public byte? EfpModDoc { get; set; }

    [Column("EFP_DEL_FOLDER")]
    public byte? EfpDelFolder { get; set; }

    [Column("EFP_DEL_DOC")]
    public byte? EfpDelDoc { get; set; }

    [Column("EFP_EXPORT")]
    public byte? EfpExport { get; set; }

    [Column("EFP_OTHER")]
    public byte? EfpOther { get; set; }

    [Column("EFP_CRT_DOC")]
    public byte? EfpCrtDoc { get; set; }

    [Column("EFP_VIEW_Fol")]
    public byte? EfpViewFol { get; set; }

    [Column("EFP_FolId", TypeName = "numeric(18, 0)")]
    public decimal? EfpFolId { get; set; }
}
