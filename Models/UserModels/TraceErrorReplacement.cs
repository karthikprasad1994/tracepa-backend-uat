using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_Error_Replacement")]
public partial class TraceErrorReplacement
{
    [Column("TER_PKID")]
    public int TerPkid { get; set; }

    [Column("TER_RunTimeError")]
    [Unicode(false)]
    public string? TerRunTimeError { get; set; }

    [Column("TER_ErrorReplacemnet")]
    [Unicode(false)]
    public string? TerErrorReplacemnet { get; set; }

    [Column("TER_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TerStatus { get; set; }
}
