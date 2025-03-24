using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ADO_Batch")]
public partial class AdoBatch
{
    [Column("BT_ID")]
    public int? BtId { get; set; }

    [Column("BT_CustomerID")]
    public int? BtCustomerId { get; set; }

    [Column("BT_TransactionType")]
    public int? BtTransactionType { get; set; }

    [Column("BT_BatchID")]
    public int? BtBatchId { get; set; }

    [Column("BT_BatchNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BtBatchNo { get; set; }

    [Column("BT_AttachID")]
    public int? BtAttachId { get; set; }

    [Column("BT_Title")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BtTitle { get; set; }

    [Column("BT_NFT")]
    public int? BtNft { get; set; }

    [Column("BT_Vouchers")]
    [StringLength(50)]
    [Unicode(false)]
    public string? BtVouchers { get; set; }

    [Column("BT_Date", TypeName = "datetime")]
    public DateTime? BtDate { get; set; }

    [Column("BT_Comments")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BtComments { get; set; }

    [Column("BT_DebitTotal", TypeName = "money")]
    public decimal? BtDebitTotal { get; set; }

    [Column("BT_CreditTotal", TypeName = "money")]
    public decimal? BtCreditTotal { get; set; }

    [Column("BT_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BtDelflag { get; set; }

    [Column("BT_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BtStatus { get; set; }

    [Column("BT_CompID")]
    public int? BtCompId { get; set; }

    [Column("BT_YearID")]
    public int? BtYearId { get; set; }

    [Column("BT_CrBy")]
    public int? BtCrBy { get; set; }

    [Column("BT_CrOn", TypeName = "datetime")]
    public DateTime? BtCrOn { get; set; }

    [Column("BT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BtIpaddress { get; set; }
}
