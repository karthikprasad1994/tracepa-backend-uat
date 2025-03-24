using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TracePca.Models.UserModels;

namespace TracePca.Data;

public partial class DynamicDbContext : DbContext
{
    public DynamicDbContext()
    {
    }

    public DynamicDbContext(DbContextOptions<DynamicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccAccountPolicy> AccAccountPolicies { get; set; }

    public virtual DbSet<AccAssetDepItact> AccAssetDepItacts { get; set; }

    public virtual DbSet<AccAssetDepreciation> AccAssetDepreciations { get; set; }

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

    public virtual DbSet<AccLedgerTransactionsDetail> AccLedgerTransactionsDetails { get; set; }

    public virtual DbSet<AccLtMaster> AccLtMasters { get; set; }

    public virtual DbSet<AccOpeningBalance> AccOpeningBalances { get; set; }

    public virtual DbSet<AccOpeningBalance1> AccOpeningBalance1s { get; set; }

    public virtual DbSet<AccPartnershipFirm> AccPartnershipFirms { get; set; }

    public virtual DbSet<AccProfitAndLossAmount> AccProfitAndLossAmounts { get; set; }

    public virtual DbSet<AccScheduleHeading> AccScheduleHeadings { get; set; }

    public virtual DbSet<AccScheduleItem> AccScheduleItems { get; set; }

    public virtual DbSet<AccScheduleSubHeading> AccScheduleSubHeadings { get; set; }

    public virtual DbSet<AccScheduleSubItem> AccScheduleSubItems { get; set; }

    public virtual DbSet<AccScheduleTemplate> AccScheduleTemplates { get; set; }

    public virtual DbSet<AccScheduleTemplates1> AccScheduleTemplates1s { get; set; }

    public virtual DbSet<AccSeperateSchedule> AccSeperateSchedules { get; set; }

    public virtual DbSet<AccSubHeadingLedgerDesc> AccSubHeadingLedgerDescs { get; set; }

    public virtual DbSet<AccSubHeadingNoteDesc> AccSubHeadingNoteDescs { get; set; }

    public virtual DbSet<AccTradeUpload> AccTradeUploads { get; set; }

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

    public virtual DbSet<AuditTimeline> AuditTimelines { get; set; }

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

    public virtual DbSet<CompanyLoSetting> CompanyLoSettings { get; set; }

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

    public virtual DbSet<EdtOutlookAttach> EdtOutlookAttaches { get; set; }

    public virtual DbSet<EdtPage> EdtPages { get; set; }

    public virtual DbSet<EdtPage1> EdtPage1s { get; set; }

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

    public virtual DbSet<SadCustomerMaster> SadCustomerMasters { get; set; }

    public virtual DbSet<SadCustomerMaster1> SadCustomerMaster1s { get; set; }

    public virtual DbSet<SadCustomerMaster2> SadCustomerMaster2s { get; set; }

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

    public virtual DbSet<SadModOperation> SadModOperations { get; set; }

    public virtual DbSet<SadModule> SadModules { get; set; }

    public virtual DbSet<SadOrgStructure> SadOrgStructures { get; set; }

    public virtual DbSet<SadOrgStructureLog> SadOrgStructureLogs { get; set; }

    public virtual DbSet<SadReportContentMaster> SadReportContentMasters { get; set; }

    public virtual DbSet<SadReportGeneration> SadReportGenerations { get; set; }

    public virtual DbSet<SadStatutoryDirectorDetail> SadStatutoryDirectorDetails { get; set; }

    public virtual DbSet<SadStatutoryPartnerDetail> SadStatutoryPartnerDetails { get; set; }

    public virtual DbSet<SadSupplierMaster> SadSupplierMasters { get; set; }

    public virtual DbSet<SadUserDetail> SadUserDetails { get; set; }

    public virtual DbSet<SadUserEmpAcademicProgress> SadUserEmpAcademicProgresses { get; set; }

    public virtual DbSet<SadUserEmpAddress> SadUserEmpAddresses { get; set; }

    public virtual DbSet<SadUserEmpAssessment> SadUserEmpAssessments { get; set; }

    public virtual DbSet<SadUserEmpAssetsLoan> SadUserEmpAssetsLoans { get; set; }

    public virtual DbSet<SadUserEmpCourse> SadUserEmpCourses { get; set; }

    public virtual DbSet<SadUserEmpParticularsofArticle> SadUserEmpParticularsofArticles { get; set; }

    public virtual DbSet<SadUserEmpProfExperiance> SadUserEmpProfExperiances { get; set; }

    public virtual DbSet<SadUserEmpQualification> SadUserEmpQualifications { get; set; }

    public virtual DbSet<SadUserEmpSpecialMention> SadUserEmpSpecialMentions { get; set; }

    public virtual DbSet<SadUserEmpTransferFirm> SadUserEmpTransferFirms { get; set; }

    public virtual DbSet<SadUserPasswordHistory> SadUserPasswordHistories { get; set; }

    public virtual DbSet<SadUserdetailsLog> SadUserdetailsLogs { get; set; }

    public virtual DbSet<SadUsersInOtherDept> SadUsersInOtherDepts { get; set; }

    public virtual DbSet<SadUsrOrGrpPermission> SadUsrOrGrpPermissions { get; set; }

    public virtual DbSet<SadUsrOrGrpPermissionD> SadUsrOrGrpPermissionDs { get; set; }

    public virtual DbSet<SadUsrOrGrpPermissionLog> SadUsrOrGrpPermissionLogs { get; set; }

    public virtual DbSet<SampleSelection> SampleSelections { get; set; }

    public virtual DbSet<SampleTable> SampleTables { get; set; }

    public virtual DbSet<ScheduleLinkageMaster> ScheduleLinkageMasters { get; set; }

    public virtual DbSet<ScheduleNoteDesc> ScheduleNoteDescs { get; set; }

    public virtual DbSet<ScheduleNoteFirst> ScheduleNoteFirsts { get; set; }

    public virtual DbSet<ScheduleNoteFourth> ScheduleNoteFourths { get; set; }

    public virtual DbSet<ScheduleNoteSecond> ScheduleNoteSeconds { get; set; }

    public virtual DbSet<ScheduleNoteThird> ScheduleNoteThirds { get; set; }

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

    public virtual DbSet<WfInwardMaster> WfInwardMasters { get; set; }

    public virtual DbSet<WfInwardMastersHistory> WfInwardMastersHistories { get; set; }

    public virtual DbSet<WfOutwardMaster> WfOutwardMasters { get; set; }

    public virtual DbSet<YearMaster> YearMasters { get; set; }

    public virtual DbSet<YearMasterLog> YearMasterLogs { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccAccountPolicy>(entity =>
        {
            entity.HasKey(e => e.AcpPkid).HasName("PK__Acc_Acco__475F870349128E00");

            entity.Property(e => e.AcpPkid).ValueGeneratedNever();
            entity.Property(e => e.AcpStatus).IsFixedLength();
        });

        modelBuilder.Entity<AccAssetDepItact>(entity =>
        {
            entity.Property(e => e.AditactDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccAssetDepreciation>(entity =>
        {
            entity.Property(e => e.AdepDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccAssetLocationSetup>(entity =>
        {
            entity.Property(e => e.LsDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccAssetMaster>(entity =>
        {
            entity.Property(e => e.AmDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccCashflow>(entity =>
        {
            entity.HasKey(e => e.AcfPkid).HasName("PK__Acc_Cash__425DFD1D2A686CB7");

            entity.Property(e => e.AcfPkid).ValueGeneratedNever();
            entity.Property(e => e.AcfStatus).IsFixedLength();
        });

        modelBuilder.Entity<AccFixedAssetAdditionDel>(entity =>
        {
            entity.Property(e => e.AfaaDelflag).IsFixedLength();
        });

        modelBuilder.Entity<AccFixedAssetDeletion>(entity =>
        {
            entity.Property(e => e.AfadDelflag).IsFixedLength();
        });

        modelBuilder.Entity<AccFixedAssetsTransaction>(entity =>
        {
            entity.Property(e => e.AccFatId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<AccGeneralMaster>(entity =>
        {
            entity.Property(e => e.MasOperation).IsFixedLength();
        });

        modelBuilder.Entity<AccSubHeadingLedgerDesc>(entity =>
        {
            entity.Property(e => e.AshlDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccSubHeadingNoteDesc>(entity =>
        {
            entity.Property(e => e.AshnDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<AccVoucherSetting>(entity =>
        {
            entity.Property(e => e.AvsOperation).IsFixedLength();
        });

        modelBuilder.Entity<AccYearMaster>(entity =>
        {
            entity.Property(e => e.YmsFreezeYear).IsFixedLength();
            entity.Property(e => e.YmsOperation).IsFixedLength();
        });

        modelBuilder.Entity<AdoBatch>(entity =>
        {
            entity.Property(e => e.BtDelflag).IsFixedLength();
        });

        modelBuilder.Entity<AuditAssignmentUserLog>(entity =>
        {
            entity.Property(e => e.AaulId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<AuditDocRequestList>(entity =>
        {
            entity.Property(e => e.DrlStatus).IsFixedLength();
        });

        modelBuilder.Entity<AuditLogDetail>(entity =>
        {
            entity.HasKey(e => e.AldId).HasName("PK__Audit_Lo__BDB474DB10D9317A");

            entity.Property(e => e.AldId).ValueGeneratedNever();
        });

        modelBuilder.Entity<AuditTimeCostBudgetDetail>(entity =>
        {
            entity.Property(e => e.AtcdType).IsFixedLength();
        });

        modelBuilder.Entity<AuditTimeCostBudgetMaster>(entity =>
        {
            entity.Property(e => e.AtcbDelFlag).IsFixedLength();
            entity.Property(e => e.AtcbType).IsFixedLength();
        });

        modelBuilder.Entity<AuditTimeSheet>(entity =>
        {
            entity.Property(e => e.TsIsApproved).IsFixedLength();
        });

        modelBuilder.Entity<AuditTimesheet1>(entity =>
        {
            entity.Property(e => e.TsIsApproved).IsFixedLength();
        });

        modelBuilder.Entity<BatchScanTable>(entity =>
        {
            entity.Property(e => e.BtDelflag).IsFixedLength();
        });

        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.Property(e => e.GlBalType).IsFixedLength();
            entity.Property(e => e.GlOperation).IsFixedLength();
        });

        modelBuilder.Entity<CmacheckMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<CmaratingCoreProcessLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<CmaratingLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<CmaratingSupportProcessLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ContentManagementMaster>(entity =>
        {
            entity.Property(e => e.CmmDelflag).IsFixedLength();
            entity.Property(e => e.CmsModule).IsFixedLength();
        });

        modelBuilder.Entity<ContentManagementMasterLog>(entity =>
        {
            entity.Property(e => e.CmmModule).IsFixedLength();
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
            entity.Property(e => e.NCmmModule).IsFixedLength();
        });

        modelBuilder.Entity<CustomerGlLinkageMaster>(entity =>
        {
            entity.Property(e => e.ClmOperation).IsFixedLength();
        });

        modelBuilder.Entity<EdtCollateLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtDescriptiosLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtDescriptorLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtDoctypeLinkLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtDoctypePermissionLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtDocumentTypeLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtPageLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EdtSettingsLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<HolidayMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<InsConfig>(entity =>
        {
            entity.Property(e => e.ConfAmPm).IsFixedLength();
            entity.Property(e => e.ConfHh).IsFixedLength();
            entity.Property(e => e.ConfMm).IsFixedLength();
        });

        modelBuilder.Entity<InsConfigLog>(entity =>
        {
            entity.Property(e => e.ConfAmPm).IsFixedLength();
            entity.Property(e => e.ConfHh).IsFixedLength();
            entity.Property(e => e.ConfMm).IsFixedLength();
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
            entity.Property(e => e.NconfAmPm).IsFixedLength();
            entity.Property(e => e.NconfHh).IsFixedLength();
            entity.Property(e => e.NconfMm).IsFixedLength();
        });

        modelBuilder.Entity<MstChecksMaster>(entity =>
        {
            entity.Property(e => e.ChkStatus).IsFixedLength();
        });

        modelBuilder.Entity<MstChecksMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstControlLibrary>(entity =>
        {
            entity.Property(e => e.MclStatus).IsFixedLength();
        });

        modelBuilder.Entity<MstControlLibraryLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstEntityMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstPasswordSettingLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstProcessMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstRiskLibraryLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstSubentityMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MstSubprocessMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RiskGeneralMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SadColorMaster>(entity =>
        {
            entity.Property(e => e.TcAccessCode).IsFixedLength();
            entity.Property(e => e.TcStatus).IsFixedLength();
        });

        modelBuilder.Entity<SadConfigSettingsLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SadEmpCategoryCharge>(entity =>
        {
            entity.Property(e => e.EmpcCdelFlag).IsFixedLength();
            entity.Property(e => e.EmpcDelFlag).IsFixedLength();
        });

        modelBuilder.Entity<SadFinalisationReportContent>(entity =>
        {
            entity.Property(e => e.FptDelflag).IsFixedLength();
        });

        modelBuilder.Entity<SadFinalisationReportTemplate>(entity =>
        {
            entity.Property(e => e.TemDelflag).IsFixedLength();
        });

        modelBuilder.Entity<SadGeneralBranchDetail>(entity =>
        {
            entity.Property(e => e.BranchStatus).IsFixedLength();
        });

        modelBuilder.Entity<SadGrpDesgnGeneralMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SadGrpOrLvlGeneralMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SadModOperation>(entity =>
        {
            entity.Property(e => e.OpStatus).IsFixedLength();
        });

        modelBuilder.Entity<SadOrgStructure>(entity =>
        {
            entity.Property(e => e.OrgCust).IsFixedLength();
        });

        modelBuilder.Entity<SadOrgStructureLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
            entity.Property(e => e.NorgCust).IsFixedLength();
            entity.Property(e => e.OrgCust).IsFixedLength();
        });

        modelBuilder.Entity<SadReportContentMaster>(entity =>
        {
            entity.Property(e => e.RcmDelflag).IsFixedLength();
        });

        modelBuilder.Entity<SadUserdetailsLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SadUsrOrGrpPermission>(entity =>
        {
            entity.Property(e => e.PermOperation).IsFixedLength();
            entity.Property(e => e.PermPtype).IsFixedLength();
            entity.Property(e => e.PermStatus).IsFixedLength();
        });

        modelBuilder.Entity<SadUsrOrGrpPermissionLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
            entity.Property(e => e.NPermPtype).IsFixedLength();
            entity.Property(e => e.PermPtype).IsFixedLength();
        });

        modelBuilder.Entity<ScheduleLinkageMaster>(entity =>
        {
            entity.Property(e => e.SlmOperation).IsFixedLength();
        });

        modelBuilder.Entity<TraceCabinet>(entity =>
        {
            entity.Property(e => e.TcStatus).IsFixedLength();
        });

        modelBuilder.Entity<TraceColorMaster>(entity =>
        {
            entity.Property(e => e.TcAccessCode).IsFixedLength();
            entity.Property(e => e.TcStatus).IsFixedLength();
        });

        modelBuilder.Entity<TraceDocument>(entity =>
        {
            entity.Property(e => e.TfStatus).IsFixedLength();
        });

        modelBuilder.Entity<TraceErrorReplacement>(entity =>
        {
            entity.Property(e => e.TerPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<TraceFolder>(entity =>
        {
            entity.Property(e => e.TfStatus).IsFixedLength();
        });

        modelBuilder.Entity<TraceSubCabinet>(entity =>
        {
            entity.Property(e => e.TscStatus).IsFixedLength();
        });

        modelBuilder.Entity<Upload>(entity =>
        {
            entity.Property(e => e.Pkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<UploadedSharedDocumentsLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WfOutwardMaster>(entity =>
        {
            entity.HasKey(e => e.WomId).HasName("PK__WF_Outwa__89CCA30BC58A6BD4");

            entity.Property(e => e.WomId).ValueGeneratedNever();
        });

        modelBuilder.Entity<SadUserDetail>(entity =>
        {
            entity.HasKey(e => e.UsrId);  
            entity.Property(e => e.UsrId).ValueGeneratedOnAdd();  

            entity.ToTable("Sad_UserDetails"); 
        });

        modelBuilder.Entity<YearMasterLog>(entity =>
        {
            entity.Property(e => e.LogPkid).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
