using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_CUSTOMER_DETAILS")]
public partial class SadCustomerDetail
{
    [Column("CDET_ID")]
    public int? CdetId { get; set; }

    [Column("CDET_CUSTID")]
    public int? CdetCustid { get; set; }

    [Column("CDET_STANDINGININDUSTRY")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetStandinginindustry { get; set; }

    [Column("CDET_PUBLICPERCEPTION")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetPublicperception { get; set; }

    [Column("CDET_GOVTPERCEPTION")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetGovtperception { get; set; }

    [Column("CDET_LITIGATIONISSUES")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetLitigationissues { get; set; }

    [Column("CDET_PRODUCTSMANUFACTURED")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetProductsmanufactured { get; set; }

    [Column("CDET_SERVICESOFFERED")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetServicesoffered { get; set; }

    [Column("CDET_TURNOVER")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetTurnover { get; set; }

    [Column("CDET_PROFITABILITY")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetProfitability { get; set; }

    [Column("CDET_FOREIGNCOLLABORATIONS")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetForeigncollaborations { get; set; }

    [Column("CDET_EMPLOYEESTRENGTH")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetEmployeestrength { get; set; }

    [Column("CDET_PROFESSIONALSERVICES")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetProfessionalservices { get; set; }

    [Column("CDET_GATHEREDBYAUDITFIRM")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetGatheredbyauditfirm { get; set; }

    [Column("CDET_LEGALADVISORS")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetLegaladvisors { get; set; }

    [Column("CDET_AUDITINCHARGE")]
    [StringLength(255)]
    [Unicode(false)]
    public string? CdetAuditincharge { get; set; }

    [Column("CDET_FileNo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CdetFileNo { get; set; }

    [Column("CDET_CRBY")]
    public int? CdetCrby { get; set; }

    [Column("CDET_CRON", TypeName = "datetime")]
    public DateTime? CdetCron { get; set; }

    [Column("CDET_UpdatedBy")]
    public int? CdetUpdatedBy { get; set; }

    [Column("CDET_UpdatedOn", TypeName = "datetime")]
    public DateTime? CdetUpdatedOn { get; set; }

    [Column("CDET_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CdetStatus { get; set; }

    [Column("CDET_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CdetIpaddress { get; set; }

    [Column("CDET_CompID")]
    public int? CdetCompId { get; set; }
}
