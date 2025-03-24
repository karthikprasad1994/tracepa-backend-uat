using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_UserEMP_Address")]
public partial class SadUserEmpAddress
{
    [Column("SUA_PKID")]
    public int? SuaPkid { get; set; }

    [Column("SUA_UserEmpID")]
    public int? SuaUserEmpId { get; set; }

    [Column("SUA_ContactName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SuaContactName { get; set; }

    [Column("SUA_Address1")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaAddress1 { get; set; }

    [Column("SUA_Address2")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaAddress2 { get; set; }

    [Column("SUA_Address3")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaAddress3 { get; set; }

    [Column("SUA_Pincode")]
    public int? SuaPincode { get; set; }

    [Column("SUA_Mobile")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SuaMobile { get; set; }

    [Column("SUA_Telephone")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SuaTelephone { get; set; }

    [Column("SUA_Email")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? SuaEmail { get; set; }

    [Column("SUA_RelationType")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SuaRelationType { get; set; }

    [Column("SUA_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SuaIpaddress { get; set; }

    [Column("SUA_CompId")]
    public int? SuaCompId { get; set; }
}
