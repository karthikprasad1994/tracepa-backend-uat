using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ACC_General_Master")]
public partial class AccGeneralMaster
{
    [Column("Mas_id")]
    public int MasId { get; set; }

    [Column("Mas_desc")]
    [Unicode(false)]
    public string? MasDesc { get; set; }

    [Column("Mas_delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasDelflag { get; set; }

    [Column("Mas_master")]
    public int? MasMaster { get; set; }

    [Column("Mas_Remarks")]
    [Unicode(false)]
    public string? MasRemarks { get; set; }

    [Column("Mas_CompID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MasCompId { get; set; }

    [Column("Mas_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MasStatus { get; set; }

    [Column("Mas_CrBy")]
    public int? MasCrBy { get; set; }

    [Column("Mas_CrOn", TypeName = "datetime")]
    public DateTime? MasCrOn { get; set; }

    [Column("Mas_UpdatedBy")]
    public int? MasUpdatedBy { get; set; }

    [Column("Mas_UpdatedOn", TypeName = "datetime")]
    public DateTime? MasUpdatedOn { get; set; }

    [Column("Mas_AppBy")]
    public int? MasAppBy { get; set; }

    [Column("Mas_AppOn", TypeName = "datetime")]
    public DateTime? MasAppOn { get; set; }

    [Column("Mas_DeletedBy")]
    public int? MasDeletedBy { get; set; }

    [Column("Mas_DeletedOn", TypeName = "datetime")]
    public DateTime? MasDeletedOn { get; set; }

    [Column("Mas_IPAddress")]
    [StringLength(20)]
    [Unicode(false)]
    public string? MasIpaddress { get; set; }

    [Column("Mas_RecalledBy")]
    public int? MasRecalledBy { get; set; }

    [Column("Mas_Operation")]
    [StringLength(1)]
    [Unicode(false)]
    public string? MasOperation { get; set; }
}
