using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_General_Master1")]
public partial class AccGeneralMaster1
{
    [Column("Mas_id")]
    public int MasId { get; set; }

    [Column("Mas_desc")]
    [StringLength(200)]
    [Unicode(false)]
    public string? MasDesc { get; set; }

    [Column("Mas_delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasDelflag { get; set; }

    [Column("Mas_master")]
    public int? MasMaster { get; set; }

    [Column("Mas_Remarks")]
    [StringLength(200)]
    [Unicode(false)]
    public string? MasRemarks { get; set; }
}
