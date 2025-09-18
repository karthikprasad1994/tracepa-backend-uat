using System.Data;
using static TracePca.Dto.FIN_Statement.SchedulePartnerFundsDto;
namespace TracePca.Interface.FIN_Statement
{
    public interface SchedulePartnerFundsInterface
    {

        //GetAllPartnershipFirms
        Task<IEnumerable<PartnershipFirmRowDto>> LoadAllPartnershipFirmsAsync(PartnershipFirmRequestDto request);

        //GetPartnernName
        Task<IEnumerable<PartnerDto>> LoadCustPartnerAsync(int custId, int compId);

        //SavePartnershipFirms
        Task<int[]> SavePartnershipFirmsAsync(SavePartnershipFirmDto objPF);

        //UpdatePartnershipFirms
        Task<int[]> SaveOrUpdatePartnershipFirmAsync(UpdatePartnershipFirmDto dto);

        //GetSelectedPartnershipFirms
        Task<IEnumerable<SelectedPartnershipFirmRowDto>> LoadSelectedPartnershipFirmAsync(int partnershipFirmId, int compId);

        //UpdateAndCalculate
        Task<decimal> UpdateAndCalculateAsync(PartnershipFirmCalculationDto dto);

        //GetCustomerPartnerDetails
        Task<IEnumerable<PartnerDetailsDto>> GetCustomerPartnerDetailsAsync(int compId, int custId, int custPartnerPkId);

        //SaveCustomerStatutoryPartner
        Task<int[]> SaveOrUpdateCustomerStatutoryPartnerAsync(SaveCustomerStatutoryPartnerDto partnerDto);
    }
}
