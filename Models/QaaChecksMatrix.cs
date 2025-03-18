using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class QaaChecksMatrix
{
    public int? QamPkid { get; set; }

    public int? QamQapkid { get; set; }

    public int? QamYearId { get; set; }

    public int? QamCustId { get; set; }

    public int? QamFunctionId { get; set; }

    public int? QamSubFunctionId { get; set; }

    public int? QamProcessId { get; set; }

    public int? QamSubProcessId { get; set; }

    public int? QamRiskId { get; set; }

    public int? QamControlId { get; set; }

    public int? QamChecksId { get; set; }

    public int? QamMmmid { get; set; }

    public string? QamStatus { get; set; }

    public string? QamIpaddress { get; set; }

    public int? QamCompId { get; set; }
}
