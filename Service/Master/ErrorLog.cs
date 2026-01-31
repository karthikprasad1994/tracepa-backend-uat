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
using TracePca.Dto.Masters;
using TracePca.Interface;
using TracePca.Interface.DigitalFilling;
using TracePca.Interface.Master;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace TracePca.Service.Master
{
    public class ErrorLog : ErrorLogInterface
	{

		private readonly Trdmyus1Context _dbcontext;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _env;
		private readonly DbConnectionProvider _dbConnectionProvider;

		public ErrorLog(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }


		public async Task<string> LogErrorAsync(string accessCode, string message, string className, string functionName, ErrorLogDto dto)
		{
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


				using var transaction = connection.BeginTransaction();

				int accessCodeId = await connection.ExecuteScalarAsync<int>(
					@"Select CM_ID from Sad_Company_Master where CM_AccessCode=@CM_AccessCode",
					new { CM_AccessCode = accessCode }, transaction);

                //var errorLogPath = await connection.ExecuteScalarAsync<string>(
                //	@"Select sad_Config_Value from sad_config_settings where sad_Config_Key=@sad_Config_Key and sad_compid=@sad_compid",
                //	new { sad_Config_Key = "ErrorLog", sad_compid = accessCodeId }, transaction);
                var errorLogPath = "C:\\inetpub\\wwwroot\\ErrorLog\\Erro";

				int gmtOffset = DateTime.Compare(DateTime.Now, DateTime.UtcNow);
				string gmtPrefix = gmtOffset > 0 ? "+" : "";

				string errorDateTime = $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day} @ " +
									$"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} " +
									$"(GMT {gmtPrefix}{gmtOffset})";

				if (!File.Exists(errorLogPath))
				{
					File.CreateText(errorLogPath).Dispose();
				}

				await using (var streamWriter = new StreamWriter(errorLogPath, true))
				{
					await streamWriter.WriteLineAsync($"Data base: {accessCode}");
					await streamWriter.WriteLineAsync($"Date And Time # {errorDateTime}");
					await streamWriter.WriteLineAsync($"Class Name    # {className}");
					await streamWriter.WriteLineAsync($"Function Name # {functionName}");
					await streamWriter.WriteLineAsync($"Error Message # {message}");
					await streamWriter.WriteLineAsync("##################################################################");
				}
				await transaction.CommitAsync();
				return message;
			}
			catch (Exception ex)
			{
				// Return the exception message or a custom error message
				return $"Error logging failed: {ex.Message}";

				// Alternatively, you could rethrow the exception:
				// throw;
				// But this would require changing the method signature to not expect a return value
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
   //         var DocumentTypeID = await connection.ExecuteScalarAsync<int>(@"Select Fol_FolID from edt_Document_Type where Dot_DocTypeID=@Dot_DocTypeID  
   //             and DOT_CompID=@DOT_CompID", new { Dot_DocTypeID = dto.DocumentTypeID, DOT_CompID = dto.CompID }, transaction);
   //         if (DocumentTypeID == 0)
   //         {
   //             return sStatus = "Invalid Document Type Id.";
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
   //             PGE_KeyWORD = dto.Keyword,
   //             PGE_SubCabinet = dto.SubCabinetID,
   //             PGE_batch_name = BaseNameID,
   //             pge_OrignalFileName = sFileName,
   //             Pge_CompID = dto.CompID
   //         });

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
             
    }
}


