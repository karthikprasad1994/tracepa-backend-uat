using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_ProfitAndLossAmount")]
public partial class AccProfitAndLossAmount
{
    [Column("Acc_PnL_Pkid")]
    public int AccPnLPkid { get; set; }

    [Column("Acc_PnL_Custid")]
    public int AccPnLCustid { get; set; }

    [Column("Acc_PnL_Amount")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AccPnLAmount { get; set; }

    [Column("Acc_PnL_CrBy")]
    public int? AccPnLCrBy { get; set; }

    [Column("Acc_PnL_CrOn", TypeName = "datetime")]
    public DateTime? AccPnLCrOn { get; set; }

    [Column("Acc_PnL_Flag")]
    public int? AccPnLFlag { get; set; }

    [Column("Acc_PnL_Yearid")]
    public int? AccPnLYearid { get; set; }

    [Column("Acc_PnL_BranchId")]
    [Unicode(false)]
    public string? AccPnLBranchId { get; set; }
}
