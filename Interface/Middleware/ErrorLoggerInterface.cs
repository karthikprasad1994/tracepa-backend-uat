using TracePca.Dto.Middleware;

namespace TracePca.Interface.Middleware
{
    public interface ErrorLoggerInterface
    {
        Task LogErrorAsync(ErrorLogDto dto);
        Task<IEnumerable<ErrorLogDto>> GetAllLogsAsync();
        Task<ErrorLogDto?> GetLogByIdAsync(int id);


    }
}
