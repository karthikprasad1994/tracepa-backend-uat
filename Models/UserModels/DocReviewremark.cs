using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Doc_Reviewremarks")]
public partial class DocReviewremark
{
    [Column("DR_ID")]
    public int? DrId { get; set; }

    [Column("DR_Custid")]
    public int? DrCustid { get; set; }

    [Column("DR_DocLoeId_Branchid")]
    public int? DrDocLoeIdBranchid { get; set; }

    [Column("DR_DocYearid")]
    public int? DrDocYearid { get; set; }

    [Column("DR_DocStatus")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DrDocStatus { get; set; }

    [Column("DR_DocType")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DrDocType { get; set; }

    [Column("DR_Date", TypeName = "datetime")]
    public DateTime? DrDate { get; set; }

    [Column("DR_CreatedBy")]
    public int? DrCreatedBy { get; set; }

    [Column("DR_CreatedOn", TypeName = "datetime")]
    public DateTime? DrCreatedOn { get; set; }

    [Column("DR_UpdatedBy")]
    public int? DrUpdatedBy { get; set; }

    [Column("DR_UpdatedOn", TypeName = "datetime")]
    public DateTime? DrUpdatedOn { get; set; }

    [Column("DR_CompId")]
    public int? DrCompId { get; set; }

    [Column("DR_emailSentTo")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DrEmailSentTo { get; set; }

    [Column("DR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DrIpaddress { get; set; }

    [Column("DR_Observation")]
    [Unicode(false)]
    public string? DrObservation { get; set; }

    [Column("DR_DocDelflag")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrDocDelflag { get; set; }
}
