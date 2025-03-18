using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditScheduleConductWorkPaper
{
    public int? SswId { get; set; }

    public int? SswSaId { get; set; }

    public string? SswWorkpaperNo { get; set; }

    public string? SswWorkpaperRef { get; set; }

    public int? SswTypeOfTest { get; set; }

    public string? SswObservation { get; set; }

    public string? SswConclusion { get; set; }

    public int? SswStatus { get; set; }

    public int? SswAttachId { get; set; }

    public int? SswCrBy { get; set; }

    public DateTime? SswCrOn { get; set; }

    public int? SswUpdatedBy { get; set; }

    public DateTime? SswUpdatedOn { get; set; }

    public string? SswIpaddress { get; set; }

    public int? SswCompId { get; set; }

    public int? SswExceededMateriality { get; set; }

    public double? SswAuditorHoursSpent { get; set; }

    public string? SswNotesSteps { get; set; }

    public string? SswCriticalAuditMatter { get; set; }

    public string? SswReviewerComments { get; set; }

    public int? SswReviewedBy { get; set; }

    public DateTime? SswReviewedOn { get; set; }

    public int? SswWpcheckListId { get; set; }

    public int? SswDrlid { get; set; }
}
