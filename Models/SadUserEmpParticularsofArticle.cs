using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadUserEmpParticularsofArticle
{
    public int? SupPkid { get; set; }

    public int? SupUserEmpId { get; set; }

    public string? SupPrincipleName { get; set; }

    public string? SupRegistrationNo { get; set; }

    public string? SupPracticeNo { get; set; }

    public DateTime? SupArticlesFrom { get; set; }

    public DateTime? SupArticlesTo { get; set; }

    public DateTime? SupExtendedTo { get; set; }

    public string? SupRemarks { get; set; }

    public int? SupAttachId { get; set; }

    public int? SupCrBy { get; set; }

    public DateTime? SupCrOn { get; set; }

    public int? SupUpdatedBy { get; set; }

    public DateTime? SupUpdatedOn { get; set; }

    public string? SupIpaddress { get; set; }

    public int? SupCompId { get; set; }
}
