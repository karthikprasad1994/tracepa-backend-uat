using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtNote
{
    public int EdtNoteId { get; set; }

    public int? EdtPageId { get; set; }

    public string? EdtNotes { get; set; }

    public int? EdtCreatedBy { get; set; }

    public DateTime? EdtCreatedOn { get; set; }
}
