using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_DESCRIPTOR_log")]
public partial class EdtDescriptorLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("DES_ID")]
    public int? DesId { get; set; }

    [Column("DESC_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DescName { get; set; }

    [Column("nDESC_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? NDescName { get; set; }

    [Column("DESC_NOTE")]
    [StringLength(200)]
    [Unicode(false)]
    public string? DescNote { get; set; }

    [Column("nDESC_NOTE")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NDescNote { get; set; }

    [Column("DESC_DATATYPE")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DescDatatype { get; set; }

    [Column("nDESC_DATATYPE")]
    [StringLength(25)]
    [Unicode(false)]
    public string? NDescDatatype { get; set; }

    [Column("DESC_SIZE")]
    [StringLength(3)]
    [Unicode(false)]
    public string? DescSize { get; set; }

    [Column("nDESC_SIZE")]
    [StringLength(3)]
    [Unicode(false)]
    public string? NDescSize { get; set; }

    [Column("DESC_DefaultValues")]
    [StringLength(250)]
    [Unicode(false)]
    public string? DescDefaultValues { get; set; }

    [Column("nDESC_DefaultValues")]
    [StringLength(250)]
    [Unicode(false)]
    public string? NDescDefaultValues { get; set; }

    [Column("DESC_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DescStatus { get; set; }

    [Column("nDESC_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NDescStatus { get; set; }

    [Column("DESC_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DescDelFlag { get; set; }

    [Column("nDESC_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? NDescDelFlag { get; set; }

    [Column("DESC_CompId")]
    public int? DescCompId { get; set; }

    [Column("DESC_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DescIpaddress { get; set; }
}
