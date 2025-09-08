using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using StackExchange.Redis;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Transactions;
using TracePca.Data;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Interface;
using TracePca.Interface.DigitalFilling;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace TracePca.Service.DigitalFilling
{
    public class Cabinet : CabinetInterface
    {

        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
          
		private readonly IWebHostEnvironment _env;
		private readonly DbConnectionProvider _dbConnectionProvider;

		public Cabinet(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

        public async Task  CheckandInsertMemberGroupAsync(int userId, int compID)
        {
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();

			

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			var templateOrgDetails = await connection.QueryAsync<OrgStructureDto>(@"Select org_node,org_name from sad_org_structure where Org_Parent = 3 and Org_DelFlag='A'",
                    new { compID = compID });
            
            foreach (var Org in templateOrgDetails)
            {
                var rowsAffected1 = await connection.ExecuteScalarAsync<int>(@"Select * from Sad_UsersInOtherDept where SUO_UserID=@SUO_UserID and  
                SUO_DeptID=@SUO_DeptID", new { SUO_UserID = userId, SUO_DeptID = Org.Org_Node });
                if (rowsAffected1 > 0)
                {
                }
                else
                {
                    using var transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync(
                     @"DECLARE @NewFeesId INT; SELECT @NewFeesId = ISNULL(MAX(SUO_PKID), 0) + 1 FROM Sad_UsersInOtherDept;
                          INSERT INTO Sad_UsersInOtherDept (SUO_PKID, SUO_UserID, SUO_DeptID, SUO_IsDeptHead, SUO_CreatedBy, SUO_CreatedOn, SUO_IPAddress, SUO_CompID)
                          VALUES (@NewFeesId, @SUO_UserID, @SUO_DeptID, 0, @SUO_UserID, GETDATE(), '', @SUO_CompID);"
                    ,
                     new { SUO_UserID = userId,  SUO_DeptID = Org.Org_Node, SUO_CompID = compID },
                      transaction);
                    await transaction.CommitAsync();
                }
            }
        }

        public async Task<IEnumerable<CabinetDto>> LoadCabinetAsync(int deptId, int userId, int compID)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            //await connection.OpenAsync();

            //string dbName1 = _httpContextAccessor.HttpContext?.Request.Headers["CustomerCode"];

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            //string dbName = _httpContextAccessor.HttpContext?.Request.Headers["X-Customer-Code"].ToString();




           // string dbName = _httpContextAccessor.HttpContext?.Request.Headers["X-Customer-Code"].ToString();

            if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			// CheckandInsertMemberGroupAsync(userId, compID);
			string query = @"
            select CBN_ID, CBN_Name, CBN_SubCabCount,CBN_FolderCount,usr_FullName as CBN_CreatedBy,CBN_CreatedOn,CBN_DelFlag
            from edt_Cabinet A join sad_UserDetails B on A.CBN_CreatedBy = B.Usr_ID where A.cbn_Status='A' and A.cbn_Department=@cbn_Department and A.cbn_userID=@cbn_userID ";

            var result = await connection.QueryAsync<CabinetDto>(query, new
            {
                cbn_Department = deptId,
                cbn_UserId = userId
            });
            return result;
        }

        public async Task<int> CreateCabinetAsync(string cabinetname, int deptId, int userId, int compID, CabinetDto dto)
        {
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            //await connection.OpenAsync();
            string dbName = _httpContextAccessor.HttpContext?.Request.Headers["X-Customer-Code"].ToString();


            //string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();


			using var transaction = connection.BeginTransaction();

            int existingTemplateCount = 0;
            if (deptId == 0)
            {
                existingTemplateCount = await connection.ExecuteScalarAsync<int>(@"Select * from edt_cabinet where CBN_Name=@cabinetname and CBN_ID <> 0 and  
                CBN_Parent =-1 And (CBN_DelFlag='A' or CBN_DelFlag='W')", new { cabinetname }, transaction);
            }
            else
            {
                existingTemplateCount = await connection.ExecuteScalarAsync<int>(@"Select * from edt_cabinet where CBN_Name=@cabinetname and CBN_Department=@deptId and
               CBN_ID <> 0  And CBN_Parent=-1 And (CBN_DelFlag='A' or CBN_DelFlag='W')", new { cabinetname, deptId }, transaction);
            }

            if (existingTemplateCount == 0)
            {
                dto.CBN_ID = await connection.ExecuteScalarAsync<int>(
                        @"DECLARE @TemplateId INT; SELECT @TemplateId = ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet;
                          INSERT INTO edt_cabinet (CBN_ID, CBN_Name, CBN_Parent, CBN_Note, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, CBN_CreatedBy, 
                          CBN_CreatedOn, CBN_Status, CBN_DelFlag, CBN_CompID, CBN_Retention)
                          VALUES ( @TemplateId, @CBN_Name, -1, @CBN_Name, @CBN_UserID, @CBN_Department, 0, 0, @CBN_UserID, GETDATE(), 'A','A', @CBN_CompID,'');
                          SELECT @TemplateId;",
                        new
                        {
                            CBN_Name = cabinetname, // Using method parameter
                            CBN_Note = cabinetname, // Assuming you want the note to be the cabinet name
                            CBN_UserID = userId,     // Using method parameter
                            CBN_Department = deptId, // Using method parameter
                            CBN_CreatedBy = userId,  // Assuming created by is the userId
                            CBN_CompID = compID    
                        },
                        transaction
                    );
            }
            await transaction.CommitAsync();
            return dto.CBN_ID ?? 0;
        }

        public async Task<int> UpdateCabinetAsync(string cabinetname, int CabinetId,int userID, int compID, CabinetDto dto)
        {
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();


			using var transaction = connection.BeginTransaction();
             
            //int iCbinID = await connection.ExecuteScalarAsync<int>(@"Select CBN_ID from edt_cabinet where CBN_Name=@cabinetname and CBN_ID = @CabinetId and  
            //    CBN_Parent =-1", new { cabinetname, CabinetId }, transaction);
             
            await connection.ExecuteAsync(
                                   @"UPDATE edt_cabinet SET CBN_Name = @CBN_Name, CBN_UpdatedBy = @CBN_UpdatedBy, CBN_UpdatedOn = GETDATE()  
                                WHERE CBN_ID = @CBN_ID and CBN_CompID=@CBN_CompID;",
                                   new
                                   {
                                       CBN_Name = cabinetname,
                                       CBN_UpdatedBy = userID,
                                       CBN_ID = CabinetId,
                                       CBN_CompID = compID
                                   }, transaction);


            await transaction.CommitAsync();
            return dto.CBN_ID ?? 0;
        }

        public async Task<string> CheckOrCreateCustomDirectory(string accessCodeDirectory, string sFolderName, string imgDocType)
        {
            if (!Directory.Exists(accessCodeDirectory))
            {
                Directory.CreateDirectory(accessCodeDirectory);
            }

            var sFoldersToCreate = new List<string> { "Tempfolder", sFolderName, imgDocType };

            foreach (var sFolder in sFoldersToCreate)
            {
                if (!string.IsNullOrEmpty(sFolder))
                {
                    accessCodeDirectory = Path.Combine(accessCodeDirectory.TrimEnd('\\'), sFolder);
                    if (!Directory.Exists(accessCodeDirectory))
                    {
                        Directory.CreateDirectory(accessCodeDirectory);
                    }
                }
            }
            return accessCodeDirectory;
        }

        public async Task<string> IndexDocuments(IndexDocumentDto dto)
        {
            string sStatus = "";

			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();


			using var transaction = connection.BeginTransaction();

            //To Get Image Path
            var AccessCodeDirectory = await connection.ExecuteScalarAsync<string>(@"Select sad_Config_Value from sad_config_settings where sad_Config_Key='ImgPath'  
                and sad_compid=@sad_compid", new { sad_compid = dto.CompID }, transaction);
            if (AccessCodeDirectory == "")
            {
                return sStatus = "Invalid Image Path.";
            }

            //Check User id is Valid
            var UserLoginName = await connection.ExecuteScalarAsync<string>(@"Select Usr_LoginName from sad_UserDetails where Usr_ID=@Usr_ID 
                and Usr_CompID=@Usr_CompID", new { Usr_ID = dto.UserID, Usr_CompID = dto.CompID }, transaction);
            if (UserLoginName == "")
            {
                return sStatus = "Invalid UserId.";
            }

            //Check for Cabinet Id is Valid
            var CabinetID = await connection.ExecuteScalarAsync<int>(@"Select Cbn_id from edt_Cabinet where Cbn_id=@Cbn_id and CBN_Parent = -1 and CBN_UserID=@CBN_UserID
                and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.CabinetID, CBN_UserID=dto.UserID, CBN_CompID = dto.CompID }, transaction);
            if (CabinetID == 0)
            {
                return sStatus = "Invalid Cabinet Id.";
            }

            //Check for SubCabinet Id is Valid
            var SubCabinetID = await connection.ExecuteScalarAsync<int>(@"Select Cbn_id from edt_Cabinet where Cbn_id=@Cbn_id and CBN_Parent=@CBN_Parent and CBN_UserID=@CBN_UserID
                and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.SubCabinetID, CBN_Parent= dto.CabinetID, CBN_UserID = dto.UserID, CBN_CompID = dto.CompID }, transaction);
            if (SubCabinetID == 0)
            {
                return sStatus = "Invalid SubCabinet Id.";
            }

            //Check for Folder Id is Valid
            var FolderId = await connection.ExecuteScalarAsync<int>(@"Select Fol_FolID from edt_folder where Fol_FolID=@Fol_FolID  and Fol_Cabinet=@Fol_Cabinet
                and FOL_CompID=@FOL_CompID", new { Fol_FolID = dto.FolderID, Fol_Cabinet=dto.SubCabinetID, FOL_CompID = dto.CompID }, transaction);
            if (FolderId == 0)
            {
                return sStatus = "Invalid Folder Id.";
            }

            //Check for Document Type Id is Valid
            var DocumentTypeID = await connection.ExecuteScalarAsync<int>(@"Select Fol_FolID from edt_Document_Type where Dot_DocTypeID=@Dot_DocTypeID  
                and DOT_CompID=@DOT_CompID", new { Dot_DocTypeID = dto.DocumentTypeID, DOT_CompID = dto.CompID }, transaction);
            if (DocumentTypeID == 0)
            {
                return sStatus = "Invalid Document Type Id.";
            }

            //Check title name exist or not
            int checkTitleName = await connection.ExecuteScalarAsync<int>(@"Select PGE_BaseName From edt_page where PGE_Title = @PGE_Title  
                and Pge_CompID=@Pge_CompID", new { PGE_TITLE = dto.Title, Pge_CompID = dto.CompID }, transaction);
            if (checkTitleName != 0)
            {
                return sStatus = "Title Already Exists.";
            }

            //To Get and Create Directory
            String sTempPath = await CheckOrCreateCustomDirectory(AccessCodeDirectory, UserLoginName, "Upload");

            //Check UploadFile valid
            if (dto.File == null || dto.File.Length == 0)
            {
                return sStatus = "Invalid file.";  
            }

            var sFileName = Path.GetFileName(dto.File.FileName);
            var fileExt = Path.GetExtension(sFileName)?.TrimStart('.');

            var sFullFilePath = Path.Combine(sTempPath, sFileName);
            using (var stream = new FileStream(sFullFilePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            //Get BaseName ID
            var BaseNameID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_BASENAME)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID", 
                new { Pge_CompID = dto.CompID }, transaction);

            //Get PageNo ID
            var PageNoID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_PAGENO)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID",
               new { Pge_CompID = dto.CompID }, transaction);

            var sObject = "";
            switch (fileExt.ToUpper())
            {
                case "TIF": case "TIFF": case "JPG": case "JPEG": case "BMP":
                case "BRK": case "CAL": case "CLP": case "DCX": case "EPS":
                case "ICO": case "IFF": case "IMT": case "ICA": case "PCT":
                case "PCX": case "PNG": case "PSD": case "RAS": case "SGI":
                case "TGA": case "XBM": case "XPM": case "XWD":
                    sObject = "IMAGE"; break;
                default:
                    sObject = "OLE"; break;
            }


            var insertQuery = @"
                INSERT INTO EDT_PAGE (PGE_BASENAME, PGE_CABINET, PGE_FOLDER, PGE_DOCUMENT_TYPE, PGE_TITLE, PGE_DATE, Pge_DETAILS_ID, Pge_CreatedBy, PGE_OBJECT, PGE_PAGENO, PGE_EXT, 
                PGE_KeyWORD, PGE_OCRText, PGE_SIZE, PGE_CURRENT_VER, PGE_STATUS, PGE_SubCabinet, PGE_QC_UsrGrpId, PGE_FTPStatus, PGE_batch_name, pge_OrignalFileName, PGE_BatchID,
                PGE_OCRDelFlag, Pge_CompID, pge_Delflag, PGE_RFID)
                VALUES (@PGE_BASENAME, @PGE_CABINET, @PGE_FOLDER, @PGE_DOCUMENT_TYPE, @PGE_TITLE, GETDATE(), @Pge_DETAILS_ID, @Pge_CreatedBy, @PGE_OBJECT, @PGE_PAGENO, @PGE_EXT,
                @PGE_KeyWORD,'',0,0,'A',@PGE_SubCabinet,0,'F',@PGE_batch_name,@pge_OrignalFileName,0,0,@Pge_CompID,'A','');";

            await connection.ExecuteAsync(insertQuery, new
            {
                PGE_BASENAME = BaseNameID,
                PGE_CABINET = dto.CabinetID,
                PGE_FOLDER = dto.FolderID,
                PGE_DOCUMENT_TYPE = dto.DocumentTypeID,
                PGE_TITLE = dto.Title,
                Pge_DETAILS_ID = BaseNameID,
                Pge_CreatedBy = dto.UserID,
                PGE_OBJECT = sObject,
                PGE_PAGENO = PageNoID,
                PGE_EXT = fileExt,
                PGE_KeyWORD = dto.Keyword,
                PGE_SubCabinet = dto.SubCabinetID,
                PGE_batch_name = BaseNameID,
                pge_OrignalFileName = sFileName,
                Pge_CompID = dto.CompID
            });

            //Check FIleIn DB true or false
            var CheckFileInDB = await connection.ExecuteScalarAsync<string>(@"Select SAD_Config_Value from sad_config_settings where SAD_Config_Key = 'FilesInDB'",
               new { Pge_CompID = dto.CompID }, transaction);

            if(CheckFileInDB.ToUpper() == "FALSE")
            {
                string sImagePath = ""; string sPaths = "";
                sImagePath = sImagePath + "\\BITMAPS\\" + BaseNameID + "\\301\\";

                if (!sImagePath.EndsWith("\\"))
                {
                    sPaths = sImagePath + "\\";
                }
                else
                {
                    sPaths = sImagePath;
                }

                if (!Directory.Exists(sPaths))
                {
                    Directory.CreateDirectory(sPaths);
                }

                if(fileExt.Contains(".") == false)
                {
                    fileExt = "." + fileExt;
                }
                sImagePath = sPaths + BaseNameID + fileExt;

                if( System.IO.File.Exists(sImagePath) == false)
                {
                    System.IO.File.Copy(sFullFilePath, sImagePath, true);
                }

                if (System.IO.File.Exists(sFullFilePath) == true)
                {
                    System.IO.File.Delete(sFullFilePath);
                }
                sStatus = "Indexed Successfully.";
            }

            await transaction.CommitAsync();
            return sStatus;
        }

		public async Task<IEnumerable<DescriptorDto>> LoadDescriptorAsync(int DescId, int compID)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			string query = "";

			if (DescId == 0)
            {
		        query = @"
                Select a.DES_ID,a.DESC_NAME,a.DESC_NOTE,c.DT_DataType,a.DESC_SIZE,a.DESC_CRON,a.DESC_DelFlag,b.usr_FullName as DESC_CRBY,a.DESC_DefaultValues
                From EDT_DESCRIPTOR a,Sad_UserDetails b,EDT_DESC_Type c where a.DESC_CRBY=b.usr_ID And c.DT_ID=a.DESC_DATATYPE
                and a.DESC_CompId = @DESC_CompId and a.DESC_DelFlag='A'";
				var result = await connection.QueryAsync<DescriptorDto>(query, new
				{
				
					DESC_CompId = compID
				});
				return result;
			}
            else
            {
		        query = @"
                Select a.DES_ID,a.DESC_NAME,a.DESC_NOTE,c.DT_DataType,a.DESC_SIZE,a.DESC_CRON,a.DESC_DelFlag,b.usr_FullName as DESC_CRBY,a.DESC_DefaultValues
                From EDT_DESCRIPTOR a,Sad_UserDetails b,EDT_DESC_Type c where a.DESC_CRBY=b.usr_ID And c.DT_ID=a.DESC_DATATYPE
                And a.DES_ID=@DES_ID and a.DESC_CompId = @DESC_CompId and a.DESC_DelFlag='A'";
				var result = await connection.QueryAsync<DescriptorDto>(query, new
				{
					DES_ID = DescId,
					DESC_CompId = compID
				});
				return result;
			}
			 
		}

		public async Task<int> CreateDescriptorAsync(string DESC_NAME, string DESC_NOTE, string DESC_DATATYPE, string DESC_SIZE, DescriptorDto dto)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			var DESCId = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(DESC_CompId)+1,1) FROM edt_descriptor Where DESC_CompId=@DESC_CompId",
				new { DESC_CompId = dto.DESC_CompId }, transaction);

            dto.DES_ID = DESCId;
		   var insertQuery = @"
                INSERT INTO edt_descriptor (DES_ID, DESC_NAME, DESC_NOTE, DESC_DATATYPE, DESC_SIZE, DESC_DefaultValues, DESC_STATUS, DESC_DelFlag, DESC_CRBY, DESC_CRON, DESC_CompId)
                VALUES (@DES_ID, @DESC_NAME, @DESC_NOTE, @DESC_DATATYPE, @DESC_SIZE,'','AR', 'A',1,GetDate(),@DESC_CompId);";

			await connection.ExecuteAsync(insertQuery, new
			{
				DES_ID = DESCId,
				DESC_NAME = DESC_NAME,
				DESC_NOTE = DESC_NOTE,
				DESC_DATATYPE = DESC_DATATYPE,
				DESC_SIZE = DESC_SIZE,
				DESC_CompId = 1
			}, transaction);
			await transaction.CommitAsync();
			return dto.DES_ID ?? 0;
		}

		public async Task<int> UpdateDescriptorAsync(DescriptorDto dto)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
             
			await connection.ExecuteAsync(
								   @"UPDATE edt_descriptor SET DESC_NAME = @DESC_NAME, DESC_UPDATEDBY = @DESC_UPDATEDBY, DESC_UPDATEDON = GETDATE()  
                                WHERE DES_ID = @DES_ID and DESC_CompId=@DESC_CompId;",
								   new
								   {
									   DESC_NAME = dto.DESC_NAME,
									   DESC_UPDATEDBY = 1,
									   DES_ID = dto.DES_ID,
									   DESC_CompId = dto.DESC_CompId
								   }, transaction);


			await transaction.CommitAsync();
			return dto.DES_ID ?? 0;
		}

		public async Task<IEnumerable<DocumentTypeDto>> LoadDocumentTypeAsync(int iDocTypeID, int iDepartmentID, DocumentTypeDto dto)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			string query = "";


			query = @"Select a.DOT_DOCTYPEID,a.DOT_DOCNAME,c.Usr_FullName as DOT_CRBY,a.DOT_NOTE,b.Org_Node As DOT_PGROUPID,
                b.Org_Name as DOT_PGROUP,a.DOT_CRON,a.DOT_STATUS,DOT_isGlobal,a.DOT_DelFlag 
                From EDT_DOCUMENT_TYPE a,Sad_Org_Structure b,Sad_UserDetails c Where a.DOT_PGROUP=Org_Node and c.Usr_ID= a.DOT_CRBY ";
            if (iDocTypeID > 0)
            {
                query = query + "And a.DOT_DOCTYPEID=@DOT_DOCTYPEID ";
            }

            if (iDepartmentID > 0)
            {
                query = query + "And b.Org_Node = @DOT_PGROUP";
            }

            var result = await connection.QueryAsync<DocumentTypeDto>(query, new
			{
				DOT_DOCTYPEID = iDocTypeID,
				DOT_DOCNAME = dto.DOT_DOCNAME,
				DOT_NOTE = dto.DOT_NOTE,
				DOT_PGROUP = iDepartmentID,
				DOT_CRBY = dto.DOT_CRBY,
				DOT_CRON = dto.DOT_CRON,
				DOT_STATUS = dto.DOT_STATUS,
				DOT_Department = dto.DOT_PGROUP
			});
			return result;
		}

		public async Task<int> CreateDescriptorAsync(string DocumentName, string DocumentNote, string DepartmentId, [FromBody] DocumentTypeDto dto)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			var DocID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(DOT_DOCTYPEID)+1,1) FROM EDT_DOCUMENT_TYPE Where DOT_CompId=1",
				new { DESC_CompId = 1 }, transaction);
            dto.DOT_DOCTYPEID = DocID;
			var insertQuery = @"
                INSERT INTO EDT_DOCUMENT_TYPE (DOT_DOCTYPEID, DOT_DOCNAME, DOT_NOTE, DOT_PGROUP, DOT_CRBY, DOT_CRON, DOT_STATUS, DOT_CompId, DOT_IPAddress,DOT_DelFlag)
                VALUES (@DOT_DOCTYPEID, @DOT_DOCNAME, @DOT_NOTE, @DOT_PGROUP, @DOT_CRBY,GetDate(),'A',1,'','A');";

			await connection.ExecuteAsync(insertQuery, new
			{
				DOT_DOCTYPEID = DocID,
				DOT_DOCNAME = DocumentName,
				DOT_NOTE = DocumentNote,
				DOT_PGROUP = DepartmentId,
				DOT_CRBY = 1
			}, transaction);
			await transaction.CommitAsync();
			return dto.DOT_DOCTYPEID ?? 0;
		}

		public async Task<int> UpdateDocumentTypeAsync(int iDocTypeID, string DocumentName, string DocumentNote,  [FromBody] DocumentTypeDto dto)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

            dto.DOT_DOCTYPEID = iDocTypeID;
			await connection.ExecuteAsync(
				@"UPDATE EDT_DOCUMENT_TYPE SET DOT_DOCNAME = @DOT_DOCNAME,DOT_NOTE=@DOT_NOTE,DOT_UPDATEDBY = @DOT_UPDATEDBY, DOT_UPDATEDON = GETDATE()  
                                WHERE DOT_DOCTYPEID = @DOT_DOCTYPEID and DOT_CompId=@DOT_CompId;",
								   new
								   {
									   DOT_DOCTYPEID = iDocTypeID,
									   DOT_DOCNAME = DocumentName,
									   DOT_NOTE = DocumentName,
									   DOT_UPDATEDBY = 1,
									   DOT_CompId = 1
								   }, transaction);



			await transaction.CommitAsync();
			return dto.DOT_DOCTYPEID ?? 0;
		}



        //public async Task<IEnumerable<CabinetDto>> SearchDocumentsAsync(string sValue)
        //{
        //	using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //	await connection.OpenAsync();

        //          string query = @"select * from edt_page where PGE_Details_ID in(Select distinct(PGE_Details_ID) from edt_page where PGE_Status='A'  and ( PGE_KeyWord like '%;@PGE_KeyWord;%' or PGE_KeyWord like '%;@PGE_KeyWord;%' or PGE_KeyWord like '%;@PGE_KeyWord;%'  or PGE_KeyWord like '%;@PGE_KeyWord;%' or PGE_KeyWord=@PGE_KeyWord) or ( pge_Title like  '%;@pge_Title;%' or pge_Title like ';@pge_Title;%'  or pge_Title like '%;@pge_Title;' or pge_Title=@pge_Title) or PGE_Details_ID in (Select distinct(epd_baseid) from edt_page_details where EPD_Value Like '%'@EPD_Value'%' or EPD_Value like ';@EPD_Value;%'  or EPD_Value like '%;@EPD_Value;' or EPD_Value=@EPD_Value)) and  Pge_Status='A' order by pge_BaseName";

        //	var result = await connection.QueryAsync<CabinetDto>(query, new
        //	{
        //		PGE_KeyWord = sValue,
        //		pge_Title = sValue,
        //		EPD_Value = sValue,
        //	});
        //	return result;
        //}


        //public async Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    await connection.OpenAsync();

        //    string query = @"
        //select * from edt_page 
        //where PGE_Details_ID in (
        //    Select distinct(PGE_Details_ID) from edt_page 
        //    where PGE_Status='A' and 
        //    (
        //        PGE_KeyWord like '%@PGE_KeyWord%' 
        //        or PGE_KeyWord like '%@PGE_KeyWord%' 
        //        or PGE_KeyWord like '%@PGE_KeyWord%'  
        //        or PGE_KeyWord like '%@PGE_KeyWord%' 
        //        or PGE_KeyWord = @PGE_KeyWord
        //    ) 
        //    or 
        //    (
        //        pge_Title like '%@pge_Title%' 
        //        or pge_Title like '@pge_Title%'  
        //        or pge_Title like '%@pge_Title' 
        //        or pge_Title = @pge_Title
        //    ) 
        //    or PGE_Details_ID in (
        //        Select distinct(epd_baseid) from edt_page_details 
        //        where EPD_Value like '%@EPD_Value%' 
        //        or EPD_Value like '@EPD_Value%'  
        //        or EPD_Value like '%@EPD_Value' 
        //        or EPD_Value = @EPD_Value
        //    )
        //) 
        //and Pge_Status='A' 
        //order by pge_BaseName";

        //    var result = await connection.QueryAsync<SearchDto>(query, new
        //    {
        //        PGE_KeyWord = sValue,
        //        pge_Title = sValue,
        //        EPD_Value = sValue,
        //    });
        //    return result;
        //}



        //public async Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue)
        //{
        //	using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //	await connection.OpenAsync();

        //	string query = @"
        //                          select Cab.CBN_Name as Cabinet,SubCab.CBN_Name as SubCabinet,Fol.Fol_Name as Folder,A.PGE_Title as Title,A.PGE_Ext as Extension from edt_page A join edt_Cabinet Cab on A.PGE_CABINET = cab.CBN_ID join EDT_Cabinet SubCab on A.PGE_SubCabinet = SubCab.cbn_Id
        //                          join edt_Folder Fol on A.PGE_FOLDER = Fol.FOL_FolID 
        //                          where A.PGE_Details_ID in (
        //                              Select distinct(PGE_Details_ID) from edt_page 
        //                              where PGE_Status='A' and 
        //                              (
        //                                  PGE_KeyWord like '%' + @PGE_KeyWord + '%' 
        //                                  or PGE_KeyWord like '%;' + @PGE_KeyWord + ';%' 
        //                                  or PGE_KeyWord like '%' + @PGE_KeyWord + ';%'  
        //                                  or PGE_KeyWord like '%;' + @PGE_KeyWord + '%' 
        //                                  or PGE_KeyWord = @PGE_KeyWord
        //                              ) 
        //                              or 
        //                              (
        //                                  pge_Title like '%' + @pge_Title + '%' 
        //                                  or pge_Title like ';' + @pge_Title + ';%'  
        //                                  or pge_Title like '%' + @pge_Title + ';' 
        //                                  or pge_Title = @pge_Title
        //                              ) 
        //                              or PGE_Details_ID in (
        //                                  Select distinct(epd_baseid) from edt_page_details 
        //                                  where EPD_Value like '%' + @EPD_Value + '%' 
        //                                  or EPD_Value like ';' + @EPD_Value + ';%'  
        //                                  or EPD_Value like '%' + @EPD_Value + ';' 
        //                                  or EPD_Value = @EPD_Value
        //                              )
        //                              or PGE_Cabinet in (
        //                                  Select distinct(CBN_ID) from edt_Cabinet 
        //                                  where CBN_Name like '%' + @pge_Title + '%' 
        //                                  or CBN_Name like ';' + @pge_Title + ';%'  
        //                                  or CBN_Name like '%' + @pge_Title + ';' 
        //                                  or CBN_Name = @pge_Title
        //                                          )
        //	                    or PGE_SubCabinet in (
        //                                  Select distinct(CBN_ID) from edt_Cabinet 
        //                                  where CBN_Name like '%' + @pge_Title + '%' 
        //                                  or CBN_Name like ';' + @pge_Title + ';%'  
        //                                  or CBN_Name like '%' + @pge_Title + ';' 
        //                                  or CBN_Name = @pge_Title
        //                                          )
        //	                    or PGE_Folder in (
        //                                  Select distinct(Fol_FolID) from edt_Folder 
        //                                  where FOL_Name like '%' + @pge_Title + '%' 
        //                                  or FOL_Name like ';' + @pge_Title + ';%'  
        //                                  or FOL_Name like '%' + @pge_Title + ';' 
        //                                  or FOL_Name = @pge_Title


        //                              )              
        //                          ) 
        //                          and A.Pge_Status='A' 
        //                          order by A.pge_BaseName";

        //	var result = await connection.QueryAsync<SearchDto>(query, new
        //	{
        //		PGE_KeyWord = sValue,
        //		pge_Title = sValue,
        //		EPD_Value = sValue,
        //	});

        //	if (result == null || !result.Any())
        //	{
        //		throw new Exception("No data found for the given criteria.");
        //	}

        //	return result;
        //}


        public async Task<IEnumerable<SearchDto>> SearchDocumentsAsync(string sValue)
        {
            if (string.IsNullOrWhiteSpace(sValue))
            {
                return Enumerable.Empty<SearchDto>();
            }

            try
            {
				//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
				//await connection.OpenAsync();

				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				// ✅ Step 2: Get the connection string
				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				string query = @"
                  SELECT Cab.CBN_Name AS Cabinet, SubCab.CBN_Name AS SubCabinet, Fol.Fol_Name AS Folder, 
                        A.PGE_Title AS Title, A.PGE_Ext AS Extension,PGE_BASENAME,
                        (select SAD_Config_Value from [Sad_Config_Settings] where sad_Config_key='DisplayPath') +
						'BITMAPS\' + CAST(FLOOR(CAST(PGE_BASENAME AS numeric)/301) AS varchar) + '\' +
						CAST(PGE_BASENAME AS varchar) + '.'  + A.PGE_Ext AS URLPath
                        FROM edt_page A 
                        JOIN edt_Cabinet Cab ON A.PGE_CABINET = Cab.CBN_ID 
                        JOIN EDT_Cabinet SubCab ON A.PGE_SubCabinet = SubCab.cbn_Id
                        JOIN edt_Folder Fol ON A.PGE_FOLDER = Fol.FOL_FolID 
                        WHERE A.Pge_Status = 'A' 
                        AND (
                                A.PGE_KeyWord LIKE '%' + @SearchTerm + '%'
                                OR A.pge_Title LIKE '%' + @SearchTerm + '%'
                        OR EXISTS (
                                SELECT 1 FROM edt_page_details 
                                WHERE epd_baseid = A.PGE_Details_ID 
                                AND EPD_Value LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Cabinet 
                                WHERE CBN_ID = A.PGE_Cabinet 
                                AND CBN_Name LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Cabinet 
                                WHERE CBN_ID = A.PGE_SubCabinet 
                                AND CBN_Name LIKE '%' + @SearchTerm + '%'
                        )
                        OR EXISTS (
                                SELECT 1 FROM edt_Folder 
                                WHERE Fol_FolID = A.PGE_Folder 
                                AND FOL_Name LIKE '%' + @SearchTerm + '%'
                        )
                  )
                  ORDER BY A.pge_BaseName";

                var result = await connection.QueryAsync<SearchDto>(query, new { SearchTerm = sValue });
                return result ?? Enumerable.Empty<SearchDto>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }



		public async Task<IEnumerable<CabinetDto>> LoadRententionDataAsync(int compID)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
			//await connection.OpenAsync();

			//string dbName1 = _httpContextAccessor.HttpContext?.Request.Headers["CustomerCode"];

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
             
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			// CheckandInsertMemberGroupAsync(userId, compID);
			string query = @"
            select CBN_ID, CBN_Name, CBN_SubCabCount,CBN_FolderCount,usr_FullName as CBN_CreatedBy,CBN_CreatedOn,CBN_DelFlag,CONVERT(varchar(10), CBN_DocumentExpiryDate, 103) as CBN_DocumentExpiryDate,CBN_ReminderDay
            from edt_Cabinet A join sad_UserDetails B on A.CBN_CreatedBy = B.Usr_ID where A.cbn_Status='A' and A.CBN_CompID=@CBN_CompID and CBN_DocumentExpiryDate != '' and CBN_ReminderDay != '' 
            and  cbn_Parent = -1";

			var result = await connection.QueryAsync<CabinetDto>(query, new
			{
				CBN_CompID = compID,
			});
			return result;
		}
	}
}


