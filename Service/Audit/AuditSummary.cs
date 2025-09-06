using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using StackExchange.Redis;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using TracePca.Data;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;
using static TracePca.Interface.Audit.AuditSummaryInterface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TracePca.Dto;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Colors = QuestPDF.Helpers.Colors;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using WordDoc = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordprocessingDocument = DocumentFormat.OpenXml.Packaging.WordprocessingDocument;

namespace TracePca.Service.Audit
{
	public class AuditSummary : AuditSummaryInterface
	{

		private readonly Trdmyus1Context _dbcontext;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IWebHostEnvironment _env;
		private readonly DbConnectionProvider _dbConnectionProvider;

		public AuditSummary(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, DbConnectionProvider dbConnectionProvider)
		{
			_dbcontext = dbcontext;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
        }
 
	
		public async Task<List<ReportTypeDto>> GetReportTypesAsync(int compId, int templateId)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);


			string query = @"
        SELECT RTM_Id, RTM_ReportTypeName
        FROM SAD_ReportTypeMaster
        WHERE RTM_CompID = @CompId
          AND RTM_DelFlag = 'A'";

			if (templateId > 0)
			{
				query += " AND RTM_TemplateId = @TemplateId";
			}
			query += " ORDER BY RTM_ReportTypeName";

			var reportTypes = await connection.QueryAsync<ReportTypeDto>(query, new { CompId = compId, TemplateId = templateId });
			return reportTypes.ToList();
		}


		public async Task<DropDownDataDto> LoadAuditNoDataAsync(int custId, int compId, int financialYearId, int loginUserId)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);


			var query = @"
        SELECT SA_ID, SA_AuditNo + ' - ' + CMM_Desc AS SA_AuditNo
        FROM StandardAudit_Schedule
        LEFT JOIN Content_Management_Master ON CMM_ID = SA_AuditTypeID
        WHERE SA_CompID = @compId";

			if (financialYearId > 0)
				query += " AND SA_YearID = @financialYearId";

			if (custId > 0)
				query += " AND SA_CustID = @custId";
			var checkPartnerSql = @"
        SELECT Usr_ID 
        FROM sad_userdetails 
        WHERE usr_compID = @CompId 
          AND USR_Partner = 1 
          AND (usr_DelFlag = 'A' OR usr_DelFlag = 'B' OR usr_DelFlag = 'L') 
          AND Usr_ID = @UserId";

			var partnerParams = new
			{
				CompId = compId,
				UserId = loginUserId
			};

			var Partner = await connection.QueryFirstOrDefaultAsync<int?>(checkPartnerSql, partnerParams);
			bool loginUserIsPartner = Partner.HasValue;
			if (!loginUserIsPartner)
			{
				query += @"
            AND (
                CONCAT(',', SA_AdditionalSupportEmployeeID, ',') LIKE '%,' + CAST(@loginUserId AS VARCHAR) + ',%' OR
                CONCAT(',', SA_EngagementPartnerID, ',') LIKE '%,' + CAST(@loginUserId AS VARCHAR) + ',%' OR
                CONCAT(',', SA_ReviewPartnerID, ',') LIKE '%,' + CAST(@loginUserId AS VARCHAR) + ',%' OR
                CONCAT(',', SA_PartnerID, ',') LIKE '%,' + CAST(@loginUserId AS VARCHAR) + ',%'
            )";
			}

			query += " ORDER BY SA_ID DESC";

			var auditNos = await connection.QueryAsync<AuditSummaryDto>(query, new
			{
				compId,
				financialYearId,
				custId,
				loginUserId
			});

			return new DropDownDataDto
			{
				AuditNoDetails = auditNos.ToList()
			};
		}

		public async Task<DropDownDataDto> LoadCustomerDataAsync(int compId)
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

			var CustomersDt = connection.QueryAsync<AuditSummaryDto>(@"
            Select Cust_Id,Cust_Name from SAD_CUSTOMER_MASTER Where CUST_DelFlg = 'A' and cust_Compid = @CompId
            ORDER BY Cust_Name ASC", new { CompId = compId });

			//var AuditNodt = connection.QueryAsync<AuditSummaryDto>(@"
			//Select SA_ID,SA_AuditNo + ' - ' + CMM_Desc As SA_AuditNo From StandardAudit_Schedule 
			//Left Join Content_Management_Master on CMM_ID=SA_AuditTypeID Where SA_CompID = @CompId
			//  And SA_CustID = @CustId
			//ORDER BY RTM_ReportTypeName", new { CompId = compId});

			//    // If you want LOE list based on only compId (not customerId/serviceId), modify your LOE fetch logic slightly:
			//    var loeListTask = connection.QueryAsync<LoEDto>(@"
			//SELECT LOE_ID AS LoeId, LOE_Name
			//FROM SAD_CUST_LOE
			//WHERE LOE_CompID = @CompId", new { CompId = compId });

			//    var FeesType = connection.QueryAsync<FeeTypeDto>(@"
			//SELECT cmm_ID AS FeeId, cmm_Desc AS FeeName
			//FROM Content_Management_Master
			//WHERE cmm_Category = 'OE' and CMM_CompID = @CompId", new { CompId = compId });
			//await Task.WhenAll(auditTypesTask, reportTypesTask, loeListTask, FeesType);
			await Task.WhenAll(CustomersDt);
			return new DropDownDataDto
			{
				CustomerDetails = CustomersDt.Result.ToList()

				//FeeTypes = FeesType.Result.ToList(),
				//Loenames = loeListTask.Result.ToList()
			};
		}


		public async Task<IEnumerable<AuditDetailsDto>> GetAuditDetailsAsync(int compId, int customerId, int auditNo)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            SELECT b.SA_ID As AuditID,DENSE_RANK() OVER (ORDER BY b.SA_ID Desc) As SrNo,b.SA_AuditNo As AuditNo,b.SA_CustID As CustID,Cust_Name As CustomerName,CONCAT(SUBSTRING(Cust_Name, 0, 25),'....') As CustomerShortName, CMM_Desc As AuditType, b.SA_Status As StatusID,ISNULL(Convert(Varchar(10),b.SA_CrOn,103),'') As AuditDate,
            Case When b.SA_Status=1 then 'Scheduled' When b.SA_Status=2 then 'Communication with Client' When b.SA_Status=3 then 'TBR' When b.SA_Status=4 then 'Conduct Audit' When b.SA_Status=5 then 'Report' End AuditStatus,Partner=STUFF ((SELECT DISTINCT '; '+ CAST(usr_FullName AS VARCHAR(MAX)) FROM Sad_UserDetails WHERE usr_id in (SELECT value FROM STRING_SPLIT((Select STUFF(LEFT(a.SA_PartnerID, LEN(a.SA_PartnerID) - PATINDEX('%[^,]%', REVERSE(a.SA_PartnerID)) + 1), 1, PATINDEX('%[^,]%', a.SA_PartnerID) - 1, '') from StandardAudit_Schedule a Where SA_ID=b.SA_ID),',')) FOR XMl PATH('')),1,1,''), Team=STUFF ((SELECT DISTINCT '; '+ CAST(usr_FullName AS VARCHAR(MAX)) FROM Sad_UserDetails WHERE usr_id in (SELECT value FROM STRING_SPLIT((Select STUFF(LEFT(a.SA_AdditionalSupportEmployeeID, LEN(a.SA_AdditionalSupportEmployeeID) - PATINDEX('%[^,]%', REVERSE(a.SA_AdditionalSupportEmployeeID)) + 1), 1, PATINDEX('%[^,]%', a.SA_AdditionalSupportEmployeeID) - 1, '') from StandardAudit_Schedule a Where SA_ID=b.SA_ID),',')) FOR XMl PATH('')),1,1,'') FROM StandardAudit_Schedule b 
                Join SAD_CUSTOMER_MASTER on Cust_Id=b.SA_CustID Join Content_Management_Master on CMM_ID=b.SA_AuditTypeID 
            Where b.SA_CompID=@CompId And b.SA_CustID=@CustomerId And b.SA_ID=@AuditNo
            Order by b.SA_ID Desc";


			var result = await connection.QueryAsync<AuditDetailsDto>(query, new
			{
				CompId = compId,
				CustomerId = customerId,
				AuditNo = auditNo
			});

			return result;
		}

		public async Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryAsync(int compId, int customerId, int auditNo, int yearId)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            SELECT ADRL_Id,ADRL_ReportType,ADRL_AuditNo,IsNull(RTM_ReportTypeName,'Unknown Report Type') AS ReportTypeText,
            ADRL_Comments,ADRL_RequestedOn,usr_FullName,ADRL_ReceivedOn,ADRL_ReceivedComments,ADRL_AttchDocId  
            FROM Audit_DRLLog LEFT JOIN Sad_UserDetails a ON a.usr_Id = ADRL_CrBy  LEFT JOIN SAD_ReportTypeMaster ON RTM_Id = ADRL_ReportType 
            WHERE ADRL_AuditNo=@AuditNo  AND ADRL_YearID = @YearId  AND ADRL_CompID = @CompId and ADRL_CustID = @CustomerId  ORDER BY  ADRL_UpdatedOn DESC"
			;

			//WHERE ADRL_AuditNo = @AuditNo  AND ADRL_YearID = @YearId  AND ADRL_CompID = @CompId and ADRL_CustID = @CustomerId and ADRL_RequestedListID = @RequestId ORDER BY  ADRL_UpdatedOn DESC


			var result = await connection.QueryAsync<DocumentRequestSummaryDto>(query, new
			{
				CompId = compId,
				CustomerId = customerId,
				AuditNo = auditNo,
				//RequestId = requestId,
				YearId = yearId
			});

			return result;
		}

		public async Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryCompletionAuditAsync(int compId, int customerId, int auditNo, int requestId, int yearId)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            SELECT ADRL_Id,ADRL_ReportType,ADRL_AuditNo,IsNull(RTM_ReportTypeName,'Unknown Report Type') AS ReportTypeText,
            ADRL_Comments,ADRL_RequestedOn,usr_FullName,ADRL_ReceivedOn,ADRL_ReceivedComments,ADRL_AttchDocId 
            FROM Audit_DRLLog LEFT JOIN Sad_UserDetails a ON a.usr_Id = ADRL_CrBy  LEFT JOIN SAD_ReportTypeMaster ON RTM_Id = ADRL_ReportType 
            WHERE ADRL_AuditNo = @AuditNo  AND ADRL_YearID = @YearId  AND ADRL_CompID = @CompId and ADRL_CustID = @CustomerId and ADRL_RequestedListID = @RequestId ORDER BY  ADRL_UpdatedOn DESC";


			var result = await connection.QueryAsync<DocumentRequestSummaryDto>(query, new
			{
				CompId = compId,
				CustomerId = customerId,
				AuditNo = auditNo,
				RequestId = requestId,
				YearId = yearId
			});

			return result;
		}


		public async Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryDuringAuditAsync(int compId, int customerId, int auditNo, int requestId, int yearId)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            SELECT ADRL_Id, Case When ADRL_RequestedListID > 0 then IsNull(CMM_Desc,'NA') When (ADRL_RequestedListID = 0 And ADRL_FunID > 0) Then 
            IsNull(ACM_Checkpoint,'NA') End AS ReportTypeText, ADRL_Comments,ADRL_RequestedOn,usr_FullName,ADRL_ReceivedOn,ADRL_ReceivedComments,ADRL_AttchDocId   
            FROM Audit_DRLLog LEFT JOIN Sad_UserDetails a ON a.usr_Id = ADRL_CrBy LEFT JOIN Content_Management_Master ON CMM_ID = ADRL_RequestedListID 
            LEFT JOIN AuditType_Checklist_Master ON ACM_ID = ADRL_FunID 
            WHERE ADRL_AuditNo =@AuditNo  AND ADRL_YearID =@YearId  AND ADRL_CompID =@CompId and ADRL_CustID = @CustomerId and (ADRL_ReportType Is NULL or ADRL_ReportType = 0) ORDER BY ADRL_UpdatedOn";


			var result = await connection.QueryAsync<DocumentRequestSummaryDto>(query, new
			{
				CompId = compId,
				CustomerId = customerId,
				AuditNo = auditNo,
				RequestId = requestId,
				YearId = yearId
			});

			return result;
		}



		public async Task<IEnumerable<AuditProgramSummaryDto>> GetAuditProgramSummaryAsync(int compId, int auditNo)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            SELECT ACM.ACM_Heading AS AuditProgram, COUNT(SAC_ID) AS TotalCheckpoints, COUNT(CASE WHEN SAC_Mandatory = 1 THEN 1 END) AS Mandatory,
            COUNT(CASE WHEN SAC_SA_ID = @AuditNo AND SAC_TestResult IS NOT NULL AND SAC_TestResult = 1 THEN 1 END) AS Tested, COUNT(CASE WHEN SAC_SA_ID = @AuditNo AND 
            SAC_Annexure IS NOT NULL AND SAC_Annexure = 1 THEN 1 END) AS Annexures, COUNT(DISTINCT SCR.SCR_CheckPointID) AS Reviewed,  
            ISNULL(U.usr_FullName, '') As Employee FROM StandardAudit_ScheduleCheckPointList SASC  LEFT JOIN AuditType_Checklist_Master ACM ON 
            ACM.ACM_ID = SASC.SAC_CheckPointID  LEFT JOIN StandardAudit_Checklist_Details SACD ON SACD.SACD_AuditId = @AuditNo And ',' + 
            SACD.SACD_CheckpointId + ',' LIKE '%,' + CAST(SASC.SAC_CheckPointID AS VARCHAR) + ',%' LEFT JOIN sad_userdetails U On Usr_ID= SACD.SACD_EmpId 
            LEFT JOIN StandardAudit_ConductAudit_RemarksHistory SCR ON SCR.SCR_CheckPointID = SASC.SAC_CheckPointID AND SCR.SCR_SA_ID = @AuditNo AND 
            SCR.SCR_RemarksBy IN (SELECT Usr_ID FROM sad_userdetails WHERE usr_compID = @CompId AND Usr_Role IN 
            (SELECT Mas_ID FROM SAD_GrpOrLvl_General_Master WHERE Mas_Delflag = 'A' AND Mas_CompID =  @CompId AND Mas_Description = 'Reviewer')) 
            WHERE SASC.SAC_SA_ID = @AuditNo AND SASC.SAC_CompID =  @CompId GROUP BY ACM.ACM_Heading,U.usr_FullName";


			var result = await connection.QueryAsync<AuditProgramSummaryDto>(query, new
			{
				CompId = compId,
				AuditNo = auditNo
			});

			return result;
		}


		//public async Task<IEnumerable<WorkspaceSummaryDto>> GetWorkspaceSummaryAsync(int compId, int auditNo)
		//{
		//    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

		//    string query = @"
		//    Select ISNULL(cm.cmm_ID, a.SSW_ID) As PKID,IsNull(cm.cmm_Desc,'NA') As WorkpaperChecklist,SSW_WorkpaperNo As WorkpaperNo,
		//    SSW_WorkpaperRef As WorkpaperRef,SSW_Observation As Observation,SSW_Conclusion As Conclusion,SSW_ReviewerComments As ReviewerComments, 
		//    Case When a.SSW_TypeOfTest=1 then 'Inquiry' When a.SSW_TypeOfTest=2 then 'Observation' When a.SSW_TypeOfTest=3 then 'Examination' When 
		//    a.SSW_TypeOfTest=4 then 'Inspection' When a.SSW_TypeOfTest=5 then 'Substantive Testing' End TypeOfTest, Case When a.SSW_Status=1 
		//    then 'Open' When a.SSW_Status=2 then 'WIP' When a.SSW_Status=3 then 'Closed' End Status,SSW_AttachID As AttachID,
		//    IsNull(b.usr_FullName,'-') As CreatedBy,ISNULL(Convert(Varchar(10),SSW_CrOn,103),'-') As CreatedOn, IsNull(c.usr_FullName,'-') As ReviewedBy,
		//    ISNULL(Convert(Varchar(10),SSW_ReviewedOn,103),'-') As ReviewedOn From Content_Management_Master cm 
		//    Full Outer Join StandardAudit_ScheduleConduct_WorkPaper a On cm.cmm_ID = a.SSW_WPCheckListID And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId 
		//    Left Join sad_userdetails b on b.Usr_ID=a.SSW_CrBy Left Join sad_userdetails c on c.Usr_ID=a.SSW_ReviewedBy 
		//    Where (cm.cmm_Delflag = 'A' And cm.cmm_Category = 'WCM') Or a.SSW_ID Is Not Null And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId Order by 
		//    CASE WHEN cm.cmm_Desc IS NULL THEN 1 ELSE 0 END, cm.cmm_ID ASC";


		//    var result = await connection.QueryAsync<WorkspaceSummaryDto>(query, new
		//    {
		//        CompId = compId,
		//        AuditNo = auditNo
		//    });

		//    return result;
		//}



		public async Task<IEnumerable<WorkspaceSummaryDto>> GetWorkspaceSummaryAsync(int compId, int auditNo)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			//string query = @"
			//Select ISNULL(cm.cmm_ID, a.SSW_ID) As PKID,IsNull(cm.cmm_Desc,'NA') As WorkpaperChecklist,SSW_WorkpaperNo As WorkpaperNo,
			//SSW_WorkpaperRef As WorkpaperRef,SSW_Observation As Observation,SSW_Conclusion As Conclusion,SSW_ReviewerComments As ReviewerComments, 
			//Case When a.SSW_TypeOfTest=1 then 'Inquiry' When a.SSW_TypeOfTest=2 then 'Observation' When a.SSW_TypeOfTest=3 then 'Examination' When 
			//a.SSW_TypeOfTest=4 then 'Inspection' When a.SSW_TypeOfTest=5 then 'Substantive Testing' End TypeOfTest, Case When a.SSW_Status=1 
			//then 'Open' When a.SSW_Status=2 then 'WIP' When a.SSW_Status=3 then 'Closed' End Status,SSW_AttachID As AttachID,
			//IsNull(b.usr_FullName,'-') As CreatedBy,ISNULL(Convert(Varchar(10),SSW_CrOn,103),'-') As CreatedOn, IsNull(c.usr_FullName,'-') As ReviewedBy,
			//ISNULL(Convert(Varchar(10),SSW_ReviewedOn,103),'-') As ReviewedOn From Content_Management_Master cm 
			//Full Outer Join StandardAudit_ScheduleConduct_WorkPaper a On cm.cmm_ID = a.SSW_WPCheckListID And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId 
			//Left Join sad_userdetails b on b.Usr_ID=a.SSW_CrBy Left Join sad_userdetails c on c.Usr_ID=a.SSW_ReviewedBy 
			//Where (cm.cmm_Delflag = 'A' And cm.cmm_Category = 'WCM') Or a.SSW_ID Is Not Null And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId Order by 
			//CASE WHEN cm.cmm_Desc IS NULL THEN 1 ELSE 0 END, cm.cmm_ID ASC";


			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");


			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);


			//Changed by steffi on 15-07-2025, Type of test data stroing multiple value.
			string query = @"SELECT A.PKID,A.WorkpaperChecklist,A.WorkpaperNo,A.WorkpaperRef,A.Observation,A.Conclusion,A.ReviewerComments,
                     STUFF((SELECT ', ' + cmm_Desc FROM Content_Management_Master
                     WHERE CHARINDEX(',' + CAST(cmm_ID AS VARCHAR) + ',', ',' + REPLACE(A.TypeOfTest, ' ', '') + ',') > 0
                     FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS TypeOfTest,
                     A.Status,A.AttachID,A.CreatedBy,A.CreatedOn,A.ReviewedBy,A.ReviewedOn
                     FROM (SELECT ISNULL(cm.cmm_ID, a.SSW_ID) As PKID,IsNull(cm.cmm_Desc,'NA') As WorkpaperChecklist,
                     SSW_WorkpaperNo As WorkpaperNo,SSW_WorkpaperRef As WorkpaperRef,SSW_Observation As Observation,SSW_Conclusion As Conclusion,
                     SSW_ReviewerComments As ReviewerComments,SSW_TypeOfTest as TypeOfTest,
                     CASE WHEN a.SSW_Status=1 THEN 'Open' WHEN a.SSW_Status=2 THEN 'WIP' WHEN a.SSW_Status=3 THEN 'Closed' END Status,
                     SSW_AttachID As AttachID,IsNull(b.usr_FullName,'-') As CreatedBy,ISNULL(Convert(Varchar(10),SSW_CrOn,103),'-') As CreatedOn, 
                     IsNull(c.usr_FullName,'-') As ReviewedBy,ISNULL(Convert(Varchar(10),SSW_ReviewedOn,103),'-') As ReviewedOn,
                     cm.cmm_Desc,cm.cmm_ID FROM Content_Management_Master cm 
                     FULL OUTER JOIN StandardAudit_ScheduleConduct_WorkPaper a ON cm.cmm_ID = a.SSW_WPCheckListID And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId 
                     LEFT JOIN sad_userdetails b on b.Usr_ID=a.SSW_CrBy 
                     LEFT JOIN sad_userdetails c on c.Usr_ID=a.SSW_ReviewedBy 
                     WHERE cm.cms_KeyComponent IN (SELECT SA_AuditFrameworkId FROM StandardAudit_Schedule WHERE SA_ID = @AuditNo AND SA_CompID = @CompId) And
					 (cm.cmm_Delflag = 'A' And cm.cmm_Category = 'WCM') OR a.SSW_ID Is Not Null And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId) A
                     ORDER BY CASE WHEN A.cmm_Desc IS NULL THEN 1 ELSE 0 END, A.cmm_ID ASC";

			var result = await connection.QueryAsync<WorkspaceSummaryDto>(query, new
			{
				CompId = compId,
				AuditNo = auditNo
			});

			return result;
		}



		public async Task<IEnumerable<CMADto>> GetCAMDetailsAsync(int compId, int auditNo)
		{
			//using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);

			string query = @"
            Select DENSE_RANK() OVER (ORDER BY SACAM_PKID) As SrNo,SACAM_PKID As DBpkId,SACAM_SSW_WorkpaperNo As WorkpaperNo,
            SACAM_SSW_WorkpaperRef As WorkpaperRef,SACAM_SSW_Observation As Observation,SACAM_SSW_Conclusion As Conclusion, 
            ISNULL(STUFF((SELECT ', ' + cmm.CMM_Desc FROM STRING_SPLIT(CAST(a.SACAM_SSW_TypeOfTest AS VARCHAR(MAX)), ',') AS s JOIN Content_Management_Master cmm ON TRY_CAST(s.value AS INT) = cmm.CMM_ID
            WHERE cmm.CMM_Category = 'TOT' FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS TypeOfTest,
            Case When a.SACAM_SSW_Status=1 then 'Open' When a.SACAM_SSW_Status=2 then 'WIP' When a.SACAM_SSW_Status=3 then 'Closed' End 
            Status,SACAM_SSW_CriticalAuditMatter As CAM,ISNULL(SACAM_AttachID,0) As AttachmentID, Case When a.SACAM_SSW_ExceededMateriality=1 then 'Yes' 
            When a.SACAM_SSW_ExceededMateriality=2 then 'No' When a.SACAM_SSW_ExceededMateriality=3 then 'NA' End ExceededMateriality, 
            SACAM_DescriptionOrReasonForSelectionAsCAM As DescriptionOrReasonForSelectionAsCAM,SACAM_AuditProcedureUndertakenToAddressTheCAM As 
            AuditProcedureUndertakenToAddressTheCAM From StandardAudit_AuditSummary_CAMDetails a Where SACAM_SA_ID=@AuditNo And SACAM_CompID=@CompId";


			var result = await connection.QueryAsync<CMADto>(query, new
			{
				CompId = compId,
				AuditNo = auditNo
			});

			return result;
		}


 
        public async Task<bool> UpdateStandardAuditASCAMdetailsAsync(int sacm_pkid, int sacm_sa_id, UpdateStandardAuditASCAMdetailsDto dto)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            using var connection = new SqlConnection(connectionString);

            var query = @"
     Update StandardAudit_AuditSummary_CAMDetails set SACAM_DescriptionOrReasonForSelectionAsCAM=@SACAM_DescriptionOrReasonForSelectionAsCAM,
     SACAM_AuditProcedureUndertakenToAddressTheCAM=@SACAM_AuditProcedureUndertakenToAddressTheCAM  
     Where SACAM_PKID=@SACAM_PKID and SACAM_SA_ID=@SACAM_SA_ID";

			var parameters = new DynamicParameters(dto);
			parameters.Add("SACAM_PKID", sacm_pkid);
			parameters.Add("SACAM_SA_ID", sacm_sa_id);

			var rowsAffected = await connection.ExecuteAsync(query, parameters);

			return rowsAffected > 0;
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


  //      private async Task<int> GenerateNextAttachmentIdAsync(int compId)
  //      {

		//	string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

		//	if (string.IsNullOrEmpty(dbName))
		//		throw new Exception("CustomerCode is missing in session. Please log in again.");

		//	// ✅ Step 2: Get the connection string
		//	var connectionString = _configuration.GetConnectionString(dbName);
             
		//	using var connection = new SqlConnection(connectionString);

		//	{
		//		Directory.CreateDirectory(accessCodeDirectory);
		//	}

		//	var sFoldersToCreate = new List<string> { "Tempfolder", sFolderName, imgDocType };

		//	{
		//		Directory.CreateDirectory(accessCodeDirectory);
		//	}

		//	var sFoldersToCreate = new List<string> { "Tempfolder", sFolderName, imgDocType };

		//	foreach (var sFolder in sFoldersToCreate)
		//	{
		//		if (!string.IsNullOrEmpty(sFolder))
		//		{
		//			accessCodeDirectory = Path.Combine(accessCodeDirectory.TrimEnd('\\'), sFolder);
		//			if (!Directory.Exists(accessCodeDirectory))
		//			{
		//				Directory.CreateDirectory(accessCodeDirectory);
		//			}
		//		}
		//	}
		//	return accessCodeDirectory;
		//}


		private async Task<int> GenerateNextAttachmentIdAsync(int compId)
		{

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				return await connection.ExecuteScalarAsync<int>(
					@"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM Edt_Attachments WHERE ATCH_CompID = @CompId",
					new { compId });
			}
		}

		private async Task<int> GetDocumentIdAsync(int compId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				return await connection.ExecuteScalarAsync<int>(
					@"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @compId",
					new { compId });
			}
		}

		private async Task<int> CheckDocumentIdAsync(int compId, int iAttachID)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				return await connection.ExecuteScalarAsync<int>(
					@"SELECT ATCH_DOCID FROM EDT_ATTACHMENTS WHERE ATCH_CompID = @compId and ATCH_ID=@iAttachID",
					new { compId, iAttachID });
			}
		}


		private async Task<string> GetAccessCodeDirectory(int compId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				return await connection.ExecuteScalarAsync<string>(
					@"Select sad_Config_Value from sad_config_settings where sad_Config_Key='ImgPath' and sad_compid=@compId",
					new { compId });
			}
		}

		private async Task<string> GetUserName(int compId, int userId)
		{
			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				return await connection.ExecuteScalarAsync<string>(
					@"Select Usr_Email from sad_UserDetails where Usr_Id=@userId and usr_compId=@compId",
					new { compId, userId });
			}
		}


		//     private async Task<int> SaveAttachmentsModulewise(CMADtoAttachment dto, int CompId, string AccessCodeDirectory, string sModule, string sFilePath, int iUserId, int iAttachID)
		//     {

		//         string sFileExtension = "";
		//         string sFileName = ""; int iDocID = 0;
		//         int iPosSlash = sFilePath.LastIndexOf('\\');
		//         int iPosDot = sFilePath.LastIndexOf('.');

		//         if (iPosDot != -1)
		//         {
		//             sFileName = sFilePath.Substring(iPosSlash + 1, iPosDot - (iPosSlash + 1));
		//             sFileExtension = sFilePath.Substring(iPosDot + 1);
		//         }
		//         else
		//         {
		//             sFileName = sFilePath.Substring(iPosSlash, sFilePath.Length - (iPosSlash + 1));
		//             sFileExtension = "unk";
		//         }

		//         sFileName = sFileName.Replace("&", " and").Substring(0, Math.Min(sFileName.Length, 95));

 
		//         iAttachID = await GenerateNextAttachmentIdAsync(CompId);
		//         iDocID = await GetDocumentIdAsync(CompId);
 
		//         iAttachID = await GenerateNextAttachmentIdAsync(CompId);
		//         iDocID = await GetDocumentIdAsync(CompId);
            //iAttachID = iAttachID == 0 ? await GenerateNextAttachmentIdAsync(CompId) : iAttachID;
            //iDocID = await GetDocumentIdAsync(CompId);

		//         if (iDocID == 0)
		//         {
		//             int icheck = await CheckDocumentIdAsync(CompId, iAttachID);
		//             if (icheck > 0)
		//             {
		//                 iAttachID = await GenerateNextAttachmentIdAsync(CompId);
		//                 iDocID = await GetDocumentIdAsync(CompId);
		//             }
		//         }

		//         long fileSize = new FileInfo(sFilePath).Length;
		//         string FilePath1 = sFilePath;


		//string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

		//if (string.IsNullOrEmpty(dbName))
		//	throw new Exception("CustomerCode is missing in session. Please log in again.");

		//// ✅ Step 2: Get the connection string
		//var connectionString = _configuration.GetConnectionString(dbName);

		//using var connection = new SqlConnection(connectionString);
		//{
		//             await connection.OpenAsync();
		//             await connection.ExecuteAsync(
		//                 @"INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION, ATCH_FLAG,
		//                 ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, ATCH_Status, ATCH_CompID,Atch_Vstatus) VALUES (@AttachID,@DocID,@FileName,@FileExtension,
		//                 @UserId, @UserId, 1, 0, @fileSize,0,0,GetDate(),'X',@CompID,'A')",
		//                 new
		//                 {
		//                     AttachID = iAttachID,
		//                     DocID = iDocID,
		//                     FileName = sFileName,
		//                     FileExtension = sFileExtension,
		//                     UserId = dto.UserId,
		//                     fileSize = fileSize,
		//                     CompID = CompId
		//                 });
		//         }

		//         //CheckOrCreateFileIsImageOrDocumentDirectory

		//         sFileExtension = Path.GetExtension(sFilePath).ToLower();
		//         string[] aImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
		//         string[] aDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };

		//         string sFileType = "";

		//         if (aImageExtensions.Contains(sFileExtension) == true)
		//         {
		//             sFileType = "Images";
		//         }
		//         else if (aDocumentExtensions.Contains(sFileExtension) == true)
		//         {
		//             sFileType = "Documents";
		//         }

		//         //string sFileType = aImageExtensions.Contains(sFileExtension) ? "Images" :
		//         //                 (aDocumentExtensions.Contains(sFileExtension) ? "Documents" :;

		//         string sAccessCodeModulePath = Path.Combine(AccessCodeDirectory, sModule);
		//         if (!Directory.Exists(sAccessCodeModulePath))
		//             Directory.CreateDirectory(sAccessCodeModulePath);

		//         string sFinalDirectory = Path.Combine(sAccessCodeModulePath, sFileType, Convert.ToInt32(iDocID / 301).ToString());
		//         if (!Directory.Exists(sFinalDirectory))
		//             Directory.CreateDirectory(sFinalDirectory);


		//         String sFinalFilePath = sFinalDirectory + "\\" + iDocID + "." + sFileExtension;

		//         if (File.Exists(sFinalFilePath))
		//         { File.Delete(sFinalFilePath); }

		//         //Encrypt(sFilePath, sFinalFilePath);

		//         string EncryptionKey = "MAKV2SPBNI99212";
		//         using (Aes encryptor = Aes.Create())
		//         {
		//             Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
		//             encryptor.Key = pdb.GetBytes(32);
		//             encryptor.IV = pdb.GetBytes(16);
		//             using (FileStream fs = new FileStream(sFinalFilePath, FileMode.Create))
		//             {
		//                 using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
		//                 {
		//                     using (FileStream fsInput = new FileStream(sFilePath, FileMode.Open))
		//                     {
		//                         int data;
		//                         while ((data = fsInput.ReadByte()) != -1)
		//                         {
		//                             cs.WriteByte((byte)data);
		//                         }
		//                     }
		//                 }
		//             }
		//         }

		//         if (File.Exists(sFilePath))
		//             File.Delete(sFilePath);

		//         return iAttachID;
		//     }

		private async Task<int> SaveAttachmentsModulewise(CMADtoAttachment dto, int CompId, string AccessCodeDirectory, string sModule, string sFilePath, int iUserId, int iAttachID)
		{

			string sFileExtension = "";
			string sFileName = ""; int iDocID = 0;
			int iPosSlash = sFilePath.LastIndexOf('\\');
			int iPosDot = sFilePath.LastIndexOf('.');

			if (iPosDot != -1)
			{
				sFileName = sFilePath.Substring(iPosSlash + 1, iPosDot - (iPosSlash + 1));
				sFileExtension = sFilePath.Substring(iPosDot + 1);
			}
			else
			{
				sFileName = sFilePath.Substring(iPosSlash, sFilePath.Length - (iPosSlash + 1));
				sFileExtension = "unk";
			}

			sFileName = sFileName.Replace("&", " and").Substring(0, Math.Min(sFileName.Length, 95));

			iAttachID = iAttachID == 0 ? await GenerateNextAttachmentIdAsync(CompId) : iAttachID;
			iDocID = await GetDocumentIdAsync(CompId);

			if (iDocID == 0)
			{
				int icheck = await CheckDocumentIdAsync(CompId, iAttachID);
				if (icheck > 0)
				{
					iAttachID = await GenerateNextAttachmentIdAsync(CompId);
					iDocID = await GetDocumentIdAsync(CompId);
				}
			}

			long fileSize = new FileInfo(sFilePath).Length;
			string FilePath1 = sFilePath;


			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				await connection.ExecuteAsync(
					@"INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION, ATCH_FLAG,
              ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, ATCH_Status, ATCH_CompID,Atch_Vstatus) VALUES (@AttachID,@DocID,@FileName,@FileExtension,
              @UserId, @UserId, 1, 0, @fileSize,0,0,GetDate(),'X',@CompID,'A')",
					new
					{
						AttachID = iAttachID,
						DocID = iDocID,
						FileName = sFileName,
						FileExtension = sFileExtension,
						UserId = dto.UserId,
						fileSize = fileSize,
						CompID = CompId
					});
			}

			//CheckOrCreateFileIsImageOrDocumentDirectory

			sFileExtension = Path.GetExtension(sFilePath).ToLower();
			string[] aImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
			string[] aDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };

			string sFileType = "";

			if (aImageExtensions.Contains(sFileExtension) == true)
			{
				sFileType = "Images";
			}
			else if (aDocumentExtensions.Contains(sFileExtension) == true)
			{
				sFileType = "Documents";
			}

			//string sFileType = aImageExtensions.Contains(sFileExtension) ? "Images" :
			//                 (aDocumentExtensions.Contains(sFileExtension) ? "Documents" :;

			string sAccessCodeModulePath = Path.Combine(AccessCodeDirectory, sModule);
			if (!Directory.Exists(sAccessCodeModulePath))
				Directory.CreateDirectory(sAccessCodeModulePath);

			string sFinalDirectory = Path.Combine(sAccessCodeModulePath, sFileType, Convert.ToInt32(iDocID / 301).ToString());
			if (!Directory.Exists(sFinalDirectory))
				Directory.CreateDirectory(sFinalDirectory);


			String sFinalFilePath = sFinalDirectory + "\\" + iDocID + "." + sFileExtension;

			if (File.Exists(sFinalFilePath))
			{ File.Delete(sFinalFilePath); }

			//Encrypt(sFilePath, sFinalFilePath);

			string EncryptionKey = "MAKV2SPBNI99212";
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (FileStream fs = new FileStream(sFinalFilePath, FileMode.Create))
				{
					using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						using (FileStream fsInput = new FileStream(sFilePath, FileMode.Open))
						{
							int data;
							while ((data = fsInput.ReadByte()) != -1)
							{
								cs.WriteByte((byte)data);
							}
						}
					}
				}
			}

			if (File.Exists(sFilePath))
				File.Delete(sFilePath);

			return iAttachID;
		}

		private async Task UpdateStandardAuditASCAMAttachmentdetails(int compId, int CAMPkID, int attachId)
		{

			string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

			if (string.IsNullOrEmpty(dbName))
				throw new Exception("CustomerCode is missing in session. Please log in again.");

			// ✅ Step 2: Get the connection string
			var connectionString = _configuration.GetConnectionString(dbName);

			using var connection = new SqlConnection(connectionString);
			{
				await connection.OpenAsync();
				await connection.ExecuteAsync(
					@"Update StandardAudit_AuditSummary_CAMDetails set SACAM_AttachID=@attachId
              WHERE SACAM_PKID = @CAMPkID",
					new { attachId = attachId, CAMPkID = CAMPkID, compId = compId });
			}
		}

        public async Task<string> UploadCMAAttachmentsAsync(CMADtoAttachment dto)
        {
            try
            {
                string accessCodeDirectory = await GetAccessCodeDirectory(dto.CompId);

                // 1. Validate file
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("Invalid file.");

                // 2. Generate file path
                string fileSavingPath = await CheckOrCreateCustomDirectory(accessCodeDirectory, dto.UserId.ToString(), "Upload");

                var selectedFileName = Path.GetFileName(dto.File.FileName);
                var fullFilePath = Path.Combine(fileSavingPath, selectedFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // 3. Save attachment
                int attachId = dto.CAMDPKID > 0 ? dto.CAMDPKID : 0;
                attachId = await SaveAttachmentsModulewise(dto, dto.CompId, accessCodeDirectory, "MRIssue", fullFilePath, dto.UserId, attachId);

                // 4. Update audit details
                UpdateStandardAuditASCAMAttachmentdetails(dto.CompId, dto.CAMDPKID, attachId);

                return "Success";
            }
            catch (Exception ex)
            {
                // Log the exception for better error handling
                return $"Error: {ex.Message}";
            }
        }


        //var sSelectedFileName = Path.GetFileName(dto.File.FileName);
        //var fileExt = Path.GetExtension(sSelectedFileName)?.TrimStart('.');
        //var sFullFilePath = Path.Combine(sFileSavingPath, sSelectedFileName);

        //using (var stream = new FileStream(sFullFilePath, FileMode.Create))
        //{
        //    await dto.File.CopyToAsync(stream);
        //}

        //int attachId = dto.CAMDPKID > 0 ? dto.CAMDPKID : 0;
        ////2.SaveAttachmentsModulewise
        //attachId = await SaveAttachmentsModulewise(dto, dto.CompId, AccessCodeDirectory, "MRIssue", sFullFilePath, dto.UserId, attachId);


        //public async Task<string> UploadCMAAttachmentsAsync(CMADtoAttachment dto)
        //{
        //    try
        //    {

        //        string AccessCodeDirectory = await GetAccessCodeDirectory(dto.CompId);

        //        string UserLoginName = await GetUserName(dto.CompId, dto.UserId);

        //        //1. Generate Filepath
        //        String sFileSavingPath = await CheckOrCreateCustomDirectory(AccessCodeDirectory, UserLoginName, "Upload");


        //        if (dto.File == null || dto.File.Length == 0)
        //            throw new ArgumentException("Invalid file.");

        //        var sSelectedFileName = Path.GetFileName(dto.File.FileName);
        //        var fileExt = Path.GetExtension(sSelectedFileName)?.TrimStart('.');
        //        var sFullFilePath = Path.Combine(sFileSavingPath, sSelectedFileName);

        //        using (var stream = new FileStream(sFullFilePath, FileMode.Create))
        //        {
        //            await dto.File.CopyToAsync(stream);
        //        }

        //        int attachId = 0;
        //        //2.SaveAttachmentsModulewise

        //        attachId = await SaveAttachmentsModulewise(dto, dto.CompId, AccessCodeDirectory, "MRIssue", sFullFilePath, dto.UserId, attachId);


        //        UpdateStandardAuditASCAMAttachmentdetails(dto.CompId, dto.CAMDPKID, attachId);

        //        return "Successs";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for better error handling
        //        return $"Error: {ex.Message}";
        //    }
        //}


        public async Task<IEnumerable<CAMAttachmentDetailsDto>> GetCAMAttachmentDetailsAsync(int AttachID)
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
			string query = "";

			query = @"Select Atch_DocID,ATCH_FNAME,ATCH_EXT,ATCH_Desc,Usr_FullName as ATCH_CreatedBy,Convert(Varchar(10),ATCH_CREATEDON,103) as 
                ATCH_CREATEDON,ATCH_SIZE,ATCH_ReportType,CASE WHEN Atch_Vstatus = 'AS' THEN 'Not Shared' WHEN Atch_Vstatus = 'A' THEN 'Shared' 
                WHEN Atch_Vstatus = 'C' THEN 'Received' END AS Atch_Vstatus From edt_attachments A join Sad_Userdetails B on A.ATCH_CreatedBy = B.Usr_ID 
                Where ATCH_CompID=1 And ATCH_ID = @atch_DocID AND ATCH_Status <> 'D' and Atch_Vstatus in ('A','AS','C') Order by ATCH_CREATEDON ";


			var result = await connection.QueryAsync<CAMAttachmentDetailsDto>(query, new
			{
				atch_DocID = AttachID
			}, transaction);

			return result;
		}

        public async Task<string> GenerateCAMReportAndGetURLPathAsync(int compId, int auditId, string format)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Audit_Issues_{timestamp}";

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await GenerateCAMPdfAsync(compId, auditId);
                    contentType = "application/pdf";
                    fileName += ".pdf";
                }
                else
                {
                    throw new ApplicationException("Unsupported format. Only PDF is currently supported.");
                }

                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tempfolder", compId.ToString());
                Directory.CreateDirectory(tempFolder);

                var filePath = Path.Combine(tempFolder, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await File.WriteAllBytesAsync(filePath, fileBytes);

                string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                string downloadUrl = $"{baseUrl}/Tempfolder/{compId}/{fileName}";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the report.", ex);
            }
        }

        private async Task<byte[]> GenerateCAMPdfAsync(int compId, int auditId)
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

            var result = await connection.QueryFirstOrDefaultAsync<(int EpPkId, int CustID, string CustName, string YearName, string AuditNo)>(
                @"SELECT LOE.LOE_ID AS EpPkId, SA.SA_CustID As CustID, CUST.CUST_NAME As CustName, YMS.YMS_ID AS YearName, SA_AuditNo + ' - ' + CMA.CMM_Desc As AuditNo FROM StandardAudit_Schedule AS SA
                  LEFT JOIN SAD_CUST_LOE AS LOE ON LOE.LOE_CustomerId = SA.SA_CustID AND LOE.LOE_YearId = SA.SA_YearID AND LOE.LOE_ServiceTypeId = SA.SA_AuditTyPeId
                  LEFT JOIN SAD_CUSTOMER_MASTER AS CUST ON SA.SA_CustID = CUST_ID 
                  LEFT JOIN YEAR_MASTER AS YMS ON YMS.YMS_YEARID = SA.SA_YearID
                  LEFT JOIN Content_Management_Master CMA On CMA.cmm_ID = SA.SA_AuditTypeID
                  WHERE LOE.LOE_CompID = @CompId AND SA.SA_ID = @AuditId;", new { CompId = compId, AuditId = auditId });

            IEnumerable<CMADto> CAMresult = await GetCAMDetailsAsync(compId, auditId);
            List<CMADto> dtoCAM = CAMresult.ToList();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

            return await Task.Run(() =>
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4.Landscape());
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().PaddingBottom(10).Text("Audit Issues and Closure Report").FontSize(16).Bold();
                            column.Item().Text(text =>
                            {
                                text.Span("Client Name: ").FontSize(10).Bold();
                                text.Span(result.CustName).FontSize(10);
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Audit No: ").FontSize(10).Bold();
                                text.Span(result.AuditNo).FontSize(10);
                            });


                            column.Item().PaddingBottom(10);

                            if (dtoCAM.Any() == true)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.5f);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.5f);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(3);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Sl No").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Workpaper Ref").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("CAM").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Exceeded Materiality").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Deviations/Exceptions Noted").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Conclusion").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Type of Test").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Status").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Description & Reason for selection as CAM").FontSize(10).Bold();
                                        header.Cell().Element(CellStyle).Text("Audit Procedure undertaken to address the CAM").FontSize(10).Bold();
                                    });

                                    int slNo = 1;
                                    foreach (var details in dtoCAM)
                                    {
                                        table.Cell().Element(CellStyle).Text(slNo.ToString()).FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.WorkpaperRef ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.CAM ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.ExceededMateriality ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Observation ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Conclusion ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.TypeOfTest ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.Status ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.DescriptionOrReasonForSelectionAsCAM ?? "").FontSize(10);
                                        table.Cell().Element(CellStyle).Text(details.AuditProcedureUndertakenToAddressTheCAM ?? "").FontSize(10);
                                        slNo++;
                                    }

                                    static IContainer CellStyle(IContainer container) =>
                                        container.Border(0.5f).PaddingVertical(3).PaddingHorizontal(4);
                                });
                            }
                        });
                    });
                });
                using var ms = new MemoryStream();
                document.GeneratePdf(ms);
                return ms.ToArray();
            });
        }


		//public async Task<IEnumerable<CAMAttachmentDetailsDto>> GetCAMAttachmentDetailsAsync(int AttachID, CAMAttachmentDetailsDto dto)
		//{
		//	using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		//	await connection.OpenAsync();
		//	using var transaction = connection.BeginTransaction();

		//	string query = "";


		//	//var AttachID = await connection.ExecuteScalarAsync<int>(@"Select SACAM_AttachID from StandardAudit_AuditSummary_CAMDetails where SACAM_PKID=@SACAM_PKID", new { SACAM_PKID = auditNo }, transaction);

		//    query = @"Select Atch_DocID,ATCH_FNAME,ATCH_EXT,ATCH_Desc,Usr_FullName as ATCH_CreatedBy,Convert(Varchar(10),ATCH_CREATEDON,103) as 
		//                  ATCH_CREATEDON,ATCH_SIZE,ATCH_ReportType,CASE WHEN Atch_Vstatus = 'AS' THEN 'Not Shared' WHEN Atch_Vstatus = 'A' THEN 'Shared' 
		//                  WHEN Atch_Vstatus = 'C' THEN 'Received' END AS Atch_Vstatus From edt_attachments A join Sad_Userdetails B on A.ATCH_CreatedBy = B.Usr_ID 
		//                  Where ATCH_CompID=1 And ATCH_ID = @atch_DocID AND ATCH_Status <> 'D' and Atch_Vstatus in ('A','AS','C') Order by ATCH_CREATEDON ";


		//	var result = await connection.QueryAsync<CAMAttachmentDetailsDto>(query, new
		//	{
		//		ATCH_FNAME = dto.ATCH_FNAME,
		//		ATCH_EXT = dto.ATCH_EXT,
		//		ATCH_Desc = dto.ATCH_Desc,
		//		ATCH_CREATEDBY = dto.ATCH_CREATEDBY,
		//		ATCH_CREATEDON = dto.ATCH_CREATEDON,
		//		atch_DocID = AttachID
		//	}, transaction);

		//	return result;
		//}


		//private async Task<int> SaveAttachmentsModulewise(int iCompID,string sAccessCodeDirectory, string sModule, string sFilePath, int iUserId, int iAttachID)
		//{
		//    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
		//    {
		//        await connection.OpenAsync();
		//        //  return await connection.ExecuteScalarAsync<int>(
		//        //      @"SELECT ISNULL(MAX( ATCH_DOCID), 0) + 1 FROM Edt_Attachments 
		//        //WHERE ATCH_COMPID = @CustomerId AND ATCH_AuditID = @AuditId",
		//        //      new { customerId, auditId });
		//        string sFileExtension = "";
		//        string sFileName = "";
		//        int iPosSlash = sFilePath.LastIndexOf('\\') + 1;
		//        int iPosDot = sFilePath.LastIndexOf('.') + 1;

		//        if (iPosDot != 0)
		//        {
		//            sFileName = sFilePath.Substring(iPosSlash, iPosDot - iPosSlash - 1);
		//            sFileExtension = sFilePath.Substring(iPosDot);
		//        }
		//        else
		//        {
		//            sFileName = sFilePath.Substring(iPosSlash);
		//            sFileExtension = "unk";
		//        }
		//        sFileName = sFileName.Replace("&", " and").Substring(0, Math.Min(sFileName.Length, 95));


		//        iAttachID = iAttachID == 0 ?
		//            iAttachID = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID=@iCompID") : iAttachID;
		//        int iDocID = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID=@iCompID");

		//        if (iDocID == 0)
		//        {
		//           int docID = await connection.ExecuteScalarAsync<int>(@"SELECT ATCH_DOCID FROM EDT_ATTACHMENTS WHERE ATCH_CompID=@iCompID AND ATCH_ID=@iAttachID  ");

		//            if (docID > 0)
		//            {
		//                iAttachID = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ATCH_ID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID=@iCompID");
		//                iDocID = await connection.ExecuteScalarAsync<int>(@"SELECT ISNULL(MAX(ATCH_DOCID), 0) + 1 FROM EDT_ATTACHMENTS WHERE ATCH_CompID=@iCompID");
		//            }
		//        }

		//        string sSql = "";
		//        long fileSize = new FileInfo(sFilePath).Length;
		//        await connection.ExecuteAsync(
		//            @"INSERT INTO EDT_ATTACHMENTS (ATCH_ID, ATCH_DOCID, ATCH_FNAME, ATCH_EXT, ATCH_CREATEDBY, ATCH_MODIFIEDBY, ATCH_VERSION, ATCH_FLAG, 
		//        ATCH_SIZE, ATCH_FROM, ATCH_Basename, ATCH_CREATEDON, ATCH_Status, ATCH_CompID,Atch_Vstatus)
		//        VALUES (
		//        @iAttachID, @iDocID, @AuditId, @sFileName,
		//        @sFileExtension, @iUserId, @UserId, 1,0, @fileSize,0,0,GetDate(),'X',@iCompID,'A')",
		//            new
		//            {
		//                RemarkId = remarkId,
		//                CustomerId = dto.CustomerId,
		//                AuditId = dto.AuditId,
		//                RequestedId = requestedId,
		//                Remark = dto.Remark,
		//                UserId = dto.UserId,
		//                //Type = dto.Type,
		//                AttachId = attachId
		//            });


		//        string sFileExtension1 = Path.GetExtension(sFilePath).ToLower();
		//        string[] aImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".svg", ".psd", ".ai", ".eps", ".ico", ".webp", ".raw", ".heic", ".heif", ".exr", ".dng", ".jp2", ".j2k", ".cr2", ".nef", ".orf", ".arw", ".raf", ".rw2", ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg", ".3gp", ".ts", ".m2ts", ".vob", ".mts", ".divx", ".ogv" };
		//        string[] aDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".ppsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".csv", ".pptm", ".xlsm", ".docm", ".xml", ".json", ".yaml", ".key", ".numbers", ".pages", ".tar", ".zip", ".rar" };

		//        string sFileType = aImageExtensions.Contains(sFileExtension) ? "Images" :
		//                         (aDocumentExtensions.Contains(sFileExtension) ? "Documents" : "Others");

		//        string sAccessCodeModulePath = Path.Combine(sAccessCodeDirectory, sModule);
		//        if (!Directory.Exists(sAccessCodeModulePath)) Directory.CreateDirectory(sAccessCodeModulePath);

		//        int iFolder = (Convert.ToInt32(iDocID) / 301);
		//        string sFinalDirectory = Path.Combine(sAccessCodeModulePath, sFileType, iFolder.ToString());
		//        if (!Directory.Exists(sFinalDirectory)) Directory.CreateDirectory(sFinalDirectory);
		//        string sFinalFilePath = $"{sFinalDirectory}\\{iDocID}.{sFileExtension}";

		//        if (File.Exists(sFinalFilePath)) File.Delete(sFinalFilePath);
		//        // File.Copy(sFilePath, sFinalFilePath);



		//        string EncryptionKey = "MAKV2SPBNI99212";
		//        using (Aes encryptor = Aes.Create())
		//        {
		//            var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
		//                                                            0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
		//                                                            0x76 });
		//            encryptor.Key = pdb.GetBytes(32);
		//            encryptor.IV = pdb.GetBytes(16);
		//            using (FileStream fs = new FileStream(sFinalFilePath, FileMode.Create))
		//            {
		//                using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
		//                {
		//                    using (FileStream fsInput = new FileStream(sFilePath, FileMode.Open))
		//                    {
		//                        int data;
		//                        while ((data = fsInput.ReadByte()) != -1)
		//                        {
		//                            cs.WriteByte((byte)data);
		//                        }
		//                    }
		//                }
		//            }
		//        }


		//        if (File.Exists(sFilePath)) File.Delete(sFilePath);
		//        return iAttachID;
		//    }
		//}


		//public async Task<string> CAMAttachmentAsync( string sAccessCodeDirectory, string sFolderName, int attachId, IFormFile file, CMADtoAttachment dto)
		//{

		//    if (!Directory.Exists(sAccessCodeDirectory))
		//    {
		//        Directory.CreateDirectory(sAccessCodeDirectory);
		//    }

		//    var sFoldersToCreate = new List<string> { "Tempfolder", sFolderName, "Upload" };
		//    foreach (var sFolder in sFoldersToCreate)
		//    {
		//        if (!string.IsNullOrEmpty(sFolder))
		//        {
		//            sAccessCodeDirectory = Path.Combine(sAccessCodeDirectory.TrimEnd('\\'), sFolder);
		//            if (!Directory.Exists(sAccessCodeDirectory))
		//            {
		//                Directory.CreateDirectory(sAccessCodeDirectory);
		//            }
		//        }
		//    }




		//public async Task<bool> SaveAllLoeDataAsync(AddEngagementDto dto)
		//{
		//    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		//    await connection.OpenAsync();
		//    using var transaction = connection.BeginTransaction();

		//    try
		//    {
		//        // 1. Insert into SAD_CUST_LOE
		//        dto.LoeId = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @NewId INT = (SELECT ISNULL(MAX(LOE_Id), 0) + 1 FROM SAD_CUST_LOE);
		//      INSERT INTO SAD_CUST_LOE (
		//          LOE_Id, LOE_YearId, LOE_CustomerId, LOE_ServiceTypeId, LOE_NatureOfService,
		//          LOE_LocationIds, LOE_TimeSchedule, LOE_ReportDueDate,
		//          LOE_ProfessionalFees, LOE_OtherFees, LOE_ServiceTax, LOE_RembFilingFee,
		//          LOE_CrBy, LOE_CrOn, LOE_Total, LOE_Name, LOE_Frequency,
		//          LOE_FunctionId, LOE_SubFunctionId, LOE_STATUS, LOE_Delflag, LOE_IPAddress, LOE_CompID
		//      )
		//      VALUES (
		//          @NewId, @LoeYearId, @LoeCustomerId, @LoeServiceTypeId, @LoeNatureOfService,
		//          '0', @LoeTimeSchedule, @LoeReportDueDate,
		//          '0', '0', '0', '0',
		//          1, GETDATE(), @LoeTotal, @LoeName, @LoeFrequency,
		//          0, '1', 'A', 'A', @LoeIpaddress, @LoeCompId);
		//      SELECT @NewId;", dto, transaction);

		//        // 2. Insert into LOE_Template
		//        dto.LOET_Id = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @TemplateId INT = (SELECT ISNULL(MAX(LOET_Id), 0) + 1 FROM LOE_Template);
		//      INSERT INTO LOE_Template (
		//          LOET_Id, LOET_LOEID , LOET_CustomerId, LOET_FunctionId, LOET_ScopeOfWork,
		//          LOET_Frequency, LOET_ProfessionalFees, LOET_Delflag, LOET_STATUS,
		//          LOET_CrOn, LOET_CrBy, LOET_IPAddress, LOET_CompID, LOE_AttachID
		//      )
		//      VALUES (
		//          @TemplateId, @LoeId, @LoeCustomerId, 0, @LoeNatureOfService,
		//          @LoeFrequency, '0', 'A', 'A', GETDATE(), 1,
		//          @LoeIpaddress, @LoeCompId, @LoeAttachId);
		//      SELECT @TemplateId;", dto, transaction);

		//        // 3. Insert into LOE_AdditionalFees
		//        dto.FeeName = await connection.QueryFirstOrDefaultAsync<string>(
		//            @"SELECT cmm_Desc FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
		//            new { dto.LoeCompId }, transaction);

		//        dto.ExpensesId = await connection.QueryFirstOrDefaultAsync<int>(
		//            @"SELECT cmm_ID FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
		//            new { dto.LoeCompId }, transaction);

		//        dto.FeesId = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @NewFeesId INT = (SELECT ISNULL(MAX(LAF_ID), 0) + 1 FROM LOE_AdditionalFees);
		//      INSERT INTO LOE_AdditionalFees (
		//          LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName,
		//          LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID
		//      )
		//      VALUES (
		//          @NewFeesId, @LoeId, @ExpensesId, @LoeTotal, @FeeName, 'A', 'C', 1,
		//          GETDATE(), @LoeIpaddress, @LoeCompId);
		//      SELECT @NewFeesId;", dto, transaction);

		//        await transaction.CommitAsync();
		//        return true;
		//    }
		//    catch
		//    {
		//        await transaction.RollbackAsync();
		//        throw;
		//    }
		//}

		 

		//    if (file == null || file.Length == 0)
		//        throw new ArgumentException("Invalid file.");

		//    var sSelectedFileName = Path.GetFileName(file.FileName);
		//    var fileExt = Path.GetExtension(sSelectedFileName)?.TrimStart('.');
		//    var sFullFilePath = Path.Combine(sAccessCodeDirectory, sSelectedFileName);

		//    using (var stream = new FileStream(sFullFilePath, FileMode.Create))
		//    {
		//        await file.CopyToAsync(stream);
		//    }


		//    int iAttachID = SaveAttachmentsModulewise();


		//}

		//private async Task<int> GenerateNextDocIdAsync(int customerId, int auditId)
		//{
		//    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
		//    {
		//        await connection.OpenAsync();
		//        return await connection.ExecuteScalarAsync<int>(
		//            @"SELECT ISNULL(MAX( ATCH_DOCID), 0) + 1 FROM Edt_Attachments 
		//WHERE ATCH_COMPID = @CustomerId AND ATCH_AuditID = @AuditId",
		//            new { customerId, auditId });
		//    }
		//}



		//public async Task<bool> SaveAllLoeDataAsync(AddEngagementDto dto)
		//{
		//    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
		//    await connection.OpenAsync();
		//    using var transaction = connection.BeginTransaction();

		//    try
		//    {
		//        // 1. Insert into SAD_CUST_LOE
		//        dto.LoeId = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @NewId INT = (SELECT ISNULL(MAX(LOE_Id), 0) + 1 FROM SAD_CUST_LOE);
		//      INSERT INTO SAD_CUST_LOE (
		//          LOE_Id, LOE_YearId, LOE_CustomerId, LOE_ServiceTypeId, LOE_NatureOfService,
		//          LOE_LocationIds, LOE_TimeSchedule, LOE_ReportDueDate,
		//          LOE_ProfessionalFees, LOE_OtherFees, LOE_ServiceTax, LOE_RembFilingFee,
		//          LOE_CrBy, LOE_CrOn, LOE_Total, LOE_Name, LOE_Frequency,
		//          LOE_FunctionId, LOE_SubFunctionId, LOE_STATUS, LOE_Delflag, LOE_IPAddress, LOE_CompID
		//      )
		//      VALUES (
		//          @NewId, @LoeYearId, @LoeCustomerId, @LoeServiceTypeId, @LoeNatureOfService,
		//          '0', @LoeTimeSchedule, @LoeReportDueDate,
		//          '0', '0', '0', '0',
		//          1, GETDATE(), @LoeTotal, @LoeName, @LoeFrequency,
		//          0, '1', 'A', 'A', @LoeIpaddress, @LoeCompId);
		//      SELECT @NewId;", dto, transaction);

		//        // 2. Insert into LOE_Template
		//        dto.LOET_Id = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @TemplateId INT = (SELECT ISNULL(MAX(LOET_Id), 0) + 1 FROM LOE_Template);
		//      INSERT INTO LOE_Template (
		//          LOET_Id, LOET_LOEID , LOET_CustomerId, LOET_FunctionId, LOET_ScopeOfWork,
		//          LOET_Frequency, LOET_ProfessionalFees, LOET_Delflag, LOET_STATUS,
		//          LOET_CrOn, LOET_CrBy, LOET_IPAddress, LOET_CompID, LOE_AttachID
		//      )
		//      VALUES (
		//          @TemplateId, @LoeId, @LoeCustomerId, 0, @LoeNatureOfService,
		//          @LoeFrequency, '0', 'A', 'A', GETDATE(), 1,
		//          @LoeIpaddress, @LoeCompId, @LoeAttachId);
		//      SELECT @TemplateId;", dto, transaction);

		//        // 3. Insert into LOE_AdditionalFees
		//        dto.FeeName = await connection.QueryFirstOrDefaultAsync<string>(
		//            @"SELECT cmm_Desc FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
		//            new { dto.LoeCompId }, transaction);

		//        dto.ExpensesId = await connection.QueryFirstOrDefaultAsync<int>(
		//            @"SELECT cmm_ID FROM Content_Management_Master WHERE cmm_Category = 'OE' AND CMM_CompID = @LoeCompId",
		//            new { dto.LoeCompId }, transaction);

		//        dto.FeesId = await connection.ExecuteScalarAsync<int>(
		//            @"DECLARE @NewFeesId INT = (SELECT ISNULL(MAX(LAF_ID), 0) + 1 FROM LOE_AdditionalFees);
		//      INSERT INTO LOE_AdditionalFees (
		//          LAF_ID, LAF_LOEID, LAF_OtherExpensesID, LAF_Charges, LAF_OtherExpensesName,
		//          LAF_Delflag, LAF_STATUS, LAF_CrBy, LAF_CrOn, LAF_IPAddress, LAF_CompID
		//      )
		//      VALUES (
		//          @NewFeesId, @LoeId, @ExpensesId, @LoeTotal, @FeeName, 'A', 'C', 1,
		//          GETDATE(), @LoeIpaddress, @LoeCompId);
		//      SELECT @NewFeesId;", dto, transaction);

		//        await transaction.CommitAsync();
		//        return true;
		//    }
		//    catch
		//    {
		//        await transaction.RollbackAsync();
		//        throw;
		//    }
		//}


	}


}

