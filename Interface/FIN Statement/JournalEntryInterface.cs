using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TracePca.Dto.FIN_Statement.JournalEntryDto;

namespace TracePca.Interface.FIN_Statement
{
    public interface JournalEntryInterface
    {

        //GetJournalEntryInformation
        Task<IEnumerable<JournalEntryInformationDto>> GetJournalEntryInformationAsync(
            string DBName, int CompId, int UserId, string Status, int CustId, int YearId, int BranchId, string DateFormat, int DurationId);
    }
}
