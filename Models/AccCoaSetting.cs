using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccCoaSetting
{
    public int AcsId { get; set; }

    public int? AcsAccHead { get; set; }

    public string? AcsAccHeadPrefix { get; set; }

    public int? AcsGroup { get; set; }

    public int? AcsSubGroup { get; set; }

    public int? AcsGl { get; set; }

    public int? AcsSubGl { get; set; }

    public int? AcsCompId { get; set; }

    public string? AcsIpaddress { get; set; }

    public string? AcsOperation { get; set; }

    public int? AcsCreatedBy { get; set; }

    public DateTime? AcsCreatedOn { get; set; }

    public int? AcsUpdatedBy { get; set; }

    public DateTime? AcsUpdatedOn { get; set; }
}
