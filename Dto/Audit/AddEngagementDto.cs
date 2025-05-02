namespace TracePca.Dto.Audit
{
    public class AddEngagementDto
    {
        public int? LoeId { get; set; }

        public int? LoeYearId { get; set; }

        public int? LoeCustomerId { get; set; }

        public int? LoeServiceTypeId { get; set; }

        public string? LoeNatureOfService { get; set; }

        public string? LoeLocationIds { get; set; }

        public string? LoeMilestones { get; set; }

        public DateTime? LoeTimeSchedule { get; set; }

        public DateTime? LoeReportDueDate { get; set; }

        public int? LoeProfessionalFees { get; set; }

        public int? LoeOtherFees { get; set; }

        public int? LoeServiceTax { get; set; }

        public int? LoeRembFilingFee { get; set; }

        public int? LoeCrBy { get; set; }

        public DateTime? LoeCrOn { get; set; }

        public int? LoeTotal { get; set; }

        public string? LoeName { get; set; }

        public int? LoeFrequency { get; set; }

        public int? LoeFunctionId { get; set; }

        public string? LoeSubFunctionId { get; set; }

        public DateTime? LoeUpdatedOn { get; set; }

        public int? LoeUpdatedBy { get; set; }

        public int? LoeApprovedby { get; set; }

        public DateTime? LoeApprovedon { get; set; }

        public string? LoeDelflag { get; set; }

        public string? LoeStatus { get; set; }

        public string? LoeIpaddress { get; set; }

        public int? LoeCompId { get; set; }
    }
}
