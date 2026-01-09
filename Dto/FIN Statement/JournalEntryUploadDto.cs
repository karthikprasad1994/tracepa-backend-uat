// Models/DTOs/JournalEntryUploadDto.cs
using System.ComponentModel.DataAnnotations;

namespace JournalEntryUploadAPI.Models.DTOs
{
  
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




    public class JournalEntryDto
    {
        public string SrNo { get; set; }
        public string Trans { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Num { get; set; }
        public string Adj { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public string Account { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class JournalEntryUploadDto
    {
        public int CustomerId { get; set; }
        public int FinancialYearId { get; set; }
        public int BranchId { get; set; }
        public int DurationId { get; set; }
        public int AccessCodeId { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
    }

    public class UploadResponseDto
    {
        public string TransactionId { get; set; }
        public bool Success { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int FailedRecords { get; set; }
        public double ProcessingTimeInSeconds { get; set; }
        public double RecordsPerSecond { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}