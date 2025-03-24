using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("ITReturns_Client")]
public partial class ItreturnsClient
{
    [Column("ITR_ID")]
    public int? ItrId { get; set; }

    [Column("ITR_ClientName")]
    [StringLength(500)]
    [Unicode(false)]
    public string? ItrClientName { get; set; }

    [Column("ITR_PAN")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ItrPan { get; set; }

    [Column("ITR_Aadhaar")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ItrAadhaar { get; set; }

    [Column("ITR_DOB", TypeName = "datetime")]
    public DateTime? ItrDob { get; set; }

    [Column("ITR_Phone")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ItrPhone { get; set; }

    [Column("ITR_Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ItrEmail { get; set; }

    [Column("ITR_ITLoginId")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ItrItloginId { get; set; }

    [Column("ITR_ITPassword")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ItrItpassword { get; set; }

    [Column("ITR_CrBy")]
    public int? ItrCrBy { get; set; }

    [Column("ITR_CrOn", TypeName = "datetime")]
    public DateTime? ItrCrOn { get; set; }

    [Column("ITR_UpdatedBy")]
    public int? ItrUpdatedBy { get; set; }

    [Column("ITR_UpdateOn", TypeName = "datetime")]
    public DateTime? ItrUpdateOn { get; set; }

    [Column("ITR_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ItrIpaddress { get; set; }

    [Column("ITR_CompID")]
    public int? ItrCompId { get; set; }
}
