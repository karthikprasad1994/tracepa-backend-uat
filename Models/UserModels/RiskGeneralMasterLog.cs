using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("RISK_GeneralMASTER_Log")]
public partial class RiskGeneralMasterLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(30)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("RAM_PKID")]
    public int? RamPkid { get; set; }

    [Column("RAM_YearID")]
    public int? RamYearId { get; set; }

    [Column("nRAM_YearID")]
    public int? NRamYearId { get; set; }

    [Column("RAM_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? RamCode { get; set; }

    [Column("nRAM_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NRamCode { get; set; }

    [Column("RAM_Category")]
    [StringLength(20)]
    [Unicode(false)]
    public string? RamCategory { get; set; }

    [Column("nRAM_Category")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NRamCategory { get; set; }

    [Column("RAM_Name")]
    [StringLength(200)]
    [Unicode(false)]
    public string? RamName { get; set; }

    [Column("nRAM_Name")]
    [StringLength(200)]
    [Unicode(false)]
    public string? NRamName { get; set; }

    [Column("RAM_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RamRemarks { get; set; }

    [Column("nRAM_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? NRamRemarks { get; set; }

    [Column("RAM_Score")]
    public int? RamScore { get; set; }

    [Column("nRAM_Score")]
    public int? NRamScore { get; set; }

    [Column("RAM_StartValue")]
    public double? RamStartValue { get; set; }

    [Column("nRAM_StartValue")]
    public double? NRamStartValue { get; set; }

    [Column("RAM_EndValue")]
    public double? RamEndValue { get; set; }

    [Column("nRAM_EndValue")]
    public double? NRamEndValue { get; set; }

    [Column("RAM_Color")]
    [StringLength(50)]
    [Unicode(false)]
    public string? RamColor { get; set; }

    [Column("nRAM_Color")]
    [StringLength(50)]
    [Unicode(false)]
    public string? NRamColor { get; set; }

    [Column("RAM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RamIpaddress { get; set; }

    [Column("RAM_CompID")]
    public int? RamCompId { get; set; }
}
