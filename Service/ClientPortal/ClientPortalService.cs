using Dapper;
using TracePca.Interface.ClientPortal;
using TracePca.Dto.ClientPortal;
using TracePca.Utility;
using System.Threading.Tasks;
using static TracePca.Dto.ClientPortal.ClientPortalDto;
using Microsoft.AspNetCore.Connections;

namespace TracePca.Service.ClientPortal
{
    public class ClientPortalService : IClientPortalInterface
    {
        private readonly IDbConnectionFactory _db;

        public ClientPortalService(IDbConnectionFactory dbConnectionFactory)
        {
            _db = dbConnectionFactory;
        }

        public async Task<UserCustomerResponse> GetUserCustomerDetailsAsync(GetUserCustomerIdRequest request)
        {
            using (var connection = _db.CreateConnection())
            {
                string sql = @"
                    SELECT 
                        usr_CompanyId AS CompanyId,
                        Cust_Name AS CustomerName
                    FROM Sad_Userdetails
                    LEFT JOIN SAD_Customer_Master a 
                        ON a.Cust_Id = usr_CompanyId
                    WHERE Usr_CompId = @CompId
                      AND Usr_ID = @UserId";

                var result = await connection.QueryFirstOrDefaultAsync<UserCustomerResponse>(
                    sql,
                    new
                    {
                        CompId = request.CompanyId,
                        UserId = request.UserId
                    }
                );

                return result ?? new UserCustomerResponse();
            }
        }

        public async Task<List<DRLLogDto>> LoadDRLLogAsync(int companyId, int auditNo, int custId, int yearId)
        {
            using var connection = _db.CreateConnection();

            // STEP 1: Load beginning & ending DRL IDs
            var beginId = await connection.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(cmm_ID,0) 
              FROM Content_Management_Master
              WHERE cmm_Category='DRL' AND cmm_Desc='Beginning of the Audit'");

            var endId = await connection.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(cmm_ID,0) 
              FROM Content_Management_Master
              WHERE cmm_Category='DRL' AND cmm_Desc='Nearing completion of the Audit'");

            string requestedListIds = $"{beginId},{endId}";

            // STEP 2: Normal DRL entries
            string sqlMain = $@"
           SELECT *,
    (
        SELECT COUNT(*) 
        FROM StandardAudit_Audit_DRLLog_RemarksHistory x
        WHERE x.SAR_MASid = ADRL_ID
    ) AS TotalDocuments,
	(
        SELECT STRING_AGG(CAST(x.ATCH_ID AS VARCHAR(20)), ',')
        FROM EDT_attachments x
        WHERE x.ATCH_ID = ADRL_AttachID
    ) AS DocumentIDList
           FROM Audit_DRLLog
           LEFT JOIN AuditType_Checklist_Master ON ACM_ID = ADRL_FunID
           LEFT JOIN Content_Management_Master ON CMM_ID = ADRL_RequestedListID 
           LEFT JOIN Audit_Doc_Request_List ON DRL_DRLID = ADRL_RequestedTypeID
           LEFT JOIN EDT_attachments b ON ADRL_AttchDocId = b.ATCH_DOCID
           WHERE ADRL_CompID = @companyId 
             AND ADRL_YearID = @yearId
             AND ADRL_AuditNo = @auditNo
             AND ADRL_RequestedListID IN ({requestedListIds})
           ORDER BY ADRL_RequestedOn DESC";

            var mainList = (await connection.QueryAsync<dynamic>(sqlMain, new
            {
                companyId,
                yearId,
                auditNo
            })).ToList();

            // STEP 3: Latest DURING-AUDIT entry
            string sqlLatest = $@"
            WITH LatestData AS (
                SELECT *,
    (
        SELECT COUNT(*) 
        FROM StandardAudit_Audit_DRLLog_RemarksHistory x
        WHERE x.SAR_MASid = ADRL_ID
    ) AS TotalDocuments,
	(
        SELECT STRING_AGG(CAST(x.ATCH_ID AS VARCHAR(20)), ',')
        FROM EDT_attachments x
        WHERE x.ATCH_ID = ADRL_AttachID
    ) AS DocumentIDList,
                ROW_NUMBER() OVER (PARTITION BY ADRL_CompID, ADRL_AuditNo ORDER BY ADRL_RequestedOn DESC) AS RowNum
                FROM Audit_DRLLog
                LEFT JOIN Content_Management_Master ON CMM_ID = ADRL_RequestedListID
                LEFT JOIN Audit_Doc_Request_List ON DRL_DRLID = ADRL_RequestedTypeID
                LEFT JOIN EDT_attachments b ON ADRL_AttchDocId = b.ATCH_DOCID
                WHERE ADRL_CompID = {companyId}
                  AND ADRL_YearID = {yearId}
                  AND ADRL_AuditNo = {auditNo}
                  AND ADRL_RequestedListID NOT IN ({requestedListIds})
            )
            SELECT * FROM LatestData WHERE RowNum = 1;
        ";

            var latestList = (await connection.QueryAsync<dynamic>(sqlLatest)).ToList();

            // Merge both
            mainList.AddRange(latestList);

            // STEP 4: Convert into DTO list
            List<DRLLogDto> result = new();

            foreach (var row in mainList)
            {
                var dict = (IDictionary<string, object>)row;

                object GetObj(string key)
                {
                    if (!dict.ContainsKey(key)) return null;
                    var val = dict[key];
                    if (val == null || val == DBNull.Value) return null;
                    if (val is string s && string.IsNullOrWhiteSpace(s)) return null;
                    return val;
                }

                int GetInt(string key)
                {
                    var val = GetObj(key);
                    if (val == null) return 0;

                    if (int.TryParse(val.ToString(), out var num))
                        return num;

                    return 0;
                }

                string GetStr(string key)
                {
                    var val = GetObj(key);
                    return val?.ToString()?.Trim() ?? "";
                }

                DateTime? GetDate(string key)
                {
                    var val = GetObj(key);
                    if (val == null) return null;

                    // SQL DateTime (Jul 10 2025  6:19PM OR 2025-08-06 OR 09-07-2025 05:53:07 PM)
                    if (DateTime.TryParse(val.ToString(), out var dt))
                        return dt;

                    return null;
                }

                var dto = new DRLLogDto
                {
                    DRLID = GetInt("ADRL_ID"),
                    CheckPointID = GetInt("ADRL_FunID"),
                    DocumentRequestedListID = GetInt("cmm_ID"),

                    DocumentRequestedList = string.IsNullOrEmpty(GetStr("cmm_Desc"))
                        ? "Beginning of the Audit"
                        : GetStr("cmm_Desc"),

                    DocumentRequestedTypeID = GetInt("DRL_DRLID"),
                    DocumentRequestedType = GetStr("DRL_Name"),

                    RequestedOn = GetDate("ADRL_RequestedOn"),
                    TimlinetoResOn = GetDate("ADRL_TimlinetoResOn"),

                    EmailID = GetStr("ADRL_EmailID").Replace(".com", ".com "),

                    Comments = GetStr("ADRL_Comments"),
                    ReceivedComments = GetStr("ADRL_ReceivedComments"),
                    ReceivedOn = GetDate("ADRL_ReceivedOn"),

                    AttachID = GetInt("ADRL_AttachID"),
                    DocID = GetInt("ATCH_DOCID"),
                    ReportType = GetInt("ADRL_ReportType"),

                    Status = ConvertStatus(GetInt("ADRL_LogStatus")),
                    Count = GetInt("TotalDocuments"),
                    AllAtchIds = GetStr("DocumentIDList")

                };

                result.Add(dto);
            }



            return result;
        }

        public async Task<List<AttachmentResponseDto>> LoadAttachmentsAsync(AttachmentRequestDto req)
        {
            using var connection = _db.CreateConnection();

            var result = new List<AttachmentResponseDto>();
            var beginId = await connection.ExecuteScalarAsync<int>(
               @"SELECT ISNULL(cmm_ID,0) 
              FROM Content_Management_Master
              WHERE cmm_Category='DRL' AND cmm_Desc='Beginning of the Audit'");

            var endId = await connection.ExecuteScalarAsync<int>(
                @"SELECT ISNULL(cmm_ID,0) 
              FROM Content_Management_Master
              WHERE cmm_Category='DRL' AND cmm_Desc='Nearing completion of the Audit'");

            string requestedListIds = $"{beginId},{endId}";

            string condition = req.Type == 3
         ? $"AND ADRL_RequestedListID NOT IN ({requestedListIds})"
         : "";

            var Attchids = await connection.ExecuteScalarAsync<string>($@"
    SELECT STRING_AGG(val, ', ') AS Attchids
    FROM (
        SELECT DISTINCT 
            CASE 
                WHEN ADRL_AttachID IS NULL THEN 'NULL'
                ELSE CAST(ADRL_AttachID AS VARCHAR(10))
            END AS val
        FROM Audit_DRLLog
        WHERE ADRL_YearId =  {req.YearId}
          AND ADRL_AuditNo =  {req.AuditId}
          AND ADRL_Custid = {req.CustomerId}
          {condition}
    ) AS tmp;
");


            string sql = req.Type != 3
                ? @"SELECT Atch_ReportType, Atch_DocID, Atch_Id, ATCH_FNAME,
                        (SELECT TOP 1 SAR_Remarks 
                         FROM StandardAudit_Audit_DRLLog_RemarksHistory r 
                         WHERE r.SAR_AtthachDocId = a.Atch_DocID) AS SAR_Remarks,
                        (select cmm_Desc from content_Management_Master where cmm_Category = 'DRL' and Cmm_Id =a.ATCH_drlid ) cmm_Desc,
                        ATCH_drlid AS SAR_DRLID,
                        ATCH_EXT, ATCH_Desc, (select usr_FullName from SAD_Userdetails  where usr_Id = a.ATCH_CreatedBy) as ATCH_CreatedBy , ATCH_CREATEDON, ATCH_SIZE
                FROM edt_attachments a
                LEFT JOIN SAD_ReportTypeMaster ON RTM_Id = ATCH_ReportType
                WHERE ATCH_Status <> 'D'
                  AND Atch_Vstatus NOT IN ('AS','DS')
                  AND ATCH_ID = @AttachId
                ORDER BY ATCH_CREATEDON DESC"
                :
                @"SELECT Atch_ReportType, Atch_DocID, Atch_Id, ATCH_FNAME,
                        (SELECT TOP 1 SAR_Remarks 
                         FROM StandardAudit_Audit_DRLLog_RemarksHistory r 
                         WHERE r.SAR_AtthachDocId = a.Atch_DocID) AS SAR_Remarks,
                        cmm_Desc,
                        ATCH_drlid AS SAR_DRLID,
                        ATCH_EXT, ATCH_Desc, (select usr_FullName from SAD_Userdetails  where usr_Id = a.ATCH_CreatedBy) as ATCH_CreatedBy , ATCH_CREATEDON, ATCH_SIZE
                FROM edt_attachments a
                LEFT JOIN Content_Management_Master ON cmm_Id = ATCH_drlid
                WHERE ATCH_Status <> 'D'
                  AND Atch_Vstatus NOT IN ('AS','DS')
                  AND Atch_Id IN (SELECT value FROM STRING_SPLIT(@DuringAttachIds, ','))
                ORDER BY ATCH_CREATEDON DESC";

            var rows = (await connection.QueryAsync(sql, new
            {
                AttachId = req.AttachId,
                DuringAttachIds = Attchids
            })).ToList();

            int sr = 1;

            foreach (var r in rows)
            {
                result.Add(new AttachmentResponseDto
                {
                    SrNo = sr++,
                    AtchDocID = r.Atch_DocID,
                    AtchID = r.Atch_Id,
                    DrlId = r.SAR_DRLID ?? 0,
                    FileName = $"{r.ATCH_FNAME}.{r.ATCH_EXT}",
                    Remarks = r.SAR_Remarks ?? "",
                    DRLName = r.cmm_Desc ?? "",
                    Description = r.ATCH_Desc ?? "",
                    ReportType = r.Atch_ReportType?.ToString() ?? "",
                    CreatedBy = r.ATCH_CreatedBy?.ToString() ?? "",
                    CreatedOn = Convert.ToDateTime(r.ATCH_CREATEDON).ToString("dd-MM-yyyy HH:mm"),
                    FileSize = $"{(Convert.ToDecimal(r.ATCH_SIZE) / 1024):0.00} KB",
                    Extension = r.ATCH_EXT
                });
            }

            return result;
        }
        private string ConvertStatus(int? status)
        {
            return status switch
            {
                1 => "Outstanding",
                2 => "Acceptable",
                3 => "Partially",
                4 => "No",
                _ => ""
            };
        }
    }
}

