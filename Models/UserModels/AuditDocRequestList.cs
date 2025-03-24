using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Doc_Request_List")]
public partial class AuditDocRequestList
{
    [Column("DRL_DRLID")]
    public int? DrlDrlid { get; set; }

    [Column("DRL_DocTypeID")]
    public int? DrlDocTypeId { get; set; }

    [Column("DRL_Name")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? DrlName { get; set; }

    [Column("DRL_Description")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? DrlDescription { get; set; }

    [Column("DRL_DocumentType")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DrlDocumentType { get; set; }

    [Column("DRL_CrBy")]
    public int? DrlCrBy { get; set; }

    [Column("DRL_CROn", TypeName = "datetime")]
    public DateTime? DrlCron { get; set; }

    [Column("DRL_UpdatedBy")]
    public int? DrlUpdatedBy { get; set; }

    [Column("DRL_UpdatedOn", TypeName = "datetime")]
    public DateTime? DrlUpdatedOn { get; set; }

    [Column("DRL_Status")]
    [StringLength(1)]
    [Unicode(false)]
    public string? DrlStatus { get; set; }

    [Column("DRL_DType")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrlDtype { get; set; }

    [Column("DRL_Size")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DrlSize { get; set; }

    [Column("DRL_SampleId")]
    [StringLength(10)]
    [Unicode(false)]
    public string? DrlSampleId { get; set; }

    [Column("DRL_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? DrlIpaddress { get; set; }

    [Column("DRL_CompID")]
    public int? DrlCompId { get; set; }
}
