using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_Checklist_Mas")]
public partial class ComplianceChecklistMa
{
    [Column("CRCM_ID")]
    public int? CrcmId { get; set; }

    [Column("CRCM_CustID")]
    public int? CrcmCustId { get; set; }

    [Column("CRCM_FunID")]
    public int? CrcmFunId { get; set; }

    [Column("CRCM_JobID")]
    public int? CrcmJobId { get; set; }

    [Column("CRCM_AttchID")]
    public int? CrcmAttchId { get; set; }

    [Column("CRCM_YearID")]
    public int? CrcmYearId { get; set; }

    [Column("CRCM_Operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CrcmOperation { get; set; }

    [Column("CRCM_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CrcmIpaddress { get; set; }

    [Column("CRCM_Status")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CrcmStatus { get; set; }

    [Column("CRCM_CrBy")]
    public int? CrcmCrBy { get; set; }

    [Column("CRCM_CrOn", TypeName = "datetime")]
    public DateTime? CrcmCrOn { get; set; }

    [Column("CRCM_UpdatedBy")]
    public int? CrcmUpdatedBy { get; set; }

    [Column("CRCM_UpdatedOn", TypeName = "datetime")]
    public DateTime? CrcmUpdatedOn { get; set; }

    [Column("CRCM_SubmittedBy")]
    public int? CrcmSubmittedBy { get; set; }

    [Column("CRCM_SubmittedOn", TypeName = "datetime")]
    public DateTime? CrcmSubmittedOn { get; set; }

    [Column("CRCM_CompID")]
    public int? CrcmCompId { get; set; }

    [Column("CRCM_PGEDetailId")]
    public int? CrcmPgedetailId { get; set; }
}
