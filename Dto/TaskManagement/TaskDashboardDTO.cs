namespace TracePca.Dto.TaskManagement
{
    public sealed class TaskScheduledCountDto
    {
        public int Overdue { get; set; }
        public int Today { get; set; }
        public int OpenTasks { get; set; }
        public int ClosedTasks { get; set; }
    }

    public sealed class TaskScheduledDataDto
    {
        public int ScheduleId { get; set; }
        public string TaskNo { get; set; } = "";
        public string Client { get; set; } = "";
        public string Partner { get; set; } = "";
        public string Task { get; set; } = "";
        public string AssignedTo { get; set; } = "";
        public string CreatedDate { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string ExpectedCompletion { get; set; } = "";
        public string WorkStatus { get; set; } = "";
        public string Comments { get; set; } = "-";
    }

    public sealed class TaskDashboardResponseDto
    {
        public TaskScheduledCountDto TaskScheduledCount { get; set; } = new();
        public List<TaskScheduledDataDto> TaskScheduledData { get; set; } = new();
        public int TotalTasks { get; set; }
    }

    public sealed class TaskDashboardRequestDto
    {
        public int? YearId { get; set; }
        public int? ClientId { get; set; }
        public int? PartnerId { get; set; }
        public int? TaskId { get; set; }
        public int CompId { get; set; }
    }
}
