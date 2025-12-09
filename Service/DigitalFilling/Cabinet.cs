using Dapper;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using StackExchange.Redis;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net.NetworkInformation;
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
using TracePca.Interface.Master;
using Google.Apis.Util;
using File = System.IO.File;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;

namespace TracePca.Service.DigitalFilling
{
    public class Cabinet : CabinetInterface
    {

        private static readonly string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private readonly string _credentialsPath;
        private readonly IWebHostEnvironment _env;
        private readonly DbConnectionProvider _dbConnectionProvider;
        private readonly string _logFilePath;
        private readonly bool _isDevelopment;
        private readonly IGoogleDriveService _driveService;

        public Cabinet(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
             
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionString = GetConnectionStringFromSession();

            _isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            string localPath = @"\\MMCS-SERVER19\EMP_Backup\Googledrivetoken\client_secret_desktop.json";
            string cloudPath = @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\GoogleDrive\client_secret.json";
            _credentialsPath = _isDevelopment ? localPath : cloudPath;

            _logFilePath = _isDevelopment
                ? @"D:\Projects\Gitlab\TraceAPI - Backend Code\tracepa-dotnet-core\Logs\GoogleDriveLog.txt"
                : @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\Logs\GoogleDriveLog.txt";

            if (!File.Exists(_credentialsPath))
                throw new FileNotFoundException($"Google API credentials file not found at {_credentialsPath}");
        }

        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
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

        public async Task<IEnumerable<CabinetDto>> LoadCabinetAsync(int compID)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
             
            string query = @"SELECT CBN_ID, CBN_Name, CBN_SubCabCount,CBN_FolderCount,CBN_CreatedBy,CONVERT(VARCHAR(10), C.CBN_CreatedOn, 103) AS CBN_CreatedOn,CBN_DelFlag,CBN_Parent,CBN_UserID,CBN_Department,CBN_CompID,
				 CONVERT(VARCHAR(10), C.CBN_DocumentExpiryDate, 103) as CBN_DocumentExpiryDate
				FROM (SELECT A.CBN_ID, A.CBN_Name, A.CBN_SubCabCount,A.CBN_FolderCount,B.Usr_FullName AS CBN_CreatedBy,
				A.CBN_CreatedOn,A.CBN_DelFlag,A.CBN_AuditID,A.CBN_Parent,A.CBN_UserID,A.CBN_Department,A.CBN_CompID,
				CONVERT(VARCHAR(10), A.CBN_DocumentExpiryDate, 103) as CBN_DocumentExpiryDate 
				FROM edt_Cabinet A
				JOIN sad_UserDetails B ON A.CBN_CreatedBy = B.Usr_ID and CBN_Parent = -1 and CBN_Status='A' and CBN_AuditID = 0) C
				LEFT JOIN StandardAudit_Schedule D ON D.SA_ID = C.CBN_AuditID
				WHERE C.CBN_ID NOT IN (SELECT C1.CBN_ID FROM edt_Cabinet C1 JOIN StandardAudit_Schedule S1 ON S1.SA_ID = C1.CBN_AuditID
				WHERE S1.SA_ForCompleteAudit = 1 AND S1.SA_IsArchived = 1 and CBN_CompID=@CBN_CompID ) order by cbn_id";

            var result = await connection.QueryAsync<CabinetDto>(query, new
            {
                CBN_CompID = compID,
            });
            return result;

        }



        //     public async Task<IEnumerable<CabinetDto>> LoadCabinetAsync(int deptId, int userId, int compID)
        //     {
        //         //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //         //await connection.OpenAsync();

        //         //string dbName1 = _httpContextAccessor.HttpContext?.Request.Headers["CustomerCode"];

        //         string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //         //string dbName = _httpContextAccessor.HttpContext?.Request.Headers["X-Customer-Code"].ToString();




        //        // string dbName = _httpContextAccessor.HttpContext?.Request.Headers["X-Customer-Code"].ToString();

        //         if (string.IsNullOrEmpty(dbName))
        //	throw new Exception("CustomerCode is missing in session. Please log in again.");

        //// ✅ Step 2: Get the connection string
        //var connectionString = _configuration.GetConnectionString(dbName);

        //using var connection = new SqlConnection(connectionString);
        //await connection.OpenAsync();

        //// CheckandInsertMemberGroupAsync(userId, compID);
        //string query = @"
        //         select CBN_ID, CBN_Name, CBN_SubCabCount,CBN_FolderCount,usr_FullName as CBN_CreatedBy,CBN_CreatedOn,CBN_DelFlag
        //         from edt_Cabinet A join sad_UserDetails B on A.CBN_CreatedBy = B.Usr_ID where A.cbn_Status='A' ";  //and A.cbn_Department=@cbn_Department and A.cbn_userID=@cbn_userID 

        //var result = await connection.QueryAsync<CabinetDto>(query, new
        //         {
        //             cbn_Department = deptId,
        //             cbn_UserId = userId
        //         });
        //         return result;
        //     }

        //public async Task<int> CreateCabinetAsync(string cabinetname, int deptId, int userId, int compID, CabinetDto dto)
        //      {


        //	string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //	if (string.IsNullOrEmpty(dbName))
        //		throw new Exception("CustomerCode is missing in session. Please log in again.");

        //	var connectionString = _configuration.GetConnectionString(dbName);

        //	using var connection = new SqlConnection(connectionString);
        //	await connection.OpenAsync();


        //	using var transaction = connection.BeginTransaction();

        //          int existingTemplateCount = 0;
        //          if (deptId == 0)
        //          {
        //              existingTemplateCount = await connection.ExecuteScalarAsync<int>(@"Select * from edt_cabinet where CBN_Name=@cabinetname and CBN_ID <> 0 and  
        //              CBN_Parent =-1 And (CBN_DelFlag='A' or CBN_DelFlag='W')", new { cabinetname }, transaction);
        //          }
        //          else
        //          {
        //              existingTemplateCount = await connection.ExecuteScalarAsync<int>(@"Select * from edt_cabinet where CBN_Name=@cabinetname and CBN_Department=@deptId and
        //             CBN_ID <> 0  And CBN_Parent=-1 And (CBN_DelFlag='A' or CBN_DelFlag='W')", new { cabinetname, deptId }, transaction);
        //          }

        //          if (existingTemplateCount == 0)
        //          {
        //              dto.CBN_ID = await connection.ExecuteScalarAsync<int>(
        //                      @"DECLARE @TemplateId INT; SELECT @TemplateId = ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet;
        //                        INSERT INTO edt_cabinet (CBN_ID, CBN_Name, CBN_Parent, CBN_Note, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, CBN_CreatedBy, 
        //                        CBN_CreatedOn, CBN_Status, CBN_DelFlag, CBN_CompID, CBN_Retention)
        //                        VALUES ( @TemplateId, @CBN_Name, -1, @CBN_Name, @CBN_UserID, @CBN_Department, 0, 0, @CBN_UserID, GETDATE(), 'A','A', @CBN_CompID,'');
        //                        SELECT @TemplateId;",
        //                      new
        //                      {
        //                          CBN_Name = cabinetname, // Using method parameter
        //                          CBN_Note = cabinetname, // Assuming you want the note to be the cabinet name
        //                          CBN_UserID = userId,     // Using method parameter
        //                          CBN_Department = deptId, // Using method parameter
        //                          CBN_CreatedBy = userId,  // Assuming created by is the userId
        //                          CBN_CompID = compID    
        //                      },
        //                      transaction
        //                  );
        //          }
        //          await transaction.CommitAsync();
        //          return dto.CBN_ID ?? 0;
        //      }


        public async Task<int> CreateCabinetAsync(string cabinetname, int deptId, int userId, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
			try
			{
				int existingTemplateCount = 0;

				if (deptId == 0)
				{
					existingTemplateCount = await connection.ExecuteScalarAsync<int>(
						@"SELECT COUNT(*) FROM edt_cabinet 
                  WHERE CBN_Name=@cabinetname AND CBN_ID<>0 AND CBN_Parent=-1 AND (CBN_DelFlag='A' OR CBN_DelFlag='W')",
						new { cabinetname },
						transaction);
				}
				else
				{
					existingTemplateCount = await connection.ExecuteScalarAsync<int>(
						@"SELECT COUNT(*) FROM edt_cabinet 
                  WHERE CBN_Name=@cabinetname AND CBN_Department=@deptId AND CBN_ID<>0 AND CBN_Parent=-1 AND (CBN_DelFlag='A' OR CBN_DelFlag='W')",
						new { cabinetname, deptId },
						transaction);
				}
				int CBN_ID = 0;
				if (existingTemplateCount == 0)
				{
					  CBN_ID = await connection.ExecuteScalarAsync<int>(
						@"DECLARE @TemplateId INT;
                  SELECT @TemplateId = ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet;
                  INSERT INTO edt_cabinet 
                  (CBN_ID, CBN_Name, CBN_Parent, CBN_Note, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, 
                   CBN_CreatedBy, CBN_CreatedOn, CBN_Status, CBN_DelFlag, CBN_CompID,CBN_AuditID)
                  VALUES 
                  (@TemplateId, @CBN_Name, -1, @CBN_Name, @CBN_UserID, @CBN_Department, 0, 0, @CBN_UserID, GETDATE(), 'A','A', @CBN_CompID,0);
                  SELECT @TemplateId;",
						new
						{
							CBN_Name = cabinetname,
							CBN_UserID = userId,
							CBN_Department = deptId,
							CBN_CompID = compID
						},
						transaction
					);
				}

				await transaction.CommitAsync();
				return CBN_ID;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<int> UpdateCabinetAsync(string cabinetName, int cabinetId, int userId, int compId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				var rowsAffected = await connection.ExecuteAsync(
					@"UPDATE edt_cabinet 
              SET CBN_Name = @CBN_Name, 
                  CBN_UpdatedBy = @CBN_UpdatedBy, 
                  CBN_UpdatedOn = GETDATE()  
              WHERE CBN_ID = @CBN_ID and CBN_CompID = @CBN_CompID",
					new
					{
						CBN_Name = cabinetName,
						CBN_UpdatedBy = userId,
						CBN_ID = cabinetId,
						CBN_CompID = compId
					},
					transaction
				);

				await transaction.CommitAsync();
 
				return rowsAffected > 0 ? cabinetId : 0;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
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
            string sStatus = ""; int iDocID = 0;
            string nameOnly = string.Empty;
            string extension = string.Empty;

            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

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
          and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.CabinetID, CBN_UserID = dto.UserID, CBN_CompID = dto.CompID }, transaction);
            if (CabinetID == 0)
            {
                return sStatus = "Invalid Cabinet Id.";
            }

            //Check for SubCabinet Id is Valid
            var SubCabinetID = await connection.ExecuteScalarAsync<int>(@"Select Cbn_id from edt_Cabinet where Cbn_id=@Cbn_id and CBN_Parent=@CBN_Parent and CBN_UserID=@CBN_UserID
          and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.SubCabinetID, CBN_Parent = dto.CabinetID, CBN_UserID = dto.UserID, CBN_CompID = dto.CompID }, transaction);
            if (SubCabinetID == 0)
            {
                return sStatus = "Invalid SubCabinet Id.";
            }

            //Check for Folder Id is Valid
            var FolderId = await connection.ExecuteScalarAsync<int>(@"Select Fol_FolID from edt_folder where Fol_FolID=@Fol_FolID  and Fol_Cabinet=@Fol_Cabinet
          and FOL_CompID=@FOL_CompID", new { Fol_FolID = dto.FolderID, Fol_Cabinet = dto.SubCabinetID, FOL_CompID = dto.CompID }, transaction);
            if (FolderId == 0)
            {
                return sStatus = "Invalid Folder Id.";
            }

            //Check for Document Type Id is Valid
            var DocumentTypeID = await connection.ExecuteScalarAsync<int>(@"Select DOT_DoctypeID from edt_Document_Type where Dot_DocTypeID=@Dot_DocTypeID  
          and DOT_CompID=@DOT_CompID", new { Dot_DocTypeID = dto.DocumentTypeID, DOT_CompID = dto.CompID }, transaction);
            if (DocumentTypeID == 0)
            {
                //return sStatus = "Invalid Document Type Id.";
                dto.DocumentTypeID = 1;
            }

            //Check title name exist or not
            int checkTitleName = await connection.ExecuteScalarAsync<int>(@"Select PGE_BaseName From edt_page where PGE_Title = @PGE_Title  
          and Pge_CompID=@Pge_CompID", new { PGE_TITLE = dto.Title, Pge_CompID = dto.CompID }, transaction);
            if (checkTitleName != 0)
            {
                return sStatus = "Title Already Exists.";
            }

            //To Get and Create Directory
            //String sTempPath = await CheckOrCreateCustomDirectory(AccessCodeDirectory, UserLoginName, "Upload");
            //var sTempPath = EnsureDirectoryExists(AccessCodeDirectory, UserLoginName, "Upload");

            //Check UploadFile valid
            if (dto.File == null || dto.File.Length == 0)
            {
                return sStatus = "Invalid file.";
            }

            var sFileName = Path.GetFileName(dto.File.FileName);
            var fileExt = Path.GetExtension(sFileName)?.TrimStart('.');

            //var sFullFilePath = Path.Combine(sTempPath, sFileName);
            //using (var stream = new FileStream(sFullFilePath, FileMode.Create))
            //{
            //	await dto.File.CopyToAsync(stream);
            //}

            //Get BaseName ID
            var BaseNameID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_BASENAME)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID",
                      new { Pge_CompID = dto.CompID }, transaction);
            iDocID = BaseNameID;
            //Get PageNo ID
            var PageNoID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_PAGENO)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID",
               new { Pge_CompID = dto.CompID }, transaction);

            var sObject = "";
            switch (fileExt.ToUpper())
            {
                case "TIF":
                case "TIFF":
                case "JPG":
                case "JPEG":
                case "BMP":
                case "BRK":
                case "CAL":
                case "CLP":
                case "DCX":
                case "EPS":
                case "ICO":
                case "IFF":
                case "IMT":
                case "ICA":
                case "PCT":
                case "PCX":
                case "PNG":
                case "PSD":
                case "RAS":
                case "SGI":
                case "TGA":
                case "XBM":
                case "XPM":
                case "XWD":
                    sObject = "IMAGE"; break;
                default:
                    sObject = "OLE"; break;
            }


            if (dto.RentensionDate != null && dto.RentensionDate != "" && dto.RentensionDate != "string")
            {
                var sSql = @"Update edt_Cabinet set CBN_DocumentExpiryDate= @CBN_DocumentExpiryDate where CBN_ID =@CBN_ID";
                await connection.ExecuteAsync(sSql, new
                {
                    CBN_DocumentExpiryDate = dto.RentensionDate,
                    CBN_ID = dto.CabinetID
                }, transaction);
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
                //PGE_KeyWORD = dto.Keyword,
                PGE_KeyWORD = string.IsNullOrEmpty(dto.Keyword) ? "" : dto.Keyword,
                PGE_SubCabinet = dto.SubCabinetID,
                PGE_batch_name = BaseNameID,
                pge_OrignalFileName = sFileName,
                Pge_CompID = dto.CompID
            }, transaction);

            ////Check FIleIn DB true or false
            //var CheckFileInDB = await connection.ExecuteScalarAsync<string>(@"Select SAD_Config_Value from sad_config_settings where SAD_Config_Key = 'FilesInDB'",
            //   new { Pge_CompID = dto.CompID }, transaction);

            //if (CheckFileInDB.ToUpper() == "FALSE")
            //{
            //    string sImagePath = ""; string sPaths = "";
            //    sImagePath = sImagePath + "\\BITMAPS\\" + BaseNameID + "\\301\\";

            //    if (!sImagePath.EndsWith("\\"))
            //    {
            //        sPaths = sImagePath + "\\";
            //    }
            //    else
            //    {
            //        sPaths = sImagePath;
            //    }

            //    if (!Directory.Exists(sPaths))
            //    {
            //        Directory.CreateDirectory(sPaths);
            //    }

            //    if (fileExt.Contains(".") == false)
            //    {
            //        fileExt = "." + fileExt;
            //    }
            //    sImagePath = sPaths + BaseNameID + fileExt;

            //    if (System.IO.File.Exists(sImagePath) == false)
            //    {
            //        System.IO.File.Copy(sFullFilePath, sImagePath, true);
            //    }

            //    if (System.IO.File.Exists(sFullFilePath) == true)
            //    {
            //        System.IO.File.Delete(sFullFilePath);
            //    }
            //    sStatus = "Indexed Successfully.";
            //}


            dynamic uploadedFile = await UploadFileToFolderAsyncNew(dto.File, "TracePA/DigitalFillings", GetUserEmail(), iDocID);
            if (uploadedFile != null)
            {
                // Check status
                if (uploadedFile.Status == "Success")
                {
                    string fullFileName = uploadedFile.File.Name;
                    nameOnly = Path.GetFileNameWithoutExtension(fullFileName);
                    extension = Path.GetExtension(fullFileName);
                }
                else if (uploadedFile.Status == "Error")
                {
                    string errorMessage = uploadedFile.Message ?? "Unknown Google Drive upload error.";
                }
            }

            await transaction.CommitAsync();
            return $"File uploaded Successfully.";
        }

        //     public async Task<string> IndexDocuments(IndexDocumentDto dto)
        //     {
        //         string sStatus = "";

        ////using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        ////await connection.OpenAsync();

        //string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

        //if (string.IsNullOrEmpty(dbName))
        //	throw new Exception("CustomerCode is missing in session. Please log in again.");

        //// ✅ Step 2: Get the connection string
        //var connectionString = _configuration.GetConnectionString(dbName);

        //using var connection = new SqlConnection(connectionString);
        //await connection.OpenAsync();


        //using var transaction = connection.BeginTransaction();

        //         //To Get Image Path
        //         var AccessCodeDirectory = await connection.ExecuteScalarAsync<string>(@"Select sad_Config_Value from sad_config_settings where sad_Config_Key='ImgPath'  
        //             and sad_compid=@sad_compid", new { sad_compid = dto.CompID }, transaction);
        //         if (AccessCodeDirectory == "")
        //         {
        //             return sStatus = "Invalid Image Path.";
        //         }

        //         //Check User id is Valid
        //         var UserLoginName = await connection.ExecuteScalarAsync<string>(@"Select Usr_LoginName from sad_UserDetails where Usr_ID=@Usr_ID 
        //             and Usr_CompID=@Usr_CompID", new { Usr_ID = dto.UserID, Usr_CompID = dto.CompID }, transaction);
        //         if (UserLoginName == "")
        //         {
        //             return sStatus = "Invalid UserId.";
        //         }

        //         //Check for Cabinet Id is Valid
        //         var CabinetID = await connection.ExecuteScalarAsync<int>(@"Select Cbn_id from edt_Cabinet where Cbn_id=@Cbn_id and CBN_Parent = -1 and CBN_UserID=@CBN_UserID
        //             and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.CabinetID, CBN_UserID=dto.UserID, CBN_CompID = dto.CompID }, transaction);
        //         if (CabinetID == 0)
        //         {
        //             return sStatus = "Invalid Cabinet Id.";
        //         }

        //         //Check for SubCabinet Id is Valid
        //         var SubCabinetID = await connection.ExecuteScalarAsync<int>(@"Select Cbn_id from edt_Cabinet where Cbn_id=@Cbn_id and CBN_Parent=@CBN_Parent and CBN_UserID=@CBN_UserID
        //             and CBN_CompID=@CBN_CompID", new { Cbn_id = dto.SubCabinetID, CBN_Parent= dto.CabinetID, CBN_UserID = dto.UserID, CBN_CompID = dto.CompID }, transaction);
        //         if (SubCabinetID == 0)
        //         {
        //             return sStatus = "Invalid SubCabinet Id.";
        //         }

        //         //Check for Folder Id is Valid
        //         var FolderId = await connection.ExecuteScalarAsync<int>(@"Select Fol_FolID from edt_folder where Fol_FolID=@Fol_FolID  and Fol_Cabinet=@Fol_Cabinet
        //             and FOL_CompID=@FOL_CompID", new { Fol_FolID = dto.FolderID, Fol_Cabinet=dto.SubCabinetID, FOL_CompID = dto.CompID }, transaction);
        //         if (FolderId == 0)
        //         {
        //             return sStatus = "Invalid Folder Id.";
        //         }

        //         //Check for Document Type Id is Valid
        //         var DocumentTypeID = await connection.ExecuteScalarAsync<int>(@"Select DOT_DoctypeID from edt_Document_Type where Dot_DocTypeID=@Dot_DocTypeID  
        //             and DOT_CompID=@DOT_CompID", new { Dot_DocTypeID = dto.DocumentTypeID, DOT_CompID = dto.CompID }, transaction);
        //         if (DocumentTypeID == 0)
        //         {
        //	//return sStatus = "Invalid Document Type Id.";
        //	dto.DocumentTypeID = 1;
        //         }

        //         //Check title name exist or not
        //         int checkTitleName = await connection.ExecuteScalarAsync<int>(@"Select PGE_BaseName From edt_page where PGE_Title = @PGE_Title  
        //             and Pge_CompID=@Pge_CompID", new { PGE_TITLE = dto.Title, Pge_CompID = dto.CompID }, transaction);
        //         if (checkTitleName != 0)
        //         {
        //             return sStatus = "Title Already Exists.";
        //         }

        //         //To Get and Create Directory
        //         String sTempPath = await CheckOrCreateCustomDirectory(AccessCodeDirectory, UserLoginName, "Upload");

        //         //Check UploadFile valid
        //         if (dto.File == null || dto.File.Length == 0)
        //         {
        //             return sStatus = "Invalid file.";  
        //         }

        //         var sFileName = Path.GetFileName(dto.File.FileName);
        //         var fileExt = Path.GetExtension(sFileName)?.TrimStart('.');

        //         var sFullFilePath = Path.Combine(sTempPath, sFileName);
        //         using (var stream = new FileStream(sFullFilePath, FileMode.Create))
        //         {
        //             await dto.File.CopyToAsync(stream);
        //         }

        //         //Get BaseName ID
        //         var BaseNameID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_BASENAME)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID", 
        //             new { Pge_CompID = dto.CompID }, transaction);

        //         //Get PageNo ID
        //         var PageNoID = await connection.ExecuteScalarAsync<int>(@"Select ISNULL(MAX(PGE_PAGENO)+1,1) FROM edt_page Where Pge_CompID=@Pge_CompID",
        //            new { Pge_CompID = dto.CompID }, transaction);

        //         var sObject = "";
        //         switch (fileExt.ToUpper())
        //         {
        //             case "TIF": case "TIFF": case "JPG": case "JPEG": case "BMP":
        //             case "BRK": case "CAL": case "CLP": case "DCX": case "EPS":
        //             case "ICO": case "IFF": case "IMT": case "ICA": case "PCT":
        //             case "PCX": case "PNG": case "PSD": case "RAS": case "SGI":
        //             case "TGA": case "XBM": case "XPM": case "XWD":
        //                 sObject = "IMAGE"; break;
        //             default:
        //                 sObject = "OLE"; break;
        //         }


        //if(dto.RentensionDate != null && dto.RentensionDate != "" && dto.RentensionDate != "string")
        //{
        //	var sSql = @"Update edt_Cabinet set CBN_DocumentExpiryDate= @CBN_DocumentExpiryDate where CBN_ID =@CBN_ID";
        //	await connection.ExecuteAsync(sSql, new
        //	{
        //		CBN_DocumentExpiryDate = dto.RentensionDate,
        //		CBN_ID = dto.CabinetID
        //	}, transaction);
        //}


        //         var insertQuery = @"
        //             INSERT INTO EDT_PAGE (PGE_BASENAME, PGE_CABINET, PGE_FOLDER, PGE_DOCUMENT_TYPE, PGE_TITLE, PGE_DATE, Pge_DETAILS_ID, Pge_CreatedBy, PGE_OBJECT, PGE_PAGENO, PGE_EXT, 
        //             PGE_KeyWORD, PGE_OCRText, PGE_SIZE, PGE_CURRENT_VER, PGE_STATUS, PGE_SubCabinet, PGE_QC_UsrGrpId, PGE_FTPStatus, PGE_batch_name, pge_OrignalFileName, PGE_BatchID,
        //             PGE_OCRDelFlag, Pge_CompID, pge_Delflag, PGE_RFID)
        //             VALUES (@PGE_BASENAME, @PGE_CABINET, @PGE_FOLDER, @PGE_DOCUMENT_TYPE, @PGE_TITLE, GETDATE(), @Pge_DETAILS_ID, @Pge_CreatedBy, @PGE_OBJECT, @PGE_PAGENO, @PGE_EXT,
        //             @PGE_KeyWORD,'',0,0,'A',@PGE_SubCabinet,0,'F',@PGE_batch_name,@pge_OrignalFileName,0,0,@Pge_CompID,'A','');";

        //         await connection.ExecuteAsync(insertQuery, new
        //         {
        //             PGE_BASENAME = BaseNameID,
        //             PGE_CABINET = dto.CabinetID,
        //             PGE_FOLDER = dto.FolderID,
        //             PGE_DOCUMENT_TYPE = dto.DocumentTypeID,
        //             PGE_TITLE = dto.Title,
        //             Pge_DETAILS_ID = BaseNameID,
        //             Pge_CreatedBy = dto.UserID,
        //             PGE_OBJECT = sObject,
        //             PGE_PAGENO = PageNoID,
        //             PGE_EXT = fileExt,
        //	//PGE_KeyWORD = dto.Keyword,
        //	PGE_KeyWORD = string.IsNullOrEmpty(dto.Keyword) ? "" : dto.Keyword,
        //	PGE_SubCabinet = dto.SubCabinetID,
        //             PGE_batch_name = BaseNameID,
        //             pge_OrignalFileName = sFileName,
        //             Pge_CompID = dto.CompID
        //         }, transaction);

        //         //Check FIleIn DB true or false
        //         var CheckFileInDB = await connection.ExecuteScalarAsync<string>(@"Select SAD_Config_Value from sad_config_settings where SAD_Config_Key = 'FilesInDB'",
        //            new { Pge_CompID = dto.CompID }, transaction);

        //         if(CheckFileInDB.ToUpper() == "FALSE")
        //         {
        //             string sImagePath = ""; string sPaths = "";
        //             sImagePath = sImagePath + "\\BITMAPS\\" + BaseNameID + "\\301\\";

        //             if (!sImagePath.EndsWith("\\"))
        //             {
        //                 sPaths = sImagePath + "\\";
        //             }
        //             else
        //             {
        //                 sPaths = sImagePath;
        //             }

        //             if (!Directory.Exists(sPaths))
        //             {
        //                 Directory.CreateDirectory(sPaths);
        //             }

        //             if(fileExt.Contains(".") == false)
        //             {
        //                 fileExt = "." + fileExt;
        //             }
        //             sImagePath = sPaths + BaseNameID + fileExt;

        //             if( System.IO.File.Exists(sImagePath) == false)
        //             {
        //                 System.IO.File.Copy(sFullFilePath, sImagePath, true);
        //             }

        //             if (System.IO.File.Exists(sFullFilePath) == true)
        //             {
        //                 System.IO.File.Delete(sFullFilePath);
        //             }
        //             sStatus = "Indexed Successfully.";
        //         }

        //         await transaction.CommitAsync();
        //         return sStatus;
        //     }

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

		public async Task<int> UpdateDocumentTypeAsync(int iDocTypeID, string DocumentName, int userId, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				var rowsAffected = await connection.ExecuteAsync(
				@"UPDATE EDT_DOCUMENT_TYPE SET DOT_DOCNAME = @DOT_DOCNAME,DOT_NOTE=@DOT_DOCNAME,DOT_UPDATEDBY = @DOT_UPDATEDBY, DOT_UPDATEDON = GETDATE()  
                                WHERE DOT_DOCTYPEID = @DOT_DOCTYPEID and DOT_CompId=@DOT_CompId;",
								   new
								   {
									   DOT_DOCTYPEID = iDocTypeID,
									   DOT_DOCNAME = DocumentName,
									   DOT_NOTE = DocumentName,
									   DOT_UPDATEDBY = userId,
									   DOT_CompId = compID
								   }, transaction);



				await transaction.CommitAsync();
				return rowsAffected > 0 ? iDocTypeID : 0;
			}
			catch(Exception ex)
			{
				await transaction.RollbackAsync();
				throw;
			}
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
            from edt_Cabinet A join sad_UserDetails B on A.CBN_CreatedBy = B.Usr_ID join standardAudit_Schedule C On C.SA_ID = A.CBN_AuditID and C.SA_ForCompleteAudit = 1 where A.cbn_Status='A' and A.CBN_CompID=@CBN_CompID and CBN_DocumentExpiryDate != '' and CBN_ReminderDay != '' 
            and  cbn_Parent = -1";

			var result = await connection.QueryAsync<CabinetDto>(query, new
			{
				CBN_CompID = compID,
			});
			return result;
		}


		public async Task<IEnumerable<DocumentTypeDto>> LoadAllDocumentTypeAsync(int iCompID)
		{
			  
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			string query = @"Select a.DOT_DOCTYPEID,a.DOT_DOCNAME,c.Usr_FullName as DOT_CRBY,a.DOT_NOTE,b.Org_Node As DOT_PGROUPID,
                b.Org_Name as DOT_PGROUP,a.DOT_CRON,a.DOT_STATUS,DOT_isGlobal,a.DOT_DelFlag 
                From EDT_DOCUMENT_TYPE a,Sad_Org_Structure b,Sad_UserDetails c Where a.DOT_PGROUP=Org_Node and c.Usr_ID= a.DOT_CRBY and DOT_CompID=@DOT_CompID";

			var result = await connection.QueryAsync<DocumentTypeDto>(query, new
			{
				DOT_CompID = iCompID
			});
			return result;
		}


		public async Task<IEnumerable<ArchiveDetailsDto>> LoadArchiveDetailsAsync(int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			string query = @"SELECT A.SA_ID,A.SA_AuditNo,A.SA_ScopeOfAudit,
                            A.SA_CustID,A.SA_AuditTypeID,A.SA_PartnerID,A.SA_ReviewPartnerID,
                            A.SA_AttachID,A.SA_CompID,A.SA_StartDate,A.SA_ExpCompDate,
                            A.SA_AuditOpinionDate,A.SA_ExpiryDate,B.CUST_NAME,B.CUST_CODE,
                            CMM.CMM_Code,CMM.CMM_Desc,A.SA_RetentionPeriod,
                            ISNULL(D.AttachmentCount, 0) AS AttachmentCount,D.SA_AttachmentID
                            FROM StandardAudit_Schedule A
                            JOIN SAD_CUSTOMER_MASTER B ON B.Cust_Id = A.SA_CustID
                            JOIN Content_Management_Master CMM ON CMM_ID = A.SA_AuditTypeID
                            OUTER APPLY (SELECT COUNT(*) AS AttachmentCount,
                            STRING_AGG(CAST(SAR_AttchId AS varchar), ',') AS SA_AttachmentID FROM (
                            SELECT SAR_AttchId FROM StandardAudit_Audit_DRLLog_RemarksHistory
                            WHERE SAR_SA_ID = A.SA_ID AND SAR_AttchID <> 0 and SAR_EmailIds <> '') AS DistinctIds) D
                            WHERE A.SA_CompID = @SA_CompID AND A.SA_IsArchived = 1 AND A.SA_ForCompleteAudit = 1";   

			var result = await connection.QueryAsync<ArchiveDetailsDto>(query, new
			{
				SA_CompID = compID
			});
			return result;
		}


		public async Task<IEnumerable<ArchivedDocumentFileDto>> ArchivedDocumentFileDetailsAsync(string sAttachID)
		{
			if (string.IsNullOrWhiteSpace(sAttachID))
			{
				return Enumerable.Empty<ArchivedDocumentFileDto>();
			}

			try
			{
				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");
 
				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				//string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
				//                            SELECT DISTINCT ATCH_FName as FileName, (SELECT TOP 1 SAD_Config_Value FROM [Sad_Config_Settings] 
				//                                    WHERE sad_Config_key = 'DisplayPath') + 'BITMAPS\' 
				//                                + CAST(FLOOR(CAST(A.Atch_DocID AS numeric)/301) AS varchar) + '\' + CAST(A.Atch_DocID AS varchar) 
				//                                + '.' + A.ATCH_Ext AS URLPath FROM edt_Attachments A
				//                            JOIN (SELECT DISTINCT CAST(value AS INT) AS Atch_ID FROM STRING_SPLIT(@ids, ',')) S
				//                                ON A.Atch_ID = S.Atch_ID and A.Atch_FName != '' and atch_Ext != '';";

				//string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
				//				SELECT DISTINCT C.Fol_Name as FolderName, ATCH_FName as FileName, (SELECT TOP 1 SAD_Config_Value FROM [Sad_Config_Settings] 
				//				WHERE sad_Config_key = 'DisplayPath') + 'BITMAPS\' 
				//				+ CAST(FLOOR(CAST(A.Atch_DocID AS numeric)/301) AS varchar) + '\' + CAST(A.Atch_DocID AS varchar) 
				//				+ '.' + A.ATCH_Ext AS URLPath FROM edt_Attachments A
				//                            JOIN (SELECT DISTINCT CAST(value AS INT) AS Atch_ID FROM STRING_SPLIT(@ids, ',')) S
				//				ON A.Atch_ID = S.Atch_ID and A.Atch_FName != '' and atch_Ext != ''
				//				Left join edt_folder C on C.FOL_FolID = A.Atch_FolderId";

				//string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
				//					select B.Fol_Name as Foldername, ATCH_FName as FileName,Atch_Path AS URLPath from edt_Attachments A  
				//					JOIN (SELECT DISTINCT CAST(value AS INT) AS Atch_ID FROM STRING_SPLIT(@ids, ',')) S
				//					ON A.Atch_ID = S.Atch_ID and A.Atch_FName != '' and atch_Ext != ''
				//					left join edt_folder B on A.Atch_FolderId = B.FOL_FolID";


				string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
								SELECT B.FOL_Name AS FolderName, A.Atch_FName AS FileName,
								REPLACE(A.Atch_Path,'C:\inetpub\vhosts\multimedia.interactivedns.com\tracelites.multimedia.interactivedns.com\',
								'https:\\tracelites.multimedia.interactivedns.com\') AS URLPath
								FROM edt_Attachments A JOIN ( SELECT DISTINCT TRY_CAST(value AS INT) AS Atch_ID
								FROM STRING_SPLIT(@ids, ',') WHERE TRY_CAST(value AS INT) IS NOT NULL
								) S ON A.Atch_ID = S.Atch_ID
								LEFT JOIN edt_Folder B ON A.Atch_FolderId = B.FOL_FolID WHERE A.Atch_FName <> ''  AND A.Atch_Ext <> '';";

				var result = await connection.QueryAsync<ArchivedDocumentFileDto>(query, new { AttachIDs = sAttachID });
				return result ?? Enumerable.Empty<ArchivedDocumentFileDto>();
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public async Task<IEnumerable<DepartmentDto>> LoadAllDepartmentAsync(int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			// CheckandInsertMemberGroupAsync(userId, compID);
			string query = @"select Org_Name as DepartmentName, Org_Node as DepartmentID from sad_Org_Structure where Org_LevelCode = 3 and org_Code <> '' and org_name <>'' and Org_Status='A' and Org_CompID=@Org_CompID"; 
			var result = await connection.QueryAsync<DepartmentDto>(query, new
			{
				Org_CompID = compID
			});
			return result;
		}


		public async Task<string> CreateDepartmentAsync(string Code, string DepartmentName, string userId, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
			try
			{
				string sStatus = ""; int Org_Node=0;
				//Check for Cabinet Id is Valid
				var OrgNod = await connection.ExecuteScalarAsync<bool>(@"select Org_Node from sad_Org_Structure where Org_Name = @Org_Name and Org_LevelCode = 3 and Org_Status='A' and Org_CompID = @Org_CompID", new { Org_Name = @DepartmentName, Org_CompID = @compID }, transaction);
				if (OrgNod == true)
				{
					return sStatus = "Department Name is already Exist.";
				}

				Org_Node = await connection.ExecuteScalarAsync<int>(
					  @"DECLARE @TemplateId INT;
                  SELECT @TemplateId = ISNULL(MAX(Org_Node), 0) + 1 FROM sad_Org_Structure;
                  INSERT INTO sad_Org_Structure 
                  (Org_Node,org_Code, org_name, org_parent, org_userid, org_DelFlag, org_Note, org_AppStrength, org_CreatedBy, 
                   org_CreatedOn, org_Status, Org_levelCode, Org_CompID, Org_IPAddress)
                  VALUES 
                  (@TemplateId, @org_Code,@org_name,3,@org_userid,'A',@org_name,0,@org_userid,GETDATE(),'A',3,@Org_CompID,'');
                  SELECT @TemplateId;",
					  new
					  {
						  org_Code = Code,
						  org_name = DepartmentName,
						  org_userid = userId,
						  Org_CompID = compID
					  },
					  transaction
				  );

				await transaction.CommitAsync();
				return sStatus = "Department Created Successfully."; ;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}


		public async Task<string> CreateSubCabinetAsync(string Subcabinetname,int iCabinetID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
			try
			{

				var CabinetIdexists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId and CBN_ID = @Id AND (CBN_DelFlag = 'A' OR CBN_DelFlag = 'W') ",
				   new { CompId = compID,  Id = iCabinetID },
				   transaction);

				if (CabinetIdexists == 0)
				{
					return "Invalid Cabinet Id.";
				}

				var exists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId AND CBN_Name = @Name AND CBN_ID <> @Id AND (CBN_DelFlag = 'A' OR CBN_DelFlag = 'W') AND CBN_Parent = @Parent",
				   new { CompId = compID, Name = Subcabinetname, Id = iCabinetID, Parent = iCabinetID },
				   transaction);

				if (exists > 0)
				{
					 return "Sub Cabinet name already exists.";
				}


				var deptInfoQuery = "Select ISNULL(cbn_department, 0) AS cbn_department from EDT_Cabinet where cbn_id = @cbn_id";
				var deptInfo = await connection.QueryFirstOrDefaultAsync(deptInfoQuery, new { cbn_id = iCabinetID }, transaction: transaction);
				int deptId = deptInfo?.cbn_department ?? 0;


				var UserInfoQuery = "Select ISNULL(CBN_UserID, 0) AS CBN_UserID from EDT_Cabinet where cbn_id = @cbn_id";
				var UserInfo = await connection.QueryFirstOrDefaultAsync(UserInfoQuery, new { cbn_id = iCabinetID }, transaction: transaction);
				int UserId = UserInfo?.CBN_UserID ?? 0;

			 

				int CBN_ID = 0;
				CBN_ID = await connection.ExecuteScalarAsync<int>(
					  @"DECLARE @TemplateId INT;
                  SELECT @TemplateId = ISNULL(MAX(CBN_ID), 0) + 1 FROM edt_cabinet;
                  INSERT INTO edt_cabinet 
                  (CBN_ID, CBN_Name, CBN_Parent, CBN_Note, CBN_UserID, CBN_Department, CBN_SubCabCount, CBN_FolderCount, 
                   CBN_CreatedBy, CBN_CreatedOn, CBN_Status, CBN_DelFlag, CBN_CompID)
                  VALUES 
                  (@TemplateId, @CBN_Name, @CBN_Parent, @CBN_Name, @CBN_UserID, @CBN_Department, 0, 0, @CBN_UserID, GETDATE(), 'A','A', @CBN_CompID);
                  SELECT @TemplateId;",
					  new
					  {
						  CBN_Name = Subcabinetname,
						  CBN_Parent = iCabinetID,
						  CBN_UserID = UserId,
						  CBN_Department = deptId,
						  CBN_CompID = compID
					  },
					  transaction
				  );


				await connection.ExecuteAsync(
				  "UPDATE EDT_Cabinet SET CBN_SubCabCount = (SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_Parent = @CabId AND CBN_DelFlag = 'A') WHERE CBN_ID = @CabId AND CBN_CompID = @CompId;",
				  new { CabId = iCabinetID, CompId = compID },
				  transaction);

				await connection.ExecuteAsync(
					"UPDATE EDT_Cabinet SET CBN_FolderCount = (SELECT COUNT(1) FROM EDT_Folder WHERE FOL_Cabinet IN (SELECT CBN_ID FROM EDT_Cabinet WHERE CBN_Parent = @CabId AND CBN_DelFlag = 'A')) WHERE CBN_ID = @CabId AND CBN_CompID = @CompId;",
					new { CabId = iCabinetID, CompId = compID },
					transaction);


				await transaction.CommitAsync();
				return "Subcabinet created Successfully.";
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		 
		public async Task<string> CreateFolderAsync(string FolderName, int iCabinetID, int iSubCabinetID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
			try
			{

				var CabinetIdexists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId and CBN_ID = @Id AND (CBN_DelFlag = 'A') ",
				   new { CompId = compID, Id = iCabinetID },
				   transaction);

				if (CabinetIdexists == 0)
				{
					return "Invalid Cabinet Id.";
				}

				 
				var SubCabinetIdexists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId and CBN_ID = @Id AND (CBN_DelFlag = 'A') ",
				   new { CompId = compID, Id = iSubCabinetID },
				   transaction);

				if (SubCabinetIdexists == 0)
				{
					return "Invalid SubCabinet Id.";
				}


				var CabSubCabinetIdexists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM EDT_Cabinet WHERE CBN_CompID = @CompId and CBN_ID = @CBN_ID and CBN_Parent=@CBN_Parent AND (CBN_DelFlag = 'A') ",
				   new { CompId = compID, CBN_ID = iSubCabinetID, CBN_Parent= iCabinetID },
				   transaction);

				if (CabSubCabinetIdexists == 0)
				{
					return "Invalid SubCabinet and Cabinet Id.";
				}


				var exists = await connection.ExecuteScalarAsync<int>(
				   "SELECT COUNT(1) FROM edt_Folder WHERE FOL_CompID = @CompId AND FOL_Name = @Name AND FOL_Status = 'A' and FOL_DelFlag = 'W' AND Fol_Cabinet = @Id",
				   new { CompId = compID, Name = FolderName, Id = iSubCabinetID },
				   transaction);

				if (exists > 0)
				{
					return "Folder name already exists.";
				}


				var deptInfoQuery = "Select ISNULL(cbn_department, 0) AS cbn_department from EDT_Cabinet where cbn_id = @cbn_id";
				var deptInfo = await connection.QueryFirstOrDefaultAsync(deptInfoQuery, new { cbn_id = iCabinetID }, transaction: transaction);
				int deptId = deptInfo?.cbn_department ?? 0;


				var UserInfoQuery = "Select ISNULL(CBN_UserID, 0) AS CBN_UserID from EDT_Cabinet where cbn_id = @cbn_id";
				var UserInfo = await connection.QueryFirstOrDefaultAsync(UserInfoQuery, new { cbn_id = iCabinetID }, transaction: transaction);
				int UserId = UserInfo?.CBN_UserID ?? 0;



				int CBN_ID = 0;
				CBN_ID = await connection.ExecuteScalarAsync<int>(
					  @"DECLARE @TemplateId INT;
                  SELECT @TemplateId = ISNULL(MAX(FOL_FolID), 0) + 1 FROM edt_Folder;
                  INSERT INTO edt_Folder 
                  (FOL_FolID, FOL_Name, FOL_Note, FOL_Cabinet, FOL_CreatedBy, FOL_CreatedOn, FOL_Status, FOL_DelFlag, FOL_CompID)
                  VALUES (@TemplateId, @FOL_Name, @FOL_Note, @FOL_Cabinet, @FOL_CreatedBy, GETDATE(),'A','A', @FOL_CompID);
                  SELECT @TemplateId;",
					  new
					  {
						  FOL_Name = FolderName,
						  FOL_Note = FolderName,
						  FOL_Cabinet = iSubCabinetID,
						  FOL_CreatedBy = UserId,
						  FOL_CompID = compID
					  },
					  transaction
				  );


				await connection.ExecuteAsync(
				   "Update edt_cabinet set CBN_FolderCount=(select count(Fol_folid) from edt_folder where fol_cabinet in (Select CBN_id from Edt_Cabinet where CBN_Parent = @CabId And (CBN_DelFlag='A'))) where CBN_ID = @CabId and CBN_CompID = @CompId",
				   new { CabId = iCabinetID, CompId = compID },
				   transaction);

				await connection.ExecuteAsync(
					"Update edt_cabinet set CBN_FolderCount=(select Count(Fol_folid) from edt_folder where fol_cabinet = @SubCabId and (FOL_Delflag='A')) where cbn_ID = @SubCabId and CBN_CompID = @CompId;",
					new { SubCabId = iSubCabinetID, CompId = compID },
					transaction);


				await transaction.CommitAsync();
				return "Folder created Successfully.";
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}



		public async Task<string> UpdateArchiveDetailsAsync(string retentionDate, int retentionPeriod, int iArchiveID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			using var transaction = connection.BeginTransaction();

			try
			{
				var cabinetExists = await connection.ExecuteScalarAsync<int>(
					@"select count(1) 
              from StandardAudit_Schedule A 
              join edt_Cabinet B on B.CBN_AuditID = A.SA_ID 
              where A.SA_ForCompleteAudit = 1 
                and SA_IsArchived = 1 
                and B.CBN_status='A' 
                and CBN_DelFlag='A' 
                and CBN_CompID = @CompId 
                and CBN_ID = @Id",
					new { CompId = compID, Id = iArchiveID }, transaction);

				if (cabinetExists == 0)
					return "Invalid Archive Id.";

				var auditID = await connection.ExecuteScalarAsync<int?>(
					@"select SA_ID 
              from StandardAudit_Schedule A 
              join edt_Cabinet B on B.CBN_AuditID = A.SA_ID 
              where A.SA_ForCompleteAudit = 1 
                and SA_IsArchived = 1 
                and B.CBN_status='A' 
                and CBN_DelFlag='A' 
                and CBN_CompID = @CompId 
                and CBN_ID = @Id",
					new { CompId = compID, Id = iArchiveID }, transaction) ?? 0;

				await connection.ExecuteAsync(
					@"UPDATE StandardAudit_Schedule 
              SET SA_RetentionPeriod = @SA_RetentionPeriod, 
                  SA_ExpiryDate = @SA_ExpiryDate  
              WHERE SA_ID = @SA_ID and SA_CompID=@SA_CompID;",
					new { SA_RetentionPeriod = retentionPeriod, SA_ExpiryDate = retentionDate, SA_ID = auditID, SA_CompID = compID },
					transaction);

				await connection.ExecuteAsync(
					@"UPDATE EDT_Cabinet 
              SET CBN_DocumentExpiryDate = @CBN_DocumentExpiryDate, 
                  CBN_ReminderDay = @CBN_ReminderDay  
              WHERE CBN_ID = @CBN_ID and CBN_CompID=@CBN_CompID;",
					new { CBN_ReminderDay = retentionPeriod, CBN_DocumentExpiryDate = retentionDate, CBN_ID = iArchiveID, CBN_CompID = compID },
					transaction);

				await transaction.CommitAsync();
				return "Updated Successfully.";
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}



		//public async Task<string> DownloadArchieveDocumentsAsync(string sAttachID)
		//{
		//	if (string.IsNullOrWhiteSpace(sAttachID))
		//		throw new ArgumentException("Attachment ID is required.");

		//	try
		//	{
		//		string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
		//		if (string.IsNullOrEmpty(dbName))
		//			throw new Exception("CustomerCode is missing in session. Please log in again.");

		//		var connectionString = _configuration.GetConnectionString(dbName);

		//		using var connection = new SqlConnection(connectionString);
		//		await connection.OpenAsync();

		//		string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
		//                      SELECT DISTINCT 
		//                          ATCH_FName AS FileName, 
		//                          (SELECT TOP 1 SAD_Config_Value FROM [Sad_Config_Settings] 
		//                              WHERE sad_Config_key = 'DisplayPath') + 'BITMAPS\' 
		//                              + CAST(FLOOR(CAST(A.Atch_DocID AS numeric)/301) AS varchar) + '\' 
		//                              + CAST(A.Atch_DocID AS varchar) + '.' + A.ATCH_Ext AS URLPath 
		//                      FROM edt_Attachments A
		//                      JOIN (SELECT DISTINCT CAST(value AS INT) AS Atch_ID 
		//                            FROM STRING_SPLIT(@ids, ',')) S
		//                      ON A.Atch_ID = S.Atch_ID 
		//                      WHERE A.Atch_FName != '' AND ATCH_Ext != '';";

		//		var files = (await connection.QueryAsync<ArchivedDocumentFileDto>(query, new { AttachIDs = sAttachID }))
		//					.ToList();

		//		if (files == null || files.Count == 0)
		//			throw new Exception("No files found.");

		//		// Temp folder to copy files
		//		string tempFolder = Path.Combine(Path.GetTempPath(), "ArchiveDocs_" + Guid.NewGuid());
		//		Directory.CreateDirectory(tempFolder);

		//		foreach (var file in files)
		//		{
		//			if (System.IO.File.Exists(file.URLPath))
		//			{
		//				string destPath = Path.Combine(tempFolder, file.FileName);
		//				System.IO.File.Copy(file.URLPath, destPath, true);
		//			}
		//		}

		//		// Create zip
		//		string zipPath = Path.Combine(Path.GetTempPath(), $"ArchiveDocs_{DateTime.Now:yyyyMMddHHmmss}.zip");
		//		ZipFile.CreateFromDirectory(tempFolder, zipPath);

		//		// Cleanup temp files (optional but recommended)
		//		Directory.Delete(tempFolder, true);

		//		return zipPath; // Return full zip file path
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception("Error downloading archived documents: " + ex.Message, ex);
		//	}
		//}
		 
		public async Task<string> DownloadArchieveDocumentsAsync(string sAttachID)
		{
			if (string.IsNullOrWhiteSpace(sAttachID))
				throw new ArgumentException("Attachment ID is required.");

			try
			{
				string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
				if (string.IsNullOrEmpty(dbName))
					throw new Exception("CustomerCode is missing in session. Please log in again.");

				var connectionString = _configuration.GetConnectionString(dbName);

				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				string query = @"DECLARE @ids NVARCHAR(MAX) = @AttachIDs;
                SELECT DISTINCT 
                    ATCH_FName AS FileName, 
                    (SELECT TOP 1 SAD_Config_Value FROM [Sad_Config_Settings] 
                        WHERE sad_Config_key = 'DisplayPath') + 'BITMAPS\' 
                        + CAST(FLOOR(CAST(A.Atch_DocID AS numeric)/301) AS varchar) + '\' 
                        + CAST(A.Atch_DocID AS varchar) + '.' + A.ATCH_Ext AS URLPath 
                FROM edt_Attachments A
                JOIN (SELECT DISTINCT CAST(value AS INT) AS Atch_ID 
                      FROM STRING_SPLIT(@ids, ',')) S
                ON A.Atch_ID = S.Atch_ID 
                WHERE A.Atch_FName != '' AND ATCH_Ext != '';";

				var files = (await connection.QueryAsync<ArchivedDocumentFileDto>(query, new { AttachIDs = sAttachID }))
							.ToList();

				if (files == null || files.Count == 0)
					throw new Exception("No files found.");

				// Define the user's Downloads folder path
				string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

				// Ensure Downloads folder exists
				if (!Directory.Exists(downloadsFolder))
					Directory.CreateDirectory(downloadsFolder);

				// Create a temp folder for the files to zip
				string tempFolder = Path.Combine(downloadsFolder, "ArchiveDocs_" + Guid.NewGuid());
				Directory.CreateDirectory(tempFolder);

				// Copy files to temp folder
				foreach (var file in files)
				{
					if (System.IO.File.Exists(file.URLPath))
					{
						string destPath = Path.Combine(tempFolder, file.FileName);
						System.IO.File.Copy(file.URLPath, destPath, true);
					}
				}

				// Create the ZIP inside Downloads folder
				string zipFileName = $"ArchiveDocs_{DateTime.Now:yyyyMMddHHmmss}.zip";
				string zipPath = Path.Combine(downloadsFolder, zipFileName);

				ZipFile.CreateFromDirectory(tempFolder, zipPath);

				// Delete temp folder after zipping
				Directory.Delete(tempFolder, true);

				return zipPath; // Full path of the ZIP file in Downloads folder
			}
			catch (Exception ex)
			{
				throw new Exception("Error downloading archived documents: " + ex.Message, ex);
			}
		}


		public async Task<string> DeleteArchiveDocumentsAsync(int iArchiveID, string sAttachID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			using var transaction = connection.BeginTransaction();

			try
			{ 
				await connection.ExecuteAsync(
					@"UPDATE StandardAudit_Schedule 
              SET SA_IsArchived = 2
              WHERE SA_ID = @SA_ID and SA_CompID=@SA_CompID;",
					new {  SA_ID = iArchiveID, SA_CompID = compID },
					transaction);

				await connection.ExecuteAsync(
					@"UPDATE edt_Attachments 
              SET ATCH_Status ='D'  
              WHERE ATCH_ID = @ATCH_ID and ATCh_CompID=@ATCh_CompID;",
					new { ATCH_ID = sAttachID, ATCh_CompID = compID },
					transaction);

				await transaction.CommitAsync();
				return "Updated Successfully.";
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}


		public async Task<int> UpdateSubCabinetAsync(string SubcabinetName, int SubcabinetId,int userId,  int compId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				var rowsAffected = await connection.ExecuteAsync(
					@"UPDATE edt_cabinet 
              SET CBN_Name = @CBN_Name, 
                  CBN_UpdatedBy = @CBN_UpdatedBy, 
                  CBN_UpdatedOn = GETDATE()  
              WHERE CBN_ID = @CBN_ID and CBN_CompID = @CBN_CompID",
					new
					{
						CBN_Name = SubcabinetName,
						CBN_UpdatedBy = userId,
						CBN_ID = SubcabinetId,
						CBN_CompID = compId
					},
					transaction
				);

				await transaction.CommitAsync();

				return rowsAffected > 0 ? SubcabinetId : 0;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		 
		public async Task<int> UpdateFolderAsync(string FolderName, int iFolderID, int userId, int compId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				var rowsAffected = await connection.ExecuteAsync(
					@"UPDATE edt_Folder 
              SET FOL_Name = @FOL_Name, 
                  FOL_UpdatedBy = @FOL_UpdatedBy, 
                  FOL_UpdatedOn = GETDATE()  
              WHERE FOL_FolID = @FOL_FolID and FOL_CompID = @FOL_CompID",
					new
					{
						FOL_Name = FolderName,
						FOL_UpdatedBy = userId,
						FOL_FolID = iFolderID,
						FOL_CompID = compId
					},
					transaction
				);

				await transaction.CommitAsync();

				return rowsAffected > 0 ? iFolderID : 0;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}


		public async Task<int> CreateDocumentTypeAsync(string DocumentName, string DepartmentId, int userID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();
			try
			{
				int DOT_DoctypeID = await connection.ExecuteScalarAsync<int>(
					  @"DECLARE @TemplateId INT;
                  SELECT @TemplateId = ISNULL(MAX(DOT_DoctypeID), 0) + 1 FROM edt_Document_Type;
                  INSERT INTO edt_Document_Type 
                  (DOT_DoctypeID, DOT_DOCNAME, DOT_Note, DOT_PGroup, DOT_CrBy, DOT_CrOn, DOT_Status, dot_operation, dot_OperationBy, DOT_isGlobal, DOT_DelFlag, DOT_COmpID)
                  VALUES 
                  (@TemplateId, @DOT_DOCNAME, @DOT_DOCNAME, @DOT_PGroup, @DOT_CrBy, GetDate(), 'A', 1,1,0,'A',@DOT_COmpID);
                  SELECT @TemplateId;",
					  new
					  {
						  DOT_DOCNAME = DocumentName,
						  DOT_PGroup = DepartmentId,
						  DOT_CrBy = userID,
						  DOT_COmpID = compID
					  },
					  transaction
				  );

				await transaction.CommitAsync();
				return DOT_DoctypeID;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}


		public async Task<int> UpdateDepartmentAsync(string Code, string DepartmentName, int iDepartmentID, int iUserID, int compID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			using var transaction = connection.BeginTransaction();

			try
			{
				var rowsAffected = await connection.ExecuteAsync(
					@"UPDATE sad_Org_Structure 
              SET Org_Code = @Org_Code, Org_Name=@Org_Name,
                  Org_UpdatedBy = @Org_UpdatedBy, 
                  Org_UpdatedOn = GETDATE()  
              WHERE Org_Node = @Org_Node and Org_CompID = @Org_CompID",
					new
					{
						Org_Node = iDepartmentID,
						Org_Code = Code,
						Org_Name = DepartmentName,
						Org_UpdatedBy = iUserID,
						Org_CompID = compID
					},
					transaction
				);

				await transaction.CommitAsync();

				return rowsAffected > 0 ? iDepartmentID : 0;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

        public async Task<Google.Apis.Drive.v3.Data.File> GetFileByIdAsync(int DocId, string userEmail)
        {
            if (DocId <= 0)
                throw new ArgumentException("Document Not Exist!");

            string fileId;

            // Retrieve FileId from database
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                fileId = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT FileId FROM UserDriveItemsNumeric WHERE DocId = @DocId",
                    new { DocId });
            }

            if (string.IsNullOrEmpty(fileId))
            {
                throw new Exception($"No FileId found in database for DocId {DocId}");
            }

            var service = await GetDriveServiceAsync(userEmail);

            try
            {
                var request = service.Files.Get(fileId);
                request.Fields = "id, name, mimeType, size, createdTime, modifiedTime, webViewLink, webContentLink, parents, thumbnailLink, fileExtension, fullFileExtension, originalFilename";

                var file = await request.ExecuteAsync();

                LogInfo($"Retrieved file by ID: {fileId} for user {userEmail}");
                return file;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LogError($"File not found with ID: {fileId} for user {userEmail}");
                throw new Exception($"File with ID '{fileId}' not found.", ex);
            }
            catch (Exception ex)
            {
                LogError($"Error getting file by ID {fileId} for user {userEmail}: {ex.Message}");
                throw;
            }
        }

        private string GetUserEmail()
        {
            // ✅ Step 1: Get DB Name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Query the latest UserEmail
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT TOP 1 UserEmail FROM UserDriveTokens ORDER BY Id DESC";

            using var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();

            return result?.ToString();
        }


        #region Drive Initialization
        public async Task<DriveService> GetDriveServiceAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email must be provided.", nameof(userEmail));

            UserCredential credential;

            try
            {

                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                // ✅ Step 2: Get the connection string
                var connectionString = _configuration.GetConnectionString(dbName);


                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT TokenJson FROM UserDriveTokens WHERE UserEmail = @UserEmail";
                var tokenJson = await connection.QueryFirstOrDefaultAsync<string>(query, new { UserEmail = userEmail });

                using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);

                var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = Scopes,
                    DataStore = new NullDataStore()
                });

                if (!string.IsNullOrEmpty(tokenJson))
                {
                    var token = JsonConvert.DeserializeObject<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(tokenJson);
                    credential = new UserCredential(flow, userEmail, token);

                    // ✅ Check and refresh token if expired
                    if (credential.Token.IsExpired(SystemClock.Default))
                    {
                        LogInfo($"Token expired for {userEmail}, refreshing...");

                        bool success = await credential.RefreshTokenAsync(CancellationToken.None);
                        if (success)
                        {
                            string updateQuery = "UPDATE UserDriveTokens SET TokenJson = @TokenJson, UpdatedOn = @UpdatedOn WHERE UserEmail = @UserEmail";
                            await connection.ExecuteAsync(updateQuery, new
                            {
                                UserEmail = userEmail,
                                TokenJson = JsonConvert.SerializeObject(credential.Token),
                                UpdatedOn = DateTime.UtcNow
                            });

                            LogInfo($"Token refreshed and updated in DB for {userEmail}");
                        }
                        else
                        {
                            LogError($"Failed to refresh token for {userEmail}. Reauthorization required.");
                            throw new UnauthorizedAccessException("Google Drive token refresh failed. User needs to reauthorize.");
                        }
                    }
                    else
                    {
                        LogInfo($"Loaded valid Google Drive token from DB for {userEmail}");
                    }
                }
                else
                {
                    // No token found → trigger Google OAuth login
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        clientSecrets,
                        Scopes,
                        userEmail,
                        CancellationToken.None,
                        new FileDataStore("Tokens", true)
                    );

                    string insertQuery = @"
                INSERT INTO UserDriveTokens (UserEmail, TokenJson, CreatedOn)
                VALUES (@UserEmail, @TokenJson, @CreatedOn)";
                    await connection.ExecuteAsync(insertQuery, new
                    {
                        UserEmail = userEmail,
                        TokenJson = JsonConvert.SerializeObject(credential.Token),
                        CreatedOn = DateTime.UtcNow
                    });

                    LogInfo($"Saved new Google Drive token to DB for {userEmail}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error creating DriveService for {userEmail}: {ex.Message}");
                throw;
            }

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TracePA Drive Integration"
            });

            LogInfo($"DriveService successfully created for {userEmail}");
            return driveService;
        }

        #endregion

        #region Logging
        private void LogInfo(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
                File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {message}{Environment.NewLine}");
            }
            catch { }
        }

        private void LogError(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
                File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {message}{Environment.NewLine}");
            }
            catch { }
        }
        #endregion


        public async Task<string> GetFolderIdByPathAsync(string folderPath, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            string parentId = null;
            var folders = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (var folder in folders)
            {
                var listRequest = service.Files.List();
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folder}' and trashed=false";
                if (!string.IsNullOrEmpty(parentId))
                    listRequest.Q += $" and '{parentId}' in parents";
                listRequest.Fields = "files(id, name)";
                var result = await listRequest.ExecuteAsync();

                if (!result.Files.Any()) return null;
                parentId = result.Files.First().Id;
            }

            return parentId;
        }


        public async Task<object> UploadFileToFolderAsyncNew(IFormFile file, string folderPath, string userEmail, int docid)
        {
            if (file == null || file.Length == 0)
                return new { Status = "Error", Message = "No file uploaded." };

            var service = await GetDriveServiceAsync(userEmail);
            var folderId = await GetFolderIdByPathAsync(folderPath, userEmail);

            if (folderId == null)
                return new { Status = "Error", Message = $"Folder path '{folderPath}' does not exist." };

            var fileMetadata = new GoogleDriveFile
            {
                Name = file.FileName,
                Parents = new[] { folderId }
            };

            using var stream = file.OpenReadStream();
            var request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id, name, parents, mimeType";
            await request.UploadAsync();

            // 🔹 Insert File record into UserDriveItemsNumeric table
            try
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                var connectionString = _configuration.GetConnectionString(dbName);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                string insertQuery = @"
              INSERT INTO UserDriveItemsNumeric (UserEmail, FolderPath, FolderId, FileName, FileId, MimeType, CreatedOn,DocId)
              VALUES (@UserEmail, @FolderPath, @FolderId, @FileName, @FileId, @MimeType, GETUTCDATE(), @DocId);";

                await connection.ExecuteScalarAsync(insertQuery, new
                {
                    UserEmail = userEmail,
                    FolderPath = folderPath,
                    FolderId = folderId,
                    FileName = file.FileName,
                    FileId = request.ResponseBody.Id,
                    MimeType = request.ResponseBody.MimeType,
                    DocId = docid
                });
                LogInfo($"File '{file.FileName}' saved to UserDriveItemsNumeric table for {userEmail}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to insert file info: {ex.Message}");
            }

            return new
            {
                Status = "Success",
                Message = "File uploaded successfully.",
                File = new
                {
                    request.ResponseBody.Id,
                    request.ResponseBody.Name,
                    request.ResponseBody.Parents
                }
            };
        }

        private string EnsureDirectoryExists(string rootPath, string user, string subFolder)
        {
            var path = Path.Combine(rootPath, "Tempfolder", user, subFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

    }
}


