using System.Threading.Tasks;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface JournalEntryInterface
    {

        //GetCustomersName
        Task<IEnumerable<CustDto>> GetCustomerNameAsync(int icompId);

        //GetFinancialYear
        Task<IEnumerable<FinancialYearDto>> GetFinancialYearAsync(int icompId);

        //GetDuration
        Task<IEnumerable<CustDurationDto>> GetDurationAsync(int compId, int custId);

        //GetBranchName
        Task<IEnumerable<CustBranchDto>> GetBranchNameAsync(int compId, int custId);

        //GetJournalEntryInformation
        Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
            int compId, int userId, string status, int custId, int yearId, int branchId, string dateFormat, int durationId);
    }
}
