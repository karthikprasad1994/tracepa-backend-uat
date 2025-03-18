using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserEmpAcademicProgress
{
    public int? SuapPkid { get; set; }

    public int? SuapUserEmpId { get; set; }

    public DateTime? SuapExamTakenOn { get; set; }

    public int? SuapLeaveGranted { get; set; }

    public int? SuapMonthofExam { get; set; }

    public string? SuapGroups { get; set; }

    public string? SuapResult { get; set; }

    public string? SuapRemarks { get; set; }

    public int? SuapAttachId { get; set; }

    public int? SuapCrBy { get; set; }

    public DateTime? SuapCrOn { get; set; }

    public int? SuapUpdatedBy { get; set; }

    public DateTime? SuapUpdatedOn { get; set; }

    public string? SuapIpaddress { get; set; }

    public int? SuapCompId { get; set; }
}
