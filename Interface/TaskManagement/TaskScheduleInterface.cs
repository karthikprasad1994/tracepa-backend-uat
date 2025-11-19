using TracePca.Dto.Audit;
using TracePca.Dto.TaskManagement;

namespace TracePca.Interface.TaskManagement
{
    public interface TaskScheduleInterface
    {
        Task<IEnumerable<DropDownListData>> LoadAllModulesAsync(int compId, int projectId);
        Task<IEnumerable<DropDownListData>> loadSubTasksByTaskIdAsync(int compId, int taskId);
        Task<int> SaveOrUpdateTaskScheduleAsync(TaskScheduleCreateDto dto);
        Task<TaskDetailsResponseDto?> GetTaskScheduledDetailsByTaskIdAsync(int taskId);
        Task<bool> UpdateTaskAndSubtasksAsync(TaskUpdateRequestDto dto);
    }
}