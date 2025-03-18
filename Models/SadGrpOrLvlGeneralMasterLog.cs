using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadGrpOrLvlGeneralMasterLog
{
    public long LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public short? MasId { get; set; }

    public string? MasCode { get; set; }

    public string? NMasCode { get; set; }

    public string? MasDescription { get; set; }

    public string? NMasDescription { get; set; }

    public string? MasNotes { get; set; }

    public string? NMasNotes { get; set; }

    public string? MasCompid { get; set; }

    public string? MasIpaddress { get; set; }
}
