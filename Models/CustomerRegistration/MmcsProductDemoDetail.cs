using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsProductDemoDetail
{
    public int? MpddPkid { get; set; }

    public int? MpddMcpPkid { get; set; }

    public int? MpddMpdPkid { get; set; }

    public string? MpddNameofPeopleandEmails { get; set; }

    public string? MpddProLink { get; set; }

    public string? MpddWhogavethedemo { get; set; }

    public string? MpddFeedback { get; set; }

    public string? MpddNextStep { get; set; }

    public DateTime? MpddCreatedOn { get; set; }

    public int? MpddSummaryRateAbc { get; set; }
}
