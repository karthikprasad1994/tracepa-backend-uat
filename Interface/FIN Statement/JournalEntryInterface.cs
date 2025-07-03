using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface JournalEntryInterface
    {

        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int CompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int CompId);

        //GetDuration
        Task<int?> GetCustomerDurationIdAsync(int CompId, int CustId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int CompId, int CustId);

        //GetJournalEntryInformation
        Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
            int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId);
    }
}
