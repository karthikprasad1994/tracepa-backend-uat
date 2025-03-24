using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MMCSPL_DB_Access")]
public partial class MmcsplDbAccess
{
    [Column("MDA_ID")]
    public int MdaId { get; set; }

    [Column("MDA_DatabaseName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MdaDatabaseName { get; set; }

    [Column("MDA_AccessCode")]
    [StringLength(500)]
    [Unicode(false)]
    public string? MdaAccessCode { get; set; }

    [Column("MDA_CompanyName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MdaCompanyName { get; set; }

    [Column("MDA_CreatedDate", TypeName = "datetime")]
    public DateTime? MdaCreatedDate { get; set; }

    [Column("MDA_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MdaIpaddress { get; set; }

    [Column("MDA_Application")]
    public int? MdaApplication { get; set; }
}
