using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_Opening_Balance1")]
public partial class AccOpeningBalance1
{
    [Column("Opn_Id")]
    public int OpnId { get; set; }

    [Column("Opn_SerialNo")]
    public int OpnSerialNo { get; set; }

    [Column("Opn_Date", TypeName = "datetime")]
    public DateTime? OpnDate { get; set; }

    [Column("Opn_AccHead")]
    public int OpnAccHead { get; set; }

    [Column("Opn_GLCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OpnGlcode { get; set; }

    [Column("Opn_DebitAmt", TypeName = "money")]
    public decimal? OpnDebitAmt { get; set; }

    [Column("Opn_CreditAmount", TypeName = "money")]
    public decimal? OpnCreditAmount { get; set; }

    [Column("Opn_YearId")]
    public int OpnYearId { get; set; }

    [Column("Opn_CreatedBy")]
    public int OpnCreatedBy { get; set; }

    [Column("Opn_CreatedOn", TypeName = "datetime")]
    public DateTime? OpnCreatedOn { get; set; }

    [Column("Opn_ApprovedBy")]
    public int OpnApprovedBy { get; set; }

    [Column("Opn_ApprovedOn", TypeName = "datetime")]
    public DateTime? OpnApprovedOn { get; set; }

    [Column("Opn_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? OpnStatus { get; set; }

    [Column("Opn_CompId")]
    public int? OpnCompId { get; set; }

    [Column("Opn_GlId")]
    public int? OpnGlId { get; set; }

    [Column("Opn_CustType")]
    public int OpnCustType { get; set; }

    [Column("Opn_IndType")]
    public int OpnIndType { get; set; }

    [Column("Opn_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? OpnIpaddress { get; set; }

    [Column("Opn_Operation")]
    [StringLength(20)]
    [Unicode(false)]
    public string? OpnOperation { get; set; }

    [Column("Opn_ClosingBalanceDebit", TypeName = "money")]
    public decimal? OpnClosingBalanceDebit { get; set; }

    [Column("Opn_ClosingBalanceCredit", TypeName = "money")]
    public decimal? OpnClosingBalanceCredit { get; set; }

    [Column("Opn_UpdatedBy")]
    public int? OpnUpdatedBy { get; set; }

    [Column("Opn_UpdatedOn", TypeName = "datetime")]
    public DateTime? OpnUpdatedOn { get; set; }
}
