using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtPageViewAndDownloadlog
{
    public int? PvdPkid { get; set; }

    public string? PvdLogOperation { get; set; }

    public int? PvdPageBaseId { get; set; }

    public int? PvdPageDetailsId { get; set; }

    public int? PvdCabinet { get; set; }

    public int? PvdSubCabinet { get; set; }

    public int? PvdFolder { get; set; }

    public int? PvdDocumentType { get; set; }

    public int? PvdVersion { get; set; }

    public int? PvdUserId { get; set; }

    public int? PvdDepId { get; set; }

    public DateTime? PvdDate { get; set; }

    public string? PvdIpaddress { get; set; }

    public int? PvdCompId { get; set; }
}
