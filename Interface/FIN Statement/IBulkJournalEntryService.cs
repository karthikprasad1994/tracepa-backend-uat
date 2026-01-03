using JournalEntryUploadAPI.Models.DTOs;

namespace TracePca.Interface.FIN_Statement
{

    public interface IBulkJournalEntryService
    {
        Task<UploadResponseDto> BulkUploadJournalEntriesAsync(IFormFile file, JournalEntryUploadDto request);
        Task<ValidationResultDto> ValidateJournalEntriesAsync(IFormFile file, JournalEntryUploadDto request);
        Task<UploadHistoryDto> GetUploadHistoryAsync(int customerId, DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 50);
    }
}
