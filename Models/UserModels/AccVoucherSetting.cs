using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_Voucher_Settings")]
public partial class AccVoucherSetting
{
    [Column("AVS_Id")]
    public int AvsId { get; set; }

    [Column("AVS_TransType")]
    public int? AvsTransType { get; set; }

    [Column("AVS_Prefix")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AvsPrefix { get; set; }

    [Column("AVS_SNTotal")]
    public int? AvsSntotal { get; set; }

    [Column("AVS_CompId")]
    public int? AvsCompId { get; set; }

    [Column("AVS_CreatedBy")]
    public int? AvsCreatedBy { get; set; }

    [Column("AVS_CreatedOn", TypeName = "datetime")]
    public DateTime? AvsCreatedOn { get; set; }

    [Column("AVS_UpdatedBy")]
    public int? AvsUpdatedBy { get; set; }

    [Column("AVS_UpdatedOn", TypeName = "datetime")]
    public DateTime? AvsUpdatedOn { get; set; }

    [Column("AVS_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AvsOperation { get; set; }

    [Column("AVS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AvsIpaddress { get; set; }
}
