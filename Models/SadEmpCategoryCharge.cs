using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadEmpCategoryCharge
{
    public int? EmpcId { get; set; }

    public int? EmpcYearId { get; set; }

    public int? EmpcCatId { get; set; }

    public int? EmpcDays { get; set; }

    public int? EmpcHours { get; set; }

    public decimal? EmpcCharges { get; set; }

    public decimal? EmpcKmcharges { get; set; }

    public string? EmpcRemarks { get; set; }

    public int? EmpcCreatedBy { get; set; }

    public DateTime? EmpcCreatedOn { get; set; }

    public int? EmpcUpdatedBy { get; set; }

    public DateTime? EmpcUpdatedOn { get; set; }

    public int? EmpcDeletedBy { get; set; }

    public DateTime? EmpcDeletedOn { get; set; }

    public int? EmpcRecalledBy { get; set; }

    public DateTime? EmpcRecalledOn { get; set; }

    public int? EmpcAppBy { get; set; }

    public DateTime? EmpcAppOn { get; set; }

    public string? EmpcIpaddress { get; set; }

    public string? EmpcDelFlag { get; set; }

    public string? EmpcStatus { get; set; }

    public int? EmpcCompId { get; set; }

    public string? EmpcCremarks { get; set; }

    public int? EmpcCcreatedBy { get; set; }

    public DateTime? EmpcCcreatedOn { get; set; }

    public int? EmpcCupdatedBy { get; set; }

    public DateTime? EmpcCupdatedOn { get; set; }

    public int? EmpcCdeletedBy { get; set; }

    public DateTime? EmpcCdeletedOn { get; set; }

    public int? EmpcCrecalledBy { get; set; }

    public DateTime? EmpcCrecalledOn { get; set; }

    public int? EmpcCappBy { get; set; }

    public DateTime? EmpcCappOn { get; set; }

    public string? EmpcCdelFlag { get; set; }

    public string? EmpcCstatus { get; set; }
}
