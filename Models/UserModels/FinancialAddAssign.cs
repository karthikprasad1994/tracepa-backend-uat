using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Financial_AddAssign")]
public partial class FinancialAddAssign
{
    [Column("FAA_ID")]
    public int FaaId { get; set; }

    [Column("FAA_AccHead")]
    public int? FaaAccHead { get; set; }

    [Column("FAA_Head")]
    public int? FaaHead { get; set; }

    [Column("FAA_GL")]
    public int? FaaGl { get; set; }

    [Column("FAA_Parent")]
    public int? FaaParent { get; set; }

    [Column("FAA_GLCode")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FaaGlcode { get; set; }

    [Column("FAA_GLDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FaaGldesc { get; set; }

    [Column("FAA_SGLDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FaaSgldesc { get; set; }

    [Column("FAA_CustID")]
    public int? FaaCustId { get; set; }

    [Column("FAA_IndType")]
    public int? FaaIndType { get; set; }

    [Column("FAA_OBDebit", TypeName = "money")]
    public decimal? FaaObdebit { get; set; }

    [Column("FAA_OBCredit", TypeName = "money")]
    public decimal? FaaObcredit { get; set; }

    [Column("FAA_TrDebit", TypeName = "money")]
    public decimal? FaaTrDebit { get; set; }

    [Column("FAA_TrCredit", TypeName = "money")]
    public decimal? FaaTrCredit { get; set; }

    [Column("FAA_CloseDebit", TypeName = "money")]
    public decimal? FaaCloseDebit { get; set; }

    [Column("FAA_CloseCredit", TypeName = "money")]
    public decimal? FaaCloseCredit { get; set; }

    [Column("FAA_Comments")]
    [StringLength(250)]
    [Unicode(false)]
    public string? FaaComments { get; set; }

    [Column("FAA_Nameoftheperson")]
    public int? FaaNameoftheperson { get; set; }

    [Column("FAA_YearID")]
    public int? FaaYearId { get; set; }

    [Column("FAA_CompID")]
    public int? FaaCompId { get; set; }

    [Column("FAA_Createdby")]
    public int? FaaCreatedby { get; set; }

    [Column("FAA_CreatedOn", TypeName = "datetime")]
    public DateTime? FaaCreatedOn { get; set; }

    [Column("FAA_Operation")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FaaOperation { get; set; }

    [Column("FAA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? FaaIpaddress { get; set; }
}
