using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadLevelsGeneralMaster
{
    public short MasId { get; set; }

    public string? MasCode { get; set; }

    public string? MasDescription { get; set; }

    public string? MasNotes { get; set; }

    public string? MasDelflag { get; set; }

    public int? MasSortOrder { get; set; }

    public string? MasType { get; set; }

    public short? MasClassify { get; set; }

    public int? MasCompId { get; set; }
}
