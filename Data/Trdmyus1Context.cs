using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TracePca.Models;

namespace TracePca.Data;

public partial class Trdmyus1Context : DbContext
{
    private readonly string _connectionString;
    public Trdmyus1Context(string connectionString)
    {
        _connectionString = connectionString; // Initialize in constructor
    }


    public Trdmyus1Context(DbContextOptions<Trdmyus1Context> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    public virtual DbSet<AccAccountingpoliciesReportGeneration> AccAccountingpoliciesReportGenerations { get; set; }

    public virtual DbSet<AccAssetDepItact> AccAssetDepItacts { get; set; }

    public virtual DbSet<AccAssetDepreciation> AccAssetDepreciations { get; set; }

    public virtual DbSet<AccAssetDepreciationPrev> AccAssetDepreciationPrevs { get; set; }

    public virtual DbSet<AccAssetLocationSetup> AccAssetLocationSetups { get; set; }

    public virtual DbSet<AccAssetMaster> AccAssetMasters { get; set; }

    public virtual DbSet<AccCashflow> AccCashflows { get; set; }

    public virtual DbSet<AccChangesInventory> AccChangesInventories { get; set; }

    public virtual DbSet<AccClosingstockItem> AccClosingstockItems { get; set; }

    public virtual DbSet<AccCoaSetting> AccCoaSettings { get; set; }

    public virtual DbSet<AccFixedAssetAdditionDel> AccFixedAssetAdditionDels { get; set; }

    public virtual DbSet<AccFixedAssetAdditionDetail> AccFixedAssetAdditionDetails { get; set; }

    public virtual DbSet<AccFixedAssetDeletion> AccFixedAssetDeletions { get; set; }

    public virtual DbSet<AccFixedAssetMaster> AccFixedAssetMasters { get; set; }

    public virtual DbSet<AccFixedAssetsTransaction> AccFixedAssetsTransactions { get; set; }

    public virtual DbSet<AccGeneralMaster> AccGeneralMasters { get; set; }

    public virtual DbSet<AccGeneralMaster1> AccGeneralMaster1s { get; set; }

    public virtual DbSet<AccGroupingAlias> AccGroupingAliases { get; set; }

    public virtual DbSet<AccJeMaster> AccJeMasters { get; set; }

    public virtual DbSet<AccJeMasterHistory> AccJeMasterHistories { get; set; }

    public virtual DbSet<AccJetransactionsDetail> AccJetransactionsDetails { get; set; }

    public virtual DbSet<AccLedgerHeadingFactor> AccLedgerHeadingFactors { get; set; }

    public virtual DbSet<AccLedgerTransactionsDetail> AccLedgerTransactionsDetails { get; set; }

    public virtual DbSet<AccLtMaster> AccLtMasters { get; set; }

    public virtual DbSet<AccLtMasterHistory> AccLtMasterHistories { get; set; }

    public virtual DbSet<AccOpeningBalance> AccOpeningBalances { get; set; }

    public virtual DbSet<AccOpeningBalance1> AccOpeningBalance1s { get; set; }

    public virtual DbSet<AccPartnershipFirm> AccPartnershipFirms { get; set; }

    public virtual DbSet<AccProfitAndLossAmount> AccProfitAndLossAmounts { get; set; }

    public virtual DbSet<AccSaptransactionsDetail> AccSaptransactionsDetails { get; set; }

    public virtual DbSet<AccScheduleHeading> AccScheduleHeadings { get; set; }

    public virtual DbSet<AccScheduleItem> AccScheduleItems { get; set; }

    public virtual DbSet<AccScheduleSubHeading> AccScheduleSubHeadings { get; set; }

    public virtual DbSet<AccScheduleSubItem> AccScheduleSubItems { get; set; }

    public virtual DbSet<AccScheduleTemplate> AccScheduleTemplates { get; set; }

    public virtual DbSet<AccSeperateSchedule> AccSeperateSchedules { get; set; }

    public virtual DbSet<AccSubHeadingLedgerDesc> AccSubHeadingLedgerDescs { get; set; }

    public virtual DbSet<AccSubHeadingNoteDesc> AccSubHeadingNoteDescs { get; set; }

    public virtual DbSet<AccTradeUpload> AccTradeUploads { get; set; }

    public virtual DbSet<AccTrailBalanceCustomerUpload> AccTrailBalanceCustomerUploads { get; set; }

    public virtual DbSet<AccTrailBalanceUpload> AccTrailBalanceUploads { get; set; }

    public virtual DbSet<AccTrailBalanceUploadDetail> AccTrailBalanceUploadDetails { get; set; }

    public virtual DbSet<AccTransactionsDetail> AccTransactionsDetails { get; set; }

    public virtual DbSet<AccVoucherSetting> AccVoucherSettings { get; set; }

    public virtual DbSet<AccYearMaster> AccYearMasters { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AdoBatch> AdoBatches { get; set; }

    public virtual DbSet<Ajax1> Ajax1s { get; set; }

    public virtual DbSet<AuditAnnualPlan> AuditAnnualPlans { get; set; }

    public virtual DbSet<AuditApmAssignmentDetail> AuditApmAssignmentDetails { get; set; }

    public virtual DbSet<AuditApmChecksMatrix> AuditApmChecksMatrices { get; set; }

    public virtual DbSet<AuditApmDetail> AuditApmDetails { get; set; }

    public virtual DbSet<AuditAra> AuditAras { get; set; }

    public virtual DbSet<AuditAraDetail> AuditAraDetails { get; set; }

    public virtual DbSet<AuditAssignmentEmpSubTask> AuditAssignmentEmpSubTasks { get; set; }

    public virtual DbSet<AuditAssignmentInvoice> AuditAssignmentInvoices { get; set; }

    public virtual DbSet<AuditAssignmentInvoiceDetail> AuditAssignmentInvoiceDetails { get; set; }

    public virtual DbSet<AuditAssignmentSchedule> AuditAssignmentSchedules { get; set; }

    public virtual DbSet<AuditAssignmentSubTask> AuditAssignmentSubTasks { get; set; }

    public virtual DbSet<AuditAssignmentSubTaskMaster> AuditAssignmentSubTaskMasters { get; set; }

    public virtual DbSet<AuditAssignmentUserLog> AuditAssignmentUserLogs { get; set; }

    public virtual DbSet<AuditCompletionSubPointMaster> AuditCompletionSubPointMasters { get; set; }

    public virtual DbSet<AuditCostBudgetDetail> AuditCostBudgetDetails { get; set; }

    public virtual DbSet<AuditCostSheetDetail> AuditCostSheetDetails { get; set; }

    public virtual DbSet<AuditDocRequestList> AuditDocRequestLists { get; set; }

    public virtual DbSet<AuditDrllog> AuditDrllogs { get; set; }

    public virtual DbSet<AuditExcelUpload> AuditExcelUploads { get; set; }

    public virtual DbSet<AuditExecutiveSummary> AuditExecutiveSummaries { get; set; }

    public virtual DbSet<AuditIssueTrackerDetail> AuditIssueTrackerDetails { get; set; }

    public virtual DbSet<AuditIssueTrackerHistory> AuditIssueTrackerHistories { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<AuditLogDetail> AuditLogDetails { get; set; }

    public virtual DbSet<AuditLogFormOperation> AuditLogFormOperations { get; set; }

    public virtual DbSet<AuditLogOperation> AuditLogOperations { get; set; }

    public virtual DbSet<AuditPlanSignOff> AuditPlanSignOffs { get; set; }

    public virtual DbSet<AuditSchedule> AuditSchedules { get; set; }

    public virtual DbSet<AuditSignOff> AuditSignOffs { get; set; }

    public virtual DbSet<AuditTimeCostBudgetDetail> AuditTimeCostBudgetDetails { get; set; }

    public virtual DbSet<AuditTimeCostBudgetMaster> AuditTimeCostBudgetMasters { get; set; }

    public virtual DbSet<AuditTimeSheet> AuditTimeSheets { get; set; }

    public virtual DbSet<AuditTimesheet1> AuditTimesheet1s { get; set; }

    public virtual DbSet<AuditTypeChecklistMaster> AuditTypeChecklistMasters { get; set; }

    public virtual DbSet<AuditWorkPaper> AuditWorkPapers { get; set; }

    public virtual DbSet<AuditWorkPaperHistory> AuditWorkPaperHistories { get; set; }

    public virtual DbSet<BatchScanTable> BatchScanTables { get; set; }

    public virtual DbSet<CaiqCategoryDescription> CaiqCategoryDescriptions { get; set; }

    public virtual DbSet<CaiqDescriptor> CaiqDescriptors { get; set; }

    public virtual DbSet<CaiqFactor> CaiqFactors { get; set; }

    public virtual DbSet<CaiqFactorCategory> CaiqFactorCategories { get; set; }

    public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }

    public virtual DbSet<CmacheckMaster> CmacheckMasters { get; set; }

    public virtual DbSet<CmacheckMasterLog> CmacheckMasterLogs { get; set; }

    public virtual DbSet<Cmarating> Cmaratings { get; set; }

    public virtual DbSet<CmaratingCoreProcess> CmaratingCoreProcesses { get; set; }

    public virtual DbSet<CmaratingCoreProcessLog> CmaratingCoreProcessLogs { get; set; }

    public virtual DbSet<CmaratingLog> CmaratingLogs { get; set; }

    public virtual DbSet<CmaratingSupportProcess> CmaratingSupportProcesses { get; set; }

    public virtual DbSet<CmaratingSupportProcessLog> CmaratingSupportProcessLogs { get; set; }

    public virtual DbSet<CompanyLogoSetting> CompanyLogoSettings { get; set; }

    public virtual DbSet<ComplianceChecklist> ComplianceChecklists { get; set; }

    public virtual DbSet<ComplianceChecklistMa> ComplianceChecklistMas { get; set; }

    public virtual DbSet<ComplianceIssueTrackerDetail> ComplianceIssueTrackerDetails { get; set; }

    public virtual DbSet<ComplianceIssueTrackerDetailsHistory> ComplianceIssueTrackerDetailsHistories { get; set; }

    public virtual DbSet<CompliancePlan> CompliancePlans { get; set; }

    public virtual DbSet<CompliancePlanHistory> CompliancePlanHistories { get; set; }

    public virtual DbSet<ContentManagementMaster> ContentManagementMasters { get; set; }

    public virtual DbSet<ContentManagementMasterLog> ContentManagementMasterLogs { get; set; }

    public virtual DbSet<CrpaAuditAssest> CrpaAuditAssests { get; set; }

    public virtual DbSet<CrpaChecklistAuditAssest> CrpaChecklistAuditAssests { get; set; }

    public virtual DbSet<CrpaProcess> CrpaProcesses { get; set; }

    public virtual DbSet<CrpaRating> CrpaRatings { get; set; }

    public virtual DbSet<CrpaSection> CrpaSections { get; set; }

    public virtual DbSet<CrpaSubProcess> CrpaSubProcesses { get; set; }

    public virtual DbSet<CrpaSubSection> CrpaSubSections { get; set; }

    public virtual DbSet<CrpaValueRating> CrpaValueRatings { get; set; }

    public virtual DbSet<CustFontstyle> CustFontstyles { get; set; }

    public virtual DbSet<CustomerCoa> CustomerCoas { get; set; }

    public virtual DbSet<CustomerGlLinkageMaster> CustomerGlLinkageMasters { get; set; }

    public virtual DbSet<DocReviewremark> DocReviewremarks { get; set; }

    public virtual DbSet<DocReviewremarksHistory> DocReviewremarksHistories { get; set; }

    public virtual DbSet<DocumentTray> DocumentTrays { get; set; }

    public virtual DbSet<EdtAnnotationDetail> EdtAnnotationDetails { get; set; }

    public virtual DbSet<EdtAttachment> EdtAttachments { get; set; }

    public virtual DbSet<EdtBigdatum> EdtBigdata { get; set; }

    public virtual DbSet<EdtCabinet> EdtCabinets { get; set; }

    public virtual DbSet<EdtCabinet1> EdtCabinet1s { get; set; }

    public virtual DbSet<EdtCabinet2> EdtCabinet2s { get; set; }

    public virtual DbSet<EdtCabinet23082024> EdtCabinet23082024s { get; set; }

    public virtual DbSet<EdtCabinetPermission> EdtCabinetPermissions { get; set; }

    public virtual DbSet<EdtCollate> EdtCollates { get; set; }

    public virtual DbSet<EdtCollateLog> EdtCollateLogs { get; set; }

    public virtual DbSet<EdtCollatedoc> EdtCollatedocs { get; set; }

    public virtual DbSet<EdtDescType> EdtDescTypes { get; set; }

    public virtual DbSet<EdtDescriptio> EdtDescriptios { get; set; }

    public virtual DbSet<EdtDescriptiosLog> EdtDescriptiosLogs { get; set; }

    public virtual DbSet<EdtDescriptor> EdtDescriptors { get; set; }

    public virtual DbSet<EdtDescriptorLog> EdtDescriptorLogs { get; set; }

    public virtual DbSet<EdtDoctypeLink> EdtDoctypeLinks { get; set; }

    public virtual DbSet<EdtDoctypeLinkLog> EdtDoctypeLinkLogs { get; set; }

    public virtual DbSet<EdtDoctypePermission> EdtDoctypePermissions { get; set; }

    public virtual DbSet<EdtDoctypePermissionLog> EdtDoctypePermissionLogs { get; set; }

    public virtual DbSet<EdtDocumentType> EdtDocumentTypes { get; set; }

    public virtual DbSet<EdtDocumentTypeLog> EdtDocumentTypeLogs { get; set; }

    public virtual DbSet<EdtFolder> EdtFolders { get; set; }

    public virtual DbSet<EdtFolder1> EdtFolder1s { get; set; }

    public virtual DbSet<EdtFolder2> EdtFolder2s { get; set; }

    public virtual DbSet<EdtFolderPermission> EdtFolderPermissions { get; set; }

    public virtual DbSet<EdtFolderRight> EdtFolderRights { get; set; }

    public virtual DbSet<EdtImageSetting> EdtImageSettings { get; set; }

    public virtual DbSet<EdtNote> EdtNotes { get; set; }

    public virtual DbSet<EdtOutlookAttach> EdtOutlookAttaches { get; set; }

    public virtual DbSet<EdtPage> EdtPages { get; set; }

    public virtual DbSet<EdtPage03082024> EdtPage03082024s { get; set; }

    public virtual DbSet<EdtPage1> EdtPage1s { get; set; }

    public virtual DbSet<EdtPage30072024> EdtPage30072024s { get; set; }

    public virtual DbSet<EdtPage31072024> EdtPage31072024s { get; set; }

    public virtual DbSet<EdtPageDetail> EdtPageDetails { get; set; }

    public virtual DbSet<EdtPageLog> EdtPageLogs { get; set; }

    public virtual DbSet<EdtPageViewAndDownloadlog> EdtPageViewAndDownloadlogs { get; set; }

    public virtual DbSet<EdtScanDocDetail> EdtScanDocDetails { get; set; }

    public virtual DbSet<EdtSetting> EdtSettings { get; set; }

    public virtual DbSet<EdtSettingsLog> EdtSettingsLogs { get; set; }

    public virtual DbSet<ExcelUploadStructure> ExcelUploadStructures { get; set; }

    public virtual DbSet<FinancialAddAssign> FinancialAddAssigns { get; set; }

    public virtual DbSet<FlaLeaveDetail> FlaLeaveDetails { get; set; }

    public virtual DbSet<GenProductDetail> GenProductDetails { get; set; }

    public virtual DbSet<GraceEmailSentDetail> GraceEmailSentDetails { get; set; }

    public virtual DbSet<GraceExcelUpload> GraceExcelUploads { get; set; }

    public virtual DbSet<GraceGrossControlScore> GraceGrossControlScores { get; set; }

    public virtual DbSet<GraceGrossRiskScore> GraceGrossRiskScores { get; set; }

    public virtual DbSet<GraceOverallBranchRatingDetail> GraceOverallBranchRatingDetails { get; set; }

    public virtual DbSet<GraceOverallFunctionRatingDetail> GraceOverallFunctionRatingDetails { get; set; }

    public virtual DbSet<GraceRiskControlMatrix> GraceRiskControlMatrices { get; set; }

    public virtual DbSet<HolidayMaster> HolidayMasters { get; set; }

    public virtual DbSet<HolidayMasterLog> HolidayMasterLogs { get; set; }

    public virtual DbSet<InsConfig> InsConfigs { get; set; }

    public virtual DbSet<InsConfigLog> InsConfigLogs { get; set; }

    public virtual DbSet<IntacctDdlItemText> IntacctDdlItemTexts { get; set; }

    public virtual DbSet<ItreturnsClient> ItreturnsClients { get; set; }

    public virtual DbSet<ItreturnsFilingDetail> ItreturnsFilingDetails { get; set; }

    public virtual DbSet<LoeAdditionalFee> LoeAdditionalFees { get; set; }

    public virtual DbSet<LoeReAmbersment> LoeReAmbersments { get; set; }

    public virtual DbSet<LoeResource> LoeResources { get; set; }

    public virtual DbSet<LoeTemplate> LoeTemplates { get; set; }

    public virtual DbSet<LoeTemplateDetail> LoeTemplateDetails { get; set; }

    public virtual DbSet<MmcsplDbAccess> MmcsplDbAccesses { get; set; }

    public virtual DbSet<MstChecksMaster> MstChecksMasters { get; set; }

    public virtual DbSet<MstChecksMasterLog> MstChecksMasterLogs { get; set; }

    public virtual DbSet<MstControlLibrary> MstControlLibraries { get; set; }

    public virtual DbSet<MstControlLibraryLog> MstControlLibraryLogs { get; set; }

    public virtual DbSet<MstEntityMaster> MstEntityMasters { get; set; }

    public virtual DbSet<MstEntityMasterLog> MstEntityMasterLogs { get; set; }

    public virtual DbSet<MstEntityUsrMap> MstEntityUsrMaps { get; set; }

    public virtual DbSet<MstInherentRiskMaster> MstInherentRiskMasters { get; set; }

    public virtual DbSet<MstMappingMaster> MstMappingMasters { get; set; }

    public virtual DbSet<MstPasswordSetting> MstPasswordSettings { get; set; }

    public virtual DbSet<MstPasswordSettingLog> MstPasswordSettingLogs { get; set; }

    public virtual DbSet<MstProcessMaster> MstProcessMasters { get; set; }

    public virtual DbSet<MstProcessMasterLog> MstProcessMasterLogs { get; set; }

    public virtual DbSet<MstRiskColorMatrix> MstRiskColorMatrices { get; set; }

    public virtual DbSet<MstRiskLibrary> MstRiskLibraries { get; set; }

    public virtual DbSet<MstRiskLibraryLog> MstRiskLibraryLogs { get; set; }

    public virtual DbSet<MstSubentityMaster> MstSubentityMasters { get; set; }

    public virtual DbSet<MstSubentityMasterLog> MstSubentityMasterLogs { get; set; }

    public virtual DbSet<MstSubprocessMaster> MstSubprocessMasters { get; set; }

    public virtual DbSet<MstSubprocessMasterLog> MstSubprocessMasterLogs { get; set; }

    public virtual DbSet<NotificationMaster> NotificationMasters { get; set; }

    public virtual DbSet<QaAssessment> QaAssessments { get; set; }

    public virtual DbSet<QaWorkPaper> QaWorkPapers { get; set; }

    public virtual DbSet<QaaChecksMatrix> QaaChecksMatrices { get; set; }

    public virtual DbSet<RiskBrrchecklistDetail> RiskBrrchecklistDetails { get; set; }

    public virtual DbSet<RiskBrrchecklistMa> RiskBrrchecklistMas { get; set; }

    public virtual DbSet<RiskBrrissueTracker> RiskBrrissueTrackers { get; set; }

    public virtual DbSet<RiskBrrissueTrackerHistory> RiskBrrissueTrackerHistories { get; set; }

    public virtual DbSet<RiskBrrplanning> RiskBrrplannings { get; set; }

    public virtual DbSet<RiskBrrreport> RiskBrrreports { get; set; }

    public virtual DbSet<RiskBrrschedule> RiskBrrschedules { get; set; }

    public virtual DbSet<RiskCheckListMaster> RiskCheckListMasters { get; set; }

    public virtual DbSet<RiskCheckListMaster1> RiskCheckListMaster1s { get; set; }

    public virtual DbSet<RiskColorMatrix> RiskColorMatrices { get; set; }

    public virtual DbSet<RiskGeneralMaster> RiskGeneralMasters { get; set; }

    public virtual DbSet<RiskGeneralMasterLog> RiskGeneralMasterLogs { get; set; }

    public virtual DbSet<RiskIssueTracker> RiskIssueTrackers { get; set; }

    public virtual DbSet<RiskIssueTrackerHistory> RiskIssueTrackerHistories { get; set; }

    public virtual DbSet<RiskKccPlanningSchecdulingDetail> RiskKccPlanningSchecdulingDetails { get; set; }

    public virtual DbSet<RiskKir> RiskKirs { get; set; }

    public virtual DbSet<RiskKri> RiskKris { get; set; }

    public virtual DbSet<RiskRa> RiskRas { get; set; }

    public virtual DbSet<RiskRaActionPlanHistory> RiskRaActionPlanHistories { get; set; }

    public virtual DbSet<RiskRaConductHistory> RiskRaConductHistories { get; set; }

    public virtual DbSet<RiskRaDetail> RiskRaDetails { get; set; }

    public virtual DbSet<RiskRcsa> RiskRcsas { get; set; }

    public virtual DbSet<RiskRcsaActionPlanHistory> RiskRcsaActionPlanHistories { get; set; }

    public virtual DbSet<RiskRcsaAssignHistory> RiskRcsaAssignHistories { get; set; }

    public virtual DbSet<RiskRcsaDetail> RiskRcsaDetails { get; set; }

    public virtual DbSet<RiskRrfPlanningSchecdulingDetail> RiskRrfPlanningSchecdulingDetails { get; set; }

    public virtual DbSet<SadColorMaster> SadColorMasters { get; set; }

    public virtual DbSet<SadCompanyMaster> SadCompanyMasters { get; set; }

    public virtual DbSet<SadComplianceDetail> SadComplianceDetails { get; set; }

    public virtual DbSet<SadConfigSetting> SadConfigSettings { get; set; }

    public virtual DbSet<SadConfigSettingsLog> SadConfigSettingsLogs { get; set; }

    public virtual DbSet<SadCurrencyMaster> SadCurrencyMasters { get; set; }

    public virtual DbSet<SadCustAccountingTemplate> SadCustAccountingTemplates { get; set; }

    public virtual DbSet<SadCustLocation> SadCustLocations { get; set; }

    public virtual DbSet<SadCustLoe> SadCustLoes { get; set; }

    public virtual DbSet<SadCustomerDetail> SadCustomerDetails { get; set; }

    public virtual DbSet<SadCustomerLegalTransfer> SadCustomerLegalTransfers { get; set; }

    public virtual DbSet<SadCustomerMaster> SadCustomerMasters { get; set; }

    public virtual DbSet<SadCustomerMaster1> SadCustomerMaster1s { get; set; }

    public virtual DbSet<SadCustomerMaster2> SadCustomerMaster2s { get; set; }

    public virtual DbSet<SadCustomerMaster23092024> SadCustomerMaster23092024s { get; set; }

    public virtual DbSet<SadEmpCategoryCharge> SadEmpCategoryCharges { get; set; }

    public virtual DbSet<SadFinalisationReportContent> SadFinalisationReportContents { get; set; }

    public virtual DbSet<SadFinalisationReportTemplate> SadFinalisationReportTemplates { get; set; }

    public virtual DbSet<SadGeneralBranchDetail> SadGeneralBranchDetails { get; set; }

    public virtual DbSet<SadGrpDesgnGeneralMasterLog> SadGrpDesgnGeneralMasterLogs { get; set; }

    public virtual DbSet<SadGrpOrLvlGeneralMaster> SadGrpOrLvlGeneralMasters { get; set; }

    public virtual DbSet<SadGrpOrLvlGeneralMasterLog> SadGrpOrLvlGeneralMasterLogs { get; set; }

    public virtual DbSet<SadGrpdesgnGeneralMaster> SadGrpdesgnGeneralMasters { get; set; }

    public virtual DbSet<SadGrplvlMember> SadGrplvlMembers { get; set; }

    public virtual DbSet<SadIssueKnowledgeBaseMaster> SadIssueKnowledgeBaseMasters { get; set; }

    public virtual DbSet<SadKnowledgeMaster> SadKnowledgeMasters { get; set; }

    public virtual DbSet<SadLevelsGeneralMaster> SadLevelsGeneralMasters { get; set; }

    public virtual DbSet<SadMembershipDetail> SadMembershipDetails { get; set; }

    public virtual DbSet<SadModOperation> SadModOperations { get; set; }

    public virtual DbSet<SadModule> SadModules { get; set; }

    public virtual DbSet<SadOrgStructure> SadOrgStructures { get; set; }

    public virtual DbSet<SadOrgStructureLog> SadOrgStructureLogs { get; set; }

    public virtual DbSet<SadReportContentMaster> SadReportContentMasters { get; set; }

    public virtual DbSet<SadReportGeneration> SadReportGenerations { get; set; }

    public virtual DbSet<SadReportTypeMaster> SadReportTypeMasters { get; set; }

    public virtual DbSet<SadStatutoryDirectorDetail> SadStatutoryDirectorDetails { get; set; }

    public virtual DbSet<SadStatutoryPartnerDetail> SadStatutoryPartnerDetails { get; set; }

    public virtual DbSet<SadSupplierMaster> SadSupplierMasters { get; set; }

    public virtual DbSet<SadUserDetail> SadUserDetails { get; set; }

    public virtual DbSet<SadUserEmpAcademicProgress> SadUserEmpAcademicProgresses { get; set; }

    public virtual DbSet<SadUserEmpAddress> SadUserEmpAddresses { get; set; }

    public virtual DbSet<SadUserEmpAssessment> SadUserEmpAssessments { get; set; }

    public virtual DbSet<SadUserEmpAssetsLoan> SadUserEmpAssetsLoans { get; set; }

    public virtual DbSet<SadUserEmpCourse> SadUserEmpCourses { get; set; }

    public virtual DbSet<SadUserEmpMembershipDetail> SadUserEmpMembershipDetails { get; set; }

    public virtual DbSet<SadUserEmpParticularsofArticle> SadUserEmpParticularsofArticles { get; set; }

    public virtual DbSet<SadUserEmpProfExperiance> SadUserEmpProfExperiances { get; set; }

    public virtual DbSet<SadUserEmpQualification> SadUserEmpQualifications { get; set; }

    public virtual DbSet<SadUserEmpSpecialMention> SadUserEmpSpecialMentions { get; set; }

    public virtual DbSet<SadUserEmpTransferFirm> SadUserEmpTransferFirms { get; set; }

    public virtual DbSet<SadUserPasswordHistory> SadUserPasswordHistories { get; set; }

    public virtual DbSet<SadUserdetailsLog> SadUserdetailsLogs { get; set; }

    public virtual DbSet<SadUsersInOtherDept> SadUsersInOtherDepts { get; set; }

    public virtual DbSet<SadUsrOrGrpPermission> SadUsrOrGrpPermissions { get; set; }

    public virtual DbSet<SadUsrOrGrpPermissionDgo> SadUsrOrGrpPermissionDgos { get; set; }

    public virtual DbSet<SadUsrOrGrpPermissionLog> SadUsrOrGrpPermissionLogs { get; set; }

    public virtual DbSet<SampleSelection> SampleSelections { get; set; }

    public virtual DbSet<SampleTable> SampleTables { get; set; }

    public virtual DbSet<ScheduleLinkageMaster> ScheduleLinkageMasters { get; set; }

    public virtual DbSet<ScheduleNoteDesc> ScheduleNoteDescs { get; set; }

    public virtual DbSet<ScheduleNoteFirst> ScheduleNoteFirsts { get; set; }

    public virtual DbSet<ScheduleNoteFourth> ScheduleNoteFourths { get; set; }

    public virtual DbSet<ScheduleNoteSecond> ScheduleNoteSeconds { get; set; }

    public virtual DbSet<ScheduleNoteThird> ScheduleNoteThirds { get; set; }

    public virtual DbSet<StandardAuditAuditCompletion> StandardAuditAuditCompletions { get; set; }

    public virtual DbSet<StandardAuditAuditDrllogRemarksHistory> StandardAuditAuditDrllogRemarksHistories { get; set; }

    public virtual DbSet<StandardAuditAuditSummaryIfc> StandardAuditAuditSummaryIfcs { get; set; }

    public virtual DbSet<StandardAuditAuditSummaryIfcdetail> StandardAuditAuditSummaryIfcdetails { get; set; }

    public virtual DbSet<StandardAuditAuditSummaryKamdetail> StandardAuditAuditSummaryKamdetails { get; set; }

    public virtual DbSet<StandardAuditAuditSummaryMrdetail> StandardAuditAuditSummaryMrdetails { get; set; }

    public virtual DbSet<StandardAuditChecklistDetail> StandardAuditChecklistDetails { get; set; }

    public virtual DbSet<StandardAuditConductAuditRemarksHistory> StandardAuditConductAuditRemarksHistories { get; set; }

    public virtual DbSet<StandardAuditReviewLedgerObservation> StandardAuditReviewLedgerObservations { get; set; }

    public virtual DbSet<StandardAuditSchedule> StandardAuditSchedules { get; set; }

    public virtual DbSet<StandardAuditScheduleCheckPointList> StandardAuditScheduleCheckPointLists { get; set; }

    public virtual DbSet<StandardAuditScheduleConductWorkPaper> StandardAuditScheduleConductWorkPapers { get; set; }

    public virtual DbSet<StandardAuditScheduleInterval> StandardAuditScheduleIntervals { get; set; }

    public virtual DbSet<StandardAuditScheduleObservation> StandardAuditScheduleObservations { get; set; }

    public virtual DbSet<TraceCabinet> TraceCabinets { get; set; }

    public virtual DbSet<TraceColorMaster> TraceColorMasters { get; set; }

    public virtual DbSet<TraceCompanyBranchDetail> TraceCompanyBranchDetails { get; set; }

    public virtual DbSet<TraceCompanyDetail> TraceCompanyDetails { get; set; }

    public virtual DbSet<TraceDocument> TraceDocuments { get; set; }

    public virtual DbSet<TraceErrorReplacement> TraceErrorReplacements { get; set; }

    public virtual DbSet<TraceFolder> TraceFolders { get; set; }

    public virtual DbSet<TraceReportMaster> TraceReportMasters { get; set; }

    public virtual DbSet<TraceReportMaster1> TraceReportMaster1s { get; set; }

    public virtual DbSet<TraceSubCabinet> TraceSubCabinets { get; set; }

    public virtual DbSet<Upload> Uploads { get; set; }

    public virtual DbSet<UploadedSharedDocument> UploadedSharedDocuments { get; set; }

    public virtual DbSet<UploadedSharedDocumentsLog> UploadedSharedDocumentsLogs { get; set; }

    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

    public virtual DbSet<ViewCabPermission> ViewCabPermissions { get; set; }

    public virtual DbSet<ViewDtpermission> ViewDtpermissions { get; set; }

    public virtual DbSet<ViewFolPermission> ViewFolPermissions { get; set; }

    public virtual DbSet<ViewFolcab> ViewFolcabs { get; set; }

    public virtual DbSet<WfInwardMaster> WfInwardMasters { get; set; }

    public virtual DbSet<WfInwardMastersHistory> WfInwardMastersHistories { get; set; }

    public virtual DbSet<WfOutwardMaster> WfOutwardMasters { get; set; }

    public virtual DbSet<YearMaster> YearMasters { get; set; }

    public virtual DbSet<YearMasterLog> YearMasterLogs { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccAccountingpoliciesReportGeneration>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_AccountingpoliciesReportGeneration");

            entity.Property(e => e.AapRBranchid).HasColumnName("AApR_Branchid");
            entity.Property(e => e.AapRCompid).HasColumnName("AApR_Compid");
            entity.Property(e => e.AapRCrBy).HasColumnName("AApR_CrBy");
            entity.Property(e => e.AapRCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AApR_CrOn");
            entity.Property(e => e.AapRCustomerId).HasColumnName("AApR_CustomerId");
            entity.Property(e => e.AapRDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AApR_Description");
            entity.Property(e => e.AapRFinancialYear).HasColumnName("AApR_FinancialYear");
            entity.Property(e => e.AapRHeading)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AApR_Heading");
            entity.Property(e => e.AapRId).HasColumnName("AApR_Id");
            entity.Property(e => e.AapRIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AApR_IPAddress");
            entity.Property(e => e.AapRReport).HasColumnName("AApR_Report");
            entity.Property(e => e.AapRReportType).HasColumnName("AApR_ReportType");
            entity.Property(e => e.AapRUpdatedBy).HasColumnName("AApR_UpdatedBy");
            entity.Property(e => e.AapRUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AApR_UpdatedOn");
            entity.Property(e => e.AapRYearId).HasColumnName("AApR_YearId");
        });

        modelBuilder.Entity<AccAssetDepItact>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_AssetDepITAct");

            entity.Property(e => e.AditactAftQtrAmount)
                .HasColumnType("money")
                .HasColumnName("ADITAct_AftQtrAmount");
            entity.Property(e => e.AditactAftQtrDep)
                .HasColumnType("money")
                .HasColumnName("ADITAct_AftQtrDep");
            entity.Property(e => e.AditactApprovedBy).HasColumnName("ADITAct_ApprovedBy");
            entity.Property(e => e.AditactApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADITAct_ApprovedOn");
            entity.Property(e => e.AditactAssetClassId).HasColumnName("ADITAct_AssetClassID");
            entity.Property(e => e.AditactBfrQtrAmount)
                .HasColumnType("money")
                .HasColumnName("ADITAct_BfrQtrAmount");
            entity.Property(e => e.AditactBfrQtrDep)
                .HasColumnType("money")
                .HasColumnName("ADITAct_BfrQtrDep");
            entity.Property(e => e.AditactCompId).HasColumnName("ADITAct_CompID");
            entity.Property(e => e.AditactCreatedBy).HasColumnName("ADITAct_CreatedBy");
            entity.Property(e => e.AditactCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADITAct_CreatedOn");
            entity.Property(e => e.AditactCustId).HasColumnName("ADITAct_CustId");
            entity.Property(e => e.AditactDelAmount)
                .HasColumnType("money")
                .HasColumnName("ADITAct_DelAmount");
            entity.Property(e => e.AditactDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADITAct_DelFlag");
            entity.Property(e => e.AditactDepreciationforFy)
                .HasColumnType("money")
                .HasColumnName("ADITAct_DepreciationforFY");
            entity.Property(e => e.AditactId).HasColumnName("ADITAct_ID");
            entity.Property(e => e.AditactInitAmt)
                .HasColumnType("money")
                .HasColumnName("ADITAct_InitAmt");
            entity.Property(e => e.AditactIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ADITAct_IPAddress");
            entity.Property(e => e.AditactOpbforYr)
                .HasColumnType("money")
                .HasColumnName("ADITAct_OPBForYR");
            entity.Property(e => e.AditactOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ADITAct_Opeartion");
            entity.Property(e => e.AditactRateofDep)
                .HasColumnType("money")
                .HasColumnName("ADITAct_RateofDep");
            entity.Property(e => e.AditactStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ADITAct_Status");
            entity.Property(e => e.AditactUpdatedBy).HasColumnName("ADITAct_UpdatedBy");
            entity.Property(e => e.AditactUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADITAct_UpdatedOn");
            entity.Property(e => e.AditactWrittenDownValue)
                .HasColumnType("money")
                .HasColumnName("ADITAct_WrittenDownValue");
            entity.Property(e => e.AditactYearId).HasColumnName("ADITAct_YearID");
        });

        modelBuilder.Entity<AccAssetDepreciation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_AssetDepreciation");

            entity.Property(e => e.AdepApprovedBy).HasColumnName("ADep_ApprovedBy");
            entity.Property(e => e.AdepApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_ApprovedOn");
            entity.Property(e => e.AdepAssetId).HasColumnName("ADep_AssetID");
            entity.Property(e => e.AdepBay).HasColumnName("ADep_Bay");
            entity.Property(e => e.AdepClosingDate)
                .HasColumnType("datetime")
                .HasColumnName("ADep_ClosingDate");
            entity.Property(e => e.AdepCompId).HasColumnName("ADep_CompID");
            entity.Property(e => e.AdepCreatedBy).HasColumnName("ADep_CreatedBy");
            entity.Property(e => e.AdepCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_CreatedOn");
            entity.Property(e => e.AdepCustId).HasColumnName("ADep_CustId");
            entity.Property(e => e.AdepDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADep_DelFlag");
            entity.Property(e => e.AdepDepartment).HasColumnName("ADep_Department");
            entity.Property(e => e.AdepDepreciationforFy)
                .HasColumnType("money")
                .HasColumnName("ADep_DepreciationforFY");
            entity.Property(e => e.AdepDivision).HasColumnName("ADep_Division");
            entity.Property(e => e.AdepId).HasColumnName("ADep_ID");
            entity.Property(e => e.AdepIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ADep_IPAddress");
            entity.Property(e => e.AdepItem)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ADep_Item");
            entity.Property(e => e.AdepLocation).HasColumnName("ADep_Location");
            entity.Property(e => e.AdepMethod).HasColumnName("ADep_Method");
            entity.Property(e => e.AdepOpbforYr)
                .HasColumnType("money")
                .HasColumnName("ADep_OPBForYR");
            entity.Property(e => e.AdepOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ADep_Opeartion");
            entity.Property(e => e.AdepRateofDep)
                .HasColumnType("money")
                .HasColumnName("ADep_RateofDep");
            entity.Property(e => e.AdepStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ADep_Status");
            entity.Property(e => e.AdepTransType).HasColumnName("ADep_TransType");
            entity.Property(e => e.AdepUpdatedBy).HasColumnName("ADep_UpdatedBy");
            entity.Property(e => e.AdepUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_UpdatedOn");
            entity.Property(e => e.AdepWrittenDownValue)
                .HasColumnType("money")
                .HasColumnName("ADep_WrittenDownValue");
            entity.Property(e => e.AdepYearId).HasColumnName("ADep_YearID");
        });

        modelBuilder.Entity<AccAssetDepreciationPrev>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_AssetDepreciationPrev");

            entity.Property(e => e.AdepApprovedBy).HasColumnName("ADep_ApprovedBy");
            entity.Property(e => e.AdepApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_ApprovedOn");
            entity.Property(e => e.AdepAssetId).HasColumnName("ADep_AssetID");
            entity.Property(e => e.AdepBay).HasColumnName("ADep_Bay");
            entity.Property(e => e.AdepClosingDate)
                .HasColumnType("datetime")
                .HasColumnName("ADep_ClosingDate");
            entity.Property(e => e.AdepCompId).HasColumnName("ADep_CompID");
            entity.Property(e => e.AdepCreatedBy).HasColumnName("ADep_CreatedBy");
            entity.Property(e => e.AdepCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_CreatedOn");
            entity.Property(e => e.AdepCustId).HasColumnName("ADep_CustId");
            entity.Property(e => e.AdepDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ADep_DelFlag");
            entity.Property(e => e.AdepDepartment).HasColumnName("ADep_Department");
            entity.Property(e => e.AdepDepreciationforFy)
                .HasColumnType("money")
                .HasColumnName("ADep_DepreciationforFY");
            entity.Property(e => e.AdepDivision).HasColumnName("ADep_Division");
            entity.Property(e => e.AdepId).HasColumnName("ADep_ID");
            entity.Property(e => e.AdepIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ADep_IPAddress");
            entity.Property(e => e.AdepItem)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ADep_Item");
            entity.Property(e => e.AdepLocation).HasColumnName("ADep_Location");
            entity.Property(e => e.AdepMethod).HasColumnName("ADep_Method");
            entity.Property(e => e.AdepOpbforYr)
                .HasColumnType("money")
                .HasColumnName("ADep_OPBForYR");
            entity.Property(e => e.AdepOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ADep_Opeartion");
            entity.Property(e => e.AdepRateofDep)
                .HasColumnType("money")
                .HasColumnName("ADep_RateofDep");
            entity.Property(e => e.AdepStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ADep_Status");
            entity.Property(e => e.AdepTransType).HasColumnName("ADep_TransType");
            entity.Property(e => e.AdepUpdatedBy).HasColumnName("ADep_UpdatedBy");
            entity.Property(e => e.AdepUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADep_UpdatedOn");
            entity.Property(e => e.AdepWrittenDownValue)
                .HasColumnType("money")
                .HasColumnName("ADep_WrittenDownValue");
            entity.Property(e => e.AdepYearId).HasColumnName("ADep_YearID");
        });

        modelBuilder.Entity<AccAssetLocationSetup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_AssetLocationSetup");

            entity.Property(e => e.LsApprovedBy).HasColumnName("LS_ApprovedBy");
            entity.Property(e => e.LsApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("LS_ApprovedOn");
            entity.Property(e => e.LsCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LS_Code");
            entity.Property(e => e.LsCompId).HasColumnName("LS_CompID");
            entity.Property(e => e.LsCreatedBy).HasColumnName("LS_CreatedBy");
            entity.Property(e => e.LsCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LS_CreatedOn");
            entity.Property(e => e.LsCustId).HasColumnName("LS_CustId");
            entity.Property(e => e.LsDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("LS_DelFlag");
            entity.Property(e => e.LsDescCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LS_DescCode");
            entity.Property(e => e.LsDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LS_Description");
            entity.Property(e => e.LsId).HasColumnName("LS_ID");
            entity.Property(e => e.LsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LS_IPAddress");
            entity.Property(e => e.LsLevelCode).HasColumnName("LS_LevelCode");
            entity.Property(e => e.LsOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LS_Opeartion");
            entity.Property(e => e.LsParentId).HasColumnName("LS_ParentID");
            entity.Property(e => e.LsStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LS_Status");
            entity.Property(e => e.LsUpdatedBy).HasColumnName("LS_UpdatedBy");
            entity.Property(e => e.LsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LS_UpdatedOn");
            entity.Property(e => e.LsYearId).HasColumnName("LS_YearID");
        });

        modelBuilder.Entity<AccAssetMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_AssetMaster");

            entity.Property(e => e.AmApprovedBy).HasColumnName("AM_ApprovedBy");
            entity.Property(e => e.AmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AM_ApprovedOn");
            entity.Property(e => e.AmCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AM_Code");
            entity.Property(e => e.AmCompId).HasColumnName("AM_CompID");
            entity.Property(e => e.AmCreatedBy).HasColumnName("AM_CreatedBy");
            entity.Property(e => e.AmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AM_CreatedOn");
            entity.Property(e => e.AmCustId).HasColumnName("AM_CustId");
            entity.Property(e => e.AmDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AM_DelFlag");
            entity.Property(e => e.AmDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AM_Description");
            entity.Property(e => e.AmId).HasColumnName("AM_ID");
            entity.Property(e => e.AmInitialDep).HasColumnName("AM_InitialDep");
            entity.Property(e => e.AmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AM_IPAddress");
            entity.Property(e => e.AmItrate)
                .HasColumnType("money")
                .HasColumnName("AM_ITRate");
            entity.Property(e => e.AmLevelCode).HasColumnName("AM_LevelCode");
            entity.Property(e => e.AmOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AM_Opeartion");
            entity.Property(e => e.AmOriginalCost)
                .HasColumnType("money")
                .HasColumnName("AM_OriginalCost");
            entity.Property(e => e.AmParentId).HasColumnName("AM_ParentID");
            entity.Property(e => e.AmResidualValue).HasColumnName("AM_ResidualValue");
            entity.Property(e => e.AmStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AM_Status");
            entity.Property(e => e.AmUpdatedBy).HasColumnName("AM_UpdatedBy");
            entity.Property(e => e.AmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AM_UpdatedOn");
            entity.Property(e => e.AmWdvitact)
                .HasColumnType("money")
                .HasColumnName("AM_WDVITAct");
            entity.Property(e => e.AmYearId).HasColumnName("AM_YearID");
        });

        modelBuilder.Entity<AccCashflow>(entity =>
        {
            entity.HasKey(e => e.AcfPkid).HasName("PK__Acc_Cash__425DFD1D57FDB2D3");

            entity.ToTable("Acc_Cashflow");

            entity.Property(e => e.AcfPkid)
                .ValueGeneratedNever()
                .HasColumnName("ACF_pkid");
            entity.Property(e => e.AcfBranchid).HasColumnName("ACF_Branchid");
            entity.Property(e => e.AcfCatagary).HasColumnName("ACF_Catagary");
            entity.Property(e => e.AcfCompid).HasColumnName("ACF_Compid");
            entity.Property(e => e.AcfCrby).HasColumnName("ACF_Crby");
            entity.Property(e => e.AcfCron)
                .HasColumnType("datetime")
                .HasColumnName("ACF_Cron");
            entity.Property(e => e.AcfCurrentAmount)
                .HasColumnType("money")
                .HasColumnName("ACF_Current_Amount");
            entity.Property(e => e.AcfCustid).HasColumnName("ACF_Custid");
            entity.Property(e => e.AcfDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ACF_Description");
            entity.Property(e => e.AcfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ACF_Ipaddress");
            entity.Property(e => e.AcfPrevAmount)
                .HasColumnType("money")
                .HasColumnName("ACF_Prev_Amount");
            entity.Property(e => e.AcfStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ACF_Status");
            entity.Property(e => e.AcfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ACF_Updated_On");
            entity.Property(e => e.AcfUpdatedby).HasColumnName("ACF_Updatedby");
            entity.Property(e => e.AcfYearid).HasColumnName("ACF_yearid");
        });

        modelBuilder.Entity<AccChangesInventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_Changes_Inventories");

            entity.Property(e => e.CiApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CI_ApprovedOn");
            entity.Property(e => e.CiApprovedby).HasColumnName("CI_Approvedby");
            entity.Property(e => e.CiCbvalues)
                .HasColumnType("money")
                .HasColumnName("CI_CBValues");
            entity.Property(e => e.CiCompId).HasColumnName("CI_CompID");
            entity.Property(e => e.CiCrBy).HasColumnName("CI_CrBy");
            entity.Property(e => e.CiCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CI_CrOn");
            entity.Property(e => e.CiCustId).HasColumnName("CI_CustId");
            entity.Property(e => e.CiDate)
                .HasColumnType("datetime")
                .HasColumnName("CI_DATE");
            entity.Property(e => e.CiDelflag)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CI_Delflag");
            entity.Property(e => e.CiFinancialYear).HasColumnName("CI_FinancialYear");
            entity.Property(e => e.CiGlid).HasColumnName("CI_Glid");
            entity.Property(e => e.CiGroup).HasColumnName("CI_Group");
            entity.Property(e => e.CiHead).HasColumnName("CI_Head");
            entity.Property(e => e.CiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CI_IPAddress");
            entity.Property(e => e.CiNote).HasColumnName("CI_Note");
            entity.Property(e => e.CiObvalues)
                .HasColumnType("money")
                .HasColumnName("CI_OBValues");
            entity.Property(e => e.CiOrgtype).HasColumnName("CI_Orgtype");
            entity.Property(e => e.CiPkid).HasColumnName("CI_PKID");
            entity.Property(e => e.CiSavedBy).HasColumnName("CI_SavedBy");
            entity.Property(e => e.CiSavedOn)
                .HasColumnType("datetime")
                .HasColumnName("CI_SavedOn");
            entity.Property(e => e.CiStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CI_Status");
            entity.Property(e => e.CiSubGlid).HasColumnName("CI_SubGlid");
            entity.Property(e => e.CiSubgroup).HasColumnName("CI_Subgroup");
            entity.Property(e => e.CiUpdatedBy).HasColumnName("CI_UpdatedBy");
            entity.Property(e => e.CiUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CI_UpdatedOn");
        });

        modelBuilder.Entity<AccClosingstockItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_Closingstock_Items");

            entity.Property(e => e.AcsiApprovedby).HasColumnName("ACSI_APPROVEDBY");
            entity.Property(e => e.AcsiApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ACSI_APPROVEDON");
            entity.Property(e => e.AcsiClassification)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACSI_classification");
            entity.Property(e => e.AcsiCompId).HasColumnName("ACSI_CompId");
            entity.Property(e => e.AcsiCrby).HasColumnName("ACSI_CRBY");
            entity.Property(e => e.AcsiCron)
                .HasColumnType("datetime")
                .HasColumnName("ACSI_CRON");
            entity.Property(e => e.AcsiCustid).HasColumnName("ACSI_Custid");
            entity.Property(e => e.AcsiDeletedby).HasColumnName("ACSI_DELETEDBY");
            entity.Property(e => e.AcsiDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ACSI_DELETEDON");
            entity.Property(e => e.AcsiDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ACSI_DELFLG");
            entity.Property(e => e.AcsiId).HasColumnName("ACSI_id");
            entity.Property(e => e.AcsiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ACSI_IPAddress");
            entity.Property(e => e.AcsiItemdesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ACSI_Itemdesc");
            entity.Property(e => e.AcsiItemdescCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACSI_ItemdescCode");
            entity.Property(e => e.AcsiQty).HasColumnName("ACSI_Qty");
            entity.Property(e => e.AcsiRate)
                .HasColumnType("money")
                .HasColumnName("ACSI_Rate");
            entity.Property(e => e.AcsiRecallby).HasColumnName("ACSI_RECALLBY");
            entity.Property(e => e.AcsiRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ACSI_RECALLON");
            entity.Property(e => e.AcsiStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ACSI_STATUS");
            entity.Property(e => e.AcsiTotal)
                .HasColumnType("money")
                .HasColumnName("ACSI_Total");
            entity.Property(e => e.AcsiType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACSI_Type");
            entity.Property(e => e.AcsiUpdatedby).HasColumnName("ACSI_UPDATEDBY");
            entity.Property(e => e.AcsiUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ACSI_UPDATEDON");
            entity.Property(e => e.AcsiYearid).HasColumnName("ACSI_YEARId");
        });

        modelBuilder.Entity<AccCoaSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("acc_coa_settings");

            entity.Property(e => e.AcsAccHead).HasColumnName("ACS_AccHead");
            entity.Property(e => e.AcsAccHeadPrefix)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("ACS_AccHeadPrefix");
            entity.Property(e => e.AcsCompId).HasColumnName("ACS_CompId");
            entity.Property(e => e.AcsCreatedBy).HasColumnName("ACS_CreatedBy");
            entity.Property(e => e.AcsCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ACS_CreatedOn");
            entity.Property(e => e.AcsGl).HasColumnName("ACS_GL");
            entity.Property(e => e.AcsGroup).HasColumnName("ACS_Group");
            entity.Property(e => e.AcsId).HasColumnName("ACS_Id");
            entity.Property(e => e.AcsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ACS_IPAddress");
            entity.Property(e => e.AcsOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACS_operation");
            entity.Property(e => e.AcsSubGl).HasColumnName("ACS_SubGL");
            entity.Property(e => e.AcsSubGroup).HasColumnName("ACS_SubGroup");
            entity.Property(e => e.AcsUpdatedBy).HasColumnName("ACS_UpdatedBy");
            entity.Property(e => e.AcsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ACS_UpdatedOn");
        });

        modelBuilder.Entity<AccFixedAssetAdditionDel>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_FixedAssetAdditionDel");

            entity.Property(e => e.AfaaActualLocn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAA_ActualLocn");
            entity.Property(e => e.AfaaAddnType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("AFAA_AddnType");
            entity.Property(e => e.AfaaAddtnDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_AddtnDate");
            entity.Property(e => e.AfaaApprovedBy).HasColumnName("AFAA_ApprovedBy");
            entity.Property(e => e.AfaaApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_ApprovedOn");
            entity.Property(e => e.AfaaAssetAge)
                .HasColumnType("money")
                .HasColumnName("AFAA_AssetAge");
            entity.Property(e => e.AfaaAssetAmount)
                .HasColumnType("money")
                .HasColumnName("AFAA_AssetAmount");
            entity.Property(e => e.AfaaAssetDelDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_AssetDelDate");
            entity.Property(e => e.AfaaAssetDelId).HasColumnName("AFAA_AssetDelID");
            entity.Property(e => e.AfaaAssetDeletionDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_AssetDeletionDate");
            entity.Property(e => e.AfaaAssetDesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAA_AssetDesc");
            entity.Property(e => e.AfaaAssetNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAA_AssetNo");
            entity.Property(e => e.AfaaAssetRefNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAA_AssetRefNo");
            entity.Property(e => e.AfaaAssetTrType).HasColumnName("AFAA_AssetTrType");
            entity.Property(e => e.AfaaAssetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAA_AssetType");
            entity.Property(e => e.AfaaAssetvalue)
                .HasColumnType("money")
                .HasColumnName("AFAA_Assetvalue");
            entity.Property(e => e.AfaaBay).HasColumnName("AFAA_Bay");
            entity.Property(e => e.AfaaCommissionDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_CommissionDate");
            entity.Property(e => e.AfaaCompId).HasColumnName("AFAA_CompID");
            entity.Property(e => e.AfaaCreatedBy).HasColumnName("AFAA_CreatedBy");
            entity.Property(e => e.AfaaCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_CreatedOn");
            entity.Property(e => e.AfaaCurrencyAmnt)
                .HasColumnType("money")
                .HasColumnName("AFAA_CurrencyAmnt");
            entity.Property(e => e.AfaaCurrencyType).HasColumnName("AFAA_CurrencyType");
            entity.Property(e => e.AfaaCustId).HasColumnName("AFAA_CustId");
            entity.Property(e => e.AfaaDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AFAA_Delflag");
            entity.Property(e => e.AfaaDelnType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("AFAA_DelnType");
            entity.Property(e => e.AfaaDepartment).HasColumnName("AFAA_Department");
            entity.Property(e => e.AfaaDepreAmount)
                .HasColumnType("money")
                .HasColumnName("AFAA_DepreAmount");
            entity.Property(e => e.AfaaDepreciation)
                .HasColumnType("money")
                .HasColumnName("AFAA_Depreciation");
            entity.Property(e => e.AfaaDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAA_Description");
            entity.Property(e => e.AfaaDivision).HasColumnName("AFAA_Division");
            entity.Property(e => e.AfaaFyamount)
                .HasColumnType("money")
                .HasColumnName("AFAA_FYAmount");
            entity.Property(e => e.AfaaId).HasColumnName("AFAA_ID");
            entity.Property(e => e.AfaaIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAA_IPAddress");
            entity.Property(e => e.AfaaItemCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAA_ItemCode");
            entity.Property(e => e.AfaaItemDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAA_ItemDescription");
            entity.Property(e => e.AfaaItemType).HasColumnName("AFAA_ItemType");
            entity.Property(e => e.AfaaLocation).HasColumnName("AFAA_Location");
            entity.Property(e => e.AfaaOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AFAA_Operation");
            entity.Property(e => e.AfaaPurchaseDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_PurchaseDate");
            entity.Property(e => e.AfaaQuantity).HasColumnName("AFAA_Quantity");
            entity.Property(e => e.AfaaStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AFAA_Status");
            entity.Property(e => e.AfaaSupplierCode).HasColumnName("AFAA_SupplierCode");
            entity.Property(e => e.AfaaSupplierName).HasColumnName("AFAA_SupplierName");
            entity.Property(e => e.AfaaTrType).HasColumnName("AFAA_TrType");
            entity.Property(e => e.AfaaUpdatedBy).HasColumnName("AFAA_UpdatedBy");
            entity.Property(e => e.AfaaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAA_UpdatedOn");
            entity.Property(e => e.AfaaYearId).HasColumnName("AFAA_YearID");
        });

        modelBuilder.Entity<AccFixedAssetAdditionDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_FixedAssetAdditionDetails");

            entity.Property(e => e.FaadAssetType).HasColumnName("FAAD_AssetType");
            entity.Property(e => e.FaadAssetValue)
                .HasColumnType("money")
                .HasColumnName("FAAD_AssetValue");
            entity.Property(e => e.FaadBasicCost)
                .HasColumnType("money")
                .HasColumnName("FAAD_BasicCost");
            entity.Property(e => e.FaadBay).HasColumnName("FAAD_Bay");
            entity.Property(e => e.FaadChkCost).HasColumnName("FAAD_chkCost");
            entity.Property(e => e.FaadCompId).HasColumnName("FAAD_CompID");
            entity.Property(e => e.FaadCreatedBy).HasColumnName("FAAD_CreatedBy");
            entity.Property(e => e.FaadCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FAAD_CreatedOn");
            entity.Property(e => e.FaadCustId).HasColumnName("FAAD_CustId");
            entity.Property(e => e.FaadDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FAAD_Delflag");
            entity.Property(e => e.FaadDepartment).HasColumnName("FAAD_Department");
            entity.Property(e => e.FaadDivision).HasColumnName("FAAD_Division");
            entity.Property(e => e.FaadDocDate)
                .HasColumnType("datetime")
                .HasColumnName("FAAD_DocDate");
            entity.Property(e => e.FaadDocNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FAAD_DocNo");
            entity.Property(e => e.FaadInitDep).HasColumnName("FAAD_InitDep");
            entity.Property(e => e.FaadIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FAAD_IPAddress");
            entity.Property(e => e.FaadItemType).HasColumnName("FAAD_ItemType");
            entity.Property(e => e.FaadLocation).HasColumnName("FAAD_Location");
            entity.Property(e => e.FaadMasId).HasColumnName("FAAD_MasID");
            entity.Property(e => e.FaadOtherTrAmount)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FAAD_OtherTrAmount");
            entity.Property(e => e.FaadOtherTrType).HasColumnName("FAAD_OtherTrType");
            entity.Property(e => e.FaadParticulars)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("FAAD_Particulars");
            entity.Property(e => e.FaadPkid).HasColumnName("FAAD_PKID");
            entity.Property(e => e.FaadStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("FAAD_Status");
            entity.Property(e => e.FaadSupplierName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FAAD_SupplierName");
            entity.Property(e => e.FaadTaxAmount)
                .HasColumnType("money")
                .HasColumnName("FAAD_TaxAmount");
            entity.Property(e => e.FaadTotal)
                .HasColumnType("money")
                .HasColumnName("FAAD_Total");
            entity.Property(e => e.FaadUpdatedBy).HasColumnName("FAAD_UpdatedBy");
            entity.Property(e => e.FaadUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FAAD_UpdatedOn");
            entity.Property(e => e.FaadYearId).HasColumnName("FAAD_YearID");
        });

        modelBuilder.Entity<AccFixedAssetDeletion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_FixedAssetDeletion");

            entity.Property(e => e.AfadAmount)
                .HasColumnType("money")
                .HasColumnName("AFAD_Amount");
            entity.Property(e => e.AfadApprovedBy).HasColumnName("AFAD_ApprovedBy");
            entity.Property(e => e.AfadApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_ApprovedOn");
            entity.Property(e => e.AfadAsset).HasColumnName("AFAD_Asset");
            entity.Property(e => e.AfadAssetClass).HasColumnName("AFAD_AssetClass");
            entity.Property(e => e.AfadAssetDelDesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAD_AssetDelDesc");
            entity.Property(e => e.AfadAssetDeletion).HasColumnName("AFAD_AssetDeletion");
            entity.Property(e => e.AfadAssetDeletionType).HasColumnName("AFAD_AssetDeletionType");
            entity.Property(e => e.AfadBay).HasColumnName("AFAD_Bay");
            entity.Property(e => e.AfadCompId).HasColumnName("AFAD_CompID");
            entity.Property(e => e.AfadContAssetValue)
                .HasColumnType("money")
                .HasColumnName("AFAD_ContAssetValue");
            entity.Property(e => e.AfadContDep)
                .HasColumnType("money")
                .HasColumnName("AFAD_ContDep");
            entity.Property(e => e.AfadContWdv)
                .HasColumnType("money")
                .HasColumnName("AFAD_ContWDV");
            entity.Property(e => e.AfadCostofTransport)
                .HasColumnType("money")
                .HasColumnName("AFAD_CostofTransport");
            entity.Property(e => e.AfadCreatedBy).HasColumnName("AFAD_CreatedBy");
            entity.Property(e => e.AfadCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_CreatedOn");
            entity.Property(e => e.AfadCustomerName).HasColumnName("AFAD_CustomerName");
            entity.Property(e => e.AfadDateofInitiate)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_DateofInitiate");
            entity.Property(e => e.AfadDateofReceived)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_DateofReceived");
            entity.Property(e => e.AfadDelDeprec)
                .HasColumnType("money")
                .HasColumnName("AFAD_DelDeprec");
            entity.Property(e => e.AfadDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_DeletedOn");
            entity.Property(e => e.AfadDeletedby).HasColumnName("AFAD_Deletedby");
            entity.Property(e => e.AfadDeletionDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_DeletionDate");
            entity.Property(e => e.AfadDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AFAD_Delflag");
            entity.Property(e => e.AfadDepartment).HasColumnName("AFAD_Department");
            entity.Property(e => e.AfadDivision).HasColumnName("AFAD_Division");
            entity.Property(e => e.AfadId).HasColumnName("AFAD_ID");
            entity.Property(e => e.AfadInsAmtClaimed)
                .HasColumnType("money")
                .HasColumnName("AFAD_InsAmtClaimed");
            entity.Property(e => e.AfadInsAmtRecvd)
                .HasColumnType("money")
                .HasColumnName("AFAD_InsAmtRecvd");
            entity.Property(e => e.AfadInsClaimedDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_InsClaimedDate");
            entity.Property(e => e.AfadInsClaimedNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAD_InsClaimedNo");
            entity.Property(e => e.AfadInsRefDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAD_InsRefDate");
            entity.Property(e => e.AfadInsRefNo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AFAD_InsRefNo");
            entity.Property(e => e.AfadInstallationCost)
                .HasColumnType("money")
                .HasColumnName("AFAD_InstallationCost");
            entity.Property(e => e.AfadIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAD_IPAddress");
            entity.Property(e => e.AfadLocation).HasColumnName("AFAD_Location");
            entity.Property(e => e.AfadPaymenttype).HasColumnName("AFAD_Paymenttype");
            entity.Property(e => e.AfadPorLamount)
                .HasColumnType("money")
                .HasColumnName("AFAD_PorLAmount");
            entity.Property(e => e.AfadPorLstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAD_PorLStatus");
            entity.Property(e => e.AfadQuantity).HasColumnName("AFAD_Quantity");
            entity.Property(e => e.AfadSalesPrice)
                .HasColumnType("money")
                .HasColumnName("AFAD_SalesPrice");
            entity.Property(e => e.AfadStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AFAD_Status");
            entity.Property(e => e.AfadToBay).HasColumnName("AFAD_ToBay");
            entity.Property(e => e.AfadToDepartment).HasColumnName("AFAD_ToDepartment");
            entity.Property(e => e.AfadToDivision).HasColumnName("AFAD_ToDivision");
            entity.Property(e => e.AfadToLocation).HasColumnName("AFAD_ToLocation");
            entity.Property(e => e.AfadTransNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAD_TransNo");
            entity.Property(e => e.AfadWdvvalue)
                .HasColumnType("money")
                .HasColumnName("AFAD_WDVValue");
            entity.Property(e => e.AfadYearId).HasColumnName("AFAD_YearID");
        });

        modelBuilder.Entity<AccFixedAssetMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_FixedAssetMaster");

            entity.Property(e => e.AfamAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_Address");
            entity.Property(e => e.AfamAmccompanyName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_AMCCompanyName");
            entity.Property(e => e.AfamAmcfrmDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_AMCFrmDate");
            entity.Property(e => e.AfamAmcto)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_AMCTo");
            entity.Property(e => e.AfamAmount)
                .HasColumnType("money")
                .HasColumnName("AFAM_Amount");
            entity.Property(e => e.AfamAssetAge)
                .HasColumnType("money")
                .HasColumnName("AFAM_AssetAge");
            entity.Property(e => e.AfamAssetCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_AssetCode");
            entity.Property(e => e.AfamAssetDeletion).HasColumnName("AFAM_AssetDeletion");
            entity.Property(e => e.AfamAssetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_AssetType");
            entity.Property(e => e.AfamBay).HasColumnName("AFAM_Bay");
            entity.Property(e => e.AfamBrokerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_BrokerName");
            entity.Property(e => e.AfamCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_Code");
            entity.Property(e => e.AfamCommissionDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_CommissionDate");
            entity.Property(e => e.AfamCompId).HasColumnName("AFAM_CompID");
            entity.Property(e => e.AfamCompanyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_CompanyName");
            entity.Property(e => e.AfamContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_ContactPerson");
            entity.Property(e => e.AfamContactPrsn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_ContactPrsn");
            entity.Property(e => e.AfamContprsn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_Contprsn");
            entity.Property(e => e.AfamCreatedBy).HasColumnName("AFAM_CreatedBy");
            entity.Property(e => e.AfamCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_CreatedOn");
            entity.Property(e => e.AfamCustId).HasColumnName("AFAM_CustId");
            entity.Property(e => e.AfamDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_Date");
            entity.Property(e => e.AfamDateOfDeletion)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_DateOfDeletion");
            entity.Property(e => e.AfamDelFlag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_DelFlag");
            entity.Property(e => e.AfamDepartment).HasColumnName("AFAM_Department");
            entity.Property(e => e.AfamDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_Description");
            entity.Property(e => e.AfamDivision).HasColumnName("AFAM_Division");
            entity.Property(e => e.AfamDlnDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_DlnDate");
            entity.Property(e => e.AfamEmailId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_EmailID");
            entity.Property(e => e.AfamEmpcode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AFAM_EMPCode");
            entity.Property(e => e.AfamEmployeeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_EmployeeCode");
            entity.Property(e => e.AfamEmployeeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_EmployeeName");
            entity.Property(e => e.AfamFax)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AFAM_Fax");
            entity.Property(e => e.AfamId).HasColumnName("AFAM_ID");
            entity.Property(e => e.AfamIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AFAM_IPAddress");
            entity.Property(e => e.AfamItemCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_ItemCode");
            entity.Property(e => e.AfamItemDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_ItemDescription");
            entity.Property(e => e.AfamLaggriNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_LAggriNo");
            entity.Property(e => e.AfamLamount)
                .HasColumnType("money")
                .HasColumnName("AFAM_LAmount");
            entity.Property(e => e.AfamLcurrencyType).HasColumnName("AFAM_LCurrencyType");
            entity.Property(e => e.AfamLdate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_LDate");
            entity.Property(e => e.AfamLexchDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_LExchDate");
            entity.Property(e => e.AfamLocation).HasColumnName("AFAM_Location");
            entity.Property(e => e.AfamLtoWhom)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_LToWhom");
            entity.Property(e => e.AfamOpeartion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AFAM_Opeartion");
            entity.Property(e => e.AfamPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("AFAM_Phone");
            entity.Property(e => e.AfamPhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("AFAM_PhoneNo");
            entity.Property(e => e.AfamPolicyNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_PolicyNo");
            entity.Property(e => e.AfamPurchaseAmount)
                .HasColumnType("money")
                .HasColumnName("AFAM_PurchaseAmount");
            entity.Property(e => e.AfamPurchaseDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_PurchaseDate");
            entity.Property(e => e.AfamQuantity).HasColumnName("AFAM_Quantity");
            entity.Property(e => e.AfamRemark)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_Remark");
            entity.Property(e => e.AfamStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_Status");
            entity.Property(e => e.AfamSuplierName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AFAM_SuplierName");
            entity.Property(e => e.AfamToDate)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_ToDate");
            entity.Property(e => e.AfamTrAssetAge)
                .HasColumnType("money")
                .HasColumnName("AFAM_TrAssetAge");
            entity.Property(e => e.AfamTrUpdatedBy).HasColumnName("AFAM_TrUpdatedBy");
            entity.Property(e => e.AfamTrassetType).HasColumnName("AFAM_TRAssetType");
            entity.Property(e => e.AfamTrstatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AFAM_TRStatus");
            entity.Property(e => e.AfamTryear).HasColumnName("AFAM_TRYear");
            entity.Property(e => e.AfamUnit).HasColumnName("AFAM_Unit");
            entity.Property(e => e.AfamUpdatedBy).HasColumnName("AFAM_UpdatedBy");
            entity.Property(e => e.AfamUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AFAM_UpdatedOn");
            entity.Property(e => e.AfamValue)
                .HasColumnType("money")
                .HasColumnName("AFAM_Value");
            entity.Property(e => e.AfamWebsite)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_Website");
            entity.Property(e => e.AfamWrntyDesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AFAM_WrntyDesc");
            entity.Property(e => e.AfamYearId).HasColumnName("AFAM_YearID");
        });

        modelBuilder.Entity<AccFixedAssetsTransaction>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("acc_FixedAssets_Transaction");

            entity.Property(e => e.AccFatAccountHeadId).HasColumnName("Acc_FAT_AccountHeadID");
            entity.Property(e => e.AccFatAdditionId).HasColumnName("Acc_FAT_AdditionID");
            entity.Property(e => e.AccFatAdditionSubid).HasColumnName("Acc_FAT_AdditionSUBID");
            entity.Property(e => e.AccFatAdditon)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_Additon");
            entity.Property(e => e.AccFatChartofAccountId).HasColumnName("Acc_FAT_ChartofAccountID");
            entity.Property(e => e.AccFatCustId).HasColumnName("Acc_FAT_CustID");
            entity.Property(e => e.AccFatDclsBal)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_DClsBal");
            entity.Property(e => e.AccFatDdeduction)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_DDeduction");
            entity.Property(e => e.AccFatDfortheYear)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_DFortheYear");
            entity.Property(e => e.AccFatFixedAssetsId).HasColumnName("Acc_FAT_FixedAssetsID");
            entity.Property(e => e.AccFatFixedAssetsSubId).HasColumnName("Acc_FAT_FixedAssetsSubID");
            entity.Property(e => e.AccFatId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Acc_FAT_ID");
            entity.Property(e => e.AccFatIndType).HasColumnName("Acc_FAT_IndType");
            entity.Property(e => e.AccFatMclsBal)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_MClsBal");
            entity.Property(e => e.AccFatMopnBal)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_MOpnBal");
            entity.Property(e => e.AccFatReduction)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_Reduction");
            entity.Property(e => e.AccFatReportHeaderId).HasColumnName("Acc_FAT_ReportHeaderID");
            entity.Property(e => e.AccFatRopnBal)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_ROpnBal");
            entity.Property(e => e.AccFatRrateoff)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_RRateoff");
            entity.Property(e => e.AccFatRreduction)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_RReduction");
            entity.Property(e => e.AccFatRtransfer)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_RTransfer");
            entity.Property(e => e.AccFatSold)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_Sold");
            entity.Property(e => e.AccFatTransfer)
                .HasColumnType("money")
                .HasColumnName("Acc_FAT_Transfer");
        });

        modelBuilder.Entity<AccGeneralMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_General_Master");

            entity.Property(e => e.MasAppBy).HasColumnName("Mas_AppBy");
            entity.Property(e => e.MasAppOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_AppOn");
            entity.Property(e => e.MasCompId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_CompID");
            entity.Property(e => e.MasCrBy).HasColumnName("Mas_CrBy");
            entity.Property(e => e.MasCrOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_CrOn");
            entity.Property(e => e.MasDeletedBy).HasColumnName("Mas_DeletedBy");
            entity.Property(e => e.MasDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_DeletedOn");
            entity.Property(e => e.MasDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Mas_delflag");
            entity.Property(e => e.MasDesc)
                .IsUnicode(false)
                .HasColumnName("Mas_desc");
            entity.Property(e => e.MasId).HasColumnName("Mas_id");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasMaster).HasColumnName("Mas_master");
            entity.Property(e => e.MasOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Mas_Operation");
            entity.Property(e => e.MasRecalledBy).HasColumnName("Mas_RecalledBy");
            entity.Property(e => e.MasRemarks)
                .IsUnicode(false)
                .HasColumnName("Mas_Remarks");
            entity.Property(e => e.MasStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mas_Status");
            entity.Property(e => e.MasUpdatedBy).HasColumnName("Mas_UpdatedBy");
            entity.Property(e => e.MasUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_UpdatedOn");
        });

        modelBuilder.Entity<AccGeneralMaster1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_General_Master1");

            entity.Property(e => e.MasDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Mas_delflag");
            entity.Property(e => e.MasDesc)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Mas_desc");
            entity.Property(e => e.MasId).HasColumnName("Mas_id");
            entity.Property(e => e.MasMaster).HasColumnName("Mas_master");
            entity.Property(e => e.MasRemarks)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Mas_Remarks");
        });

        modelBuilder.Entity<AccGroupingAlias>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_GroupingAlias");

            entity.Property(e => e.AgaCompid).HasColumnName("AGA_Compid");
            entity.Property(e => e.AgaCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AGA_CreatedOn");
            entity.Property(e => e.AgaCreatedby).HasColumnName("AGA_Createdby");
            entity.Property(e => e.AgaDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AGA_Description");
            entity.Property(e => e.AgaGldesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AGA_GLDESC");
            entity.Property(e => e.AgaGlid).HasColumnName("AGA_GLID");
            entity.Property(e => e.AgaGrpLevel).HasColumnName("AGA_GrpLevel");
            entity.Property(e => e.AgaId).HasColumnName("AGA_ID");
            entity.Property(e => e.AgaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AGA_IPaddress");
            entity.Property(e => e.AgaOrgtype).HasColumnName("AGA_Orgtype");
            entity.Property(e => e.AgaScheduletype).HasColumnName("AGA_scheduletype");
            entity.Property(e => e.AgaStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AGA_Status");
        });

        modelBuilder.Entity<AccJeMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_JE_Master");

            entity.Property(e => e.AccJeAdvanceAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_AdvanceAmount");
            entity.Property(e => e.AccJeAdvanceNaration)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_AdvanceNaration");
            entity.Property(e => e.AccJeApprovedBy).HasColumnName("Acc_JE_ApprovedBy");
            entity.Property(e => e.AccJeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_ApprovedOn");
            entity.Property(e => e.AccJeBalanceAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_BalanceAmount");
            entity.Property(e => e.AccJeBankName)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BankName");
            entity.Property(e => e.AccJeBillAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_BillAmount");
            entity.Property(e => e.AccJeBillCreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_BillCreatedDate");
            entity.Property(e => e.AccJeBillDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_BillDate");
            entity.Property(e => e.AccJeBillNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BillNo");
            entity.Property(e => e.AccJeBillType).HasColumnName("Acc_JE_BillType");
            entity.Property(e => e.AccJeBranchId).HasColumnName("acc_JE_BranchId");
            entity.Property(e => e.AccJeBranchName)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BranchName");
            entity.Property(e => e.AccJeChequeDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_ChequeDate");
            entity.Property(e => e.AccJeChequeNo)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_ChequeNo");
            entity.Property(e => e.AccJeComnments)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Comnments");
            entity.Property(e => e.AccJeCompId).HasColumnName("Acc_JE_CompID");
            entity.Property(e => e.AccJeCreatedBy).HasColumnName("Acc_JE_CreatedBy");
            entity.Property(e => e.AccJeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_CreatedOn");
            entity.Property(e => e.AccJeDeletedBy).HasColumnName("Acc_JE_DeletedBy");
            entity.Property(e => e.AccJeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_DeletedOn");
            entity.Property(e => e.AccJeId).HasColumnName("Acc_JE_ID");
            entity.Property(e => e.AccJeIfsccode)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_IFSCCode");
            entity.Property(e => e.AccJeIpaddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_IPAddress");
            entity.Property(e => e.AccJeLocation).HasColumnName("Acc_JE_Location");
            entity.Property(e => e.AccJeNetAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_NetAmount");
            entity.Property(e => e.AccJeOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Operation");
            entity.Property(e => e.AccJeParty).HasColumnName("Acc_JE_Party");
            entity.Property(e => e.AccJePaymentNarration)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_PaymentNarration");
            entity.Property(e => e.AccJeQuarterId).HasColumnName("Acc_JE_QuarterId");
            entity.Property(e => e.AccJeRecalledBy).HasColumnName("Acc_JE_RecalledBy");
            entity.Property(e => e.AccJeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_RecalledOn");
            entity.Property(e => e.AccJeStatus)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Status");
            entity.Property(e => e.AccJeTransactionNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_TransactionNo");
            entity.Property(e => e.AccJeYearId).HasColumnName("Acc_JE_YearID");
        });

        modelBuilder.Entity<AccJeMasterHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_JE_Master_History");

            entity.Property(e => e.AjehAccJeid).HasColumnName("AJEH_AccJEID");
            entity.Property(e => e.AjehComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AJEH_Comments");
            entity.Property(e => e.AjehCompId).HasColumnName("AJEH_CompID");
            entity.Property(e => e.AjehDate)
                .HasColumnType("datetime")
                .HasColumnName("AJEH_Date");
            entity.Property(e => e.AjehIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AJEH_IPAddress");
            entity.Property(e => e.AjehPkid).HasColumnName("AJEH_PKID");
            entity.Property(e => e.AjehStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AJEH_Status");
            entity.Property(e => e.AjehUserId).HasColumnName("AJEH_UserID");
        });

        modelBuilder.Entity<AccJetransactionsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_JETransactions_Details");

            entity.Property(e => e.AjtbApprovedBy).HasColumnName("AJTB_ApprovedBy");
            entity.Property(e => e.AjtbApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_ApprovedOn");
            entity.Property(e => e.AjtbBillType).HasColumnName("AJTB_BillType");
            entity.Property(e => e.AjtbBranchId).HasColumnName("AJTB_BranchId");
            entity.Property(e => e.AjtbCompId).HasColumnName("AJTB_CompID");
            entity.Property(e => e.AjtbCreatedBy).HasColumnName("AJTB_CreatedBy");
            entity.Property(e => e.AjtbCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_CreatedOn");
            entity.Property(e => e.AjtbCredit)
                .HasColumnType("money")
                .HasColumnName("AJTB_Credit");
            entity.Property(e => e.AjtbCustId).HasColumnName("AJTB_CustId");
            entity.Property(e => e.AjtbDebit)
                .HasColumnType("money")
                .HasColumnName("AJTB_Debit");
            entity.Property(e => e.AjtbDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_DeletedOn");
            entity.Property(e => e.AjtbDeletedby).HasColumnName("AJTB_Deletedby");
            entity.Property(e => e.AjtbDesc).HasColumnName("AJTB_Desc");
            entity.Property(e => e.AjtbDescName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AJTB_DescName");
            entity.Property(e => e.AjtbDeschead).HasColumnName("AJTB_Deschead");
            entity.Property(e => e.AjtbId).HasColumnName("AJTB_ID");
            entity.Property(e => e.AjtbIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AJTB_IPAddress");
            entity.Property(e => e.AjtbMasid).HasColumnName("Ajtb_Masid");
            entity.Property(e => e.AjtbOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AJTB_Operation");
            entity.Property(e => e.AjtbQuarterId).HasColumnName("AJTB_QuarterId");
            entity.Property(e => e.AjtbScheduleTypeid).HasColumnName("AJTB_ScheduleTypeid");
            entity.Property(e => e.AjtbSeqReferenceNum).HasColumnName("AJTB_SeqReferenceNum");
            entity.Property(e => e.AjtbStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AJTB_Status");
            entity.Property(e => e.AjtbTranscNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AJTB_TranscNo");
            entity.Property(e => e.AjtbUpdatedBy).HasColumnName("AJTB_UpdatedBy");
            entity.Property(e => e.AjtbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_UpdatedOn");
            entity.Property(e => e.AjtbYearId).HasColumnName("AJTB_YearID");
        });

        modelBuilder.Entity<AccLedgerHeadingFactor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_LedgerHeadingFactors");

            entity.Property(e => e.AlhfBranchid).HasColumnName("ALHF_Branchid");
            entity.Property(e => e.AlhfCrby).HasColumnName("ALHF_CRBY");
            entity.Property(e => e.AlhfCron)
                .HasColumnType("datetime")
                .HasColumnName("ALHF_CRON");
            entity.Property(e => e.AlhfCustId).HasColumnName("ALHF_CustId");
            entity.Property(e => e.AlhfHeadingId).HasColumnName("ALHF_HeadingId");
            entity.Property(e => e.AlhfId).HasColumnName("ALHF_ID");
            entity.Property(e => e.AlhfMaterialId).HasColumnName("ALHF_MaterialId");
            entity.Property(e => e.AlhfRiskId).HasColumnName("ALHF_RiskId");
            entity.Property(e => e.AlhfSchedule).HasColumnName("ALHF_Schedule");
            entity.Property(e => e.AlhfYearid).HasColumnName("ALHF_Yearid");
        });

        modelBuilder.Entity<AccLedgerTransactionsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_LedgerTransactions_Details");

            entity.Property(e => e.AjtbApprovedBy).HasColumnName("AJTB_ApprovedBy");
            entity.Property(e => e.AjtbApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_ApprovedOn");
            entity.Property(e => e.AjtbBillType).HasColumnName("AJTB_BillType");
            entity.Property(e => e.AjtbBranchId).HasColumnName("AJTB_BranchId");
            entity.Property(e => e.AjtbCompId).HasColumnName("AJTB_CompID");
            entity.Property(e => e.AjtbCreatedBy).HasColumnName("AJTB_CreatedBy");
            entity.Property(e => e.AjtbCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_CreatedOn");
            entity.Property(e => e.AjtbCredit)
                .HasColumnType("money")
                .HasColumnName("AJTB_Credit");
            entity.Property(e => e.AjtbCustId).HasColumnName("AJTB_CustId");
            entity.Property(e => e.AjtbDebit)
                .HasColumnType("money")
                .HasColumnName("AJTB_Debit");
            entity.Property(e => e.AjtbDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_DeletedOn");
            entity.Property(e => e.AjtbDeletedby).HasColumnName("AJTB_Deletedby");
            entity.Property(e => e.AjtbDesc).HasColumnName("AJTB_Desc");
            entity.Property(e => e.AjtbDescName)
                .IsUnicode(false)
                .HasColumnName("AJTB_DescName");
            entity.Property(e => e.AjtbDeschead).HasColumnName("AJTB_Deschead");
            entity.Property(e => e.AjtbId).HasColumnName("AJTB_ID");
            entity.Property(e => e.AjtbIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AJTB_IPAddress");
            entity.Property(e => e.AjtbMasid).HasColumnName("Ajtb_Masid");
            entity.Property(e => e.AjtbOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AJTB_Operation");
            entity.Property(e => e.AjtbQuarterId).HasColumnName("AJTB_QuarterId");
            entity.Property(e => e.AjtbScheduleTypeid).HasColumnName("AJTB_ScheduleTypeid");
            entity.Property(e => e.AjtbStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AJTB_Status");
            entity.Property(e => e.AjtbTranscNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AJTB_TranscNo");
            entity.Property(e => e.AjtbUpdatedBy).HasColumnName("AJTB_UpdatedBy");
            entity.Property(e => e.AjtbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AJTB_UpdatedOn");
            entity.Property(e => e.AjtbYearId).HasColumnName("AJTB_YearID");
        });

        modelBuilder.Entity<AccLtMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_LT_Master");

            entity.Property(e => e.AccJeAdvanceAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_AdvanceAmount");
            entity.Property(e => e.AccJeAdvanceNaration)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_AdvanceNaration");
            entity.Property(e => e.AccJeApprovedBy).HasColumnName("Acc_JE_ApprovedBy");
            entity.Property(e => e.AccJeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_ApprovedOn");
            entity.Property(e => e.AccJeBalanceAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_BalanceAmount");
            entity.Property(e => e.AccJeBankName)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BankName");
            entity.Property(e => e.AccJeBillAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_BillAmount");
            entity.Property(e => e.AccJeBillCreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_BillCreatedDate");
            entity.Property(e => e.AccJeBillDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_BillDate");
            entity.Property(e => e.AccJeBillNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BillNo");
            entity.Property(e => e.AccJeBillType).HasColumnName("Acc_JE_BillType");
            entity.Property(e => e.AccJeBranchId).HasColumnName("acc_JE_BranchId");
            entity.Property(e => e.AccJeBranchName)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_BranchName");
            entity.Property(e => e.AccJeChequeDate)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_ChequeDate");
            entity.Property(e => e.AccJeChequeNo)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_ChequeNo");
            entity.Property(e => e.AccJeComnments)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Comnments");
            entity.Property(e => e.AccJeCompId).HasColumnName("Acc_JE_CompID");
            entity.Property(e => e.AccJeCreatedBy).HasColumnName("Acc_JE_CreatedBy");
            entity.Property(e => e.AccJeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_CreatedOn");
            entity.Property(e => e.AccJeDeletedBy).HasColumnName("Acc_JE_DeletedBy");
            entity.Property(e => e.AccJeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_DeletedOn");
            entity.Property(e => e.AccJeId).HasColumnName("Acc_JE_ID");
            entity.Property(e => e.AccJeIfsccode)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_IFSCCode");
            entity.Property(e => e.AccJeIpaddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_IPAddress");
            entity.Property(e => e.AccJeLocation).HasColumnName("Acc_JE_Location");
            entity.Property(e => e.AccJeNetAmount)
                .HasColumnType("money")
                .HasColumnName("Acc_JE_NetAmount");
            entity.Property(e => e.AccJeOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Operation");
            entity.Property(e => e.AccJeParty).HasColumnName("Acc_JE_Party");
            entity.Property(e => e.AccJePaymentNarration)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_PaymentNarration");
            entity.Property(e => e.AccJeQuarterId).HasColumnName("Acc_JE_QuarterId");
            entity.Property(e => e.AccJeRecalledBy).HasColumnName("Acc_JE_RecalledBy");
            entity.Property(e => e.AccJeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_JE_RecalledOn");
            entity.Property(e => e.AccJeStatus)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_Status");
            entity.Property(e => e.AccJeTransactionNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Acc_JE_TransactionNo");
            entity.Property(e => e.AccJeYearId).HasColumnName("Acc_JE_YearID");
        });

        modelBuilder.Entity<AccLtMasterHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_LT_Master_History");

            entity.Property(e => e.AjehAccJeid).HasColumnName("AJEH_AccJEID");
            entity.Property(e => e.AjehComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AJEH_Comments");
            entity.Property(e => e.AjehCompId).HasColumnName("AJEH_CompID");
            entity.Property(e => e.AjehDate)
                .HasColumnType("datetime")
                .HasColumnName("AJEH_Date");
            entity.Property(e => e.AjehIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AJEH_IPAddress");
            entity.Property(e => e.AjehPkid).HasColumnName("AJEH_PKID");
            entity.Property(e => e.AjehStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AJEH_Status");
            entity.Property(e => e.AjehUserId).HasColumnName("AJEH_UserID");
        });

        modelBuilder.Entity<AccOpeningBalance>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_Opening_Balance");

            entity.Property(e => e.OpnAccHead).HasColumnName("Opn_AccHead");
            entity.Property(e => e.OpnApprovedBy).HasColumnName("Opn_ApprovedBy");
            entity.Property(e => e.OpnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_ApprovedOn");
            entity.Property(e => e.OpnClosingBalanceCredit)
                .HasColumnType("money")
                .HasColumnName("Opn_ClosingBalanceCredit");
            entity.Property(e => e.OpnClosingBalanceDebit)
                .HasColumnType("money")
                .HasColumnName("Opn_ClosingBalanceDebit");
            entity.Property(e => e.OpnCompId).HasColumnName("Opn_CompId");
            entity.Property(e => e.OpnCreatedBy).HasColumnName("Opn_CreatedBy");
            entity.Property(e => e.OpnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_CreatedOn");
            entity.Property(e => e.OpnCreditAmount).HasColumnName("Opn_CreditAmount");
            entity.Property(e => e.OpnCustType).HasColumnName("Opn_CustType");
            entity.Property(e => e.OpnDate)
                .HasColumnType("datetime")
                .HasColumnName("Opn_Date");
            entity.Property(e => e.OpnDebitAmt).HasColumnName("Opn_DebitAmt");
            entity.Property(e => e.OpnGlId).HasColumnName("Opn_GlId");
            entity.Property(e => e.OpnGlcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Opn_GLCode");
            entity.Property(e => e.OpnId).HasColumnName("Opn_Id");
            entity.Property(e => e.OpnIndType).HasColumnName("Opn_IndType");
            entity.Property(e => e.OpnIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Opn_IPAddress");
            entity.Property(e => e.OpnOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Opn_Operation");
            entity.Property(e => e.OpnSerialNo).HasColumnName("Opn_SerialNo");
            entity.Property(e => e.OpnStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Opn_Status");
            entity.Property(e => e.OpnUpdatedBy).HasColumnName("Opn_UpdatedBy");
            entity.Property(e => e.OpnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_UpdatedOn");
            entity.Property(e => e.OpnYearId).HasColumnName("Opn_YearId");
        });

        modelBuilder.Entity<AccOpeningBalance1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_Opening_Balance1");

            entity.Property(e => e.OpnAccHead).HasColumnName("Opn_AccHead");
            entity.Property(e => e.OpnApprovedBy).HasColumnName("Opn_ApprovedBy");
            entity.Property(e => e.OpnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_ApprovedOn");
            entity.Property(e => e.OpnClosingBalanceCredit)
                .HasColumnType("money")
                .HasColumnName("Opn_ClosingBalanceCredit");
            entity.Property(e => e.OpnClosingBalanceDebit)
                .HasColumnType("money")
                .HasColumnName("Opn_ClosingBalanceDebit");
            entity.Property(e => e.OpnCompId).HasColumnName("Opn_CompId");
            entity.Property(e => e.OpnCreatedBy).HasColumnName("Opn_CreatedBy");
            entity.Property(e => e.OpnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_CreatedOn");
            entity.Property(e => e.OpnCreditAmount)
                .HasColumnType("money")
                .HasColumnName("Opn_CreditAmount");
            entity.Property(e => e.OpnCustType).HasColumnName("Opn_CustType");
            entity.Property(e => e.OpnDate)
                .HasColumnType("datetime")
                .HasColumnName("Opn_Date");
            entity.Property(e => e.OpnDebitAmt)
                .HasColumnType("money")
                .HasColumnName("Opn_DebitAmt");
            entity.Property(e => e.OpnGlId).HasColumnName("Opn_GlId");
            entity.Property(e => e.OpnGlcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Opn_GLCode");
            entity.Property(e => e.OpnId).HasColumnName("Opn_Id");
            entity.Property(e => e.OpnIndType).HasColumnName("Opn_IndType");
            entity.Property(e => e.OpnIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Opn_IPAddress");
            entity.Property(e => e.OpnOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Opn_Operation");
            entity.Property(e => e.OpnSerialNo).HasColumnName("Opn_SerialNo");
            entity.Property(e => e.OpnStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Opn_Status");
            entity.Property(e => e.OpnUpdatedBy).HasColumnName("Opn_UpdatedBy");
            entity.Property(e => e.OpnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Opn_UpdatedOn");
            entity.Property(e => e.OpnYearId).HasColumnName("Opn_YearId");
        });

        modelBuilder.Entity<AccPartnershipFirm>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_Partnership_Firms");

            entity.Property(e => e.ApfAddOthers)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_AddOthers");
            entity.Property(e => e.ApfBranchId).HasColumnName("APF_Branch_ID");
            entity.Property(e => e.ApfCapitalAmount)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("APF_CapitalAmount");
            entity.Property(e => e.ApfCompId).HasColumnName("APF_CompID");
            entity.Property(e => e.ApfCrBy).HasColumnName("APF_CrBy");
            entity.Property(e => e.ApfCrOn)
                .HasColumnType("datetime")
                .HasColumnName("APF_CrOn");
            entity.Property(e => e.ApfCustId).HasColumnName("APF_Cust_ID");
            entity.Property(e => e.ApfDrawings)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_Drawings");
            entity.Property(e => e.ApfId).HasColumnName("APF_ID");
            entity.Property(e => e.ApfInterestOnCapital)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_InterestOnCapital");
            entity.Property(e => e.ApfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("APF_IPAddress");
            entity.Property(e => e.ApfLessOthers)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_LessOthers");
            entity.Property(e => e.ApfOpeningBalance)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_OpeningBalance");
            entity.Property(e => e.ApfPartnerId).HasColumnName("APF_Partner_ID");
            entity.Property(e => e.ApfPartnersSalary)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_PartnersSalary");
            entity.Property(e => e.ApfShareOfprofit)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_ShareOfprofit");
            entity.Property(e => e.ApfTransferToFixedCapital)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_TransferToFixedCapital");
            entity.Property(e => e.ApfUnsecuredLoanTreatedAsCapital)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("APF_UnsecuredLoanTreatedAsCapital");
            entity.Property(e => e.ApfUpdateBy).HasColumnName("APF_UpdateBy");
            entity.Property(e => e.ApfUpdateOn)
                .HasColumnType("datetime")
                .HasColumnName("APF_UpdateOn");
            entity.Property(e => e.ApfYearId).HasColumnName("APF_YearID");
        });

        modelBuilder.Entity<AccProfitAndLossAmount>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_ProfitAndLossAmount");

            entity.Property(e => e.AccPnLAmount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Acc_PnL_Amount");
            entity.Property(e => e.AccPnLBranchId)
                .IsUnicode(false)
                .HasColumnName("Acc_PnL_BranchId");
            entity.Property(e => e.AccPnLCrBy).HasColumnName("Acc_PnL_CrBy");
            entity.Property(e => e.AccPnLCrOn)
                .HasColumnType("datetime")
                .HasColumnName("Acc_PnL_CrOn");
            entity.Property(e => e.AccPnLCustid).HasColumnName("Acc_PnL_Custid");
            entity.Property(e => e.AccPnLDurtnId).HasColumnName("Acc_PnL_DurtnId");
            entity.Property(e => e.AccPnLFlag).HasColumnName("Acc_PnL_Flag");
            entity.Property(e => e.AccPnLPkid).HasColumnName("Acc_PnL_Pkid");
            entity.Property(e => e.AccPnLYearid).HasColumnName("Acc_PnL_Yearid");
        });

        modelBuilder.Entity<AccSaptransactionsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_SAPTransactions_Details");

            entity.Property(e => e.AstbApprovedBy).HasColumnName("ASTB_ApprovedBy");
            entity.Property(e => e.AstbApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASTB_ApprovedOn");
            entity.Property(e => e.AstbBillType).HasColumnName("ASTB_BillType");
            entity.Property(e => e.AstbBranchId).HasColumnName("ASTB_BranchId");
            entity.Property(e => e.AstbCompId).HasColumnName("ASTB_CompID");
            entity.Property(e => e.AstbCreatedBy).HasColumnName("ASTB_CreatedBy");
            entity.Property(e => e.AstbCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASTB_CreatedOn");
            entity.Property(e => e.AstbCredit)
                .HasColumnType("money")
                .HasColumnName("ASTB_Credit");
            entity.Property(e => e.AstbCustId).HasColumnName("ASTB_CustId");
            entity.Property(e => e.AstbDebit)
                .HasColumnType("money")
                .HasColumnName("ASTB_Debit");
            entity.Property(e => e.AstbDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASTB_DeletedOn");
            entity.Property(e => e.AstbDeletedby).HasColumnName("ASTB_Deletedby");
            entity.Property(e => e.AstbDesc).HasColumnName("ASTB_Desc");
            entity.Property(e => e.AstbDescName)
                .IsUnicode(false)
                .HasColumnName("ASTB_DescName");
            entity.Property(e => e.AstbDeschead).HasColumnName("ASTB_Deschead");
            entity.Property(e => e.AstbId).HasColumnName("ASTB_ID");
            entity.Property(e => e.AstbIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASTB_IPAddress");
            entity.Property(e => e.AstbMasid).HasColumnName("ASTB_Masid");
            entity.Property(e => e.AstbOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASTB_Operation");
            entity.Property(e => e.AstbQuarterId).HasColumnName("ASTB_QuarterId");
            entity.Property(e => e.AstbScheduleTypeid).HasColumnName("ASTB_ScheduleTypeid");
            entity.Property(e => e.AstbSeqReferenceNum).HasColumnName("ASTB_SeqReferenceNum");
            entity.Property(e => e.AstbStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASTB_Status");
            entity.Property(e => e.AstbTranscNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ASTB_TranscNo");
            entity.Property(e => e.AstbUpdatedBy).HasColumnName("ASTB_UpdatedBy");
            entity.Property(e => e.AstbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASTB_UpdatedOn");
            entity.Property(e => e.AstbYearId).HasColumnName("ASTB_YearID");
        });

        modelBuilder.Entity<AccScheduleHeading>(entity =>
        {
            entity.HasKey(e => e.AshId);

            entity.ToTable("ACC_ScheduleHeading");

            entity.Property(e => e.AshId)
                .ValueGeneratedNever()
                .HasColumnName("ASH_ID");
            entity.Property(e => e.AshApprovedby).HasColumnName("ASH_APPROVEDBY");
            entity.Property(e => e.AshApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ASH_APPROVEDON");
            entity.Property(e => e.AshCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASH_Code");
            entity.Property(e => e.AshCompId).HasColumnName("ASH_CompId");
            entity.Property(e => e.AshCrby).HasColumnName("ASH_CRBY");
            entity.Property(e => e.AshCron)
                .HasColumnType("datetime")
                .HasColumnName("ASH_CRON");
            entity.Property(e => e.AshDeletedby).HasColumnName("ASH_DELETEDBY");
            entity.Property(e => e.AshDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ASH_DELETEDON");
            entity.Property(e => e.AshDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASH_DELFLG");
            entity.Property(e => e.AshIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASH_IPAddress");
            entity.Property(e => e.AshName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ASH_Name");
            entity.Property(e => e.AshNotes).HasColumnName("ASH_Notes");
            entity.Property(e => e.AshOrgtype).HasColumnName("Ash_Orgtype");
            entity.Property(e => e.AshRecallby).HasColumnName("ASH_RECALLBY");
            entity.Property(e => e.AshRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ASH_RECALLON");
            entity.Property(e => e.AshScheduletype).HasColumnName("Ash_scheduletype");
            entity.Property(e => e.AshStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ASH_STATUS");
            entity.Property(e => e.AshTotal)
                .HasColumnType("money")
                .HasColumnName("Ash_Total");
            entity.Property(e => e.AshUpdatedby).HasColumnName("ASH_UPDATEDBY");
            entity.Property(e => e.AshUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ASH_UPDATEDON");
            entity.Property(e => e.AshYearid).HasColumnName("ASH_YEARId");
        });

        modelBuilder.Entity<AccScheduleItem>(entity =>
        {
            entity.HasKey(e => e.AsiId);

            entity.ToTable("ACC_ScheduleItems");

            entity.Property(e => e.AsiId)
                .ValueGeneratedNever()
                .HasColumnName("ASI_ID");
            entity.Property(e => e.AsiApprovedby).HasColumnName("ASI_APPROVEDBY");
            entity.Property(e => e.AsiApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ASI_APPROVEDON");
            entity.Property(e => e.AsiCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASI_Code");
            entity.Property(e => e.AsiCompId).HasColumnName("ASI_CompId");
            entity.Property(e => e.AsiCrby).HasColumnName("ASI_CRBY");
            entity.Property(e => e.AsiCron)
                .HasColumnType("datetime")
                .HasColumnName("ASI_CRON");
            entity.Property(e => e.AsiDeletedby).HasColumnName("ASI_DELETEDBY");
            entity.Property(e => e.AsiDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ASI_DELETEDON");
            entity.Property(e => e.AsiDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASI_DELFLG");
            entity.Property(e => e.AsiHeadingId).HasColumnName("ASI_HeadingID");
            entity.Property(e => e.AsiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASI_IPAddress");
            entity.Property(e => e.AsiName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ASI_Name");
            entity.Property(e => e.AsiOrgtype).HasColumnName("Asi_Orgtype");
            entity.Property(e => e.AsiRecallby).HasColumnName("ASI_RECALLBY");
            entity.Property(e => e.AsiRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ASI_RECALLON");
            entity.Property(e => e.AsiScheduletype).HasColumnName("Asi_scheduletype");
            entity.Property(e => e.AsiStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ASI_STATUS");
            entity.Property(e => e.AsiSubHeadingId).HasColumnName("ASI_SubHeadingID");
            entity.Property(e => e.AsiUpdatedby).HasColumnName("ASI_UPDATEDBY");
            entity.Property(e => e.AsiUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ASI_UPDATEDON");
            entity.Property(e => e.AsiYearid).HasColumnName("ASI_YEARId");
        });

        modelBuilder.Entity<AccScheduleSubHeading>(entity =>
        {
            entity.HasKey(e => e.AsshId);

            entity.ToTable("ACC_ScheduleSubHeading");

            entity.Property(e => e.AsshId)
                .ValueGeneratedNever()
                .HasColumnName("ASSH_ID");
            entity.Property(e => e.AsshApprovedby).HasColumnName("ASSH_APPROVEDBY");
            entity.Property(e => e.AsshApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSH_APPROVEDON");
            entity.Property(e => e.AsshCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASSH_Code");
            entity.Property(e => e.AsshCompId).HasColumnName("ASSH_CompId");
            entity.Property(e => e.AsshCrby).HasColumnName("ASSH_CRBY");
            entity.Property(e => e.AsshCron)
                .HasColumnType("datetime")
                .HasColumnName("ASSH_CRON");
            entity.Property(e => e.AsshDeletedby).HasColumnName("ASSH_DELETEDBY");
            entity.Property(e => e.AsshDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSH_DELETEDON");
            entity.Property(e => e.AsshDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASSH_DELFLG");
            entity.Property(e => e.AsshHeadingId).HasColumnName("ASSH_HeadingID");
            entity.Property(e => e.AsshIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASSH_IPAddress");
            entity.Property(e => e.AsshName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ASSH_Name");
            entity.Property(e => e.AsshNotes).HasColumnName("ASSH_Notes");
            entity.Property(e => e.AsshOrgtype).HasColumnName("Assh_Orgtype");
            entity.Property(e => e.AsshRecallby).HasColumnName("ASSH_RECALLBY");
            entity.Property(e => e.AsshRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ASSH_RECALLON");
            entity.Property(e => e.AsshScheduletype).HasColumnName("Assh_scheduletype");
            entity.Property(e => e.AsshStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ASSH_STATUS");
            entity.Property(e => e.AsshUpdatedby).HasColumnName("ASSH_UPDATEDBY");
            entity.Property(e => e.AsshUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSH_UPDATEDON");
            entity.Property(e => e.AsshYearid).HasColumnName("ASSH_YEARId");
        });

        modelBuilder.Entity<AccScheduleSubItem>(entity =>
        {
            entity.HasKey(e => e.AssiId);

            entity.ToTable("ACC_ScheduleSubItems");

            entity.Property(e => e.AssiId)
                .ValueGeneratedNever()
                .HasColumnName("ASSI_ID");
            entity.Property(e => e.AssiApprovedby).HasColumnName("ASSI_APPROVEDBY");
            entity.Property(e => e.AssiApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSI_APPROVEDON");
            entity.Property(e => e.AssiCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASSI_Code");
            entity.Property(e => e.AssiCompId).HasColumnName("ASSI_CompId");
            entity.Property(e => e.AssiCrby).HasColumnName("ASSI_CRBY");
            entity.Property(e => e.AssiCron)
                .HasColumnType("datetime")
                .HasColumnName("ASSI_CRON");
            entity.Property(e => e.AssiDeletedby).HasColumnName("ASSI_DELETEDBY");
            entity.Property(e => e.AssiDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSI_DELETEDON");
            entity.Property(e => e.AssiDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASSI_DELFLG");
            entity.Property(e => e.AssiHeadingId).HasColumnName("ASSI_HeadingID");
            entity.Property(e => e.AssiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASSI_IPAddress");
            entity.Property(e => e.AssiItemsId).HasColumnName("ASSI_ItemsID");
            entity.Property(e => e.AssiName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ASSI_Name");
            entity.Property(e => e.AssiOrgtype).HasColumnName("Assi_Orgtype");
            entity.Property(e => e.AssiRecallby).HasColumnName("ASSI_RECALLBY");
            entity.Property(e => e.AssiRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ASSI_RECALLON");
            entity.Property(e => e.AssiScheduletype).HasColumnName("Assi_scheduletype");
            entity.Property(e => e.AssiStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ASSI_STATUS");
            entity.Property(e => e.AssiSubHeadingId).HasColumnName("ASSI_SubHeadingID");
            entity.Property(e => e.AssiUpdatedby).HasColumnName("ASSI_UPDATEDBY");
            entity.Property(e => e.AssiUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ASSI_UPDATEDON");
            entity.Property(e => e.AssiYearid).HasColumnName("ASSI_YEARId");
        });

        modelBuilder.Entity<AccScheduleTemplate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_ScheduleTemplates");

            entity.Property(e => e.AstAccHeadId).HasColumnName("AST_AccHeadId");
            entity.Property(e => e.AstApprovedby).HasColumnName("AST_APPROVEDBY");
            entity.Property(e => e.AstApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("AST_APPROVEDON");
            entity.Property(e => e.AstCompId).HasColumnName("AST_CompId");
            entity.Property(e => e.AstCompanyLimit).HasColumnName("AST_Company_limit");
            entity.Property(e => e.AstCompanytype).HasColumnName("AST_Companytype");
            entity.Property(e => e.AstCrby).HasColumnName("AST_CRBY");
            entity.Property(e => e.AstCron)
                .HasColumnType("datetime")
                .HasColumnName("AST_CRON");
            entity.Property(e => e.AstDeletedby).HasColumnName("AST_DELETEDBY");
            entity.Property(e => e.AstDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("AST_DELETEDON");
            entity.Property(e => e.AstDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AST_DELFLG");
            entity.Property(e => e.AstHeadingId).HasColumnName("AST_HeadingID");
            entity.Property(e => e.AstId).HasColumnName("AST_ID");
            entity.Property(e => e.AstIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AST_IPAddress");
            entity.Property(e => e.AstItemId).HasColumnName("AST_ItemID");
            entity.Property(e => e.AstNotes).HasColumnName("AST_Notes");
            entity.Property(e => e.AstRecallby).HasColumnName("AST_RECALLBY");
            entity.Property(e => e.AstRecallon)
                .HasColumnType("datetime")
                .HasColumnName("AST_RECALLON");
            entity.Property(e => e.AstScheduleType).HasColumnName("AST_Schedule_type");
            entity.Property(e => e.AstStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("AST_STATUS");
            entity.Property(e => e.AstSubHeadingId).HasColumnName("AST_SubHeadingID");
            entity.Property(e => e.AstSubItemId).HasColumnName("AST_SubItemID");
            entity.Property(e => e.AstUpdatedby).HasColumnName("AST_UPDATEDBY");
            entity.Property(e => e.AstUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("AST_UPDATEDON");
            entity.Property(e => e.AstYearid).HasColumnName("AST_YEARId");
        });

        modelBuilder.Entity<AccSeperateSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_Seperate_Schedule");

            entity.Property(e => e.SsApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("SS_ApprovedOn");
            entity.Property(e => e.SsApprovedby).HasColumnName("SS_Approvedby");
            entity.Property(e => e.SsCompId).HasColumnName("SS_CompID");
            entity.Property(e => e.SsCrBy).HasColumnName("SS_CrBy");
            entity.Property(e => e.SsCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SS_CrOn");
            entity.Property(e => e.SsCustId).HasColumnName("SS_CustId");
            entity.Property(e => e.SsDate)
                .HasColumnType("datetime")
                .HasColumnName("SS_DATE");
            entity.Property(e => e.SsDelflag)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SS_Delflag");
            entity.Property(e => e.SsFinancialYear).HasColumnName("SS_FinancialYear");
            entity.Property(e => e.SsGroup).HasColumnName("SS_Group");
            entity.Property(e => e.SsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SS_IPAddress");
            entity.Property(e => e.SsOrgtype).HasColumnName("SS_Orgtype");
            entity.Property(e => e.SsParticulars)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SS_Particulars");
            entity.Property(e => e.SsPkid).HasColumnName("SS_PKID");
            entity.Property(e => e.SsStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SS_Status");
            entity.Property(e => e.SsUpdatedBy).HasColumnName("SS_UpdatedBy");
            entity.Property(e => e.SsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SS_UpdatedOn");
            entity.Property(e => e.SsValues)
                .HasColumnType("money")
                .HasColumnName("SS_Values");
        });

        modelBuilder.Entity<AccSubHeadingLedgerDesc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_SubHeadingLedgerDesc");

            entity.Property(e => e.AshlApprovedBy).HasColumnName("ASHL_ApprovedBy");
            entity.Property(e => e.AshlApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHL_ApprovedOn");
            entity.Property(e => e.AshlBranchId).HasColumnName("ASHL_BranchId");
            entity.Property(e => e.AshlCompId).HasColumnName("ASHL_CompID");
            entity.Property(e => e.AshlCreatedBy).HasColumnName("ASHL_CreatedBy");
            entity.Property(e => e.AshlCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHL_CreatedOn");
            entity.Property(e => e.AshlCustomerId).HasColumnName("ASHL_CustomerId");
            entity.Property(e => e.AshlDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ASHL_DelFlag");
            entity.Property(e => e.AshlDescription)
                .IsUnicode(false)
                .HasColumnName("ASHL_Description");
            entity.Property(e => e.AshlId).HasColumnName("ASHL_ID");
            entity.Property(e => e.AshlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASHL_IPAddress");
            entity.Property(e => e.AshlOperation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASHL_Operation");
            entity.Property(e => e.AshlStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASHL_Status");
            entity.Property(e => e.AshlSubHeadingId).HasColumnName("ASHL_SubHeadingId");
            entity.Property(e => e.AshlUpdatedBy).HasColumnName("ASHL_UpdatedBy");
            entity.Property(e => e.AshlUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHL_UpdatedOn");
            entity.Property(e => e.AshlYearId).HasColumnName("ASHL_YearID");
        });

        modelBuilder.Entity<AccSubHeadingNoteDesc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_SubHeadingNoteDesc");

            entity.Property(e => e.AshnApprovedBy).HasColumnName("ASHN_ApprovedBy");
            entity.Property(e => e.AshnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHN_ApprovedOn");
            entity.Property(e => e.AshnCompId).HasColumnName("ASHN_CompID");
            entity.Property(e => e.AshnCreatedBy).HasColumnName("ASHN_CreatedBy");
            entity.Property(e => e.AshnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHN_CreatedOn");
            entity.Property(e => e.AshnCustomerId).HasColumnName("ASHN_CustomerId");
            entity.Property(e => e.AshnDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ASHN_DelFlag");
            entity.Property(e => e.AshnDescription)
                .IsUnicode(false)
                .HasColumnName("ASHN_Description");
            entity.Property(e => e.AshnId).HasColumnName("ASHN_ID");
            entity.Property(e => e.AshnIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASHN_IPAddress");
            entity.Property(e => e.AshnOperation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ASHN_Operation");
            entity.Property(e => e.AshnStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASHN_Status");
            entity.Property(e => e.AshnSubHeadingId).HasColumnName("ASHN_SubHeadingId");
            entity.Property(e => e.AshnUpdatedBy).HasColumnName("ASHN_UpdatedBy");
            entity.Property(e => e.AshnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASHN_UpdatedOn");
            entity.Property(e => e.AshnYearId).HasColumnName("ASHN_YearID");
        });

        modelBuilder.Entity<AccTradeUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_Trade_Upload");

            entity.Property(e => e.AtuApprovedby).HasColumnName("ATU_APPROVEDBY");
            entity.Property(e => e.AtuApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ATU_APPROVEDON");
            entity.Property(e => e.AtuBranchid).HasColumnName("ATU_Branchid");
            entity.Property(e => e.AtuCategory).HasColumnName("ATU_Category");
            entity.Property(e => e.AtuCrby).HasColumnName("ATU_CRBY");
            entity.Property(e => e.AtuCron)
                .HasColumnType("datetime")
                .HasColumnName("ATU_CRON");
            entity.Property(e => e.AtuCustId).HasColumnName("ATU_CustId");
            entity.Property(e => e.AtuId).HasColumnName("ATU_ID");
            entity.Property(e => e.AtuIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATU_IPAddress");
            entity.Property(e => e.AtuLessThanSixMonth)
                .HasColumnType("money")
                .HasColumnName("ATU_Less_than_six_Month");
            entity.Property(e => e.AtuMoreThan)
                .HasColumnType("money")
                .HasColumnName("ATU_More_than");
            entity.Property(e => e.AtuMoreThanSixMonth)
                .HasColumnType("money")
                .HasColumnName("ATU_More_than_six_Month");
            entity.Property(e => e.AtuName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATU_Name");
            entity.Property(e => e.AtuOneYear)
                .HasColumnType("money")
                .HasColumnName("ATU_One_Year");
            entity.Property(e => e.AtuOtherType).HasColumnName("ATU_OtherType");
            entity.Property(e => e.AtuThreeYear)
                .HasColumnType("money")
                .HasColumnName("ATU_Three_Year");
            entity.Property(e => e.AtuTotalAmount)
                .HasColumnType("money")
                .HasColumnName("ATU_Total_Amount");
            entity.Property(e => e.AtuTwoYear)
                .HasColumnType("money")
                .HasColumnName("ATU_Two_Year");
            entity.Property(e => e.AtuUpdatedby).HasColumnName("ATU_UPDATEDBY");
            entity.Property(e => e.AtuUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATU_UPDATEDON");
            entity.Property(e => e.AtuYearid).HasColumnName("ATU_YEARId");
        });

        modelBuilder.Entity<AccTrailBalanceCustomerUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_TrailBalance_CustomerUpload");

            entity.Property(e => e.AtbcuApprovedby).HasColumnName("ATBCU_APPROVEDBY");
            entity.Property(e => e.AtbcuApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBCU_APPROVEDON");
            entity.Property(e => e.AtbcuBranchid).HasColumnName("ATBCU_Branchid");
            entity.Property(e => e.AtbcuClosingCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Closing_Credit_Amount");
            entity.Property(e => e.AtbcuClosingDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Closing_Debit_Amount");
            entity.Property(e => e.AtbcuClosingTotalCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Closing_TotalCredit_Amount");
            entity.Property(e => e.AtbcuClosingTotalDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Closing_TotalDebit_Amount");
            entity.Property(e => e.AtbcuCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATBCU_CODE");
            entity.Property(e => e.AtbcuCompId).HasColumnName("ATBCU_CompId");
            entity.Property(e => e.AtbcuCrby).HasColumnName("ATBCU_CRBY");
            entity.Property(e => e.AtbcuCron)
                .HasColumnType("datetime")
                .HasColumnName("ATBCU_CRON");
            entity.Property(e => e.AtbcuCustId).HasColumnName("ATBCU_CustId");
            entity.Property(e => e.AtbcuDeletedby).HasColumnName("ATBCU_DELETEDBY");
            entity.Property(e => e.AtbcuDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBCU_DELETEDON");
            entity.Property(e => e.AtbcuDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATBCU_DELFLG");
            entity.Property(e => e.AtbcuDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ATBCU_Description");
            entity.Property(e => e.AtbcuId).HasColumnName("ATBCU_ID");
            entity.Property(e => e.AtbcuIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATBCU_IPAddress");
            entity.Property(e => e.AtbcuMasId).HasColumnName("ATBCU_MasId");
            entity.Property(e => e.AtbcuOpeningCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Opening_Credit_Amount");
            entity.Property(e => e.AtbcuOpeningDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_Opening_Debit_Amount");
            entity.Property(e => e.AtbcuProgress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ATBCU_Progress");
            entity.Property(e => e.AtbcuQuarterId).HasColumnName("ATBCU_QuarterId");
            entity.Property(e => e.AtbcuRecallby).HasColumnName("ATBCU_RECALLBY");
            entity.Property(e => e.AtbcuRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ATBCU_RECALLON");
            entity.Property(e => e.AtbcuStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ATBCU_STATUS");
            entity.Property(e => e.AtbcuTrCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_TR_Credit_Amount");
            entity.Property(e => e.AtbcuTrDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBCU_TR_Debit_Amount");
            entity.Property(e => e.AtbcuUpdatedby).HasColumnName("ATBCU_UPDATEDBY");
            entity.Property(e => e.AtbcuUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBCU_UPDATEDON");
            entity.Property(e => e.AtbcuYearid).HasColumnName("ATBCU_YEARId");
        });

        modelBuilder.Entity<AccTrailBalanceUpload>(entity =>
        {
            entity.HasKey(e => e.AtbuId);

            entity.ToTable("Acc_TrailBalance_Upload");

            entity.Property(e => e.AtbuId)
                .ValueGeneratedNever()
                .HasColumnName("ATBU_ID");
            entity.Property(e => e.AtbuApprovedby).HasColumnName("ATBU_APPROVEDBY");
            entity.Property(e => e.AtbuApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBU_APPROVEDON");
            entity.Property(e => e.AtbuBranchid).HasColumnName("ATBU_Branchid");
            entity.Property(e => e.AtbuClosingCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Closing_Credit_Amount");
            entity.Property(e => e.AtbuClosingDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Closing_Debit_Amount");
            entity.Property(e => e.AtbuClosingTotalCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Closing_TotalCredit_Amount");
            entity.Property(e => e.AtbuClosingTotalDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Closing_TotalDebit_Amount");
            entity.Property(e => e.AtbuCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATBU_CODE");
            entity.Property(e => e.AtbuCompId).HasColumnName("ATBU_CompId");
            entity.Property(e => e.AtbuCrby).HasColumnName("ATBU_CRBY");
            entity.Property(e => e.AtbuCron)
                .HasColumnType("datetime")
                .HasColumnName("ATBU_CRON");
            entity.Property(e => e.AtbuCustId).HasColumnName("ATBU_CustId");
            entity.Property(e => e.AtbuDeletedby).HasColumnName("ATBU_DELETEDBY");
            entity.Property(e => e.AtbuDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBU_DELETEDON");
            entity.Property(e => e.AtbuDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATBU_DELFLG");
            entity.Property(e => e.AtbuDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ATBU_Description");
            entity.Property(e => e.AtbuIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATBU_IPAddress");
            entity.Property(e => e.AtbuOpeningCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Opening_Credit_Amount");
            entity.Property(e => e.AtbuOpeningDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_Opening_Debit_Amount");
            entity.Property(e => e.AtbuProgress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ATBU_Progress");
            entity.Property(e => e.AtbuQuarterId).HasColumnName("ATBU_QuarterId");
            entity.Property(e => e.AtbuRecallby).HasColumnName("ATBU_RECALLBY");
            entity.Property(e => e.AtbuRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ATBU_RECALLON");
            entity.Property(e => e.AtbuStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ATBU_STATUS");
            entity.Property(e => e.AtbuTrCreditAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_TR_Credit_Amount");
            entity.Property(e => e.AtbuTrDebitAmount)
                .HasColumnType("money")
                .HasColumnName("ATBU_TR_Debit_Amount");
            entity.Property(e => e.AtbuUpdatedby).HasColumnName("ATBU_UPDATEDBY");
            entity.Property(e => e.AtbuUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBU_UPDATEDON");
            entity.Property(e => e.AtbuYearid).HasColumnName("ATBU_YEARId");
        });

        modelBuilder.Entity<AccTrailBalanceUploadDetail>(entity =>
        {
            entity.HasKey(e => e.AtbudId);

            entity.ToTable("Acc_TrailBalance_Upload_Details");

            entity.Property(e => e.AtbudId)
                .ValueGeneratedNever()
                .HasColumnName("ATBUD_ID");
            entity.Property(e => e.AtbudApprovedby).HasColumnName("ATBUD_APPROVEDBY");
            entity.Property(e => e.AtbudApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBUD_APPROVEDON");
            entity.Property(e => e.AtbudBranchnameid).HasColumnName("Atbud_Branchnameid");
            entity.Property(e => e.AtbudCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATBUD_CODE");
            entity.Property(e => e.AtbudCompId).HasColumnName("ATBUD_CompId");
            entity.Property(e => e.AtbudCompanyType).HasColumnName("ATBUD_Company_Type");
            entity.Property(e => e.AtbudCrby).HasColumnName("ATBUD_CRBY");
            entity.Property(e => e.AtbudCron)
                .HasColumnType("datetime")
                .HasColumnName("ATBUD_CRON");
            entity.Property(e => e.AtbudCustId).HasColumnName("ATBUD_CustId");
            entity.Property(e => e.AtbudDeletedby).HasColumnName("ATBUD_DELETEDBY");
            entity.Property(e => e.AtbudDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBUD_DELETEDON");
            entity.Property(e => e.AtbudDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATBUD_DELFLG");
            entity.Property(e => e.AtbudDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ATBUD_Description");
            entity.Property(e => e.AtbudHeadingid).HasColumnName("ATBUD_Headingid");
            entity.Property(e => e.AtbudIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATBUD_IPAddress");
            entity.Property(e => e.AtbudItemid).HasColumnName("ATBUD_itemid");
            entity.Property(e => e.AtbudMasid).HasColumnName("ATBUD_Masid");
            entity.Property(e => e.AtbudProgress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATBUD_Progress");
            entity.Property(e => e.AtbudQuarterId).HasColumnName("ATBUD_QuarterId");
            entity.Property(e => e.AtbudRecallby).HasColumnName("ATBUD_RECALLBY");
            entity.Property(e => e.AtbudRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ATBUD_RECALLON");
            entity.Property(e => e.AtbudScheduleType).HasColumnName("ATBUD_SChedule_Type");
            entity.Property(e => e.AtbudStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ATBUD_STATUS");
            entity.Property(e => e.AtbudSubItemId).HasColumnName("ATBUD_SubItemId");
            entity.Property(e => e.AtbudSubheading).HasColumnName("ATBUD_Subheading");
            entity.Property(e => e.AtbudUpdatedby).HasColumnName("ATBUD_UPDATEDBY");
            entity.Property(e => e.AtbudUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATBUD_UPDATEDON");
            entity.Property(e => e.AtbudYearid).HasColumnName("ATBUD_YEARId");

            entity.HasOne(d => d.AtbudMas).WithMany(p => p.AccTrailBalanceUploadDetails)
                .HasForeignKey(d => d.AtbudMasid)
                .HasConstraintName("FK_Acc_TrailBalance_Upload_Details_Acc_TrailBalance_Upload");
        });

        modelBuilder.Entity<AccTransactionsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Acc_Transactions_Details");

            entity.Property(e => e.AtdBillName)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATD_BillName");
            entity.Property(e => e.AtdBranchId).HasColumnName("ATD_BranchID");
            entity.Property(e => e.AtdClosingCredit)
                .HasColumnType("money")
                .HasColumnName("ATD_ClosingCredit");
            entity.Property(e => e.AtdClosingDebit)
                .HasColumnType("money")
                .HasColumnName("ATD_ClosingDebit");
            entity.Property(e => e.AtdCompId).HasColumnName("ATD_CompID");
            entity.Property(e => e.AtdCreatedBy).HasColumnName("ATD_CreatedBy");
            entity.Property(e => e.AtdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ATD_CreatedOn");
            entity.Property(e => e.AtdCredit)
                .HasColumnType("money")
                .HasColumnName("ATD_Credit");
            entity.Property(e => e.AtdCustId).HasColumnName("atd_custId");
            entity.Property(e => e.AtdDbOrCr).HasColumnName("ATD_DbOrCr");
            entity.Property(e => e.AtdDebit)
                .HasColumnType("money")
                .HasColumnName("ATD_Debit");
            entity.Property(e => e.AtdDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATD_Delflag");
            entity.Property(e => e.AtdId).HasColumnName("ATD_ID");
            entity.Property(e => e.AtdIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ATD_IPAddress");
            entity.Property(e => e.AtdLedgerId).HasColumnName("ATD_LedgerId");
            entity.Property(e => e.AtdOpenCredit)
                .HasColumnType("money")
                .HasColumnName("ATD_OpenCredit");
            entity.Property(e => e.AtdOpenDebit)
                .HasColumnType("money")
                .HasColumnName("ATD_OpenDebit");
            entity.Property(e => e.AtdOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATD_Operation");
            entity.Property(e => e.AtdOrgType).HasColumnName("ATD_OrgType");
            entity.Property(e => e.AtdSeqReferenceNum).HasColumnName("ATD_SeqReferenceNum");
            entity.Property(e => e.AtdStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ATD_Status");
            entity.Property(e => e.AtdTrType).HasColumnName("ATD_TrType");
            entity.Property(e => e.AtdTransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("ATD_TransactionDate");
            entity.Property(e => e.AtdYearId).HasColumnName("ATD_YearID");
        });

        modelBuilder.Entity<AccVoucherSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ACC_Voucher_Settings");

            entity.Property(e => e.AvsCompId).HasColumnName("AVS_CompId");
            entity.Property(e => e.AvsCreatedBy).HasColumnName("AVS_CreatedBy");
            entity.Property(e => e.AvsCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AVS_CreatedOn");
            entity.Property(e => e.AvsId).HasColumnName("AVS_Id");
            entity.Property(e => e.AvsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AVS_IPAddress");
            entity.Property(e => e.AvsOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("AVS_Operation");
            entity.Property(e => e.AvsPrefix)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("AVS_Prefix");
            entity.Property(e => e.AvsSntotal).HasColumnName("AVS_SNTotal");
            entity.Property(e => e.AvsTransType).HasColumnName("AVS_TransType");
            entity.Property(e => e.AvsUpdatedBy).HasColumnName("AVS_UpdatedBy");
            entity.Property(e => e.AvsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AVS_UpdatedOn");
        });

        modelBuilder.Entity<AccYearMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("acc_Year_Master");

            entity.Property(e => e.YmsApprovedBy).HasColumnName("YMS_ApprovedBy");
            entity.Property(e => e.YmsApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_ApprovedOn");
            entity.Property(e => e.YmsCompId).HasColumnName("YMS_CompID");
            entity.Property(e => e.YmsCrby).HasColumnName("YMS_CRBY");
            entity.Property(e => e.YmsCron)
                .HasColumnType("datetime")
                .HasColumnName("YMS_CRON");
            entity.Property(e => e.YmsDefault).HasColumnName("YMS_Default");
            entity.Property(e => e.YmsDelFlag)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("YMS_DelFlag");
            entity.Property(e => e.YmsDeletedBy).HasColumnName("YMS_DeletedBy");
            entity.Property(e => e.YmsFreezeYear)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("yms_FreezeYear");
            entity.Property(e => e.YmsFromYear).HasColumnName("YMS_FROM_YEAR");
            entity.Property(e => e.YmsFromdate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_FROMDATE");
            entity.Property(e => e.YmsId).HasColumnName("YMS_ID");
            entity.Property(e => e.YmsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("YMS_IPAddress");
            entity.Property(e => e.YmsOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("YMS_Operation");
            entity.Property(e => e.YmsReCalledBy).HasColumnName("YMS_ReCalledBy");
            entity.Property(e => e.YmsStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("YMS_Status");
            entity.Property(e => e.YmsToYear).HasColumnName("YMS_TO_YEAR");
            entity.Property(e => e.YmsTodate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_TODATE");
            entity.Property(e => e.YmsUpdatedBy).HasColumnName("YMS_UpdatedBy");
            entity.Property(e => e.YmsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_UpdatedOn");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Account");

            entity.Property(e => e.Aaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AAddress");
            entity.Property(e => e.Acode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ACode");
            entity.Property(e => e.AemailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AEmailID");
            entity.Property(e => e.Aid).HasColumnName("AID");
            entity.Property(e => e.AmoblieNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AMoblieNo");
            entity.Property(e => e.Aname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AName");
        });

        modelBuilder.Entity<AdoBatch>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ADO_Batch");

            entity.Property(e => e.BtAttachId).HasColumnName("BT_AttachID");
            entity.Property(e => e.BtBatchId).HasColumnName("BT_BatchID");
            entity.Property(e => e.BtBatchNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BT_BatchNo");
            entity.Property(e => e.BtComments)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("BT_Comments");
            entity.Property(e => e.BtCompId).HasColumnName("BT_CompID");
            entity.Property(e => e.BtCrBy).HasColumnName("BT_CrBy");
            entity.Property(e => e.BtCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BT_CrOn");
            entity.Property(e => e.BtCreditTotal)
                .HasColumnType("money")
                .HasColumnName("BT_CreditTotal");
            entity.Property(e => e.BtCustomerId).HasColumnName("BT_CustomerID");
            entity.Property(e => e.BtDate)
                .HasColumnType("datetime")
                .HasColumnName("BT_Date");
            entity.Property(e => e.BtDebitTotal)
                .HasColumnType("money")
                .HasColumnName("BT_DebitTotal");
            entity.Property(e => e.BtDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("BT_Delflag");
            entity.Property(e => e.BtId).HasColumnName("BT_ID");
            entity.Property(e => e.BtIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BT_IPAddress");
            entity.Property(e => e.BtNft).HasColumnName("BT_NFT");
            entity.Property(e => e.BtStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BT_Status");
            entity.Property(e => e.BtTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BT_Title");
            entity.Property(e => e.BtTransactionType).HasColumnName("BT_TransactionType");
            entity.Property(e => e.BtVouchers)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BT_Vouchers");
            entity.Property(e => e.BtYearId).HasColumnName("BT_YearID");
        });

        modelBuilder.Entity<Ajax1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ajax1");

            entity.Property(e => e.Browser)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("browser");
            entity.Property(e => e.Engine)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("engine");
            entity.Property(e => e.Grade)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("grade");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Marketshare)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("marketshare");
            entity.Property(e => e.Platform1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("platform1");
            entity.Property(e => e.Released)
                .HasColumnType("datetime")
                .HasColumnName("released");
            entity.Property(e => e.Version1).HasColumnName("version1");
        });

        modelBuilder.Entity<AuditAnnualPlan>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_AnnualPlan");

            entity.Property(e => e.AapComments)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAP_Comments");
            entity.Property(e => e.AapCompId).HasColumnName("AAP_CompID");
            entity.Property(e => e.AapCrby).HasColumnName("AAP_Crby");
            entity.Property(e => e.AapCron)
                .HasColumnType("datetime")
                .HasColumnName("AAP_Cron");
            entity.Property(e => e.AapCustId).HasColumnName("AAP_CustID");
            entity.Property(e => e.AapFunId).HasColumnName("AAP_FunID");
            entity.Property(e => e.AapIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAP_IPAddress");
            entity.Property(e => e.AapMonthId).HasColumnName("AAP_MonthID");
            entity.Property(e => e.AapPkid).HasColumnName("AAP_PKID");
            entity.Property(e => e.AapResourceId)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAP_ResourceID");
            entity.Property(e => e.AapUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AAP_UpdatedOn");
            entity.Property(e => e.AapUpdatedby).HasColumnName("AAP_Updatedby");
            entity.Property(e => e.AapYearId).HasColumnName("AAP_YearID");
        });

        modelBuilder.Entity<AuditApmAssignmentDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_APM_Assignment_Details");

            entity.Property(e => e.AapmAuditCodeId).HasColumnName("AAPM_AuditCodeID");
            entity.Property(e => e.AapmAuditTaskId).HasColumnName("AAPM_AuditTaskID");
            entity.Property(e => e.AapmAuditTaskType)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAPM_AuditTaskType");
            entity.Property(e => e.AapmCompId).HasColumnName("AAPM_CompID");
            entity.Property(e => e.AapmCrBy).HasColumnName("AAPM_CrBy");
            entity.Property(e => e.AapmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAPM_CrOn");
            entity.Property(e => e.AapmCustId).HasColumnName("AAPM_CustID");
            entity.Property(e => e.AapmFunctionId).HasColumnName("AAPM_FunctionID");
            entity.Property(e => e.AapmId).HasColumnName("AAPM_ID");
            entity.Property(e => e.AapmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAPM_IPAddress");
            entity.Property(e => e.AapmPendDate)
                .HasColumnType("datetime")
                .HasColumnName("AAPM_PEndDate");
            entity.Property(e => e.AapmPstartDate)
                .HasColumnType("datetime")
                .HasColumnName("AAPM_PStartDate");
            entity.Property(e => e.AapmResource)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAPM_Resource");
            entity.Property(e => e.AapmUpdatedBy).HasColumnName("AAPM_UpdatedBy");
            entity.Property(e => e.AapmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AAPM_UpdatedOn");
            entity.Property(e => e.AapmYearId).HasColumnName("AAPM_YearID");
        });

        modelBuilder.Entity<AuditApmChecksMatrix>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_APM_ChecksMatrix");

            entity.Property(e => e.ApmcmApmpkid).HasColumnName("APMCM_APMPKID");
            entity.Property(e => e.ApmcmChecksId).HasColumnName("APMCM_ChecksID");
            entity.Property(e => e.ApmcmCompId).HasColumnName("APMCM_CompID");
            entity.Property(e => e.ApmcmControlId).HasColumnName("APMCM_ControlID");
            entity.Property(e => e.ApmcmCustId).HasColumnName("APMCM_CustID");
            entity.Property(e => e.ApmcmFunctionId).HasColumnName("APMCM_FunctionID");
            entity.Property(e => e.ApmcmIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("APMCM_IPAddress");
            entity.Property(e => e.ApmcmMmmid).HasColumnName("APMCM_MMMID");
            entity.Property(e => e.ApmcmPkid).HasColumnName("APMCM_PKID");
            entity.Property(e => e.ApmcmProcessId).HasColumnName("APMCM_ProcessID");
            entity.Property(e => e.ApmcmRiskId).HasColumnName("APMCM_RiskID");
            entity.Property(e => e.ApmcmStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("APMCM_Status");
            entity.Property(e => e.ApmcmSubFunctionId).HasColumnName("APMCM_SubFunctionID");
            entity.Property(e => e.ApmcmSubProcessId).HasColumnName("APMCM_SubProcessID");
            entity.Property(e => e.ApmcmYearId).HasColumnName("APMCM_YearID");
        });

        modelBuilder.Entity<AuditApmDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_APM_Details");

            entity.Property(e => e.ApmApmcrstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APM_APMCRStatus");
            entity.Property(e => e.ApmApmstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APM_APMStatus");
            entity.Property(e => e.ApmApmtastatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APM_APMTAStatus");
            entity.Property(e => e.ApmAppBy).HasColumnName("APM_AppBy");
            entity.Property(e => e.ApmAppOn)
                .HasColumnType("datetime")
                .HasColumnName("APM_AppOn");
            entity.Property(e => e.ApmAttachId).HasColumnName("APM_AttachID");
            entity.Property(e => e.ApmAuditCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APM_AuditCode");
            entity.Property(e => e.ApmAuditConfirm).HasColumnName("APM_Audit_Confirm");
            entity.Property(e => e.ApmAuditConfirmYes)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_Audit_Confirm_Yes");
            entity.Property(e => e.ApmAuditTeamsId)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_AuditTeamsID");
            entity.Property(e => e.ApmAuditorsRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_AuditorsRemarks");
            entity.Property(e => e.ApmAuditorsRoleId).HasColumnName("APM_AuditorsRoleID");
            entity.Property(e => e.ApmBody)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("APM_Body");
            entity.Property(e => e.ApmBranchId).HasColumnName("APM_BranchID");
            entity.Property(e => e.ApmCcemail)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_CCEmail");
            entity.Property(e => e.ApmCompId).HasColumnName("APM_CompID");
            entity.Property(e => e.ApmCrBy).HasColumnName("APM_CrBy");
            entity.Property(e => e.ApmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("APM_CrOn");
            entity.Property(e => e.ApmCustId).HasColumnName("APM_CustID");
            entity.Property(e => e.ApmCustomerRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_CustomerRemarks");
            entity.Property(e => e.ApmEstimatedEffortDays).HasColumnName("APM_EstimatedEffortDays");
            entity.Property(e => e.ApmFunctionId).HasColumnName("APM_FunctionID");
            entity.Property(e => e.ApmId).HasColumnName("APM_ID");
            entity.Property(e => e.ApmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("APM_IPAddress");
            entity.Property(e => e.ApmObjectives)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_Objectives");
            entity.Property(e => e.ApmPartnersId)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_PartnersID");
            entity.Property(e => e.ApmPgedetailId).HasColumnName("APM_PGEDetailId");
            entity.Property(e => e.ApmStatusId).HasColumnName("APM_StatusID");
            entity.Property(e => e.ApmSubject)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("APM_Subject");
            entity.Property(e => e.ApmTendDate)
                .HasColumnType("datetime")
                .HasColumnName("APM_TEndDate");
            entity.Property(e => e.ApmToemail)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("APM_TOEmail");
            entity.Property(e => e.ApmTstartDate)
                .HasColumnType("datetime")
                .HasColumnName("APM_TStartDate");
            entity.Property(e => e.ApmUpdatedBy).HasColumnName("APM_UpdatedBy");
            entity.Property(e => e.ApmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("APM_UpdatedOn");
            entity.Property(e => e.ApmYearId).HasColumnName("APM_YearID");
        });

        modelBuilder.Entity<AuditAra>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_ARA");

            entity.Property(e => e.AraAuditCodeId).HasColumnName("ARA_AuditCodeID");
            entity.Property(e => e.AraComments)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ARA_Comments");
            entity.Property(e => e.AraCompId).HasColumnName("ARA_CompID");
            entity.Property(e => e.AraCrBy).HasColumnName("ARA_CrBy");
            entity.Property(e => e.AraCrOn)
                .HasColumnType("datetime")
                .HasColumnName("ARA_CrOn");
            entity.Property(e => e.AraCustId).HasColumnName("ARA_CustID");
            entity.Property(e => e.AraFinancialYear).HasColumnName("ARA_FinancialYear");
            entity.Property(e => e.AraFunId).HasColumnName("ARA_FunID");
            entity.Property(e => e.AraIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ARA_IPAddress");
            entity.Property(e => e.AraNetScore).HasColumnName("ARA_NetScore");
            entity.Property(e => e.AraPkid).HasColumnName("ARA_PKID");
            entity.Property(e => e.AraStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ARA_Status");
            entity.Property(e => e.AraSubmittedBy).HasColumnName("ARA_SubmittedBy");
            entity.Property(e => e.AraSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("ARA_SubmittedOn");
            entity.Property(e => e.AraUpdatedBy).HasColumnName("ARA_UpdatedBy");
            entity.Property(e => e.AraUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ARA_UpdatedOn");
        });

        modelBuilder.Entity<AuditAraDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_ARA_Details");

            entity.Property(e => e.AradArapkid).HasColumnName("ARAD_ARAPKID");
            entity.Property(e => e.AradChecksId).HasColumnName("ARAD_ChecksID");
            entity.Property(e => e.AradCompId).HasColumnName("ARAD_CompID");
            entity.Property(e => e.AradControlId).HasColumnName("ARAD_ControlID");
            entity.Property(e => e.AradControlRating).HasColumnName("ARAD_ControlRating");
            entity.Property(e => e.AradDes).HasColumnName("ARAD_DES");
            entity.Property(e => e.AradImpactId).HasColumnName("ARAD_ImpactID");
            entity.Property(e => e.AradIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ARAD_IPAddress");
            entity.Property(e => e.AradIssueHeading)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ARAD_IssueHeading");
            entity.Property(e => e.AradLikelihoodId).HasColumnName("ARAD_LikelihoodID");
            entity.Property(e => e.AradOes).HasColumnName("ARAD_OES");
            entity.Property(e => e.AradPkid).HasColumnName("ARAD_PKID");
            entity.Property(e => e.AradPmid).HasColumnName("ARAD_PMID");
            entity.Property(e => e.AradRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("ARAD_Remarks");
            entity.Property(e => e.AradResidualRiskRating).HasColumnName("ARAD_ResidualRiskRating");
            entity.Property(e => e.AradRiskId).HasColumnName("ARAD_RiskID");
            entity.Property(e => e.AradRiskRating).HasColumnName("ARAD_RiskRating");
            entity.Property(e => e.AradRiskTypeId).HasColumnName("ARAD_RiskTypeID");
            entity.Property(e => e.AradSemid).HasColumnName("ARAD_SEMID");
            entity.Property(e => e.AradSpmid).HasColumnName("ARAD_SPMID");
        });

        modelBuilder.Entity<AuditAssignmentEmpSubTask>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_EmpSubTask");

            entity.Property(e => e.AaestAasId).HasColumnName("AAEST_AAS_ID");
            entity.Property(e => e.AaestAastId).HasColumnName("AAEST_AAST_ID");
            entity.Property(e => e.AaestAttachId).HasColumnName("AAEST_AttachID");
            entity.Property(e => e.AaestComments)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAEST_Comments");
            entity.Property(e => e.AaestCompId).HasColumnName("AAEST_CompID");
            entity.Property(e => e.AaestCrBy).HasColumnName("AAEST_CrBy");
            entity.Property(e => e.AaestCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAEST_CrOn");
            entity.Property(e => e.AaestId).HasColumnName("AAEST_ID");
            entity.Property(e => e.AaestIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAEST_IPAddress");
            entity.Property(e => e.AaestWorkStatusId).HasColumnName("AAEST_WorkStatusID");
        });

        modelBuilder.Entity<AuditAssignmentInvoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_Invoice");

            entity.Property(e => e.AaiAuthorizedSignatory).HasColumnName("AAI_AuthorizedSignatory");
            entity.Property(e => e.AaiBillingEntityId).HasColumnName("AAI_BillingEntity_ID");
            entity.Property(e => e.AaiCompId).HasColumnName("AAI_CompID");
            entity.Property(e => e.AaiCrBy).HasColumnName("AAI_CrBy");
            entity.Property(e => e.AaiCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAI_CrOn");
            entity.Property(e => e.AaiCustId).HasColumnName("AAI_Cust_ID");
            entity.Property(e => e.AaiId).HasColumnName("AAI_ID");
            entity.Property(e => e.AaiInvoiceNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AAI_InvoiceNo");
            entity.Property(e => e.AaiInvoiceTypeId).HasColumnName("AAI_InvoiceTypeID");
            entity.Property(e => e.AaiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAI_IPAddress");
            entity.Property(e => e.AaiNotes)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AAI_Notes");
            entity.Property(e => e.AaiTaxType1).HasColumnName("AAI_TaxType1");
            entity.Property(e => e.AaiTaxType1Percentage)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AAI_TaxType1Percentage");
            entity.Property(e => e.AaiTaxType2).HasColumnName("AAI_TaxType2");
            entity.Property(e => e.AaiTaxType2Percentage)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AAI_TaxType2Percentage");
            entity.Property(e => e.AaiYearId).HasColumnName("AAI_YearID");
        });

        modelBuilder.Entity<AuditAssignmentInvoiceDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_InvoiceDetails");

            entity.Property(e => e.AaidAaiId).HasColumnName("AAID_AAI_ID");
            entity.Property(e => e.AaidAasId).HasColumnName("AAID_AAS_ID");
            entity.Property(e => e.AaidCompId).HasColumnName("AAID_CompID");
            entity.Property(e => e.AaidCrBy).HasColumnName("AAID_CrBy");
            entity.Property(e => e.AaidCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAID_CrOn");
            entity.Property(e => e.AaidDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAID_Desc");
            entity.Property(e => e.AaidHsnsac).HasColumnName("AAID_HSNSAC");
            entity.Property(e => e.AaidId).HasColumnName("AAID_ID");
            entity.Property(e => e.AaidIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAID_IPAddress");
            entity.Property(e => e.AaidIsTaxable).HasColumnName("AAID_IsTaxable");
            entity.Property(e => e.AaidPricePerUnit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AAID_PricePerUnit");
            entity.Property(e => e.AaidQuantity).HasColumnName("AAID_Quantity");
        });

        modelBuilder.Entity<AuditAssignmentSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_Schedule");

            entity.Property(e => e.AasAdvancePartialBilling).HasColumnName("AAS_AdvancePartialBilling");
            entity.Property(e => e.AasAssessmentYearId)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AAS_AssessmentYearID");
            entity.Property(e => e.AasAssignmentNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AAS_AssignmentNo");
            entity.Property(e => e.AasAttachId).HasColumnName("AAS_AttachID");
            entity.Property(e => e.AasBillingType).HasColumnName("AAS_BillingType");
            entity.Property(e => e.AasCompId).HasColumnName("AAS_CompID");
            entity.Property(e => e.AasCrBy).HasColumnName("AAS_CrBy");
            entity.Property(e => e.AasCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAS_CrOn");
            entity.Property(e => e.AasCustId).HasColumnName("AAS_CustID");
            entity.Property(e => e.AasFolderPath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AAS_FolderPath");
            entity.Property(e => e.AasId).HasColumnName("AAS_ID");
            entity.Property(e => e.AasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAS_IPAddress");
            entity.Property(e => e.AasIsComplianceAsg).HasColumnName("AAS_IsComplianceAsg");
            entity.Property(e => e.AasMonthId).HasColumnName("AAS_MonthID");
            entity.Property(e => e.AasPartnerId).HasColumnName("AAS_PartnerID");
            entity.Property(e => e.AasStatus).HasColumnName("AAS_Status");
            entity.Property(e => e.AasTaskId).HasColumnName("AAS_TaskID");
            entity.Property(e => e.AasUpdatedBy).HasColumnName("AAS_UpdatedBy");
            entity.Property(e => e.AasUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AAS_UpdatedOn");
            entity.Property(e => e.AasYearId).HasColumnName("AAS_YearID");
        });

        modelBuilder.Entity<AuditAssignmentSubTask>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_SubTask");

            entity.Property(e => e.AastAasId).HasColumnName("AAST_AAS_ID");
            entity.Property(e => e.AastAssistedByEmployeesId)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAST_AssistedByEmployeesID");
            entity.Property(e => e.AastAttachId).HasColumnName("AAST_AttachID");
            entity.Property(e => e.AastClosed).HasColumnName("AAST_Closed");
            entity.Property(e => e.AastCompId).HasColumnName("AAST_CompID");
            entity.Property(e => e.AastCrBy).HasColumnName("AAST_CrBy");
            entity.Property(e => e.AastCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AAST_CrOn");
            entity.Property(e => e.AastDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("AAST_Desc");
            entity.Property(e => e.AastDueDate)
                .HasColumnType("datetime")
                .HasColumnName("AAST_DueDate");
            entity.Property(e => e.AastEmployeeId).HasColumnName("AAST_EmployeeID");
            entity.Property(e => e.AastExpectedCompletionDate)
                .HasColumnType("datetime")
                .HasColumnName("AAST_ExpectedCompletionDate");
            entity.Property(e => e.AastFrequencyId).HasColumnName("AAST_FrequencyID");
            entity.Property(e => e.AastId).HasColumnName("AAST_ID");
            entity.Property(e => e.AastIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AAST_IPAddress");
            entity.Property(e => e.AastReview).HasColumnName("AAST_Review");
            entity.Property(e => e.AastSubTaskId).HasColumnName("AAST_SubTaskID");
            entity.Property(e => e.AastWorkStatusId).HasColumnName("AAST_WorkStatusID");
            entity.Property(e => e.AastYearOrMonthId).HasColumnName("AAST_YearOrMonthID");
        });

        modelBuilder.Entity<AuditAssignmentSubTaskMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignmentSubTask_Master");

            entity.Property(e => e.AmApprovedby).HasColumnName("AM_APPROVEDBY");
            entity.Property(e => e.AmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("AM_APPROVEDON");
            entity.Property(e => e.AmAuditAssignmentId).HasColumnName("AM_AuditAssignmentID");
            entity.Property(e => e.AmBillingTypeId).HasColumnName("AM_BillingTypeID");
            entity.Property(e => e.AmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("AM_CODE");
            entity.Property(e => e.AmCompId).HasColumnName("AM_CompId");
            entity.Property(e => e.AmCrby).HasColumnName("AM_CRBY");
            entity.Property(e => e.AmCron)
                .HasColumnType("datetime")
                .HasColumnName("AM_CRON");
            entity.Property(e => e.AmDeletedby).HasColumnName("AM_DELETEDBY");
            entity.Property(e => e.AmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("AM_DELETEDON");
            entity.Property(e => e.AmDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AM_DELFLG");
            entity.Property(e => e.AmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AM_Desc");
            entity.Property(e => e.AmId).HasColumnName("AM_ID");
            entity.Property(e => e.AmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AM_IPAddress");
            entity.Property(e => e.AmName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AM_Name");
            entity.Property(e => e.AmRecallby).HasColumnName("AM_RECALLBY");
            entity.Property(e => e.AmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("AM_RECALLON");
            entity.Property(e => e.AmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("AM_STATUS");
            entity.Property(e => e.AmUpdatedby).HasColumnName("AM_UPDATEDBY");
            entity.Property(e => e.AmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("AM_UPDATEDON");
        });

        modelBuilder.Entity<AuditAssignmentUserLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditAssignment_UserLog");

            entity.Property(e => e.AaulAasId).HasColumnName("AAUL_AAS_ID");
            entity.Property(e => e.AaulAasStatus).HasColumnName("AAUL_AAS_Status");
            entity.Property(e => e.AaulAdtKeyid).HasColumnName("AAUL_ADT_KEYID");
            entity.Property(e => e.AaulCompId).HasColumnName("AAUL_CompID");
            entity.Property(e => e.AaulDate)
                .HasColumnType("datetime")
                .HasColumnName("AAUL_Date");
            entity.Property(e => e.AaulId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AAUL_ID");
            entity.Property(e => e.AaulUserId).HasColumnName("AAUL_UserID");
        });

        modelBuilder.Entity<AuditCompletionSubPointMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditCompletion_SubPoint_Master");

            entity.Property(e => e.AsmApprovedby).HasColumnName("ASM_APPROVEDBY");
            entity.Property(e => e.AsmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ASM_APPROVEDON");
            entity.Property(e => e.AsmCheckpointId).HasColumnName("ASM_CheckpointID");
            entity.Property(e => e.AsmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ASM_Code");
            entity.Property(e => e.AsmCompId).HasColumnName("ASM_CompId");
            entity.Property(e => e.AsmCrby).HasColumnName("ASM_CRBY");
            entity.Property(e => e.AsmCron)
                .HasColumnType("datetime")
                .HasColumnName("ASM_CRON");
            entity.Property(e => e.AsmDeletedby).HasColumnName("ASM_DELETEDBY");
            entity.Property(e => e.AsmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ASM_DELETEDON");
            entity.Property(e => e.AsmDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASM_DELFLG");
            entity.Property(e => e.AsmId).HasColumnName("ASM_ID");
            entity.Property(e => e.AsmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASM_IPAddress");
            entity.Property(e => e.AsmRecallby).HasColumnName("ASM_RECALLBY");
            entity.Property(e => e.AsmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ASM_RECALLON");
            entity.Property(e => e.AsmRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ASM_Remarks");
            entity.Property(e => e.AsmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ASM_STATUS");
            entity.Property(e => e.AsmSubPoint)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ASM_SubPoint");
            entity.Property(e => e.AsmUpdatedby).HasColumnName("ASM_UPDATEDBY");
            entity.Property(e => e.AsmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ASM_UPDATEDON");
        });

        modelBuilder.Entity<AuditCostBudgetDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_CostBudgetDetails");

            entity.Property(e => e.CbdApprovedBy).HasColumnName("CBD_ApprovedBy");
            entity.Property(e => e.CbdApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBD_ApprovedOn");
            entity.Property(e => e.CbdAuditCodeId).HasColumnName("CBD_AuditCodeID");
            entity.Property(e => e.CbdCompId).HasColumnName("CBD_CompID");
            entity.Property(e => e.CbdCrBy).HasColumnName("CBD_CrBy");
            entity.Property(e => e.CbdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CBD_CrOn");
            entity.Property(e => e.CbdDescId).HasColumnName("CBD_DescID");
            entity.Property(e => e.CbdId).HasColumnName("CBD_ID");
            entity.Property(e => e.CbdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CBD_IPAddress");
            entity.Property(e => e.CbdPerHead).HasColumnName("CBD_Per_Head");
            entity.Property(e => e.CbdStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CBD_Status");
            entity.Property(e => e.CbdUpdateBy).HasColumnName("CBD_UpdateBy");
            entity.Property(e => e.CbdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBD_UpdatedOn");
            entity.Property(e => e.CbdUserId).HasColumnName("CBD_UserID");
            entity.Property(e => e.CbdYearId).HasColumnName("CBD_YearID");
        });

        modelBuilder.Entity<AuditCostSheetDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_CostSheetDetails");

            entity.Property(e => e.CsdApprovedBy).HasColumnName("CSD_ApprovedBy");
            entity.Property(e => e.CsdApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CSD_ApprovedOn");
            entity.Property(e => e.CsdAuditCodeId).HasColumnName("CSD_AuditCodeID");
            entity.Property(e => e.CsdComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CSD_Comments");
            entity.Property(e => e.CsdCompId).HasColumnName("CSD_CompID");
            entity.Property(e => e.CsdCosts).HasColumnName("CSD_Costs");
            entity.Property(e => e.CsdCrBy).HasColumnName("CSD_CrBy");
            entity.Property(e => e.CsdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CSD_CrOn");
            entity.Property(e => e.CsdCustId).HasColumnName("CSD_CustID");
            entity.Property(e => e.CsdDate)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CSD_Date");
            entity.Property(e => e.CsdDescId).HasColumnName("CSD_DescID");
            entity.Property(e => e.CsdFunId).HasColumnName("CSD_FunID");
            entity.Property(e => e.CsdId).HasColumnName("CSD_ID");
            entity.Property(e => e.CsdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CSD_IPAddress");
            entity.Property(e => e.CsdKmsTravelled).HasColumnName("CSD_KmsTravelled");
            entity.Property(e => e.CsdStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CSD_Status");
            entity.Property(e => e.CsdUpdateBy).HasColumnName("CSD_UpdateBy");
            entity.Property(e => e.CsdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CSD_UpdatedOn");
            entity.Property(e => e.CsdUserId).HasColumnName("CSD_UserID");
            entity.Property(e => e.CsdYearId).HasColumnName("CSD_YearID");
        });

        modelBuilder.Entity<AuditDocRequestList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Doc_Request_List");

            entity.Property(e => e.DrlCompId).HasColumnName("DRL_CompID");
            entity.Property(e => e.DrlCrBy).HasColumnName("DRL_CrBy");
            entity.Property(e => e.DrlCron)
                .HasColumnType("datetime")
                .HasColumnName("DRL_CROn");
            entity.Property(e => e.DrlDescription)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("DRL_Description");
            entity.Property(e => e.DrlDocTypeId).HasColumnName("DRL_DocTypeID");
            entity.Property(e => e.DrlDocumentType)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DRL_DocumentType");
            entity.Property(e => e.DrlDrlid).HasColumnName("DRL_DRLID");
            entity.Property(e => e.DrlDtype)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DRL_DType");
            entity.Property(e => e.DrlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DRL_IPAddress");
            entity.Property(e => e.DrlName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("DRL_Name");
            entity.Property(e => e.DrlSampleId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DRL_SampleId");
            entity.Property(e => e.DrlSize)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DRL_Size");
            entity.Property(e => e.DrlStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DRL_Status");
            entity.Property(e => e.DrlUpdatedBy).HasColumnName("DRL_UpdatedBy");
            entity.Property(e => e.DrlUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("DRL_UpdatedOn");
        });

        modelBuilder.Entity<AuditDrllog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_DRLLog");

            entity.Property(e => e.AdrlAttachId).HasColumnName("ADRL_AttachID");
            entity.Property(e => e.AdrlAttchDocId).HasColumnName("ADRL_AttchDocId");
            entity.Property(e => e.AdrlAuditNo).HasColumnName("ADRL_AuditNo");
            entity.Property(e => e.AdrlComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ADRL_Comments");
            entity.Property(e => e.AdrlCompId).HasColumnName("ADRL_CompID");
            entity.Property(e => e.AdrlCrBy).HasColumnName("ADRL_CrBy");
            entity.Property(e => e.AdrlCrOn)
                .HasColumnType("datetime")
                .HasColumnName("ADRL_CrOn");
            entity.Property(e => e.AdrlCustId).HasColumnName("ADRL_CustID");
            entity.Property(e => e.AdrlEmailId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADRL_EmailID");
            entity.Property(e => e.AdrlFunId).HasColumnName("ADRL_FunID");
            entity.Property(e => e.AdrlId).HasColumnName("ADRL_ID");
            entity.Property(e => e.AdrlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ADRL_IPAddress");
            entity.Property(e => e.AdrlLogStatus).HasColumnName("ADRL_LogStatus");
            entity.Property(e => e.AdrlReceivedComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ADRL_ReceivedComments");
            entity.Property(e => e.AdrlReceivedOn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ADRL_ReceivedOn");
            entity.Property(e => e.AdrlReportType).HasColumnName("ADRL_ReportType");
            entity.Property(e => e.AdrlRequestedListId).HasColumnName("ADRL_RequestedListID");
            entity.Property(e => e.AdrlRequestedOn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ADRL_RequestedOn");
            entity.Property(e => e.AdrlRequestedTypeId).HasColumnName("ADRL_RequestedTypeID");
            entity.Property(e => e.AdrlStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ADRL_Status");
            entity.Property(e => e.AdrlTimlinetoResOn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ADRL_TimlinetoResOn");
            entity.Property(e => e.AdrlUpdatedBy).HasColumnName("ADRL_UpdatedBy");
            entity.Property(e => e.AdrlUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ADRL_UpdatedOn");
            entity.Property(e => e.AdrlYearId).HasColumnName("ADRL_YearID");
        });

        modelBuilder.Entity<AuditExcelUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Excel_Upload");

            entity.Property(e => e.AeuApprovedby).HasColumnName("AEU_APPROVEDBY");
            entity.Property(e => e.AeuApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("AEU_APPROVEDON");
            entity.Property(e => e.AeuAttachmentId).HasColumnName("AEU_AttachmentId");
            entity.Property(e => e.AeuAuditId).HasColumnName("AEU_AuditId");
            entity.Property(e => e.AeuAuditTypeId).HasColumnName("AEU_AuditTypeId");
            entity.Property(e => e.AeuCcamount)
                .HasColumnType("money")
                .HasColumnName("AEU_CCAmount");
            entity.Property(e => e.AeuCdamount)
                .HasColumnType("money")
                .HasColumnName("AEU_CDAmount");
            entity.Property(e => e.AeuClientComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AEU_ClientComments");
            entity.Property(e => e.AeuCompId).HasColumnName("AEU_CompId");
            entity.Property(e => e.AeuCrby).HasColumnName("AEU_CRBY");
            entity.Property(e => e.AeuCron)
                .HasColumnType("datetime")
                .HasColumnName("AEU_CRON");
            entity.Property(e => e.AeuCustId).HasColumnName("AEU_CustId");
            entity.Property(e => e.AeuDeletedby).HasColumnName("AEU_DELETEDBY");
            entity.Property(e => e.AeuDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("AEU_DELETEDON");
            entity.Property(e => e.AeuDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AEU_DELFLG");
            entity.Property(e => e.AeuDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AEU_Description");
            entity.Property(e => e.AeuId).HasColumnName("AEU_ID");
            entity.Property(e => e.AeuIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AEU_IPAddress");
            entity.Property(e => e.AeuObservation)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AEU_Observation");
            entity.Property(e => e.AeuOcamount)
                .HasColumnType("money")
                .HasColumnName("AEU_OCAmount");
            entity.Property(e => e.AeuOdamount)
                .HasColumnType("money")
                .HasColumnName("AEU_ODAmount");
            entity.Property(e => e.AeuRecallby).HasColumnName("AEU_RECALLBY");
            entity.Property(e => e.AeuRecallon)
                .HasColumnType("datetime")
                .HasColumnName("AEU_RECALLON");
            entity.Property(e => e.AeuReviewerObservation)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AEU_ReviewerObservation");
            entity.Property(e => e.AeuStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("AEU_STATUS");
            entity.Property(e => e.AeuTrcamount)
                .HasColumnType("money")
                .HasColumnName("AEU_TRCAmount");
            entity.Property(e => e.AeuTrdamount)
                .HasColumnType("money")
                .HasColumnName("AEU_TRDAmount");
            entity.Property(e => e.AeuUpdatedby).HasColumnName("AEU_UPDATEDBY");
            entity.Property(e => e.AeuUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("AEU_UPDATEDON");
            entity.Property(e => e.AeuYearid).HasColumnName("AEU_YEARId");
        });

        modelBuilder.Entity<AuditExecutiveSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_ExecutiveSummary");

            entity.Property(e => e.AesActualPeriodEndDate)
                .HasColumnType("datetime")
                .HasColumnName("AES_ActualPeriodEndDate");
            entity.Property(e => e.AesActualPeriodStartDate)
                .HasColumnType("datetime")
                .HasColumnName("AES_ActualPeriodStartDate");
            entity.Property(e => e.AesAttchId).HasColumnName("AES_AttchID");
            entity.Property(e => e.AesAuditCode).HasColumnName("AES_AuditCode");
            entity.Property(e => e.AesAuditPeriodEndDate)
                .HasColumnType("datetime")
                .HasColumnName("AES_AuditPeriodEndDate");
            entity.Property(e => e.AesAuditPeriodStartDate)
                .HasColumnType("datetime")
                .HasColumnName("AES_AuditPeriodStartDate");
            entity.Property(e => e.AesAuditRating)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_AuditRating");
            entity.Property(e => e.AesAuditRatingId).HasColumnName("AES_AuditRatingID");
            entity.Property(e => e.AesAuditRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_AuditRemarks");
            entity.Property(e => e.AesAuditScope)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_AuditScope");
            entity.Property(e => e.AesAuditScopeOut)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_AuditScopeOut");
            entity.Property(e => e.AesBusinessOverview)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_BusinessOverview");
            entity.Property(e => e.AesComment)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_Comment");
            entity.Property(e => e.AesCompId).HasColumnName("AES_CompID");
            entity.Property(e => e.AesCrBy).HasColumnName("AES_CrBy");
            entity.Property(e => e.AesCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AES_CrOn");
            entity.Property(e => e.AesCustId).HasColumnName("AES_CustID");
            entity.Property(e => e.AesFunctionId).HasColumnName("AES_FunctionID");
            entity.Property(e => e.AesIntroduction)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_Introduction");
            entity.Property(e => e.AesIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AES_IPAddress");
            entity.Property(e => e.AesIssuanceDate)
                .HasColumnType("datetime")
                .HasColumnName("AES_IssuanceDate");
            entity.Property(e => e.AesKeyAuditObservation)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AES_KeyAuditObservation");
            entity.Property(e => e.AesPgedetailId).HasColumnName("AES_PGEDetailId");
            entity.Property(e => e.AesPkid).HasColumnName("AES_PKID");
            entity.Property(e => e.AesReviewedOn)
                .HasColumnType("datetime")
                .HasColumnName("AES_ReviewedOn");
            entity.Property(e => e.AesRevieweddBy).HasColumnName("AES_RevieweddBy");
            entity.Property(e => e.AesStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AES_Status");
            entity.Property(e => e.AesSubmittedBy).HasColumnName("AES_SubmittedBy");
            entity.Property(e => e.AesSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("AES_SubmittedOn");
            entity.Property(e => e.AesUpdatedBy).HasColumnName("AES_UpdatedBy");
            entity.Property(e => e.AesUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AES_UpdatedOn");
            entity.Property(e => e.AesYearId).HasColumnName("AES_YearID");
        });

        modelBuilder.Entity<AuditIssueTrackerDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_IssueTracker_Details");

            entity.Property(e => e.AitAttachId).HasColumnName("AIT_AttachID");
            entity.Property(e => e.AitAuditCode).HasColumnName("AIT_AuditCode");
            entity.Property(e => e.AitAuditorRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_AuditorRemarks");
            entity.Property(e => e.AitCheckId).HasColumnName("AIT_CheckID");
            entity.Property(e => e.AitCompId).HasColumnName("AIT_CompID");
            entity.Property(e => e.AitCondition)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_Condition");
            entity.Property(e => e.AitControlId).HasColumnName("AIT_ControlID");
            entity.Property(e => e.AitCreatedBy).HasColumnName("AIT_CreatedBy");
            entity.Property(e => e.AitCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AIT_CreatedOn");
            entity.Property(e => e.AitCriteria)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_Criteria");
            entity.Property(e => e.AitCustId).HasColumnName("AIT_CustID");
            entity.Property(e => e.AitDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_Details");
            entity.Property(e => e.AitFunctionId).HasColumnName("AIT_FunctionID");
            entity.Property(e => e.AitImpact)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_Impact");
            entity.Property(e => e.AitIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AIT_IPAddress");
            entity.Property(e => e.AitIssueJobNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AIT_IssueJobNo");
            entity.Property(e => e.AitIssueName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_IssueName");
            entity.Property(e => e.AitIssueNameId).HasColumnName("AIT_IssueNameID");
            entity.Property(e => e.AitOpenCloseStatus).HasColumnName("AIT_OpenCloseStatus");
            entity.Property(e => e.AitPgedetailId).HasColumnName("AIT_PGEDetailId");
            entity.Property(e => e.AitPkid).HasColumnName("AIT_PKID");
            entity.Property(e => e.AitProcessId).HasColumnName("AIT_ProcessID");
            entity.Property(e => e.AitReviewedBy).HasColumnName("AIT_ReviewedBy");
            entity.Property(e => e.AitReviewedOn)
                .HasColumnType("datetime")
                .HasColumnName("AIT_ReviewedOn");
            entity.Property(e => e.AitReviewerRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_ReviewerRemarks");
            entity.Property(e => e.AitRiskCategoryId).HasColumnName("AIT_RiskCategoryID");
            entity.Property(e => e.AitRiskId).HasColumnName("AIT_RiskID");
            entity.Property(e => e.AitRootCause)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_RootCause");
            entity.Property(e => e.AitSeverityId).HasColumnName("AIT_SeverityID");
            entity.Property(e => e.AitStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AIT_Status");
            entity.Property(e => e.AitSubFunctionId).HasColumnName("AIT_SubFunctionID");
            entity.Property(e => e.AitSubProcessId).HasColumnName("AIT_SubProcessID");
            entity.Property(e => e.AitSuggestedRemedies)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("AIT_SuggestedRemedies");
            entity.Property(e => e.AitUpdatedBy).HasColumnName("AIT_UpdatedBy");
            entity.Property(e => e.AitUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AIT_UpdatedOn");
            entity.Property(e => e.AitWorkPaperId).HasColumnName("AIT_WorkPaperID");
            entity.Property(e => e.AitYearId).HasColumnName("AIT_YearID");
        });

        modelBuilder.Entity<AuditIssueTrackerHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_IssueTracker_History");

            entity.Property(e => e.AithArcrBy).HasColumnName("AITH_ARCrBy");
            entity.Property(e => e.AithArcrOn)
                .HasColumnType("datetime")
                .HasColumnName("AITH_ARCrOn");
            entity.Property(e => e.AithAuditId).HasColumnName("AITH_AuditID");
            entity.Property(e => e.AithAuditorRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AITH_AuditorRemarks");
            entity.Property(e => e.AithCompId).HasColumnName("AITH_CompID");
            entity.Property(e => e.AithCustId).HasColumnName("AITH_CustID");
            entity.Property(e => e.AithFunctionId).HasColumnName("AITH_FunctionID");
            entity.Property(e => e.AithIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AITH_IPAddress");
            entity.Property(e => e.AithIssuePkid).HasColumnName("AITH_IssuePKID");
            entity.Property(e => e.AithPkid).HasColumnName("AITH_PKID");
            entity.Property(e => e.AithReviewerRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AITH_ReviewerRemarks");
            entity.Property(e => e.AithRrcrBy).HasColumnName("AITH_RRCrBy");
            entity.Property(e => e.AithRrcrOn)
                .HasColumnType("datetime")
                .HasColumnName("AITH_RRCrOn");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Log");

            entity.Property(e => e.AdtCompId).HasColumnName("ADT_CompID");
            entity.Property(e => e.AdtKeyid).HasColumnName("ADT_KEYID");
            entity.Property(e => e.AdtLogin)
                .HasColumnType("datetime")
                .HasColumnName("ADT_LOGIN");
            entity.Property(e => e.AdtLogout)
                .HasColumnType("datetime")
                .HasColumnName("ADT_LOGOUT");
            entity.Property(e => e.AdtUserid).HasColumnName("ADT_USERID");
        });

        modelBuilder.Entity<AuditLogDetail>(entity =>
        {
            entity.HasKey(e => e.AldId).HasName("PK__Audit_Lo__BDB474DBB721437E");

            entity.ToTable("Audit_Log_Details");

            entity.Property(e => e.AldId)
                .ValueGeneratedNever()
                .HasColumnName("ALD_ID");
            entity.Property(e => e.AldCompId).HasColumnName("ALD_CompId");
            entity.Property(e => e.AldMasid).HasColumnName("ALD_MASID");
            entity.Property(e => e.AldModuleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ALD_ModuleName");
            entity.Property(e => e.AldModuleTime).HasColumnName("ALD_ModuleTime");
            entity.Property(e => e.AldScreenTotalTime).HasColumnName("ALD_ScreenTotalTime");
            entity.Property(e => e.AldTotalIdleTime).HasColumnName("ALD_TotalIdleTime");
            entity.Property(e => e.AldUserId).HasColumnName("ALD_UserId");
        });

        modelBuilder.Entity<AuditLogFormOperation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Log_Form_Operations");

            entity.Property(e => e.AlfoCompId).HasColumnName("ALFO_CompID");
            entity.Property(e => e.AlfoDate)
                .HasColumnType("datetime")
                .HasColumnName("ALFO_Date");
            entity.Property(e => e.AlfoEvent)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ALFO_Event");
            entity.Property(e => e.AlfoForm)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ALFO_Form");
            entity.Property(e => e.AlfoIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ALFO_IPAddress");
            entity.Property(e => e.AlfoMasterId).HasColumnName("ALFO_MasterID");
            entity.Property(e => e.AlfoMasterName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ALFO_MasterName");
            entity.Property(e => e.AlfoModule)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ALFO_Module");
            entity.Property(e => e.AlfoPkid).HasColumnName("ALFO_PKID");
            entity.Property(e => e.AlfoSubMasterId).HasColumnName("ALFO_SubMasterID");
            entity.Property(e => e.AlfoSubMasterName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ALFO_SubMasterName");
            entity.Property(e => e.AlfoUserId).HasColumnName("ALFO_UserID");
        });

        modelBuilder.Entity<AuditLogOperation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Log_Operations");

            entity.Property(e => e.AlpCompId).HasColumnName("ALP_CompID");
            entity.Property(e => e.AlpDate)
                .HasColumnType("datetime")
                .HasColumnName("ALP_Date");
            entity.Property(e => e.AlpIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ALP_IPAddress");
            entity.Property(e => e.AlpLogOut)
                .HasColumnType("datetime")
                .HasColumnName("ALP_LogOut");
            entity.Property(e => e.AlpLogType)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ALP_LogType");
            entity.Property(e => e.AlpPassword)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ALP_Password");
            entity.Property(e => e.AlpPkid).HasColumnName("ALP_PKID");
            entity.Property(e => e.AlpUserId).HasColumnName("ALP_UserID");
            entity.Property(e => e.AlpUserName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ALP_UserName");
        });

        modelBuilder.Entity<AuditPlanSignOff>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_PlanSignOff");

            entity.Property(e => e.ApsoAppBy).HasColumnName("APSO_AppBy");
            entity.Property(e => e.ApsoAppOn)
                .HasColumnType("datetime")
                .HasColumnName("APSO_AppOn");
            entity.Property(e => e.ApsoAttachId).HasColumnName("APSO_AttachID");
            entity.Property(e => e.ApsoAuditCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APSO_AuditCode");
            entity.Property(e => e.ApsoAuditPlanStatus).HasColumnName("APSO_AuditPlanStatus");
            entity.Property(e => e.ApsoAuditReview).HasColumnName("APSO_AuditReview");
            entity.Property(e => e.ApsoCompId).HasColumnName("APSO_CompID");
            entity.Property(e => e.ApsoCrBy).HasColumnName("APSO_CrBy");
            entity.Property(e => e.ApsoCrOn)
                .HasColumnType("datetime")
                .HasColumnName("APSO_CrOn");
            entity.Property(e => e.ApsoCustId).HasColumnName("APSO_CustID");
            entity.Property(e => e.ApsoFunctionId).HasColumnName("APSO_FunctionID");
            entity.Property(e => e.ApsoId).HasColumnName("APSO_ID");
            entity.Property(e => e.ApsoIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("APSO_IPAddress");
            entity.Property(e => e.ApsoPgedetailId).HasColumnName("APSO_PGEDetailId");
            entity.Property(e => e.ApsoRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("APSO_Remarks");
            entity.Property(e => e.ApsoStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("APSO_Status");
            entity.Property(e => e.ApsoUpdatedBy).HasColumnName("APSO_UpdatedBy");
            entity.Property(e => e.ApsoUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("APSO_UpdatedOn");
            entity.Property(e => e.ApsoYearId).HasColumnName("APSO_YearID");
        });

        modelBuilder.Entity<AuditSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_Schedule");

            entity.Property(e => e.AudApprovedBy).HasColumnName("AUD_ApprovedBy");
            entity.Property(e => e.AudApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("AUD_ApprovedOn");
            entity.Property(e => e.AudAuditorIds)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AUD_AuditorIDs");
            entity.Property(e => e.AudCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AUD_Code");
            entity.Property(e => e.AudCompId).HasColumnName("AUD_CompID");
            entity.Property(e => e.AudCrBy).HasColumnName("AUD_CrBy");
            entity.Property(e => e.AudCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AUD_CrOn");
            entity.Property(e => e.AudFirstmail).HasColumnName("AUD_Firstmail");
            entity.Property(e => e.AudFromDate)
                .HasColumnType("datetime")
                .HasColumnName("AUD_FromDate");
            entity.Property(e => e.AudId).HasColumnName("AUD_ID");
            entity.Property(e => e.AudIntmail).HasColumnName("AUD_Intmail");
            entity.Property(e => e.AudIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AUD_IPAddress");
            entity.Property(e => e.AudKitchenId).HasColumnName("AUD_KitchenID");
            entity.Property(e => e.AudMonthId).HasColumnName("AUD_MonthID");
            entity.Property(e => e.AudOperation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AUD_Operation");
            entity.Property(e => e.AudSecondMail).HasColumnName("AUD_SecondMail");
            entity.Property(e => e.AudSectionId).HasColumnName("AUD_SectionID");
            entity.Property(e => e.AudStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AUD_Status");
            entity.Property(e => e.AudToDate)
                .HasColumnType("datetime")
                .HasColumnName("AUD_ToDate");
            entity.Property(e => e.AudUpdateOn)
                .HasColumnType("datetime")
                .HasColumnName("AUD_UpdateOn");
            entity.Property(e => e.AudUpdatedBy).HasColumnName("AUD_UpdatedBy");
            entity.Property(e => e.AudYearId).HasColumnName("AUD_YearID");
        });

        modelBuilder.Entity<AuditSignOff>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_SignOff");

            entity.Property(e => e.AsoAttachId).HasColumnName("ASO_AttachID");
            entity.Property(e => e.AsoAuditCodeId).HasColumnName("ASO_AuditCodeID");
            entity.Property(e => e.AsoAuditRatingId).HasColumnName("ASO_AuditRatingID");
            entity.Property(e => e.AsoComments)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ASO_Comments");
            entity.Property(e => e.AsoCompId).HasColumnName("ASO_CompID");
            entity.Property(e => e.AsoCrBy).HasColumnName("ASO_CrBy");
            entity.Property(e => e.AsoCrOn)
                .HasColumnType("datetime")
                .HasColumnName("ASO_CrOn");
            entity.Property(e => e.AsoCustId).HasColumnName("ASO_CustID");
            entity.Property(e => e.AsoFunctionId).HasColumnName("ASO_FunctionID");
            entity.Property(e => e.AsoIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASO_IPAddress");
            entity.Property(e => e.AsoKeyObservation)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ASO_KeyObservation");
            entity.Property(e => e.AsoMasterId).HasColumnName("ASO_MasterID");
            entity.Property(e => e.AsoOverAllComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ASO_OverAllComments");
            entity.Property(e => e.AsoPkid).HasColumnName("ASO_PKID");
            entity.Property(e => e.AsoSignOffStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("ASO_SignOffStatus");
            entity.Property(e => e.AsoStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ASO_Status");
            entity.Property(e => e.AsoSubmittedBy).HasColumnName("ASO_SubmittedBy");
            entity.Property(e => e.AsoSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASO_SubmittedOn");
            entity.Property(e => e.AsoUpdatedBy).HasColumnName("ASO_UpdatedBy");
            entity.Property(e => e.AsoUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ASO_UpdatedOn");
            entity.Property(e => e.AsoYearId).HasColumnName("ASO_YearID");
        });

        modelBuilder.Entity<AuditTimeCostBudgetDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_TimeCostBudgetDetails");

            entity.Property(e => e.AtcdAtcbid).HasColumnName("ATCD_ATCBID");
            entity.Property(e => e.AtcdAuditCodeId).HasColumnName("ATCD_AuditCodeID");
            entity.Property(e => e.AtcdCompId).HasColumnName("ATCD_CompID");
            entity.Property(e => e.AtcdCost).HasColumnName("ATCD_Cost");
            entity.Property(e => e.AtcdCostPerDay).HasColumnName("ATCD_CostPerDay");
            entity.Property(e => e.AtcdDays).HasColumnName("ATCD_Days");
            entity.Property(e => e.AtcdHours).HasColumnName("ATCD_Hours");
            entity.Property(e => e.AtcdHoursPerDay).HasColumnName("ATCD_HoursPerDay");
            entity.Property(e => e.AtcdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATCD_IPAddress");
            entity.Property(e => e.AtcdPkid).HasColumnName("ATCD_PKID");
            entity.Property(e => e.AtcdTaskProcessId).HasColumnName("ATCD_TaskProcessID");
            entity.Property(e => e.AtcdType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ATCD_Type");
            entity.Property(e => e.AtcdUserId).HasColumnName("ATCD_UserID");
            entity.Property(e => e.AtcdYearId).HasColumnName("ATCD_YearID");
        });

        modelBuilder.Entity<AuditTimeCostBudgetMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_TimeCostBudgetMaster");

            entity.Property(e => e.AtcbAuditCodeId).HasColumnName("ATCB_AuditCodeID");
            entity.Property(e => e.AtcbCompId).HasColumnName("ATCB_CompID");
            entity.Property(e => e.AtcbCreatedby).HasColumnName("ATCB_Createdby");
            entity.Property(e => e.AtcbCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATCB_Createdon");
            entity.Property(e => e.AtcbDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ATCB_DelFlag");
            entity.Property(e => e.AtcbIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATCB_IPAddress");
            entity.Property(e => e.AtcbPkid).HasColumnName("ATCB_PKID");
            entity.Property(e => e.AtcbStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ATCB_Status");
            entity.Property(e => e.AtcbSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("ATCB_SubmittedOn");
            entity.Property(e => e.AtcbSubmittedby).HasColumnName("ATCB_Submittedby");
            entity.Property(e => e.AtcbTaskProcessId).HasColumnName("ATCB_TaskProcessID");
            entity.Property(e => e.AtcbTotalCost).HasColumnName("ATCB_TotalCost");
            entity.Property(e => e.AtcbTotalDays).HasColumnName("ATCB_TotalDays");
            entity.Property(e => e.AtcbTotalHours).HasColumnName("ATCB_TotalHours");
            entity.Property(e => e.AtcbType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ATCB_Type");
            entity.Property(e => e.AtcbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("ATCB_UpdatedOn");
            entity.Property(e => e.AtcbUpdatedby).HasColumnName("ATCB_Updatedby");
            entity.Property(e => e.AtcbYearId).HasColumnName("ATCB_YearID");
        });

        modelBuilder.Entity<AuditTimeSheet>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_TimeSheet");

            entity.Property(e => e.TsApprovedby).HasColumnName("TS_APPROVEDBY");
            entity.Property(e => e.TsApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("TS_APPROVEDON");
            entity.Property(e => e.TsApproverRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("TS_Approver_Remarks");
            entity.Property(e => e.TsAuditCodeId).HasColumnName("TS_AuditCodeID");
            entity.Property(e => e.TsComments)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("TS_Comments");
            entity.Property(e => e.TsCompId).HasColumnName("TS_CompID");
            entity.Property(e => e.TsCrby).HasColumnName("TS_CRBY");
            entity.Property(e => e.TsCron)
                .HasColumnType("datetime")
                .HasColumnName("TS_CRON");
            entity.Property(e => e.TsCustId).HasColumnName("TS_CustID");
            entity.Property(e => e.TsDate)
                .HasColumnType("datetime")
                .HasColumnName("TS_Date");
            entity.Property(e => e.TsDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("TS_DELFLG");
            entity.Property(e => e.TsDescid).HasColumnName("TS_DESCID");
            entity.Property(e => e.TsFunId).HasColumnName("TS_FunID");
            entity.Property(e => e.TsHours)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("TS_Hours");
            entity.Property(e => e.TsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TS_IPAddress");
            entity.Property(e => e.TsIsApproved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TS_IsApproved");
            entity.Property(e => e.TsPkid).HasColumnName("TS_PKID");
            entity.Property(e => e.TsStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TS_STATUS");
            entity.Property(e => e.TsTaskId).HasColumnName("TS_TaskID");
            entity.Property(e => e.TsTaskType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TS_TaskType");
            entity.Property(e => e.TsUpdatedBy).HasColumnName("TS_UpdatedBy");
            entity.Property(e => e.TsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TS_UpdatedOn");
            entity.Property(e => e.TsUserId).HasColumnName("TS_UserID");
            entity.Property(e => e.TsYearId).HasColumnName("TS_YearID");
        });

        modelBuilder.Entity<AuditTimesheet1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("audit_timesheet1");

            entity.Property(e => e.TsApprovedby).HasColumnName("TS_APPROVEDBY");
            entity.Property(e => e.TsApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("TS_APPROVEDON");
            entity.Property(e => e.TsApproverRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("TS_Approver_Remarks");
            entity.Property(e => e.TsAuditCodeId).HasColumnName("TS_AuditCodeID");
            entity.Property(e => e.TsComments)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("TS_Comments");
            entity.Property(e => e.TsCompId).HasColumnName("TS_CompID");
            entity.Property(e => e.TsCrby).HasColumnName("TS_CRBY");
            entity.Property(e => e.TsCron)
                .HasColumnType("datetime")
                .HasColumnName("TS_CRON");
            entity.Property(e => e.TsCustId).HasColumnName("TS_CustID");
            entity.Property(e => e.TsDate)
                .HasColumnType("datetime")
                .HasColumnName("TS_Date");
            entity.Property(e => e.TsDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("TS_DELFLG");
            entity.Property(e => e.TsDescid).HasColumnName("TS_DESCID");
            entity.Property(e => e.TsFunId).HasColumnName("TS_FunID");
            entity.Property(e => e.TsHours)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("TS_Hours");
            entity.Property(e => e.TsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TS_IPAddress");
            entity.Property(e => e.TsIsApproved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TS_IsApproved");
            entity.Property(e => e.TsPkid).HasColumnName("TS_PKID");
            entity.Property(e => e.TsStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TS_STATUS");
            entity.Property(e => e.TsTaskId).HasColumnName("TS_TaskID");
            entity.Property(e => e.TsTaskType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TS_TaskType");
            entity.Property(e => e.TsUpdatedBy).HasColumnName("TS_UpdatedBy");
            entity.Property(e => e.TsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TS_UpdatedOn");
            entity.Property(e => e.TsUserId).HasColumnName("TS_UserID");
            entity.Property(e => e.TsYearId).HasColumnName("TS_YearID");
        });

        modelBuilder.Entity<AuditTypeChecklistMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditType_Checklist_Master");

            entity.Property(e => e.AcmApprovedby).HasColumnName("ACM_APPROVEDBY");
            entity.Property(e => e.AcmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ACM_APPROVEDON");
            entity.Property(e => e.AcmAssertions)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ACM_Assertions");
            entity.Property(e => e.AcmAuditTypeId).HasColumnName("ACM_AuditTypeID");
            entity.Property(e => e.AcmCheckpoint)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ACM_Checkpoint");
            entity.Property(e => e.AcmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACM_Code");
            entity.Property(e => e.AcmCompId).HasColumnName("ACM_CompId");
            entity.Property(e => e.AcmCrby).HasColumnName("ACM_CRBY");
            entity.Property(e => e.AcmCron)
                .HasColumnType("datetime")
                .HasColumnName("ACM_CRON");
            entity.Property(e => e.AcmDeletedby).HasColumnName("ACM_DELETEDBY");
            entity.Property(e => e.AcmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ACM_DELETEDON");
            entity.Property(e => e.AcmDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ACM_DELFLG");
            entity.Property(e => e.AcmHeading)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("ACM_Heading");
            entity.Property(e => e.AcmId).HasColumnName("ACM_ID");
            entity.Property(e => e.AcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ACM_IPAddress");
            entity.Property(e => e.AcmRecallby).HasColumnName("ACM_RECALLBY");
            entity.Property(e => e.AcmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ACM_RECALLON");
            entity.Property(e => e.AcmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ACM_STATUS");
            entity.Property(e => e.AcmUpdatedby).HasColumnName("ACM_UPDATEDBY");
            entity.Property(e => e.AcmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ACM_UPDATEDON");
        });

        modelBuilder.Entity<AuditWorkPaper>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_WorkPaper");

            entity.Property(e => e.AwpAttachId).HasColumnName("AWP_AttachID");
            entity.Property(e => e.AwpAuditCode).HasColumnName("AWP_AuditCode");
            entity.Property(e => e.AwpAuditeeResponseName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AWP_AuditeeResponseName");
            entity.Property(e => e.AwpAuditorObservationName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AWP_AuditorObservationName");
            entity.Property(e => e.AwpAuditorRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWP_AuditorRemarks");
            entity.Property(e => e.AwpChecksId).HasColumnName("AWP_ChecksID");
            entity.Property(e => e.AwpCompId).HasColumnName("AWP_CompID");
            entity.Property(e => e.AwpConclusionId).HasColumnName("AWP_ConclusionID");
            entity.Property(e => e.AwpControlId).HasColumnName("AWP_ControlID");
            entity.Property(e => e.AwpCrBy).HasColumnName("AWP_CrBy");
            entity.Property(e => e.AwpCrOn)
                .HasColumnType("datetime")
                .HasColumnName("AWP_CrOn");
            entity.Property(e => e.AwpCustId).HasColumnName("AWP_CustID");
            entity.Property(e => e.AwpFunctionId).HasColumnName("AWP_FunctionID");
            entity.Property(e => e.AwpIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AWP_IPAddress");
            entity.Property(e => e.AwpNote)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWP_Note");
            entity.Property(e => e.AwpOpenCloseStatus).HasColumnName("AWP_OpenCloseStatus");
            entity.Property(e => e.AwpPgedetailId).HasColumnName("AWP_PGEDetailId");
            entity.Property(e => e.AwpPkid).HasColumnName("AWP_PKID");
            entity.Property(e => e.AwpProcessId).HasColumnName("AWP_ProcessID");
            entity.Property(e => e.AwpResponse)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWP_Response");
            entity.Property(e => e.AwpReviewedBy).HasColumnName("AWP_ReviewedBy");
            entity.Property(e => e.AwpReviewedOn)
                .HasColumnType("datetime")
                .HasColumnName("AWP_ReviewedOn");
            entity.Property(e => e.AwpReviewerRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWP_ReviewerRemarks");
            entity.Property(e => e.AwpRiskId).HasColumnName("AWP_RiskID");
            entity.Property(e => e.AwpStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AWP_Status");
            entity.Property(e => e.AwpSubFunctionId).HasColumnName("AWP_SubFunctionID");
            entity.Property(e => e.AwpSubProcessId).HasColumnName("AWP_SubProcessID");
            entity.Property(e => e.AwpSubmittedBy).HasColumnName("AWP_SubmittedBy");
            entity.Property(e => e.AwpSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("AWP_SubmittedOn");
            entity.Property(e => e.AwpTypeofTestId).HasColumnName("AWP_TypeofTestID");
            entity.Property(e => e.AwpUpdatedBy).HasColumnName("AWP_UpdatedBy");
            entity.Property(e => e.AwpUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("AWP_UpdatedOn");
            entity.Property(e => e.AwpWorkPaperDone)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWP_WorkPaperDone");
            entity.Property(e => e.AwpWorkPaperNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("AWP_WorkPaperNo");
            entity.Property(e => e.AwpYearId).HasColumnName("AWP_YearID");
        });

        modelBuilder.Entity<AuditWorkPaperHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit_WorkPaper_History");

            entity.Property(e => e.AwphArcrBy).HasColumnName("AWPH_ARCrBy");
            entity.Property(e => e.AwphArcrOn)
                .HasColumnType("datetime")
                .HasColumnName("AWPH_ARCrOn");
            entity.Property(e => e.AwphAuditId).HasColumnName("AWPH_AuditID");
            entity.Property(e => e.AwphAuditorRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWPH_AuditorRemarks");
            entity.Property(e => e.AwphCompId).HasColumnName("AWPH_CompID");
            entity.Property(e => e.AwphCustId).HasColumnName("AWPH_CustID");
            entity.Property(e => e.AwphFunctionId).HasColumnName("AWPH_FunctionID");
            entity.Property(e => e.AwphIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("AWPH_IPAddress");
            entity.Property(e => e.AwphPkid).HasColumnName("AWPH_PKID");
            entity.Property(e => e.AwphReviewerRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("AWPH_ReviewerRemarks");
            entity.Property(e => e.AwphRrcrBy).HasColumnName("AWPH_RRCrBy");
            entity.Property(e => e.AwphRrcrOn)
                .HasColumnType("datetime")
                .HasColumnName("AWPH_RRCrOn");
            entity.Property(e => e.AwphWpid).HasColumnName("AWPH_WPID");
        });

        modelBuilder.Entity<BatchScanTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BatchScan_Table");

            entity.Property(e => e.BtBatchNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BT_BatchNo");
            entity.Property(e => e.BtCompId).HasColumnName("BT_CompID");
            entity.Property(e => e.BtCrBy).HasColumnName("BT_CrBy");
            entity.Property(e => e.BtCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BT_CrOn");
            entity.Property(e => e.BtCreditTotal)
                .HasColumnType("money")
                .HasColumnName("BT_CreditTotal");
            entity.Property(e => e.BtCustomerId).HasColumnName("BT_CustomerID");
            entity.Property(e => e.BtDebitTotal)
                .HasColumnType("money")
                .HasColumnName("BT_DebitTotal");
            entity.Property(e => e.BtDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("BT_Delflag");
            entity.Property(e => e.BtId).HasColumnName("BT_ID");
            entity.Property(e => e.BtIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BT_IPAddress");
            entity.Property(e => e.BtNoOfTransaction).HasColumnName("BT_NoOfTransaction");
            entity.Property(e => e.BtOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BT_Operation");
            entity.Property(e => e.BtStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BT_Status");
            entity.Property(e => e.BtTrType).HasColumnName("BT_TrType");
            entity.Property(e => e.BtYearId).HasColumnName("BT_YearID");
        });

        modelBuilder.Entity<CaiqCategoryDescription>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CAIQ_CategoryDescription");

            entity.Property(e => e.CcdApprovedBy).HasColumnName("CCD_ApprovedBy");
            entity.Property(e => e.CcdApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CCD_ApprovedOn");
            entity.Property(e => e.CcdAuditId).HasColumnName("CCD_AuditID");
            entity.Property(e => e.CcdCategoryId).HasColumnName("CCD_CategoryID");
            entity.Property(e => e.CcdCompId).HasColumnName("CCD_CompId");
            entity.Property(e => e.CcdCrBy).HasColumnName("CCD_CrBy");
            entity.Property(e => e.CcdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CCD_CrOn");
            entity.Property(e => e.CcdDeletedBy).HasColumnName("CCD_DeletedBy");
            entity.Property(e => e.CcdDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CCD_DeletedOn");
            entity.Property(e => e.CcdDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CCD_Desc");
            entity.Property(e => e.CcdDescValue).HasColumnName("CCD_DescValue");
            entity.Property(e => e.CcdDescriptorId).HasColumnName("CCD_DescriptorID");
            entity.Property(e => e.CcdFactorId).HasColumnName("CCD_FactorID");
            entity.Property(e => e.CcdFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CCD_FLAG");
            entity.Property(e => e.CcdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CCD_IPAddress");
            entity.Property(e => e.CcdName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CCD_Name");
            entity.Property(e => e.CcdPkid).HasColumnName("CCD_PKID");
            entity.Property(e => e.CcdRecallBy).HasColumnName("CCD_RecallBy");
            entity.Property(e => e.CcdRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CCD_RecallOn");
            entity.Property(e => e.CcdStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CCD_STATUS");
            entity.Property(e => e.CcdUpdatedBy).HasColumnName("CCD_UpdatedBy");
            entity.Property(e => e.CcdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CCD_UpdatedOn");
            entity.Property(e => e.CcdYearId).HasColumnName("CCD_YearID");
        });

        modelBuilder.Entity<CaiqDescriptor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CAIQ_Descriptors");

            entity.Property(e => e.CdApprovedBy).HasColumnName("CD_ApprovedBy");
            entity.Property(e => e.CdApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CD_ApprovedOn");
            entity.Property(e => e.CdAuditId).HasColumnName("CD_AuditID");
            entity.Property(e => e.CdCompId).HasColumnName("CD_CompId");
            entity.Property(e => e.CdCrBy).HasColumnName("CD_CrBy");
            entity.Property(e => e.CdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CD_CrOn");
            entity.Property(e => e.CdDeletedBy).HasColumnName("CD_DeletedBy");
            entity.Property(e => e.CdDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CD_DeletedOn");
            entity.Property(e => e.CdDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CD_Desc");
            entity.Property(e => e.CdFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CD_FLAG");
            entity.Property(e => e.CdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CD_IPAddress");
            entity.Property(e => e.CdName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CD_Name");
            entity.Property(e => e.CdPkid).HasColumnName("CD_PKID");
            entity.Property(e => e.CdRange)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CD_Range");
            entity.Property(e => e.CdRecallBy).HasColumnName("CD_RecallBy");
            entity.Property(e => e.CdRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CD_RecallOn");
            entity.Property(e => e.CdStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CD_STATUS");
            entity.Property(e => e.CdUpdatedBy).HasColumnName("CD_UpdatedBy");
            entity.Property(e => e.CdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CD_UpdatedOn");
            entity.Property(e => e.CdYearId).HasColumnName("CD_YearID");
        });

        modelBuilder.Entity<CaiqFactor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CAIQ_Factors");

            entity.Property(e => e.CfApprovedBy).HasColumnName("CF_ApprovedBy");
            entity.Property(e => e.CfApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CF_ApprovedOn");
            entity.Property(e => e.CfAuditId).HasColumnName("CF_AuditID");
            entity.Property(e => e.CfCompId).HasColumnName("CF_CompId");
            entity.Property(e => e.CfCrBy).HasColumnName("CF_CrBy");
            entity.Property(e => e.CfCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CF_CrOn");
            entity.Property(e => e.CfDeletedBy).HasColumnName("CF_DeletedBy");
            entity.Property(e => e.CfDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CF_DeletedOn");
            entity.Property(e => e.CfDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CF_Desc");
            entity.Property(e => e.CfFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CF_FLAG");
            entity.Property(e => e.CfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CF_IPAddress");
            entity.Property(e => e.CfName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CF_Name");
            entity.Property(e => e.CfPkid).HasColumnName("CF_PKID");
            entity.Property(e => e.CfRecallBy).HasColumnName("CF_RecallBy");
            entity.Property(e => e.CfRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CF_RecallOn");
            entity.Property(e => e.CfStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CF_STATUS");
            entity.Property(e => e.CfUpdatedBy).HasColumnName("CF_UpdatedBy");
            entity.Property(e => e.CfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CF_UpdatedOn");
            entity.Property(e => e.CfYearId).HasColumnName("CF_YearID");
        });

        modelBuilder.Entity<CaiqFactorCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CAIQ_FactorCategory");

            entity.Property(e => e.CfcApprovedBy).HasColumnName("CFC_ApprovedBy");
            entity.Property(e => e.CfcApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CFC_ApprovedOn");
            entity.Property(e => e.CfcAuditId).HasColumnName("CFC_AuditID");
            entity.Property(e => e.CfcCompId).HasColumnName("CFC_CompId");
            entity.Property(e => e.CfcCrBy).HasColumnName("CFC_CrBy");
            entity.Property(e => e.CfcCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CFC_CrOn");
            entity.Property(e => e.CfcDeletedBy).HasColumnName("CFC_DeletedBy");
            entity.Property(e => e.CfcDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CFC_DeletedOn");
            entity.Property(e => e.CfcDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CFC_Desc");
            entity.Property(e => e.CfcFactorId).HasColumnName("CFC_FactorID");
            entity.Property(e => e.CfcFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CFC_FLAG");
            entity.Property(e => e.CfcIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CFC_IPAddress");
            entity.Property(e => e.CfcName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CFC_Name");
            entity.Property(e => e.CfcPkid).HasColumnName("CFC_PKID");
            entity.Property(e => e.CfcRecallBy).HasColumnName("CFC_RecallBy");
            entity.Property(e => e.CfcRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CFC_RecallOn");
            entity.Property(e => e.CfcStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CFC_STATUS");
            entity.Property(e => e.CfcUpdatedBy).HasColumnName("CFC_UpdatedBy");
            entity.Property(e => e.CfcUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CFC_UpdatedOn");
            entity.Property(e => e.CfcYearId).HasColumnName("CFC_YearID");
        });

        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("chart_of_Accounts");

            entity.Property(e => e.GlAccHead).HasColumnName("gl_AccHead");
            entity.Property(e => e.GlAccType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gl_AccType");
            entity.Property(e => e.GlAppBy).HasColumnName("gl_AppBy");
            entity.Property(e => e.GlAppOn)
                .HasColumnType("datetime")
                .HasColumnName("gl_AppOn");
            entity.Property(e => e.GlBalAmt).HasColumnName("gl_BalAmt");
            entity.Property(e => e.GlBalType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gl_BalType");
            entity.Property(e => e.GlBranchCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gl_BranchCode");
            entity.Property(e => e.GlCompId).HasColumnName("gl_CompId");
            entity.Property(e => e.GlCrBy).HasColumnName("gl_CrBy");
            entity.Property(e => e.GlCrDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("gl_CrDate");
            entity.Property(e => e.GlCustId).HasColumnName("gl_CustID");
            entity.Property(e => e.GlDelBy).HasColumnName("gl_DelBy");
            entity.Property(e => e.GlDelDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("gl_DelDate");
            entity.Property(e => e.GlDelflag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gl_delflag");
            entity.Property(e => e.GlDesc)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("gl_desc");
            entity.Property(e => e.GlEffectiveDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("gl_effective_date");
            entity.Property(e => e.GlFring).HasColumnName("gl_fring");
            entity.Property(e => e.GlGlcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gl_glcode");
            entity.Property(e => e.GlHead).HasColumnName("gl_head");
            entity.Property(e => e.GlId).HasColumnName("gl_id");
            entity.Property(e => e.GlInvItem).HasColumnName("gl_InvItem");
            entity.Property(e => e.GlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("gl_IPAddress");
            entity.Property(e => e.GlLinkInv).HasColumnName("gl_LinkInv");
            entity.Property(e => e.GlOdlimit)
                .HasColumnType("money")
                .HasColumnName("gl_ODLimit");
            entity.Property(e => e.GlOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gl_Operation");
            entity.Property(e => e.GlOrderby).HasColumnName("gl_orderby");
            entity.Property(e => e.GlOrgTypeId).HasColumnName("gl_OrgTypeID");
            entity.Property(e => e.GlParent).HasColumnName("gl_parent");
            entity.Property(e => e.GlReason)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("gl_reason");
            entity.Property(e => e.GlReasonCreation)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("gl_reason_Creation");
            entity.Property(e => e.GlRemarks)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("gl_remarks");
            entity.Property(e => e.GlSortOrder).HasColumnName("gl_SortOrder");
            entity.Property(e => e.GlStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("gl_Status");
            entity.Property(e => e.GlSubglexist).HasColumnName("gl_subglexist");
            entity.Property(e => e.GlTds).HasColumnName("gl_TDS");
            entity.Property(e => e.GlTrialBalanceCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("gl_TrialBalanceCode");
            entity.Property(e => e.GlUpdatedBy).HasColumnName("gl_UpdatedBy");
            entity.Property(e => e.GlUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("gl_UpdatedOn");
        });

        modelBuilder.Entity<CmacheckMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMACheckMaster");

            entity.Property(e => e.CmApprovedBy).HasColumnName("CM_ApprovedBy");
            entity.Property(e => e.CmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_ApprovedOn");
            entity.Property(e => e.CmAreaId).HasColumnName("CM_AreaId");
            entity.Property(e => e.CmAreaNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CM_AreaNo");
            entity.Property(e => e.CmCheckPoint)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("CM_CheckPoint");
            entity.Property(e => e.CmCheckPointNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CM_CheckPointNo");
            entity.Property(e => e.CmCompId).HasColumnName("CM_CompID");
            entity.Property(e => e.CmCrBy).HasColumnName("CM_CrBy");
            entity.Property(e => e.CmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_CrOn");
            entity.Property(e => e.CmDeletedBy).HasColumnName("CM_DeletedBy");
            entity.Property(e => e.CmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_DeletedOn");
            entity.Property(e => e.CmDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CM_Delflag");
            entity.Property(e => e.CmFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CM_FunType");
            entity.Property(e => e.CmFunctionId).HasColumnName("CM_FunctionId");
            entity.Property(e => e.CmId).HasColumnName("CM_Id");
            entity.Property(e => e.CmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CM_IPAddress");
            entity.Property(e => e.CmMethodologyId).HasColumnName("CM_MethodologyId");
            entity.Property(e => e.CmRecallBy).HasColumnName("CM_RecallBy");
            entity.Property(e => e.CmRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_RecallOn");
            entity.Property(e => e.CmRiskCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CM_RiskCategory");
            entity.Property(e => e.CmRiskWeight).HasColumnName("CM_RiskWeight");
            entity.Property(e => e.CmSampleSize).HasColumnName("CM_SampleSize");
            entity.Property(e => e.CmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CM_Status");
            entity.Property(e => e.CmUpdatedBy).HasColumnName("CM_UpdatedBy");
            entity.Property(e => e.CmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_UpdatedOn");
            entity.Property(e => e.CmYearId).HasColumnName("CM_YearId");
        });

        modelBuilder.Entity<CmacheckMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMACheckMaster_Log");

            entity.Property(e => e.CmAreaId).HasColumnName("CM_AreaId");
            entity.Property(e => e.CmAreaNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CM_AreaNo");
            entity.Property(e => e.CmCheckPoint)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("CM_CheckPoint");
            entity.Property(e => e.CmCheckPointNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CM_CheckPointNo");
            entity.Property(e => e.CmCompId).HasColumnName("CM_CompID");
            entity.Property(e => e.CmFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CM_FunType");
            entity.Property(e => e.CmFunctionId).HasColumnName("CM_FunctionId");
            entity.Property(e => e.CmId).HasColumnName("CM_Id");
            entity.Property(e => e.CmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CM_IPAddress");
            entity.Property(e => e.CmMethodologyId).HasColumnName("CM_MethodologyId");
            entity.Property(e => e.CmRiskCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CM_RiskCategory");
            entity.Property(e => e.CmRiskWeight).HasColumnName("CM_RiskWeight");
            entity.Property(e => e.CmSampleSize).HasColumnName("CM_SampleSize");
            entity.Property(e => e.CmYearId).HasColumnName("CM_YearId");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCmAreaId).HasColumnName("nCM_AreaId");
            entity.Property(e => e.NCmAreaNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCM_AreaNo");
            entity.Property(e => e.NCmCheckPoint)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("nCM_CheckPoint");
            entity.Property(e => e.NCmCheckPointNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nCM_CheckPointNo");
            entity.Property(e => e.NCmFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nCM_FunType");
            entity.Property(e => e.NCmFunctionId).HasColumnName("nCM_FunctionId");
            entity.Property(e => e.NCmMethodologyId).HasColumnName("nCM_MethodologyId");
            entity.Property(e => e.NCmRiskCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nCM_RiskCategory");
            entity.Property(e => e.NCmRiskWeight).HasColumnName("nCM_RiskWeight");
            entity.Property(e => e.NCmSampleSize).HasColumnName("nCM_SampleSize");
            entity.Property(e => e.NCmYearId).HasColumnName("nCM_YearId");
        });

        modelBuilder.Entity<Cmarating>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating");

            entity.Property(e => e.CmarApprovedBy).HasColumnName("CMAR_ApprovedBy");
            entity.Property(e => e.CmarApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMAR_ApprovedOn");
            entity.Property(e => e.CmarColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMAR_Color");
            entity.Property(e => e.CmarCompId).HasColumnName("CMAR_CompId");
            entity.Property(e => e.CmarCrBy).HasColumnName("CMAR_CrBy");
            entity.Property(e => e.CmarCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CMAR_CrOn");
            entity.Property(e => e.CmarDeletedBy).HasColumnName("CMAR_DeletedBy");
            entity.Property(e => e.CmarDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMAR_DeletedOn");
            entity.Property(e => e.CmarDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMAR_Desc");
            entity.Property(e => e.CmarEndValue).HasColumnName("CMAR_EndValue");
            entity.Property(e => e.CmarFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CMAR_FLAG");
            entity.Property(e => e.CmarId).HasColumnName("CMAR_ID");
            entity.Property(e => e.CmarIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMAR_IPAddress");
            entity.Property(e => e.CmarName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMAR_Name");
            entity.Property(e => e.CmarRecallBy).HasColumnName("CMAR_RecallBy");
            entity.Property(e => e.CmarRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CMAR_RecallOn");
            entity.Property(e => e.CmarStartValue).HasColumnName("CMAR_StartValue");
            entity.Property(e => e.CmarStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CMAR_STATUS");
            entity.Property(e => e.CmarUpdatedBy).HasColumnName("CMAR_UpdatedBy");
            entity.Property(e => e.CmarUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMAR_UpdatedOn");
            entity.Property(e => e.CmarYearId).HasColumnName("CMAR_YearID");
        });

        modelBuilder.Entity<CmaratingCoreProcess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating_CoreProcess");

            entity.Property(e => e.CmacrApprovedBy).HasColumnName("CMACR_ApprovedBy");
            entity.Property(e => e.CmacrApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMACR_ApprovedOn");
            entity.Property(e => e.CmacrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMACR_Color");
            entity.Property(e => e.CmacrCompId).HasColumnName("CMACR_CompId");
            entity.Property(e => e.CmacrCrBy).HasColumnName("CMACR_CrBy");
            entity.Property(e => e.CmacrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CMACR_CrOn");
            entity.Property(e => e.CmacrDeletedBy).HasColumnName("CMACR_DeletedBy");
            entity.Property(e => e.CmacrDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMACR_DeletedOn");
            entity.Property(e => e.CmacrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMACR_Desc");
            entity.Property(e => e.CmacrEndValue).HasColumnName("CMACR_EndValue");
            entity.Property(e => e.CmacrFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CMACR_FLAG");
            entity.Property(e => e.CmacrId).HasColumnName("CMACR_ID");
            entity.Property(e => e.CmacrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMACR_IPAddress");
            entity.Property(e => e.CmacrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMACR_Name");
            entity.Property(e => e.CmacrRecallBy).HasColumnName("CMACR_RecallBy");
            entity.Property(e => e.CmacrRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CMACR_RecallOn");
            entity.Property(e => e.CmacrStartValue).HasColumnName("CMACR_StartValue");
            entity.Property(e => e.CmacrStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CMACR_STATUS");
            entity.Property(e => e.CmacrUpdatedBy).HasColumnName("CMACR_UpdatedBy");
            entity.Property(e => e.CmacrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMACR_UpdatedOn");
            entity.Property(e => e.CmacrYearId).HasColumnName("CMACR_YearID");
        });

        modelBuilder.Entity<CmaratingCoreProcessLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating_CoreProcess_log");

            entity.Property(e => e.CmacrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMACR_Color");
            entity.Property(e => e.CmacrCompId).HasColumnName("CMACR_CompId");
            entity.Property(e => e.CmacrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMACR_Desc");
            entity.Property(e => e.CmacrEndValue)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CMACR_EndValue");
            entity.Property(e => e.CmacrId).HasColumnName("CMACR_ID");
            entity.Property(e => e.CmacrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMACR_IPAddress");
            entity.Property(e => e.CmacrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMACR_Name");
            entity.Property(e => e.CmacrStartValue)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CMACR_StartValue");
            entity.Property(e => e.CmacrYearId).HasColumnName("CMACR_YearID");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCmacrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMACR_Color");
            entity.Property(e => e.NCmacrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMACR_Desc");
            entity.Property(e => e.NCmacrEndValue)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nCMACR_EndValue");
            entity.Property(e => e.NCmacrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMACR_Name");
            entity.Property(e => e.NCmacrStartValue)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nCMACR_StartValue");
            entity.Property(e => e.NCmacrYearId).HasColumnName("nCMACR_YearID");
        });

        modelBuilder.Entity<CmaratingLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating_Log");

            entity.Property(e => e.CmarColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMAR_Color");
            entity.Property(e => e.CmarCompId).HasColumnName("CMAR_CompId");
            entity.Property(e => e.CmarDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMAR_Desc");
            entity.Property(e => e.CmarEndValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMAR_EndValue");
            entity.Property(e => e.CmarId).HasColumnName("CMAR_ID");
            entity.Property(e => e.CmarIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMAR_IPAddress");
            entity.Property(e => e.CmarName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMAR_Name");
            entity.Property(e => e.CmarStartValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMAR_StartValue");
            entity.Property(e => e.CmarYearId).HasColumnName("CMAR_YearID");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCmarColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMAR_Color");
            entity.Property(e => e.NCmarDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMAR_Desc");
            entity.Property(e => e.NCmarEndValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMAR_EndValue");
            entity.Property(e => e.NCmarName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMAR_Name");
            entity.Property(e => e.NCmarStartValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMAR_StartValue");
            entity.Property(e => e.NCmarYearId).HasColumnName("nCMAR_YearID");
        });

        modelBuilder.Entity<CmaratingSupportProcess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating_SupportProcess");

            entity.Property(e => e.CmasrApprovedBy).HasColumnName("CMASR_ApprovedBy");
            entity.Property(e => e.CmasrApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMASR_ApprovedOn");
            entity.Property(e => e.CmasrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMASR_Color");
            entity.Property(e => e.CmasrCompId).HasColumnName("CMASR_CompId");
            entity.Property(e => e.CmasrCrBy).HasColumnName("CMASR_CrBy");
            entity.Property(e => e.CmasrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CMASR_CrOn");
            entity.Property(e => e.CmasrDeletedBy).HasColumnName("CMASR_DeletedBy");
            entity.Property(e => e.CmasrDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMASR_DeletedOn");
            entity.Property(e => e.CmasrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMASR_Desc");
            entity.Property(e => e.CmasrEndValue).HasColumnName("CMASR_EndValue");
            entity.Property(e => e.CmasrFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CMASR_FLAG");
            entity.Property(e => e.CmasrId).HasColumnName("CMASR_ID");
            entity.Property(e => e.CmasrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMASR_IPAddress");
            entity.Property(e => e.CmasrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMASR_Name");
            entity.Property(e => e.CmasrRecallBy).HasColumnName("CMASR_RecallBy");
            entity.Property(e => e.CmasrRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CMASR_RecallOn");
            entity.Property(e => e.CmasrStartValue).HasColumnName("CMASR_StartValue");
            entity.Property(e => e.CmasrStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CMASR_STATUS");
            entity.Property(e => e.CmasrUpdatedBy).HasColumnName("CMASR_UpdatedBy");
            entity.Property(e => e.CmasrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMASR_UpdatedOn");
            entity.Property(e => e.CmasrYearId).HasColumnName("CMASR_YearID");
        });

        modelBuilder.Entity<CmaratingSupportProcessLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CMARating_SupportProcess_log");

            entity.Property(e => e.CmasrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMASR_Color");
            entity.Property(e => e.CmasrCompId).HasColumnName("CMASR_CompId");
            entity.Property(e => e.CmasrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMASR_Desc");
            entity.Property(e => e.CmasrEndValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMASR_EndValue");
            entity.Property(e => e.CmasrId).HasColumnName("CMASR_ID");
            entity.Property(e => e.CmasrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMASR_IPAddress");
            entity.Property(e => e.CmasrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMASR_Name");
            entity.Property(e => e.CmasrStartValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMASR_StartValue");
            entity.Property(e => e.CmasrYearId).HasColumnName("CMASR_YearID");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCmasrColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMASR_Color");
            entity.Property(e => e.NCmasrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMASR_Desc");
            entity.Property(e => e.NCmasrEndValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMASR_EndValue");
            entity.Property(e => e.NCmasrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMASR_Name");
            entity.Property(e => e.NCmasrStartValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nCMASR_StartValue");
            entity.Property(e => e.NCmasrYearId).HasColumnName("nCMASR_YearID");
        });

        modelBuilder.Entity<CompanyLogoSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("company_logo_settings");

            entity.Property(e => e.ClsBigdata)
                .HasColumnType("image")
                .HasColumnName("CLS_BIGDATA");
            entity.Property(e => e.ClsCompId).HasColumnName("CLS_CompID");
            entity.Property(e => e.ClsExtn)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CLS_Extn");
            entity.Property(e => e.ClsFileName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CLS_FileName");
            entity.Property(e => e.ClsId).HasColumnName("CLS_ID");
            entity.Property(e => e.ClsSize).HasColumnName("CLS_SIZE");
            entity.Property(e => e.ClsStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CLS_Status");
        });

        modelBuilder.Entity<ComplianceChecklist>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_Checklist");

            entity.Property(e => e.CrcdAttchId).HasColumnName("CRCD_AttchID");
            entity.Property(e => e.CrcdCertId).HasColumnName("CRCD_CertID");
            entity.Property(e => e.CrcdCheckDesc)
                .IsUnicode(false)
                .HasColumnName("CRCD_CheckDesc");
            entity.Property(e => e.CrcdCheckId).HasColumnName("CRCD_CheckID");
            entity.Property(e => e.CrcdCheckRemarks)
                .IsUnicode(false)
                .HasColumnName("CRCD_CheckRemarks");
            entity.Property(e => e.CrcdCompId).HasColumnName("CRCD_CompID");
            entity.Property(e => e.CrcdControl)
                .IsUnicode(false)
                .HasColumnName("CRCD_Control");
            entity.Property(e => e.CrcdControlId).HasColumnName("CRCD_ControlID");
            entity.Property(e => e.CrcdInherentRiskId).HasColumnName("CRCD_InherentRiskID");
            entity.Property(e => e.CrcdIpaddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CRCD_IPAddress");
            entity.Property(e => e.CrcdMasId).HasColumnName("CRCD_MasID");
            entity.Property(e => e.CrcdOperation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CRCD_Operation");
            entity.Property(e => e.CrcdPid).HasColumnName("CRCD_PID");
            entity.Property(e => e.CrcdPkId).HasColumnName("CRCD_PkID");
            entity.Property(e => e.CrcdProcess)
                .IsUnicode(false)
                .HasColumnName("CRCD_Process");
            entity.Property(e => e.CrcdRisk)
                .IsUnicode(false)
                .HasColumnName("CRCD_Risk");
            entity.Property(e => e.CrcdRiskId).HasColumnName("CRCD_RiskID");
            entity.Property(e => e.CrcdRiskRemarks)
                .IsUnicode(false)
                .HasColumnName("CRCD_RiskRemarks");
            entity.Property(e => e.CrcdSubFunId).HasColumnName("CRCD_SubFunID");
            entity.Property(e => e.CrcdSubPid).HasColumnName("CRCD_SubPID");
            entity.Property(e => e.CrcdSunFunc)
                .IsUnicode(false)
                .HasColumnName("CRCD_SunFunc");
            entity.Property(e => e.CrcdSunProcess)
                .IsUnicode(false)
                .HasColumnName("CRCD_SunProcess");
            entity.Property(e => e.CrcdYearId).HasColumnName("CRCD_YearID");
        });

        modelBuilder.Entity<ComplianceChecklistMa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_Checklist_Mas");

            entity.Property(e => e.CrcmAttchId).HasColumnName("CRCM_AttchID");
            entity.Property(e => e.CrcmCompId).HasColumnName("CRCM_CompID");
            entity.Property(e => e.CrcmCrBy).HasColumnName("CRCM_CrBy");
            entity.Property(e => e.CrcmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CRCM_CrOn");
            entity.Property(e => e.CrcmCustId).HasColumnName("CRCM_CustID");
            entity.Property(e => e.CrcmFunId).HasColumnName("CRCM_FunID");
            entity.Property(e => e.CrcmId).HasColumnName("CRCM_ID");
            entity.Property(e => e.CrcmIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CRCM_IPAddress");
            entity.Property(e => e.CrcmJobId).HasColumnName("CRCM_JobID");
            entity.Property(e => e.CrcmOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CRCM_Operation");
            entity.Property(e => e.CrcmPgedetailId).HasColumnName("CRCM_PGEDetailId");
            entity.Property(e => e.CrcmStatus)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CRCM_Status");
            entity.Property(e => e.CrcmSubmittedBy).HasColumnName("CRCM_SubmittedBy");
            entity.Property(e => e.CrcmSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CRCM_SubmittedOn");
            entity.Property(e => e.CrcmUpdatedBy).HasColumnName("CRCM_UpdatedBy");
            entity.Property(e => e.CrcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CRCM_UpdatedOn");
            entity.Property(e => e.CrcmYearId).HasColumnName("CRCM_YearID");
        });

        modelBuilder.Entity<ComplianceIssueTrackerDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_IssueTracker_details");

            entity.Property(e => e.CitActionPlan)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CIT_ActionPlan");
            entity.Property(e => e.CitActualLoss)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CIT_ActualLoss");
            entity.Property(e => e.CitAttachId).HasColumnName("CIT_AttachID");
            entity.Property(e => e.CitCheckId).HasColumnName("CIT_CheckID");
            entity.Property(e => e.CitChecklistId).HasColumnName("CIT_ChecklistID");
            entity.Property(e => e.CitCompId).HasColumnName("CIT_CompID");
            entity.Property(e => e.CitComplianceCodeId).HasColumnName("CIT_ComplianceCodeID");
            entity.Property(e => e.CitControlId).HasColumnName("CIT_ControlID");
            entity.Property(e => e.CitCreatedBy).HasColumnName("CIT_CreatedBy");
            entity.Property(e => e.CitCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CIT_CreatedOn");
            entity.Property(e => e.CitCustomerId).HasColumnName("CIT_CustomerID");
            entity.Property(e => e.CitFunctionId).HasColumnName("CIT_FunctionID");
            entity.Property(e => e.CitFunctionManagerId).HasColumnName("CIT_FunctionManagerID");
            entity.Property(e => e.CitImpact)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CIT_Impact");
            entity.Property(e => e.CitIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CIT_IPAddress");
            entity.Property(e => e.CitIssueDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CIT_IssueDetails");
            entity.Property(e => e.CitIssueHeading)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CIT_IssueHeading");
            entity.Property(e => e.CitIssueJobNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CIT_IssueJobNo");
            entity.Property(e => e.CitIssueRatingId).HasColumnName("CIT_IssueRatingID");
            entity.Property(e => e.CitIssueStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CIT_IssueStatus");
            entity.Property(e => e.CitPgedetailId).HasColumnName("CIT_PGEDetailId");
            entity.Property(e => e.CitPkid).HasColumnName("CIT_PKID");
            entity.Property(e => e.CitProbableLoss)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CIT_ProbableLoss");
            entity.Property(e => e.CitProcessId).HasColumnName("CIT_ProcessID");
            entity.Property(e => e.CitRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CIT_Remarks");
            entity.Property(e => e.CitResponsibleFunctionId).HasColumnName("CIT_ResponsibleFunctionID");
            entity.Property(e => e.CitRiskId).HasColumnName("CIT_RiskID");
            entity.Property(e => e.CitRiskTypeId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CIT_RiskTypeID");
            entity.Property(e => e.CitStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CIT_Status");
            entity.Property(e => e.CitSubFunctionId).HasColumnName("CIT_SubFunctionID");
            entity.Property(e => e.CitSubProcessId).HasColumnName("CIT_SubProcessID");
            entity.Property(e => e.CitSubmittedBy).HasColumnName("CIT_SubmittedBy");
            entity.Property(e => e.CitSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CIT_SubmittedOn");
            entity.Property(e => e.CitTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("CIT_TargetDate");
            entity.Property(e => e.CitUpdatedBy).HasColumnName("CIT_UpdatedBy");
            entity.Property(e => e.CitUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CIT_UpdatedOn");
            entity.Property(e => e.CitYearId).HasColumnName("CIT_YearID");
        });

        modelBuilder.Entity<ComplianceIssueTrackerDetailsHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_IssueTracker_details_History");

            entity.Property(e => e.CithActionPlan)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CITH_ActionPlan");
            entity.Property(e => e.CithChecklistId).HasColumnName("CITH_ChecklistID");
            entity.Property(e => e.CithCitpkid).HasColumnName("CITH_CITPKID");
            entity.Property(e => e.CithCompId).HasColumnName("CITH_CompID");
            entity.Property(e => e.CithComplianceCodeId).HasColumnName("CITH_ComplianceCodeID");
            entity.Property(e => e.CithCreatedBy).HasColumnName("CITH_CreatedBy");
            entity.Property(e => e.CithCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CITH_CreatedOn");
            entity.Property(e => e.CithCustomerId).HasColumnName("CITH_CustomerID");
            entity.Property(e => e.CithFunctionManagerId).HasColumnName("CITH_FunctionManagerID");
            entity.Property(e => e.CithIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CITH_IPAddress");
            entity.Property(e => e.CithIssueStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CITH_IssueStatus");
            entity.Property(e => e.CithPkid).HasColumnName("CITH_PKID");
            entity.Property(e => e.CithRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CITH_Remarks");
            entity.Property(e => e.CithResponsibleFunctionId).HasColumnName("CITH_ResponsibleFunctionID");
            entity.Property(e => e.CithTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("CITH_TargetDate");
            entity.Property(e => e.CithUpdatedBy).HasColumnName("CITH_UpdatedBy");
            entity.Property(e => e.CithUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CITH_UpdatedOn");
        });

        modelBuilder.Entity<CompliancePlan>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_Plan");

            entity.Property(e => e.CpCompId).HasColumnName("CP_CompID");
            entity.Property(e => e.CpComplianceCode)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("CP_ComplianceCode");
            entity.Property(e => e.CpComplianceStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CP_ComplianceStatus");
            entity.Property(e => e.CpCustomerId).HasColumnName("CP_CustomerID");
            entity.Property(e => e.CpFunctionId).HasColumnName("CP_FunctionID");
            entity.Property(e => e.CpId).HasColumnName("CP_ID");
            entity.Property(e => e.CpInherentRiskId).HasColumnName("CP_InherentRiskID");
            entity.Property(e => e.CpIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CP_IPAddress");
            entity.Property(e => e.CpIsCurrentYear).HasColumnName("CP_IsCurrentYear");
            entity.Property(e => e.CpNetRatingId).HasColumnName("CP_NetRatingID");
            entity.Property(e => e.CpPlanCreatedBy).HasColumnName("CP_PlanCreatedBy");
            entity.Property(e => e.CpPlanCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_PlanCreatedOn");
            entity.Property(e => e.CpPlanStatus)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CP_PlanStatus");
            entity.Property(e => e.CpPlanSubmittedBy).HasColumnName("CP_PlanSubmittedBy");
            entity.Property(e => e.CpPlanSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_PlanSubmittedOn");
            entity.Property(e => e.CpPlanUpdatedBy).HasColumnName("CP_PlanUpdatedBy");
            entity.Property(e => e.CpPlanUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_PlanUpdatedOn");
            entity.Property(e => e.CpRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CP_Remarks");
            entity.Property(e => e.CpReportTitle)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CP_ReportTitle");
            entity.Property(e => e.CpScheduleCreatedBy).HasColumnName("CP_ScheduleCreatedBy");
            entity.Property(e => e.CpScheduleCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_ScheduleCreatedOn");
            entity.Property(e => e.CpScheduleStatus)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CP_ScheduleStatus");
            entity.Property(e => e.CpScheduleSubmittedBy).HasColumnName("CP_ScheduleSubmittedBy");
            entity.Property(e => e.CpScheduleSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_ScheduleSubmittedOn");
            entity.Property(e => e.CpScheduleUpdatedBy).HasColumnName("CP_ScheduleUpdatedBy");
            entity.Property(e => e.CpScheduleUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CP_ScheduleUpdatedOn");
            entity.Property(e => e.CpScheduledMonthId).HasColumnName("CP_ScheduledMonthID");
            entity.Property(e => e.CpStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CP_status");
            entity.Property(e => e.CpSubFunctionId).HasColumnName("CP_SubFunctionID");
            entity.Property(e => e.CpYearId).HasColumnName("CP_YearID");
        });

        modelBuilder.Entity<CompliancePlanHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Compliance_Plan_history");

            entity.Property(e => e.CphCompId).HasColumnName("CPH_CompID");
            entity.Property(e => e.CphCpid).HasColumnName("CPH_CPID");
            entity.Property(e => e.CphCreatedBy).HasColumnName("CPH_CreatedBy");
            entity.Property(e => e.CphCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CPH_CreatedOn");
            entity.Property(e => e.CphCustomerId).HasColumnName("CPH_CustomerID");
            entity.Property(e => e.CphFunctionId).HasColumnName("CPH_FunctionID");
            entity.Property(e => e.CphId).HasColumnName("CPH_ID");
            entity.Property(e => e.CphIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CPH_IPAddress");
            entity.Property(e => e.CphIsCurrentYear).HasColumnName("CPH_IsCurrentYear");
            entity.Property(e => e.CphRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CPH_Remarks");
            entity.Property(e => e.CphScheduledMonthId).HasColumnName("CPH_ScheduledMonthID");
            entity.Property(e => e.CphStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CPH_status");
            entity.Property(e => e.CphSubFunctionId).HasColumnName("CPH_SubFunctionID");
            entity.Property(e => e.CphYearId).HasColumnName("CPH_YearID");
        });

        modelBuilder.Entity<ContentManagementMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Content_Management_Master");

            entity.Property(e => e.CmmAct)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CMM_Act");
            entity.Property(e => e.CmmApprovedBy).HasColumnName("CMM_ApprovedBy");
            entity.Property(e => e.CmmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMM_ApprovedOn");
            entity.Property(e => e.CmmAudrptType).HasColumnName("cmm_AudrptType");
            entity.Property(e => e.CmmCategory)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("cmm_Category");
            entity.Property(e => e.CmmCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cmm_Code");
            entity.Property(e => e.CmmCompId).HasColumnName("CMM_CompID");
            entity.Property(e => e.CmmCrBy).HasColumnName("CMM_CrBy");
            entity.Property(e => e.CmmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CMM_CrOn");
            entity.Property(e => e.CmmDeletedBy).HasColumnName("CMM_DeletedBy");
            entity.Property(e => e.CmmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMM_DeletedOn");
            entity.Property(e => e.CmmDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cmm_Delflag");
            entity.Property(e => e.CmmDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("cmm_Desc");
            entity.Property(e => e.CmmHsnsac)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CMM_HSNSAC");
            entity.Property(e => e.CmmId).HasColumnName("cmm_ID");
            entity.Property(e => e.CmmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMM_IPAddress");
            entity.Property(e => e.CmmRate)
                .HasColumnType("money")
                .HasColumnName("cmm_Rate");
            entity.Property(e => e.CmmRecallBy).HasColumnName("CMM_RecallBy");
            entity.Property(e => e.CmmRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CMM_RecallOn");
            entity.Property(e => e.CmmRiskCategory).HasColumnName("CMM_RiskCategory");
            entity.Property(e => e.CmmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CMM_Status");
            entity.Property(e => e.CmmUpdatedBy).HasColumnName("CMM_UpdatedBy");
            entity.Property(e => e.CmmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CMM_UpdatedOn");
            entity.Property(e => e.CmsKeyComponent).HasColumnName("cms_KeyComponent");
            entity.Property(e => e.CmsModule)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cms_Module");
            entity.Property(e => e.CmsRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("cms_Remarks");
        });

        modelBuilder.Entity<ContentManagementMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Content_Management_Master_log");

            entity.Property(e => e.CmmCategory)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CMM_Category");
            entity.Property(e => e.CmmCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CMM_Code");
            entity.Property(e => e.CmmCompId).HasColumnName("CMM_CompID");
            entity.Property(e => e.CmmDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CMM_Desc");
            entity.Property(e => e.CmmId).HasColumnName("CMM_ID");
            entity.Property(e => e.CmmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CMM_IPAddress");
            entity.Property(e => e.CmmKeyComponent).HasColumnName("CMM_KeyComponent");
            entity.Property(e => e.CmmModule)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CMM_Module");
            entity.Property(e => e.CmmRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CMM_Remarks");
            entity.Property(e => e.CmmRiskCategory).HasColumnName("CMM_RiskCategory");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCmmCategory)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("nCMM_Category");
            entity.Property(e => e.NCmmCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nCMM_Code");
            entity.Property(e => e.NCmmDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nCMM_Desc");
            entity.Property(e => e.NCmmKeyComponent).HasColumnName("nCMM_KeyComponent");
            entity.Property(e => e.NCmmModule)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nCMM_Module");
            entity.Property(e => e.NCmmRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nCMM_Remarks");
            entity.Property(e => e.NCmmRiskCategory).HasColumnName("nCMM_RiskCategory");
        });

        modelBuilder.Entity<CrpaAuditAssest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_AuditAssest");

            entity.Property(e => e.CaAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CA_ADDRESS");
            entity.Property(e => e.CaAsgNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CA_AsgNo");
            entity.Property(e => e.CaAsubmittedBy).HasColumnName("CA_ASubmittedBy");
            entity.Property(e => e.CaAsubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CA_ASubmittedOn");
            entity.Property(e => e.CaAuditorname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CA_AUDITORNAME");
            entity.Property(e => e.CaBsubmittedBy).HasColumnName("CA_BSubmittedBy");
            entity.Property(e => e.CaBsubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("CA_BSubmittedOn");
            entity.Property(e => e.CaCompId).HasColumnName("CA_CompID");
            entity.Property(e => e.CaCrBy).HasColumnName("CA_CrBy");
            entity.Property(e => e.CaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CA_CrOn");
            entity.Property(e => e.CaDate)
                .HasColumnType("datetime")
                .HasColumnName("CA_Date");
            entity.Property(e => e.CaFinancialYear).HasColumnName("CA_FinancialYear");
            entity.Property(e => e.CaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CA_IPAddress");
            entity.Property(e => e.CaLocationid).HasColumnName("CA_LOCATIONID");
            entity.Property(e => e.CaNameOfOpsHead)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CA_NAME_OF_OPS_HEAD");
            entity.Property(e => e.CaNameOfUnitPresident)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CA_NAME_OF_UNIT_PRESIDENT");
            entity.Property(e => e.CaNetScore).HasColumnName("CA_NetScore");
            entity.Property(e => e.CaPkid).HasColumnName("CA_PKID");
            entity.Property(e => e.CaSectionid).HasColumnName("CA_SECTIONID");
            entity.Property(e => e.CaStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CA_Status");
            entity.Property(e => e.CaUpdatedBy).HasColumnName("CA_UpdatedBy");
            entity.Property(e => e.CaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CA_UpdatedOn");
        });

        modelBuilder.Entity<CrpaChecklistAuditAssest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_ChecklistAuditAssest");

            entity.Property(e => e.CradCauditId).HasColumnName("CRAD_CAuditID");
            entity.Property(e => e.CradComments)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CRAD_COMMENTS");
            entity.Property(e => e.CradCompId).HasColumnName("CRAD_CompID");
            entity.Property(e => e.CradCreatedby).HasColumnName("CRAD_CREATEDBY");
            entity.Property(e => e.CradCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("CRAD_CREATEDON");
            entity.Property(e => e.CradDate)
                .HasColumnType("datetime")
                .HasColumnName("CRAD_DATE");
            entity.Property(e => e.CradFindings).HasColumnName("CRAD_FINDINGS");
            entity.Property(e => e.CradIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CRAD_IPAddress");
            entity.Property(e => e.CradLocationid).HasColumnName("CRAD_LOCATIONID");
            entity.Property(e => e.CradPkid).HasColumnName("CRAD_PKID");
            entity.Property(e => e.CradProcessid).HasColumnName("CRAD_PROCESSID");
            entity.Property(e => e.CradScoreResult).HasColumnName("CRAD_SCORE_RESULT");
            entity.Property(e => e.CradScoreStandard).HasColumnName("CRAD_SCORE_STANDARD");
            entity.Property(e => e.CradSectionid).HasColumnName("CRAD_SECTIONID");
            entity.Property(e => e.CradSubprocessid).HasColumnName("CRAD_SUBPROCESSID");
            entity.Property(e => e.CradSubsectionid).HasColumnName("CRAD_SUBSECTIONID");
            entity.Property(e => e.CradUpdatedby).HasColumnName("CRAD_UPDATEDBY");
            entity.Property(e => e.CradUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CRAD_UPDATEDON");
            entity.Property(e => e.CradYearid).HasColumnName("CRAD_YEARID");
        });

        modelBuilder.Entity<CrpaProcess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_Process");

            entity.Property(e => e.CapApprovedby).HasColumnName("CAP_APPROVEDBY");
            entity.Property(e => e.CapApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CAP_APPROVEDON");
            entity.Property(e => e.CapCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAP_CODE");
            entity.Property(e => e.CapCompId).HasColumnName("CAP_CompId");
            entity.Property(e => e.CapCrby).HasColumnName("CAP_CRBY");
            entity.Property(e => e.CapCron)
                .HasColumnType("datetime")
                .HasColumnName("CAP_CRON");
            entity.Property(e => e.CapDeletedby).HasColumnName("CAP_DELETEDBY");
            entity.Property(e => e.CapDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("CAP_DELETEDON");
            entity.Property(e => e.CapDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CAP_DELFLG");
            entity.Property(e => e.CapDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CAP_Desc");
            entity.Property(e => e.CapId).HasColumnName("CAP_ID");
            entity.Property(e => e.CapIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CAP_IPAddress");
            entity.Property(e => e.CapPoints).HasColumnName("CAP_POINTS");
            entity.Property(e => e.CapProcessname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CAP_PROCESSNAME");
            entity.Property(e => e.CapRecallby).HasColumnName("CAP_RECALLBY");
            entity.Property(e => e.CapRecallon)
                .HasColumnType("datetime")
                .HasColumnName("CAP_RECALLON");
            entity.Property(e => e.CapSectionid).HasColumnName("CAP_SECTIONID");
            entity.Property(e => e.CapStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CAP_STATUS");
            entity.Property(e => e.CapSubsectionid).HasColumnName("CAP_SUBSECTIONID");
            entity.Property(e => e.CapUpdatedby).HasColumnName("CAP_UPDATEDBY");
            entity.Property(e => e.CapUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CAP_UPDATEDON");
            entity.Property(e => e.CapYearid).HasColumnName("CAP_YEARId");
        });

        modelBuilder.Entity<CrpaRating>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_Rating");

            entity.Property(e => e.CratApprovedBy).HasColumnName("CRAT_ApprovedBy");
            entity.Property(e => e.CratApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CRAT_ApprovedOn");
            entity.Property(e => e.CratAuditId).HasColumnName("CRAT_AuditID");
            entity.Property(e => e.CratColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CRAT_Color");
            entity.Property(e => e.CratCompId).HasColumnName("CRAT_CompId");
            entity.Property(e => e.CratCrBy).HasColumnName("CRAT_CrBy");
            entity.Property(e => e.CratCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CRAT_CrOn");
            entity.Property(e => e.CratDeletedBy).HasColumnName("CRAT_DeletedBy");
            entity.Property(e => e.CratDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CRAT_DeletedOn");
            entity.Property(e => e.CratDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CRAT_Desc");
            entity.Property(e => e.CratEndValue).HasColumnName("CRAT_EndValue");
            entity.Property(e => e.CratFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CRAT_FLAG");
            entity.Property(e => e.CratIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CRAT_IPAddress");
            entity.Property(e => e.CratName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CRAT_Name");
            entity.Property(e => e.CratPkid).HasColumnName("CRAT_PKID");
            entity.Property(e => e.CratRecallBy).HasColumnName("CRAT_RecallBy");
            entity.Property(e => e.CratRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CRAT_RecallOn");
            entity.Property(e => e.CratStartValue).HasColumnName("CRAT_StartValue");
            entity.Property(e => e.CratStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CRAT_STATUS");
            entity.Property(e => e.CratUpdatedBy).HasColumnName("CRAT_UpdatedBy");
            entity.Property(e => e.CratUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CRAT_UpdatedOn");
            entity.Property(e => e.CratYearId).HasColumnName("CRAT_YearID");
        });

        modelBuilder.Entity<CrpaSection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_Section");

            entity.Property(e => e.CasApprovedby).HasColumnName("CAS_APPROVEDBY");
            entity.Property(e => e.CasApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CAS_APPROVEDON");
            entity.Property(e => e.CasCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAS_CODE");
            entity.Property(e => e.CasCompId).HasColumnName("CAS_CompId");
            entity.Property(e => e.CasCrby).HasColumnName("CAS_CRBY");
            entity.Property(e => e.CasCron)
                .HasColumnType("datetime")
                .HasColumnName("CAS_CRON");
            entity.Property(e => e.CasDeletedby).HasColumnName("CAS_DELETEDBY");
            entity.Property(e => e.CasDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("CAS_DELETEDON");
            entity.Property(e => e.CasDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CAS_DELFLG");
            entity.Property(e => e.CasDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CAS_Desc");
            entity.Property(e => e.CasId).HasColumnName("CAS_ID");
            entity.Property(e => e.CasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CAS_IPAddress");
            entity.Property(e => e.CasPoints).HasColumnName("CAS_POINTS");
            entity.Property(e => e.CasRecallby).HasColumnName("CAS_RECALLBY");
            entity.Property(e => e.CasRecallon)
                .HasColumnType("datetime")
                .HasColumnName("CAS_RECALLON");
            entity.Property(e => e.CasSectionname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CAS_SECTIONNAME");
            entity.Property(e => e.CasStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CAS_STATUS");
            entity.Property(e => e.CasUpdatedby).HasColumnName("CAS_UPDATEDBY");
            entity.Property(e => e.CasUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CAS_UPDATEDON");
            entity.Property(e => e.CasYearid).HasColumnName("CAS_YEARId");
        });

        modelBuilder.Entity<CrpaSubProcess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_SubProcess");

            entity.Property(e => e.CaspApprovedby).HasColumnName("CASP_APPROVEDBY");
            entity.Property(e => e.CaspApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CASP_APPROVEDON");
            entity.Property(e => e.CaspCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CASP_CODE");
            entity.Property(e => e.CaspCompId).HasColumnName("CASP_CompId");
            entity.Property(e => e.CaspCrby).HasColumnName("CASP_CRBY");
            entity.Property(e => e.CaspCron)
                .HasColumnType("datetime")
                .HasColumnName("CASP_CRON");
            entity.Property(e => e.CaspDeletedby).HasColumnName("CASP_DELETEDBY");
            entity.Property(e => e.CaspDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("CASP_DELETEDON");
            entity.Property(e => e.CaspDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CASP_DELFLG");
            entity.Property(e => e.CaspDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CASP_Desc");
            entity.Property(e => e.CaspId).HasColumnName("CASP_ID");
            entity.Property(e => e.CaspIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CASP_IPAddress");
            entity.Property(e => e.CaspPoints).HasColumnName("CASP_POINTS");
            entity.Property(e => e.CaspProcessid).HasColumnName("CASP_PROCESSID");
            entity.Property(e => e.CaspRecallby).HasColumnName("CASP_RECALLBY");
            entity.Property(e => e.CaspRecallon)
                .HasColumnType("datetime")
                .HasColumnName("CASP_RECALLON");
            entity.Property(e => e.CaspSectionid).HasColumnName("CASP_SECTIONID");
            entity.Property(e => e.CaspStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CASP_STATUS");
            entity.Property(e => e.CaspSubprocessname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CASP_SUBPROCESSNAME");
            entity.Property(e => e.CaspSubsectionid).HasColumnName("CASP_SUBSECTIONID");
            entity.Property(e => e.CaspUpdatedby).HasColumnName("CASP_UPDATEDBY");
            entity.Property(e => e.CaspUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CASP_UPDATEDON");
            entity.Property(e => e.CaspYearid).HasColumnName("CASP_YEARId");
        });

        modelBuilder.Entity<CrpaSubSection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_SubSection");

            entity.Property(e => e.CasuApprovedby).HasColumnName("CASU_APPROVEDBY");
            entity.Property(e => e.CasuApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CASU_APPROVEDON");
            entity.Property(e => e.CasuCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CASU_CODE");
            entity.Property(e => e.CasuCompId).HasColumnName("CASU_CompId");
            entity.Property(e => e.CasuCrby).HasColumnName("CASU_CRBY");
            entity.Property(e => e.CasuCron)
                .HasColumnType("datetime")
                .HasColumnName("CASU_CRON");
            entity.Property(e => e.CasuDeletedby).HasColumnName("CASU_DELETEDBY");
            entity.Property(e => e.CasuDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("CASU_DELETEDON");
            entity.Property(e => e.CasuDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CASU_DELFLG");
            entity.Property(e => e.CasuDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CASU_Desc");
            entity.Property(e => e.CasuId).HasColumnName("CASU_ID");
            entity.Property(e => e.CasuIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CASU_IPAddress");
            entity.Property(e => e.CasuPoints).HasColumnName("CASU_Points");
            entity.Property(e => e.CasuRecallby).HasColumnName("CASU_RECALLBY");
            entity.Property(e => e.CasuRecallon)
                .HasColumnType("datetime")
                .HasColumnName("CASU_RECALLON");
            entity.Property(e => e.CasuSectionid)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CASU_SECTIONID");
            entity.Property(e => e.CasuStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CASU_STATUS");
            entity.Property(e => e.CasuSubsectionname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CASU_SUBSECTIONNAME");
            entity.Property(e => e.CasuUpdatedby).HasColumnName("CASU_UPDATEDBY");
            entity.Property(e => e.CasuUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CASU_UPDATEDON");
            entity.Property(e => e.CasuYearid).HasColumnName("CASU_YEARId");
        });

        modelBuilder.Entity<CrpaValueRating>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CRPA_ValueRating");

            entity.Property(e => e.CvrApprovedBy).HasColumnName("CVR_ApprovedBy");
            entity.Property(e => e.CvrApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CVR_ApprovedOn");
            entity.Property(e => e.CvrAuditId).HasColumnName("CVR_AuditId");
            entity.Property(e => e.CvrCompId).HasColumnName("CVR_CompId");
            entity.Property(e => e.CvrCrBy).HasColumnName("CVR_CrBy");
            entity.Property(e => e.CvrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CVR_CrOn");
            entity.Property(e => e.CvrDeletedBy).HasColumnName("CVR_DeletedBy");
            entity.Property(e => e.CvrDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CVR_DeletedOn");
            entity.Property(e => e.CvrDesc)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CVR_Desc");
            entity.Property(e => e.CvrFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CVR_FLAG");
            entity.Property(e => e.CvrId).HasColumnName("CVR_ID");
            entity.Property(e => e.CvrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CVR_IPAddress");
            entity.Property(e => e.CvrName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("CVR_Name");
            entity.Property(e => e.CvrPoint)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CVR_Point");
            entity.Property(e => e.CvrRecallBy).HasColumnName("CVR_RecallBy");
            entity.Property(e => e.CvrRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CVR_RecallOn");
            entity.Property(e => e.CvrStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CVR_STATUS");
            entity.Property(e => e.CvrUpdatedBy).HasColumnName("CVR_UpdatedBy");
            entity.Property(e => e.CvrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CVR_UpdatedOn");
            entity.Property(e => e.CvrYearId).HasColumnName("CVR_YearID");
        });

        modelBuilder.Entity<CustFontstyle>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Cust_fontstyle");

            entity.Property(e => e.CfCompId).HasColumnName("CF_CompId");
            entity.Property(e => e.CfCrby).HasColumnName("CF_CRBY");
            entity.Property(e => e.CfCron)
                .HasColumnType("datetime")
                .HasColumnName("CF_CRON");
            entity.Property(e => e.CfCustId).HasColumnName("CF_CustId");
            entity.Property(e => e.CfDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CF_DELFLAG");
            entity.Property(e => e.CfId).HasColumnName("CF_ID");
            entity.Property(e => e.CfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CF_IPAddress");
            entity.Property(e => e.CfName)
                .IsUnicode(false)
                .HasColumnName("CF_name");
            entity.Property(e => e.CfStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CF_STATUS");
            entity.Property(e => e.CfUpdatedby).HasColumnName("CF_UPDATEDBY");
            entity.Property(e => e.CfUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CF_UPDATEDON");
            entity.Property(e => e.CfYearid).HasColumnName("CF_YEARId");
        });

        modelBuilder.Entity<CustomerCoa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Customer_COA");

            entity.Property(e => e.CcAccHead).HasColumnName("CC_AccHead");
            entity.Property(e => e.CcCloseCredit)
                .HasColumnType("money")
                .HasColumnName("CC_CloseCredit");
            entity.Property(e => e.CcCloseDebit)
                .HasColumnType("money")
                .HasColumnName("CC_CloseDebit");
            entity.Property(e => e.CcCompId).HasColumnName("CC_CompID");
            entity.Property(e => e.CcCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CC_CreatedOn");
            entity.Property(e => e.CcCreatedby).HasColumnName("CC_Createdby");
            entity.Property(e => e.CcCustId).HasColumnName("CC_CustID");
            entity.Property(e => e.CcGl).HasColumnName("CC_GL");
            entity.Property(e => e.CcGlcode)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CC_GLCode");
            entity.Property(e => e.CcGldesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CC_GLDesc");
            entity.Property(e => e.CcHead).HasColumnName("CC_Head");
            entity.Property(e => e.CcId).HasColumnName("CC_ID");
            entity.Property(e => e.CcIndType).HasColumnName("CC_IndType");
            entity.Property(e => e.CcIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CC_IPAddress");
            entity.Property(e => e.CcObcredit)
                .HasColumnType("money")
                .HasColumnName("CC_OBCredit");
            entity.Property(e => e.CcObdebit)
                .HasColumnType("money")
                .HasColumnName("CC_OBDebit");
            entity.Property(e => e.CcOperation)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CC_Operation");
            entity.Property(e => e.CcParent).HasColumnName("CC_Parent");
            entity.Property(e => e.CcStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CC_Status");
            entity.Property(e => e.CcTrCredit)
                .HasColumnType("money")
                .HasColumnName("CC_TrCredit");
            entity.Property(e => e.CcTrDebit)
                .HasColumnType("money")
                .HasColumnName("CC_TrDebit");
            entity.Property(e => e.CcYearId).HasColumnName("CC_YearID");
        });

        modelBuilder.Entity<CustomerGlLinkageMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CustomerGL_Linkage_Master");

            entity.Property(e => e.ClmCompId).HasColumnName("CLM_CompID");
            entity.Property(e => e.ClmCreatedBy).HasColumnName("CLM_CreatedBy");
            entity.Property(e => e.ClmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CLM_CreatedOn");
            entity.Property(e => e.ClmCustId).HasColumnName("CLM_CustID");
            entity.Property(e => e.ClmDeletedBy).HasColumnName("CLM_DeletedBy");
            entity.Property(e => e.ClmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CLM_DeletedOn");
            entity.Property(e => e.ClmGl).HasColumnName("CLM_GL");
            entity.Property(e => e.ClmGlid).HasColumnName("CLM_GLID");
            entity.Property(e => e.ClmGlledger)
                .IsUnicode(false)
                .HasColumnName("CLM_GLLedger");
            entity.Property(e => e.ClmGroupId).HasColumnName("CLM_GroupID");
            entity.Property(e => e.ClmHead).HasColumnName("CLM_Head");
            entity.Property(e => e.ClmId).HasColumnName("CLM_ID");
            entity.Property(e => e.ClmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CLM_IPAddress");
            entity.Property(e => e.ClmOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CLM_Operation");
            entity.Property(e => e.ClmOrgId).HasColumnName("CLM_OrgID");
            entity.Property(e => e.ClmStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CLM_Status");
            entity.Property(e => e.ClmSubGl).HasColumnName("CLM_SubGL");
            entity.Property(e => e.ClmSubGroupId).HasColumnName("CLM_SubGroupID");
            entity.Property(e => e.ClmUpdatedBy).HasColumnName("CLM_UpdatedBy");
            entity.Property(e => e.ClmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CLM_UpdatedOn");
            entity.Property(e => e.ClmYearId).HasColumnName("CLM_YearID");
        });

        modelBuilder.Entity<DocReviewremark>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Doc_Reviewremarks");

            entity.Property(e => e.DrCompId).HasColumnName("DR_CompId");
            entity.Property(e => e.DrCreatedBy).HasColumnName("DR_CreatedBy");
            entity.Property(e => e.DrCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("DR_CreatedOn");
            entity.Property(e => e.DrCustid).HasColumnName("DR_Custid");
            entity.Property(e => e.DrDate)
                .HasColumnType("datetime")
                .HasColumnName("DR_Date");
            entity.Property(e => e.DrDocDelflag)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DR_DocDelflag");
            entity.Property(e => e.DrDocLoeIdBranchid).HasColumnName("DR_DocLoeId_Branchid");
            entity.Property(e => e.DrDocStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DR_DocStatus");
            entity.Property(e => e.DrDocType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DR_DocType");
            entity.Property(e => e.DrDocYearid).HasColumnName("DR_DocYearid");
            entity.Property(e => e.DrEmailSentTo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DR_emailSentTo");
            entity.Property(e => e.DrId).HasColumnName("DR_ID");
            entity.Property(e => e.DrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DR_IPAddress");
            entity.Property(e => e.DrObservation)
                .IsUnicode(false)
                .HasColumnName("DR_Observation");
            entity.Property(e => e.DrUpdatedBy).HasColumnName("DR_UpdatedBy");
            entity.Property(e => e.DrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("DR_UpdatedOn");
        });

        modelBuilder.Entity<DocReviewremarksHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Doc_Reviewremarks_History");

            entity.Property(e => e.DrhAttchmentid).HasColumnName("DRH_attchmentid");
            entity.Property(e => e.DrhCompId).HasColumnName("DRH_CompID");
            entity.Property(e => e.DrhCustid).HasColumnName("DRH_Custid");
            entity.Property(e => e.DrhDate)
                .HasColumnType("datetime")
                .HasColumnName("DRH_Date");
            entity.Property(e => e.DrhId).HasColumnName("DRH_ID");
            entity.Property(e => e.DrhIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DRH_IPAddress");
            entity.Property(e => e.DrhLoeid).HasColumnName("DRH_Loeid");
            entity.Property(e => e.DrhMasid).HasColumnName("DRH_MASid");
            entity.Property(e => e.DrhRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("DRH_Remarks");
            entity.Property(e => e.DrhRemarksBy).HasColumnName("DRH_RemarksBy");
            entity.Property(e => e.DrhRemarksType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DRH_RemarksType");
            entity.Property(e => e.DrhStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DRH_Status");
            entity.Property(e => e.DrhYearid).HasColumnName("DRH_Yearid");
        });

        modelBuilder.Entity<DocumentTray>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DocumentTray");

            entity.Property(e => e.CrOn).HasColumnType("datetime");
            entity.Property(e => e.Delflag)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DocFormat).IsUnicode(false);
            entity.Property(e => e.DocId).HasColumnName("DocID");
            entity.Property(e => e.Title).IsUnicode(false);
        });

        modelBuilder.Entity<EdtAnnotationDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_Annotation_Details");

            entity.Property(e => e.EadCompId).HasColumnName("EAD_CompID");
            entity.Property(e => e.EadCreatedBy).HasColumnName("EAD_CreatedBy");
            entity.Property(e => e.EadCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EAD_CreatedOn");
            entity.Property(e => e.EadDocumentId).HasColumnName("EAD_DocumentID");
            entity.Property(e => e.EadExt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EAD_EXT");
            entity.Property(e => e.EadFileId).HasColumnName("EAD_FileID");
            entity.Property(e => e.EadIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EAD_IPAddress");
            entity.Property(e => e.EadOle)
                .HasMaxLength(1)
                .HasColumnName("EAD_OLE");
            entity.Property(e => e.EadOriginalName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EAD_OriginalName");
            entity.Property(e => e.EadPkid).HasColumnName("EAD_PKID");
            entity.Property(e => e.EadSize).HasColumnName("EAD_SIZE");
        });

        modelBuilder.Entity<EdtAttachment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Edt_Attachments");

            entity.Property(e => e.AtchAuditId).HasColumnName("ATCH_AuditID");
            entity.Property(e => e.AtchAudscheduleId).HasColumnName("ATCH_AUDScheduleID");
            entity.Property(e => e.AtchBasename).HasColumnName("ATCH_Basename");
            entity.Property(e => e.AtchCompId).HasColumnName("ATCH_CompID");
            entity.Property(e => e.AtchCreatedby).HasColumnName("ATCH_CREATEDBY");
            entity.Property(e => e.AtchCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("ATCH_CREATEDON");
            entity.Property(e => e.AtchDesc)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("ATCH_Desc");
            entity.Property(e => e.AtchDocid).HasColumnName("ATCH_DOCID");
            entity.Property(e => e.AtchDrlid).HasColumnName("ATCH_drlid");
            entity.Property(e => e.AtchExt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ATCH_EXT");
            entity.Property(e => e.AtchFlag).HasColumnName("ATCH_FLAG");
            entity.Property(e => e.AtchFname)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ATCH_FNAME");
            entity.Property(e => e.AtchFrom)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ATCH_FROM");
            entity.Property(e => e.AtchId).HasColumnName("ATCH_ID");
            entity.Property(e => e.AtchModifiedby).HasColumnName("ATCH_MODIFIEDBY");
            entity.Property(e => e.AtchOle)
                .HasMaxLength(1)
                .HasColumnName("ATCH_OLE");
            entity.Property(e => e.AtchReportType).HasColumnName("ATCH_ReportType");
            entity.Property(e => e.AtchSize).HasColumnName("ATCH_SIZE");
            entity.Property(e => e.AtchStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ATCH_Status");
            entity.Property(e => e.AtchSubProcessId).HasColumnName("ATCH_SubProcessID");
            entity.Property(e => e.AtchVersion).HasColumnName("ATCH_VERSION");
            entity.Property(e => e.AtchVstatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Atch_Vstatus");
        });

        modelBuilder.Entity<EdtBigdatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_BIGDATA");

            entity.Property(e => e.BdtBasename)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("BDT_BASENAME");
            entity.Property(e => e.BdtBigdata)
                .HasColumnType("image")
                .HasColumnName("BDT_BIGDATA");
            entity.Property(e => e.BdtSize).HasColumnName("BDT_SIZE");
        });

        modelBuilder.Entity<EdtCabinet>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_Cabinet");

            entity.Property(e => e.CbnApprovedBy).HasColumnName("CBN_ApprovedBy");
            entity.Property(e => e.CbnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_ApprovedOn");
            entity.Property(e => e.CbnCompId).HasColumnName("CBN_CompID");
            entity.Property(e => e.CbnCreatedBy).HasColumnName("CBN_CreatedBy");
            entity.Property(e => e.CbnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CreatedOn");
            entity.Property(e => e.CbnDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBN_DelFlag");
            entity.Property(e => e.CbnDeletedBy).HasColumnName("CBN_DeletedBy");
            entity.Property(e => e.CbnDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_DeletedOn");
            entity.Property(e => e.CbnDepartment).HasColumnName("CBN_Department");
            entity.Property(e => e.CbnFolderCount).HasColumnName("CBN_FolderCount");
            entity.Property(e => e.CbnId).HasColumnName("CBN_ID");
            entity.Property(e => e.CbnName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_Name");
            entity.Property(e => e.CbnNote)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnParent).HasColumnName("CBN_Parent");
            entity.Property(e => e.CbnRecalledBy).HasColumnName("CBN_RecalledBy");
            entity.Property(e => e.CbnRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_RecalledOn");
            entity.Property(e => e.CbnRetention)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CBN_Retention");
            entity.Property(e => e.CbnStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CBN_Status");
            entity.Property(e => e.CbnSubCabCount).HasColumnName("CBN_SubCabCount");
            entity.Property(e => e.CbnUpdatedBy).HasColumnName("CBN_UpdatedBy");
            entity.Property(e => e.CbnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_UpdatedOn");
            entity.Property(e => e.CbnUserId).HasColumnName("CBN_UserID");
        });

        modelBuilder.Entity<EdtCabinet1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_CABINET1");

            entity.Property(e => e.CbnCron)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CRON");
            entity.Property(e => e.CbnDelStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("cbn_DelStatus");
            entity.Property(e => e.CbnFolCount)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_FolCount");
            entity.Property(e => e.CbnMailcabinet)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_MAILCABINET");
            entity.Property(e => e.CbnName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CBN_NAME");
            entity.Property(e => e.CbnNode)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_NODE");
            entity.Property(e => e.CbnNote)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("cbn_Operation");
            entity.Property(e => e.CbnOperationBy).HasColumnName("cbn_OperationBy");
            entity.Property(e => e.CbnParGrp)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_ParGrp");
            entity.Property(e => e.CbnParent)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_PARENT");
            entity.Property(e => e.CbnPermission)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_PERMISSION");
            entity.Property(e => e.CbnSccount)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_SCCount");
            entity.Property(e => e.CbnUsergroup)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_USERGROUP");
            entity.Property(e => e.CbnUserid)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_USERID");
        });

        modelBuilder.Entity<EdtCabinet2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_CABINET2");

            entity.Property(e => e.CbnCron)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CRON");
            entity.Property(e => e.CbnDelStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("cbn_DelStatus");
            entity.Property(e => e.CbnFolCount)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_FolCount");
            entity.Property(e => e.CbnMailcabinet)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_MAILCABINET");
            entity.Property(e => e.CbnName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CBN_NAME");
            entity.Property(e => e.CbnNode)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_NODE");
            entity.Property(e => e.CbnNote)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("cbn_Operation");
            entity.Property(e => e.CbnOperationBy).HasColumnName("cbn_OperationBy");
            entity.Property(e => e.CbnParGrp)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_ParGrp");
            entity.Property(e => e.CbnParent)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_PARENT");
            entity.Property(e => e.CbnPermission)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_PERMISSION");
            entity.Property(e => e.CbnSccount)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_SCCount");
            entity.Property(e => e.CbnUsergroup)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_USERGROUP");
            entity.Property(e => e.CbnUserid)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CBN_USERID");
        });

        modelBuilder.Entity<EdtCabinet23082024>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_Cabinet23082024");

            entity.Property(e => e.CbnApprovedBy).HasColumnName("CBN_ApprovedBy");
            entity.Property(e => e.CbnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_ApprovedOn");
            entity.Property(e => e.CbnCompId).HasColumnName("CBN_CompID");
            entity.Property(e => e.CbnCreatedBy).HasColumnName("CBN_CreatedBy");
            entity.Property(e => e.CbnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CreatedOn");
            entity.Property(e => e.CbnDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBN_DelFlag");
            entity.Property(e => e.CbnDeletedBy).HasColumnName("CBN_DeletedBy");
            entity.Property(e => e.CbnDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_DeletedOn");
            entity.Property(e => e.CbnDepartment).HasColumnName("CBN_Department");
            entity.Property(e => e.CbnFolderCount).HasColumnName("CBN_FolderCount");
            entity.Property(e => e.CbnId).HasColumnName("CBN_ID");
            entity.Property(e => e.CbnName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_Name");
            entity.Property(e => e.CbnNote)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnParent).HasColumnName("CBN_Parent");
            entity.Property(e => e.CbnRecalledBy).HasColumnName("CBN_RecalledBy");
            entity.Property(e => e.CbnRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_RecalledOn");
            entity.Property(e => e.CbnStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CBN_Status");
            entity.Property(e => e.CbnSubCabCount).HasColumnName("CBN_SubCabCount");
            entity.Property(e => e.CbnUpdatedBy).HasColumnName("CBN_UpdatedBy");
            entity.Property(e => e.CbnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_UpdatedOn");
            entity.Property(e => e.CbnUserId).HasColumnName("CBN_UserID");
        });

        modelBuilder.Entity<EdtCabinetPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_cabinet_Permission");

            entity.Property(e => e.CbpCabinet).HasColumnName("CBP_Cabinet");
            entity.Property(e => e.CbpCreate).HasColumnName("CBP_Create");
            entity.Property(e => e.CbpCreateFolder).HasColumnName("CBP_CreateFolder");
            entity.Property(e => e.CbpDelete).HasColumnName("CBP_Delete");
            entity.Property(e => e.CbpDepartment).HasColumnName("CBP_Department");
            entity.Property(e => e.CbpId).HasColumnName("CBP_ID");
            entity.Property(e => e.CbpIndex).HasColumnName("CBP_Index");
            entity.Property(e => e.CbpModify).HasColumnName("CBP_Modify");
            entity.Property(e => e.CbpOther).HasColumnName("CBP_Other");
            entity.Property(e => e.CbpPermissionType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBP_PermissionType");
            entity.Property(e => e.CbpSearch).HasColumnName("CBP_Search");
            entity.Property(e => e.CbpUser).HasColumnName("CBP_User");
            entity.Property(e => e.CbpView).HasColumnName("CBP_View");
        });

        modelBuilder.Entity<EdtCollate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_collate");

            entity.Property(e => e.CltAllow)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("CLT_ALLOW");
            entity.Property(e => e.CltApprovedby).HasColumnName("CLT_APPROVEDBY");
            entity.Property(e => e.CltApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CLT_APPROVEDON");
            entity.Property(e => e.CltCollateno)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("CLT_COLLATENO");
            entity.Property(e => e.CltCollateref)
                .HasMaxLength(200)
                .HasColumnName("CLT_COLLATEREF");
            entity.Property(e => e.CltComment)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CLT_Comment");
            entity.Property(e => e.CltCompId).HasColumnName("CLT_CompId");
            entity.Property(e => e.CltCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("CLT_CREATEDON");
            entity.Property(e => e.CltCreator)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("CLT_CREATOR");
            entity.Property(e => e.CltDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CLT_DelFlag");
            entity.Property(e => e.CltDeletedby).HasColumnName("CLT_DELETEDBY");
            entity.Property(e => e.CltDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("CLT_DELETEDON");
            entity.Property(e => e.CltGroup).HasColumnName("clt_Group");
            entity.Property(e => e.CltIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CLT_IPAddress");
            entity.Property(e => e.CltOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("clt_operation");
            entity.Property(e => e.CltOperationby).HasColumnName("clt_operationby");
            entity.Property(e => e.CltRecallby).HasColumnName("CLT_RECALLBY");
            entity.Property(e => e.CltRecallon)
                .HasColumnType("datetime")
                .HasColumnName("CLT_RECALLON");
            entity.Property(e => e.CltUpdatedby).HasColumnName("CLT_UPDATEDBY");
            entity.Property(e => e.CltUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("CLT_UPDATEDON");
        });

        modelBuilder.Entity<EdtCollateLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_COLLATE_log");

            entity.Property(e => e.CltAllow).HasColumnName("CLT_ALLOW");
            entity.Property(e => e.CltCollateno).HasColumnName("CLT_COLLATENO");
            entity.Property(e => e.CltCollateref)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CLT_COLLATEREF");
            entity.Property(e => e.CltComment)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CLT_Comment");
            entity.Property(e => e.CltCompId).HasColumnName("CLT_CompId");
            entity.Property(e => e.CltDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CLT_DelFlag");
            entity.Property(e => e.CltGroup).HasColumnName("clt_Group");
            entity.Property(e => e.CltIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CLT_IPAddress");
            entity.Property(e => e.CltOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("clt_operation");
            entity.Property(e => e.CltOperationby).HasColumnName("clt_operationby");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NCltAllow).HasColumnName("nCLT_ALLOW");
            entity.Property(e => e.NCltCollateref)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nCLT_COLLATEREF");
            entity.Property(e => e.NCltComment)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nCLT_Comment");
            entity.Property(e => e.NCltDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("nCLT_DelFlag");
            entity.Property(e => e.NcltGroup).HasColumnName("nclt_Group");
            entity.Property(e => e.NcltOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nclt_operation");
            entity.Property(e => e.NcltOperationby).HasColumnName("nclt_operationby");
        });

        modelBuilder.Entity<EdtCollatedoc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_collatedoc");

            entity.Property(e => e.CldCollateno)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("CLD_COLLATENO");
            entity.Property(e => e.CldDocid)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("CLD_DOCID");
            entity.Property(e => e.CldPageid)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("CLD_PAGEID");
        });

        modelBuilder.Entity<EdtDescType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DESC_TYPE");

            entity.Property(e => e.DtDataType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DT_DataType");
            entity.Property(e => e.DtId).HasColumnName("DT_ID");
            entity.Property(e => e.DtName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DT_Name");
            entity.Property(e => e.DtSize).HasColumnName("DT_Size");
            entity.Property(e => e.DtStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DT_Status");
            entity.Property(e => e.DtValue)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DT_Value");
        });

        modelBuilder.Entity<EdtDescriptio>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DESCRIPTIOS");

            entity.Property(e => e.DesId).HasColumnName("DES_ID");
            entity.Property(e => e.DescApprovedby).HasColumnName("DESC_APPROVEDBY");
            entity.Property(e => e.DescApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_APPROVEDON");
            entity.Property(e => e.DescCompId).HasColumnName("DESC_CompId");
            entity.Property(e => e.DescCrby).HasColumnName("DESC_CRBY");
            entity.Property(e => e.DescCron)
                .HasColumnType("datetime")
                .HasColumnName("DESC_CRON");
            entity.Property(e => e.DescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DESC_DATATYPE");
            entity.Property(e => e.DescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_DelFlag");
            entity.Property(e => e.DescDeletedby).HasColumnName("DESC_DELETEDBY");
            entity.Property(e => e.DescDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_DELETEDON");
            entity.Property(e => e.DescIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESC_IPAddress");
            entity.Property(e => e.DescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESC_NAME");
            entity.Property(e => e.DescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_NOTE");
            entity.Property(e => e.DescRecallby).HasColumnName("DESC_RECALLBY");
            entity.Property(e => e.DescRecallon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_RECALLON");
            entity.Property(e => e.DescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("DESC_SIZE");
            entity.Property(e => e.DescStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DESC_STATUS");
            entity.Property(e => e.DescUpdatedby).HasColumnName("DESC_UPDATEDBY");
            entity.Property(e => e.DescUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_UPDATEDON");
            entity.Property(e => e.EddDefaultValues)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("EDD_DefaultValues");
            entity.Property(e => e.EddPk).HasColumnName("EDD_Pk");
        });

        modelBuilder.Entity<EdtDescriptiosLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DESCRIPTIOS_log");

            entity.Property(e => e.DesId).HasColumnName("DES_ID");
            entity.Property(e => e.DescCompId).HasColumnName("DESC_CompId");
            entity.Property(e => e.DescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DESC_DATATYPE");
            entity.Property(e => e.DescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_DelFlag");
            entity.Property(e => e.DescIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESC_IPAddress");
            entity.Property(e => e.DescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESC_NAME");
            entity.Property(e => e.DescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_NOTE");
            entity.Property(e => e.DescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("DESC_SIZE");
            entity.Property(e => e.EddPk).HasColumnName("EDD_Pk");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NDescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nDESC_DATATYPE");
            entity.Property(e => e.NDescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("nDESC_DelFlag");
            entity.Property(e => e.NDescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nDESC_NAME");
            entity.Property(e => e.NDescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nDESC_NOTE");
            entity.Property(e => e.NDescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("nDESC_SIZE");
            entity.Property(e => e.NEddPk).HasColumnName("nEDD_Pk");
        });

        modelBuilder.Entity<EdtDescriptor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DESCRIPTOR");

            entity.Property(e => e.DesId).HasColumnName("DES_ID");
            entity.Property(e => e.DescApprovedby).HasColumnName("DESC_APPROVEDBY");
            entity.Property(e => e.DescApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_APPROVEDON");
            entity.Property(e => e.DescCompId).HasColumnName("DESC_CompId");
            entity.Property(e => e.DescCrby).HasColumnName("DESC_CRBY");
            entity.Property(e => e.DescCron)
                .HasColumnType("datetime")
                .HasColumnName("DESC_CRON");
            entity.Property(e => e.DescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DESC_DATATYPE");
            entity.Property(e => e.DescDefaultValues)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("DESC_DefaultValues");
            entity.Property(e => e.DescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_DelFlag");
            entity.Property(e => e.DescDeletedby).HasColumnName("DESC_DELETEDBY");
            entity.Property(e => e.DescDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_DELETEDON");
            entity.Property(e => e.DescIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESC_IPAddress");
            entity.Property(e => e.DescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESC_NAME");
            entity.Property(e => e.DescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_NOTE");
            entity.Property(e => e.DescRecallby).HasColumnName("DESC_RECALLBY");
            entity.Property(e => e.DescRecallon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_RECALLON");
            entity.Property(e => e.DescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("DESC_SIZE");
            entity.Property(e => e.DescStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_STATUS");
            entity.Property(e => e.DescUpdatedby).HasColumnName("DESC_UPDATEDBY");
            entity.Property(e => e.DescUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("DESC_UPDATEDON");
        });

        modelBuilder.Entity<EdtDescriptorLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DESCRIPTOR_log");

            entity.Property(e => e.DesId).HasColumnName("DES_ID");
            entity.Property(e => e.DescCompId).HasColumnName("DESC_CompId");
            entity.Property(e => e.DescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("DESC_DATATYPE");
            entity.Property(e => e.DescDefaultValues)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("DESC_DefaultValues");
            entity.Property(e => e.DescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_DelFlag");
            entity.Property(e => e.DescIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESC_IPAddress");
            entity.Property(e => e.DescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESC_NAME");
            entity.Property(e => e.DescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_NOTE");
            entity.Property(e => e.DescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("DESC_SIZE");
            entity.Property(e => e.DescStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DESC_STATUS");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NDescDatatype)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nDESC_DATATYPE");
            entity.Property(e => e.NDescDefaultValues)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nDESC_DefaultValues");
            entity.Property(e => e.NDescDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("nDESC_DelFlag");
            entity.Property(e => e.NDescName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nDESC_NAME");
            entity.Property(e => e.NDescNote)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nDESC_NOTE");
            entity.Property(e => e.NDescSize)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("nDESC_SIZE");
            entity.Property(e => e.NDescStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("nDESC_STATUS");
        });

        modelBuilder.Entity<EdtDoctypeLink>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DOCTYPE_LINK");

            entity.Property(e => e.EddApprovedby).HasColumnName("EDD_APPROVEDBY");
            entity.Property(e => e.EddApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("EDD_APPROVEDON");
            entity.Property(e => e.EddCompId).HasColumnName("EDD_CompId");
            entity.Property(e => e.EddCrby).HasColumnName("EDD_CRBY");
            entity.Property(e => e.EddCron)
                .HasColumnType("datetime")
                .HasColumnName("EDD_CRON");
            entity.Property(e => e.EddDeletedby).HasColumnName("EDD_DELETEDBY");
            entity.Property(e => e.EddDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("EDD_DELETEDON");
            entity.Property(e => e.EddDoctypeid).HasColumnName("EDD_DOCTYPEID");
            entity.Property(e => e.EddDptrid).HasColumnName("EDD_DPTRID");
            entity.Property(e => e.EddIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EDD_IPAddress");
            entity.Property(e => e.EddIsrequired)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("EDD_ISREQUIRED");
            entity.Property(e => e.EddPk).HasColumnName("EDD_Pk");
            entity.Property(e => e.EddRecallby).HasColumnName("EDD_RECALLBY");
            entity.Property(e => e.EddRecallon)
                .HasColumnType("datetime")
                .HasColumnName("EDD_RECALLON");
            entity.Property(e => e.EddSize).HasColumnName("EDD_Size");
            entity.Property(e => e.EddStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("EDD_Status");
            entity.Property(e => e.EddUpdatedby).HasColumnName("EDD_UPDATEDBY");
            entity.Property(e => e.EddUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("EDD_UPDATEDON");
            entity.Property(e => e.EddValidate)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDD_VALIDATE");
            entity.Property(e => e.EddValues)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("EDD_VALUES");
        });

        modelBuilder.Entity<EdtDoctypeLinkLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DOCTYPE_LINK_log");

            entity.Property(e => e.EddCompId).HasColumnName("EDD_CompId");
            entity.Property(e => e.EddDoctypeid).HasColumnName("EDD_DOCTYPEID");
            entity.Property(e => e.EddDptrid).HasColumnName("EDD_DPTRID");
            entity.Property(e => e.EddIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EDD_IPAddress");
            entity.Property(e => e.EddIsrequired)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("EDD_ISREQUIRED");
            entity.Property(e => e.EddPk).HasColumnName("EDD_Pk");
            entity.Property(e => e.EddSize).HasColumnName("EDD_Size");
            entity.Property(e => e.EddValidate)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDD_VALIDATE");
            entity.Property(e => e.EddValues)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("EDD_VALUES");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NEddDoctypeid).HasColumnName("nEDD_DOCTYPEID");
            entity.Property(e => e.NEddDptrid).HasColumnName("nEDD_DPTRID");
            entity.Property(e => e.NEddIsrequired)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("nEDD_ISREQUIRED");
            entity.Property(e => e.NEddSize).HasColumnName("nEDD_Size");
            entity.Property(e => e.NEddValidate)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nEDD_VALIDATE");
            entity.Property(e => e.NEddValues)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("nEDD_VALUES");
        });

        modelBuilder.Entity<EdtDoctypePermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DOCTYPE_PERMISSION");

            entity.Property(e => e.EdpApprovedby).HasColumnName("EDP_APPROVEDBY");
            entity.Property(e => e.EdpApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("EDP_APPROVEDON");
            entity.Property(e => e.EdpCompId).HasColumnName("EDP_CompId");
            entity.Property(e => e.EdpCrby).HasColumnName("EDP_CRBY");
            entity.Property(e => e.EdpCron)
                .HasColumnType("datetime")
                .HasColumnName("EDP_CRON");
            entity.Property(e => e.EdpDelDocument).HasColumnName("EDP_DEL_DOCUMENT");
            entity.Property(e => e.EdpDeletedby).HasColumnName("EDP_DELETEDBY");
            entity.Property(e => e.EdpDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("EDP_DELETEDON");
            entity.Property(e => e.EdpDoctypeid).HasColumnName("EDP_DOCTYPEID");
            entity.Property(e => e.EdpGrpid).HasColumnName("EDP_GRPID");
            entity.Property(e => e.EdpIndex).HasColumnName("EDP_INDEX");
            entity.Property(e => e.EdpIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EDP_IPAddress");
            entity.Property(e => e.EdpMfyDocument).HasColumnName("EDP_MFY_DOCUMENT");
            entity.Property(e => e.EdpMfyType).HasColumnName("EDP_MFY_TYPE");
            entity.Property(e => e.EdpOther).HasColumnName("EDP_OTHER");
            entity.Property(e => e.EdpPid).HasColumnName("EDP_PID");
            entity.Property(e => e.EdpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDP_PTYPE");
            entity.Property(e => e.EdpRecallby).HasColumnName("EDP_RECALLBY");
            entity.Property(e => e.EdpRecallon)
                .HasColumnType("datetime")
                .HasColumnName("EDP_RECALLON");
            entity.Property(e => e.EdpSearch).HasColumnName("EDP_SEARCH");
            entity.Property(e => e.EdpStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("EDP_Status");
            entity.Property(e => e.EdpUpdatedby).HasColumnName("EDP_UPDATEDBY");
            entity.Property(e => e.EdpUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("EDP_UPDATEDON");
            entity.Property(e => e.EdpUsrid).HasColumnName("EDP_USRID");
            entity.Property(e => e.EdpWhen)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDP_When");
        });

        modelBuilder.Entity<EdtDoctypePermissionLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DOCTYPE_PERMISSION_log");

            entity.Property(e => e.EdpCompId).HasColumnName("EDP_CompId");
            entity.Property(e => e.EdpDelDocument).HasColumnName("EDP_DEL_DOCUMENT");
            entity.Property(e => e.EdpDoctypeid).HasColumnName("EDP_DOCTYPEID");
            entity.Property(e => e.EdpGrpid).HasColumnName("EDP_GRPID");
            entity.Property(e => e.EdpIndex).HasColumnName("EDP_INDEX");
            entity.Property(e => e.EdpIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EDP_IPAddress");
            entity.Property(e => e.EdpMfyDocument).HasColumnName("EDP_MFY_DOCUMENT");
            entity.Property(e => e.EdpMfyType).HasColumnName("EDP_MFY_TYPE");
            entity.Property(e => e.EdpOther).HasColumnName("EDP_OTHER");
            entity.Property(e => e.EdpPid).HasColumnName("EDP_PID");
            entity.Property(e => e.EdpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDP_PTYPE");
            entity.Property(e => e.EdpSearch).HasColumnName("EDP_SEARCH");
            entity.Property(e => e.EdpUsrid).HasColumnName("EDP_USRID");
            entity.Property(e => e.EdpWhen)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDP_When");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NEdpDelDocument).HasColumnName("nEDP_DEL_DOCUMENT");
            entity.Property(e => e.NEdpDoctypeid).HasColumnName("nEDP_DOCTYPEID");
            entity.Property(e => e.NEdpGrpid).HasColumnName("nEDP_GRPID");
            entity.Property(e => e.NEdpIndex).HasColumnName("nEDP_INDEX");
            entity.Property(e => e.NEdpMfyDocument).HasColumnName("nEDP_MFY_DOCUMENT");
            entity.Property(e => e.NEdpMfyType).HasColumnName("nEDP_MFY_TYPE");
            entity.Property(e => e.NEdpOther).HasColumnName("nEDP_OTHER");
            entity.Property(e => e.NEdpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nEDP_PTYPE");
            entity.Property(e => e.NEdpSearch).HasColumnName("nEDP_SEARCH");
            entity.Property(e => e.NEdpUsrid).HasColumnName("nEDP_USRID");
            entity.Property(e => e.NEdpWhen)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nEDP_When");
        });

        modelBuilder.Entity<EdtDocumentType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_document_type");

            entity.Property(e => e.DotApprovedby).HasColumnName("DOT_APPROVEDBY");
            entity.Property(e => e.DotApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("DOT_APPROVEDON");
            entity.Property(e => e.DotCompId).HasColumnName("DOT_CompId");
            entity.Property(e => e.DotCrby).HasColumnName("DOT_CRBY");
            entity.Property(e => e.DotCron)
                .HasColumnType("datetime")
                .HasColumnName("DOT_CRON");
            entity.Property(e => e.DotDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DOT_DelFlag");
            entity.Property(e => e.DotDeletedby).HasColumnName("DOT_DELETEDBY");
            entity.Property(e => e.DotDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("DOT_DELETEDON");
            entity.Property(e => e.DotDocname)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("DOT_DOCNAME");
            entity.Property(e => e.DotDoctypeid).HasColumnName("DOT_DOCTYPEID");
            entity.Property(e => e.DotIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DOT_IPAddress");
            entity.Property(e => e.DotIsGlobal).HasColumnName("DOT_isGlobal");
            entity.Property(e => e.DotNote)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("DOT_NOTE");
            entity.Property(e => e.DotOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dot_operation");
            entity.Property(e => e.DotOperationby)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("dot_operationby");
            entity.Property(e => e.DotPgroup).HasColumnName("DOT_PGROUP");
            entity.Property(e => e.DotRecallby).HasColumnName("DOT_RECALLBY");
            entity.Property(e => e.DotRecallon)
                .HasColumnType("datetime")
                .HasColumnName("DOT_RECALLON");
            entity.Property(e => e.DotStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("DOT_STATUS");
            entity.Property(e => e.DotUpdatedby).HasColumnName("DOT_UPDATEDBY");
            entity.Property(e => e.DotUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("DOT_UPDATEDON");
        });

        modelBuilder.Entity<EdtDocumentTypeLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_DOCUMENT_TYPE_log");

            entity.Property(e => e.DotCompId).HasColumnName("DOT_CompId");
            entity.Property(e => e.DotDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("DOT_DelFlag");
            entity.Property(e => e.DotDocname)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("DOT_DOCNAME");
            entity.Property(e => e.DotDoctypeid).HasColumnName("DOT_DOCTYPEID");
            entity.Property(e => e.DotIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DOT_IPAddress");
            entity.Property(e => e.DotIsGlobal).HasColumnName("DOT_isGlobal");
            entity.Property(e => e.DotNote)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("DOT_NOTE");
            entity.Property(e => e.DotOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dot_operation");
            entity.Property(e => e.DotOperationby).HasColumnName("dot_operationby");
            entity.Property(e => e.DotPgroup).HasColumnName("DOT_PGROUP");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NDotDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("nDOT_DelFlag");
            entity.Property(e => e.NDotDocname)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("nDOT_DOCNAME");
            entity.Property(e => e.NDotIsGlobal).HasColumnName("nDOT_isGlobal");
            entity.Property(e => e.NDotNote)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("nDOT_NOTE");
            entity.Property(e => e.NDotPgroup).HasColumnName("nDOT_PGROUP");
            entity.Property(e => e.NdotOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ndot_operation");
            entity.Property(e => e.NdotOperationby).HasColumnName("ndot_operationby");
        });

        modelBuilder.Entity<EdtFolder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_FOLDER");

            entity.Property(e => e.FolApprovedBy).HasColumnName("FOL_ApprovedBy");
            entity.Property(e => e.FolApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_ApprovedOn");
            entity.Property(e => e.FolCabinet).HasColumnName("FOL_Cabinet");
            entity.Property(e => e.FolCompId).HasColumnName("FOL_CompID");
            entity.Property(e => e.FolCreatedBy).HasColumnName("FOL_CreatedBy");
            entity.Property(e => e.FolCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_CreatedOn");
            entity.Property(e => e.FolDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FOL_DelFlag");
            entity.Property(e => e.FolDeletedBy).HasColumnName("FOL_DeletedBy");
            entity.Property(e => e.FolDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_DeletedOn");
            entity.Property(e => e.FolFolId).HasColumnName("FOL_FolID");
            entity.Property(e => e.FolName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FOL_Name");
            entity.Property(e => e.FolNote)
                .IsUnicode(false)
                .HasColumnName("FOL_Note");
            entity.Property(e => e.FolRecalledBy).HasColumnName("FOL_RecalledBy");
            entity.Property(e => e.FolRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_RecalledOn");
            entity.Property(e => e.FolStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("FOL_Status");
            entity.Property(e => e.FolUpdatedBy).HasColumnName("FOL_UpdatedBy");
            entity.Property(e => e.FolUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_UpdatedOn");
        });

        modelBuilder.Entity<EdtFolder1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_FOLDER1");

            entity.Property(e => e.FolCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("FOL_CABINET");
            entity.Property(e => e.FolCrby).HasColumnName("FOL_CRBY");
            entity.Property(e => e.FolCron)
                .HasColumnType("datetime")
                .HasColumnName("FOL_CRON");
            entity.Property(e => e.FolExpirydate)
                .HasColumnType("datetime")
                .HasColumnName("FOL_EXPIRYDATE");
            entity.Property(e => e.FolFolid)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("FOL_FOLID");
            entity.Property(e => e.FolName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FOL_NAME");
            entity.Property(e => e.FolNotes)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("FOL_NOTES");
            entity.Property(e => e.FolOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("fol_operation");
            entity.Property(e => e.FolOperationBy)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("fol_operationBy");
            entity.Property(e => e.FolPagecount).HasColumnName("FOL_PAGECOUNT");
            entity.Property(e => e.FolStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FOL_STATUS");
        });

        modelBuilder.Entity<EdtFolder2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_FOLDER2");

            entity.Property(e => e.FolCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("FOL_CABINET");
            entity.Property(e => e.FolCrby).HasColumnName("FOL_CRBY");
            entity.Property(e => e.FolCron)
                .HasColumnType("datetime")
                .HasColumnName("FOL_CRON");
            entity.Property(e => e.FolExpirydate)
                .HasColumnType("datetime")
                .HasColumnName("FOL_EXPIRYDATE");
            entity.Property(e => e.FolFolid)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("FOL_FOLID");
            entity.Property(e => e.FolName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FOL_NAME");
            entity.Property(e => e.FolNotes)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("FOL_NOTES");
            entity.Property(e => e.FolOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("fol_operation");
            entity.Property(e => e.FolOperationBy)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("fol_operationBy");
            entity.Property(e => e.FolPagecount).HasColumnName("FOL_PAGECOUNT");
            entity.Property(e => e.FolStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FOL_STATUS");
        });

        modelBuilder.Entity<EdtFolderPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_Folder_Permission");

            entity.Property(e => e.EfpCrtDoc).HasColumnName("EFP_CRT_DOC");
            entity.Property(e => e.EfpDelDoc).HasColumnName("EFP_DEL_DOC");
            entity.Property(e => e.EfpDelFolder).HasColumnName("EFP_DEL_FOLDER");
            entity.Property(e => e.EfpExport).HasColumnName("EFP_EXPORT");
            entity.Property(e => e.EfpFolId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("EFP_FolId");
            entity.Property(e => e.EfpGrpid).HasColumnName("EFP_GRPID");
            entity.Property(e => e.EfpId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("EFP_ID");
            entity.Property(e => e.EfpIndex).HasColumnName("EFP_INDEX");
            entity.Property(e => e.EfpModDoc).HasColumnName("EFP_MOD_DOC");
            entity.Property(e => e.EfpModFolder).HasColumnName("EFP_MOD_FOLDER");
            entity.Property(e => e.EfpOther).HasColumnName("EFP_OTHER");
            entity.Property(e => e.EfpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EFP_PTYPE");
            entity.Property(e => e.EfpSearch).HasColumnName("EFP_SEARCH");
            entity.Property(e => e.EfpUsrid).HasColumnName("EFP_USRID");
            entity.Property(e => e.EfpViewFol).HasColumnName("EFP_VIEW_Fol");
        });

        modelBuilder.Entity<EdtFolderRight>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_folder_rights");

            entity.Property(e => e.FerDoctypeId)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("FER_DoctypeID");
            entity.Property(e => e.FerFolderId)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("FER_FolderID");
            entity.Property(e => e.FerIndex)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FER_Index");
            entity.Property(e => e.FerSearch)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FER_Search");
            entity.Property(e => e.FerUserId)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("FER_UserID");
        });

        modelBuilder.Entity<EdtImageSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_Image_Settings");

            entity.Property(e => e.ImgBright).HasColumnName("Img_Bright");
            entity.Property(e => e.ImgContrast).HasColumnName("Img_Contrast");
            entity.Property(e => e.ImgForm)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Img_Form");
            entity.Property(e => e.ImgGamma).HasColumnName("Img_Gamma");
            entity.Property(e => e.ImgId).HasColumnName("Img_Id");
            entity.Property(e => e.ImgImgId).HasColumnName("Img_ImgId");
            entity.Property(e => e.ImgRotate).HasColumnName("Img_Rotate");
            entity.Property(e => e.ImgRotateAny).HasColumnName("Img_RotateAny");
        });

        modelBuilder.Entity<EdtNote>(entity =>
        {
            entity.HasKey(e => e.EdtNoteId).HasName("PK__edt_Note__3D07B1F6D19205B4");

            entity.ToTable("edt_Notes");

            entity.Property(e => e.EdtNoteId).HasColumnName("Edt_NoteId");
            entity.Property(e => e.EdtCreatedBy).HasColumnName("Edt_CreatedBy");
            entity.Property(e => e.EdtCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Edt_CreatedOn");
            entity.Property(e => e.EdtNotes)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("Edt_Notes");
            entity.Property(e => e.EdtPageId).HasColumnName("Edt_PageID");
        });

        modelBuilder.Entity<EdtOutlookAttach>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Edt_OutlookAttach");

            entity.Property(e => e.EdtBaseName).HasColumnName("Edt_BaseName");
            entity.Property(e => e.EdtCompId).HasColumnName("edt_CompID");
            entity.Property(e => e.EdtCreatedBy).HasColumnName("Edt_createdBy");
            entity.Property(e => e.EdtCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Edt_createdOn");
            entity.Property(e => e.EdtId).HasColumnName("Edt_id");
            entity.Property(e => e.EdtOutlookId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Edt_OutlookID");
            entity.Property(e => e.EdtReceivedDate)
                .HasColumnType("datetime")
                .HasColumnName("Edt_ReceivedDate");
        });

        modelBuilder.Entity<EdtPage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_page");

            entity.Property(e => e.PgeApprovedBy).HasColumnName("Pge_ApprovedBy");
            entity.Property(e => e.PgeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_ApprovedOn");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCdpath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_CDPATH");
            entity.Property(e => e.PgeCheckedoutby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CHECKEDOUTBY");
            entity.Property(e => e.PgeCheckout)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("PGE_CHECKOUT");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCrby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CRBY");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCron)
                .HasColumnType("datetime")
                .HasColumnName("PGE_CRON");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelBy).HasColumnName("pge_DelBy");
            entity.Property(e => e.PgeDelOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_DelOn");
            entity.Property(e => e.PgeDeletedBy).HasColumnName("Pge_DeletedBy");
            entity.Property(e => e.PgeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_DeletedOn");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeEncrypt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_ENCRYPT");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeLastViewed)
                .HasColumnType("datetime")
                .HasColumnName("PGE_LastViewed");
            entity.Property(e => e.PgeModBy).HasColumnName("pge_ModBy");
            entity.Property(e => e.PgeModOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_ModOn");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrStatus).HasColumnName("PGE_OCR_Status");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOcrtextLine1)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line1");
            entity.Property(e => e.PgeOcrtextLine2)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line2");
            entity.Property(e => e.PgeOcrtextLine3)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line3");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgePagetype)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_PAGETYPE");
            entity.Property(e => e.PgeQcTo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PGE_QC_TO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeRecalledBy).HasColumnName("Pge_RecalledBy");
            entity.Property(e => e.PgeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_RecalledOn");
            entity.Property(e => e.PgeRefno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pge_refno");
            entity.Property(e => e.PgeRfid)
                .IsUnicode(false)
                .HasColumnName("PGE_RFID");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPage03082024>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_Page03082024");

            entity.Property(e => e.PgeApprovedBy).HasColumnName("Pge_ApprovedBy");
            entity.Property(e => e.PgeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_ApprovedOn");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCdpath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_CDPATH");
            entity.Property(e => e.PgeCheckedoutby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CHECKEDOUTBY");
            entity.Property(e => e.PgeCheckout)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("PGE_CHECKOUT");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCrby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CRBY");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCron)
                .HasColumnType("datetime")
                .HasColumnName("PGE_CRON");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelBy).HasColumnName("pge_DelBy");
            entity.Property(e => e.PgeDelOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_DelOn");
            entity.Property(e => e.PgeDeletedBy).HasColumnName("Pge_DeletedBy");
            entity.Property(e => e.PgeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_DeletedOn");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeEncrypt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_ENCRYPT");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeLastViewed)
                .HasColumnType("datetime")
                .HasColumnName("PGE_LastViewed");
            entity.Property(e => e.PgeModBy).HasColumnName("pge_ModBy");
            entity.Property(e => e.PgeModOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_ModOn");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrStatus).HasColumnName("PGE_OCR_Status");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOcrtextLine1)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line1");
            entity.Property(e => e.PgeOcrtextLine2)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line2");
            entity.Property(e => e.PgeOcrtextLine3)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line3");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgePagetype)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_PAGETYPE");
            entity.Property(e => e.PgeQcTo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PGE_QC_TO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeRecalledBy).HasColumnName("Pge_RecalledBy");
            entity.Property(e => e.PgeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_RecalledOn");
            entity.Property(e => e.PgeRefno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pge_refno");
            entity.Property(e => e.PgeRfid)
                .IsUnicode(false)
                .HasColumnName("PGE_RFID");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPage1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_page1");

            entity.Property(e => e.PgeApprovedBy).HasColumnName("Pge_ApprovedBy");
            entity.Property(e => e.PgeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_ApprovedOn");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCdpath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_CDPATH");
            entity.Property(e => e.PgeCheckedoutby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CHECKEDOUTBY");
            entity.Property(e => e.PgeCheckout)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("PGE_CHECKOUT");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCrby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CRBY");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCron)
                .HasColumnType("datetime")
                .HasColumnName("PGE_CRON");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelBy).HasColumnName("pge_DelBy");
            entity.Property(e => e.PgeDelOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_DelOn");
            entity.Property(e => e.PgeDeletedBy).HasColumnName("Pge_DeletedBy");
            entity.Property(e => e.PgeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_DeletedOn");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeEncrypt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_ENCRYPT");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeModBy).HasColumnName("pge_ModBy");
            entity.Property(e => e.PgeModOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_ModOn");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgePagetype)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_PAGETYPE");
            entity.Property(e => e.PgeQcTo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PGE_QC_TO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeRecalledBy).HasColumnName("Pge_RecalledBy");
            entity.Property(e => e.PgeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_RecalledOn");
            entity.Property(e => e.PgeRefno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pge_refno");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPage30072024>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_page30072024");

            entity.Property(e => e.PgeApprovedBy).HasColumnName("Pge_ApprovedBy");
            entity.Property(e => e.PgeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_ApprovedOn");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCdpath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_CDPATH");
            entity.Property(e => e.PgeCheckedoutby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CHECKEDOUTBY");
            entity.Property(e => e.PgeCheckout)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("PGE_CHECKOUT");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCrby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CRBY");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCron)
                .HasColumnType("datetime")
                .HasColumnName("PGE_CRON");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelBy).HasColumnName("pge_DelBy");
            entity.Property(e => e.PgeDelOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_DelOn");
            entity.Property(e => e.PgeDeletedBy).HasColumnName("Pge_DeletedBy");
            entity.Property(e => e.PgeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_DeletedOn");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeEncrypt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_ENCRYPT");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeLastViewed)
                .HasColumnType("datetime")
                .HasColumnName("PGE_LastViewed");
            entity.Property(e => e.PgeModBy).HasColumnName("pge_ModBy");
            entity.Property(e => e.PgeModOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_ModOn");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrStatus).HasColumnName("PGE_OCR_Status");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOcrtextLine1)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line1");
            entity.Property(e => e.PgeOcrtextLine2)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line2");
            entity.Property(e => e.PgeOcrtextLine3)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line3");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgePagetype)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_PAGETYPE");
            entity.Property(e => e.PgeQcTo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PGE_QC_TO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeRecalledBy).HasColumnName("Pge_RecalledBy");
            entity.Property(e => e.PgeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_RecalledOn");
            entity.Property(e => e.PgeRefno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pge_refno");
            entity.Property(e => e.PgeRfid)
                .IsUnicode(false)
                .HasColumnName("PGE_RFID");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPage31072024>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_Page31072024");

            entity.Property(e => e.PgeApprovedBy).HasColumnName("Pge_ApprovedBy");
            entity.Property(e => e.PgeApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_ApprovedOn");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCdpath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_CDPATH");
            entity.Property(e => e.PgeCheckedoutby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CHECKEDOUTBY");
            entity.Property(e => e.PgeCheckout)
                .HasColumnType("numeric(1, 0)")
                .HasColumnName("PGE_CHECKOUT");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCrby)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("PGE_CRBY");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCron)
                .HasColumnType("datetime")
                .HasColumnName("PGE_CRON");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelBy).HasColumnName("pge_DelBy");
            entity.Property(e => e.PgeDelOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_DelOn");
            entity.Property(e => e.PgeDeletedBy).HasColumnName("Pge_DeletedBy");
            entity.Property(e => e.PgeDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_DeletedOn");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeEncrypt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_ENCRYPT");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeLastViewed)
                .HasColumnType("datetime")
                .HasColumnName("PGE_LastViewed");
            entity.Property(e => e.PgeModBy).HasColumnName("pge_ModBy");
            entity.Property(e => e.PgeModOn)
                .HasColumnType("datetime")
                .HasColumnName("pge_ModOn");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrStatus).HasColumnName("PGE_OCR_Status");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOcrtextLine1)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line1");
            entity.Property(e => e.PgeOcrtextLine2)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line2");
            entity.Property(e => e.PgeOcrtextLine3)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText_Line3");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgePagetype)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_PAGETYPE");
            entity.Property(e => e.PgeQcTo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PGE_QC_TO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeRecalledBy).HasColumnName("Pge_RecalledBy");
            entity.Property(e => e.PgeRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_RecalledOn");
            entity.Property(e => e.PgeRefno)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pge_refno");
            entity.Property(e => e.PgeRfid)
                .IsUnicode(false)
                .HasColumnName("PGE_RFID");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPageDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_page_details");

            entity.Property(e => e.EpdBaseid)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("EPD_BASEID");
            entity.Property(e => e.EpdCompId).HasColumnName("EPD_CompID");
            entity.Property(e => e.EpdDescid)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("EPD_DESCID");
            entity.Property(e => e.EpdDoctype)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("EPD_DOCTYPE");
            entity.Property(e => e.EpdKeyword)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("EPD_KEYWORD");
            entity.Property(e => e.EpdValue)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("EPD_VALUE");
        });

        modelBuilder.Entity<EdtPageLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Edt_page_log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.PgeBasename)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_BASENAME");
            entity.Property(e => e.PgeBatchId).HasColumnName("PGE_BatchID");
            entity.Property(e => e.PgeBatchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PGE_batch_name");
            entity.Property(e => e.PgeCabinet)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_CABINET");
            entity.Property(e => e.PgeCompId).HasColumnName("Pge_CompID");
            entity.Property(e => e.PgeCreatedBy).HasColumnName("Pge_CreatedBy");
            entity.Property(e => e.PgeCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_CreatedOn");
            entity.Property(e => e.PgeCurrentVer)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_CURRENT_VER");
            entity.Property(e => e.PgeDate)
                .HasColumnType("datetime")
                .HasColumnName("PGE_DATE");
            entity.Property(e => e.PgeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("pge_Delflag");
            entity.Property(e => e.PgeDetailsId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("Pge_DETAILS_ID");
            entity.Property(e => e.PgeDocumentType)
                .HasColumnType("numeric(15, 0)")
                .HasColumnName("PGE_DOCUMENT_TYPE");
            entity.Property(e => e.PgeExt)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("PGE_EXT");
            entity.Property(e => e.PgeFolder)
                .HasColumnType("numeric(7, 0)")
                .HasColumnName("PGE_FOLDER");
            entity.Property(e => e.PgeFtpstatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_FTPStatus");
            entity.Property(e => e.PgeKeyWord)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PGE_KeyWORD");
            entity.Property(e => e.PgeObject)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PGE_OBJECT");
            entity.Property(e => e.PgeOcrdelFlag).HasColumnName("PGE_OCRDelFlag");
            entity.Property(e => e.PgeOcrtext)
                .IsUnicode(false)
                .HasColumnName("PGE_OCRText");
            entity.Property(e => e.PgeOrignalFileName)
                .IsUnicode(false)
                .HasColumnName("pge_OrignalFileName");
            entity.Property(e => e.PgePageno)
                .HasColumnType("numeric(4, 0)")
                .HasColumnName("PGE_PAGENO");
            entity.Property(e => e.PgeQcUsrGrpId).HasColumnName("PGE_QC_UsrGrpId");
            entity.Property(e => e.PgeSize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SIZE");
            entity.Property(e => e.PgeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("PGE_STATUS");
            entity.Property(e => e.PgeSubCabinet)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("PGE_SubCabinet");
            entity.Property(e => e.PgeTitle)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PGE_TITLE");
            entity.Property(e => e.PgeUpdatedBy).HasColumnName("Pge_UpdatedBy");
            entity.Property(e => e.PgeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Pge_UpdatedOn");
        });

        modelBuilder.Entity<EdtPageViewAndDownloadlog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_PAGE_ViewAndDownloadlogs");

            entity.Property(e => e.PvdCabinet).HasColumnName("PVD_Cabinet");
            entity.Property(e => e.PvdCompId).HasColumnName("PVD_CompId");
            entity.Property(e => e.PvdDate)
                .HasColumnType("datetime")
                .HasColumnName("PVD_Date");
            entity.Property(e => e.PvdDepId).HasColumnName("PVD_DepId");
            entity.Property(e => e.PvdDocumentType).HasColumnName("PVD_DocumentType");
            entity.Property(e => e.PvdFolder).HasColumnName("PVD_Folder");
            entity.Property(e => e.PvdIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PVD_Ipaddress");
            entity.Property(e => e.PvdLogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PVD_LogOperation");
            entity.Property(e => e.PvdPageBaseId).HasColumnName("PVD_PageBaseID");
            entity.Property(e => e.PvdPageDetailsId).HasColumnName("PVD_PageDetailsID");
            entity.Property(e => e.PvdPkid).HasColumnName("PVD_PKID");
            entity.Property(e => e.PvdSubCabinet).HasColumnName("PVD_SubCabinet");
            entity.Property(e => e.PvdUserId).HasColumnName("PVD_UserId");
            entity.Property(e => e.PvdVersion).HasColumnName("PVD_Version");
        });

        modelBuilder.Entity<EdtScanDocDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("edt_ScanDoc_Details");

            entity.Property(e => e.ScanBatchid).HasColumnName("SCAN_BATCHID");
            entity.Property(e => e.ScanBuilding)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_BUILDING");
            entity.Property(e => e.ScanColumn)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_COLUMN");
            entity.Property(e => e.ScanDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_DESCRIPTION");
            entity.Property(e => e.ScanFloor)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_FLOOR");
            entity.Property(e => e.ScanId).HasColumnName("SCAN_ID");
            entity.Property(e => e.ScanLocation)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_LOCATION");
            entity.Property(e => e.ScanRackno)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_RACKNO");
            entity.Property(e => e.ScanRoomno)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_ROOMNO");
            entity.Property(e => e.ScanRow)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SCAN_ROW");
        });

        modelBuilder.Entity<EdtSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_Settings");

            entity.Property(e => e.SadUpdatedBy).HasColumnName("SAD_UpdatedBy");
            entity.Property(e => e.SadUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SAD_UpdatedOn");
            entity.Property(e => e.SetCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SET_CODE");
            entity.Property(e => e.SetCompId).HasColumnName("SET_CompID");
            entity.Property(e => e.SetId).HasColumnName("SET_ID");
            entity.Property(e => e.SetIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SET_IPAddress");
            entity.Property(e => e.SetOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SET_Operation");
            entity.Property(e => e.SetValue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SET_Value");
        });

        modelBuilder.Entity<EdtSettingsLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EDT_Settings_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NSetCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nSET_CODE");
            entity.Property(e => e.NSetOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nSET_Operation");
            entity.Property(e => e.NSetValue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nSET_Value");
            entity.Property(e => e.SadRunDate)
                .HasColumnType("datetime")
                .HasColumnName("SAD_RunDate");
            entity.Property(e => e.SetCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SET_CODE");
            entity.Property(e => e.SetCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SET_CompID");
            entity.Property(e => e.SetId).HasColumnName("SET_ID");
            entity.Property(e => e.SetIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SET_IPAddress");
            entity.Property(e => e.SetOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SET_Operation");
            entity.Property(e => e.SetValue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SET_Value");
        });

        modelBuilder.Entity<ExcelUploadStructure>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Excel_Upload_Structure");

            entity.Property(e => e.EusCompId).HasColumnName("EUS_CompID");
            entity.Property(e => e.EusDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EUS_Delflag");
            entity.Property(e => e.EusFields)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("EUS_Fields");
            entity.Property(e => e.EusId).HasColumnName("EUS_Id");
            entity.Property(e => e.EusName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EUS_Name");
            entity.Property(e => e.EusValue).HasColumnName("EUS_Value");
            entity.Property(e => e.EusValues)
                .HasMaxLength(1500)
                .IsUnicode(false)
                .HasColumnName("EUS_Values");
        });

        modelBuilder.Entity<FinancialAddAssign>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Financial_AddAssign");

            entity.Property(e => e.FaaAccHead).HasColumnName("FAA_AccHead");
            entity.Property(e => e.FaaCloseCredit)
                .HasColumnType("money")
                .HasColumnName("FAA_CloseCredit");
            entity.Property(e => e.FaaCloseDebit)
                .HasColumnType("money")
                .HasColumnName("FAA_CloseDebit");
            entity.Property(e => e.FaaComments)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("FAA_Comments");
            entity.Property(e => e.FaaCompId).HasColumnName("FAA_CompID");
            entity.Property(e => e.FaaCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FAA_CreatedOn");
            entity.Property(e => e.FaaCreatedby).HasColumnName("FAA_Createdby");
            entity.Property(e => e.FaaCustId).HasColumnName("FAA_CustID");
            entity.Property(e => e.FaaGl).HasColumnName("FAA_GL");
            entity.Property(e => e.FaaGlcode)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FAA_GLCode");
            entity.Property(e => e.FaaGldesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FAA_GLDesc");
            entity.Property(e => e.FaaHead).HasColumnName("FAA_Head");
            entity.Property(e => e.FaaId).HasColumnName("FAA_ID");
            entity.Property(e => e.FaaIndType).HasColumnName("FAA_IndType");
            entity.Property(e => e.FaaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FAA_IPAddress");
            entity.Property(e => e.FaaNameoftheperson).HasColumnName("FAA_Nameoftheperson");
            entity.Property(e => e.FaaObcredit)
                .HasColumnType("money")
                .HasColumnName("FAA_OBCredit");
            entity.Property(e => e.FaaObdebit)
                .HasColumnType("money")
                .HasColumnName("FAA_OBDebit");
            entity.Property(e => e.FaaOperation)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FAA_Operation");
            entity.Property(e => e.FaaParent).HasColumnName("FAA_Parent");
            entity.Property(e => e.FaaSgldesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FAA_SGLDesc");
            entity.Property(e => e.FaaTrCredit)
                .HasColumnType("money")
                .HasColumnName("FAA_TrCredit");
            entity.Property(e => e.FaaTrDebit)
                .HasColumnType("money")
                .HasColumnName("FAA_TrDebit");
            entity.Property(e => e.FaaYearId).HasColumnName("FAA_YearID");
        });

        modelBuilder.Entity<FlaLeaveDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Fla_LeaveDetails");

            entity.Property(e => e.LpeApprove)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LPE_Approve");
            entity.Property(e => e.LpeApprovedDetails)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LPE_ApprovedDetails");
            entity.Property(e => e.LpeCompId).HasColumnName("LPE_CompID");
            entity.Property(e => e.LpeCrBy).HasColumnName("LPE_CrBY");
            entity.Property(e => e.LpeCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LPE_CrOn");
            entity.Property(e => e.LpeDays).HasColumnName("LPE_DAYS");
            entity.Property(e => e.LpeDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LPE_DelFlag");
            entity.Property(e => e.LpeEmpid).HasColumnName("LPE_EMPID");
            entity.Property(e => e.LpeFromdate)
                .HasColumnType("datetime")
                .HasColumnName("LPE_FROMDATE");
            entity.Property(e => e.LpeId).HasColumnName("LPE_ID");
            entity.Property(e => e.LpeIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LPE_IPAddress");
            entity.Property(e => e.LpePurpose)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LPE_PURPOSE");
            entity.Property(e => e.LpeStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LPE_Status");
            entity.Property(e => e.LpeTodate)
                .HasColumnType("datetime")
                .HasColumnName("LPE_TODATE");
            entity.Property(e => e.LpeUpdatedBy).HasColumnName("LPE_UpdatedBY");
            entity.Property(e => e.LpeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LPE_UpdatedOn");
            entity.Property(e => e.LpeYearId).HasColumnName("LPE_YearID");
        });

        modelBuilder.Entity<GenProductDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Gen_ProductDetails");

            entity.Property(e => e.GpAccesscode)
                .IsUnicode(false)
                .HasColumnName("GP_Accesscode");
            entity.Property(e => e.GpLicensetype).HasColumnName("GP_Licensetype");
            entity.Property(e => e.GpLicenseuser).HasColumnName("GP_Licenseuser");
            entity.Property(e => e.GpPkid).HasColumnName("GP_Pkid");
            entity.Property(e => e.GpProductkey)
                .IsUnicode(false)
                .HasColumnName("GP_Productkey");
            entity.Property(e => e.GpStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("GP_Status");
        });

        modelBuilder.Entity<GraceEmailSentDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_EMailSent_Details");

            entity.Property(e => e.EmdAttachedDocIds)
                .IsUnicode(false)
                .HasColumnName("EMD_AttachedDocIDs");
            entity.Property(e => e.EmdAttachedPath)
                .IsUnicode(false)
                .HasColumnName("EMD_AttachedPath");
            entity.Property(e => e.EmdBody)
                .IsUnicode(false)
                .HasColumnName("EMD_Body");
            entity.Property(e => e.EmdCcemailIds)
                .IsUnicode(false)
                .HasColumnName("EMD_CCEmailIDs");
            entity.Property(e => e.EmdCompId).HasColumnName("EMD_CompID");
            entity.Property(e => e.EmdCreatedBy).HasColumnName("EMD_CreatedBy");
            entity.Property(e => e.EmdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMD_CreatedOn");
            entity.Property(e => e.EmdEmailStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("EMD_EMailStatus");
            entity.Property(e => e.EmdFormName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMD_FormName");
            entity.Property(e => e.EmdFromEmailId)
                .IsUnicode(false)
                .HasColumnName("EMD_FromEmailID");
            entity.Property(e => e.EmdId).HasColumnName("EMD_ID");
            entity.Property(e => e.EmdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("EMD_IPAddress");
            entity.Property(e => e.EmdMstPkid).HasColumnName("EMD_MstPKID");
            entity.Property(e => e.EmdSentOn)
                .HasColumnType("datetime")
                .HasColumnName("EMD_SentOn");
            entity.Property(e => e.EmdSentUsrId).HasColumnName("EMD_SentUsrID");
            entity.Property(e => e.EmdSubject)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("EMD_Subject");
            entity.Property(e => e.EmdToEmailIds)
                .IsUnicode(false)
                .HasColumnName("EMD_ToEmailIDs");
            entity.Property(e => e.EmdYearId).HasColumnName("EMD_YearID");
        });

        modelBuilder.Entity<GraceExcelUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Grace_ExcelUpload");

            entity.Property(e => e.GeuCompid).HasColumnName("GEU_Compid");
            entity.Property(e => e.GeuMasterName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GEU_MasterName");
            entity.Property(e => e.GeuPkId).HasColumnName("GEU_Pk_Id");
            entity.Property(e => e.GeuStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GEU_Status");
        });

        modelBuilder.Entity<GraceGrossControlScore>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_GrossControlScore");

            entity.Property(e => e.GgCompId).HasColumnName("GG_CompID");
            entity.Property(e => e.GgControlScore).HasColumnName("GG_ControlScore");
            entity.Property(e => e.GgDe).HasColumnName("GG_DE");
            entity.Property(e => e.GgOe).HasColumnName("GG_OE");
            entity.Property(e => e.GgPkid).HasColumnName("GG_PKID");
        });

        modelBuilder.Entity<GraceGrossRiskScore>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_GrossRiskScore");

            entity.Property(e => e.GgCompId).HasColumnName("GG_CompID");
            entity.Property(e => e.GgImpact).HasColumnName("GG_Impact");
            entity.Property(e => e.GgLikelihood).HasColumnName("GG_Likelihood");
            entity.Property(e => e.GgPkid).HasColumnName("GG_PKID");
            entity.Property(e => e.GgRiskScore).HasColumnName("GG_RiskScore");
        });

        modelBuilder.Entity<GraceOverallBranchRatingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_OverallBranchRating_Details");

            entity.Property(e => e.GodBacoreProcessRatingId).HasColumnName("GOD_BACoreProcessRatingID");
            entity.Property(e => e.GodBacoreProcessScore).HasColumnName("GOD_BACoreProcessScore");
            entity.Property(e => e.GodBanetRatingId).HasColumnName("GOD_BANetRatingID");
            entity.Property(e => e.GodBanetScore).HasColumnName("GOD_BANetScore");
            entity.Property(e => e.GodBasupportProcessRatingId).HasColumnName("GOD_BASupportProcessRatingID");
            entity.Property(e => e.GodBasupportProcessScore).HasColumnName("GOD_BASupportProcessScore");
            entity.Property(e => e.GodBcmcoreProcessRatingId).HasColumnName("GOD_BCMCoreProcessRatingID");
            entity.Property(e => e.GodBcmcoreProcessScore).HasColumnName("GOD_BCMCoreProcessScore");
            entity.Property(e => e.GodBcmnetRatingId).HasColumnName("GOD_BCMNetRatingID");
            entity.Property(e => e.GodBcmnetScore).HasColumnName("GOD_BCMNetScore");
            entity.Property(e => e.GodBcmsupportProcessRatingId).HasColumnName("GOD_BCMSupportProcessRatingID");
            entity.Property(e => e.GodBcmsupportProcessScore).HasColumnName("GOD_BCMSupportProcessScore");
            entity.Property(e => e.GodBranchId).HasColumnName("GOD_BranchID");
            entity.Property(e => e.GodBrrcoreProcessRatingId).HasColumnName("GOD_BRRCoreProcessRatingID");
            entity.Property(e => e.GodBrrcoreProcessScore).HasColumnName("GOD_BRRCoreProcessScore");
            entity.Property(e => e.GodBrrnetRatingId).HasColumnName("GOD_BRRNetRatingID");
            entity.Property(e => e.GodBrrnetScore).HasColumnName("GOD_BRRNetScore");
            entity.Property(e => e.GodBrrsupportProcessRatingId).HasColumnName("GOD_BRRSupportProcessRatingID");
            entity.Property(e => e.GodBrrsupportProcessScore).HasColumnName("GOD_BRRSupportProcessScore");
            entity.Property(e => e.GodCompId).HasColumnName("GOD_CompID");
            entity.Property(e => e.GodCrBy).HasColumnName("GOD_CrBy");
            entity.Property(e => e.GodCrOn)
                .HasColumnType("datetime")
                .HasColumnName("GOD_CrOn");
            entity.Property(e => e.GodCustId).HasColumnName("GOD_CustID");
            entity.Property(e => e.GodIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("GOD_IPAddress");
            entity.Property(e => e.GodPkid).HasColumnName("GOD_PKID");
            entity.Property(e => e.GodYearId).HasColumnName("GOD_YearID");
        });

        modelBuilder.Entity<GraceOverallFunctionRatingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_OverallFunctionRating_Details");

            entity.Property(e => e.GodCompId).HasColumnName("GOD_CompID");
            entity.Property(e => e.GodCrBy).HasColumnName("GOD_CrBy");
            entity.Property(e => e.GodCrOn)
                .HasColumnType("datetime")
                .HasColumnName("GOD_CrOn");
            entity.Property(e => e.GodCustId).HasColumnName("GOD_CustID");
            entity.Property(e => e.GodFunId).HasColumnName("GOD_FunID");
            entity.Property(e => e.GodIamnetRatingId).HasColumnName("GOD_IAMNetRatingID");
            entity.Property(e => e.GodIanetScore).HasColumnName("GOD_IANetScore");
            entity.Property(e => e.GodIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("GOD_IPAddress");
            entity.Property(e => e.GodPkid).HasColumnName("GOD_PKID");
            entity.Property(e => e.GodRanetRatingId).HasColumnName("GOD_RANetRatingID");
            entity.Property(e => e.GodRanetScore).HasColumnName("GOD_RANetScore");
            entity.Property(e => e.GodSubFunId).HasColumnName("GOD_SubFunID");
            entity.Property(e => e.GodYearId).HasColumnName("GOD_YearID");
        });

        modelBuilder.Entity<GraceRiskControlMatrix>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("GRACe_RiskControlMatrix");

            entity.Property(e => e.GgCompId).HasColumnName("GG_CompID");
            entity.Property(e => e.GgControls).HasColumnName("GG_Controls");
            entity.Property(e => e.GgPkid).HasColumnName("GG_PKID");
            entity.Property(e => e.GgRisk).HasColumnName("GG_Risk");
            entity.Property(e => e.GgRiskControlScore).HasColumnName("GG_RiskControlScore");
        });

        modelBuilder.Entity<HolidayMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Holiday_Master");

            entity.Property(e => e.HolApprovedBy).HasColumnName("Hol_ApprovedBy");
            entity.Property(e => e.HolApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Hol_ApprovedOn");
            entity.Property(e => e.HolCompId).HasColumnName("Hol_CompID");
            entity.Property(e => e.HolCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Hol_CreatedOn");
            entity.Property(e => e.HolCreatedby).HasColumnName("Hol_Createdby");
            entity.Property(e => e.HolDate)
                .HasColumnType("datetime")
                .HasColumnName("Hol_Date");
            entity.Property(e => e.HolDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Hol_DelFlag");
            entity.Property(e => e.HolIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Hol_IPAddress");
            entity.Property(e => e.HolRemarks)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Hol_Remarks");
            entity.Property(e => e.HolStatus)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Hol_Status");
            entity.Property(e => e.HolUpdatedBy).HasColumnName("Hol_UpdatedBy");
            entity.Property(e => e.HolUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Hol_UpdatedOn");
            entity.Property(e => e.HolYearId).HasColumnName("Hol_YearId");
        });

        modelBuilder.Entity<HolidayMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Holiday_Master_Log");

            entity.Property(e => e.HolCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("HOL_CompID");
            entity.Property(e => e.HolDate)
                .HasColumnType("datetime")
                .HasColumnName("Hol_Date");
            entity.Property(e => e.HolIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Hol_IPAddress");
            entity.Property(e => e.HolRemarks)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Hol_Remarks");
            entity.Property(e => e.HolYearId).HasColumnName("Hol_YearId");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NHolDate)
                .HasColumnType("datetime")
                .HasColumnName("nHol_Date");
            entity.Property(e => e.NHolRemarks)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nHol_Remarks");
        });

        modelBuilder.Entity<InsConfig>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("INS_Config");

            entity.Property(e => e.ConfAmPm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_AmPm");
            entity.Property(e => e.ConfCompId).HasColumnName("Conf_CompID");
            entity.Property(e => e.ConfConString)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("conf_ConString");
            entity.Property(e => e.ConfFrom)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("conf_From");
            entity.Property(e => e.ConfHh)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_hh");
            entity.Property(e => e.ConfInsIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Conf_INS_IPAddress");
            entity.Property(e => e.ConfIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("conf_IPAddress");
            entity.Property(e => e.ConfMm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_mm");
            entity.Property(e => e.ConfPort).HasColumnName("conf_Port");
            entity.Property(e => e.ConfRdbms)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("conf_RDBMS");
            entity.Property(e => e.ConfSenderId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("conf_SenderID");
            entity.Property(e => e.ConfStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Conf_Status");
            entity.Property(e => e.ConfUpdatedBy).HasColumnName("Conf_UpdatedBy");
            entity.Property(e => e.ConfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Conf_UpdatedOn");
        });

        modelBuilder.Entity<InsConfigLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("INS_Config_Log");

            entity.Property(e => e.ConfAmPm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_AmPm");
            entity.Property(e => e.ConfCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("conf_CompID");
            entity.Property(e => e.ConfConString)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("conf_ConString");
            entity.Property(e => e.ConfFrom)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("conf_From");
            entity.Property(e => e.ConfHh)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_hh");
            entity.Property(e => e.ConfInsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Conf_INS_IPAddress");
            entity.Property(e => e.ConfIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("conf_IPAddress");
            entity.Property(e => e.ConfMm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("conf_mm");
            entity.Property(e => e.ConfPort).HasColumnName("conf_Port");
            entity.Property(e => e.ConfRdbms)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("conf_RDBMS");
            entity.Property(e => e.ConfRunDate)
                .HasColumnType("datetime")
                .HasColumnName("Conf_RunDate");
            entity.Property(e => e.ConfSenderId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("conf_SenderID");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NconfAmPm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nconf_AmPm");
            entity.Property(e => e.NconfConString)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nconf_ConString");
            entity.Property(e => e.NconfFrom)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nconf_From");
            entity.Property(e => e.NconfHh)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nconf_hh");
            entity.Property(e => e.NconfIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nconf_IPAddress");
            entity.Property(e => e.NconfMm)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nconf_mm");
            entity.Property(e => e.NconfPort).HasColumnName("nconf_Port");
            entity.Property(e => e.NconfRdbms)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nconf_RDBMS");
            entity.Property(e => e.NconfSenderId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("nconf_SenderID");
        });

        modelBuilder.Entity<IntacctDdlItemText>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("IntacctDDlItemText");

            entity.Property(e => e.IntacId).HasColumnName("Intac_Id");
            entity.Property(e => e.IntacObjName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Intac_ObjName");
            entity.Property(e => e.IntacObjval)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Intac_Objval");
        });

        modelBuilder.Entity<ItreturnsClient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ITReturns_Client");

            entity.Property(e => e.ItrAadhaar)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ITR_Aadhaar");
            entity.Property(e => e.ItrClientName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ITR_ClientName");
            entity.Property(e => e.ItrCompId).HasColumnName("ITR_CompID");
            entity.Property(e => e.ItrCrBy).HasColumnName("ITR_CrBy");
            entity.Property(e => e.ItrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("ITR_CrOn");
            entity.Property(e => e.ItrDob)
                .HasColumnType("datetime")
                .HasColumnName("ITR_DOB");
            entity.Property(e => e.ItrDueDate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITR_dueDate");
            entity.Property(e => e.ItrEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ITR_Email");
            entity.Property(e => e.ItrId).HasColumnName("ITR_ID");
            entity.Property(e => e.ItrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ITR_IPAddress");
            entity.Property(e => e.ItrItloginId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITR_ITLoginId");
            entity.Property(e => e.ItrItpassword)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITR_ITPassword");
            entity.Property(e => e.ItrPan)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ITR_PAN");
            entity.Property(e => e.ItrPhone)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ITR_Phone");
            entity.Property(e => e.ItrSubDate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITR_SubDate");
            entity.Property(e => e.ItrTab).HasColumnName("ITR_Tab");
            entity.Property(e => e.ItrUpdateOn)
                .HasColumnType("datetime")
                .HasColumnName("ITR_UpdateOn");
            entity.Property(e => e.ItrUpdatedBy).HasColumnName("ITR_UpdatedBy");
            entity.Property(e => e.ItrVat)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITR_VAT");
        });

        modelBuilder.Entity<ItreturnsFilingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ITReturnsFiling_Details");

            entity.Property(e => e.ItrfdAssessmentYearId).HasColumnName("ITRFD_AssessmentYearID");
            entity.Property(e => e.ItrfdAssignTo).HasColumnName("ITRFD_AssignTo");
            entity.Property(e => e.ItrfdBillingEntityId).HasColumnName("ITRFD_BillingEntityId");
            entity.Property(e => e.ItrfdCompId).HasColumnName("ITRFD_CompID");
            entity.Property(e => e.ItrfdCrBy).HasColumnName("ITRFD_CrBy");
            entity.Property(e => e.ItrfdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("ITRFD_CrOn");
            entity.Property(e => e.ItrfdFinancialYearId).HasColumnName("ITRFD_FinancialYearID");
            entity.Property(e => e.ItrfdId).HasColumnName("ITRFD_ID");
            entity.Property(e => e.ItrfdInvoiceMail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ITRFD_InvoiceMail");
            entity.Property(e => e.ItrfdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ITRFD_IPAddress");
            entity.Property(e => e.ItrfdItrId).HasColumnName("ITRFD_ITR_ID");
            entity.Property(e => e.ItrfdItrno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ITRFD_ITRNo");
            entity.Property(e => e.ItrfdServiceChargeInInr)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("ITRFD_ServiceChargeInINR");
            entity.Property(e => e.ItrfdStatus).HasColumnName("ITRFD_Status");
            entity.Property(e => e.ItrfdTab).HasColumnName("ITRFD_Tab");
            entity.Property(e => e.ItrfdUpdateOn)
                .HasColumnType("datetime")
                .HasColumnName("ITRFD_UpdateOn");
            entity.Property(e => e.ItrfdUpdatedBy).HasColumnName("ITRFD_UpdatedBy");
        });

        modelBuilder.Entity<LoeAdditionalFee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LOE_AdditionalFees");

            entity.Property(e => e.LafCharges).HasColumnName("LAF_Charges");
            entity.Property(e => e.LafCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("LAF_CODE");
            entity.Property(e => e.LafCompId).HasColumnName("LAF_CompID");
            entity.Property(e => e.LafCrBy).HasColumnName("LAF_CrBy");
            entity.Property(e => e.LafCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LAF_CrOn");
            entity.Property(e => e.LafDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LAF_Delflag");
            entity.Property(e => e.LafId).HasColumnName("LAF_ID");
            entity.Property(e => e.LafIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LAF_IPAddress");
            entity.Property(e => e.LafLoeid).HasColumnName("LAF_LOEID");
            entity.Property(e => e.LafOtherExpensesId).HasColumnName("LAF_OtherExpensesID");
            entity.Property(e => e.LafOtherExpensesName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LAF_OtherExpensesName");
            entity.Property(e => e.LafStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("LAF_STATUS");
            entity.Property(e => e.LafUpdatedBy).HasColumnName("LAF_UpdatedBy");
            entity.Property(e => e.LafUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LAF_UpdatedOn");
        });

        modelBuilder.Entity<LoeReAmbersment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LOE_ReAmbersment");

            entity.Property(e => e.LarCharges).HasColumnName("LAR_Charges");
            entity.Property(e => e.LarCompId).HasColumnName("LAR_CompID");
            entity.Property(e => e.LarCrBy).HasColumnName("LAR_CrBy");
            entity.Property(e => e.LarCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LAR_CrOn");
            entity.Property(e => e.LarDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LAR_Delflag");
            entity.Property(e => e.LarId).HasColumnName("LAR_ID");
            entity.Property(e => e.LarIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LAR_IPAddress");
            entity.Property(e => e.LarLoeid).HasColumnName("LAR_LOEID");
            entity.Property(e => e.LarReambName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LAR_ReambName");
            entity.Property(e => e.LarReambersmentId).HasColumnName("LAR_ReambersmentID");
            entity.Property(e => e.LarStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("LAR_STATUS");
            entity.Property(e => e.LarUpdatedBy).HasColumnName("LAR_UpdatedBy");
            entity.Property(e => e.LarUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LAR_UpdatedOn");
        });

        modelBuilder.Entity<LoeResource>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LOE_Resources");

            entity.Property(e => e.LoerCategoryId).HasColumnName("LOER_CategoryID");
            entity.Property(e => e.LoerCategoryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOER_CategoryName");
            entity.Property(e => e.LoerChargesPerDay).HasColumnName("LOER_ChargesPerDay");
            entity.Property(e => e.LoerCompId).HasColumnName("LOER_CompID");
            entity.Property(e => e.LoerCrBy).HasColumnName("LOER_CrBy");
            entity.Property(e => e.LoerCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LOER_CrOn");
            entity.Property(e => e.LoerDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LOER_Delflag");
            entity.Property(e => e.LoerId).HasColumnName("LOER_ID");
            entity.Property(e => e.LoerIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LOER_IPAddress");
            entity.Property(e => e.LoerLoeid).HasColumnName("LOER_LOEID");
            entity.Property(e => e.LoerNoDays).HasColumnName("LOER_NoDays");
            entity.Property(e => e.LoerNoResources).HasColumnName("LOER_NoResources");
            entity.Property(e => e.LoerResTotal).HasColumnName("LOER_ResTotal");
            entity.Property(e => e.LoerStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("LOER_STATUS");
            entity.Property(e => e.LoerUpdatedBy).HasColumnName("LOER_UpdatedBy");
            entity.Property(e => e.LoerUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LOER_UpdatedOn");
        });

        modelBuilder.Entity<LoeTemplate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LOE_Template");

            entity.Property(e => e.LoeAttachId).HasColumnName("LOE_AttachID");
            entity.Property(e => e.LoetApprovedBy).HasColumnName("LOET_ApprovedBy");
            entity.Property(e => e.LoetApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("LOET_ApprovedOn");
            entity.Property(e => e.LoetCompId).HasColumnName("LOET_CompID");
            entity.Property(e => e.LoetCrBy).HasColumnName("LOET_CrBy");
            entity.Property(e => e.LoetCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LOET_CrOn");
            entity.Property(e => e.LoetCustomerId).HasColumnName("LOET_CustomerId");
            entity.Property(e => e.LoetDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LOET_Delflag");
            entity.Property(e => e.LoetDeliverable)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_Deliverable");
            entity.Property(e => e.LoetFrequency)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LOET_Frequency");
            entity.Property(e => e.LoetFunctionId).HasColumnName("LOET_FunctionId");
            entity.Property(e => e.LoetGeneral)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_General");
            entity.Property(e => e.LoetId).HasColumnName("LOET_Id");
            entity.Property(e => e.LoetInfrastructure)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_Infrastructure");
            entity.Property(e => e.LoetIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LOET_IPAddress");
            entity.Property(e => e.LoetLoeid).HasColumnName("LOET_LOEID");
            entity.Property(e => e.LoetNda)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_NDA");
            entity.Property(e => e.LoetProfessionalFees)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LOET_ProfessionalFees");
            entity.Property(e => e.LoetResponsibilities)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_Responsibilities");
            entity.Property(e => e.LoetScopeOfWork)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LOET_ScopeOfWork");
            entity.Property(e => e.LoetStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("LOET_STATUS");
            entity.Property(e => e.LoetStdsInternalAudit)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LOET_StdsInternalAudit");
            entity.Property(e => e.LoetUpdatedBy).HasColumnName("LOET_UpdatedBy");
            entity.Property(e => e.LoetUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LOET_UpdatedOn");
        });

        modelBuilder.Entity<LoeTemplateDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LOE_Template_Details");

            entity.Property(e => e.LtdCompId).HasColumnName("LTD_CompID");
            entity.Property(e => e.LtdCrBy).HasColumnName("LTD_CrBy");
            entity.Property(e => e.LtdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LTD_CrOn");
            entity.Property(e => e.LtdDecription)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("LTD_Decription");
            entity.Property(e => e.LtdFormName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LTD_FormName");
            entity.Property(e => e.LtdHeading)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("LTD_Heading");
            entity.Property(e => e.LtdHeadingId).HasColumnName("LTD_HeadingID");
            entity.Property(e => e.LtdId).HasColumnName("LTD_ID");
            entity.Property(e => e.LtdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LTD_IPAddress");
            entity.Property(e => e.LtdLoeId).HasColumnName("LTD_LOE_ID");
            entity.Property(e => e.LtdReportTypeId).HasColumnName("LTD_ReportTypeID");
            entity.Property(e => e.LtdUpdatedBy).HasColumnName("LTD_UpdatedBy");
            entity.Property(e => e.LtdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LTD_UpdatedOn");
        });

        modelBuilder.Entity<MmcsplDbAccess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MMCSPL_DB_Access");

            entity.Property(e => e.MdaAccessCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MDA_AccessCode");
            entity.Property(e => e.MdaApplication).HasColumnName("MDA_Application");
            entity.Property(e => e.MdaCompanyName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MDA_CompanyName");
            entity.Property(e => e.MdaCreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("MDA_CreatedDate");
            entity.Property(e => e.MdaDatabaseName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MDA_DatabaseName");
            entity.Property(e => e.MdaId).HasColumnName("MDA_ID");
            entity.Property(e => e.MdaIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MDA_IPAddress");
        });

        modelBuilder.Entity<MstChecksMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Checks_Master");

            entity.Property(e => e.ChkApprovedBy).HasColumnName("CHK_ApprovedBy");
            entity.Property(e => e.ChkApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CHK_ApprovedOn");
            entity.Property(e => e.ChkCatId).HasColumnName("CHK_CatId");
            entity.Property(e => e.ChkCheckDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CHK_CheckDesc");
            entity.Property(e => e.ChkCheckName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CHK_CheckName");
            entity.Property(e => e.ChkCompId).HasColumnName("CHK_CompID");
            entity.Property(e => e.ChkControlId).HasColumnName("CHK_ControlID");
            entity.Property(e => e.ChkCrBy).HasColumnName("CHK_CrBy");
            entity.Property(e => e.ChkCrOn)
                .HasColumnType("datetime")
                .HasColumnName("CHK_CrOn");
            entity.Property(e => e.ChkDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CHK_DelFlag");
            entity.Property(e => e.ChkDeletedBy).HasColumnName("CHK_DeletedBy");
            entity.Property(e => e.ChkDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CHK_DeletedOn");
            entity.Property(e => e.ChkId).HasColumnName("CHK_ID");
            entity.Property(e => e.ChkIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CHK_IPAddress");
            entity.Property(e => e.ChkIsKey).HasColumnName("CHK_IsKey");
            entity.Property(e => e.ChkRecallBy).HasColumnName("CHK_RecallBy");
            entity.Property(e => e.ChkRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CHK_RecallOn");
            entity.Property(e => e.ChkStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CHK_Status");
            entity.Property(e => e.ChkUpdatedBy).HasColumnName("CHK_UpdatedBy");
            entity.Property(e => e.ChkUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CHK_UpdatedOn");
        });

        modelBuilder.Entity<MstChecksMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Checks_Master_Log");

            entity.Property(e => e.ChkCatId).HasColumnName("CHK_CatId");
            entity.Property(e => e.ChkCheckDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CHK_CheckDesc");
            entity.Property(e => e.ChkCheckName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CHK_CheckName");
            entity.Property(e => e.ChkCompId).HasColumnName("CHK_CompID");
            entity.Property(e => e.ChkControlId).HasColumnName("CHK_ControlID");
            entity.Property(e => e.ChkId).HasColumnName("CHK_ID");
            entity.Property(e => e.ChkIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CHK_IPAddress");
            entity.Property(e => e.ChkIsKey).HasColumnName("CHK_IsKey");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NChkCatId).HasColumnName("nCHK_CatId");
            entity.Property(e => e.NChkCheckDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nCHK_CheckDesc");
            entity.Property(e => e.NChkCheckName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nCHK_CheckName");
            entity.Property(e => e.NChkControlId).HasColumnName("nCHK_ControlID");
            entity.Property(e => e.NChkIsKey).HasColumnName("nCHK_IsKey");
        });

        modelBuilder.Entity<MstControlLibrary>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_CONTROL_Library");

            entity.Property(e => e.MclApprovedBy).HasColumnName("MCL_ApprovedBy");
            entity.Property(e => e.MclApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("MCL_ApprovedOn");
            entity.Property(e => e.MclCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCL_Code");
            entity.Property(e => e.MclCompId).HasColumnName("MCL_CompID");
            entity.Property(e => e.MclControlDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("MCL_ControlDesc");
            entity.Property(e => e.MclControlName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MCL_ControlName");
            entity.Property(e => e.MclCrBy).HasColumnName("MCL_CrBy");
            entity.Property(e => e.MclCrOn)
                .HasColumnType("datetime")
                .HasColumnName("MCL_CrOn");
            entity.Property(e => e.MclDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MCL_DelFlag");
            entity.Property(e => e.MclDeletedBy).HasColumnName("MCL_DeletedBy");
            entity.Property(e => e.MclDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("MCL_DeletedOn");
            entity.Property(e => e.MclIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MCL_IPAddress");
            entity.Property(e => e.MclIsKey).HasColumnName("MCL_IsKey");
            entity.Property(e => e.MclPkid).HasColumnName("MCL_PKID");
            entity.Property(e => e.MclRecallBy).HasColumnName("MCL_RecallBy");
            entity.Property(e => e.MclRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("MCL_RecallOn");
            entity.Property(e => e.MclStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MCL_Status");
            entity.Property(e => e.MclUpdatedBy).HasColumnName("MCL_UpdatedBy");
            entity.Property(e => e.MclUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MCL_UpdatedOn");
        });

        modelBuilder.Entity<MstControlLibraryLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_CONTROL_Library_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.MclCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCL_Code");
            entity.Property(e => e.MclCompId).HasColumnName("MCL_CompID");
            entity.Property(e => e.MclControlDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("MCL_ControlDesc");
            entity.Property(e => e.MclControlName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MCL_ControlName");
            entity.Property(e => e.MclIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MCL_IPAddress");
            entity.Property(e => e.MclIsKey).HasColumnName("MCL_IsKey");
            entity.Property(e => e.MclPkid).HasColumnName("MCL_PKID");
            entity.Property(e => e.NMclCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nMCL_Code");
            entity.Property(e => e.NMclControlDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nMCL_ControlDesc");
            entity.Property(e => e.NMclControlName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nMCL_ControlName");
            entity.Property(e => e.NMclIsKey).HasColumnName("nMCL_IsKey");
        });

        modelBuilder.Entity<MstEntityMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MSt_Entity_Master");

            entity.Property(e => e.EntApprovedby).HasColumnName("ENT_APPROVEDBY");
            entity.Property(e => e.EntApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("ENT_APPROVEDON");
            entity.Property(e => e.EntBranch)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("Ent_Branch");
            entity.Property(e => e.EntCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ENT_CODE");
            entity.Property(e => e.EntCompId).HasColumnName("ENT_CompId");
            entity.Property(e => e.EntCrby).HasColumnName("ENT_CRBY");
            entity.Property(e => e.EntCron)
                .HasColumnType("datetime")
                .HasColumnName("ENT_CRON");
            entity.Property(e => e.EntDeletedby).HasColumnName("ENT_DELETEDBY");
            entity.Property(e => e.EntDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DELETEDON");
            entity.Property(e => e.EntDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ENT_DELFLG");
            entity.Property(e => e.EntDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ENT_Desc");
            entity.Property(e => e.EntEntityname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ENT_ENTITYNAME");
            entity.Property(e => e.EntFunManagerId).HasColumnName("Ent_FunManagerID");
            entity.Property(e => e.EntFunOwnerId).HasColumnName("Ent_FunOwnerID");
            entity.Property(e => e.EntFunSpocid).HasColumnName("Ent_FunSPOCID");
            entity.Property(e => e.EntId).HasColumnName("ENT_ID");
            entity.Property(e => e.EntIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ENT_IPAddress");
            entity.Property(e => e.EntKri).HasColumnName("ENT_KRI");
            entity.Property(e => e.EntModule).HasColumnName("Ent_Module");
            entity.Property(e => e.EntOrder).HasColumnName("ENT_ORDER");
            entity.Property(e => e.EntOrgId).HasColumnName("Ent_OrgId");
            entity.Property(e => e.EntRecallby).HasColumnName("ENT_RECALLBY");
            entity.Property(e => e.EntRecallon)
                .HasColumnType("datetime")
                .HasColumnName("ENT_RECALLON");
            entity.Property(e => e.EntRrpstatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ENT_RRPSTATUS");
            entity.Property(e => e.EntStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("ENT_STATUS");
            entity.Property(e => e.EntUpdatedby).HasColumnName("ENT_UPDATEDBY");
            entity.Property(e => e.EntUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("ENT_UPDATEDON");
        });

        modelBuilder.Entity<MstEntityMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Entity_Master_log");

            entity.Property(e => e.EntBranch)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("Ent_Branch");
            entity.Property(e => e.EntCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ENT_CODE");
            entity.Property(e => e.EntCompId).HasColumnName("ENT_CompId");
            entity.Property(e => e.EntDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ENT_Desc");
            entity.Property(e => e.EntEntityname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("ENT_ENTITYNAME");
            entity.Property(e => e.EntFunManagerId).HasColumnName("Ent_FunManagerID");
            entity.Property(e => e.EntFunOwnerId).HasColumnName("ENT_FunOwnerID");
            entity.Property(e => e.EntFunSpocid).HasColumnName("Ent_FunSPOCID");
            entity.Property(e => e.EntId).HasColumnName("ENT_ID");
            entity.Property(e => e.EntIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("ENT_IPAddress");
            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NEntBranch)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("nEnt_Branch");
            entity.Property(e => e.NEntCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nENT_CODE");
            entity.Property(e => e.NEntDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nENT_Desc");
            entity.Property(e => e.NEntEntityname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nENT_ENTITYNAME");
            entity.Property(e => e.NEntFunManagerId).HasColumnName("nEnt_FunManagerID");
            entity.Property(e => e.NEntFunOwnerId).HasColumnName("nENT_FunOwnerID");
            entity.Property(e => e.NEntFunSpocid).HasColumnName("nEnt_FunSPOCID");
        });

        modelBuilder.Entity<MstEntityUsrMap>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Entity_UsrMap");

            entity.Property(e => e.MeumCompId).HasColumnName("MEUM_CompID");
            entity.Property(e => e.MeumEntityId).HasColumnName("MEUM_EntityID");
            entity.Property(e => e.MeumPkid).HasColumnName("MEUM_PKID");
            entity.Property(e => e.MeumUsrId)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("MEUM_UsrID");
        });

        modelBuilder.Entity<MstInherentRiskMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_InherentRisk_Master");

            entity.Property(e => e.MimColor)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MIM_Color");
            entity.Property(e => e.MimCompId).HasColumnName("MIM_CompID");
            entity.Property(e => e.MimCrBy).HasColumnName("MIM_CrBy");
            entity.Property(e => e.MimCrOn)
                .HasColumnType("datetime")
                .HasColumnName("MIM_CrOn");
            entity.Property(e => e.MimDesc)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MIM_Desc");
            entity.Property(e => e.MimFrequency)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MIM_Frequency");
            entity.Property(e => e.MimFromScore).HasColumnName("MIM_FromScore");
            entity.Property(e => e.MimId).HasColumnName("MIM_ID");
            entity.Property(e => e.MimIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MIM_IPAddress");
            entity.Property(e => e.MimName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MIM_Name");
            entity.Property(e => e.MimToScore).HasColumnName("MIM_ToScore");
        });

        modelBuilder.Entity<MstMappingMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_MAPPING_MASTER");

            entity.Property(e => e.MmmApprovedBy).HasColumnName("MMM_ApprovedBy");
            entity.Property(e => e.MmmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("MMM_ApprovedOn");
            entity.Property(e => e.MmmCheckS)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MMM_CheckS");
            entity.Property(e => e.MmmChecksId).HasColumnName("MMM_ChecksID");
            entity.Property(e => e.MmmChecksKey).HasColumnName("MMM_ChecksKey");
            entity.Property(e => e.MmmCompId).HasColumnName("MMM_CompID");
            entity.Property(e => e.MmmControl)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MMM_Control");
            entity.Property(e => e.MmmControlId).HasColumnName("MMM_ControlID");
            entity.Property(e => e.MmmControlKey).HasColumnName("MMM_ControlKey");
            entity.Property(e => e.MmmCrBy).HasColumnName("MMM_CrBy");
            entity.Property(e => e.MmmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("MMM_CrOn");
            entity.Property(e => e.MmmCustid).HasColumnName("MMM_CUSTID");
            entity.Property(e => e.MmmDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MMM_DelFlag");
            entity.Property(e => e.MmmDeletedBy).HasColumnName("MMM_DeletedBy");
            entity.Property(e => e.MmmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("MMM_DeletedOn");
            entity.Property(e => e.MmmFunId).HasColumnName("MMM_FunID");
            entity.Property(e => e.MmmId).HasColumnName("MMM_ID");
            entity.Property(e => e.MmmInherentRisk)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MMM_InherentRisk");
            entity.Property(e => e.MmmInherentRiskId).HasColumnName("MMM_InherentRiskID");
            entity.Property(e => e.MmmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MMM_IPaddress");
            entity.Property(e => e.MmmModule)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MMM_Module");
            entity.Property(e => e.MmmPmid).HasColumnName("MMM_PMID");
            entity.Property(e => e.MmmRecallBy).HasColumnName("MMM_RecallBy");
            entity.Property(e => e.MmmRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("MMM_RecallOn");
            entity.Property(e => e.MmmRisk)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MMM_Risk");
            entity.Property(e => e.MmmRiskKey).HasColumnName("MMM_RiskKey");
            entity.Property(e => e.MmmRiskid).HasColumnName("MMM_RISKID");
            entity.Property(e => e.MmmSemid).HasColumnName("MMM_SEMID");
            entity.Property(e => e.MmmSpmid).HasColumnName("MMM_SPMID");
            entity.Property(e => e.MmmSpmkey).HasColumnName("MMM_SPMKey");
            entity.Property(e => e.MmmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MMM_Status");
            entity.Property(e => e.MmmUpdatedBy).HasColumnName("MMM_UpdatedBy");
            entity.Property(e => e.MmmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MMM_UpdatedOn");
            entity.Property(e => e.MmmYearId).HasColumnName("MMM_YearID");
        });

        modelBuilder.Entity<MstPasswordSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Password_Setting");

            entity.Property(e => e.MpsCompId).HasColumnName("MPS_CompID");
            entity.Property(e => e.MpsIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MPS_IPAddress");
            entity.Property(e => e.MpsMaximumChar).HasColumnName("MPS_MaximumChar");
            entity.Property(e => e.MpsMinimumChar).HasColumnName("MPS_MinimumChar");
            entity.Property(e => e.MpsNotLoginDays).HasColumnName("MPS_NotLoginDays");
            entity.Property(e => e.MpsOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("MPS_Operation");
            entity.Property(e => e.MpsPasswordContains)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MPS_Password_Contains");
            entity.Property(e => e.MpsPasswordExpiryAlertDays).HasColumnName("MPS_PasswordExpiryAlertDays");
            entity.Property(e => e.MpsPasswordExpiryDays).HasColumnName("MPS_PasswordExpiryDays");
            entity.Property(e => e.MpsPkId).HasColumnName("MPS_pkID");
            entity.Property(e => e.MpsRecoveryAttempts).HasColumnName("MPS_RecoveryAttempts");
            entity.Property(e => e.MpsUnsuccessfulAttempts).HasColumnName("MPS_UnsuccessfulAttempts");
            entity.Property(e => e.MpsUpdatedBy).HasColumnName("MPS_UpdatedBy");
            entity.Property(e => e.MpsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MPS_UpdatedOn");
        });

        modelBuilder.Entity<MstPasswordSettingLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Password_Setting_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.MpsCompId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MPS_CompID");
            entity.Property(e => e.MpsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MPS_IPAddress");
            entity.Property(e => e.MpsMaximumChar).HasColumnName("MPS_MaximumChar");
            entity.Property(e => e.MpsMinimumChar).HasColumnName("MPS_MinimumChar");
            entity.Property(e => e.MpsNotLoginDays).HasColumnName("MPS_NotLoginDays");
            entity.Property(e => e.MpsPasswordContains)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MPS_Password_Contains");
            entity.Property(e => e.MpsPasswordExpiryAlertDays).HasColumnName("MPS_PasswordExpiryAlertDays");
            entity.Property(e => e.MpsPasswordExpiryDays).HasColumnName("MPS_PasswordExpiryDays");
            entity.Property(e => e.MpsPkid).HasColumnName("MPS_PKID");
            entity.Property(e => e.MpsRunDate)
                .HasColumnType("datetime")
                .HasColumnName("MPS_RunDate");
            entity.Property(e => e.MpsUnSuccessfulAttempts).HasColumnName("MPS_UnSuccessfulAttempts");
            entity.Property(e => e.MspRecoveryAttempts).HasColumnName("MSP_RecoveryAttempts");
            entity.Property(e => e.NMpsMaximumChar).HasColumnName("nMPS_MaximumChar");
            entity.Property(e => e.NMpsMinimumChar).HasColumnName("nMPS_MinimumChar");
            entity.Property(e => e.NMpsNotLoginDays).HasColumnName("nMPS_NotLoginDays");
            entity.Property(e => e.NMpsPasswordContains)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nMPS_Password_Contains");
            entity.Property(e => e.NMpsPasswordExpiryAlertDays).HasColumnName("nMPS_PasswordExpiryAlertDays");
            entity.Property(e => e.NMpsPasswordExpiryDays).HasColumnName("nMPS_PasswordExpiryDays");
            entity.Property(e => e.NMpsUnSuccessfulAttempts).HasColumnName("nMPS_UnSuccessfulAttempts");
            entity.Property(e => e.NMspRecoveryAttempts).HasColumnName("nMSP_RecoveryAttempts");
        });

        modelBuilder.Entity<MstProcessMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_PROCESS_MASTER");

            entity.Property(e => e.PmApprovedby).HasColumnName("PM_APPROVEDBY");
            entity.Property(e => e.PmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("PM_APPROVEDON");
            entity.Property(e => e.PmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PM_CODE");
            entity.Property(e => e.PmCompId).HasColumnName("PM_CompID");
            entity.Property(e => e.PmCrby).HasColumnName("PM_CRBY");
            entity.Property(e => e.PmCron)
                .HasColumnType("datetime")
                .HasColumnName("PM_CRON");
            entity.Property(e => e.PmDeletedby).HasColumnName("PM_DELETEDBY");
            entity.Property(e => e.PmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("PM_DELETEDON");
            entity.Property(e => e.PmDelflg)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PM_DELFLG");
            entity.Property(e => e.PmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PM_Desc");
            entity.Property(e => e.PmEntId).HasColumnName("PM_ENT_ID");
            entity.Property(e => e.PmId).HasColumnName("PM_ID");
            entity.Property(e => e.PmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("PM_IPAddress");
            entity.Property(e => e.PmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PM_NAME");
            entity.Property(e => e.PmRecallby).HasColumnName("PM_RECALLBY");
            entity.Property(e => e.PmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("PM_RECALLON");
            entity.Property(e => e.PmSemId).HasColumnName("PM_SEM_ID");
            entity.Property(e => e.PmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("PM_STATUS");
            entity.Property(e => e.PmUpdatedby).HasColumnName("PM_UPDATEDBY");
            entity.Property(e => e.PmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("PM_UPDATEDON");
        });

        modelBuilder.Entity<MstProcessMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_PROCESS_MASTER_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NPmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nPM_CODE");
            entity.Property(e => e.NPmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nPM_Desc");
            entity.Property(e => e.NPmEntId).HasColumnName("nPM_ENT_ID");
            entity.Property(e => e.NPmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nPM_NAME");
            entity.Property(e => e.NPmSemId).HasColumnName("nPM_SEM_ID");
            entity.Property(e => e.PmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PM_CODE");
            entity.Property(e => e.PmCompId).HasColumnName("PM_CompID");
            entity.Property(e => e.PmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PM_Desc");
            entity.Property(e => e.PmEntId).HasColumnName("PM_ENT_ID");
            entity.Property(e => e.PmId).HasColumnName("PM_ID");
            entity.Property(e => e.PmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("PM_IPAddress");
            entity.Property(e => e.PmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PM_NAME");
            entity.Property(e => e.PmSemId).HasColumnName("PM_SEM_ID");
        });

        modelBuilder.Entity<MstRiskColorMatrix>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_Risk_Color_Matrix");

            entity.Property(e => e.RcmCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_Category");
            entity.Property(e => e.RcmColorsName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_ColorsName");
            entity.Property(e => e.RcmColumnId).HasColumnName("RCM_ColumnID");
            entity.Property(e => e.RcmCompId).HasColumnName("RCM_CompID");
            entity.Property(e => e.RcmCreatedBy).HasColumnName("RCM_CreatedBy");
            entity.Property(e => e.RcmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_CreatedOn");
            entity.Property(e => e.RcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_IPAddress");
            entity.Property(e => e.RcmRowId).HasColumnName("RCM_RowID");
            entity.Property(e => e.RcmUpdatedBy).HasColumnName("RCM_UpdatedBy");
            entity.Property(e => e.RcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_UpdatedOn");
        });

        modelBuilder.Entity<MstRiskLibrary>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_RISK_Library");

            entity.Property(e => e.MrlApprovedBy).HasColumnName("MRL_ApprovedBy");
            entity.Property(e => e.MrlApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("MRL_ApprovedOn");
            entity.Property(e => e.MrlCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MRL_Code");
            entity.Property(e => e.MrlCompId).HasColumnName("MRL_CompID");
            entity.Property(e => e.MrlCrBy).HasColumnName("MRL_CrBy");
            entity.Property(e => e.MrlCrOn)
                .HasColumnType("datetime")
                .HasColumnName("MRL_CrOn");
            entity.Property(e => e.MrlDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MRL_DelFlag");
            entity.Property(e => e.MrlDeletedBy).HasColumnName("MRL_DeletedBy");
            entity.Property(e => e.MrlDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("MRL_DeletedOn");
            entity.Property(e => e.MrlInherentRiskId).HasColumnName("MRL_InherentRiskID");
            entity.Property(e => e.MrlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MRL_IPAddress");
            entity.Property(e => e.MrlIsKey).HasColumnName("MRL_IsKey");
            entity.Property(e => e.MrlModule)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MRL_Module");
            entity.Property(e => e.MrlPkid).HasColumnName("MRL_PKID");
            entity.Property(e => e.MrlRecallBy).HasColumnName("MRL_RecallBy");
            entity.Property(e => e.MrlRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("MRL_RecallOn");
            entity.Property(e => e.MrlRiskDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("MRL_RiskDesc");
            entity.Property(e => e.MrlRiskName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MRL_RiskName");
            entity.Property(e => e.MrlRiskTypeId).HasColumnName("MRL_RiskTypeID");
            entity.Property(e => e.MrlStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MRL_Status");
            entity.Property(e => e.MrlUpdatedBy).HasColumnName("MRL_UpdatedBy");
            entity.Property(e => e.MrlUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("MRL_UpdatedOn");
        });

        modelBuilder.Entity<MstRiskLibraryLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_RISK_Library_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.MrlCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MRL_Code");
            entity.Property(e => e.MrlCompId).HasColumnName("MRL_CompID");
            entity.Property(e => e.MrlIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MRL_IPAddress");
            entity.Property(e => e.MrlIsKey).HasColumnName("MRL_IsKey");
            entity.Property(e => e.MrlPkid).HasColumnName("MRL_PKID");
            entity.Property(e => e.MrlRiskDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("MRL_RiskDesc");
            entity.Property(e => e.MrlRiskName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("MRL_RiskName");
            entity.Property(e => e.MrlRiskTypeId).HasColumnName("MRL_RiskTypeID");
            entity.Property(e => e.NMrlCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nMRL_Code");
            entity.Property(e => e.NMrlIsKey).HasColumnName("nMRL_IsKey");
            entity.Property(e => e.NMrlRiskDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nMRL_RiskDesc");
            entity.Property(e => e.NMrlRiskName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nMRL_RiskName");
            entity.Property(e => e.NMrlRiskTypeId).HasColumnName("nMRL_RiskTypeID");
        });

        modelBuilder.Entity<MstSubentityMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_SUBENTITY_MASTER");

            entity.Property(e => e.SemApprovedby).HasColumnName("SEM_APPROVEDBY");
            entity.Property(e => e.SemApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("SEM_APPROVEDON");
            entity.Property(e => e.SemCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SEM_CODE");
            entity.Property(e => e.SemCompId).HasColumnName("SEM_CompID");
            entity.Property(e => e.SemCrby).HasColumnName("SEM_CRBY");
            entity.Property(e => e.SemCron)
                .HasColumnType("datetime")
                .HasColumnName("SEM_CRON");
            entity.Property(e => e.SemDeletedby).HasColumnName("SEM_DELETEDBY");
            entity.Property(e => e.SemDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("SEM_DELETEDON");
            entity.Property(e => e.SemDelflg)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SEM_DELFLG");
            entity.Property(e => e.SemDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SEM_Desc");
            entity.Property(e => e.SemEntId).HasColumnName("SEM_Ent_ID");
            entity.Property(e => e.SemId).HasColumnName("SEM_ID");
            entity.Property(e => e.SemIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SEM_IPAddress");
            entity.Property(e => e.SemName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SEM_NAME");
            entity.Property(e => e.SemRecallby).HasColumnName("SEM_RECALLBY");
            entity.Property(e => e.SemRecallon)
                .HasColumnType("datetime")
                .HasColumnName("SEM_RECALLON");
            entity.Property(e => e.SemStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SEM_STATUS");
            entity.Property(e => e.SemUpdatedby).HasColumnName("SEM_UPDATEDBY");
            entity.Property(e => e.SemUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SEM_UPDATEDON");
        });

        modelBuilder.Entity<MstSubentityMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_SUBENTITY_MASTER_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NSemCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nSEM_CODE");
            entity.Property(e => e.NSemDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nSEM_Desc");
            entity.Property(e => e.NSemEntId).HasColumnName("nSEM_Ent_ID");
            entity.Property(e => e.NSemName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nSEM_NAME");
            entity.Property(e => e.SemCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SEM_CODE");
            entity.Property(e => e.SemCompId).HasColumnName("SEM_CompID");
            entity.Property(e => e.SemDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SEM_Desc");
            entity.Property(e => e.SemEntId).HasColumnName("SEM_Ent_ID");
            entity.Property(e => e.SemId).HasColumnName("SEM_ID");
            entity.Property(e => e.SemIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SEM_IPAddress");
            entity.Property(e => e.SemName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SEM_NAME");
        });

        modelBuilder.Entity<MstSubprocessMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_SUBPROCESS_MASTER");

            entity.Property(e => e.SpmApprovedby).HasColumnName("SPM_APPROVEDBY");
            entity.Property(e => e.SpmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("SPM_APPROVEDON");
            entity.Property(e => e.SpmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SPM_CODE");
            entity.Property(e => e.SpmCompId).HasColumnName("SPM_CompID");
            entity.Property(e => e.SpmCrby).HasColumnName("SPM_CRBY");
            entity.Property(e => e.SpmCron)
                .HasColumnType("datetime")
                .HasColumnName("SPM_CRON");
            entity.Property(e => e.SpmDeletedby).HasColumnName("SPM_DELETEDBY");
            entity.Property(e => e.SpmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("SPM_DELETEDON");
            entity.Property(e => e.SpmDelflg)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SPM_DELFLG");
            entity.Property(e => e.SpmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SPM_Desc");
            entity.Property(e => e.SpmEntId).HasColumnName("SPM_ENT_ID");
            entity.Property(e => e.SpmId).HasColumnName("SPM_ID");
            entity.Property(e => e.SpmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SPM_IPAddress");
            entity.Property(e => e.SpmIsKey).HasColumnName("SPM_IsKey");
            entity.Property(e => e.SpmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SPM_NAME");
            entity.Property(e => e.SpmPmId).HasColumnName("SPM_PM_ID");
            entity.Property(e => e.SpmRecallby).HasColumnName("SPM_RECALLBY");
            entity.Property(e => e.SpmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("SPM_RECALLON");
            entity.Property(e => e.SpmSemId).HasColumnName("SPM_SEM_ID");
            entity.Property(e => e.SpmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SPM_STATUS");
            entity.Property(e => e.SpmUpdatedby).HasColumnName("SPM_UPDATEDBY");
            entity.Property(e => e.SpmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SPM_UPDATEDON");
        });

        modelBuilder.Entity<MstSubprocessMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MST_SUBPROCESS_MASTER_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NSpmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nSPM_CODE");
            entity.Property(e => e.NSpmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nSPM_Desc");
            entity.Property(e => e.NSpmEntId).HasColumnName("nSPM_ENT_ID");
            entity.Property(e => e.NSpmIsKey).HasColumnName("nSPM_IsKey");
            entity.Property(e => e.NSpmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nSPM_NAME");
            entity.Property(e => e.NSpmPmId).HasColumnName("nSPM_PM_ID");
            entity.Property(e => e.NSpmSemId).HasColumnName("nSPM_SEM_ID");
            entity.Property(e => e.SpmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SPM_CODE");
            entity.Property(e => e.SpmCompId).HasColumnName("SPM_CompID");
            entity.Property(e => e.SpmDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SPM_Desc");
            entity.Property(e => e.SpmEntId).HasColumnName("SPM_ENT_ID");
            entity.Property(e => e.SpmId).HasColumnName("SPM_ID");
            entity.Property(e => e.SpmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SPM_IPAddress");
            entity.Property(e => e.SpmIsKey).HasColumnName("SPM_IsKey");
            entity.Property(e => e.SpmName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SPM_NAME");
            entity.Property(e => e.SpmPmId).HasColumnName("SPM_PM_ID");
            entity.Property(e => e.SpmSemId).HasColumnName("SPM_SEM_ID");
        });

        modelBuilder.Entity<NotificationMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("notification_Master");

            entity.Property(e => e.NmApprovedby).HasColumnName("Nm_APPROVEDBY");
            entity.Property(e => e.NmApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("Nm_APPROVEDON");
            entity.Property(e => e.NmCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Nm_Code");
            entity.Property(e => e.NmCompId).HasColumnName("Nm_CompId");
            entity.Property(e => e.NmCrby).HasColumnName("Nm_CRBY");
            entity.Property(e => e.NmCron)
                .HasColumnType("datetime")
                .HasColumnName("Nm_CRON");
            entity.Property(e => e.NmDeletedby).HasColumnName("Nm_DELETEDBY");
            entity.Property(e => e.NmDeletedon)
                .HasColumnType("datetime")
                .HasColumnName("Nm_DELETEDON");
            entity.Property(e => e.NmDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Nm_DELFLG");
            entity.Property(e => e.NmDesc)
                .HasMaxLength(800)
                .IsUnicode(false)
                .HasColumnName("Nm_desc");
            entity.Property(e => e.NmDuedate)
                .HasColumnType("datetime")
                .HasColumnName("Nm_Duedate");
            entity.Property(e => e.NmFrequency).HasColumnName("Nm_Frequency");
            entity.Property(e => e.NmId).HasColumnName("Nm_ID");
            entity.Property(e => e.NmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Nm_IPAddress");
            entity.Property(e => e.NmName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nm_Name");
            entity.Property(e => e.NmNoOfDays).HasColumnName("Nm_NoOfDays");
            entity.Property(e => e.NmPosition).HasColumnName("Nm_position");
            entity.Property(e => e.NmRecallby).HasColumnName("Nm_RECALLBY");
            entity.Property(e => e.NmRecallon)
                .HasColumnType("datetime")
                .HasColumnName("Nm_RECALLON");
            entity.Property(e => e.NmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Nm_STATUS");
            entity.Property(e => e.NmUpdatedby).HasColumnName("Nm_UPDATEDBY");
            entity.Property(e => e.NmUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("Nm_UPDATEDON");
        });

        modelBuilder.Entity<QaAssessment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QA_Assessment");

            entity.Property(e => e.QaAuditorteam)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("QA_AUDITORTEAM");
            entity.Property(e => e.QaAudittitle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("QA_AUDITTITLE");
            entity.Property(e => e.QaAudstatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QA_AUDStatus");
            entity.Property(e => e.QaCode)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QA_Code");
            entity.Property(e => e.QaCompId).HasColumnName("QA_CompID");
            entity.Property(e => e.QaCrBy).HasColumnName("QA_CrBy");
            entity.Property(e => e.QaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("QA_CrOn");
            entity.Property(e => e.QaCustid).HasColumnName("QA_CUSTID");
            entity.Property(e => e.QaDelflag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("QA_Delflag");
            entity.Property(e => e.QaEndDate)
                .HasColumnType("datetime")
                .HasColumnName("QA_EndDate");
            entity.Property(e => e.QaFinancialYear).HasColumnName("QA_FinancialYear");
            entity.Property(e => e.QaFunid).HasColumnName("QA_FUNID");
            entity.Property(e => e.QaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QA_IPAddress");
            entity.Property(e => e.QaPkid).HasColumnName("QA_PKID");
            entity.Property(e => e.QaSavedBy).HasColumnName("QA_SavedBy");
            entity.Property(e => e.QaSavedOn)
                .HasColumnType("datetime")
                .HasColumnName("QA_SavedOn");
            entity.Property(e => e.QaStartDate)
                .HasColumnType("datetime")
                .HasColumnName("QA_StartDate");
            entity.Property(e => e.QaUpdatedBy).HasColumnName("QA_UpdatedBy");
            entity.Property(e => e.QaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("QA_UpdatedOn");
            entity.Property(e => e.QaWpstatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QA_WPStatus");
        });

        modelBuilder.Entity<QaWorkPaper>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QA_WorkPaper");

            entity.Property(e => e.QawAttachId).HasColumnName("QAW_AttachID");
            entity.Property(e => e.QawAuditCode).HasColumnName("QAW_AuditCode");
            entity.Property(e => e.QawAuditeeResponseName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("QAW_AuditeeResponseName");
            entity.Property(e => e.QawAuditorObservationName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("QAW_AuditorObservationName");
            entity.Property(e => e.QawAuditorRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("QAW_AuditorRemarks");
            entity.Property(e => e.QawChecksId).HasColumnName("QAW_ChecksID");
            entity.Property(e => e.QawCompId).HasColumnName("QAW_CompID");
            entity.Property(e => e.QawConclusionId).HasColumnName("QAW_ConclusionID");
            entity.Property(e => e.QawControlId).HasColumnName("QAW_ControlID");
            entity.Property(e => e.QawCrBy).HasColumnName("QAW_CrBy");
            entity.Property(e => e.QawCrOn)
                .HasColumnType("datetime")
                .HasColumnName("QAW_CrOn");
            entity.Property(e => e.QawCustId).HasColumnName("QAW_CustID");
            entity.Property(e => e.QawFunctionId).HasColumnName("QAW_FunctionID");
            entity.Property(e => e.QawIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QAW_IPAddress");
            entity.Property(e => e.QawNote)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("QAW_Note");
            entity.Property(e => e.QawOpenCloseStatus).HasColumnName("QAW_OpenCloseStatus");
            entity.Property(e => e.QawPgedetailId).HasColumnName("QAW_PGEDetailId");
            entity.Property(e => e.QawPkid).HasColumnName("QAW_PKID");
            entity.Property(e => e.QawProcessId).HasColumnName("QAW_ProcessID");
            entity.Property(e => e.QawResponse)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("QAW_Response");
            entity.Property(e => e.QawReviewedBy).HasColumnName("QAW_ReviewedBy");
            entity.Property(e => e.QawReviewedOn)
                .HasColumnType("datetime")
                .HasColumnName("QAW_ReviewedOn");
            entity.Property(e => e.QawReviewerRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("QAW_ReviewerRemarks");
            entity.Property(e => e.QawRiskId).HasColumnName("QAW_RiskID");
            entity.Property(e => e.QawStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("QAW_Status");
            entity.Property(e => e.QawSubFunctionId).HasColumnName("QAW_SubFunctionID");
            entity.Property(e => e.QawSubProcessId).HasColumnName("QAW_SubProcessID");
            entity.Property(e => e.QawSubmittedBy).HasColumnName("QAW_SubmittedBy");
            entity.Property(e => e.QawSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("QAW_SubmittedOn");
            entity.Property(e => e.QawTypeofTestId).HasColumnName("QAW_TypeofTestID");
            entity.Property(e => e.QawUpdatedBy).HasColumnName("QAW_UpdatedBy");
            entity.Property(e => e.QawUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("QAW_UpdatedOn");
            entity.Property(e => e.QawWorkPaperDone)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("QAW_WorkPaperDone");
            entity.Property(e => e.QawWorkPaperNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("QAW_WorkPaperNo");
            entity.Property(e => e.QawYearId).HasColumnName("QAW_YearID");
        });

        modelBuilder.Entity<QaaChecksMatrix>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QAA_ChecksMatrix");

            entity.Property(e => e.QamChecksId).HasColumnName("QAM_ChecksID");
            entity.Property(e => e.QamCompId).HasColumnName("QAM_CompID");
            entity.Property(e => e.QamControlId).HasColumnName("QAM_ControlID");
            entity.Property(e => e.QamCustId).HasColumnName("QAM_CustID");
            entity.Property(e => e.QamFunctionId).HasColumnName("QAM_FunctionID");
            entity.Property(e => e.QamIpaddress)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("QAM_IPAddress");
            entity.Property(e => e.QamMmmid).HasColumnName("QAM_MMMID");
            entity.Property(e => e.QamPkid).HasColumnName("QAM_PKID");
            entity.Property(e => e.QamProcessId).HasColumnName("QAM_ProcessID");
            entity.Property(e => e.QamQapkid).HasColumnName("QAM_QAPKID");
            entity.Property(e => e.QamRiskId).HasColumnName("QAM_RiskID");
            entity.Property(e => e.QamStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("QAM_Status");
            entity.Property(e => e.QamSubFunctionId).HasColumnName("QAM_SubFunctionID");
            entity.Property(e => e.QamSubProcessId).HasColumnName("QAM_SubProcessID");
            entity.Property(e => e.QamYearId).HasColumnName("QAM_YearID");
        });

        modelBuilder.Entity<RiskBrrchecklistDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRChecklist_Details");

            entity.Property(e => e.BrrdAnnexure)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BRRD_Annexure");
            entity.Property(e => e.BrrdAreaId).HasColumnName("BRRD_AreaID");
            entity.Property(e => e.BrrdAttachId).HasColumnName("BRRD_AttachID");
            entity.Property(e => e.BrrdBrrpkid).HasColumnName("BRRD_BRRPKID");
            entity.Property(e => e.BrrdCheckPoint)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BRRD_CheckPoint");
            entity.Property(e => e.BrrdCompId).HasColumnName("BRRD_CompID");
            entity.Property(e => e.BrrdDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BRRD_DelFlag");
            entity.Property(e => e.BrrdFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BRRD_FunType");
            entity.Property(e => e.BrrdFunctionId).HasColumnName("BRRD_FunctionID");
            entity.Property(e => e.BrrdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRRD_IPAddress");
            entity.Property(e => e.BrrdIssueDetails)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("BRRD_IssueDetails");
            entity.Property(e => e.BrrdMethodology)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRRD_Methodology");
            entity.Property(e => e.BrrdMethodologyId).HasColumnName("BRRD_MethodologyID");
            entity.Property(e => e.BrrdOweightage).HasColumnName("BRRD_OWeightage");
            entity.Property(e => e.BrrdPkid).HasColumnName("BRRD_PKID");
            entity.Property(e => e.BrrdRcmid).HasColumnName("BRRD_RCMID");
            entity.Property(e => e.BrrdRefNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("BRRD_RefNo");
            entity.Property(e => e.BrrdRiskCategory)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("BRRD_RiskCategory");
            entity.Property(e => e.BrrdRiskScore).HasColumnName("BRRD_RiskScore");
            entity.Property(e => e.BrrdSampleSizeId).HasColumnName("BRRD_SampleSizeID");
            entity.Property(e => e.BrrdSampleSizeName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRRD_SampleSizeName");
            entity.Property(e => e.BrrdStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("BRRD_Status");
            entity.Property(e => e.BrrdWeightedRiskScore).HasColumnName("BRRD_WeightedRiskScore");
            entity.Property(e => e.BrrdYesnona).HasColumnName("BRRD_YESNONA");
        });

        modelBuilder.Entity<RiskBrrchecklistMa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRChecklist_Mas");

            entity.Property(e => e.BrrAedate)
                .HasColumnType("datetime")
                .HasColumnName("BRR_AEDate");
            entity.Property(e => e.BrrApprovedBy).HasColumnName("BRR_ApprovedBy");
            entity.Property(e => e.BrrApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRR_ApprovedOn");
            entity.Property(e => e.BrrAsdate)
                .HasColumnType("datetime")
                .HasColumnName("BRR_ASDate");
            entity.Property(e => e.BrrAsgId).HasColumnName("BRR_AsgID");
            entity.Property(e => e.BrrAttachId).HasColumnName("BRR_AttachID");
            entity.Property(e => e.BrrBranchId).HasColumnName("BRR_BranchId");
            entity.Property(e => e.BrrCompId).HasColumnName("BRR_CompID");
            entity.Property(e => e.BrrCrBy).HasColumnName("BRR_CrBy");
            entity.Property(e => e.BrrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BRR_CrOn");
            entity.Property(e => e.BrrCustId).HasColumnName("BRR_CustID");
            entity.Property(e => e.BrrFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BRR_Flag");
            entity.Property(e => e.BrrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRR_IPAddress");
            entity.Property(e => e.BrrPgedetailId).HasColumnName("BRR_PGEDetailId");
            entity.Property(e => e.BrrPkid).HasColumnName("BRR_PKID");
            entity.Property(e => e.BrrRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRR_Remarks");
            entity.Property(e => e.BrrStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BRR_Status");
            entity.Property(e => e.BrrSubmittedBy).HasColumnName("BRR_SubmittedBy");
            entity.Property(e => e.BrrSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRR_SubmittedOn");
            entity.Property(e => e.BrrTitle)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRR_Title");
            entity.Property(e => e.BrrUpdatedBy).HasColumnName("BRR_UpdatedBy");
            entity.Property(e => e.BrrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRR_UpdatedOn");
            entity.Property(e => e.BrrYearId).HasColumnName("BRR_YearID");
        });

        modelBuilder.Entity<RiskBrrissueTracker>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRIssueTracker");

            entity.Property(e => e.BbritActionPlan)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BBRIT_ActionPlan");
            entity.Property(e => e.BbritAreaId).HasColumnName("BBRIT_AreaID");
            entity.Property(e => e.BbritAsgNo).HasColumnName("BBRIT_AsgNo");
            entity.Property(e => e.BbritAttchId).HasColumnName("BBRIT_AttchID");
            entity.Property(e => e.BbritBranchId).HasColumnName("BBRIT_BranchId");
            entity.Property(e => e.BbritBrrdpkid).HasColumnName("BBRIT_BRRDPKID");
            entity.Property(e => e.BbritCheckPointId).HasColumnName("BBRIT_CheckPointID");
            entity.Property(e => e.BbritCompId).HasColumnName("BBRIT_CompID");
            entity.Property(e => e.BbritCrBy).HasColumnName("BBRIT_CrBy");
            entity.Property(e => e.BbritCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BBRIT_CrOn");
            entity.Property(e => e.BbritCustId).HasColumnName("BBRIT_CustID");
            entity.Property(e => e.BbritDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BBRIT_DelFlag");
            entity.Property(e => e.BbritFinancialYear).HasColumnName("BBRIT_FinancialYear");
            entity.Property(e => e.BbritFunctionId).HasColumnName("BBRIT_FunctionID");
            entity.Property(e => e.BbritIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BBRIT_IPAddress");
            entity.Property(e => e.BbritIssueDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BBRIT_IssueDesc");
            entity.Property(e => e.BbritIssueHeading)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BBRIT_IssueHeading");
            entity.Property(e => e.BbritOpenCloseStatus).HasColumnName("BBRIT_OpenCloseStatus");
            entity.Property(e => e.BbritPkid).HasColumnName("BBRIT_PKID");
            entity.Property(e => e.BbritRcmid).HasColumnName("BBRIT_RCMID");
            entity.Property(e => e.BbritRemaks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BBRIT_Remaks");
            entity.Property(e => e.BbritResponsible).HasColumnName("BBRIT_Responsible");
            entity.Property(e => e.BbritStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("BBRIT_Status");
            entity.Property(e => e.BbritSubmittedBy).HasColumnName("BBRIT_SubmittedBy");
            entity.Property(e => e.BbritSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("BBRIT_SubmittedOn");
            entity.Property(e => e.BbritTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("BBRIT_TargetDate");
            entity.Property(e => e.BbritUpdatedBy).HasColumnName("BBRIT_UpdatedBy");
            entity.Property(e => e.BbritUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BBRIT_UpdatedOn");
        });

        modelBuilder.Entity<RiskBrrissueTrackerHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRIssueTracker_History");

            entity.Property(e => e.BrrithActionPlan)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRRITH_ActionPlan");
            entity.Property(e => e.BrrithAsgNo).HasColumnName("BRRITH_AsgNo");
            entity.Property(e => e.BrrithBbritpkid).HasColumnName("BRRITH_BBRITPKID");
            entity.Property(e => e.BrrithCompId).HasColumnName("BRRITH_CompID");
            entity.Property(e => e.BrrithCrBy).HasColumnName("BRRITH_CrBy");
            entity.Property(e => e.BrrithCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRITH_CrOn");
            entity.Property(e => e.BrrithCustId).HasColumnName("BRRITH_CustID");
            entity.Property(e => e.BrrithIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRRITH_IPAddress");
            entity.Property(e => e.BrrithOpenCloseStatus).HasColumnName("BRRITH_OpenCloseStatus");
            entity.Property(e => e.BrrithPkid).HasColumnName("BRRITH_PKID");
            entity.Property(e => e.BrrithRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BRRITH_Remarks");
            entity.Property(e => e.BrrithTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("BRRITH_TargetDate");
            entity.Property(e => e.BrrithUpdatedBy).HasColumnName("BRRITH_UpdatedBy");
            entity.Property(e => e.BrrithUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRITH_UpdatedOn");
        });

        modelBuilder.Entity<RiskBrrplanning>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRPlanning");

            entity.Property(e => e.BrrpAaplan).HasColumnName("BRRP_AAPlan");
            entity.Property(e => e.BrrpBcmratingId).HasColumnName("BRRP_BCMRatingID");
            entity.Property(e => e.BrrpBranchId).HasColumnName("BRRP_BranchID");
            entity.Property(e => e.BrrpBrrratingId).HasColumnName("BRRP_BRRRatingID");
            entity.Property(e => e.BrrpCompId).HasColumnName("BRRP_CompID");
            entity.Property(e => e.BrrpCrBy).HasColumnName("BRRP_CrBy");
            entity.Property(e => e.BrrpCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRP_CrOn");
            entity.Property(e => e.BrrpCustId).HasColumnName("BRRP_CustId");
            entity.Property(e => e.BrrpDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("BRRP_DelFlag");
            entity.Property(e => e.BrrpGrossControlScore).HasColumnName("BRRP_GrossControlScore");
            entity.Property(e => e.BrrpIaratingId).HasColumnName("BRRP_IARatingID");
            entity.Property(e => e.BrrpIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRRP_IPAddress");
            entity.Property(e => e.BrrpNetScore).HasColumnName("BRRP_NetScore");
            entity.Property(e => e.BrrpPkid).HasColumnName("BRRP_PKID");
            entity.Property(e => e.BrrpRegionId).HasColumnName("BRRP_RegionID");
            entity.Property(e => e.BrrpRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("BRRP_Remarks");
            entity.Property(e => e.BrrpRiskScore).HasColumnName("BRRP_RiskScore");
            entity.Property(e => e.BrrpSalesUnitCode).HasColumnName("BRRP_SalesUnitCode");
            entity.Property(e => e.BrrpStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("BRRP_Status");
            entity.Property(e => e.BrrpSubmittedBy).HasColumnName("BRRP_SubmittedBy");
            entity.Property(e => e.BrrpSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRP_SubmittedOn");
            entity.Property(e => e.BrrpUpdatedBy).HasColumnName("BRRP_UpdatedBy");
            entity.Property(e => e.BrrpUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRP_UpdatedOn");
            entity.Property(e => e.BrrpYearId).HasColumnName("BRRP_YearId");
            entity.Property(e => e.BrrpZoneId).HasColumnName("BRRP_ZoneID");
        });

        modelBuilder.Entity<RiskBrrreport>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRReport");

            entity.Property(e => e.BrrrActionPlanDate)
                .HasColumnType("datetime")
                .HasColumnName("BRRR_ActionPlanDate");
            entity.Property(e => e.BrrrAreaId).HasColumnName("BRRR_AreaID");
            entity.Property(e => e.BrrrAsgId).HasColumnName("BRRR_AsgID");
            entity.Property(e => e.BrrrAttachId).HasColumnName("BRRR_AttachID");
            entity.Property(e => e.BrrrBbritid).HasColumnName("BRRR_BBRITID");
            entity.Property(e => e.BrrrBranchId).HasColumnName("BRRR_BranchId");
            entity.Property(e => e.BrrrBrrdid).HasColumnName("BRRR_BRRDID");
            entity.Property(e => e.BrrrClosingDate)
                .HasColumnType("datetime")
                .HasColumnName("BRRR_ClosingDate");
            entity.Property(e => e.BrrrCompId).HasColumnName("BRRR_CompID");
            entity.Property(e => e.BrrrCreatedBy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("BRRR_CreatedBy");
            entity.Property(e => e.BrrrCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRR_CreatedOn");
            entity.Property(e => e.BrrrCustId).HasColumnName("BRRR_CustID");
            entity.Property(e => e.BrrrDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("BRRR_DelFlag");
            entity.Property(e => e.BrrrDisAgreedRsn)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BRRR_DisAgreedRsn");
            entity.Property(e => e.BrrrFunctionId).HasColumnName("BRRR_FunctionID");
            entity.Property(e => e.BrrrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRRR_IPAddress");
            entity.Property(e => e.BrrrIssuAgreed).HasColumnName("BRRR_IssuAgreed");
            entity.Property(e => e.BrrrIssuStatus).HasColumnName("BRRR_IssuStatus");
            entity.Property(e => e.BrrrPgedetailId).HasColumnName("BRRR_PGEDetailId");
            entity.Property(e => e.BrrrPkid).HasColumnName("BRRR_Pkid");
            entity.Property(e => e.BrrrStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("BRRR_Status");
            entity.Property(e => e.BrrrUpdatedBy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("BRRR_UpdatedBy");
            entity.Property(e => e.BrrrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRR_UpdatedOn");
            entity.Property(e => e.BrrrYearId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("BRRR_YearID");
        });

        modelBuilder.Entity<RiskBrrschedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_BRRSchedule");

            entity.Property(e => e.BrrsApprovedBy).HasColumnName("BRRS_ApprovedBy");
            entity.Property(e => e.BrrsApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRS_ApprovedOn");
            entity.Property(e => e.BrrsAsgNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BRRS_AsgNo");
            entity.Property(e => e.BrrsAttchId).HasColumnName("BRRS_AttchID");
            entity.Property(e => e.BrrsBranchId).HasColumnName("BRRS_BranchID");
            entity.Property(e => e.BrrsBranchMgrId).HasColumnName("BRRS_BranchMgrID");
            entity.Property(e => e.BrrsCompId).HasColumnName("BRRS_CompID");
            entity.Property(e => e.BrrsCrBy).HasColumnName("BRRS_CrBy");
            entity.Property(e => e.BrrsCrOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRS_CrOn");
            entity.Property(e => e.BrrsCustId).HasColumnName("BRRS_CustID");
            entity.Property(e => e.BrrsEmployeeId).HasColumnName("BRRS_EmployeeID");
            entity.Property(e => e.BrrsFinancialYear).HasColumnName("BRRS_FinancialYear");
            entity.Property(e => e.BrrsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("BRRS_IPAddress");
            entity.Property(e => e.BrrsPkid).HasColumnName("BRRS_PKID");
            entity.Property(e => e.BrrsRegionId).HasColumnName("BRRS_RegionID");
            entity.Property(e => e.BrrsRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("BRRS_Remarks");
            entity.Property(e => e.BrrsReviewerTypeId).HasColumnName("BRRS_ReviewerTypeID");
            entity.Property(e => e.BrrsScheduleMonth).HasColumnName("BRRS_ScheduleMonth");
            entity.Property(e => e.BrrsStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BRRS_Status");
            entity.Property(e => e.BrrsSubmittedBy).HasColumnName("BRRS_SubmittedBy");
            entity.Property(e => e.BrrsSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRS_SubmittedOn");
            entity.Property(e => e.BrrsUpdatedBy).HasColumnName("BRRS_UpdatedBy");
            entity.Property(e => e.BrrsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("BRRS_UpdatedOn");
            entity.Property(e => e.BrrsZonalMgrId).HasColumnName("BRRS_ZonalMgrID");
            entity.Property(e => e.BrrsZoneId).HasColumnName("BRRS_ZoneID");
        });

        modelBuilder.Entity<RiskCheckListMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_CheckList_Master");

            entity.Property(e => e.RcmApprovedBy).HasColumnName("RCM_ApprovedBy");
            entity.Property(e => e.RcmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_ApprovedOn");
            entity.Property(e => e.RcmAreaId).HasColumnName("RCM_AreaId");
            entity.Property(e => e.RcmAreaNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCM_AreaNo");
            entity.Property(e => e.RcmCheckPoint)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("RCM_CheckPoint");
            entity.Property(e => e.RcmCheckPointNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_CheckPointNo");
            entity.Property(e => e.RcmCompId).HasColumnName("RCM_CompID");
            entity.Property(e => e.RcmCrBy).HasColumnName("RCM_CrBy");
            entity.Property(e => e.RcmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_CrOn");
            entity.Property(e => e.RcmCustId).HasColumnName("RCM_CustID");
            entity.Property(e => e.RcmDeletedBy).HasColumnName("RCM_DeletedBy");
            entity.Property(e => e.RcmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_DeletedOn");
            entity.Property(e => e.RcmDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("RCM_Delflag");
            entity.Property(e => e.RcmFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("RCM_FunType");
            entity.Property(e => e.RcmFunctionId).HasColumnName("RCM_FunctionId");
            entity.Property(e => e.RcmId).HasColumnName("RCM_Id");
            entity.Property(e => e.RcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_IPAddress");
            entity.Property(e => e.RcmMethodologyId).HasColumnName("RCM_MethodologyId");
            entity.Property(e => e.RcmRecallBy).HasColumnName("RCM_RecallBy");
            entity.Property(e => e.RcmRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_RecallOn");
            entity.Property(e => e.RcmRiskCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_RiskCategory");
            entity.Property(e => e.RcmRiskWeight).HasColumnName("RCM_RiskWeight");
            entity.Property(e => e.RcmSampleSize).HasColumnName("RCM_SampleSize");
            entity.Property(e => e.RcmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("RCM_Status");
            entity.Property(e => e.RcmUpdatedBy).HasColumnName("RCM_UpdatedBy");
            entity.Property(e => e.RcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_UpdatedOn");
            entity.Property(e => e.RcmYearId).HasColumnName("RCM_YearId");
        });

        modelBuilder.Entity<RiskCheckListMaster1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_CheckList_Master1");

            entity.Property(e => e.RcmApprovedBy).HasColumnName("RCM_ApprovedBy");
            entity.Property(e => e.RcmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_ApprovedOn");
            entity.Property(e => e.RcmAreaId).HasColumnName("RCM_AreaId");
            entity.Property(e => e.RcmAreaNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCM_AreaNo");
            entity.Property(e => e.RcmCheckPoint)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("RCM_CheckPoint");
            entity.Property(e => e.RcmCheckPointNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_CheckPointNo");
            entity.Property(e => e.RcmCompId).HasColumnName("RCM_CompID");
            entity.Property(e => e.RcmCrBy).HasColumnName("RCM_CrBy");
            entity.Property(e => e.RcmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_CrOn");
            entity.Property(e => e.RcmDeletedBy).HasColumnName("RCM_DeletedBy");
            entity.Property(e => e.RcmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_DeletedOn");
            entity.Property(e => e.RcmDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("RCM_Delflag");
            entity.Property(e => e.RcmFunType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("RCM_FunType");
            entity.Property(e => e.RcmFunctionId).HasColumnName("RCM_FunctionId");
            entity.Property(e => e.RcmId).HasColumnName("RCM_Id");
            entity.Property(e => e.RcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_IPAddress");
            entity.Property(e => e.RcmMethodologyId).HasColumnName("RCM_MethodologyId");
            entity.Property(e => e.RcmRecallBy).HasColumnName("RCM_RecallBy");
            entity.Property(e => e.RcmRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_RecallOn");
            entity.Property(e => e.RcmRiskCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_RiskCategory");
            entity.Property(e => e.RcmRiskWeight).HasColumnName("RCM_RiskWeight");
            entity.Property(e => e.RcmSampleSize).HasColumnName("RCM_SampleSize");
            entity.Property(e => e.RcmStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("RCM_Status");
            entity.Property(e => e.RcmUpdatedBy).HasColumnName("RCM_UpdatedBy");
            entity.Property(e => e.RcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_UpdatedOn");
            entity.Property(e => e.RcmYearId).HasColumnName("RCM_YearId");
        });

        modelBuilder.Entity<RiskColorMatrix>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_Color_Matrix");

            entity.Property(e => e.RcmCategory)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RCM_Category");
            entity.Property(e => e.RcmColorsName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_ColorsName");
            entity.Property(e => e.RcmColumnId).HasColumnName("RCM_ColumnID");
            entity.Property(e => e.RcmCompId).HasColumnName("RCM_CompID");
            entity.Property(e => e.RcmCreatedBy).HasColumnName("RCM_CreatedBy");
            entity.Property(e => e.RcmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_CreatedOn");
            entity.Property(e => e.RcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCM_IPAddress");
            entity.Property(e => e.RcmRowId).HasColumnName("RCM_RowID");
            entity.Property(e => e.RcmUpdatedBy).HasColumnName("RCM_UpdatedBy");
            entity.Property(e => e.RcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_UpdatedOn");
        });

        modelBuilder.Entity<RiskGeneralMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_GeneralMaster");

            entity.Property(e => e.RamApprovedBy).HasColumnName("RAM_ApprovedBy");
            entity.Property(e => e.RamApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RAM_ApprovedOn");
            entity.Property(e => e.RamCategory)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RAM_Category");
            entity.Property(e => e.RamCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RAM_Code");
            entity.Property(e => e.RamColor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RAM_Color");
            entity.Property(e => e.RamCompId).HasColumnName("RAM_CompID");
            entity.Property(e => e.RamCrBy).HasColumnName("RAM_CrBy");
            entity.Property(e => e.RamCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RAM_CrOn");
            entity.Property(e => e.RamDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("RAM_DelFlag");
            entity.Property(e => e.RamDeletedBy).HasColumnName("RAM_DeletedBy");
            entity.Property(e => e.RamDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("RAM_DeletedOn");
            entity.Property(e => e.RamEndValue).HasColumnName("RAM_EndValue");
            entity.Property(e => e.RamIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RAM_IPAddress");
            entity.Property(e => e.RamName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("RAM_Name");
            entity.Property(e => e.RamPkid).HasColumnName("RAM_PKID");
            entity.Property(e => e.RamRecallBy).HasColumnName("RAM_RecallBy");
            entity.Property(e => e.RamRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("RAM_RecallOn");
            entity.Property(e => e.RamRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RAM_Remarks");
            entity.Property(e => e.RamScore).HasColumnName("RAM_Score");
            entity.Property(e => e.RamStartValue).HasColumnName("RAM_StartValue");
            entity.Property(e => e.RamStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("RAM_Status");
            entity.Property(e => e.RamUpdatedBy).HasColumnName("RAM_UpdatedBy");
            entity.Property(e => e.RamUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RAM_UpdatedOn");
            entity.Property(e => e.RamYearId).HasColumnName("RAM_YearID");
        });

        modelBuilder.Entity<RiskGeneralMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RISK_GeneralMASTER_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NRamCategory)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nRAM_Category");
            entity.Property(e => e.NRamCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nRAM_Code");
            entity.Property(e => e.NRamColor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nRAM_Color");
            entity.Property(e => e.NRamEndValue).HasColumnName("nRAM_EndValue");
            entity.Property(e => e.NRamName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nRAM_Name");
            entity.Property(e => e.NRamRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("nRAM_Remarks");
            entity.Property(e => e.NRamScore).HasColumnName("nRAM_Score");
            entity.Property(e => e.NRamStartValue).HasColumnName("nRAM_StartValue");
            entity.Property(e => e.NRamYearId).HasColumnName("nRAM_YearID");
            entity.Property(e => e.RamCategory)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RAM_Category");
            entity.Property(e => e.RamCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RAM_Code");
            entity.Property(e => e.RamColor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RAM_Color");
            entity.Property(e => e.RamCompId).HasColumnName("RAM_CompID");
            entity.Property(e => e.RamEndValue).HasColumnName("RAM_EndValue");
            entity.Property(e => e.RamIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RAM_IPAddress");
            entity.Property(e => e.RamName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("RAM_Name");
            entity.Property(e => e.RamPkid).HasColumnName("RAM_PKID");
            entity.Property(e => e.RamRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RAM_Remarks");
            entity.Property(e => e.RamScore).HasColumnName("RAM_Score");
            entity.Property(e => e.RamStartValue).HasColumnName("RAM_StartValue");
            entity.Property(e => e.RamYearId).HasColumnName("RAM_YearID");
        });

        modelBuilder.Entity<RiskIssueTracker>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_IssueTracker");

            entity.Property(e => e.RitActionPlan)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("RIT_ActionPlan");
            entity.Property(e => e.RitActualLoss)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("RIT_ActualLoss");
            entity.Property(e => e.RitAsgNo).HasColumnName("RIT_AsgNo");
            entity.Property(e => e.RitAttchId).HasColumnName("RIT_AttchID");
            entity.Property(e => e.RitCompId).HasColumnName("RIT_CompID");
            entity.Property(e => e.RitControlId).HasColumnName("RIT_ControlID");
            entity.Property(e => e.RitCrBy).HasColumnName("RIT_CrBy");
            entity.Property(e => e.RitCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RIT_CrOn");
            entity.Property(e => e.RitCustId).HasColumnName("RIT_CustID");
            entity.Property(e => e.RitFinancialYear).HasColumnName("RIT_FinancialYear");
            entity.Property(e => e.RitFunId).HasColumnName("RIT_FunID");
            entity.Property(e => e.RitIndividualResponsible).HasColumnName("RIT_IndividualResponsible");
            entity.Property(e => e.RitIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RIT_IPAddress");
            entity.Property(e => e.RitIssueDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RIT_Issue_Desc");
            entity.Property(e => e.RitIssueHeading)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RIT_IssueHeading");
            entity.Property(e => e.RitIssueNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RIT_IssueNo");
            entity.Property(e => e.RitManagerResponsible).HasColumnName("RIT_ManagerResponsible");
            entity.Property(e => e.RitOpenCloseStatus).HasColumnName("RIT_OpenCloseStatus");
            entity.Property(e => e.RitPgedetailId).HasColumnName("RIT_PGEDetailId");
            entity.Property(e => e.RitPkid).HasColumnName("RIT_PKID");
            entity.Property(e => e.RitProbableLoss)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("RIT_ProbableLoss");
            entity.Property(e => e.RitReferenceNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RIT_ReferenceNo");
            entity.Property(e => e.RitRemaks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RIT_Remaks");
            entity.Property(e => e.RitRiskId).HasColumnName("RIT_RiskID");
            entity.Property(e => e.RitRiskTypeId).HasColumnName("RIT_RiskTypeID");
            entity.Property(e => e.RitSource)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RIT_Source");
            entity.Property(e => e.RitStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RIT_Status");
            entity.Property(e => e.RitSubFunId).HasColumnName("RIT_SubFunID");
            entity.Property(e => e.RitSubmittedBy).HasColumnName("RIT_SubmittedBy");
            entity.Property(e => e.RitSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RIT_SubmittedOn");
            entity.Property(e => e.RitTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("RIT_TargetDate");
            entity.Property(e => e.RitUpdatedBy).HasColumnName("RIT_UpdatedBy");
            entity.Property(e => e.RitUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RIT_UpdatedOn");
        });

        modelBuilder.Entity<RiskIssueTrackerHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_IssueTracker_History");

            entity.Property(e => e.RithActionPlan)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("RITH_ActionPlan");
            entity.Property(e => e.RithAsgNo).HasColumnName("RITH_AsgNo");
            entity.Property(e => e.RithCompId).HasColumnName("RITH_CompID");
            entity.Property(e => e.RithCrBy).HasColumnName("RITH_CrBy");
            entity.Property(e => e.RithCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RITH_CrOn");
            entity.Property(e => e.RithIndividualResponsible).HasColumnName("RITH_IndividualResponsible");
            entity.Property(e => e.RithIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RITH_IPAddress");
            entity.Property(e => e.RithManagerResponsible).HasColumnName("RITH_ManagerResponsible");
            entity.Property(e => e.RithOpenCloseStatus).HasColumnName("RITH_OpenCloseStatus");
            entity.Property(e => e.RithPkid).HasColumnName("RITH_PKID");
            entity.Property(e => e.RithRemaks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RITH_Remaks");
            entity.Property(e => e.RithRitpkid).HasColumnName("RITH_RITPKID");
            entity.Property(e => e.RithTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("RITH_TargetDate");
            entity.Property(e => e.RithUpdatedBy).HasColumnName("RITH_UpdatedBy");
            entity.Property(e => e.RithUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RITH_UpdatedOn");
        });

        modelBuilder.Entity<RiskKccPlanningSchecdulingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_KCC_PlanningSchecduling_Details");

            entity.Property(e => e.KccAsgNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_AsgNo");
            entity.Property(e => e.KccCompId).HasColumnName("KCC_CompID");
            entity.Property(e => e.KccConductAttachId).HasColumnName("KCC_ConductAttachID");
            entity.Property(e => e.KccConductPgedetailId).HasColumnName("KCC_ConductPGEDetailId");
            entity.Property(e => e.KccConductingActualClosure)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ConductingActualClosure");
            entity.Property(e => e.KccConductingActualStartDate)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ConductingActualStartDate");
            entity.Property(e => e.KccConductingCrBy).HasColumnName("KCC_ConductingCrBy");
            entity.Property(e => e.KccConductingCrOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ConductingCrOn");
            entity.Property(e => e.KccConductingIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_ConductingIPaddress");
            entity.Property(e => e.KccConductingKccstatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_ConductingKCCStatus");
            entity.Property(e => e.KccConductingRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KCC_ConductingRemarks");
            entity.Property(e => e.KccConductingStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_ConductingStatus");
            entity.Property(e => e.KccConductingSubmittedBy).HasColumnName("KCC_ConductingSubmittedBy");
            entity.Property(e => e.KccConductingSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ConductingSubmittedOn");
            entity.Property(e => e.KccConductingUpdatedBy).HasColumnName("KCC_ConductingUpdatedBy");
            entity.Property(e => e.KccConductingUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ConductingUpdatedOn");
            entity.Property(e => e.KccCrBy).HasColumnName("KCC_CrBy");
            entity.Property(e => e.KccCrOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_CrOn");
            entity.Property(e => e.KccCustId).HasColumnName("KCC_CustID");
            entity.Property(e => e.KccFunId).HasColumnName("KCC_FunID");
            entity.Property(e => e.KccIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_IPaddress");
            entity.Property(e => e.KccPkid).HasColumnName("KCC_PKID");
            entity.Property(e => e.KccPlanningAttachId).HasColumnName("KCC_PlanningAttachID");
            entity.Property(e => e.KccPlanningPgedetailId).HasColumnName("KCC_PlanningPGEDetailId");
            entity.Property(e => e.KccReviewerId).HasColumnName("KCC_ReviewerID");
            entity.Property(e => e.KccReviewerTypeId).HasColumnName("KCC_ReviewerTypeID");
            entity.Property(e => e.KccRiskReportReferenceNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KCC_RiskReportReferenceNo");
            entity.Property(e => e.KccScheduleClosure)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ScheduleClosure");
            entity.Property(e => e.KccScheduleStartDate)
                .HasColumnType("datetime")
                .HasColumnName("KCC_ScheduleStartDate");
            entity.Property(e => e.KccScope)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KCC_Scope");
            entity.Property(e => e.KccStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KCC_Status");
            entity.Property(e => e.KccSubFunId).HasColumnName("KCC_SubFunID");
            entity.Property(e => e.KccSubmittedBy).HasColumnName("KCC_SubmittedBy");
            entity.Property(e => e.KccSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_SubmittedOn");
            entity.Property(e => e.KccTitle)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KCC_Title");
            entity.Property(e => e.KccUpdatedBy).HasColumnName("KCC_UpdatedBy");
            entity.Property(e => e.KccUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("KCC_UpdatedOn");
            entity.Property(e => e.KccYearId).HasColumnName("KCC_YearID");
        });

        modelBuilder.Entity<RiskKir>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_KIR");

            entity.Property(e => e.KirActionAgainstEmp)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_ActionAgainstEmp");
            entity.Property(e => e.KirActionAgainstInter)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_ActionAgainstInter");
            entity.Property(e => e.KirActualLoss)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_ActualLoss");
            entity.Property(e => e.KirAdvisorCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_AdvisorCode");
            entity.Property(e => e.KirAdvisorName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_AdvisorName");
            entity.Property(e => e.KirAsgnDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_AsgnDate");
            entity.Property(e => e.KirAssignmentDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_AssignmentDate");
            entity.Property(e => e.KirBusinessSegment)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_BusinessSegment");
            entity.Property(e => e.KirCaseClassification)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_CaseClassification");
            entity.Property(e => e.KirCaseSummary)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("KIR_CaseSummary");
            entity.Property(e => e.KirCauseInitiationDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_CauseInitiationDate");
            entity.Property(e => e.KirCedcdate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_CEDCDate");
            entity.Property(e => e.KirChannel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Channel");
            entity.Property(e => e.KirClosureDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_ClosureDate");
            entity.Property(e => e.KirClosureDays)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_ClosureDays");
            entity.Property(e => e.KirCompId).HasColumnName("KIR_CompID");
            entity.Property(e => e.KirContractNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_ContractNo");
            entity.Property(e => e.KirCrBy).HasColumnName("KIR_CrBy");
            entity.Property(e => e.KirCrOn)
                .HasColumnType("datetime")
                .HasColumnName("KIR_CrOn");
            entity.Property(e => e.KirCustName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_CustName");
            entity.Property(e => e.KirDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("KIR_DelFlag");
            entity.Property(e => e.KirDeviationRsn)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("KIR_DeviationRsn");
            entity.Property(e => e.KirEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Email");
            entity.Property(e => e.KirEmpCode).HasColumnName("KIR_EmpCode");
            entity.Property(e => e.KirEmpName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_EmpName");
            entity.Property(e => e.KirEntityInv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_EntityInv");
            entity.Property(e => e.KirFirfrwdDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_FIRfrwdDate");
            entity.Property(e => e.KirFraudReptdStage)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_FraudReptdStage");
            entity.Property(e => e.KirInvOutcome)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_InvOutcome");
            entity.Property(e => e.KirInvSummary)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("KIR_InvSummary");
            entity.Property(e => e.KirIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KIR_IPAddress");
            entity.Property(e => e.KirIssuanceDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_IssuanceDate");
            entity.Property(e => e.KirKirstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_KIRStatus");
            entity.Property(e => e.KirLawName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_LawName");
            entity.Property(e => e.KirLocation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Location");
            entity.Property(e => e.KirLoginDate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_LoginDate");
            entity.Property(e => e.KirLossAmtRecvd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_LossAmtRecvd");
            entity.Property(e => e.KirMatrixAction)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("KIR_MatrixAction");
            entity.Property(e => e.KirMonth)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Month");
            entity.Property(e => e.KirNoActionRsn)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("KIR_NoActionRsn");
            entity.Property(e => e.KirNotionalLoss)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_NotionalLoss");
            entity.Property(e => e.KirPkid).HasColumnName("KIR_Pkid");
            entity.Property(e => e.KirPlan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Plan");
            entity.Property(e => e.KirPreDispAction)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_PreDispAction");
            entity.Property(e => e.KirPremium)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_Premium");
            entity.Property(e => e.KirPreventiveStep)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_PreventiveStep");
            entity.Property(e => e.KirRcaname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_RCAName");
            entity.Property(e => e.KirRcastatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_RCAstatus");
            entity.Property(e => e.KirRegion).HasColumnName("KIR_Region");
            entity.Property(e => e.KirRiskActionable)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_RiskActionable");
            entity.Property(e => e.KirRiskType).HasColumnName("KIR_RiskType");
            entity.Property(e => e.KirSmcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_SMCode");
            entity.Property(e => e.KirSmname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_SMName");
            entity.Property(e => e.KirStatus)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("KIR_STATUS");
            entity.Property(e => e.KirSumAssured)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_SumAssured");
            entity.Property(e => e.KirTerm)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_Term");
            entity.Property(e => e.KirTraceRefNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_TraceRefNo");
            entity.Property(e => e.KirTrigger)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("KIR_Trigger");
            entity.Property(e => e.KirYearId).HasColumnName("KIR_YearID");
            entity.Property(e => e.KirZcar)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KIR_ZCAR");
            entity.Property(e => e.KirZedcdate)
                .HasColumnType("datetime")
                .HasColumnName("KIR_ZEDCDate");
            entity.Property(e => e.KirZone).HasColumnName("KIR_Zone");
        });

        modelBuilder.Entity<RiskKri>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_KRI");

            entity.Property(e => e.KriAttachId).HasColumnName("KRI_AttachID");
            entity.Property(e => e.KriCategoryId).HasColumnName("KRI_CategoryID");
            entity.Property(e => e.KriCompId).HasColumnName("KRI_CompId");
            entity.Property(e => e.KriCrBy).HasColumnName("KRI_CrBy");
            entity.Property(e => e.KriCrOn)
                .HasColumnType("datetime")
                .HasColumnName("KRI_CrOn");
            entity.Property(e => e.KriDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("KRI_DelFlag");
            entity.Property(e => e.KriDeletedBy).HasColumnName("KRI_DeletedBy");
            entity.Property(e => e.KriDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("KRI_DeletedOn");
            entity.Property(e => e.KriIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("KRI_IPAddress");
            entity.Property(e => e.KriMeasureId).HasColumnName("KRI_MeasureID");
            entity.Property(e => e.KriPeriodId).HasColumnName("KRI_PeriodID");
            entity.Property(e => e.KriPkid).HasColumnName("KRI_PKID");
            entity.Property(e => e.KriRiskDescription)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("KRI_RiskDescription");
            entity.Property(e => e.KriRiskId).HasColumnName("KRI_RiskID");
            entity.Property(e => e.KriStatus)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("KRI_STATUS");
            entity.Property(e => e.KriSubCategoryId).HasColumnName("KRI_SubCategoryID");
            entity.Property(e => e.KriYearId).HasColumnName("KRI_YearID");
        });

        modelBuilder.Entity<RiskRa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RA");

            entity.Property(e => e.RaApprovedBy).HasColumnName("RA_ApprovedBy");
            entity.Property(e => e.RaApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RA_ApprovedOn");
            entity.Property(e => e.RaAsgNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RA_AsgNo");
            entity.Property(e => e.RaComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RA_Comments");
            entity.Property(e => e.RaCompId).HasColumnName("RA_CompID");
            entity.Property(e => e.RaCrBy).HasColumnName("RA_CrBy");
            entity.Property(e => e.RaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RA_CrOn");
            entity.Property(e => e.RaCustId).HasColumnName("RA_CustID");
            entity.Property(e => e.RaFinancialYear).HasColumnName("RA_FinancialYear");
            entity.Property(e => e.RaFunId).HasColumnName("RA_FunID");
            entity.Property(e => e.RaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RA_IPAddress");
            entity.Property(e => e.RaMasterStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RA_MasterStatus");
            entity.Property(e => e.RaNetScore).HasColumnName("RA_NetScore");
            entity.Property(e => e.RaPkid).HasColumnName("RA_PKID");
            entity.Property(e => e.RaReAssignBy).HasColumnName("RA_ReAssignBy");
            entity.Property(e => e.RaReAssignOn)
                .HasColumnType("datetime")
                .HasColumnName("RA_ReAssignOn");
            entity.Property(e => e.RaStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RA_Status");
            entity.Property(e => e.RaSubmittedBy).HasColumnName("RA_SubmittedBy");
            entity.Property(e => e.RaSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RA_SubmittedOn");
            entity.Property(e => e.RaUpdatedBy).HasColumnName("RA_UpdatedBy");
            entity.Property(e => e.RaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RA_UpdatedOn");
        });

        modelBuilder.Entity<RiskRaActionPlanHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RA_ActionPlan_History");

            entity.Property(e => e.RahActionPlan)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RAH_ActionPlan");
            entity.Property(e => e.RahCompId).HasColumnName("RAH_CompID");
            entity.Property(e => e.RahCrBy).HasColumnName("RAH_CrBy");
            entity.Property(e => e.RahCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RAH_CrOn");
            entity.Property(e => e.RahCustid).HasColumnName("RAH_CUSTID");
            entity.Property(e => e.RahFactorDecrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RAH_FactorDecrease");
            entity.Property(e => e.RahFactorIncrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RAH_FactorIncrease");
            entity.Property(e => e.RahFinancialYear).HasColumnName("RAH_FinancialYear");
            entity.Property(e => e.RahFunid).HasColumnName("RAH_FUNID");
            entity.Property(e => e.RahPkid).HasColumnName("RAH_PKID");
            entity.Property(e => e.RahRapkid).HasColumnName("RAH_RAPKID");
            entity.Property(e => e.RahTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("RAH_TargetDate");
        });

        modelBuilder.Entity<RiskRaConductHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RA_Conduct_History");

            entity.Property(e => e.RaahComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RAAH_Comments");
            entity.Property(e => e.RaahCompId).HasColumnName("RAAH_CompID");
            entity.Property(e => e.RaahDate)
                .HasColumnType("datetime")
                .HasColumnName("RAAH_Date");
            entity.Property(e => e.RaahIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RAAH_IPAddress");
            entity.Property(e => e.RaahPkid).HasColumnName("RAAH_PKID");
            entity.Property(e => e.RaahRaapkid).HasColumnName("RAAH_RAAPKID");
            entity.Property(e => e.RaahStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RAAH_Status");
            entity.Property(e => e.RaahUserId).HasColumnName("RAAH_UserID");
        });

        modelBuilder.Entity<RiskRaDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RA_Details");

            entity.Property(e => e.RadChecksId).HasColumnName("RAD_ChecksID");
            entity.Property(e => e.RadCompId).HasColumnName("RAD_CompID");
            entity.Property(e => e.RadControlId).HasColumnName("RAD_ControlID");
            entity.Property(e => e.RadControlRating).HasColumnName("RAD_ControlRating");
            entity.Property(e => e.RadDes).HasColumnName("RAD_DES");
            entity.Property(e => e.RadImpactId).HasColumnName("RAD_ImpactID");
            entity.Property(e => e.RadIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RAD_IPAddress");
            entity.Property(e => e.RadLikelihoodId).HasColumnName("RAD_LikelihoodID");
            entity.Property(e => e.RadOes).HasColumnName("RAD_OES");
            entity.Property(e => e.RadPkid).HasColumnName("RAD_PKID");
            entity.Property(e => e.RadPmid).HasColumnName("RAD_PMID");
            entity.Property(e => e.RadRapkid).HasColumnName("RAD_RAPKID");
            entity.Property(e => e.RadRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RAD_Remarks");
            entity.Property(e => e.RadResidualRiskRating).HasColumnName("RAD_ResidualRiskRating");
            entity.Property(e => e.RadRiskId).HasColumnName("RAD_RiskID");
            entity.Property(e => e.RadRiskRating).HasColumnName("RAD_RiskRating");
            entity.Property(e => e.RadRiskTypeId).HasColumnName("RAD_RiskTypeID");
            entity.Property(e => e.RadSemid).HasColumnName("RAD_SEMID");
            entity.Property(e => e.RadSpmid).HasColumnName("RAD_SPMID");
        });

        modelBuilder.Entity<RiskRcsa>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RCSA");

            entity.Property(e => e.RcsaActionPlan)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RCSA_ActionPlan");
            entity.Property(e => e.RcsaApprovedBy).HasColumnName("RCSA_ApprovedBy");
            entity.Property(e => e.RcsaApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_ApprovedOn");
            entity.Property(e => e.RcsaAsgNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSA_AsgNo");
            entity.Property(e => e.RcsaBsubmittedBy).HasColumnName("RCSA_BSubmittedBy");
            entity.Property(e => e.RcsaBsubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_BSubmittedOn");
            entity.Property(e => e.RcsaBupdatedBy).HasColumnName("RCSA_BUpdatedBy");
            entity.Property(e => e.RcsaBupdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_BUpdatedOn");
            entity.Property(e => e.RcsaComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RCSA_Comments");
            entity.Property(e => e.RcsaCompId).HasColumnName("RCSA_CompID");
            entity.Property(e => e.RcsaCrBy).HasColumnName("RCSA_CrBy");
            entity.Property(e => e.RcsaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_CrOn");
            entity.Property(e => e.RcsaCustId).HasColumnName("RCSA_CustID");
            entity.Property(e => e.RcsaFactorDecrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCSA_FactorDecrease");
            entity.Property(e => e.RcsaFactorIncrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCSA_FactorIncrease");
            entity.Property(e => e.RcsaFinancialYear).HasColumnName("RCSA_FinancialYear");
            entity.Property(e => e.RcsaFunId).HasColumnName("RCSA_FunID");
            entity.Property(e => e.RcsaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSA_IPAddress");
            entity.Property(e => e.RcsaNetScore).HasColumnName("RCSA_NetScore");
            entity.Property(e => e.RcsaOwnerId).HasColumnName("RCSA_OwnerID");
            entity.Property(e => e.RcsaPkid).HasColumnName("RCSA_PKID");
            entity.Property(e => e.RcsaReAssignBy).HasColumnName("RCSA_ReAssignBy");
            entity.Property(e => e.RcsaReAssignOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_ReAssignOn");
            entity.Property(e => e.RcsaRsubmittedBy).HasColumnName("RCSA_RSubmittedBy");
            entity.Property(e => e.RcsaRsubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_RSubmittedOn");
            entity.Property(e => e.RcsaRupdatedBy).HasColumnName("RCSA_RUpdatedBy");
            entity.Property(e => e.RcsaRupdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_RUpdatedOn");
            entity.Property(e => e.RcsaStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSA_Status");
            entity.Property(e => e.RcsaTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("RCSA_TargetDate");
        });

        modelBuilder.Entity<RiskRcsaActionPlanHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RCSA_ActionPlan_History");

            entity.Property(e => e.RahActionPlan)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RAH_ActionPlan");
            entity.Property(e => e.RahCompId).HasColumnName("RAH_CompID");
            entity.Property(e => e.RahCrBy).HasColumnName("RAH_CrBy");
            entity.Property(e => e.RahCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RAH_CrOn");
            entity.Property(e => e.RahCustid).HasColumnName("RAH_CUSTID");
            entity.Property(e => e.RahFactorDecrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RAH_FactorDecrease");
            entity.Property(e => e.RahFactorIncrease)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RAH_FactorIncrease");
            entity.Property(e => e.RahFinancialYear).HasColumnName("RAH_FinancialYear");
            entity.Property(e => e.RahFunid).HasColumnName("RAH_FUNID");
            entity.Property(e => e.RahPkid).HasColumnName("RAH_PKID");
            entity.Property(e => e.RahRcsapkid).HasColumnName("RAH_RCSAPKID");
            entity.Property(e => e.RahTargetDate)
                .HasColumnType("datetime")
                .HasColumnName("RAH_TargetDate");
        });

        modelBuilder.Entity<RiskRcsaAssignHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RCSA_Assign_History");

            entity.Property(e => e.RcsaahComments)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RCSAAH_Comments");
            entity.Property(e => e.RcsaahCompId).HasColumnName("RCSAAH_CompID");
            entity.Property(e => e.RcsaahDate)
                .HasColumnType("datetime")
                .HasColumnName("RCSAAH_Date");
            entity.Property(e => e.RcsaahIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSAAH_IPAddress");
            entity.Property(e => e.RcsaahPkid).HasColumnName("RCSAAH_PKID");
            entity.Property(e => e.RcsaahRcsaapkid).HasColumnName("RCSAAH_RCSAAPKID");
            entity.Property(e => e.RcsaahStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSAAH_Status");
            entity.Property(e => e.RcsaahUserId).HasColumnName("RCSAAH_UserID");
        });

        modelBuilder.Entity<RiskRcsaDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RCSA_Details");

            entity.Property(e => e.RcsadChecksId).HasColumnName("RCSAD_ChecksID");
            entity.Property(e => e.RcsadCompId).HasColumnName("RCSAD_CompID");
            entity.Property(e => e.RcsadControlId).HasColumnName("RCSAD_ControlID");
            entity.Property(e => e.RcsadControlRating).HasColumnName("RCSAD_ControlRating");
            entity.Property(e => e.RcsadDes).HasColumnName("RCSAD_DES");
            entity.Property(e => e.RcsadImpactId).HasColumnName("RCSAD_ImpactID");
            entity.Property(e => e.RcsadIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RCSAD_IPAddress");
            entity.Property(e => e.RcsadLikelihoodId).HasColumnName("RCSAD_LikelihoodID");
            entity.Property(e => e.RcsadOes).HasColumnName("RCSAD_OES");
            entity.Property(e => e.RcsadPkid).HasColumnName("RCSAD_PKID");
            entity.Property(e => e.RcsadPmid).HasColumnName("RCSAD_PMID");
            entity.Property(e => e.RcsadRcsapkid).HasColumnName("RCSAD_RCSAPKID");
            entity.Property(e => e.RcsadRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCSAD_Remarks");
            entity.Property(e => e.RcsadRemarksRt)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RCSAD_RemarksRT");
            entity.Property(e => e.RcsadResidualRiskRating).HasColumnName("RCSAD_ResidualRiskRating");
            entity.Property(e => e.RcsadRiskId).HasColumnName("RCSAD_RiskID");
            entity.Property(e => e.RcsadRiskRating).HasColumnName("RCSAD_RiskRating");
            entity.Property(e => e.RcsadRiskTypeId).HasColumnName("RCSAD_RiskTypeID");
            entity.Property(e => e.RcsadSemid).HasColumnName("RCSAD_SEMID");
            entity.Property(e => e.RcsadSpmid).HasColumnName("RCSAD_SPMID");
        });

        modelBuilder.Entity<RiskRrfPlanningSchecdulingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Risk_RRF_PlanningSchecduling_Details");

            entity.Property(e => e.RpdAsgNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_AsgNo");
            entity.Property(e => e.RpdCompId).HasColumnName("RPD_CompID");
            entity.Property(e => e.RpdConductAttachId).HasColumnName("RPD_ConductAttachID");
            entity.Property(e => e.RpdConductPgedetailId).HasColumnName("RPD_ConductPGEDetailId");
            entity.Property(e => e.RpdConductingActualClosure)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ConductingActualClosure");
            entity.Property(e => e.RpdConductingActualStartDate)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ConductingActualStartDate");
            entity.Property(e => e.RpdConductingCrBy).HasColumnName("RPD_ConductingCrBy");
            entity.Property(e => e.RpdConductingCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ConductingCrOn");
            entity.Property(e => e.RpdConductingIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_ConductingIPaddress");
            entity.Property(e => e.RpdConductingRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RPD_ConductingRemarks");
            entity.Property(e => e.RpdConductingRrstatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_ConductingRRStatus");
            entity.Property(e => e.RpdConductingStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_ConductingStatus");
            entity.Property(e => e.RpdConductingSubmittedBy).HasColumnName("RPD_ConductingSubmittedBy");
            entity.Property(e => e.RpdConductingSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ConductingSubmittedOn");
            entity.Property(e => e.RpdConductingUpdatedBy).HasColumnName("RPD_ConductingUpdatedBy");
            entity.Property(e => e.RpdConductingUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ConductingUpdatedOn");
            entity.Property(e => e.RpdCrBy).HasColumnName("RPD_CrBy");
            entity.Property(e => e.RpdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_CrOn");
            entity.Property(e => e.RpdCustId).HasColumnName("RPD_CustID");
            entity.Property(e => e.RpdFunId).HasColumnName("RPD_FunID");
            entity.Property(e => e.RpdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_IPaddress");
            entity.Property(e => e.RpdPgedetailId).HasColumnName("RPD_PGEDetailId");
            entity.Property(e => e.RpdPkid).HasColumnName("RPD_PKID");
            entity.Property(e => e.RpdPlanningAttachId).HasColumnName("RPD_PlanningAttachID");
            entity.Property(e => e.RpdRefNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("RPD_RefNO");
            entity.Property(e => e.RpdReviewerId).HasColumnName("RPD_ReviewerID");
            entity.Property(e => e.RpdReviewerTypeId).HasColumnName("RPD_ReviewerTypeID");
            entity.Property(e => e.RpdScheduleClosure)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ScheduleClosure");
            entity.Property(e => e.RpdScheduleStartDate)
                .HasColumnType("datetime")
                .HasColumnName("RPD_ScheduleStartDate");
            entity.Property(e => e.RpdScope)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RPD_Scope");
            entity.Property(e => e.RpdStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RPD_Status");
            entity.Property(e => e.RpdSubFunId).HasColumnName("RPD_SubFunID");
            entity.Property(e => e.RpdSubmittedBy).HasColumnName("RPD_SubmittedBy");
            entity.Property(e => e.RpdSubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_SubmittedOn");
            entity.Property(e => e.RpdTitle)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("RPD_Title");
            entity.Property(e => e.RpdUpdatedBy).HasColumnName("RPD_UpdatedBy");
            entity.Property(e => e.RpdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RPD_UpdatedOn");
            entity.Property(e => e.RpdYearId).HasColumnName("RPD_YearID");
        });

        modelBuilder.Entity<SadColorMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Color_Master");

            entity.Property(e => e.TcAccessCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TC_AccessCode");
            entity.Property(e => e.TcColorHex)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TC_Color_HEX");
            entity.Property(e => e.TcColorName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TC_Color_Name");
            entity.Property(e => e.TcCompId).HasColumnName("TC_CompID");
            entity.Property(e => e.TcId).HasColumnName("TC_ID");
            entity.Property(e => e.TcKeyCode)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("TC_KeyCode");
            entity.Property(e => e.TcStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TC_Status");
        });

        modelBuilder.Entity<SadCompanyMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Company_Master");

            entity.Property(e => e.CmAccessCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CM_AccessCode");
            entity.Property(e => e.CmCompanyName)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CM_CompanyName");
            entity.Property(e => e.CmCreatedBy).HasColumnName("CM_CreatedBy");
            entity.Property(e => e.CmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CM_CreatedOn");
            entity.Property(e => e.CmDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CM_DelFlag");
            entity.Property(e => e.CmId).HasColumnName("CM_ID");
        });

        modelBuilder.Entity<SadComplianceDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Compliance_Details");

            entity.Property(e => e.CompAadhaarAuthen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Comp_AadhaarAuthen");
            entity.Property(e => e.CompAccountdetails).HasColumnName("Comp_Accountdetails");
            entity.Property(e => e.CompCompId).HasColumnName("Comp_CompID");
            entity.Property(e => e.CompCrby).HasColumnName("Comp_CRBY");
            entity.Property(e => e.CompCron)
                .HasColumnType("datetime")
                .HasColumnName("Comp_CRON");
            entity.Property(e => e.CompCustId).HasColumnName("Comp_CustID");
            entity.Property(e => e.CompDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Comp_DelFlag");
            entity.Property(e => e.CompEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Comp_Email");
            entity.Property(e => e.CompFrequency).HasColumnName("Comp_Frequency");
            entity.Property(e => e.CompGstin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Comp_GSTIN");
            entity.Property(e => e.CompId).HasColumnName("Comp_Id");
            entity.Property(e => e.CompIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Comp_IPAddress");
            entity.Property(e => e.CompLoginName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Comp_LoginName");
            entity.Property(e => e.CompMobileNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Comp_MobileNo");
            entity.Property(e => e.CompPassword)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Comp_Password");
            entity.Property(e => e.CompRemarks)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Comp_Remarks");
            entity.Property(e => e.CompStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Comp_STATUS");
            entity.Property(e => e.CompTask).HasColumnName("Comp_Task");
            entity.Property(e => e.CompUpdatedBy).HasColumnName("Comp_UpdatedBy");
            entity.Property(e => e.CompUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Comp_UpdatedOn");
        });

        modelBuilder.Entity<SadConfigSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Config_Settings");

            entity.Property(e => e.SadCompId).HasColumnName("SAD_CompID");
            entity.Property(e => e.SadConfigId).HasColumnName("SAD_Config_ID");
            entity.Property(e => e.SadConfigIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SAD_Config_IPAddress");
            entity.Property(e => e.SadConfigKey)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SAD_Config_Key");
            entity.Property(e => e.SadConfigOperation)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAD_Config_Operation");
            entity.Property(e => e.SadConfigValue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("SAD_Config_Value");
            entity.Property(e => e.SadUpdatedBy).HasColumnName("SAD_UpdatedBy");
            entity.Property(e => e.SadUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SAD_UpdatedOn");
        });

        modelBuilder.Entity<SadConfigSettingsLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Config_Settings_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NsadConfigKey)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nsad_Config_Key");
            entity.Property(e => e.NsadConfigValue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nsad_Config_Value");
            entity.Property(e => e.SadCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SAD_CompID");
            entity.Property(e => e.SadConfigId).HasColumnName("sad_config_ID");
            entity.Property(e => e.SadConfigIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("sad_Config_IPAddress");
            entity.Property(e => e.SadConfigKey)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("sad_Config_Key");
            entity.Property(e => e.SadConfigValue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("sad_Config_Value");
            entity.Property(e => e.SadRunDate)
                .HasColumnType("datetime")
                .HasColumnName("SAD_RunDate");
        });

        modelBuilder.Entity<SadCurrencyMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Currency_Master");

            entity.Property(e => e.CurCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CUR_CODE");
            entity.Property(e => e.CurCompId).HasColumnName("CUR_CompID");
            entity.Property(e => e.CurCountryName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CUR_CountryName");
            entity.Property(e => e.CurId).HasColumnName("CUR_ID");
            entity.Property(e => e.CurStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CUR_Status");
        });

        modelBuilder.Entity<SadCustAccountingTemplate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUST_Accounting_Template");

            entity.Property(e => e.CustAttchId).HasColumnName("Cust_AttchID");
            entity.Property(e => e.CustCompid).HasColumnName("Cust_Compid");
            entity.Property(e => e.CustCrBy).HasColumnName("Cust_CrBy");
            entity.Property(e => e.CustCrOn)
                .HasColumnType("datetime")
                .HasColumnName("Cust_CrOn");
            entity.Property(e => e.CustDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Cust_Delflag");
            entity.Property(e => e.CustDesc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Cust_Desc");
            entity.Property(e => e.CustId).HasColumnName("Cust_ID");
            entity.Property(e => e.CustIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Cust_IPAddress");
            entity.Property(e => e.CustLocationId).HasColumnName("Cust_LocationId");
            entity.Property(e => e.CustPkid).HasColumnName("Cust_PKID");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Cust_Status");
            entity.Property(e => e.CustUpdatedBy).HasColumnName("Cust_UpdatedBy");
            entity.Property(e => e.CustUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Cust_UpdatedOn");
            entity.Property(e => e.CustValue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Cust_Value");
        });

        modelBuilder.Entity<SadCustLocation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUST_LOCATION");

            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mas_code");
            entity.Property(e => e.MasCompId).HasColumnName("Mas_CompID");
            entity.Property(e => e.MasContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Contact_Email");
            entity.Property(e => e.MasContactLandLineNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mas_Contact_LandLineNo");
            entity.Property(e => e.MasContactMobileNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Mas_Contact_MobileNo");
            entity.Property(e => e.MasContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mas_Contact_Person");
            entity.Property(e => e.MasCrby).HasColumnName("Mas_CRBY");
            entity.Property(e => e.MasCron)
                .HasColumnType("datetime")
                .HasColumnName("Mas_CRON");
            entity.Property(e => e.MasCustId).HasColumnName("Mas_CustID");
            entity.Property(e => e.MasDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Mas_DelFlag");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Description");
            entity.Property(e => e.MasDesignation)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("mas_Designation");
            entity.Property(e => e.MasId).HasColumnName("Mas_Id");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasLocAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Mas_Loc_Address");
            entity.Property(e => e.MasStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mas_STATUS");
            entity.Property(e => e.MasUpdatedBy).HasColumnName("Mas_UpdatedBy");
            entity.Property(e => e.MasUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_UpdatedOn");
        });

        modelBuilder.Entity<SadCustLoe>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUST_LOE");

            entity.Property(e => e.LoeApprovedby).HasColumnName("LOE_APPROVEDBY");
            entity.Property(e => e.LoeApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("LOE_APPROVEDON");
            entity.Property(e => e.LoeCompId).HasColumnName("LOE_CompID");
            entity.Property(e => e.LoeCrBy).HasColumnName("LOE_CrBy");
            entity.Property(e => e.LoeCrOn)
                .HasColumnType("datetime")
                .HasColumnName("LOE_CrOn");
            entity.Property(e => e.LoeCustomerId).HasColumnName("LOE_CustomerId");
            entity.Property(e => e.LoeDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("LOE_Delflag");
            entity.Property(e => e.LoeFrequency).HasColumnName("LOE_Frequency");
            entity.Property(e => e.LoeFunctionId).HasColumnName("LOE_FunctionId");
            entity.Property(e => e.LoeId).HasColumnName("LOE_Id");
            entity.Property(e => e.LoeIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("LOE_IPAddress");
            entity.Property(e => e.LoeLocationIds)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LOE_LocationIds");
            entity.Property(e => e.LoeMilestones)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOE_Milestones");
            entity.Property(e => e.LoeName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("LOE_Name");
            entity.Property(e => e.LoeNatureOfService)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("LOE_NatureOfService");
            entity.Property(e => e.LoeOtherFees).HasColumnName("LOE_OtherFees");
            entity.Property(e => e.LoeProfessionalFees).HasColumnName("LOE_ProfessionalFees");
            entity.Property(e => e.LoeRembFilingFee).HasColumnName("LOE_RembFilingFee");
            entity.Property(e => e.LoeReportDueDate)
                .HasColumnType("datetime")
                .HasColumnName("LOE_ReportDueDate");
            entity.Property(e => e.LoeServiceTax).HasColumnName("LOE_ServiceTax");
            entity.Property(e => e.LoeServiceTypeId).HasColumnName("LOE_ServiceTypeId");
            entity.Property(e => e.LoeStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("LOE_STATUS");
            entity.Property(e => e.LoeSubFunctionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOE_SubFunctionId");
            entity.Property(e => e.LoeTimeSchedule)
                .HasColumnType("datetime")
                .HasColumnName("LOE_TimeSchedule");
            entity.Property(e => e.LoeTotal).HasColumnName("LOE_Total");
            entity.Property(e => e.LoeUpdatedBy).HasColumnName("LOE_UpdatedBy");
            entity.Property(e => e.LoeUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("LOE_UpdatedOn");
            entity.Property(e => e.LoeYearId).HasColumnName("LOE_YearId");
        });

        modelBuilder.Entity<SadCustomerDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUSTOMER_DETAILS");

            entity.Property(e => e.CdetAuditincharge)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_AUDITINCHARGE");
            entity.Property(e => e.CdetCompId).HasColumnName("CDET_CompID");
            entity.Property(e => e.CdetCrby).HasColumnName("CDET_CRBY");
            entity.Property(e => e.CdetCron)
                .HasColumnType("datetime")
                .HasColumnName("CDET_CRON");
            entity.Property(e => e.CdetCustid).HasColumnName("CDET_CUSTID");
            entity.Property(e => e.CdetEmployeestrength)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_EMPLOYEESTRENGTH");
            entity.Property(e => e.CdetFileNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CDET_FileNo");
            entity.Property(e => e.CdetForeigncollaborations)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_FOREIGNCOLLABORATIONS");
            entity.Property(e => e.CdetGatheredbyauditfirm)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_GATHEREDBYAUDITFIRM");
            entity.Property(e => e.CdetGovtperception)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_GOVTPERCEPTION");
            entity.Property(e => e.CdetId).HasColumnName("CDET_ID");
            entity.Property(e => e.CdetIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CDET_IPAddress");
            entity.Property(e => e.CdetLegaladvisors)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_LEGALADVISORS");
            entity.Property(e => e.CdetLitigationissues)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_LITIGATIONISSUES");
            entity.Property(e => e.CdetManagementpeople)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CDET_Managementpeople");
            entity.Property(e => e.CdetProductsmanufactured)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_PRODUCTSMANUFACTURED");
            entity.Property(e => e.CdetProfessionalservices)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_PROFESSIONALSERVICES");
            entity.Property(e => e.CdetProfitability)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_PROFITABILITY");
            entity.Property(e => e.CdetPublicperception)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_PUBLICPERCEPTION");
            entity.Property(e => e.CdetServicesoffered)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_SERVICESOFFERED");
            entity.Property(e => e.CdetStandinginindustry)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_STANDINGININDUSTRY");
            entity.Property(e => e.CdetStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CDET_STATUS");
            entity.Property(e => e.CdetTurnover)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CDET_TURNOVER");
            entity.Property(e => e.CdetUpdatedBy).HasColumnName("CDET_UpdatedBy");
            entity.Property(e => e.CdetUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CDET_UpdatedOn");
        });

        modelBuilder.Entity<SadCustomerLegalTransfer>(entity =>
        {
            entity.HasKey(e => e.CltId).HasName("PK__Sad_Cust__AA325E6CC002655E");

            entity.ToTable("Sad_Customer_Legal_Transfer");

            entity.Property(e => e.CltId).HasColumnName("CLT_Id");
            entity.Property(e => e.CltAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CLT_Address");
            entity.Property(e => e.CltCompId).HasColumnName("CLT_CompID");
            entity.Property(e => e.CltCrBy).HasColumnName("CLT_CrBy");
            entity.Property(e => e.CltCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CLT_CreatedOn");
            entity.Property(e => e.CltCustomerId).HasColumnName("CLT_CustomerID");
            entity.Property(e => e.CltEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CLT_Email");
            entity.Property(e => e.CltName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("CLT_Name");
            entity.Property(e => e.CltStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CLT_Status");
            entity.Property(e => e.CltType)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CLT_Type");
        });

        modelBuilder.Entity<SadCustomerMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUSTOMER_MASTER");

            entity.Property(e => e.CustAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_ADDRESS");
            entity.Property(e => e.CustAmountType).HasColumnName("CUST_Amount_Type");
            entity.Property(e => e.CustApprovedby).HasColumnName("CUST_APPROVEDBY");
            entity.Property(e => e.CustApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CUST_APPROVEDON");
            entity.Property(e => e.CustAttachId).HasColumnName("CUST_AttachId");
            entity.Property(e => e.CustBoardofdirectors)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CUST_BOARDOFDIRECTORS");
            entity.Property(e => e.CustBranchId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUSt_BranchId");
            entity.Property(e => e.CustCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CITY");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_CODE");
            entity.Property(e => e.CustCommAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_ADDRESS");
            entity.Property(e => e.CustCommCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_CITY");
            entity.Property(e => e.CustCommCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_COUNTRY");
            entity.Property(e => e.CustCommEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_Email");
            entity.Property(e => e.CustCommFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_FAX");
            entity.Property(e => e.CustCommPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_PIN");
            entity.Property(e => e.CustCommState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_STATE");
            entity.Property(e => e.CustCommTel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_TEL");
            entity.Property(e => e.CustCommitmentDate)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CommitmentDate");
            entity.Property(e => e.CustCompId).HasColumnName("CUST_CompID");
            entity.Property(e => e.CustConEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_ConEmailID");
            entity.Property(e => e.CustCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COUNTRY");
            entity.Property(e => e.CustCrby).HasColumnName("CUST_CRBY");
            entity.Property(e => e.CustCron)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CRON");
            entity.Property(e => e.CustCtr)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CTR");
            entity.Property(e => e.CustDeletedBy).HasColumnName("CUST_DeletedBy");
            entity.Property(e => e.CustDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_DeletedOn");
            entity.Property(e => e.CustDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CUST_DELFLG");
            entity.Property(e => e.CustDepmethod).HasColumnName("CUST_DEPMETHOD");
            entity.Property(e => e.CustDeptId).HasColumnName("Cust_DeptID");
            entity.Property(e => e.CustDurtnId).HasColumnName("Cust_DurtnId");
            entity.Property(e => e.CustEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_EMAIL");
            entity.Property(e => e.CustFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_FAX");
            entity.Property(e => e.CustFontstyleid).HasColumnName("CUST_fontstyleid");
            entity.Property(e => e.CustFy).HasColumnName("Cust_FY");
            entity.Property(e => e.CustGroupindividual).HasColumnName("CUST_GROUPINDIVIDUAL");
            entity.Property(e => e.CustGroupname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_GROUPNAME");
            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
            entity.Property(e => e.CustIndtypeid).HasColumnName("CUST_INDTYPEID");
            entity.Property(e => e.CustIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_IPAddress");
            entity.Property(e => e.CustLocationid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_LOCATIONID");
            entity.Property(e => e.CustMgmttypeid).HasColumnName("CUST_MGMTTYPEID");
            entity.Property(e => e.CustName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CUST_NAME");
            entity.Property(e => e.CustOrgid).HasColumnName("CUST_ORGID");
            entity.Property(e => e.CustOrgtypeid).HasColumnName("CUST_ORGTYPEID");
            entity.Property(e => e.CustPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_PIN");
            entity.Property(e => e.CustRecallBy).HasColumnName("CUST_RecallBy");
            entity.Property(e => e.CustRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_RecallOn");
            entity.Property(e => e.CustRoundOff).HasColumnName("CUST_RoundOff");
            entity.Property(e => e.CustRptBorder).HasColumnName("CUST_rptBorder");
            entity.Property(e => e.CustState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_STATE");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_STATUS");
            entity.Property(e => e.CustTasks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TASKS");
            entity.Property(e => e.CustTelphone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TELPHONE");
            entity.Property(e => e.CustTrn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_TRN");
            entity.Property(e => e.CustUpdatedBy).HasColumnName("CUST_UpdatedBy");
            entity.Property(e => e.CustUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_UpdatedOn");
            entity.Property(e => e.CustWebsite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_WEBSITE");
        });

        modelBuilder.Entity<SadCustomerMaster1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUSTOMER_MASTER1");

            entity.Property(e => e.CustAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_ADDRESS");
            entity.Property(e => e.CustApprovedby).HasColumnName("CUST_APPROVEDBY");
            entity.Property(e => e.CustApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CUST_APPROVEDON");
            entity.Property(e => e.CustBoardofdirectors)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CUST_BOARDOFDIRECTORS");
            entity.Property(e => e.CustBranchId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUSt_BranchId");
            entity.Property(e => e.CustCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CITY");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_CODE");
            entity.Property(e => e.CustCommAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_ADDRESS");
            entity.Property(e => e.CustCommCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_CITY");
            entity.Property(e => e.CustCommCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_COUNTRY");
            entity.Property(e => e.CustCommEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_Email");
            entity.Property(e => e.CustCommFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_FAX");
            entity.Property(e => e.CustCommPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_PIN");
            entity.Property(e => e.CustCommState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_STATE");
            entity.Property(e => e.CustCommTel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_TEL");
            entity.Property(e => e.CustCommitmentDate)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CommitmentDate");
            entity.Property(e => e.CustCompId).HasColumnName("CUST_CompID");
            entity.Property(e => e.CustConEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_ConEmailID");
            entity.Property(e => e.CustCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COUNTRY");
            entity.Property(e => e.CustCrby).HasColumnName("CUST_CRBY");
            entity.Property(e => e.CustCron)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CRON");
            entity.Property(e => e.CustDeletedBy).HasColumnName("CUST_DeletedBy");
            entity.Property(e => e.CustDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_DeletedOn");
            entity.Property(e => e.CustDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CUST_DELFLG");
            entity.Property(e => e.CustEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_EMAIL");
            entity.Property(e => e.CustFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_FAX");
            entity.Property(e => e.CustGroupindividual).HasColumnName("CUST_GROUPINDIVIDUAL");
            entity.Property(e => e.CustGroupname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_GROUPNAME");
            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
            entity.Property(e => e.CustIndtypeid).HasColumnName("CUST_INDTYPEID");
            entity.Property(e => e.CustIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_IPAddress");
            entity.Property(e => e.CustLocationid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_LOCATIONID");
            entity.Property(e => e.CustMgmttypeid).HasColumnName("CUST_MGMTTYPEID");
            entity.Property(e => e.CustName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CUST_NAME");
            entity.Property(e => e.CustOrgid).HasColumnName("CUST_ORGID");
            entity.Property(e => e.CustOrgtypeid).HasColumnName("CUST_ORGTYPEID");
            entity.Property(e => e.CustPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_PIN");
            entity.Property(e => e.CustRecallBy).HasColumnName("CUST_RecallBy");
            entity.Property(e => e.CustRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_RecallOn");
            entity.Property(e => e.CustState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_STATE");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_STATUS");
            entity.Property(e => e.CustTasks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TASKS");
            entity.Property(e => e.CustTelphone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TELPHONE");
            entity.Property(e => e.CustUpdatedBy).HasColumnName("CUST_UpdatedBy");
            entity.Property(e => e.CustUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_UpdatedOn");
            entity.Property(e => e.CustWebsite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_WEBSITE");
        });

        modelBuilder.Entity<SadCustomerMaster2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUSTOMER_MASTER2");

            entity.Property(e => e.CustAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_ADDRESS");
            entity.Property(e => e.CustApprovedby).HasColumnName("CUST_APPROVEDBY");
            entity.Property(e => e.CustApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CUST_APPROVEDON");
            entity.Property(e => e.CustBoardofdirectors)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CUST_BOARDOFDIRECTORS");
            entity.Property(e => e.CustBranchId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUSt_BranchId");
            entity.Property(e => e.CustCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CITY");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_CODE");
            entity.Property(e => e.CustCommAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_ADDRESS");
            entity.Property(e => e.CustCommCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_CITY");
            entity.Property(e => e.CustCommCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_COUNTRY");
            entity.Property(e => e.CustCommEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_Email");
            entity.Property(e => e.CustCommFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_FAX");
            entity.Property(e => e.CustCommPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_PIN");
            entity.Property(e => e.CustCommState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_STATE");
            entity.Property(e => e.CustCommTel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_TEL");
            entity.Property(e => e.CustCommitmentDate)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CommitmentDate");
            entity.Property(e => e.CustCompId).HasColumnName("CUST_CompID");
            entity.Property(e => e.CustConEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_ConEmailID");
            entity.Property(e => e.CustCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COUNTRY");
            entity.Property(e => e.CustCrby).HasColumnName("CUST_CRBY");
            entity.Property(e => e.CustCron)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CRON");
            entity.Property(e => e.CustDeletedBy).HasColumnName("CUST_DeletedBy");
            entity.Property(e => e.CustDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_DeletedOn");
            entity.Property(e => e.CustDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CUST_DELFLG");
            entity.Property(e => e.CustEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_EMAIL");
            entity.Property(e => e.CustFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_FAX");
            entity.Property(e => e.CustGroupindividual).HasColumnName("CUST_GROUPINDIVIDUAL");
            entity.Property(e => e.CustGroupname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_GROUPNAME");
            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
            entity.Property(e => e.CustIndtypeid).HasColumnName("CUST_INDTYPEID");
            entity.Property(e => e.CustIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_IPAddress");
            entity.Property(e => e.CustLocationid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_LOCATIONID");
            entity.Property(e => e.CustMgmttypeid).HasColumnName("CUST_MGMTTYPEID");
            entity.Property(e => e.CustName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CUST_NAME");
            entity.Property(e => e.CustOrgid).HasColumnName("CUST_ORGID");
            entity.Property(e => e.CustOrgtypeid).HasColumnName("CUST_ORGTYPEID");
            entity.Property(e => e.CustPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_PIN");
            entity.Property(e => e.CustRecallBy).HasColumnName("CUST_RecallBy");
            entity.Property(e => e.CustRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_RecallOn");
            entity.Property(e => e.CustState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_STATE");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_STATUS");
            entity.Property(e => e.CustTasks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TASKS");
            entity.Property(e => e.CustTelphone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TELPHONE");
            entity.Property(e => e.CustUpdatedBy).HasColumnName("CUST_UpdatedBy");
            entity.Property(e => e.CustUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_UpdatedOn");
            entity.Property(e => e.CustWebsite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_WEBSITE");
        });

        modelBuilder.Entity<SadCustomerMaster23092024>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_CUSTOMER_MASTER23092024");

            entity.Property(e => e.CustAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_ADDRESS");
            entity.Property(e => e.CustAmountType).HasColumnName("CUST_Amount_Type");
            entity.Property(e => e.CustApprovedby).HasColumnName("CUST_APPROVEDBY");
            entity.Property(e => e.CustApprovedon)
                .HasColumnType("datetime")
                .HasColumnName("CUST_APPROVEDON");
            entity.Property(e => e.CustAttachId).HasColumnName("CUST_AttachId");
            entity.Property(e => e.CustBoardofdirectors)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CUST_BOARDOFDIRECTORS");
            entity.Property(e => e.CustBranchId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUSt_BranchId");
            entity.Property(e => e.CustCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CITY");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_CODE");
            entity.Property(e => e.CustCommAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_ADDRESS");
            entity.Property(e => e.CustCommCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_CITY");
            entity.Property(e => e.CustCommCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_COUNTRY");
            entity.Property(e => e.CustCommEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_Email");
            entity.Property(e => e.CustCommFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_FAX");
            entity.Property(e => e.CustCommPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_PIN");
            entity.Property(e => e.CustCommState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_STATE");
            entity.Property(e => e.CustCommTel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_COMM_TEL");
            entity.Property(e => e.CustCommitmentDate)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CommitmentDate");
            entity.Property(e => e.CustCompId).HasColumnName("CUST_CompID");
            entity.Property(e => e.CustConEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_ConEmailID");
            entity.Property(e => e.CustCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_COUNTRY");
            entity.Property(e => e.CustCrby).HasColumnName("CUST_CRBY");
            entity.Property(e => e.CustCron)
                .HasColumnType("datetime")
                .HasColumnName("CUST_CRON");
            entity.Property(e => e.CustCtr)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_CTR");
            entity.Property(e => e.CustDeletedBy).HasColumnName("CUST_DeletedBy");
            entity.Property(e => e.CustDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_DeletedOn");
            entity.Property(e => e.CustDelflg)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CUST_DELFLG");
            entity.Property(e => e.CustDepmethod).HasColumnName("CUST_DEPMETHOD");
            entity.Property(e => e.CustDeptId).HasColumnName("Cust_DeptID");
            entity.Property(e => e.CustDurtnId).HasColumnName("Cust_DurtnId");
            entity.Property(e => e.CustEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_EMAIL");
            entity.Property(e => e.CustFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_FAX");
            entity.Property(e => e.CustFontstyleid).HasColumnName("CUST_fontstyleid");
            entity.Property(e => e.CustFy).HasColumnName("Cust_FY");
            entity.Property(e => e.CustGroupindividual).HasColumnName("CUST_GROUPINDIVIDUAL");
            entity.Property(e => e.CustGroupname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_GROUPNAME");
            entity.Property(e => e.CustId).HasColumnName("CUST_ID");
            entity.Property(e => e.CustIndtypeid).HasColumnName("CUST_INDTYPEID");
            entity.Property(e => e.CustIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_IPAddress");
            entity.Property(e => e.CustLocationid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_LOCATIONID");
            entity.Property(e => e.CustMgmttypeid).HasColumnName("CUST_MGMTTYPEID");
            entity.Property(e => e.CustName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CUST_NAME");
            entity.Property(e => e.CustOrgid).HasColumnName("CUST_ORGID");
            entity.Property(e => e.CustOrgtypeid).HasColumnName("CUST_ORGTYPEID");
            entity.Property(e => e.CustPin)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CUST_PIN");
            entity.Property(e => e.CustRecallBy).HasColumnName("CUST_RecallBy");
            entity.Property(e => e.CustRecallOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_RecallOn");
            entity.Property(e => e.CustRoundOff).HasColumnName("CUST_RoundOff");
            entity.Property(e => e.CustRptBorder).HasColumnName("CUST_rptBorder");
            entity.Property(e => e.CustState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_STATE");
            entity.Property(e => e.CustStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("CUST_STATUS");
            entity.Property(e => e.CustTasks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TASKS");
            entity.Property(e => e.CustTelphone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_TELPHONE");
            entity.Property(e => e.CustTrn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CUST_TRN");
            entity.Property(e => e.CustUpdatedBy).HasColumnName("CUST_UpdatedBy");
            entity.Property(e => e.CustUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CUST_UpdatedOn");
            entity.Property(e => e.CustWebsite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CUST_WEBSITE");
        });

        modelBuilder.Entity<SadEmpCategoryCharge>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_EmpCategory_Charges");

            entity.Property(e => e.EmpcAppBy).HasColumnName("EMPC_AppBy");
            entity.Property(e => e.EmpcAppOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_AppOn");
            entity.Property(e => e.EmpcCappBy).HasColumnName("EMPC_CAppBy");
            entity.Property(e => e.EmpcCappOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CAppOn");
            entity.Property(e => e.EmpcCatId).HasColumnName("EMPC_CAT_ID");
            entity.Property(e => e.EmpcCcreatedBy).HasColumnName("EMPC_CCreatedBy");
            entity.Property(e => e.EmpcCcreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CCreatedOn");
            entity.Property(e => e.EmpcCdelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("EMPC_CDelFlag");
            entity.Property(e => e.EmpcCdeletedBy).HasColumnName("EMPC_CDeletedBy");
            entity.Property(e => e.EmpcCdeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CDeletedOn");
            entity.Property(e => e.EmpcCharges)
                .HasColumnType("money")
                .HasColumnName("EMPC_CHARGES");
            entity.Property(e => e.EmpcCompId).HasColumnName("EMPC_CompID");
            entity.Property(e => e.EmpcCreatedBy).HasColumnName("EMPC_CreatedBy");
            entity.Property(e => e.EmpcCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CreatedOn");
            entity.Property(e => e.EmpcCrecalledBy).HasColumnName("EMPC_CRecalledBy");
            entity.Property(e => e.EmpcCrecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CRecalledOn");
            entity.Property(e => e.EmpcCremarks)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMPC_CRemarks");
            entity.Property(e => e.EmpcCstatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPC_CStatus");
            entity.Property(e => e.EmpcCupdatedBy).HasColumnName("EMPC_CUpdatedBy");
            entity.Property(e => e.EmpcCupdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_CUpdatedOn");
            entity.Property(e => e.EmpcDays).HasColumnName("EMPC_DAYS");
            entity.Property(e => e.EmpcDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("EMPC_DelFlag");
            entity.Property(e => e.EmpcDeletedBy).HasColumnName("EMPC_DeletedBy");
            entity.Property(e => e.EmpcDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_DeletedOn");
            entity.Property(e => e.EmpcHours).HasColumnName("EMPC_HOURS");
            entity.Property(e => e.EmpcId).HasColumnName("EMPC_ID");
            entity.Property(e => e.EmpcIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("EMPC_IPAddress");
            entity.Property(e => e.EmpcKmcharges)
                .HasColumnType("money")
                .HasColumnName("EMPC_KMCharges");
            entity.Property(e => e.EmpcRecalledBy).HasColumnName("EMPC_RecalledBy");
            entity.Property(e => e.EmpcRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_RecalledOn");
            entity.Property(e => e.EmpcRemarks)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMPC_Remarks");
            entity.Property(e => e.EmpcStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMPC_Status");
            entity.Property(e => e.EmpcUpdatedBy).HasColumnName("EMPC_UpdatedBy");
            entity.Property(e => e.EmpcUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("EMPC_UpdatedOn");
            entity.Property(e => e.EmpcYearId).HasColumnName("EMPC_YearID");
        });

        modelBuilder.Entity<SadFinalisationReportContent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Finalisation_Report_Contents");

            entity.Property(e => e.FptAppBy).HasColumnName("FPT_AppBy");
            entity.Property(e => e.FptAppOn)
                .HasColumnType("datetime")
                .HasColumnName("FPT_AppOn");
            entity.Property(e => e.FptCompId).HasColumnName("FPT_CompID");
            entity.Property(e => e.FptCrBy).HasColumnName("FPT_CrBy");
            entity.Property(e => e.FptCrOn)
                .HasColumnType("datetime")
                .HasColumnName("FPT_CrOn");
            entity.Property(e => e.FptDeletedBy).HasColumnName("FPT_DeletedBy");
            entity.Property(e => e.FptDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("FPT_DeletedOn");
            entity.Property(e => e.FptDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("FPT_Delflag");
            entity.Property(e => e.FptDetails)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("FPT_Details");
            entity.Property(e => e.FptFunctionId).HasColumnName("FPT_FunctionId");
            entity.Property(e => e.FptFunctionName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FPT_FunctionName");
            entity.Property(e => e.FptId).HasColumnName("FPT_Id");
            entity.Property(e => e.FptIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("FPT_IPAddress");
            entity.Property(e => e.FptStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("FPT_Status");
            entity.Property(e => e.FptTitle)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FPT_Title");
            entity.Property(e => e.FptUpdatedBy).HasColumnName("FPT_UpdatedBy");
            entity.Property(e => e.FptUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FPT_UpdatedOn");
            entity.Property(e => e.FptYearid).HasColumnName("FPT_Yearid");
        });

        modelBuilder.Entity<SadFinalisationReportTemplate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Finalisation_Report_Template");

            entity.Property(e => e.TemAppBy).HasColumnName("TEM_AppBy");
            entity.Property(e => e.TemAppOn)
                .HasColumnType("datetime")
                .HasColumnName("TEM_AppOn");
            entity.Property(e => e.TemCompid).HasColumnName("TEM_Compid");
            entity.Property(e => e.TemContentId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_ContentId");
            entity.Property(e => e.TemCrBy).HasColumnName("TEM_CrBy");
            entity.Property(e => e.TemCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TEM_CrOn");
            entity.Property(e => e.TemDeletedBy).HasColumnName("TEM_DeletedBy");
            entity.Property(e => e.TemDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("TEM_DeletedOn");
            entity.Property(e => e.TemDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_Delflag");
            entity.Property(e => e.TemFunctionId).HasColumnName("TEM_FunctionId");
            entity.Property(e => e.TemId).HasColumnName("TEM_Id");
            entity.Property(e => e.TemIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_IPAddress");
            entity.Property(e => e.TemModule)
                .HasMaxLength(500)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_Module");
            entity.Property(e => e.TemReportTitle).HasColumnName("TEM_ReportTitle");
            entity.Property(e => e.TemSortOrder)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_SortOrder");
            entity.Property(e => e.TemStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("TEM_Status");
            entity.Property(e => e.TemUpdatedBy).HasColumnName("TEM_UpdatedBy");
            entity.Property(e => e.TemUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TEM_UpdatedOn");
            entity.Property(e => e.TemYearid).HasColumnName("TEM_Yearid");
        });

        modelBuilder.Entity<SadGeneralBranchDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_General_BranchDetails");

            entity.Property(e => e.BranchAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Branch_Address");
            entity.Property(e => e.BranchCity)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Branch_City");
            entity.Property(e => e.BranchContactEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Branch_ContactEmailID");
            entity.Property(e => e.BranchContactPerson)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Branch_ContactPerson");
            entity.Property(e => e.BranchCountry)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Branch_Country");
            entity.Property(e => e.BranchCrBy).HasColumnName("Branch_CrBy");
            entity.Property(e => e.BranchCrOn)
                .HasColumnType("datetime")
                .HasColumnName("Branch_CrOn");
            entity.Property(e => e.BranchEmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Branch_EmailID");
            entity.Property(e => e.BranchEstablishmentDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Branch_Establishment_Date");
            entity.Property(e => e.BranchId).HasColumnName("Branch_ID");
            entity.Property(e => e.BranchMobileNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Branch_MobileNo");
            entity.Property(e => e.BranchName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Branch_Name");
            entity.Property(e => e.BranchPinCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Branch_PinCode");
            entity.Property(e => e.BranchState)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Branch_State");
            entity.Property(e => e.BranchStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Branch_Status");
            entity.Property(e => e.BranchTelephoneNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Branch_TelephoneNo");
        });

        modelBuilder.Entity<SadGrpDesgnGeneralMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_GrpDesgn_general_master_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mas_Code");
            entity.Property(e => e.MasCompid)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Mas_Compid");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Description");
            entity.Property(e => e.MasId).HasColumnName("Mas_Id");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Notes");
            entity.Property(e => e.NMasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nMas_Code");
            entity.Property(e => e.NMasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nMas_Description");
            entity.Property(e => e.NMasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nMas_Notes");
        });

        modelBuilder.Entity<SadGrpOrLvlGeneralMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_GrpOrLvl_General_Master");

            entity.Property(e => e.MasApprovedBy).HasColumnName("Mas_ApprovedBy");
            entity.Property(e => e.MasApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_ApprovedOn");
            entity.Property(e => e.MasClassify).HasColumnName("Mas_Classify");
            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mas_Code");
            entity.Property(e => e.MasCompId).HasColumnName("Mas_CompID");
            entity.Property(e => e.MasCreatedby).HasColumnName("Mas_Createdby");
            entity.Property(e => e.MasCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("Mas_Createdon");
            entity.Property(e => e.MasDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mas_DelFlag");
            entity.Property(e => e.MasDeletedBy).HasColumnName("Mas_DeletedBy");
            entity.Property(e => e.MasDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_DeletedOn");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Description");
            entity.Property(e => e.MasId).HasColumnName("Mas_ID");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Notes");
            entity.Property(e => e.MasRecalledBy).HasColumnName("Mas_RecalledBy");
            entity.Property(e => e.MasRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_RecalledOn");
            entity.Property(e => e.MasStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mas_Status");
            entity.Property(e => e.MasUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_UpdatedOn");
            entity.Property(e => e.MasUpdatedby).HasColumnName("Mas_Updatedby");
        });

        modelBuilder.Entity<SadGrpOrLvlGeneralMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_GrpOrLvl_General_Master_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mas_Code");
            entity.Property(e => e.MasCompid)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Mas_Compid");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Description");
            entity.Property(e => e.MasId).HasColumnName("Mas_Id");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Notes");
            entity.Property(e => e.NMasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nMas_Code");
            entity.Property(e => e.NMasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nMas_Description");
            entity.Property(e => e.NMasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nMas_Notes");
        });

        modelBuilder.Entity<SadGrpdesgnGeneralMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_GRPDESGN_General_Master");

            entity.Property(e => e.MasApprovedBy).HasColumnName("Mas_ApprovedBy");
            entity.Property(e => e.MasApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_ApprovedOn");
            entity.Property(e => e.MasClassify).HasColumnName("Mas_Classify");
            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mas_Code");
            entity.Property(e => e.MasCompId).HasColumnName("Mas_CompID");
            entity.Property(e => e.MasCreatedby).HasColumnName("Mas_Createdby");
            entity.Property(e => e.MasCreatedon)
                .HasColumnType("datetime")
                .HasColumnName("Mas_Createdon");
            entity.Property(e => e.MasDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mas_DelFlag");
            entity.Property(e => e.MasDeletedBy).HasColumnName("Mas_DeletedBy");
            entity.Property(e => e.MasDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_DeletedOn");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Description");
            entity.Property(e => e.MasId).HasColumnName("Mas_ID");
            entity.Property(e => e.MasIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Mas_IPAddress");
            entity.Property(e => e.MasNotes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Mas_Notes");
            entity.Property(e => e.MasRecalledBy).HasColumnName("Mas_RecalledBy");
            entity.Property(e => e.MasRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_RecalledOn");
            entity.Property(e => e.MasStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mas_Status");
            entity.Property(e => e.MasUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Mas_UpdatedOn");
            entity.Property(e => e.MasUpdatedby).HasColumnName("Mas_Updatedby");
        });

        modelBuilder.Entity<SadGrplvlMember>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sad_grplvl_members");

            entity.Property(e => e.GldCrBy).HasColumnName("Gld_CrBy");
            entity.Property(e => e.GldCrDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Gld_CrDate");
            entity.Property(e => e.GldFromDate)
                .HasColumnType("datetime")
                .HasColumnName("Gld_FromDate");
            entity.Property(e => e.GldGrpLvlId).HasColumnName("Gld_GrpLvlId");
            entity.Property(e => e.GldGrpLvlPosn).HasColumnName("Gld_GrpLvlPosn");
            entity.Property(e => e.GldToDate)
                .HasColumnType("datetime")
                .HasColumnName("Gld_ToDate");
            entity.Property(e => e.GldUserId).HasColumnName("Gld_UserId");
        });

        modelBuilder.Entity<SadIssueKnowledgeBaseMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_IssueKnowledgeBase_Master");

            entity.Property(e => e.IkbApprovedBy).HasColumnName("IKB_ApprovedBy");
            entity.Property(e => e.IkbApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("IKB_ApprovedOn");
            entity.Property(e => e.IkbCompId).HasColumnName("IKB_CompID");
            entity.Property(e => e.IkbCrBy).HasColumnName("IKB_CrBy");
            entity.Property(e => e.IkbCrOn)
                .HasColumnType("datetime")
                .HasColumnName("IKB_CrOn");
            entity.Property(e => e.IkbDelFlag)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("IKB_DelFlag");
            entity.Property(e => e.IkbId).HasColumnName("IKB_ID");
            entity.Property(e => e.IkbIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("IKB_IPAddress");
            entity.Property(e => e.IkbIssueDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("IKB_IssueDetails");
            entity.Property(e => e.IkbIssueHeading)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("IKB_IssueHeading");
            entity.Property(e => e.IkbIssueRatingId).HasColumnName("IKB_IssueRatingID");
            entity.Property(e => e.IkbStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("IKB_Status");
            entity.Property(e => e.IkbUpdatedBy).HasColumnName("IKB_UpdatedBy");
            entity.Property(e => e.IkbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("IKB_UpdatedOn");
        });

        modelBuilder.Entity<SadKnowledgeMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Knowledge_Master");

            entity.Property(e => e.TkbApprovedBy).HasColumnName("TKB_ApprovedBy");
            entity.Property(e => e.TkbApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("TKB_ApprovedOn");
            entity.Property(e => e.TkbCompId).HasColumnName("TKB_CompID");
            entity.Property(e => e.TkbContent)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TKB_Content");
            entity.Property(e => e.TkbCrBy).HasColumnName("TKB_CrBy");
            entity.Property(e => e.TkbCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TKB_CrOn");
            entity.Property(e => e.TkbId).HasColumnName("TKB_ID");
            entity.Property(e => e.TkbIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TKB_IPAddress");
            entity.Property(e => e.TkbStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TKB_Status");
            entity.Property(e => e.TkbSubject)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("TKB_Subject");
            entity.Property(e => e.TkbUpdatedBy).HasColumnName("TKB_UpdatedBy");
            entity.Property(e => e.TkbUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TKB_UpdatedOn");
        });

        modelBuilder.Entity<SadLevelsGeneralMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sad_Levels_General_Master");

            entity.Property(e => e.MasClassify).HasColumnName("Mas_Classify");
            entity.Property(e => e.MasCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("mas_Code");
            entity.Property(e => e.MasCompId).HasColumnName("Mas_CompID");
            entity.Property(e => e.MasDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("mas_Delflag");
            entity.Property(e => e.MasDescription)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mas_Description");
            entity.Property(e => e.MasId).HasColumnName("mas_Id");
            entity.Property(e => e.MasNotes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("mas_Notes");
            entity.Property(e => e.MasSortOrder).HasColumnName("mas_SortOrder");
            entity.Property(e => e.MasType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("mas_Type");
        });

        modelBuilder.Entity<SadMembershipDetail>(entity =>
        {
            entity.HasKey(e => e.SmdId).HasName("PK__sad_Memb__F4CE9BECD0C6FC9D");

            entity.ToTable("sad_Membership_Details");

            entity.Property(e => e.SmdId).HasColumnName("SMD_ID");
            entity.Property(e => e.SmdCompId).HasColumnName("SMD_CompID");
            entity.Property(e => e.SmdCrBy).HasColumnName("SMD_CrBy");
            entity.Property(e => e.SmdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SMD_CreatedOn");
            entity.Property(e => e.SmdDateOfReg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMD_DateOfReg");
            entity.Property(e => e.SmdEmployeeId).HasColumnName("SMD_EmployeeID");
            entity.Property(e => e.SmdMasterMembershipId).HasColumnName("SMD_MasterMembershipID");
            entity.Property(e => e.SmdMembershipNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMD_MembershipNo");
            entity.Property(e => e.SmdRegistrationNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMD_RegistrationNo");
            entity.Property(e => e.SmdRenewalDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SMD_RenewalDate");
            entity.Property(e => e.SmdStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SMD_Status");
        });

        modelBuilder.Entity<SadModOperation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Mod_Operations");

            entity.Property(e => e.OpCompId).HasColumnName("OP_CompID");
            entity.Property(e => e.OpModuleId).HasColumnName("OP_ModuleID");
            entity.Property(e => e.OpOperationCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OP_OperationCode");
            entity.Property(e => e.OpOperationName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OP_OperationName");
            entity.Property(e => e.OpPkid).HasColumnName("OP_PKID");
            entity.Property(e => e.OpStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("OP_Status");
        });

        modelBuilder.Entity<SadModule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Module");

            entity.Property(e => e.ModButtons)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("Mod_Buttons");
            entity.Property(e => e.ModCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mod_Code");
            entity.Property(e => e.ModCompId).HasColumnName("Mod_CompID");
            entity.Property(e => e.ModDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Mod_Delflag");
            entity.Property(e => e.ModDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("Mod_Description");
            entity.Property(e => e.ModId).HasColumnName("Mod_ID");
            entity.Property(e => e.ModNavFunc)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Mod_NavFunc");
            entity.Property(e => e.ModNotes)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("Mod_Notes");
            entity.Property(e => e.ModParent).HasColumnName("Mod_Parent");
        });

        modelBuilder.Entity<SadOrgStructure>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sad_org_structure");

            entity.Property(e => e.OrgAppBy).HasColumnName("org_AppBy");
            entity.Property(e => e.OrgAppOn)
                .HasColumnType("datetime")
                .HasColumnName("org_AppOn");
            entity.Property(e => e.OrgAppStrength).HasColumnName("org_AppStrength");
            entity.Property(e => e.OrgBranchCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Org_BranchCode");
            entity.Property(e => e.OrgCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("org_Code");
            entity.Property(e => e.OrgCompId).HasColumnName("Org_CompID");
            entity.Property(e => e.OrgCreatedBy).HasColumnName("org_CreatedBy");
            entity.Property(e => e.OrgCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("org_CreatedOn");
            entity.Property(e => e.OrgCust)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("org_cust");
            entity.Property(e => e.OrgCust1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Org_Cust1");
            entity.Property(e => e.OrgDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("org_DelFlag");
            entity.Property(e => e.OrgDeletedBy).HasColumnName("Org_DeletedBy");
            entity.Property(e => e.OrgDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("Org_DeletedOn");
            entity.Property(e => e.OrgIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Org_IPAddress");
            entity.Property(e => e.OrgLevelCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Org_levelCode");
            entity.Property(e => e.OrgName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("org_name");
            entity.Property(e => e.OrgNode).HasColumnName("org_node");
            entity.Property(e => e.OrgNote)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("org_Note");
            entity.Property(e => e.OrgParent).HasColumnName("org_parent");
            entity.Property(e => e.OrgRecalledBy).HasColumnName("Org_RecalledBy");
            entity.Property(e => e.OrgRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("org_RecalledOn");
            entity.Property(e => e.OrgSalesUnitCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Org_SalesUnitCode");
            entity.Property(e => e.OrgStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("org_Status");
            entity.Property(e => e.OrgType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("org_Type");
            entity.Property(e => e.OrgUpdatedBy).HasColumnName("Org_UpdatedBy");
            entity.Property(e => e.OrgUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Org_UpdatedOn");
            entity.Property(e => e.OrgUserid).HasColumnName("org_userid");
        });

        modelBuilder.Entity<SadOrgStructureLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_Org_Structure_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NOrgBranchCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nOrg_BranchCode");
            entity.Property(e => e.NOrgLevelCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nOrg_levelCode");
            entity.Property(e => e.NOrgSalesUnitCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nOrg_SalesUnitCode");
            entity.Property(e => e.NorgAppStrength).HasColumnName("norg_AppStrength");
            entity.Property(e => e.NorgCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("norg_Code");
            entity.Property(e => e.NorgCust)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("norg_cust");
            entity.Property(e => e.NorgDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("norg_DelFlag");
            entity.Property(e => e.NorgEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("norg_Email");
            entity.Property(e => e.NorgName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("norg_name");
            entity.Property(e => e.NorgNote)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("norg_Note");
            entity.Property(e => e.NorgParent).HasColumnName("norg_parent");
            entity.Property(e => e.NorgStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("norg_Status");
            entity.Property(e => e.NorgType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("norg_Type");
            entity.Property(e => e.NorgUserid).HasColumnName("norg_userid");
            entity.Property(e => e.OrgAppStrength).HasColumnName("org_AppStrength");
            entity.Property(e => e.OrgBranchCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Org_BranchCode");
            entity.Property(e => e.OrgCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("org_Code");
            entity.Property(e => e.OrgCompId)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("Org_CompId");
            entity.Property(e => e.OrgCust)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("org_cust");
            entity.Property(e => e.OrgDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("org_DelFlag");
            entity.Property(e => e.OrgEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("org_Email");
            entity.Property(e => e.OrgIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Org_IPAddress");
            entity.Property(e => e.OrgLevelCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Org_levelCode");
            entity.Property(e => e.OrgName)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("org_name");
            entity.Property(e => e.OrgNode).HasColumnName("org_node");
            entity.Property(e => e.OrgNote)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("org_Note");
            entity.Property(e => e.OrgParent).HasColumnName("org_parent");
            entity.Property(e => e.OrgSalesUnitCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Org_SalesUnitCode");
            entity.Property(e => e.OrgStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("org_Status");
            entity.Property(e => e.OrgType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("org_Type");
            entity.Property(e => e.OrgUserid).HasColumnName("org_userid");
        });

        modelBuilder.Entity<SadReportContentMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_ReportContentMaster");

            entity.Property(e => e.RcmAppBy).HasColumnName("RCM_AppBy");
            entity.Property(e => e.RcmAppOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_AppOn");
            entity.Property(e => e.RcmCompId).HasColumnName("RCM_CompID");
            entity.Property(e => e.RcmCrBy).HasColumnName("RCM_CrBy");
            entity.Property(e => e.RcmCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_CrOn");
            entity.Property(e => e.RcmDeletedBy).HasColumnName("RCM_DeletedBy");
            entity.Property(e => e.RcmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_DeletedOn");
            entity.Property(e => e.RcmDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("RCM_Delflag");
            entity.Property(e => e.RcmDescription)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("RCM_Description");
            entity.Property(e => e.RcmHeading)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("RCM_Heading");
            entity.Property(e => e.RcmId).HasColumnName("RCM_Id");
            entity.Property(e => e.RcmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("RCM_IPAddress");
            entity.Property(e => e.RcmReportId).HasColumnName("RCM_ReportId");
            entity.Property(e => e.RcmReportName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("RCM_ReportName");
            entity.Property(e => e.RcmStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .UseCollation("Latin1_General_CI_AI")
                .HasColumnName("RCM_Status");
            entity.Property(e => e.RcmUpdatedBy).HasColumnName("RCM_UpdatedBy");
            entity.Property(e => e.RcmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RCM_UpdatedOn");
            entity.Property(e => e.RcmYearid).HasColumnName("RCM_Yearid");
        });

        modelBuilder.Entity<SadReportGeneration>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_ReportGeneration");

            entity.Property(e => e.RgAuditId).HasColumnName("RG_AuditId");
            entity.Property(e => e.RgCompid).HasColumnName("RG_Compid");
            entity.Property(e => e.RgCrBy).HasColumnName("RG_CrBy");
            entity.Property(e => e.RgCrOn)
                .HasColumnType("datetime")
                .HasColumnName("RG_CrOn");
            entity.Property(e => e.RgCustomerId).HasColumnName("RG_CustomerId");
            entity.Property(e => e.RgDescription)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("RG_Description");
            entity.Property(e => e.RgFinancialYear).HasColumnName("RG_FinancialYear");
            entity.Property(e => e.RgHeading).HasColumnName("RG_Heading");
            entity.Property(e => e.RgId).HasColumnName("RG_Id");
            entity.Property(e => e.RgIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("RG_IPAddress");
            entity.Property(e => e.RgModule).HasColumnName("RG_Module");
            entity.Property(e => e.RgPartner).HasColumnName("RG_Partner");
            entity.Property(e => e.RgReport).HasColumnName("RG_Report");
            entity.Property(e => e.RgReportType).HasColumnName("RG_ReportType");
            entity.Property(e => e.RgSignedby).HasColumnName("RG_Signedby");
            entity.Property(e => e.RgUdin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RG_UDIN");
            entity.Property(e => e.RgUdindate)
                .HasColumnType("datetime")
                .HasColumnName("RG_UDINdate");
            entity.Property(e => e.RgUpdatedBy).HasColumnName("RG_UpdatedBy");
            entity.Property(e => e.RgUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RG_UpdatedOn");
            entity.Property(e => e.RgYearId).HasColumnName("RG_YearId");
        });

        modelBuilder.Entity<SadReportTypeMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_ReportTypeMaster");

            entity.Property(e => e.RtmApprovedBy).HasColumnName("RTM_ApprovedBy");
            entity.Property(e => e.RtmApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("RTM_ApprovedOn");
            entity.Property(e => e.RtmAudrptType).HasColumnName("RTM_AudrptType");
            entity.Property(e => e.RtmCommunicationId).HasColumnName("RTM_CommunicationId");
            entity.Property(e => e.RtmCompId).HasColumnName("RTM_CompID");
            entity.Property(e => e.RtmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RTM_CreatedOn");
            entity.Property(e => e.RtmCreatedby).HasColumnName("RTM_Createdby");
            entity.Property(e => e.RtmDelFlag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("RTM_DelFlag");
            entity.Property(e => e.RtmId).HasColumnName("RTM_Id");
            entity.Property(e => e.RtmIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RTM_IPAddress");
            entity.Property(e => e.RtmReportTypeName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("RTM_ReportTypeName");
            entity.Property(e => e.RtmStatus)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("RTM_Status");
            entity.Property(e => e.RtmTemplateId).HasColumnName("RTM_TemplateId");
            entity.Property(e => e.RtmUpdatedBy).HasColumnName("RTM_UpdatedBy");
            entity.Property(e => e.RtmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("RTM_UpdatedOn");
        });

        modelBuilder.Entity<SadStatutoryDirectorDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Statutory_DirectorDetails");

            entity.Property(e => e.SsdCompId).HasColumnName("SSD_CompID");
            entity.Property(e => e.SsdCrby).HasColumnName("SSD_CRBY");
            entity.Property(e => e.SsdCron)
                .HasColumnType("datetime")
                .HasColumnName("SSD_CRON");
            entity.Property(e => e.SsdCustId).HasColumnName("SSD_CustID");
            entity.Property(e => e.SsdDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SSD_DelFlag");
            entity.Property(e => e.SsdDin)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSD_DIN");
            entity.Property(e => e.SsdDirectorName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SSD_DirectorName");
            entity.Property(e => e.SsdDob)
                .HasColumnType("datetime")
                .HasColumnName("SSD_DOB");
            entity.Property(e => e.SsdEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SSD_Email");
            entity.Property(e => e.SsdId).HasColumnName("SSD_Id");
            entity.Property(e => e.SsdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSD_IPAddress");
            entity.Property(e => e.SsdMobileNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SSD_MobileNo");
            entity.Property(e => e.SsdRemarks)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SSD_Remarks");
            entity.Property(e => e.SsdStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SSD_STATUS");
            entity.Property(e => e.SsdUpdatedBy).HasColumnName("SSD_UpdatedBy");
            entity.Property(e => e.SsdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SSD_UpdatedOn");
        });

        modelBuilder.Entity<SadStatutoryPartnerDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_Statutory_PartnerDetails");

            entity.Property(e => e.SspAttachId).HasColumnName("SSP_AttachId");
            entity.Property(e => e.SspCapitalAmount)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("SSP_CapitalAmount");
            entity.Property(e => e.SspCompId).HasColumnName("SSP_CompID");
            entity.Property(e => e.SspCrby).HasColumnName("SSP_CRBY");
            entity.Property(e => e.SspCron)
                .HasColumnType("datetime")
                .HasColumnName("SSP_CRON");
            entity.Property(e => e.SspCustId).HasColumnName("SSP_CustID");
            entity.Property(e => e.SspDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SSP_DelFlag");
            entity.Property(e => e.SspDoj)
                .HasColumnType("datetime")
                .HasColumnName("SSP_DOJ");
            entity.Property(e => e.SspId).HasColumnName("SSP_Id");
            entity.Property(e => e.SspIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSP_IPAddress");
            entity.Property(e => e.SspPan)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSP_PAN");
            entity.Property(e => e.SspPartnerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SSP_PartnerName");
            entity.Property(e => e.SspShareOfProfit)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("SSP_ShareOfProfit");
            entity.Property(e => e.SspStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SSP_STATUS");
            entity.Property(e => e.SspUpdatedBy).HasColumnName("SSP_UpdatedBy");
            entity.Property(e => e.SspUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SSP_UpdatedOn");
        });

        modelBuilder.Entity<SadSupplierMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_SUPPLIER_MASTER");

            entity.Property(e => e.SupAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_Address");
            entity.Property(e => e.SupCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_Code");
            entity.Property(e => e.SupCompId).HasColumnName("SUP_CompID");
            entity.Property(e => e.SupContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_ContactPerson");
            entity.Property(e => e.SupCrby).HasColumnName("SUP_CRBY");
            entity.Property(e => e.SupCron)
                .HasColumnType("datetime")
                .HasColumnName("SUP_CRON");
            entity.Property(e => e.SupEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_Email");
            entity.Property(e => e.SupFax)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_Fax");
            entity.Property(e => e.SupId).HasColumnName("SUP_ID");
            entity.Property(e => e.SupIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SUP_IPAddress");
            entity.Property(e => e.SupName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("SUP_Name");
            entity.Property(e => e.SupPhoneNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_PhoneNo");
            entity.Property(e => e.SupStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SUP_STATUS");
            entity.Property(e => e.SupWebsite)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_Website");
        });

        modelBuilder.Entity<SadUserDetail>(entity =>
        {
            entity.HasKey(m => m.UsrId);
            entity.ToTable("Sad_UserDetails");

            entity.Property(e => e.UsrAccadateReg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_ACCADateReg");
            entity.Property(e => e.UsrAccamembershipNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_ACCAMembershipNo");
            entity.Property(e => e.UsrAccarenewalDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_ACCARenewalDate");
            entity.Property(e => e.UsrAns)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("usr_Ans");
            entity.Property(e => e.UsrAppBy).HasColumnName("usr_AppBy");
            entity.Property(e => e.UsrAppOn)
                .HasColumnType("datetime")
                .HasColumnName("usr_AppOn");
            entity.Property(e => e.UsrAttachId).HasColumnName("usr_AttachId");
            entity.Property(e => e.UsrAuditModule).HasColumnName("Usr_AuditModule");
            entity.Property(e => e.UsrAuditRole).HasColumnName("Usr_AuditRole");
            entity.Property(e => e.UsrBcmmodule).HasColumnName("Usr_BCMModule");
            entity.Property(e => e.UsrBcmrole).HasColumnName("Usr_BCMRole");
            entity.Property(e => e.UsrBloodGroup)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("usr_BloodGroup");
            entity.Property(e => e.UsrBrowser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_Browser");
            entity.Property(e => e.UsrCadateReg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_CADateReg");
            entity.Property(e => e.UsrCarenewalDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_CARenewalDate");
            entity.Property(e => e.UsrCategory).HasColumnName("usr_Category");
            entity.Property(e => e.UsrCaukdateReg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_CAUKDateReg");
            entity.Property(e => e.UsrCaukmembershipNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_CAUKMembershipNo");
            entity.Property(e => e.UsrCaukrenewalDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_CAUKRenewalDate");
            entity.Property(e => e.UsrCmadateReg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CMADateReg");
            entity.Property(e => e.UsrCmaregNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CMARegNo");
            entity.Property(e => e.UsrCmarenewalDate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CMARenewalDate");
            entity.Property(e => e.UsrCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("usr_Code");
            entity.Property(e => e.UsrCompId).HasColumnName("Usr_CompId");
            entity.Property(e => e.UsrCompanyId).HasColumnName("usr_CompanyId");
            entity.Property(e => e.UsrComplianceModule).HasColumnName("Usr_ComplianceModule");
            entity.Property(e => e.UsrComplianceRole).HasColumnName("Usr_ComplianceRole");
            entity.Property(e => e.UsrCpadateReg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CPADateReg");
            entity.Property(e => e.UsrCpamembershipNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CPAMembershipNo");
            entity.Property(e => e.UsrCparenewalDate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_CPARenewalDate");
            entity.Property(e => e.UsrCreatedBy).HasColumnName("usr_CreatedBy");
            entity.Property(e => e.UsrCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("usr_CreatedOn");
            entity.Property(e => e.UsrCurWrkAddId).HasColumnName("usr_CurWrkAddId");
            entity.Property(e => e.UsrCustDesignation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Usr_CustDesignation");
            entity.Property(e => e.UsrDelFlag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_DelFlag");
            entity.Property(e => e.UsrDeptid).HasColumnName("usr_deptid");
            entity.Property(e => e.UsrDesignation).HasColumnName("usr_Designation");
            entity.Property(e => e.UsrDigitalOfficeModule).HasColumnName("Usr_DigitalOfficeModule");
            entity.Property(e => e.UsrDigitalOfficeRole).HasColumnName("Usr_DigitalOfficeRole");
            entity.Property(e => e.UsrDisableBy).HasColumnName("Usr_DisableBy");
            entity.Property(e => e.UsrDisableOn)
                .HasColumnType("datetime")
                .HasColumnName("Usr_DisableOn");
            entity.Property(e => e.UsrDob)
                .HasColumnType("datetime")
                .HasColumnName("usr_DOB");
            entity.Property(e => e.UsrDoj)
                .HasColumnType("datetime")
                .HasColumnName("usr_DOJ");
            entity.Property(e => e.UsrDutyStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("usr_DutyStatus");
            entity.Property(e => e.UsrEductionDetail)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Usr_EductionDetail");
            entity.Property(e => e.UsrEmail)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("usr_Email");
            entity.Property(e => e.UsrEnableBy).HasColumnName("usr_EnableBy");
            entity.Property(e => e.UsrEnableOn)
                .HasColumnType("datetime")
                .HasColumnName("usr_EnableOn");
            entity.Property(e => e.UsrExpDate)
                .HasColumnType("datetime")
                .HasColumnName("usr_ExpDate");
            entity.Property(e => e.UsrExperience)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Usr_Experience");
            entity.Property(e => e.UsrFullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_FullName");
            entity.Property(e => e.UsrGender).HasColumnName("usr_Gender");
            entity.Property(e => e.UsrGrpOrUserLvlPerm).HasColumnName("Usr_GrpOrUserLvlPerm");
            entity.Property(e => e.UsrHusOrFathName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_HusOrFathName");
            entity.Property(e => e.UsrIcwadateReg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_ICWADateReg");
            entity.Property(e => e.UsrIcwaregNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_ICWARegNo");
            entity.Property(e => e.UsrIcwarenewalDate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_ICWARenewalDate");
            entity.Property(e => e.UsrId).HasColumnName("usr_Id");
            entity.Property(e => e.UsrIpaddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Usr_IPAddress");
            entity.Property(e => e.UsrIsLogin)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Usr_IsLogin");
            entity.Property(e => e.UsrIsPasswordReset).HasColumnName("Usr_IsPasswordReset");
            entity.Property(e => e.UsrIsSuperuser).HasColumnName("usr_IsSuperuser");
            entity.Property(e => e.UsrKinAddId).HasColumnName("usr_KinAddId");
            entity.Property(e => e.UsrKinName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_KinName");
            entity.Property(e => e.UsrLastLoginDate)
                .HasColumnType("datetime")
                .HasColumnName("USR_LastLoginDate");
            entity.Property(e => e.UsrLastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_LastName");
            entity.Property(e => e.UsrLevelGrp).HasColumnName("usr_LevelGrp");
            entity.Property(e => e.UsrLevelcode).HasColumnName("USR_Levelcode");
            entity.Property(e => e.UsrLoginName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_LoginName");
            entity.Property(e => e.UsrMaritalStatus).HasColumnName("usr_MaritalStatus");
            entity.Property(e => e.UsrMasterModule).HasColumnName("Usr_MasterModule");
            entity.Property(e => e.UsrMasterRole).HasColumnName("Usr_MasterRole");
            entity.Property(e => e.UsrMemberType).HasColumnName("USR_MemberType");
            entity.Property(e => e.UsrMiddleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_MiddleName");
            entity.Property(e => e.UsrMobileNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_MobileNo");
            entity.Property(e => e.UsrNoOfChildren).HasColumnName("usr_NoOfChildren");
            entity.Property(e => e.UsrNoOfLogin).HasColumnName("USR_NoOfLogin");
            entity.Property(e => e.UsrNoOfUnSucsfAtteptts).HasColumnName("usr_NoOfUnSucsfAtteptts");
            entity.Property(e => e.UsrNode).HasColumnName("usr_Node");
            entity.Property(e => e.UsrOffPhExtn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_OffPhExtn");
            entity.Property(e => e.UsrOfficePhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_OfficePhone");
            entity.Property(e => e.UsrOfficialAddId).HasColumnName("usr_OfficialAddId");
            entity.Property(e => e.UsrOrgnId).HasColumnName("usr_OrgnId");
            entity.Property(e => e.UsrOtherCertificate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherCertificate");
            entity.Property(e => e.UsrOtherCertificateDateReg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherCertificateDateReg");
            entity.Property(e => e.UsrOtherCertificateRenewalDate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherCertificateRenewalDate");
            entity.Property(e => e.UsrOtherDateReg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherDateReg");
            entity.Property(e => e.UsrOtherRegNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherRegNo");
            entity.Property(e => e.UsrOtherRenewalDate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_OtherRenewalDate");
            entity.Property(e => e.UsrOthersQualification)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("usr_othersQualification");
            entity.Property(e => e.UsrPartner).HasColumnName("usr_partner");
            entity.Property(e => e.UsrPassWord)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("usr_PassWord");
            entity.Property(e => e.UsrPcaob)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usr_PCAOB");
            entity.Property(e => e.UsrPcaobdateReg)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_PCAOBDateReg");
            entity.Property(e => e.UsrPcaobregNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_PCAOBRegNo");
            entity.Property(e => e.UsrPcaobrenewalDate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usr_PCAOBRenewalDate");
            entity.Property(e => e.UsrPermAddId).HasColumnName("usr_PermAddId");
            entity.Property(e => e.UsrPhoneNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_PhoneNo");
            entity.Property(e => e.UsrPhoto).HasColumnName("usr_Photo");
            entity.Property(e => e.UsrQualification)
                .IsUnicode(false)
                .HasColumnName("Usr_Qualification");
            entity.Property(e => e.UsrQue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("usr_Que");
            entity.Property(e => e.UsrReasonPwdBlock)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("usr_ReasonPwd_Block");
            entity.Property(e => e.UsrRefStaffNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_RefStaffNo");
            entity.Property(e => e.UsrResAddId).HasColumnName("usr_ResAddId");
            entity.Property(e => e.UsrResume).HasColumnName("usr_Resume");
            entity.Property(e => e.UsrRiskModule).HasColumnName("Usr_RiskModule");
            entity.Property(e => e.UsrRiskRole).HasColumnName("Usr_RiskRole");
            entity.Property(e => e.UsrRole).HasColumnName("Usr_Role");
            entity.Property(e => e.UsrSignature).HasColumnName("usr_Signature");
            entity.Property(e => e.UsrSkillSet)
                .IsUnicode(false)
                .HasColumnName("Usr_SkillSet");
            entity.Property(e => e.UsrStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("usr_Status");
            entity.Property(e => e.UsrSuggetions).HasColumnName("Usr_Suggetions");
            entity.Property(e => e.UsrType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("usr_Type");
            entity.Property(e => e.UsrUnBlockLockBy).HasColumnName("usr_UnBlockLockBy");
            entity.Property(e => e.UsrUnBlockLockOn)
                .HasColumnType("datetime")
                .HasColumnName("usr_UnBlockLockOn");
            entity.Property(e => e.UsrUpdatedBy).HasColumnName("Usr_UpdatedBy");
            entity.Property(e => e.UsrUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Usr_UpdatedOn");
            entity.Property(e => e.UsrUserTypeId).HasColumnName("usr_UserTypeId");
        });

        modelBuilder.Entity<SadUserEmpAcademicProgress>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserEMP_AcademicProgress");

            entity.Property(e => e.SuapAttachId).HasColumnName("SUAP_AttachID");
            entity.Property(e => e.SuapCompId).HasColumnName("SUAP_CompId");
            entity.Property(e => e.SuapCrBy).HasColumnName("SUAP_CrBy");
            entity.Property(e => e.SuapCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUAP_CrOn");
            entity.Property(e => e.SuapExamTakenOn)
                .HasColumnType("datetime")
                .HasColumnName("SUAP_ExamTakenOn");
            entity.Property(e => e.SuapGroups)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUAP_Groups");
            entity.Property(e => e.SuapIpaddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUAP_IPAddress");
            entity.Property(e => e.SuapLeaveGranted).HasColumnName("SUAP_LeaveGranted");
            entity.Property(e => e.SuapMonthofExam).HasColumnName("SUAP_MonthofExam");
            entity.Property(e => e.SuapPkid).HasColumnName("SUAP_PKID");
            entity.Property(e => e.SuapRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUAP_Remarks");
            entity.Property(e => e.SuapResult)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUAP_Result");
            entity.Property(e => e.SuapUpdatedBy).HasColumnName("SUAP_UpdatedBy");
            entity.Property(e => e.SuapUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUAP_UpdatedOn");
            entity.Property(e => e.SuapUserEmpId).HasColumnName("SUAP_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpAddress>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserEMP_Address");

            entity.Property(e => e.SuaAddress1)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_Address1");
            entity.Property(e => e.SuaAddress2)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_Address2");
            entity.Property(e => e.SuaAddress3)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_Address3");
            entity.Property(e => e.SuaCompId).HasColumnName("SUA_CompId");
            entity.Property(e => e.SuaContactName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUA_ContactName");
            entity.Property(e => e.SuaEmail)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_Email");
            entity.Property(e => e.SuaIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUA_IPAddress");
            entity.Property(e => e.SuaMobile)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SUA_Mobile");
            entity.Property(e => e.SuaPincode).HasColumnName("SUA_Pincode");
            entity.Property(e => e.SuaPkid).HasColumnName("SUA_PKID");
            entity.Property(e => e.SuaRelationType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUA_RelationType");
            entity.Property(e => e.SuaTelephone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SUA_Telephone");
            entity.Property(e => e.SuaUserEmpId).HasColumnName("SUA_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpAssessment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_Assessment");

            entity.Property(e => e.SuaAttachId).HasColumnName("SUA_AttachID");
            entity.Property(e => e.SuaCompId).HasColumnName("SUA_CompID");
            entity.Property(e => e.SuaCrBy).HasColumnName("SUA_CrBy");
            entity.Property(e => e.SuaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUA_CrOn");
            entity.Property(e => e.SuaGradesPromotedFrom)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_GradesPromotedFrom");
            entity.Property(e => e.SuaGradesPromotedTo)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_GradesPromotedTo");
            entity.Property(e => e.SuaIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUA_IPAddress");
            entity.Property(e => e.SuaIssueDate)
                .HasColumnType("datetime")
                .HasColumnName("SUA_IssueDate");
            entity.Property(e => e.SuaPerformanceAwardPaid)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_PerformanceAwardPaid");
            entity.Property(e => e.SuaPkid).HasColumnName("SUA_PKID");
            entity.Property(e => e.SuaRating)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUA_Rating");
            entity.Property(e => e.SuaRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUA_Remarks");
            entity.Property(e => e.SuaUpdatedBy).HasColumnName("SUA_UpdatedBy");
            entity.Property(e => e.SuaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUA_UpdatedOn");
            entity.Property(e => e.SuaUserEmpId).HasColumnName("SUA_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpAssetsLoan>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserEMP_AssetsLoan");

            entity.Property(e => e.SualApproValue).HasColumnName("SUAL_ApproValue");
            entity.Property(e => e.SualAssetType)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUAL_AssetType");
            entity.Property(e => e.SualAttachId).HasColumnName("SUAL_AttachID");
            entity.Property(e => e.SualCompId).HasColumnName("SUAL_CompId");
            entity.Property(e => e.SualConditionIssue)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUAL_ConditionIssue");
            entity.Property(e => e.SualConditionReceipt)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUAL_ConditionReceipt");
            entity.Property(e => e.SualCrBy).HasColumnName("SUAL_CrBy");
            entity.Property(e => e.SualCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUAL_CrOn");
            entity.Property(e => e.SualDueDate)
                .HasColumnType("datetime")
                .HasColumnName("SUAL_DueDate");
            entity.Property(e => e.SualIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUAL_IPAddress");
            entity.Property(e => e.SualIssueDate)
                .HasColumnType("datetime")
                .HasColumnName("SUAL_IssueDate");
            entity.Property(e => e.SualPkid).HasColumnName("SUAL_PKID");
            entity.Property(e => e.SualRecievedDate)
                .HasColumnType("datetime")
                .HasColumnName("SUAL_RecievedDate");
            entity.Property(e => e.SualRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUAL_Remarks");
            entity.Property(e => e.SualSerialNo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUAL_SerialNo");
            entity.Property(e => e.SualUpdatedBy).HasColumnName("SUAL_UpdatedBy");
            entity.Property(e => e.SualUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUAL_UpdatedOn");
            entity.Property(e => e.SualUserEmpId).HasColumnName("SUAL_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_Courses");

            entity.Property(e => e.SucAttachId).HasColumnName("SUC_AttachID");
            entity.Property(e => e.SucBriefDescription)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUC_BriefDescription");
            entity.Property(e => e.SucCompId).HasColumnName("SUC_CompID");
            entity.Property(e => e.SucConductedBy)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUC_ConductedBy");
            entity.Property(e => e.SucCpepoints)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUC_CPEPoints");
            entity.Property(e => e.SucCrBy).HasColumnName("SUC_CrBy");
            entity.Property(e => e.SucCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUC_CrOn");
            entity.Property(e => e.SucDate)
                .HasColumnType("datetime")
                .HasColumnName("SUC_Date");
            entity.Property(e => e.SucFeeEmployee)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUC_FeeEmployee");
            entity.Property(e => e.SucFeeEmployer)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUC_FeeEmployer");
            entity.Property(e => e.SucFeedBack)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUC_FeedBack");
            entity.Property(e => e.SucIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUC_IPAddress");
            entity.Property(e => e.SucPapers)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUC_Papers");
            entity.Property(e => e.SucPkid).HasColumnName("SUC_PKID");
            entity.Property(e => e.SucRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUC_Remarks");
            entity.Property(e => e.SucSubject)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUC_Subject");
            entity.Property(e => e.SucUpdatedBy).HasColumnName("SUC_UpdatedBy");
            entity.Property(e => e.SucUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUC_UpdatedOn");
            entity.Property(e => e.SucUserEmpId).HasColumnName("SUC_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpMembershipDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_MembershipDetails");

            entity.Property(e => e.SumdAttachId).HasColumnName("SUMD_AttachID");
            entity.Property(e => e.SumdCmaDob)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_CMA_DOB");
            entity.Property(e => e.SumdCmaRd)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_CMA_RD");
            entity.Property(e => e.SumdCmaRegNo)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUMD_CMA_RegNo");
            entity.Property(e => e.SumdCompId).HasColumnName("SUMD_CompID");
            entity.Property(e => e.SumdCrBy).HasColumnName("SUMD_CrBy");
            entity.Property(e => e.SumdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_CrOn");
            entity.Property(e => e.SumdIcwaDob)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_ICWA_DOB");
            entity.Property(e => e.SumdIcwaRd)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_ICWA_RD");
            entity.Property(e => e.SumdIcwaRegNo)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUMD_ICWA_RegNo");
            entity.Property(e => e.SumdIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUMD_IPAddress");
            entity.Property(e => e.SumdOtherDob)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_Other_DOB");
            entity.Property(e => e.SumdOtherRd)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_Other_RD");
            entity.Property(e => e.SumdOtherRegNo)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUMD_Other_RegNo");
            entity.Property(e => e.SumdPcaobDob)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_PCAOB_DOB");
            entity.Property(e => e.SumdPcaobRd)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_PCAOB_RD");
            entity.Property(e => e.SumdPcaobRegNo)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUMD_PCAOB_RegNo");
            entity.Property(e => e.SumdPkid).HasColumnName("SUMD_PKID");
            entity.Property(e => e.SumdUpdatedBy).HasColumnName("SUMD_UpdatedBy");
            entity.Property(e => e.SumdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUMD_UpdatedOn");
            entity.Property(e => e.SumdUserEmpMembId).HasColumnName("SUMD_UserEmpMembID");
            entity.Property(e => e.SumdYear).HasColumnName("SUMD_Year");
        });

        modelBuilder.Entity<SadUserEmpParticularsofArticle>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_ParticularsofArticles");

            entity.Property(e => e.SupArticlesFrom)
                .HasColumnType("datetime")
                .HasColumnName("SUP_ArticlesFrom");
            entity.Property(e => e.SupArticlesTo)
                .HasColumnType("datetime")
                .HasColumnName("SUP_ArticlesTo");
            entity.Property(e => e.SupAttachId).HasColumnName("SUP_AttachID");
            entity.Property(e => e.SupCompId).HasColumnName("SUP_CompID");
            entity.Property(e => e.SupCrBy).HasColumnName("SUP_CrBy");
            entity.Property(e => e.SupCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUP_CrOn");
            entity.Property(e => e.SupExtendedTo)
                .HasColumnType("datetime")
                .HasColumnName("SUP_ExtendedTo");
            entity.Property(e => e.SupIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_IPAddress");
            entity.Property(e => e.SupPkid).HasColumnName("SUP_PKID");
            entity.Property(e => e.SupPracticeNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUP_PracticeNo");
            entity.Property(e => e.SupPrincipleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUP_PrincipleName");
            entity.Property(e => e.SupRegistrationNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUP_RegistrationNo");
            entity.Property(e => e.SupRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUP_Remarks");
            entity.Property(e => e.SupUpdatedBy).HasColumnName("SUP_UpdatedBy");
            entity.Property(e => e.SupUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUP_UpdatedOn");
            entity.Property(e => e.SupUserEmpId).HasColumnName("SUP_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpProfExperiance>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserEMP_ProfExperiance");

            entity.Property(e => e.SupAssignment)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUP_Assignment");
            entity.Property(e => e.SupAttachId).HasColumnName("SUP_AttachID");
            entity.Property(e => e.SupCompId).HasColumnName("SUP_CompId");
            entity.Property(e => e.SupCrBy).HasColumnName("SUP_CrBy");
            entity.Property(e => e.SupCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUP_CrOn");
            entity.Property(e => e.SupFrom).HasColumnName("SUP_From");
            entity.Property(e => e.SupIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_IPAddress");
            entity.Property(e => e.SupPkid).HasColumnName("SUP_PKID");
            entity.Property(e => e.SupPosition)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SUP_Position");
            entity.Property(e => e.SupRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUP_Remarks");
            entity.Property(e => e.SupReportingTo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUP_ReportingTo");
            entity.Property(e => e.SupSalaryPerAnnum).HasColumnName("SUP_SalaryPerAnnum");
            entity.Property(e => e.SupTo).HasColumnName("SUP_To");
            entity.Property(e => e.SupUpdatedBy).HasColumnName("SUP_UpdatedBy");
            entity.Property(e => e.SupUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUP_UpdatedOn");
            entity.Property(e => e.SupUserEmpId).HasColumnName("SUP_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpQualification>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_Qualification");

            entity.Property(e => e.SuqAttachId).HasColumnName("SUQ_AttachID");
            entity.Property(e => e.SuqCompId).HasColumnName("SUQ_CompID");
            entity.Property(e => e.SuqCrBy).HasColumnName("SUQ_CrBy");
            entity.Property(e => e.SuqCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUQ_CrOn");
            entity.Property(e => e.SuqEducation)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUQ_Education");
            entity.Property(e => e.SuqIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUQ_IPAddress");
            entity.Property(e => e.SuqMarks).HasColumnName("SUQ_Marks");
            entity.Property(e => e.SuqPkid).HasColumnName("SUQ_PKID");
            entity.Property(e => e.SuqRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUQ_Remarks");
            entity.Property(e => e.SuqSchool)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUQ_School");
            entity.Property(e => e.SuqUniversity)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUQ_University");
            entity.Property(e => e.SuqUpdatedBy).HasColumnName("SUQ_UpdatedBy");
            entity.Property(e => e.SuqUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUQ_UpdatedOn");
            entity.Property(e => e.SuqUserEmpId).HasColumnName("SUQ_UserEmpID");
            entity.Property(e => e.SuqYear).HasColumnName("SUQ_Year");
        });

        modelBuilder.Entity<SadUserEmpSpecialMention>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UserEMP_SpecialMentions");

            entity.Property(e => e.SusAttachId).HasColumnName("SUS_AttachID");
            entity.Property(e => e.SusCompId).HasColumnName("SUS_CompID");
            entity.Property(e => e.SusCrBy).HasColumnName("SUS_CrBy");
            entity.Property(e => e.SusCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUS_CrOn");
            entity.Property(e => e.SusDate)
                .HasColumnType("datetime")
                .HasColumnName("SUS_Date");
            entity.Property(e => e.SusDealtWith)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUS_DealtWith");
            entity.Property(e => e.SusIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUS_IPAddress");
            entity.Property(e => e.SusParticulars)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUS_Particulars");
            entity.Property(e => e.SusPkid).HasColumnName("SUS_PKID");
            entity.Property(e => e.SusSpecialMention)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUS_SpecialMention");
            entity.Property(e => e.SusUpdatedBy).HasColumnName("SUS_UpdatedBy");
            entity.Property(e => e.SusUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUS_UpdatedOn");
            entity.Property(e => e.SusUserEmpId).HasColumnName("SUS_UserEmpID");
        });

        modelBuilder.Entity<SadUserEmpTransferFirm>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserEMP_TransferFirm");

            entity.Property(e => e.SutfAttachId).HasColumnName("SUTF_AttachID");
            entity.Property(e => e.SutfCompId).HasColumnName("SUTF_CompId");
            entity.Property(e => e.SutfCompletionDate)
                .HasColumnType("datetime")
                .HasColumnName("SUTF_CompletionDate");
            entity.Property(e => e.SutfCrBy).HasColumnName("SUTF_CrBy");
            entity.Property(e => e.SutfCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SUTF_CrOn");
            entity.Property(e => e.SutfDateofTransfer)
                .HasColumnType("datetime")
                .HasColumnName("SUTF_DateofTransfer");
            entity.Property(e => e.SutfDurationWithNewPrinciple)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUTF_DurationWithNewPrinciple");
            entity.Property(e => e.SutfEarlierPrinciple)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUTF_EarlierPrinciple");
            entity.Property(e => e.SutfExtendedTo)
                .HasColumnType("datetime")
                .HasColumnName("SUTF_ExtendedTo");
            entity.Property(e => e.SutfIpaddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SUTF_IPAddress");
            entity.Property(e => e.SutfNewPrinciple)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SUTF_NewPrinciple");
            entity.Property(e => e.SutfPkid).HasColumnName("SUTF_PKID");
            entity.Property(e => e.SutfRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SUTF_Remarks");
            entity.Property(e => e.SutfUpdatedBy).HasColumnName("SUTF_UpdatedBy");
            entity.Property(e => e.SutfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUTF_UpdatedOn");
            entity.Property(e => e.SutfUserEmpId).HasColumnName("SUTF_UserEmpID");
        });

        modelBuilder.Entity<SadUserPasswordHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UserPassword_History");

            entity.Property(e => e.UspCompId).HasColumnName("USP_CompID");
            entity.Property(e => e.UspDate)
                .HasColumnType("datetime")
                .HasColumnName("USP_DATE");
            entity.Property(e => e.UspId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("USP_ID");
            entity.Property(e => e.UspPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USP_PASSWORD");
            entity.Property(e => e.UspUserid).HasColumnName("USP_USERID");
        });

        modelBuilder.Entity<SadUserdetailsLog>(entity =>
        {
            entity
                .HasNoKey()



                .ToTable("sad_userdetails_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NUsrCategory).HasColumnName("nUSR_Category");
            entity.Property(e => e.NUsrCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_Code");
            entity.Property(e => e.NUsrCompanyId)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("nUSR_CompanyID");
            entity.Property(e => e.NUsrDelFlag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_DelFlag");
            entity.Property(e => e.NUsrDesignation).HasColumnName("nUSR_Designation");
            entity.Property(e => e.NUsrDutyStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nUSR_DutyStatus");
            entity.Property(e => e.NUsrEmail)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nUSR_Email");
            entity.Property(e => e.NUsrFullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_FullName");
            entity.Property(e => e.NUsrGrpOrUserLvlPerm).HasColumnName("nUSR_GrpOrUserLvlPerm");
            entity.Property(e => e.NUsrLastLoginDate)
                .HasColumnType("datetime")
                .HasColumnName("nUSR_LastLoginDate");
            entity.Property(e => e.NUsrLevelGrp).HasColumnName("nUSR_LevelGrp");
            entity.Property(e => e.NUsrLoginName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_LoginName");
            entity.Property(e => e.NUsrMobileNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_MobileNo");
            entity.Property(e => e.NUsrNoOfLogin).HasColumnName("nUSR_NoOfLogin");
            entity.Property(e => e.NUsrNoOfUnSucsfAtteptts).HasColumnName("nUSR_NoOfUnSucsfAtteptts");
            entity.Property(e => e.NUsrNode).HasColumnName("nUSR_Node");
            entity.Property(e => e.NUsrOffPhExtn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_OffPhExtn");
            entity.Property(e => e.NUsrOfficePhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_OfficePhone");
            entity.Property(e => e.NUsrOrgnId).HasColumnName("nUSR_OrgnID");
            entity.Property(e => e.NUsrPassWord)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_PassWord");
            entity.Property(e => e.NUsrPhoneNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSR_PhoneNo");
            entity.Property(e => e.NUsrReasonPwdBlock)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("nUSR_ReasonPwd_Block");
            entity.Property(e => e.NUsrRole).HasColumnName("nUsr_Role");
            entity.Property(e => e.NUsrType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("nUSR_Type");
            entity.Property(e => e.NUsrUserTypeId).HasColumnName("nUSR_UserTypeID");
            entity.Property(e => e.NusrAns)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nusr_ans");
            entity.Property(e => e.NusrAuditModule).HasColumnName("nusr_AuditModule");
            entity.Property(e => e.NusrAuditRole).HasColumnName("nusr_AuditRole");
            entity.Property(e => e.NusrBcmmodule).HasColumnName("nusr_BCMModule");
            entity.Property(e => e.NusrBcmrole).HasColumnName("nusr_BCMRole");
            entity.Property(e => e.NusrComplianceModule).HasColumnName("nusr_ComplianceModule");
            entity.Property(e => e.NusrComplianceRole).HasColumnName("nusr_ComplianceRole");
            entity.Property(e => e.NusrExperience).HasColumnName("nusr_Experience");
            entity.Property(e => e.NusrMasterModule).HasColumnName("nusr_masterModule");
            entity.Property(e => e.NusrMasterRole).HasColumnName("nusr_masterRole");
            entity.Property(e => e.NusrOthersQualification)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nusr_othersQualification");
            entity.Property(e => e.NusrPartner).HasColumnName("nusr_partner");
            entity.Property(e => e.NusrQualification)
                .IsUnicode(false)
                .HasColumnName("nusr_qualification");
            entity.Property(e => e.NusrQue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nusr_Que");
            entity.Property(e => e.NusrRiskModule).HasColumnName("nusr_RiskModule");
            entity.Property(e => e.NusrRiskRole).HasColumnName("nusr_RiskRole");
            entity.Property(e => e.NusrSkillSet)
                .IsUnicode(false)
                .HasColumnName("nusr_skillSet");
            entity.Property(e => e.UsrAns)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("usr_ans");
            entity.Property(e => e.UsrAuditModule).HasColumnName("usr_AuditModule");
            entity.Property(e => e.UsrAuditRole).HasColumnName("usr_AuditRole");
            entity.Property(e => e.UsrBcmmodule).HasColumnName("usr_BCMModule");
            entity.Property(e => e.UsrBcmrole).HasColumnName("usr_BCMRole");
            entity.Property(e => e.UsrCategory).HasColumnName("USR_Category");
            entity.Property(e => e.UsrCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_Code");
            entity.Property(e => e.UsrCompId).HasColumnName("Usr_CompID");
            entity.Property(e => e.UsrCompanyId)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("USR_CompanyID");
            entity.Property(e => e.UsrComplianceModule).HasColumnName("usr_ComplianceModule");
            entity.Property(e => e.UsrComplianceRole).HasColumnName("usr_ComplianceRole");
            entity.Property(e => e.UsrDelFlag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_DelFlag");
            entity.Property(e => e.UsrDesignation).HasColumnName("USR_Designation");
            entity.Property(e => e.UsrDutyStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("USR_DutyStatus");
            entity.Property(e => e.UsrEmail)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("USR_Email");
            entity.Property(e => e.UsrExperience).HasColumnName("usr_Experience");
            entity.Property(e => e.UsrFullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_FullName");
            entity.Property(e => e.UsrGrpOrUserLvlPerm).HasColumnName("USR_GrpOrUserLvlPerm");
            entity.Property(e => e.UsrId).HasColumnName("USR_ID");
            entity.Property(e => e.UsrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Usr_IPAddress");
            entity.Property(e => e.UsrLastLoginDate)
                .HasColumnType("datetime")
                .HasColumnName("USR_LastLoginDate");
            entity.Property(e => e.UsrLevelGrp).HasColumnName("USR_LevelGrp");
            entity.Property(e => e.UsrLoginName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_LoginName");
            entity.Property(e => e.UsrMasterModule).HasColumnName("usr_masterModule");
            entity.Property(e => e.UsrMasterRole).HasColumnName("usr_masterRole");
            entity.Property(e => e.UsrMobileNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_MobileNo");
            entity.Property(e => e.UsrNoOfLogin).HasColumnName("USR_NoOfLogin");
            entity.Property(e => e.UsrNoOfUnSucsfAtteptts).HasColumnName("USR_NoOfUnSucsfAtteptts");
            entity.Property(e => e.UsrNode).HasColumnName("USR_Node");
            entity.Property(e => e.UsrOffPhExtn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_OffPhExtn");
            entity.Property(e => e.UsrOfficePhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_OfficePhone");
            entity.Property(e => e.UsrOrgnId).HasColumnName("USR_OrgnID");
            entity.Property(e => e.UsrOthersQualification)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("usr_othersQualification");
            entity.Property(e => e.UsrPartner).HasColumnName("usr_partner");
            entity.Property(e => e.UsrPassWord)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_PassWord");
            entity.Property(e => e.UsrPhoneNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USR_PhoneNo");
            entity.Property(e => e.UsrQualification)
                .IsUnicode(false)
                .HasColumnName("usr_qualification");
            entity.Property(e => e.UsrQue)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("usr_Que");
            entity.Property(e => e.UsrReasonPwdBlock)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("USR_ReasonPwd_Block");
            entity.Property(e => e.UsrRiskModule).HasColumnName("usr_RiskModule");
            entity.Property(e => e.UsrRiskRole).HasColumnName("usr_RiskRole");
            entity.Property(e => e.UsrRole).HasColumnName("Usr_Role");
            entity.Property(e => e.UsrSkillSet)
                .IsUnicode(false)
                .HasColumnName("usr_skillSet");
            entity.Property(e => e.UsrType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("USR_Type");
            entity.Property(e => e.UsrUserTypeId).HasColumnName("USR_UserTypeID");
        });

        modelBuilder.Entity<SadUsersInOtherDept>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UsersInOtherDept");

            entity.Property(e => e.SuoCompId).HasColumnName("SUO_CompID");
            entity.Property(e => e.SuoCreatedBy).HasColumnName("SUO_CreatedBy");
            entity.Property(e => e.SuoCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SUO_CreatedOn");
            entity.Property(e => e.SuoDeptId).HasColumnName("SUO_DeptID");
            entity.Property(e => e.SuoIpaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SUO_IPAddress");
            entity.Property(e => e.SuoIsDeptHead).HasColumnName("SUO_IsDeptHead");
            entity.Property(e => e.SuoPkid).HasColumnName("SUO_PKID");
            entity.Property(e => e.SuoUserId).HasColumnName("SUO_UserID");
        });

        modelBuilder.Entity<SadUsrOrGrpPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UsrOrGrp_Permission");

            entity.Property(e => e.PermCompId).HasColumnName("Perm_CompID");
            entity.Property(e => e.PermCrby).HasColumnName("Perm_Crby");
            entity.Property(e => e.PermCron)
                .HasColumnType("datetime")
                .HasColumnName("Perm_Cron");
            entity.Property(e => e.PermIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Perm_IPAddress");
            entity.Property(e => e.PermModuleId).HasColumnName("Perm_ModuleID");
            entity.Property(e => e.PermOpPkid).HasColumnName("Perm_OpPKID");
            entity.Property(e => e.PermOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Perm_Operation");
            entity.Property(e => e.PermPkid).HasColumnName("Perm_PKID");
            entity.Property(e => e.PermPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Perm_PType");
            entity.Property(e => e.PermStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Perm_Status");
            entity.Property(e => e.PermUsrOrgrpId).HasColumnName("Perm_UsrORGrpID");
        });

        modelBuilder.Entity<SadUsrOrGrpPermissionDgo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sad_UsrOrGrp_permissionDGO");

            entity.Property(e => e.SgpActiveOrDeactive).HasColumnName("SGP_ActiveOrDeactive");
            entity.Property(e => e.SgpAnnotaion).HasColumnName("SGP_Annotaion");
            entity.Property(e => e.SgpApprovedBy).HasColumnName("SGP_ApprovedBy");
            entity.Property(e => e.SgpApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("SGP_ApprovedOn");
            entity.Property(e => e.SgpCompId).HasColumnName("SGP_CompID");
            entity.Property(e => e.SgpCreatedBy).HasColumnName("SGP_CreatedBy");
            entity.Property(e => e.SgpCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SGP_CreatedOn");
            entity.Property(e => e.SgpDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SGP_DelFlag");
            entity.Property(e => e.SgpDownload).HasColumnName("SGP_Download");
            entity.Property(e => e.SgpId).HasColumnName("SGP_ID");
            entity.Property(e => e.SgpLevelGroup)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SGP_LevelGroup");
            entity.Property(e => e.SgpLevelGroupId).HasColumnName("SGP_LevelGroupID");
            entity.Property(e => e.SgpModId).HasColumnName("SGP_ModID");
            entity.Property(e => e.SgpReport).HasColumnName("SGP_Report");
            entity.Property(e => e.SgpSaveOrUpdate).HasColumnName("SGP_SaveOrUpdate");
            entity.Property(e => e.SgpStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SGP_Status");
            entity.Property(e => e.SgpUpdatedBy).HasColumnName("SGP_UpdatedBy");
            entity.Property(e => e.SgpUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SGP_UpdatedOn");
            entity.Property(e => e.SgpView).HasColumnName("SGP_View");
        });

        modelBuilder.Entity<SadUsrOrGrpPermissionLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAD_UsrOrGrp_Permission_Log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NPermModuleId).HasColumnName("nPerm_ModuleID");
            entity.Property(e => e.NPermOpPkid).HasColumnName("nPerm_OpPKID");
            entity.Property(e => e.NPermPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nPerm_PType");
            entity.Property(e => e.NPermUsrOrgrpId).HasColumnName("nPerm_UsrORGrpID");
            entity.Property(e => e.PermCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Perm_CompID");
            entity.Property(e => e.PermIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Perm_IPAddress");
            entity.Property(e => e.PermModuleId).HasColumnName("Perm_ModuleID");
            entity.Property(e => e.PermOpPkid).HasColumnName("Perm_OpPKID");
            entity.Property(e => e.PermPkid).HasColumnName("Perm_PKID");
            entity.Property(e => e.PermPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Perm_PType");
            entity.Property(e => e.PermUsrOrgrpId).HasColumnName("Perm_UsrORGrpID");
        });

        modelBuilder.Entity<SampleSelection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Sample_selection");

            entity.Property(e => e.SsAttachId).HasColumnName("SS_AttachID");
            entity.Property(e => e.SsAuditCodeId).HasColumnName("SS_AuditCodeID");
            entity.Property(e => e.SsCheckPointId).HasColumnName("SS_CheckPointID");
            entity.Property(e => e.SsCompId).HasColumnName("SS_CompID");
            entity.Property(e => e.SsPkid).HasColumnName("SS_PKID");
        });

        modelBuilder.Entity<SampleTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SampleTable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ScheduleLinkageMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Schedule_Linkage_Master");

            entity.Property(e => e.SlmCompId).HasColumnName("SLM_CompID");
            entity.Property(e => e.SlmCreatedBy).HasColumnName("SLM_CreatedBy");
            entity.Property(e => e.SlmCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SLM_CreatedOn");
            entity.Property(e => e.SlmCustId).HasColumnName("SLM_CustID");
            entity.Property(e => e.SlmDeletedBy).HasColumnName("SLM_DeletedBy");
            entity.Property(e => e.SlmDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("SLM_DeletedOn");
            entity.Property(e => e.SlmGlid).HasColumnName("SLM_GLId");
            entity.Property(e => e.SlmGlledger)
                .IsUnicode(false)
                .HasColumnName("SLM_GLLedger");
            entity.Property(e => e.SlmGroupId).HasColumnName("SLM_GroupID");
            entity.Property(e => e.SlmHead).HasColumnName("SLM_Head");
            entity.Property(e => e.SlmId).HasColumnName("SLM_ID");
            entity.Property(e => e.SlmIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SLM_IPAddress");
            entity.Property(e => e.SlmNoteNo).HasColumnName("SLM_NoteNo");
            entity.Property(e => e.SlmOperation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SLM_Operation");
            entity.Property(e => e.SlmOrgId).HasColumnName("SLM_OrgID");
            entity.Property(e => e.SlmStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SLM_Status");
            entity.Property(e => e.SlmSubGroupId).HasColumnName("SLM_SubGroupID");
            entity.Property(e => e.SlmUpdatedBy).HasColumnName("SLM_UpdatedBy");
            entity.Property(e => e.SlmUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SLM_UpdatedOn");
            entity.Property(e => e.SlmYearId).HasColumnName("SLM_YearID");
        });

        modelBuilder.Entity<ScheduleNoteDesc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ScheduleNote_Desc");

            entity.Property(e => e.SndCategory)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SND_Category");
            entity.Property(e => e.SndCompId).HasColumnName("SND_CompId");
            entity.Property(e => e.SndCrby).HasColumnName("SND_CRBY");
            entity.Property(e => e.SndCron)
                .HasColumnType("datetime")
                .HasColumnName("SND_CRON");
            entity.Property(e => e.SndCustId).HasColumnName("SND_CustId");
            entity.Property(e => e.SndDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SND_DELFLAG");
            entity.Property(e => e.SndDescription)
                .IsUnicode(false)
                .HasColumnName("SND_Description");
            entity.Property(e => e.SndId).HasColumnName("SND_ID");
            entity.Property(e => e.SndIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SND_IPAddress");
            entity.Property(e => e.SndStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SND_STATUS");
            entity.Property(e => e.SndUpdatedby).HasColumnName("SND_UPDATEDBY");
            entity.Property(e => e.SndUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SND_UPDATEDON");
            entity.Property(e => e.SndYearid).HasColumnName("SND_YEARId");
        });

        modelBuilder.Entity<ScheduleNoteFirst>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ScheduleNote_First");

            entity.Property(e => e.SnfCategory)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNF_Category");
            entity.Property(e => e.SnfCompId).HasColumnName("SNF_CompId");
            entity.Property(e => e.SnfCrby).HasColumnName("SNF_CRBY");
            entity.Property(e => e.SnfCron)
                .HasColumnType("datetime")
                .HasColumnName("SNF_CRON");
            entity.Property(e => e.SnfCustId).HasColumnName("SNF_CustId");
            entity.Property(e => e.SnfCyearAmount)
                .HasColumnType("money")
                .HasColumnName("SNF_CYear_Amount");
            entity.Property(e => e.SnfDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SNF_DELFLAG");
            entity.Property(e => e.SnfDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SNF_Description");
            entity.Property(e => e.SnfId).HasColumnName("SNF_ID");
            entity.Property(e => e.SnfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNF_IPAddress");
            entity.Property(e => e.SnfPyearAmount)
                .HasColumnType("money")
                .HasColumnName("SNF_PYear_Amount");
            entity.Property(e => e.SnfStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SNF_STATUS");
            entity.Property(e => e.SnfUpdatedby).HasColumnName("SNF_UPDATEDBY");
            entity.Property(e => e.SnfUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SNF_UPDATEDON");
            entity.Property(e => e.SnfYearid).HasColumnName("SNF_YEARId");
        });

        modelBuilder.Entity<ScheduleNoteFourth>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ScheduleNote_Fourth");

            entity.Property(e => e.SnftCategory)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNFT_Category");
            entity.Property(e => e.SnftChangedShares)
                .HasColumnType("money")
                .HasColumnName("SNFT_ChangedShares");
            entity.Property(e => e.SnftCompId).HasColumnName("SNFT_CompId");
            entity.Property(e => e.SnftCrby).HasColumnName("SNFT_CRBY");
            entity.Property(e => e.SnftCron)
                .HasColumnType("datetime")
                .HasColumnName("SNFT_CRON");
            entity.Property(e => e.SnftCustId).HasColumnName("SNFT_CustId");
            entity.Property(e => e.SnftDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SNFT_DELFLAG");
            entity.Property(e => e.SnftDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SNFT_Description");
            entity.Property(e => e.SnftId).HasColumnName("SNFT_ID");
            entity.Property(e => e.SnftIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNFT_IPAddress");
            entity.Property(e => e.SnftNumShares)
                .HasColumnType("money")
                .HasColumnName("SNFT_NumShares");
            entity.Property(e => e.SnftStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SNFT_STATUS");
            entity.Property(e => e.SnftTotalShares)
                .HasColumnType("money")
                .HasColumnName("SNFT_TotalShares");
            entity.Property(e => e.SnftUpdatedby).HasColumnName("SNFT_UPDATEDBY");
            entity.Property(e => e.SnftUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SNFT_UPDATEDON");
            entity.Property(e => e.SnftYearid).HasColumnName("SNFT_YEARId");
        });

        modelBuilder.Entity<ScheduleNoteSecond>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ScheduleNote_Second");

            entity.Property(e => e.SnsCategory)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNS_Category");
            entity.Property(e => e.SnsCompId).HasColumnName("SNS_CompId");
            entity.Property(e => e.SnsCrby).HasColumnName("SNS_CRBY");
            entity.Property(e => e.SnsCron)
                .HasColumnType("datetime")
                .HasColumnName("SNS_CRON");
            entity.Property(e => e.SnsCustId).HasColumnName("SNS_CustId");
            entity.Property(e => e.SnsCyearAddAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_AddAmount");
            entity.Property(e => e.SnsCyearAddShares)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_AddShares");
            entity.Property(e => e.SnsCyearBegAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_BegAmount");
            entity.Property(e => e.SnsCyearBegShares)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_BegShares");
            entity.Property(e => e.SnsCyearEndAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_EndAmount");
            entity.Property(e => e.SnsCyearEndShares)
                .HasColumnType("money")
                .HasColumnName("SNS_CYear_EndShares");
            entity.Property(e => e.SnsDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SNS_DELFLAG");
            entity.Property(e => e.SnsDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SNS_Description");
            entity.Property(e => e.SnsId).HasColumnName("SNS_ID");
            entity.Property(e => e.SnsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNS_IPAddress");
            entity.Property(e => e.SnsPyearAddAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_AddAmount");
            entity.Property(e => e.SnsPyearAddShares)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_AddShares");
            entity.Property(e => e.SnsPyearBegAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_BegAmount");
            entity.Property(e => e.SnsPyearBegShares)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_BegShares");
            entity.Property(e => e.SnsPyearEndAmount)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_EndAmount");
            entity.Property(e => e.SnsPyearEndShares)
                .HasColumnType("money")
                .HasColumnName("SNS_PYear_EndShares");
            entity.Property(e => e.SnsStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SNS_STATUS");
            entity.Property(e => e.SnsUpdatedby).HasColumnName("SNS_UPDATEDBY");
            entity.Property(e => e.SnsUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SNS_UPDATEDON");
            entity.Property(e => e.SnsYearid).HasColumnName("SNS_YEARId");
        });

        modelBuilder.Entity<ScheduleNoteThird>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ScheduleNote_Third");

            entity.Property(e => e.SntCategory)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNT_Category");
            entity.Property(e => e.SntCompId).HasColumnName("SNT_CompId");
            entity.Property(e => e.SntCrby).HasColumnName("SNT_CRBY");
            entity.Property(e => e.SntCron)
                .HasColumnType("datetime")
                .HasColumnName("SNT_CRON");
            entity.Property(e => e.SntCustId).HasColumnName("SNT_CustId");
            entity.Property(e => e.SntCyearAmount)
                .HasColumnType("money")
                .HasColumnName("SNT_CYear_Amount");
            entity.Property(e => e.SntCyearShares)
                .HasColumnType("money")
                .HasColumnName("SNT_CYear_Shares");
            entity.Property(e => e.SntDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("SNT_DELFLAG");
            entity.Property(e => e.SntDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SNT_Description");
            entity.Property(e => e.SntId).HasColumnName("SNT_ID");
            entity.Property(e => e.SntIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SNT_IPAddress");
            entity.Property(e => e.SntPyearAmount)
                .HasColumnType("money")
                .HasColumnName("SNT_PYear_Amount");
            entity.Property(e => e.SntPyearShares)
                .HasColumnType("money")
                .HasColumnName("SNT_PYear_Shares");
            entity.Property(e => e.SntStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("SNT_STATUS");
            entity.Property(e => e.SntUpdatedby).HasColumnName("SNT_UPDATEDBY");
            entity.Property(e => e.SntUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SNT_UPDATEDON");
            entity.Property(e => e.SntYearid).HasColumnName("SNT_YEARId");
        });

        modelBuilder.Entity<StandardAuditAuditCompletion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_Audit_Completion");

            entity.Property(e => e.SacAttachmentId).HasColumnName("SAC_AttachmentId");
            entity.Property(e => e.SacAuditId).HasColumnName("SAC_AuditID");
            entity.Property(e => e.SacCheckPointId).HasColumnName("SAC_CheckPointId");
            entity.Property(e => e.SacCompId).HasColumnName("SAC_CompID");
            entity.Property(e => e.SacCreatedBy).HasColumnName("SAC_CreatedBy");
            entity.Property(e => e.SacCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SAC_CreatedOn");
            entity.Property(e => e.SacCustId).HasColumnName("SAC_CustID");
            entity.Property(e => e.SacId).HasColumnName("SAC_ID");
            entity.Property(e => e.SacIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAC_IPAddress");
            entity.Property(e => e.SacRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SAC_Remarks");
            entity.Property(e => e.SacSubPointId).HasColumnName("SAC_SubPointId");
            entity.Property(e => e.SacUpdatedBy).HasColumnName("SAC_UpdatedBy");
            entity.Property(e => e.SacUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SAC_UpdatedOn");
            entity.Property(e => e.SacWorkPaperId).HasColumnName("SAC_WorkPaperId");
            entity.Property(e => e.SacYearId).HasColumnName("SAC_YearID");
        });

        modelBuilder.Entity<StandardAuditAuditDrllogRemarksHistory>(entity =>
        {
            entity.HasKey(e => e.CSarId).HasName("PK__Standard__DD1B2C6A080DC57A");

            entity.ToTable("StandardAudit_Audit_DRLLog_RemarksHistory");

            entity.Property(e => e.CSarId).HasColumnName("C_SAR_ID");
            entity.Property(e => e.SarAttchId).HasColumnName("SAR_AttchId");
            entity.Property(e => e.SarAtthachDocId).HasColumnName("SAR_AtthachDocId");
            entity.Property(e => e.SarCheckPointIds)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SAR_CheckPointIDs");
            entity.Property(e => e.SarClonedAuditId).HasColumnName("SAR_ClonedAuditId");
            entity.Property(e => e.SarClonedDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAR_ClonedDate");
            entity.Property(e => e.SarClonedYear).HasColumnName("SAR_ClonedYear");
            entity.Property(e => e.SarCompId).HasColumnName("SAR_CompID");
            entity.Property(e => e.SarComplStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAR_ComplStatus");
            entity.Property(e => e.SarDate)
                .HasColumnType("datetime")
                .HasColumnName("SAR_Date");
            entity.Property(e => e.SarDbflag)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAR_DBFlag");
            entity.Property(e => e.SarDelflag)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SAR_Delflag");
            entity.Property(e => e.SarDrlid).HasColumnName("SAR_DRLId");
            entity.Property(e => e.SarEmailIds)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SAR_EmailIds");
            entity.Property(e => e.SarId).HasColumnName("SAR_ID");
            entity.Property(e => e.SarIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAR_IPAddress");
            entity.Property(e => e.SarIsIssueRaised).HasColumnName("SAR_IsIssueRaised");
            entity.Property(e => e.SarMasid).HasColumnName("SAR_MASid");
            entity.Property(e => e.SarRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SAR_Remarks");
            entity.Property(e => e.SarRemarksBy).HasColumnName("SAR_RemarksBy");
            entity.Property(e => e.SarRemarksType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAR_RemarksType");
            entity.Property(e => e.SarReportType).HasColumnName("SAR_ReportType");
            entity.Property(e => e.SarSaId).HasColumnName("SAR_SA_ID");
            entity.Property(e => e.SarSacId).HasColumnName("SAR_SAC_ID");
            entity.Property(e => e.SarTimlinetoResOn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SAR_TimlinetoResOn");
            entity.Property(e => e.SarYearid).HasColumnName("sar_Yearid");
        });

        modelBuilder.Entity<StandardAuditAuditSummaryIfc>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_AuditSummary_IFC");

            entity.Property(e => e.SaifcAttachId).HasColumnName("SAIFC_AttachID");
            entity.Property(e => e.SaifcColumnCount).HasColumnName("SAIFC_ColumnCount");
            entity.Property(e => e.SaifcComments)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SAIFC_Comments");
            entity.Property(e => e.SaifcCompId).HasColumnName("SAIFC_CompID");
            entity.Property(e => e.SaifcCrBy).HasColumnName("SAIFC_CrBy");
            entity.Property(e => e.SaifcCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAIFC_CrOn");
            entity.Property(e => e.SaifcCustId).HasColumnName("SAIFC_CustID");
            entity.Property(e => e.SaifcIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAIFC_IPAddress");
            entity.Property(e => e.SaifcPkid).HasColumnName("SAIFC_PKID");
            entity.Property(e => e.SaifcReportBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SAIFC_ReportBy");
            entity.Property(e => e.SaifcReportDate)
                .HasColumnType("datetime")
                .HasColumnName("SAIFC_ReportDate");
            entity.Property(e => e.SaifcSaId).HasColumnName("SAIFC_SA_ID");
            entity.Property(e => e.SaifcYearId).HasColumnName("SAIFC_YearID");
        });

        modelBuilder.Entity<StandardAuditAuditSummaryIfcdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_AuditSummary_IFCDetails");

            entity.Property(e => e.SaifcdAttachId).HasColumnName("SAIFCD_AttachID");
            entity.Property(e => e.SaifcdColumn1)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column1");
            entity.Property(e => e.SaifcdColumn2)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column2");
            entity.Property(e => e.SaifcdColumn3)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column3");
            entity.Property(e => e.SaifcdColumn4)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column4");
            entity.Property(e => e.SaifcdColumn5)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column5");
            entity.Property(e => e.SaifcdColumn6)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_Column6");
            entity.Property(e => e.SaifcdColumnRowType).HasColumnName("SAIFCD_ColumnRowType");
            entity.Property(e => e.SaifcdConclusion).HasColumnName("SAIFCD_Conclusion");
            entity.Property(e => e.SaifcdCrBy).HasColumnName("SAIFCD_CrBy");
            entity.Property(e => e.SaifcdCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAIFCD_CrOn");
            entity.Property(e => e.SaifcdDateOfTesting)
                .HasColumnType("datetime")
                .HasColumnName("SAIFCD_DateOfTesting");
            entity.Property(e => e.SaifcdPkid).HasColumnName("SAIFCD_PKID");
            entity.Property(e => e.SaifcdSaifcPkid).HasColumnName("SAIFCD_SAIFC_PKID");
            entity.Property(e => e.SaifcdSampleSizeUsed)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_SampleSizeUsed");
            entity.Property(e => e.SaifcdTypeOfTestingDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAIFCD_TypeOfTestingDetails");
        });

        modelBuilder.Entity<StandardAuditAuditSummaryKamdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_AuditSummary_KAMDetails");

            entity.Property(e => e.SakamAttachId).HasColumnName("SAKAM_AttachID");
            entity.Property(e => e.SakamAuditProcedureUndertakenToAddressTheKam)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAKAM_AuditProcedureUndertakenToAddressTheKAM");
            entity.Property(e => e.SakamCrBy).HasColumnName("SAKAM_CrBy");
            entity.Property(e => e.SakamCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAKAM_CrOn");
            entity.Property(e => e.SakamDescriptionOrReasonForSelectionAsKam)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAKAM_DescriptionOrReasonForSelectionAsKAM");
            entity.Property(e => e.SakamSaifcdPkid).HasColumnName("SAKAM_SAIFCD_PKID");
            entity.Property(e => e.SakamdPkid).HasColumnName("SAKAMD_PKID");
        });

        modelBuilder.Entity<StandardAuditAuditSummaryMrdetail>(entity =>
        {
            entity.HasKey(e => e.CSamrPkid).HasName("PK__Standard__D9950B1DAFE62FD5");

            entity.ToTable("StandardAudit_AuditSummary_MRDetails");

            entity.Property(e => e.CSamrPkid).HasColumnName("C_SAMR_PKID");
            entity.Property(e => e.SamrAtchdocid).HasColumnName("SAMR_ATCHDOCID");
            entity.Property(e => e.SamrAttachId).HasColumnName("SAMR_AttachID");
            entity.Property(e => e.SamrCompId).HasColumnName("SAMR_CompID");
            entity.Property(e => e.SamrComplStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAMR_ComplStatus");
            entity.Property(e => e.SamrCrBy).HasColumnName("SAMR_CrBy");
            entity.Property(e => e.SamrCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAMR_CrOn");
            entity.Property(e => e.SamrCustId).HasColumnName("SAMR_CustID");
            entity.Property(e => e.SamrDbflag)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SAMR_DBFlag");
            entity.Property(e => e.SamrDueDateReceiveDocs)
                .HasColumnType("datetime")
                .HasColumnName("SAMR_DueDateReceiveDocs");
            entity.Property(e => e.SamrEmailIds)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SAMR_EmailIds");
            entity.Property(e => e.SamrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAMR_IPAddress");
            entity.Property(e => e.SamrMrid).HasColumnName("SAMR_MRID");
            entity.Property(e => e.SamrPkid).HasColumnName("SAMR_PKID");
            entity.Property(e => e.SamrRequestedByPerson)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("SAMR_RequestedByPerson");
            entity.Property(e => e.SamrRequestedDate)
                .HasColumnType("datetime")
                .HasColumnName("SAMR_RequestedDate");
            entity.Property(e => e.SamrRequestedRemarks)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("SAMR_RequestedRemarks");
            entity.Property(e => e.SamrResponsesDetails)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAMR_ResponsesDetails");
            entity.Property(e => e.SamrResponsesReceivedDate)
                .HasColumnType("datetime")
                .HasColumnName("SAMR_ResponsesReceivedDate");
            entity.Property(e => e.SamrResponsesRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SAMR_ResponsesRemarks");
            entity.Property(e => e.SamrSaPkid).HasColumnName("SAMR_SA_PKID");
            entity.Property(e => e.SamrYearId).HasColumnName("SAMR_YearID");
        });

        modelBuilder.Entity<StandardAuditChecklistDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_Checklist_Details");

            entity.Property(e => e.SacdAuditId).HasColumnName("SACD_AuditId");
            entity.Property(e => e.SacdAuditType).HasColumnName("SACD_AuditType");
            entity.Property(e => e.SacdCheckpointId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SACD_CheckpointId");
            entity.Property(e => e.SacdCompId).HasColumnName("SACD_CompId");
            entity.Property(e => e.SacdCrby).HasColumnName("SACD_CRBY");
            entity.Property(e => e.SacdCron)
                .HasColumnType("datetime")
                .HasColumnName("SACD_CRON");
            entity.Property(e => e.SacdCustId).HasColumnName("SACD_CustId");
            entity.Property(e => e.SacdEmpId).HasColumnName("SACD_EmpId");
            entity.Property(e => e.SacdEndDate)
                .HasColumnType("datetime")
                .HasColumnName("SACD_EndDate");
            entity.Property(e => e.SacdHeading)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SACD_Heading");
            entity.Property(e => e.SacdHrPrDay)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SACD_HrPrDay");
            entity.Property(e => e.SacdId).HasColumnName("SACD_ID");
            entity.Property(e => e.SacdIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SACD_IPAddress");
            entity.Property(e => e.SacdStartDate)
                .HasColumnType("datetime")
                .HasColumnName("SACD_StartDate");
            entity.Property(e => e.SacdTotalHr)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SACD_TotalHr");
            entity.Property(e => e.SacdUpdatedby).HasColumnName("SACD_UPDATEDBY");
            entity.Property(e => e.SacdUpdatedon)
                .HasColumnType("datetime")
                .HasColumnName("SACD_UPDATEDON");
            entity.Property(e => e.SacdWorkType).HasColumnName("SACD_WorkType");
        });

        modelBuilder.Entity<StandardAuditConductAuditRemarksHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ConductAudit_RemarksHistory");

            entity.Property(e => e.ScrCheckPointId).HasColumnName("SCR_CheckPointID");
            entity.Property(e => e.ScrCompId).HasColumnName("SCR_CompID");
            entity.Property(e => e.ScrComplStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SCR_ComplStatus");
            entity.Property(e => e.ScrDate)
                .HasColumnType("datetime")
                .HasColumnName("SCR_Date");
            entity.Property(e => e.ScrDbflag)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SCR_DBFlag");
            entity.Property(e => e.ScrDueDate)
                .HasColumnType("datetime")
                .HasColumnName("SCR_DueDate");
            entity.Property(e => e.ScrEmailIds)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SCR_EmailIds");
            entity.Property(e => e.ScrId).HasColumnName("SCR_ID");
            entity.Property(e => e.ScrIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SCR_IPAddress");
            entity.Property(e => e.ScrIsIssueRaised).HasColumnName("SCR_IsIssueRaised");
            entity.Property(e => e.ScrMailUniqueId)
                .IsUnicode(false)
                .HasColumnName("SCR_MailUniqueId");
            entity.Property(e => e.ScrRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SCR_Remarks");
            entity.Property(e => e.ScrRemarksBy).HasColumnName("SCR_RemarksBy");
            entity.Property(e => e.ScrRemarksType).HasColumnName("SCR_RemarksType");
            entity.Property(e => e.ScrSaId).HasColumnName("SCR_SA_ID");
            entity.Property(e => e.ScrSacId).HasColumnName("SCR_SAC_ID");
        });

        modelBuilder.Entity<StandardAuditReviewLedgerObservation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ReviewLedger_Observations");

            entity.Property(e => e.SroAeuId).HasColumnName("SRO_AEU_ID");
            entity.Property(e => e.SroAuditId).HasColumnName("SRO_AuditId");
            entity.Property(e => e.SroAuditTypeId).HasColumnName("SRO_AuditTypeId");
            entity.Property(e => e.SroCompId).HasColumnName("SRO_CompID");
            entity.Property(e => e.SroComplStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SRO_ComplStatus");
            entity.Property(e => e.SroCustId).HasColumnName("SRO_CustId");
            entity.Property(e => e.SroDate)
                .HasColumnType("datetime")
                .HasColumnName("SRO_Date");
            entity.Property(e => e.SroDbflag)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SRO_DBFlag");
            entity.Property(e => e.SroDueDate)
                .HasColumnType("datetime")
                .HasColumnName("SRO_DueDate");
            entity.Property(e => e.SroEmailIds)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SRO_EmailIds");
            entity.Property(e => e.SroFinancialYearId).HasColumnName("SRO_FinancialYearId");
            entity.Property(e => e.SroIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SRO_IPAddress");
            entity.Property(e => e.SroIsIssueRaised).HasColumnName("SRO_IsIssueRaised");
            entity.Property(e => e.SroLevel).HasColumnName("SRO_Level");
            entity.Property(e => e.SroMailUniqueId)
                .IsUnicode(false)
                .HasColumnName("SRO_MailUniqueId");
            entity.Property(e => e.SroObservationBy).HasColumnName("SRO_ObservationBy");
            entity.Property(e => e.SroObservations)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SRO_Observations");
            entity.Property(e => e.SroPkid).HasColumnName("SRO_PKID");
            entity.Property(e => e.SroStatusVarchar)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SRO_Status varchar");
        });

        modelBuilder.Entity<StandardAuditSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_Schedule");

            entity.Property(e => e.SaAdditionalSupportEmployeeId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SA_AdditionalSupportEmployeeID");
            entity.Property(e => e.SaAttachId).HasColumnName("SA_AttachID");
            entity.Property(e => e.SaAuditNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SA_AuditNo");
            entity.Property(e => e.SaAuditOpinionDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_AuditOpinionDate");
            entity.Property(e => e.SaAuditTypeId).HasColumnName("SA_AuditTypeID");
            entity.Property(e => e.SaBinderCompletedDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_BinderCompletedDate");
            entity.Property(e => e.SaCompId).HasColumnName("SA_CompID");
            entity.Property(e => e.SaCrBy).HasColumnName("SA_CrBy");
            entity.Property(e => e.SaCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SA_CrOn");
            entity.Property(e => e.SaCustId).HasColumnName("SA_CustID");
            entity.Property(e => e.SaEngagementPartnerId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SA_EngagementPartnerID");
            entity.Property(e => e.SaExpCompDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_ExpCompDate");
            entity.Property(e => e.SaFilingDatePcaob)
                .HasColumnType("datetime")
                .HasColumnName("SA_FilingDatePCAOB");
            entity.Property(e => e.SaFilingDateSec)
                .HasColumnType("datetime")
                .HasColumnName("SA_FilingDateSEC");
            entity.Property(e => e.SaId).HasColumnName("SA_ID");
            entity.Property(e => e.SaIntervalId).HasColumnName("SA_IntervalId");
            entity.Property(e => e.SaIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SA_IPAddress");
            entity.Property(e => e.SaMrldate)
                .HasColumnType("datetime")
                .HasColumnName("SA_MRLDate");
            entity.Property(e => e.SaMrsdate)
                .HasColumnType("datetime")
                .HasColumnName("SA_MRSDate");
            entity.Property(e => e.SaPartnerId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SA_PartnerID");
            entity.Property(e => e.SaReviewPartnerId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("SA_ReviewPartnerID");
            entity.Property(e => e.SaRptFilDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_RptFilDate");
            entity.Property(e => e.SaRptRvDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_RptRvDate");
            entity.Property(e => e.SaScopeOfAudit)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SA_ScopeOfAudit");
            entity.Property(e => e.SaSignedBy).HasColumnName("SA_SignedBy");
            entity.Property(e => e.SaStartDate)
                .HasColumnType("datetime")
                .HasColumnName("SA_StartDate");
            entity.Property(e => e.SaStatus).HasColumnName("SA_Status");
            entity.Property(e => e.SaUdin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SA_UDIN");
            entity.Property(e => e.SaUdindate)
                .HasColumnType("datetime")
                .HasColumnName("SA_UDINdate");
            entity.Property(e => e.SaUpdatedBy).HasColumnName("SA_UpdatedBy");
            entity.Property(e => e.SaUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SA_UpdatedOn");
            entity.Property(e => e.SaYearId).HasColumnName("SA_YearID");
        });

        modelBuilder.Entity<StandardAuditScheduleCheckPointList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ScheduleCheckPointList");

            entity.Property(e => e.SacAnnexure).HasColumnName("SAC_Annexure");
            entity.Property(e => e.SacAtchdocid).HasColumnName("SAC_ATCHDOCID");
            entity.Property(e => e.SacAttachId).HasColumnName("SAC_AttachID");
            entity.Property(e => e.SacCheckPointId).HasColumnName("SAC_CheckPointID");
            entity.Property(e => e.SacCompId).HasColumnName("SAC_CompID");
            entity.Property(e => e.SacConductedBy).HasColumnName("SAC_ConductedBy");
            entity.Property(e => e.SacCrBy).HasColumnName("SAC_CrBy");
            entity.Property(e => e.SacCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAC_CrOn");
            entity.Property(e => e.SacDrlid).HasColumnName("SAC_DRLID");
            entity.Property(e => e.SacId).HasColumnName("SAC_ID");
            entity.Property(e => e.SacIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAC_IPAddress");
            entity.Property(e => e.SacLastUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SAC_LastUpdatedOn");
            entity.Property(e => e.SacMandatory).HasColumnName("SAC_Mandatory");
            entity.Property(e => e.SacRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SAC_Remarks");
            entity.Property(e => e.SacReviewLedgerId).HasColumnName("SAC_ReviewLedgerID");
            entity.Property(e => e.SacReviewerRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SAC_ReviewerRemarks");
            entity.Property(e => e.SacSaId).HasColumnName("SAC_SA_ID");
            entity.Property(e => e.SacSamplingId).HasColumnName("SAC_SamplingID");
            entity.Property(e => e.SacStatus).HasColumnName("SAC_Status");
            entity.Property(e => e.SacTestResult).HasColumnName("SAC_TestResult");
            entity.Property(e => e.SacWorkpaperId).HasColumnName("SAC_WorkpaperID");
        });

        modelBuilder.Entity<StandardAuditScheduleConductWorkPaper>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ScheduleConduct_WorkPaper");

            entity.Property(e => e.SswAttachId).HasColumnName("SSW_AttachID");
            entity.Property(e => e.SswAuditorHoursSpent).HasColumnName("SSW_AuditorHoursSpent");
            entity.Property(e => e.SswCompId).HasColumnName("SSW_CompID");
            entity.Property(e => e.SswConclusion)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SSW_Conclusion");
            entity.Property(e => e.SswCrBy).HasColumnName("SSW_CrBy");
            entity.Property(e => e.SswCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SSW_CrOn");
            entity.Property(e => e.SswCriticalAuditMatter)
                .IsUnicode(false)
                .HasColumnName("SSW_CriticalAuditMatter");
            entity.Property(e => e.SswDrlid).HasColumnName("SSW_DRLID");
            entity.Property(e => e.SswExceededMateriality).HasColumnName("SSW_ExceededMateriality");
            entity.Property(e => e.SswId).HasColumnName("SSW_ID");
            entity.Property(e => e.SswIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSW_IPAddress");
            entity.Property(e => e.SswNotesSteps)
                .IsUnicode(false)
                .HasColumnName("SSW_NotesSteps");
            entity.Property(e => e.SswObservation)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SSW_Observation");
            entity.Property(e => e.SswReviewedBy).HasColumnName("SSW_ReviewedBy");
            entity.Property(e => e.SswReviewedOn)
                .HasColumnType("datetime")
                .HasColumnName("SSW_ReviewedOn");
            entity.Property(e => e.SswReviewerComments)
                .IsUnicode(false)
                .HasColumnName("SSW_ReviewerComments");
            entity.Property(e => e.SswSaId).HasColumnName("SSW_SA_ID");
            entity.Property(e => e.SswStatus).HasColumnName("SSW_Status");
            entity.Property(e => e.SswTypeOfTest).HasColumnName("SSW_TypeOfTest");
            entity.Property(e => e.SswUpdatedBy).HasColumnName("SSW_UpdatedBy");
            entity.Property(e => e.SswUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("SSW_UpdatedOn");
            entity.Property(e => e.SswWorkpaperNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SSW_WorkpaperNo");
            entity.Property(e => e.SswWorkpaperRef)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SSW_WorkpaperRef");
            entity.Property(e => e.SswWpcheckListId).HasColumnName("SSW_WPCheckListID");
        });

        modelBuilder.Entity<StandardAuditScheduleInterval>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ScheduleIntervals");

            entity.Property(e => e.SaiCompId).HasColumnName("SAI_CompID");
            entity.Property(e => e.SaiCrBy).HasColumnName("SAI_CrBy");
            entity.Property(e => e.SaiCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SAI_CrOn");
            entity.Property(e => e.SaiEndDate)
                .HasColumnType("datetime")
                .HasColumnName("SAI_EndDate");
            entity.Property(e => e.SaiId).HasColumnName("SAI_ID");
            entity.Property(e => e.SaiIntervalId).HasColumnName("SAI_IntervalID");
            entity.Property(e => e.SaiIntervalSubId).HasColumnName("SAI_IntervalSubID");
            entity.Property(e => e.SaiIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SAI_IPAddress");
            entity.Property(e => e.SaiSaId).HasColumnName("SAI_SA_ID");
            entity.Property(e => e.SaiStartDate)
                .HasColumnType("datetime")
                .HasColumnName("SAI_StartDate");
        });

        modelBuilder.Entity<StandardAuditScheduleObservation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardAudit_ScheduleObservations");

            entity.Property(e => e.SsoCompId).HasColumnName("SSO_CompID");
            entity.Property(e => e.SsoCrBy).HasColumnName("SSO_CrBy");
            entity.Property(e => e.SsoCrOn)
                .HasColumnType("datetime")
                .HasColumnName("SSO_CrOn");
            entity.Property(e => e.SsoIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SSO_IPAddress");
            entity.Property(e => e.SsoObservations)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("SSO_Observations");
            entity.Property(e => e.SsoPkid).HasColumnName("SSO_PKID");
            entity.Property(e => e.SsoSaId).HasColumnName("SSO_SA_ID");
            entity.Property(e => e.SsoSacCheckPointId).HasColumnName("SSO_SAC_CheckPointID");
        });

        modelBuilder.Entity<TraceCabinet>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_Cabinet");

            entity.Property(e => e.TcCompId).HasColumnName("TC_CompID");
            entity.Property(e => e.TcCrBy).HasColumnName("TC_CrBy");
            entity.Property(e => e.TcCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TC_CrOn");
            entity.Property(e => e.TcIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TC_IPAddress");
            entity.Property(e => e.TcName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TC_Name");
            entity.Property(e => e.TcPkid).HasColumnName("TC_PKID");
            entity.Property(e => e.TcRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TC_Remarks");
            entity.Property(e => e.TcStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TC_Status");
        });

        modelBuilder.Entity<TraceColorMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_Color_Master");

            entity.Property(e => e.TcAccessCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TC_AccessCode");
            entity.Property(e => e.TcColorHex)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TC_Color_HEX");
            entity.Property(e => e.TcColorName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TC_Color_Name");
            entity.Property(e => e.TcCompId).HasColumnName("TC_CompID");
            entity.Property(e => e.TcId).HasColumnName("TC_ID");
            entity.Property(e => e.TcKeyCode)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("TC_KeyCode");
            entity.Property(e => e.TcStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TC_Status");
        });

        modelBuilder.Entity<TraceCompanyBranchDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_CompanyBranchDetails");

            entity.Property(e => e.CompanyBranchAddress)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Address");
            entity.Property(e => e.CompanyBranchCompId).HasColumnName("Company_Branch_CompID");
            entity.Property(e => e.CompanyBranchCompanyId).HasColumnName("Company_Branch_CompanyID");
            entity.Property(e => e.CompanyBranchContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Contact_Email");
            entity.Property(e => e.CompanyBranchContactLandLineNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Contact_LandLineNo");
            entity.Property(e => e.CompanyBranchContactMobileNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Contact_MobileNo");
            entity.Property(e => e.CompanyBranchContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Contact_Person");
            entity.Property(e => e.CompanyBranchCrby).HasColumnName("Company_Branch_CRBY");
            entity.Property(e => e.CompanyBranchCron)
                .HasColumnType("datetime")
                .HasColumnName("Company_Branch_CRON");
            entity.Property(e => e.CompanyBranchDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_DelFlag");
            entity.Property(e => e.CompanyBranchDesignation)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Designation");
            entity.Property(e => e.CompanyBranchId).HasColumnName("Company_Branch_Id");
            entity.Property(e => e.CompanyBranchIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_IPAddress");
            entity.Property(e => e.CompanyBranchName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_Name");
            entity.Property(e => e.CompanyBranchStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("Company_Branch_STATUS");
            entity.Property(e => e.CompanyBranchUpdatedBy).HasColumnName("Company_Branch_UpdatedBy");
            entity.Property(e => e.CompanyBranchUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Company_Branch_UpdatedOn");
        });

        modelBuilder.Entity<TraceCompanyDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Trace_CompanyDetails");

            entity.Property(e => e.CompanyAccountNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_AccountNo");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("Company_Address");
            entity.Property(e => e.CompanyAttachId).HasColumnName("Company_AttachID");
            entity.Property(e => e.CompanyBankAddress)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("Company_BankAddress");
            entity.Property(e => e.CompanyBankCity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_BankCity");
            entity.Property(e => e.CompanyBankCountry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_BankCountry");
            entity.Property(e => e.CompanyBankPincode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Company_BankPincode");
            entity.Property(e => e.CompanyBankState)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_BankState");
            entity.Property(e => e.CompanyBankname)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Bankname");
            entity.Property(e => e.CompanyBranch)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Company_Branch");
            entity.Property(e => e.CompanyCity)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_City");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Company_Code");
            entity.Property(e => e.CompanyCompId).HasColumnName("Company_CompID");
            entity.Property(e => e.CompanyConditions)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Conditions");
            entity.Property(e => e.CompanyContactEmailId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Company_ContactEmailID");
            entity.Property(e => e.CompanyContactNo1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Company_ContactNo1");
            entity.Property(e => e.CompanyContactNo2)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Company_ContactNo2");
            entity.Property(e => e.CompanyContactPerson)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_ContactPerson");
            entity.Property(e => e.CompanyCopdateOfRegNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_COPDateOfRegNo");
            entity.Property(e => e.CompanyCopregNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_COPRegNo");
            entity.Property(e => e.CompanyCoprenewalDate)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_COPRenewalDate");
            entity.Property(e => e.CompanyCountry)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Country");
            entity.Property(e => e.CompanyCrBy).HasColumnName("Company_CrBy");
            entity.Property(e => e.CompanyCrOn)
                .HasColumnType("datetime")
                .HasColumnName("Company_CrOn");
            entity.Property(e => e.CompanyEmailId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Company_EmailID");
            entity.Property(e => e.CompanyEstablishmentDate)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Establishment_Date");
            entity.Property(e => e.CompanyHolderName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Company_HolderName");
            entity.Property(e => e.CompanyId).HasColumnName("Company_ID");
            entity.Property(e => e.CompanyIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_IPAddress");
            entity.Property(e => e.CompanyMobileNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Company_MobileNo");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Name");
            entity.Property(e => e.CompanyPaymentterms)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_Paymentterms");
            entity.Property(e => e.CompanyPcaobdateOfRegNo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_PCAOBDateOfRegNo");
            entity.Property(e => e.CompanyPcaobrenewalDate)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_PCAOBRenewalDate");
            entity.Property(e => e.CompanyPinCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_PinCode");
            entity.Property(e => e.CompanyState)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Company_State");
            entity.Property(e => e.CompanyStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_Status");
            entity.Property(e => e.CompanySubmittedBy).HasColumnName("Company_SubmittedBy");
            entity.Property(e => e.CompanySubmittedOn)
                .HasColumnType("datetime")
                .HasColumnName("Company_SubmittedOn");
            entity.Property(e => e.CompanySwiftCode)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Company_SwiftCode");
            entity.Property(e => e.CompanyTelephoneNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Company_TelephoneNo");
            entity.Property(e => e.CompanyUpdatedBy).HasColumnName("Company_UpdatedBy");
            entity.Property(e => e.CompanyUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Company_UpdatedOn");
            entity.Property(e => e.CompanyWebSite)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Company_WebSite");
        });

        modelBuilder.Entity<TraceDocument>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_Documents");

            entity.Property(e => e.TfAttchCrBy).HasColumnName("TF_Attch_CrBy");
            entity.Property(e => e.TfAttchCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TF_Attch_CrOn");
            entity.Property(e => e.TfCabinetId).HasColumnName("TF_CabinetID");
            entity.Property(e => e.TfCompId).HasColumnName("TF_CompID");
            entity.Property(e => e.TfCrBy).HasColumnName("TF_CrBy");
            entity.Property(e => e.TfCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TF_CrOn");
            entity.Property(e => e.TfDesc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TF_Desc");
            entity.Property(e => e.TfFilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("TF_FilePath");
            entity.Property(e => e.TfFolderId).HasColumnName("TF_FolderID");
            entity.Property(e => e.TfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TF_IPAddress");
            entity.Property(e => e.TfName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TF_Name");
            entity.Property(e => e.TfOle)
                .HasMaxLength(1)
                .HasColumnName("TF_OLE");
            entity.Property(e => e.TfPkid).HasColumnName("TF_PKID");
            entity.Property(e => e.TfRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TF_Remarks");
            entity.Property(e => e.TfStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TF_Status");
            entity.Property(e => e.TfSubCabinetId).HasColumnName("TF_SubCabinetID");
            entity.Property(e => e.TfUpdatedBy).HasColumnName("TF_UpdatedBy");
            entity.Property(e => e.TfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TF_UpdatedOn");
        });

        modelBuilder.Entity<TraceErrorReplacement>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_Error_Replacement");

            entity.Property(e => e.TerErrorReplacemnet)
                .IsUnicode(false)
                .HasColumnName("TER_ErrorReplacemnet");
            entity.Property(e => e.TerPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("TER_PKID");
            entity.Property(e => e.TerRunTimeError)
                .IsUnicode(false)
                .HasColumnName("TER_RunTimeError");
            entity.Property(e => e.TerStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("TER_Status");
        });

        modelBuilder.Entity<TraceFolder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_Folder");

            entity.Property(e => e.TfCabinetId).HasColumnName("TF_CabinetID");
            entity.Property(e => e.TfCompId).HasColumnName("TF_CompID");
            entity.Property(e => e.TfCrBy).HasColumnName("TF_CrBy");
            entity.Property(e => e.TfCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TF_CrOn");
            entity.Property(e => e.TfIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TF_IPAddress");
            entity.Property(e => e.TfName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TF_Name");
            entity.Property(e => e.TfPkid).HasColumnName("TF_PKID");
            entity.Property(e => e.TfRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TF_Remarks");
            entity.Property(e => e.TfStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TF_Status");
            entity.Property(e => e.TfSubCabinetId).HasColumnName("TF_SubCabinetID");
            entity.Property(e => e.TfUpdatedBy).HasColumnName("TF_UpdatedBy");
            entity.Property(e => e.TfUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("TF_UpdatedOn");
        });

        modelBuilder.Entity<TraceReportMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Trace_Report_Master");

            entity.Property(e => e.TrmCustId).HasColumnName("TRM_CustID");
            entity.Property(e => e.TrmDescription)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TRM_Description");
            entity.Property(e => e.TrmHead).HasColumnName("TRM_Head");
            entity.Property(e => e.TrmHeaderName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TRM_HeaderName");
            entity.Property(e => e.TrmId).HasColumnName("TRM_ID");
            entity.Property(e => e.TrmIndId).HasColumnName("TRM_IndID");
            entity.Property(e => e.TrmParent).HasColumnName("TRM_Parent");
            entity.Property(e => e.TrmRptId).HasColumnName("TRM_RptID");
            entity.Property(e => e.TrmSubParent).HasColumnName("TRM_SubParent");
        });

        modelBuilder.Entity<TraceReportMaster1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Trace_Report_Master1");

            entity.Property(e => e.TrmCustId).HasColumnName("TRM_CustID");
            entity.Property(e => e.TrmDescription)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TRM_Description");
            entity.Property(e => e.TrmHead).HasColumnName("TRM_Head");
            entity.Property(e => e.TrmHeaderName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TRM_HeaderName");
            entity.Property(e => e.TrmId).HasColumnName("TRM_ID");
            entity.Property(e => e.TrmIndId).HasColumnName("TRM_IndID");
            entity.Property(e => e.TrmParent).HasColumnName("TRM_Parent");
            entity.Property(e => e.TrmRptId).HasColumnName("TRM_RptID");
            entity.Property(e => e.TrmSubParent).HasColumnName("TRM_SubParent");
        });

        modelBuilder.Entity<TraceSubCabinet>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRACe_SubCabinet");

            entity.Property(e => e.TscCabinetId).HasColumnName("TSC_CabinetID");
            entity.Property(e => e.TscCompId).HasColumnName("TSC_CompID");
            entity.Property(e => e.TscCrBy).HasColumnName("TSC_CrBy");
            entity.Property(e => e.TscCrOn)
                .HasColumnType("datetime")
                .HasColumnName("TSC_CrOn");
            entity.Property(e => e.TscDecs)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TSC_Decs");
            entity.Property(e => e.TscIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("TSC_IPAddress");
            entity.Property(e => e.TscName)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("TSC_Name");
            entity.Property(e => e.TscPkid).HasColumnName("TSC_PKID");
            entity.Property(e => e.TscRemarks)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("TSC_Remarks");
            entity.Property(e => e.TscStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TSC_Status");
        });

        modelBuilder.Entity<Upload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("upload");

            entity.Property(e => e.CompId).HasColumnName("CompID");
            entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");
            entity.Property(e => e.Pkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("PKID");
            entity.Property(e => e.YearId).HasColumnName("YearID");
        });

        modelBuilder.Entity<UploadedSharedDocument>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Uploaded_Shared_Documents");

            entity.Property(e => e.UsdCompId).HasColumnName("USD_CompID");
            entity.Property(e => e.UsdCreatedBy)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_CreatedBy");
            entity.Property(e => e.UsdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_CreatedOn");
            entity.Property(e => e.UsdDeletedBy).HasColumnName("USD_DeletedBy");
            entity.Property(e => e.UsdDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_DeletedOn");
            entity.Property(e => e.UsdExt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USD_Ext");
            entity.Property(e => e.UsdFname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_FName");
            entity.Property(e => e.UsdIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USD_IPAddress");
            entity.Property(e => e.UsdPkid).HasColumnName("USD_PKID");
            entity.Property(e => e.UsdRecalledBy).HasColumnName("USD_RecalledBy");
            entity.Property(e => e.UsdRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_RecalledOn");
            entity.Property(e => e.UsdSharedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_SharedOn");
            entity.Property(e => e.UsdSharedWith)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_SharedWith");
            entity.Property(e => e.UsdSize)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("USD_Size");
            entity.Property(e => e.UsdStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("USD_Status");
            entity.Property(e => e.UsdUpdatedBy)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_UpdatedBy");
            entity.Property(e => e.UsdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_UpdatedOn");
            entity.Property(e => e.UsdUploadedDocType).HasColumnName("USD_UploadedDocType");
            entity.Property(e => e.UsdYearId).HasColumnName("USD_YearID");
        });

        modelBuilder.Entity<UploadedSharedDocumentsLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Uploaded_Shared_Documents_log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NUsdExt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nUSD_Ext");
            entity.Property(e => e.NUsdFname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nUSD_FName");
            entity.Property(e => e.NUsdSharedOn)
                .HasColumnType("datetime")
                .HasColumnName("nUSD_SharedOn");
            entity.Property(e => e.NUsdSharedWith)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("nUSD_SharedWith");
            entity.Property(e => e.NUsdSize)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("nUSD_Size");
            entity.Property(e => e.NUsdUploadedDocType).HasColumnName("nUSD_UploadedDocType");
            entity.Property(e => e.NUsdYearId).HasColumnName("nUSD_YearID");
            entity.Property(e => e.UsdCompId).HasColumnName("USD_CompID");
            entity.Property(e => e.UsdCreatedBy)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_CreatedBy");
            entity.Property(e => e.UsdCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_CreatedOn");
            entity.Property(e => e.UsdDeletedBy).HasColumnName("USD_DeletedBy");
            entity.Property(e => e.UsdDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_DeletedOn");
            entity.Property(e => e.UsdExt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USD_Ext");
            entity.Property(e => e.UsdFname)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_FName");
            entity.Property(e => e.UsdIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USD_IPAddress");
            entity.Property(e => e.UsdPkid).HasColumnName("USD_PKID");
            entity.Property(e => e.UsdRecalledBy).HasColumnName("USD_RecalledBy");
            entity.Property(e => e.UsdRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_RecalledOn");
            entity.Property(e => e.UsdSharedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_SharedOn");
            entity.Property(e => e.UsdSharedWith)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_SharedWith");
            entity.Property(e => e.UsdSize)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("USD_Size");
            entity.Property(e => e.UsdUpdatedBy)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("USD_UpdatedBy");
            entity.Property(e => e.UsdUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("USD_UpdatedOn");
            entity.Property(e => e.UsdUploadedDocType).HasColumnName("USD_UploadedDocType");
            entity.Property(e => e.UsdYearId).HasColumnName("USD_YearID");
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("User_activity_logs");

            entity.Property(e => e.UtAsgnmntCompleted).HasColumnName("ut_asgnmnt_completed");
            entity.Property(e => e.UtAsgnmntCreated).HasColumnName("ut_asgnmnt_created");
            entity.Property(e => e.UtAsgnmntInprogress).HasColumnName("ut_asgnmnt_inprogress");
            entity.Property(e => e.UtCompid).HasColumnName("ut_compid");
            entity.Property(e => e.UtLoginDatetime)
                .HasColumnType("datetime")
                .HasColumnName("ut_login_datetime");
            entity.Property(e => e.UtLogindate).HasColumnName("ut_logindate");
            entity.Property(e => e.UtModule)
                .IsUnicode(false)
                .HasColumnName("ut_module");
            entity.Property(e => e.UtPkid).HasColumnName("ut_pkid");
            entity.Property(e => e.UtSubmodule)
                .IsUnicode(false)
                .HasColumnName("ut_submodule");
            entity.Property(e => e.UtUsrid).HasColumnName("ut_usrid");
        });

        modelBuilder.Entity<ViewCabPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_CabPermissions");

            entity.Property(e => e.CbnApprovedBy).HasColumnName("CBN_ApprovedBy");
            entity.Property(e => e.CbnApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_ApprovedOn");
            entity.Property(e => e.CbnCompId).HasColumnName("CBN_CompID");
            entity.Property(e => e.CbnCreatedBy).HasColumnName("CBN_CreatedBy");
            entity.Property(e => e.CbnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CreatedOn");
            entity.Property(e => e.CbnDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBN_DelFlag");
            entity.Property(e => e.CbnDeletedBy).HasColumnName("CBN_DeletedBy");
            entity.Property(e => e.CbnDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_DeletedOn");
            entity.Property(e => e.CbnDepartment).HasColumnName("CBN_Department");
            entity.Property(e => e.CbnFolderCount).HasColumnName("CBN_FolderCount");
            entity.Property(e => e.CbnId).HasColumnName("CBN_ID");
            entity.Property(e => e.CbnName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_Name");
            entity.Property(e => e.CbnNote)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnParent).HasColumnName("CBN_Parent");
            entity.Property(e => e.CbnRecalledBy).HasColumnName("CBN_RecalledBy");
            entity.Property(e => e.CbnRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_RecalledOn");
            entity.Property(e => e.CbnStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CBN_Status");
            entity.Property(e => e.CbnSubCabCount).HasColumnName("CBN_SubCabCount");
            entity.Property(e => e.CbnUpdatedBy).HasColumnName("CBN_UpdatedBy");
            entity.Property(e => e.CbnUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_UpdatedOn");
            entity.Property(e => e.CbnUserId).HasColumnName("CBN_UserID");
            entity.Property(e => e.CbpCabinet).HasColumnName("CBP_Cabinet");
            entity.Property(e => e.CbpCreate).HasColumnName("CBP_Create");
            entity.Property(e => e.CbpCreateFolder).HasColumnName("CBP_CreateFolder");
            entity.Property(e => e.CbpDelete).HasColumnName("CBP_Delete");
            entity.Property(e => e.CbpDepartment).HasColumnName("CBP_Department");
            entity.Property(e => e.CbpId).HasColumnName("CBP_ID");
            entity.Property(e => e.CbpIndex).HasColumnName("CBP_Index");
            entity.Property(e => e.CbpModify).HasColumnName("CBP_Modify");
            entity.Property(e => e.CbpOther).HasColumnName("CBP_Other");
            entity.Property(e => e.CbpPermissionType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBP_PermissionType");
            entity.Property(e => e.CbpSearch).HasColumnName("CBP_Search");
            entity.Property(e => e.CbpUser).HasColumnName("CBP_User");
            entity.Property(e => e.CbpView).HasColumnName("CBP_View");
        });

        modelBuilder.Entity<ViewDtpermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_DTPermissions");

            entity.Property(e => e.DotCrby).HasColumnName("DOT_CRBY");
            entity.Property(e => e.DotCron)
                .HasColumnType("datetime")
                .HasColumnName("DOT_CRON");
            entity.Property(e => e.DotDocname)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("DOT_DOCNAME");
            entity.Property(e => e.DotDoctypeid).HasColumnName("DOT_DOCTYPEID");
            entity.Property(e => e.DotNote)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("DOT_NOTE");
            entity.Property(e => e.DotPgroup).HasColumnName("DOT_PGROUP");
            entity.Property(e => e.DotStatus)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("DOT_STATUS");
            entity.Property(e => e.EdpDelDocument).HasColumnName("EDP_DEL_DOCUMENT");
            entity.Property(e => e.EdpDoctypeid).HasColumnName("EDP_DOCTYPEID");
            entity.Property(e => e.EdpGrpid).HasColumnName("EDP_GRPID");
            entity.Property(e => e.EdpIndex).HasColumnName("EDP_INDEX");
            entity.Property(e => e.EdpMfyDocument).HasColumnName("EDP_MFY_DOCUMENT");
            entity.Property(e => e.EdpMfyType).HasColumnName("EDP_MFY_TYPE");
            entity.Property(e => e.EdpOther).HasColumnName("EDP_OTHER");
            entity.Property(e => e.EdpPid).HasColumnName("EDP_PID");
            entity.Property(e => e.EdpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EDP_PTYPE");
            entity.Property(e => e.EdpSearch).HasColumnName("EDP_SEARCH");
            entity.Property(e => e.EdpUsrid).HasColumnName("EDP_USRID");
        });

        modelBuilder.Entity<ViewFolPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_FolPermissions");

            entity.Property(e => e.EfpCrtDoc).HasColumnName("EFP_CRT_DOC");
            entity.Property(e => e.EfpDelDoc).HasColumnName("EFP_DEL_DOC");
            entity.Property(e => e.EfpDelFolder).HasColumnName("EFP_DEL_FOLDER");
            entity.Property(e => e.EfpExport).HasColumnName("EFP_EXPORT");
            entity.Property(e => e.EfpFolId)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("EFP_FolId");
            entity.Property(e => e.EfpGrpid).HasColumnName("EFP_GRPID");
            entity.Property(e => e.EfpId)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("EFP_ID");
            entity.Property(e => e.EfpIndex).HasColumnName("EFP_INDEX");
            entity.Property(e => e.EfpModDoc).HasColumnName("EFP_MOD_DOC");
            entity.Property(e => e.EfpModFolder).HasColumnName("EFP_MOD_FOLDER");
            entity.Property(e => e.EfpOther).HasColumnName("EFP_OTHER");
            entity.Property(e => e.EfpPtype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EFP_PTYPE");
            entity.Property(e => e.EfpSearch).HasColumnName("EFP_SEARCH");
            entity.Property(e => e.EfpUsrid).HasColumnName("EFP_USRID");
            entity.Property(e => e.EfpViewFol).HasColumnName("EFP_VIEW_Fol");
            entity.Property(e => e.FolCabinet).HasColumnName("FOL_CABINET");
            entity.Property(e => e.FolCreatedBy).HasColumnName("FOL_CreatedBy");
            entity.Property(e => e.FolCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_CreatedOn");
            entity.Property(e => e.FolDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("Fol_DelFlag");
            entity.Property(e => e.FolFolid).HasColumnName("FOL_FOLID");
            entity.Property(e => e.FolName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FOL_NAME");
            entity.Property(e => e.FolNote)
                .IsUnicode(false)
                .HasColumnName("FOL_NOTE");
            entity.Property(e => e.FolStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("FOL_STATUS");
        });

        modelBuilder.Entity<ViewFolcab>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_folcab");

            entity.Property(e => e.CbnCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CBN_CreatedOn");
            entity.Property(e => e.CbnDelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CBN_DelFlag");
            entity.Property(e => e.CbnDepartment).HasColumnName("CBN_Department");
            entity.Property(e => e.CbnFolderCount).HasColumnName("CBN_FolderCount");
            entity.Property(e => e.CbnId).HasColumnName("CBN_ID");
            entity.Property(e => e.CbnName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CBN_NAME");
            entity.Property(e => e.CbnNote)
                .IsUnicode(false)
                .HasColumnName("CBN_Note");
            entity.Property(e => e.CbnParent).HasColumnName("CBN_PARENT");
            entity.Property(e => e.CbnSubCabCount).HasColumnName("CBN_SubCabCount");
            entity.Property(e => e.CbnUserid).HasColumnName("CBN_USERID");
            entity.Property(e => e.FolCabinet).HasColumnName("FOL_CABINET");
            entity.Property(e => e.FolCreatedBy).HasColumnName("FOL_CreatedBy");
            entity.Property(e => e.FolCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("FOL_CreatedOn");
            entity.Property(e => e.FolDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("fol_delflag");
            entity.Property(e => e.FolFolid).HasColumnName("FOL_FOLID");
            entity.Property(e => e.FolName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("FOL_NAME");
            entity.Property(e => e.FolNote)
                .IsUnicode(false)
                .HasColumnName("FOL_NOTE");
            entity.Property(e => e.FolStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("FOL_STATUS");
            entity.Property(e => e.UsrFullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usr_FullName");
        });

        modelBuilder.Entity<WfInwardMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WF_Inward_Masters");

            entity.Property(e => e.WimAddress)
                .IsUnicode(false)
                .HasColumnName("WIM_Address");
            entity.Property(e => e.WimApprovedBy).HasColumnName("WIM_ApprovedBy");
            entity.Property(e => e.WimApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_ApprovedOn");
            entity.Property(e => e.WimAttachId).HasColumnName("WIM_AttachID");
            entity.Property(e => e.WimCompId).HasColumnName("WIM_CompID");
            entity.Property(e => e.WimContactEmailId)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_ContactEmailID");
            entity.Property(e => e.WimContactPerson)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_ContactPerson");
            entity.Property(e => e.WimContactPhNo)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_ContactPhNO");
            entity.Property(e => e.WimCreatedBy).HasColumnName("WIM_CreatedBy");
            entity.Property(e => e.WimCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_CreatedOn");
            entity.Property(e => e.WimCustomer).HasColumnName("WIM_Customer");
            entity.Property(e => e.WimDateOnDocument)
                .HasColumnType("datetime")
                .HasColumnName("WIM_DateOnDocument");
            entity.Property(e => e.WimDeletedBy).HasColumnName("WIM_DeletedBy");
            entity.Property(e => e.WimDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_DeletedOn");
            entity.Property(e => e.WimDelflag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("WIM_Delflag");
            entity.Property(e => e.WimDeptartment).HasColumnName("WIM_Deptartment");
            entity.Property(e => e.WimDocRecievedDate)
                .HasColumnType("datetime")
                .HasColumnName("WIM_DocRecievedDate");
            entity.Property(e => e.WimDocReferenceno)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_DocReferenceno");
            entity.Property(e => e.WimInwardDate)
                .HasColumnType("datetime")
                .HasColumnName("WIM_InwardDate");
            entity.Property(e => e.WimInwardNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WIM_InwardNo");
            entity.Property(e => e.WimInwardOrWorkFlow).HasColumnName("WIM_InwardOrWorkFlow");
            entity.Property(e => e.WimInwardTime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WIM_InwardTime");
            entity.Property(e => e.WimIpadress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WIM_IPAdress");
            entity.Property(e => e.WimMonthId).HasColumnName("WIM_MonthID");
            entity.Property(e => e.WimPkid).HasColumnName("WIM_PKID");
            entity.Property(e => e.WimProgressStatus).HasColumnName("WIM_Progress_Status");
            entity.Property(e => e.WimRecalledBy).HasColumnName("WIM_RecalledBy");
            entity.Property(e => e.WimRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_RecalledOn");
            entity.Property(e => e.WimReceiptMode).HasColumnName("WIM_ReceiptMode");
            entity.Property(e => e.WimRemarks)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_Remarks");
            entity.Property(e => e.WimStage).HasColumnName("WIM_Stage");
            entity.Property(e => e.WimStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("WIM_Status");
            entity.Property(e => e.WimTitle)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("WIM_Title");
            entity.Property(e => e.WimUpdatedBy).HasColumnName("WIM_UpdatedBy");
            entity.Property(e => e.WimUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_UpdatedOn");
            entity.Property(e => e.WimWorkFlowArchiveId).HasColumnName("WIM_WorkFLowArchiveID");
            entity.Property(e => e.WimWorkFlowComments)
                .IsUnicode(false)
                .HasColumnName("WIM_WorkFlowComments");
            entity.Property(e => e.WimWorkFlowCreatedBy).HasColumnName("WIM_WorkFLowCreatedBy");
            entity.Property(e => e.WimWorkFlowCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("WIM_WorkFLowCreatedOn");
            entity.Property(e => e.WimWorkFlowId).HasColumnName("WIM_WorkFLowID");
            entity.Property(e => e.WimYearId).HasColumnName("WIM_YearID");
        });

        modelBuilder.Entity<WfInwardMastersHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WF_Inward_Masters_history");

            entity.Property(e => e.WimhCompId).HasColumnName("WIMH_CompID");
            entity.Property(e => e.WimhDatetime)
                .HasColumnType("datetime")
                .HasColumnName("WIMH_Datetime");
            entity.Property(e => e.WimhInwardPkid).HasColumnName("WIMH_InwardPKID");
            entity.Property(e => e.WimhLineNo).HasColumnName("WIMH_LineNo");
            entity.Property(e => e.WimhPkid).HasColumnName("WIMH_PKID");
            entity.Property(e => e.WimhRemarks)
                .IsUnicode(false)
                .HasColumnName("WIMH_Remarks");
            entity.Property(e => e.WimhReplyOrForward).HasColumnName("WIMH_ReplyOrForward");
            entity.Property(e => e.WimhSentToid).HasColumnName("WIMH_SentTOID");
            entity.Property(e => e.WimhStage).HasColumnName("WIMH_Stage");
            entity.Property(e => e.WimhUser).HasColumnName("WIMH_User");
        });

        modelBuilder.Entity<WfOutwardMaster>(entity =>
        {
            entity.HasKey(e => e.WomId).HasName("PK__WF_Outwa__89CCA30B066B556D");

            entity.ToTable("WF_Outward_Masters");

            entity.Property(e => e.WomId)
                .ValueGeneratedNever()
                .HasColumnName("WOM_ID");
            entity.Property(e => e.WomAddress)
                .IsUnicode(false)
                .HasColumnName("WOM_Address");
            entity.Property(e => e.WomApprovedBy).HasColumnName("WOM_ApprovedBy");
            entity.Property(e => e.WomApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("WOM_ApprovedOn");
            entity.Property(e => e.WomAttachId).HasColumnName("WOM_AttachID");
            entity.Property(e => e.WomAttachmentDetails)
                .IsUnicode(false)
                .HasColumnName("WOM_AttachmentDetails");
            entity.Property(e => e.WomCompId).HasColumnName("WOM_CompID");
            entity.Property(e => e.WomCreatedBy).HasColumnName("WOM_CreatedBy");
            entity.Property(e => e.WomCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("WOM_CreatedOn");
            entity.Property(e => e.WomCustomer).HasColumnName("WOM_Customer");
            entity.Property(e => e.WomDeletedBy).HasColumnName("WOM_DeletedBy");
            entity.Property(e => e.WomDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("WOM_DeletedOn");
            entity.Property(e => e.WomDelflag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("WOM_Delflag");
            entity.Property(e => e.WomDepartment).HasColumnName("WOM_Department");
            entity.Property(e => e.WomDispathMode).HasColumnName("WOM_DispathMode");
            entity.Property(e => e.WomDocumentType).HasColumnName("WOM_DocumentType");
            entity.Property(e => e.WomInwardId).HasColumnName("WOM_InwardID");
            entity.Property(e => e.WomInwardName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_InwardName");
            entity.Property(e => e.WomInwardRefNo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_InwardRefNo");
            entity.Property(e => e.WomIpaddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_IPAddress");
            entity.Property(e => e.WomMailingExpenses)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_MailingExpenses");
            entity.Property(e => e.WomMonthId).HasColumnName("WOM_MonthID");
            entity.Property(e => e.WomOutwardDate)
                .HasColumnType("datetime")
                .HasColumnName("WOM_OutwardDate");
            entity.Property(e => e.WomOutwardNo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_OutwardNo");
            entity.Property(e => e.WomOutwardRefNo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_OutwardRefNo");
            entity.Property(e => e.WomOutwardTime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WOM_OutwardTime");
            entity.Property(e => e.WomPage)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WOM_Page");
            entity.Property(e => e.WomRecalledBy).HasColumnName("WOM_RecalledBy");
            entity.Property(e => e.WomRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("WOM_RecalledOn");
            entity.Property(e => e.WomRemarks)
                .IsUnicode(false)
                .HasColumnName("WOM_Remarks");
            entity.Property(e => e.WomReplyAwaited).HasColumnName("WOM_ReplyAwaited");
            entity.Property(e => e.WomSendTo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("WOM_SendTo");
            entity.Property(e => e.WomSensitivity).HasColumnName("WOM_Sensitivity");
            entity.Property(e => e.WomStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("WOM_Status");
            entity.Property(e => e.WomUpdatedBy).HasColumnName("WOM_UpdatedBy");
            entity.Property(e => e.WomUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("WOM_UpdatedOn");
            entity.Property(e => e.WomYearId).HasColumnName("WOM_YearID");
        });

        modelBuilder.Entity<YearMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Year_Master");

            entity.Property(e => e.YmsApprovedBy).HasColumnName("YMS_ApprovedBy");
            entity.Property(e => e.YmsApprovedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_ApprovedOn");
            entity.Property(e => e.YmsCompId).HasColumnName("YMS_CompID");
            entity.Property(e => e.YmsCreatedBy).HasColumnName("YMS_CreatedBy");
            entity.Property(e => e.YmsCreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_CreatedOn");
            entity.Property(e => e.YmsDefault).HasColumnName("YMS_Default");
            entity.Property(e => e.YmsDeletedBy).HasColumnName("YMS_DeletedBy");
            entity.Property(e => e.YmsDeletedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_DeletedOn");
            entity.Property(e => e.YmsDelflag)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("YMS_Delflag");
            entity.Property(e => e.YmsFromdate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_FROMDATE");
            entity.Property(e => e.YmsId)
                .HasMaxLength(10)
                .HasColumnName("YMS_ID");
            entity.Property(e => e.YmsIpaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("YMS_IPAddress");
            entity.Property(e => e.YmsRecalledBy).HasColumnName("YMS_RecalledBy");
            entity.Property(e => e.YmsRecalledOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_RecalledOn");
            entity.Property(e => e.YmsStatus)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("YMS_Status");
            entity.Property(e => e.YmsTodate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_TODATE");
            entity.Property(e => e.YmsUpdatedBy).HasColumnName("YMS_UpdatedBy");
            entity.Property(e => e.YmsUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("YMS_UpdatedOn");
            entity.Property(e => e.YmsYearid)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("YMS_YEARID");
        });

        modelBuilder.Entity<YearMasterLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Year_Master_log");

            entity.Property(e => e.LogDate)
                .HasColumnType("datetime")
                .HasColumnName("Log_Date");
            entity.Property(e => e.LogOperation)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Log_Operation");
            entity.Property(e => e.LogPkid)
                .ValueGeneratedOnAdd()
                .HasColumnName("Log_PKID");
            entity.Property(e => e.LogUserId).HasColumnName("Log_UserID");
            entity.Property(e => e.NYmsDefault).HasColumnName("nYMS_Default");
            entity.Property(e => e.NYmsFromdate)
                .HasColumnType("datetime")
                .HasColumnName("nYMS_FROMDATE");
            entity.Property(e => e.NYmsId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nYMS_ID");
            entity.Property(e => e.NYmsTodate)
                .HasColumnType("datetime")
                .HasColumnName("nYMS_TODATE");
            entity.Property(e => e.YmsCompId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("YMS_CompID");
            entity.Property(e => e.YmsDefault).HasColumnName("YMS_Default");
            entity.Property(e => e.YmsFromdate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_FROMDATE");
            entity.Property(e => e.YmsId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("YMS_ID");
            entity.Property(e => e.YmsIpaddress)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("YMS_IPAddress");
            entity.Property(e => e.YmsTodate)
                .HasColumnType("datetime")
                .HasColumnName("YMS_TODATE");
            entity.Property(e => e.YmsYearid)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("YMS_YEARID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
