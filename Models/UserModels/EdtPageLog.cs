using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Edt_page_log")]
public partial class EdtPageLog
{
    [Column("Log_PKID")]
    public int LogPkid { get; set; }

    [Column("Log_Operation")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LogOperation { get; set; }

    [Column("Log_Date", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("Log_UserID")]
    public int? LogUserId { get; set; }

    [Column("PGE_BASENAME", TypeName = "numeric(15, 0)")]
    public decimal? PgeBasename { get; set; }

    [Column("PGE_CABINET", TypeName = "numeric(7, 0)")]
    public decimal? PgeCabinet { get; set; }

    [Column("PGE_FOLDER", TypeName = "numeric(7, 0)")]
    public decimal? PgeFolder { get; set; }

    [Column("PGE_DOCUMENT_TYPE", TypeName = "numeric(15, 0)")]
    public decimal? PgeDocumentType { get; set; }

    [Column("PGE_TITLE")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? PgeTitle { get; set; }

    [Column("PGE_DATE", TypeName = "datetime")]
    public DateTime? PgeDate { get; set; }

    [Column("Pge_DETAILS_ID", TypeName = "numeric(10, 0)")]
    public decimal? PgeDetailsId { get; set; }

    [Column("PGE_OBJECT")]
    [StringLength(10)]
    [Unicode(false)]
    public string? PgeObject { get; set; }

    [Column("PGE_PAGENO", TypeName = "numeric(4, 0)")]
    public decimal? PgePageno { get; set; }

    [Column("PGE_EXT")]
    [StringLength(5)]
    [Unicode(false)]
    public string? PgeExt { get; set; }

    [Column("PGE_KeyWORD")]
    [StringLength(500)]
    [Unicode(false)]
    public string? PgeKeyWord { get; set; }

    [Column("PGE_OCRText")]
    [Unicode(false)]
    public string? PgeOcrtext { get; set; }

    [Column("PGE_SIZE", TypeName = "numeric(10, 0)")]
    public decimal? PgeSize { get; set; }

    [Column("PGE_CURRENT_VER", TypeName = "numeric(10, 0)")]
    public decimal? PgeCurrentVer { get; set; }

    [Column("PGE_STATUS")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeStatus { get; set; }

    [Column("PGE_SubCabinet", TypeName = "numeric(10, 0)")]
    public decimal? PgeSubCabinet { get; set; }

    [Column("PGE_QC_UsrGrpId")]
    public int? PgeQcUsrGrpId { get; set; }

    [Column("PGE_FTPStatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeFtpstatus { get; set; }

    [Column("PGE_batch_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PgeBatchName { get; set; }

    [Column("pge_OrignalFileName")]
    [Unicode(false)]
    public string? PgeOrignalFileName { get; set; }

    [Column("PGE_BatchID")]
    public int? PgeBatchId { get; set; }

    [Column("PGE_OCRDelFlag")]
    public int? PgeOcrdelFlag { get; set; }

    [Column("Pge_CompID")]
    public int? PgeCompId { get; set; }

    [Column("pge_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeDelflag { get; set; }

    [Column("Pge_CreatedBy")]
    public int? PgeCreatedBy { get; set; }

    [Column("Pge_CreatedOn", TypeName = "datetime")]
    public DateTime? PgeCreatedOn { get; set; }

    [Column("Pge_UpdatedBy")]
    public int? PgeUpdatedBy { get; set; }

    [Column("Pge_UpdatedOn", TypeName = "datetime")]
    public DateTime? PgeUpdatedOn { get; set; }
}
