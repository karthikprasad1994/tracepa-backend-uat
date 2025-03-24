using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_Trade_Upload")]
public partial class AccTradeUpload
{
    [Column("ATU_ID")]
    public int? AtuId { get; set; }

    [Column("ATU_Name")]
    [StringLength(50)]
    [Unicode(false)]
    public string AtuName { get; set; } = null!;

    [Column("ATU_CustId")]
    public int? AtuCustId { get; set; }

    [Column("ATU_Less_than_six_Month", TypeName = "money")]
    public decimal? AtuLessThanSixMonth { get; set; }

    [Column("ATU_More_than_six_Month", TypeName = "money")]
    public decimal? AtuMoreThanSixMonth { get; set; }

    [Column("ATU_One_Year", TypeName = "money")]
    public decimal? AtuOneYear { get; set; }

    [Column("ATU_Two_Year", TypeName = "money")]
    public decimal? AtuTwoYear { get; set; }

    [Column("ATU_Three_Year", TypeName = "money")]
    public decimal? AtuThreeYear { get; set; }

    [Column("ATU_More_than", TypeName = "money")]
    public decimal? AtuMoreThan { get; set; }

    [Column("ATU_Total_Amount", TypeName = "money")]
    public decimal? AtuTotalAmount { get; set; }

    [Column("ATU_CRON", TypeName = "datetime")]
    public DateTime? AtuCron { get; set; }

    [Column("ATU_CRBY")]
    public int? AtuCrby { get; set; }

    [Column("ATU_APPROVEDBY")]
    public int? AtuApprovedby { get; set; }

    [Column("ATU_APPROVEDON", TypeName = "datetime")]
    public DateTime? AtuApprovedon { get; set; }

    [Column("ATU_UPDATEDBY")]
    public int? AtuUpdatedby { get; set; }

    [Column("ATU_UPDATEDON", TypeName = "datetime")]
    public DateTime? AtuUpdatedon { get; set; }

    [Column("ATU_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtuIpaddress { get; set; }

    [Column("ATU_YEARId")]
    public int? AtuYearid { get; set; }

    [Column("ATU_Branchid")]
    public int? AtuBranchid { get; set; }

    [Column("ATU_Category")]
    public int? AtuCategory { get; set; }

    [Column("ATU_OtherType")]
    public int? AtuOtherType { get; set; }
}
