using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_CUSTOMER_MASTER2")]
public partial class SadCustomerMaster2
{
    [Column("CUST_ID")]
    public int? CustId { get; set; }

    [Column("CUST_NAME")]
    [StringLength(150)]
    [Unicode(false)]
    public string? CustName { get; set; }

    [Column("CUST_CODE")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustCode { get; set; }

    [Column("CUST_WEBSITE")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustWebsite { get; set; }

    [Column("CUST_EMAIL")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustEmail { get; set; }

    [Column("CUST_GROUPNAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustGroupname { get; set; }

    [Column("CUST_GROUPINDIVIDUAL")]
    public int? CustGroupindividual { get; set; }

    [Column("CUST_ORGTYPEID")]
    public short? CustOrgtypeid { get; set; }

    [Column("CUST_INDTYPEID")]
    public short? CustIndtypeid { get; set; }

    [Column("CUST_MGMTTYPEID")]
    public short? CustMgmttypeid { get; set; }

    [Column("CUST_CommitmentDate", TypeName = "datetime")]
    public DateTime? CustCommitmentDate { get; set; }

    [Column("CUSt_BranchId")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustBranchId { get; set; }

    [Column("CUST_COMM_ADDRESS")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? CustCommAddress { get; set; }

    [Column("CUST_COMM_CITY")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustCommCity { get; set; }

    [Column("CUST_COMM_PIN")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CustCommPin { get; set; }

    [Column("CUST_COMM_STATE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustCommState { get; set; }

    [Column("CUST_COMM_COUNTRY")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustCommCountry { get; set; }

    [Column("CUST_COMM_FAX")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustCommFax { get; set; }

    [Column("CUST_COMM_TEL")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustCommTel { get; set; }

    [Column("CUST_COMM_Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustCommEmail { get; set; }

    [Column("CUST_ADDRESS")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? CustAddress { get; set; }

    [Column("CUST_CITY")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustCity { get; set; }

    [Column("CUST_PIN")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CustPin { get; set; }

    [Column("CUST_STATE")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustState { get; set; }

    [Column("CUST_COUNTRY")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CustCountry { get; set; }

    [Column("CUST_FAX")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustFax { get; set; }

    [Column("CUST_TELPHONE")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustTelphone { get; set; }

    [Column("CUST_ConEmailID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustConEmailId { get; set; }

    [Column("CUST_LOCATIONID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustLocationid { get; set; }

    [Column("CUST_TASKS")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CustTasks { get; set; }

    [Column("CUST_ORGID")]
    public int? CustOrgid { get; set; }

    [Column("CUST_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CustDelflg { get; set; }

    [Column("CUST_CRBY")]
    public int? CustCrby { get; set; }

    [Column("CUST_CRON", TypeName = "datetime")]
    public DateTime? CustCron { get; set; }

    [Column("CUST_UpdatedBy")]
    public int? CustUpdatedBy { get; set; }

    [Column("CUST_UpdatedOn", TypeName = "datetime")]
    public DateTime? CustUpdatedOn { get; set; }

    [Column("CUST_DeletedBy")]
    public int? CustDeletedBy { get; set; }

    [Column("CUST_DeletedOn", TypeName = "datetime")]
    public DateTime? CustDeletedOn { get; set; }

    [Column("CUST_RecallBy")]
    public int? CustRecallBy { get; set; }

    [Column("CUST_RecallOn", TypeName = "datetime")]
    public DateTime? CustRecallOn { get; set; }

    [Column("CUST_APPROVEDBY")]
    public int? CustApprovedby { get; set; }

    [Column("CUST_APPROVEDON", TypeName = "datetime")]
    public DateTime? CustApprovedon { get; set; }

    [Column("CUST_STATUS")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CustStatus { get; set; }

    [Column("CUST_BOARDOFDIRECTORS")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CustBoardofdirectors { get; set; }

    [Column("CUST_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CustIpaddress { get; set; }

    [Column("CUST_CompID")]
    public int? CustCompId { get; set; }
}
