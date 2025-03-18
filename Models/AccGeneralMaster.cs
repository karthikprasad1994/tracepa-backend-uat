using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccGeneralMaster
{
    public int MasId { get; set; }

    public string? MasDesc { get; set; }

    public string? MasDelflag { get; set; }

    public int? MasMaster { get; set; }

    public string? MasRemarks { get; set; }

    public string? MasCompId { get; set; }

    public string? MasStatus { get; set; }

    public int? MasCrBy { get; set; }

    public DateTime? MasCrOn { get; set; }

    public int? MasUpdatedBy { get; set; }

    public DateTime? MasUpdatedOn { get; set; }

    public int? MasAppBy { get; set; }

    public DateTime? MasAppOn { get; set; }

    public int? MasDeletedBy { get; set; }

    public DateTime? MasDeletedOn { get; set; }

    public string? MasIpaddress { get; set; }

    public int? MasRecalledBy { get; set; }

    public string? MasOperation { get; set; }
}
