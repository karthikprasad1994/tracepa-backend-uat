using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_CUST_LOE")]
public partial class SadCustLoe
{
    [Column("LOE_Id")]
    public int? LoeId { get; set; }

    [Column("LOE_YearId")]
    public int? LoeYearId { get; set; }

    [Column("LOE_CustomerId")]
    public int? LoeCustomerId { get; set; }

    [Column("LOE_ServiceTypeId")]
    public int? LoeServiceTypeId { get; set; }

    [Column("LOE_NatureOfService")]
    [StringLength(200)]
    [Unicode(false)]
    public string? LoeNatureOfService { get; set; }

    [Column("LOE_LocationIds")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LoeLocationIds { get; set; }

    [Column("LOE_Milestones")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LoeMilestones { get; set; }

    [Column("LOE_TimeSchedule", TypeName = "datetime")]
    public DateTime? LoeTimeSchedule { get; set; }

    [Column("LOE_ReportDueDate", TypeName = "datetime")]
    public DateTime? LoeReportDueDate { get; set; }

    [Column("LOE_ProfessionalFees")]
    public int? LoeProfessionalFees { get; set; }

    [Column("LOE_OtherFees")]
    public int? LoeOtherFees { get; set; }

    [Column("LOE_ServiceTax")]
    public int? LoeServiceTax { get; set; }

    [Column("LOE_RembFilingFee")]
    public int? LoeRembFilingFee { get; set; }

    [Column("LOE_CrBy")]
    public int? LoeCrBy { get; set; }

    [Column("LOE_CrOn", TypeName = "datetime")]
    public DateTime? LoeCrOn { get; set; }

    [Column("LOE_Total")]
    public int? LoeTotal { get; set; }

    [Column("LOE_Name")]
    [StringLength(200)]
    [Unicode(false)]
    public string? LoeName { get; set; }

    [Column("LOE_Frequency")]
    public int? LoeFrequency { get; set; }

    [Column("LOE_FunctionId")]
    public int? LoeFunctionId { get; set; }

    [Column("LOE_SubFunctionId")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LoeSubFunctionId { get; set; }

    [Column("LOE_UpdatedOn", TypeName = "datetime")]
    public DateTime? LoeUpdatedOn { get; set; }

    [Column("LOE_UpdatedBy")]
    public int? LoeUpdatedBy { get; set; }

    [Column("LOE_APPROVEDBY")]
    public int? LoeApprovedby { get; set; }

    [Column("LOE_APPROVEDON", TypeName = "datetime")]
    public DateTime? LoeApprovedon { get; set; }

    [Column("LOE_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LoeDelflag { get; set; }

    [Column("LOE_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? LoeStatus { get; set; }

    [Column("LOE_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LoeIpaddress { get; set; }

    [Column("LOE_CompID")]
    public int? LoeCompId { get; set; }
}
