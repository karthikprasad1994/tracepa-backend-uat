using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class DocumentTray
{
    public int? PkId { get; set; }

    public int? DocId { get; set; }

    public int? DocParent { get; set; }

    public string? Title { get; set; }

    public string? DocFormat { get; set; }

    public string? DocBytes { get; set; }

    public string? Delflag { get; set; }

    public int? CrBy { get; set; }

    public DateTime? CrOn { get; set; }

    public int? Compid { get; set; }
}
