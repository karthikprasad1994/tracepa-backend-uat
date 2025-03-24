using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Account")]
public partial class Account
{
    [Column("AID")]
    public int? Aid { get; set; }

    [Column("AName")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Aname { get; set; }

    [Column("ACode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Acode { get; set; }

    [Column("AMoblieNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AmoblieNo { get; set; }

    [Column("AEmailID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AemailId { get; set; }

    [Column("AAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Aaddress { get; set; }
}
