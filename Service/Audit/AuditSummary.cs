using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using TracePca.Data;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Interface;
using TracePca.Interface.Audit;



namespace TracePca.Service.Audit
{
    public class AuditSummary : AuditSummaryInterface
    {

        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;

        public AuditSummary(Trdmyus1Context dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<List<ReportTypeDto>> GetReportTypesAsync(int compId, int templateId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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


        public async Task<DropDownDataDto> LoadAuditNoDataAsync(int CustID, int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var AuditNoDt = connection.QueryAsync<AuditSummaryDto>(@"
            Select SA_ID,SA_AuditNo + ' - ' + CMM_Desc As SA_AuditNo From StandardAudit_Schedule 
            Left Join Content_Management_Master on CMM_ID=SA_AuditTypeID Where SA_CompID = @CompId And SA_CustID=@CustID
            ORDER BY SA_ID ASC", new { CompId = compId , CustID = CustID });
             
            await Task.WhenAll(AuditNoDt);
            return new DropDownDataDto
            {
                AuditNoDetails = AuditNoDt.Result.ToList()
            };
        }

        public async Task<DropDownDataDto> LoadCustomerDataAsync(int compId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Open the connection once for better performance
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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




        public async Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryAsync(int compId, int customerId, int auditNo, int requestId, int yearId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
                RequestId = requestId,
                YearId = yearId
            });

            return result;
        }

        public async Task<IEnumerable<DocumentRequestSummaryDto>> GetDocumentRequestSummaryCompletionAuditAsync(int compId, int customerId, int auditNo, int requestId, int yearId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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


       



        public async Task<IEnumerable<AuditProgramSummaryDto>> GetAuditProgramSummaryAsync(int compId,  int auditNo)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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


        public async Task<IEnumerable<WorkspaceSummaryDto>> GetWorkspaceSummaryAsync(int compId, int auditNo)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
            Select ISNULL(cm.cmm_ID, a.SSW_ID) As PKID,IsNull(cm.cmm_Desc,'NA') As WorkpaperChecklist,SSW_WorkpaperNo As WorkpaperNo,
            SSW_WorkpaperRef As WorkpaperRef,SSW_Observation As Observation,SSW_Conclusion As Conclusion,SSW_ReviewerComments As ReviewerComments, 
            Case When a.SSW_TypeOfTest=1 then 'Inquiry' When a.SSW_TypeOfTest=2 then 'Observation' When a.SSW_TypeOfTest=3 then 'Examination' When 
            a.SSW_TypeOfTest=4 then 'Inspection' When a.SSW_TypeOfTest=5 then 'Substantive Testing' End TypeOfTest, Case When a.SSW_Status=1 
            then 'Open' When a.SSW_Status=2 then 'WIP' When a.SSW_Status=3 then 'Closed' End Status,SSW_AttachID As AttachID,
            IsNull(b.usr_FullName,'-') As CreatedBy,ISNULL(Convert(Varchar(10),SSW_CrOn,103),'-') As CreatedOn, IsNull(c.usr_FullName,'-') As ReviewedBy,
            ISNULL(Convert(Varchar(10),SSW_ReviewedOn,103),'-') As ReviewedOn From Content_Management_Master cm 
            Full Outer Join StandardAudit_ScheduleConduct_WorkPaper a On cm.cmm_ID = a.SSW_WPCheckListID And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId 
            Left Join sad_userdetails b on b.Usr_ID=a.SSW_CrBy Left Join sad_userdetails c on c.Usr_ID=a.SSW_ReviewedBy 
            Where (cm.cmm_Delflag = 'A' And cm.cmm_Category = 'WCM') Or a.SSW_ID Is Not Null And a.SSW_SA_ID=@AuditNo And a.SSW_CompID=@CompId Order by 
            CASE WHEN cm.cmm_Desc IS NULL THEN 1 ELSE 0 END, cm.cmm_ID ASC";


            var result = await connection.QueryAsync<WorkspaceSummaryDto>(query, new
            {
                CompId = compId,
                AuditNo = auditNo
            });

            return result;
        }

         
        public async Task<IEnumerable<CMADto>> GetCAMDetailsAsync(int compId, int auditNo)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string query = @"
            Select DENSE_RANK() OVER (ORDER BY SACAM_PKID) As SrNo,SACAM_PKID As DBpkId,SACAM_SSW_WorkpaperNo As WorkpaperNo,
            SACAM_SSW_WorkpaperRef As WorkpaperRef,SACAM_SSW_Observation As Observation,SACAM_SSW_Conclusion As Conclusion, 
            Case When a.SACAM_SSW_TypeOfTest=1 then 'Inquiry' When a.SACAM_SSW_TypeOfTest=2 then 'Observation' When a.SACAM_SSW_TypeOfTest=3 
            then 'Examination' When a.SACAM_SSW_TypeOfTest=4 then 'Inspection' When a.SACAM_SSW_TypeOfTest=5 then 'Substantive Testing' End TypeOfTest, 
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
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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
    }
}