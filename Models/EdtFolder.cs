using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtFolder
{
    public int? FolFolId { get; set; }

    public string? FolName { get; set; }

    public string? FolNote { get; set; }

    public int? FolCabinet { get; set; }

    public int? FolCreatedBy { get; set; }

    public DateTime? FolCreatedOn { get; set; }

    public int? FolUpdatedBy { get; set; }

    public DateTime? FolUpdatedOn { get; set; }

    public int? FolApprovedBy { get; set; }

    public DateTime? FolApprovedOn { get; set; }

    public int? FolDeletedBy { get; set; }

    public DateTime? FolDeletedOn { get; set; }

    public int? FolRecalledBy { get; set; }

    public DateTime? FolRecalledOn { get; set; }

    public string? FolStatus { get; set; }

    public string? FolDelFlag { get; set; }

    public int? FolCompId { get; set; }
}
