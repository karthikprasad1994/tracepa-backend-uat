using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFiling;
using TracePca.Dto.DigitalFilling;
using TracePca.Service;
using static TracePca.Service.DigitalFilling.Cabinet;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;

namespace TracePca.Interface.DigitalFilling
{
    public interface CabinetInterface
    {

 
		Task<IEnumerable<CabinetDto>> LoadCabinetAsync( int compID);
		 
		Task<int> CreateCabinetAsync(string cabinetName,int deptId, int userId, int compID);

        Task<int> UpdateCabinetAsync(string cabinetName, int iCabinetID, int iUserID, int compID);

		Task<int> UpdateSubCabinetAsync(string SubcabinetName, int iSubCabinetID, int iUserID, int compID);

		Task<int> UpdateFolderAsync(string FolderName, int iFolder, int iUserID, int compID);

		Task<string> IndexDocuments(IndexDocumentDto dto);

		Task<IEnumerable<DescriptorDto>> LoadDescriptorAsync(int iDescId, int compID);

		Task<int> CreateDescriptorAsync(string DESC_NAME, string DESC_NOTE, string DESC_DATATYPE, string DESC_SIZE, DescriptorDto dto);

		Task<int> UpdateDescriptorAsync(DescriptorDto dto);

		Task<IEnumerable<DocumentTypeDto>> LoadDocumentTypeAsync(int iDocTypeID, int iDepartmentID, DocumentTypeDto dto);

		Task<IEnumerable<DocumentTypeDto>> LoadAllDocumentTypeAsync(int iCompID);

		Task<int> CreateDescriptorAsync(string DocumentName, string DocumentNote, string DepartmentId, [FromBody] DocumentTypeDto dto);

		Task<int> CreateDocumentTypeAsync(string DocumentName,  string DepartmentId, int UserID, int CompID);

		Task<int> UpdateDocumentTypeAsync(int iDocTypeID, string DocumentName,  int UserID, int CompID);

		Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue);

		Task<IEnumerable<CabinetDto>> LoadRententionDataAsync(int compID);

		Task<IEnumerable<ArchiveDetailsDto>> LoadArchiveDetailsAsync(int compID);

		Task<IEnumerable<ArchivedDocumentFileDto>> ArchivedDocumentFileDetailsAsync(string sAttachID);

		Task<IEnumerable<DepartmentDto>> LoadAllDepartmentAsync(int compID);

		Task<string> CreateDepartmentAsync(string Code, string DepartmentName, string userId, int compID);

		Task<string> CreateSubCabinetAsync(string SubcabinetName, int iCabinetId,   int compID);

		Task<string> CreateFolderAsync(string FolderName, int iCabinetId, int iSubCabinetID, int compID);

		Task<string> UpdateArchiveDetailsAsync(string retentionDate, int retentionPeriod, int archiveId, int compId);

		Task<string> DownloadArchieveDocumentsAsync(string sAttachID);

		Task<string> DeleteArchiveDocumentsAsync(int archiveId,string sAttachID, int compId);

		Task<int> UpdateDepartmentAsync(string Code, string DepartmentName, int iDepartmentID, int iUserID, int compID);

		Task<GoogleDriveFile> GetFileByIdAsync(int DocId, string userEmail);

        Task<IEnumerable<SearchDto>> LoadFolderDocumentsDetailsAsync(int FolderID, int compID);
    }
}
