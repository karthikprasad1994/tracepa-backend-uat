using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtFolderPermission
{
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
