using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserEmpAssessment
{
    public int? SuaPkid { get; set; }

    public int? SuaUserEmpId { get; set; }

    public DateTime? SuaIssueDate { get; set; }

    public string? SuaRating { get; set; }

    public string? SuaPerformanceAwardPaid { get; set; }

    public string? SuaGradesPromotedFrom { get; set; }

    public string? SuaGradesPromotedTo { get; set; }

    public string? SuaRemarks { get; set; }

    public int? SuaAttachId { get; set; }

    public int? SuaCrBy { get; set; }

    public DateTime? SuaCrOn { get; set; }

    public int? SuaUpdatedBy { get; set; }

    public DateTime? SuaUpdatedOn { get; set; }

    public string? SuaIpaddress { get; set; }

    public int? SuaCompId { get; set; }
}
