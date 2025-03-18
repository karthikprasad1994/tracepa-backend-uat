using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class UserActivityLog
{
    public int? UtPkid { get; set; }

    public int? UtUsrid { get; set; }

    public string? UtModule { get; set; }

    public string? UtSubmodule { get; set; }

    public DateTime? UtLoginDatetime { get; set; }

    public int? UtAsgnmntCreated { get; set; }

    public int? UtAsgnmntInprogress { get; set; }

    public int? UtAsgnmntCompleted { get; set; }

    public int? UtCompid { get; set; }

    public DateOnly? UtLogindate { get; set; }
}
