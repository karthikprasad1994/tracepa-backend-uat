using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("TRACe_Folder")]
public partial class TraceFolder
{
    [Column("TF_PKID")]
    public int? TfPkid { get; set; }

    [Column("TF_CabinetID")]
    public int? TfCabinetId { get; set; }

    [Column("TF_SubCabinetID")]
    public int? TfSubCabinetId { get; set; }

    [Column("TF_Name")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? TfName { get; set; }

    [Column("TF_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? TfRemarks { get; set; }

    [Column("TF_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TfStatus { get; set; }

    [Column("TF_CrOn", TypeName = "datetime")]
    public DateTime? TfCrOn { get; set; }

    [Column("TF_CrBy")]
    public int? TfCrBy { get; set; }

    [Column("TF_UpdatedOn", TypeName = "datetime")]
    public DateTime? TfUpdatedOn { get; set; }

    [Column("TF_UpdatedBy")]
    public int? TfUpdatedBy { get; set; }

    [Column("TF_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TfIpaddress { get; set; }

    [Column("TF_CompID")]
    public int? TfCompId { get; set; }
}
