using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ViewFolPermission
{
    public int? FolFolid { get; set; }

    public int? FolCabinet { get; set; }

    public string? FolName { get; set; }

    public string? FolNote { get; set; }

    public int? FolCreatedBy { get; set; }

    public DateTime? FolCreatedOn { get; set; }

    public string? FolStatus { get; set; }

    public string? FolDelFlag { get; set; }

    public decimal? EfpId { get; set; }

    public string? EfpPtype { get; set; }

    public short EfpGrpid { get; set; }

    public short EfpUsrid { get; set; }

    public byte? EfpIndex { get; set; }

    public byte? EfpSearch { get; set; }

    public byte? EfpModFolder { get; set; }

    public byte? EfpModDoc { get; set; }

    public byte? EfpDelFolder { get; set; }

    public byte? EfpDelDoc { get; set; }

    public byte? EfpExport { get; set; }

    public byte? EfpOther { get; set; }

    public byte? EfpCrtDoc { get; set; }

    public byte? EfpViewFol { get; set; }

    public decimal? EfpFolId { get; set; }
}
