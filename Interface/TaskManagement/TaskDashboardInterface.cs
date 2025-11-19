using TracePca.Dto.TaskManagement;

namespace TracePca.Interface.TaskManagement
{
    public interface TaskDashboardInterface
    {
        Task<TaskDropDownListDataDTO> LoadAllDDLDataAsync(int compId);
        Task<TaskDashboardResponseDto> GetTaskScheduledDashboardDataAsync(TaskDashboardRequestDto dto);
    }
}