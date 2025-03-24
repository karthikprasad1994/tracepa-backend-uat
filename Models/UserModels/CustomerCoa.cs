using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Customer_COA")]
public partial class CustomerCoa
{
    [Column("CC_ID")]
    public int CcId { get; set; }

    [Column("CC_AccHead")]
    public int? CcAccHead { get; set; }

    [Column("CC_Head")]
    public int? CcHead { get; set; }

    [Column("CC_GL")]
    public int? CcGl { get; set; }

    [Column("CC_Parent")]
    public int? CcParent { get; set; }

    [Column("CC_GLCode")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CcGlcode { get; set; }

    [Column("CC_GLDesc")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CcGldesc { get; set; }

    [Column("CC_CustID")]
    public int? CcCustId { get; set; }

    [Column("CC_IndType")]
    public int? CcIndType { get; set; }

    [Column("CC_OBDebit", TypeName = "money")]
    public decimal? CcObdebit { get; set; }

    [Column("CC_OBCredit", TypeName = "money")]
    public decimal? CcObcredit { get; set; }

    [Column("CC_TrDebit", TypeName = "money")]
    public decimal? CcTrDebit { get; set; }

    [Column("CC_TrCredit", TypeName = "money")]
    public decimal? CcTrCredit { get; set; }

    [Column("CC_CloseDebit", TypeName = "money")]
    public decimal? CcCloseDebit { get; set; }

    [Column("CC_CloseCredit", TypeName = "money")]
    public decimal? CcCloseCredit { get; set; }

    [Column("CC_YearID")]
    public int? CcYearId { get; set; }

    [Column("CC_CompID")]
    public int? CcCompId { get; set; }

    [Column("CC_Status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CcStatus { get; set; }

    [Column("CC_Createdby")]
    public int? CcCreatedby { get; set; }

    [Column("CC_CreatedOn", TypeName = "datetime")]
    public DateTime? CcCreatedOn { get; set; }

    [Column("CC_Operation")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CcOperation { get; set; }

    [Column("CC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CcIpaddress { get; set; }
}
