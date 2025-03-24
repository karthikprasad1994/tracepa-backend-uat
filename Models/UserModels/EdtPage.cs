using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_page")]
public partial class EdtPage
{
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

    [Column("PGE_CRBY", TypeName = "numeric(5, 0)")]
    public decimal? PgeCrby { get; set; }

    [Column("PGE_CRON", TypeName = "datetime")]
    public DateTime? PgeCron { get; set; }

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

    [Column("PGE_PAGETYPE")]
    [StringLength(5)]
    [Unicode(false)]
    public string? PgePagetype { get; set; }

    [Column("PGE_KeyWORD")]
    [StringLength(500)]
    [Unicode(false)]
    public string? PgeKeyWord { get; set; }

    [Column("PGE_OCRText")]
    [Unicode(false)]
    public string? PgeOcrtext { get; set; }

    [Column("PGE_CHECKOUT", TypeName = "numeric(1, 0)")]
    public decimal? PgeCheckout { get; set; }

    [Column("PGE_CHECKEDOUTBY", TypeName = "numeric(5, 0)")]
    public decimal? PgeCheckedoutby { get; set; }

    [Column("PGE_ENCRYPT")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeEncrypt { get; set; }

    [Column("PGE_CDPATH")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PgeCdpath { get; set; }

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

    [Column("pge_ModBy")]
    public int? PgeModBy { get; set; }

    [Column("pge_ModOn", TypeName = "datetime")]
    public DateTime? PgeModOn { get; set; }

    [Column("pge_DelBy")]
    public int? PgeDelBy { get; set; }

    [Column("pge_DelOn", TypeName = "datetime")]
    public DateTime? PgeDelOn { get; set; }

    [Column("PGE_FTPStatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeFtpstatus { get; set; }

    [Column("pge_refno")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PgeRefno { get; set; }

    [Column("PGE_QC_TO")]
    [StringLength(2)]
    [Unicode(false)]
    public string? PgeQcTo { get; set; }

    [Column("PGE_QC_UsrGrpId")]
    public int? PgeQcUsrGrpId { get; set; }

    [Column("PGE_batch_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PgeBatchName { get; set; }

    [Column("PGE_BatchID")]
    public int? PgeBatchId { get; set; }

    [Column("PGE_OCRDelFlag")]
    public int? PgeOcrdelFlag { get; set; }

    [Column("pge_OrignalFileName")]
    [Unicode(false)]
    public string? PgeOrignalFileName { get; set; }

    [Column("Pge_CompID")]
    public int? PgeCompId { get; set; }

    [Column("Pge_CreatedBy")]
    public int? PgeCreatedBy { get; set; }

    [Column("Pge_CreatedOn", TypeName = "datetime")]
    public DateTime? PgeCreatedOn { get; set; }

    [Column("Pge_UpdatedBy")]
    public int? PgeUpdatedBy { get; set; }

    [Column("Pge_UpdatedOn", TypeName = "datetime")]
    public DateTime? PgeUpdatedOn { get; set; }

    [Column("Pge_DeletedBy")]
    public int? PgeDeletedBy { get; set; }

    [Column("Pge_DeletedOn", TypeName = "datetime")]
    public DateTime? PgeDeletedOn { get; set; }

    [Column("Pge_RecalledBy")]
    public int? PgeRecalledBy { get; set; }

    [Column("Pge_RecalledOn", TypeName = "datetime")]
    public DateTime? PgeRecalledOn { get; set; }

    [Column("pge_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? PgeDelflag { get; set; }

    [Column("Pge_ApprovedBy")]
    public int? PgeApprovedBy { get; set; }

    [Column("Pge_ApprovedOn", TypeName = "datetime")]
    public DateTime? PgeApprovedOn { get; set; }

    [Column("PGE_OCRText_Line1")]
    [Unicode(false)]
    public string? PgeOcrtextLine1 { get; set; }

    [Column("PGE_OCRText_Line2")]
    [Unicode(false)]
    public string? PgeOcrtextLine2 { get; set; }

    [Column("PGE_OCRText_Line3")]
    [Unicode(false)]
    public string? PgeOcrtextLine3 { get; set; }

    [Column("PGE_OCR_Status")]
    public int? PgeOcrStatus { get; set; }

    [Column("PGE_RFID")]
    [Unicode(false)]
    public string? PgeRfid { get; set; }

    [Column("PGE_LastViewed", TypeName = "datetime")]
    public DateTime? PgeLastViewed { get; set; }
}
