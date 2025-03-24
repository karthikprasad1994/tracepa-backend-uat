using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_Statutory_PartnerDetails")]
public partial class SadStatutoryPartnerDetail
{
    [Column("SSP_Id")]
    public int? SspId { get; set; }

    [Column("SSP_CustID")]
    public int? SspCustId { get; set; }

    [Column("SSP_PartnerName")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SspPartnerName { get; set; }

    [Column("SSP_PAN")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SspPan { get; set; }

    [Column("SSP_DOJ", TypeName = "datetime")]
    public DateTime? SspDoj { get; set; }

    [Column("SSP_ShareOfProfit", TypeName = "decimal(19, 2)")]
    public decimal? SspShareOfProfit { get; set; }

    [Column("SSP_CapitalAmount", TypeName = "decimal(19, 2)")]
    public decimal? SspCapitalAmount { get; set; }

    [Column("SSP_CRON", TypeName = "datetime")]
    public DateTime? SspCron { get; set; }

    [Column("SSP_CRBY")]
    public int? SspCrby { get; set; }

    [Column("SSP_UpdatedOn", TypeName = "datetime")]
    public DateTime? SspUpdatedOn { get; set; }

    [Column("SSP_UpdatedBy")]
    public int? SspUpdatedBy { get; set; }

    [Column("SSP_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? SspDelFlag { get; set; }

    [Column("SSP_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SspStatus { get; set; }

    [Column("SSP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SspIpaddress { get; set; }

    [Column("SSP_CompID")]
    public int? SspCompId { get; set; }
}
