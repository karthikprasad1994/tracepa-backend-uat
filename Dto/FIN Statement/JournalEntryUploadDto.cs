// Models/DTOs/JournalEntryUploadDto.cs
using System.ComponentModel.DataAnnotations;

namespace JournalEntryUploadAPI.Models.DTOs
{
    public class JournalEntryUploadDto
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Financial Year ID is required")]
        public int FinancialYearId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Duration ID is required")]
        public int DurationId { get; set; }

        [Required(ErrorMessage = "Access Code ID is required")]
        public int AccessCodeId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        public string IpAddress { get; set; } = "127.0.0.1";

        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; }
    }

    public class JournalEntryRecordDto
    {
        public string SrNo { get; set; }
        public string TransactionNumber { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Number { get; set; }
        public string Adjustment { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public string Account { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int RowIndex { get; set; }
    }

    public class UploadResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int FailedRecords { get; set; }
        public double ProcessingTimeInSeconds { get; set; }
        public double RecordsPerSecond { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string TransactionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

  
    public class ValidationResultDto
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<ValidationErrorDto> Errors { get; set; } = new List<ValidationErrorDto>();
    }

    public class ValidationErrorDto
    {
        public int RowIndex { get; set; }
        public string SrNo { get; set; }
        public string Account { get; set; }
        public string Type { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UploadHistoryDto
    {
        public List<UploadHistoryItemDto> Uploads { get; set; } = new List<UploadHistoryItemDto>();
        public int TotalCount { get; set; }
    }

    public class UploadHistoryItemDto
    {
        public int UploadId { get; set; }
        public int CustomerId { get; set; }
        public int FinancialYearId { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int FailedRecords { get; set; }
        public int DurationMs { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}