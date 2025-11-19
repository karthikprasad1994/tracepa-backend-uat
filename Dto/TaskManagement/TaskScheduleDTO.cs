namespace TracePca.Dto.TaskManagement
{
    public class TaskScheduleCreateDto
    {
        public int TMS_Id { get; set; }
        public string? TMS_TaskNo { get; set; }
        public int TMS_FinancialYearId { get; set; }
        public int TMS_CustomerId { get; set; }
        public int? TMS_ProjectId { get; set; }
        public int? TMS_ModuleId { get; set; }
        public int TMS_TaskId { get; set; }
        public string? TMS_PartnerIds { get; set; }
        public string? TMS_TeamMemberIds { get; set; }
        public DateTime TMS_StartDate { get; set; }
        public DateTime TMS_DueDate { get; set; }
        public int TMS_FrequencyId { get; set; }
        public string? TMS_Description { get; set; }
        public int TMS_StatusId { get; set; }
        public int TMS_CreatedBy { get; set; }
        public int? TMS_AttachmentId { get; set; }
        public int TMS_CompId { get; set; }
        public string? TMS_IPAddress { get; set; }
        public List<TaskSubTaskDetailCreateDto>? TaskManagement_SubTask_Details { get; set; }
    }

    public class TaskSubTaskDetailCreateDto
    {
        public int TMSD_SubtaskId { get; set; }
        public int? TMSD_WorkStatusId { get; set; }
        public int TMSD_CreatedBy { get; set; }
        public int? TMSD_ApprovedBy { get; set; }
        public DateTime? TMSD_ApprovedOn { get; set; }
        public int? TMSD_ModifiedBy { get; set; }
        public DateTime? TMSD_ModifiedOn { get; set; }
        public int? TMSD_AttachmentId { get; set; }
        public int TMSD_CompId { get; set; }
        public string? TMSD_IPAddress { get; set; }
    }

    public class TaskScheduleDetailsDto
    {
        public string? Year { get; set; }
        public int TaskPkId { get; set; }
        public int TaskId { get; set; }
        public string TaskNo { get; set; } = "";
        public string ClientName { get; set; } = "";
        public string TaskName { get; set; } = "";
        public List<string> PartnerNames { get; set; } = new();
        public List<string> TeamMemberNames { get; set; } = new();
        public int? StatusId { get; set; }
    }

    public class SubTaskListDto
    {
        public int SubtaskPkId { get; set; } 
        public int SubtaskId { get; set; }
        public string SubtaskName { get; set; } = "";
        public string? Comment { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class TaskDetailsResponseDto
    {
        public TaskScheduleDetailsDto TaskDetails { get; set; } = new();
        public List<SubTaskListDto> SubTaskList { get; set; } = new();
        public List<SubTaskListDto> SubTaskHistory { get; set; } = new();
    }

    public class TaskUpdateRequestDto
    {
        public int TaskPKId { get; set; }
        public int TaskId { get; set; }
        public string? StatusId { get; set; }
        public string? Description { get; set; }
        public DateTime? WorkDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? CompId { get; set; }
        public string? IpAddress { get; set; }        
        public List<SubTaskListDto> SubTasks { get; set; } = new();
    }
}
