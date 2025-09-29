using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFiling;
using TracePca.Dto.DigitalFilling;
using TracePca.Service;
using static TracePca.Service.DigitalFilling.Cabinet;

namespace TracePca.Interface.DigitalFilling
{
    public interface CabinetInterface
    {

 
		Task<IEnumerable<CabinetDto>> LoadCabinetAsync( int compID);
		 
		Task<int> CreateCabinetAsync(string cabinetName,int deptId, int userId, int compID);

        Task<int> UpdateCabinetAsync(string cabinetName, int iCabinetID, int userId, int compID, CabinetDto dto);

        Task<string> IndexDocuments(IndexDocumentDto dto);

		Task<IEnumerable<DescriptorDto>> LoadDescriptorAsync(int iDescId, int compID);

		Task<int> CreateDescriptorAsync(string DESC_NAME, string DESC_NOTE, string DESC_DATATYPE, string DESC_SIZE, DescriptorDto dto);

		Task<int> UpdateDescriptorAsync(DescriptorDto dto);

		Task<IEnumerable<DocumentTypeDto>> LoadDocumentTypeAsync(int iDocTypeID, int iDepartmentID, DocumentTypeDto dto);

		Task<IEnumerable<DocumentTypeDto>> LoadAllDocumentTypeAsync(int iCompID);

		Task<int> CreateDescriptorAsync(string DocumentName, string DocumentNote, string DepartmentId, [FromBody] DocumentTypeDto dto);

		Task<int> UpdateDocumentTypeAsync(int iDocTypeID, string DocumentName, string DocumentNote, [FromBody] DocumentTypeDto dto);

		Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue);

		Task<IEnumerable<CabinetDto>> LoadRententionDataAsync(int compID);

		Task<IEnumerable<ArchiveDetailsDto>> LoadArchiveDetailsAsync(int compID);

		Task<IEnumerable<ArchivedDocumentFileDto>> ArchivedDocumentFileDetailsAsync(string sAttachID);

		Task<IEnumerable<DepartmentDto>> LoadAllDepartmentAsync(int compID);

		Task<string> CreateDepartmentAsync(string Code, string DepartmentName, string userId, int compID);

	}
}
