using System;
using System.Collections.Generic;

namespace TracePca.Models.CustomerRegistration;

public partial class MmcsCustomerProspect
{
    public int? McpPkid { get; set; }

    public string? McpProspectName { get; set; }

    public string? McpProspecttype { get; set; }

    public int? McpProspectSize { get; set; }

    public int? McpNumOffices { get; set; }

    public string? McpTypeofBusiness { get; set; }

    public string? McpUrl { get; set; }

    public string? McpContactName { get; set; }

    public string? McpContactPhone { get; set; }

    public string? McpContactEmail { get; set; }

    public string? McpRefferredBy { get; set; }

    public DateTime? McpRefferredOn { get; set; }

    public string? McpStatus { get; set; }

    public DateTime? McpCreatedOn { get; set; }
}
