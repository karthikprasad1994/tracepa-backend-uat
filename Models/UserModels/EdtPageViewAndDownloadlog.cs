using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_PAGE_ViewAndDownloadlogs")]
public partial class EdtPageViewAndDownloadlog
{
    [Column("PVD_PKID")]
    public int? PvdPkid { get; set; }

    [Column("PVD_LogOperation")]
    [StringLength(50)]
    [Unicode(false)]
    public string? PvdLogOperation { get; set; }

    [Column("PVD_PageBaseID")]
    public int? PvdPageBaseId { get; set; }

    [Column("PVD_PageDetailsID")]
    public int? PvdPageDetailsId { get; set; }

    [Column("PVD_Cabinet")]
    public int? PvdCabinet { get; set; }

    [Column("PVD_SubCabinet")]
    public int? PvdSubCabinet { get; set; }

    [Column("PVD_Folder")]
    public int? PvdFolder { get; set; }

    [Column("PVD_DocumentType")]
    public int? PvdDocumentType { get; set; }

    [Column("PVD_Version")]
    public int? PvdVersion { get; set; }

    [Column("PVD_UserId")]
    public int? PvdUserId { get; set; }

    [Column("PVD_DepId")]
    public int? PvdDepId { get; set; }

    [Column("PVD_Date", TypeName = "datetime")]
    public DateTime? PvdDate { get; set; }

    [Column("PVD_Ipaddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? PvdIpaddress { get; set; }

    [Column("PVD_CompId")]
    public int? PvdCompId { get; set; }
}
