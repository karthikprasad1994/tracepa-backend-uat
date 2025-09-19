namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleNoteDto
    {

        //GetSubHeadingname(Notes For SubHeading)
        public class SubHeadingNoteDto
        {
            public int ASHN_ID { get; set; }
            public string Description { get; set; }
        }

        //SaveOrUpdateSubHeadingNotes(Notes For SubHeading)
        public class SubHeadingNotesDto
        {
            public int ASHN_ID { get; set; }
            public int ASHN_SubHeadingId { get; set; }
            public int ASHN_CustomerId { get; set; }
            public string ASHN_Description { get; set; }
            public string ASHN_DelFlag { get; set; }
            public string ASHN_Status { get; set; }
            public string ASHN_Operation { get; set; }
            public int ASHN_CreatedBy { get; set; }
            public DateTime ASHN_CreatedOn { get; set; }
            public int ASHN_CompID { get; set; }
            public int ASHN_YearID { get; set; }
            public string ASHN_IPAddress { get; set; }
        }

        //GetBranch(Notes For Ledger)
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //GetLedger(Notes For Ledger)
        public class LedgerIndividualDto
        {
            public string Description { get; set; }
            public int ASHL_ID { get; set; }
        }

        //SaveOrUpdateLedger(Notes For Ledger)
        public class SubHeadingLedgerNoteDto
        {
            public int ASHL_ID { get; set; }
            public int ASHL_SubHeadingId { get; set; }
            public int ASHL_CustomerId { get; set; }
            public int ASHL_BranchId { get; set; }
            public string ASHL_Description { get; set; }
            public string ASHL_DelFlag { get; set; }
            public string ASHL_Status { get; set; }
            public string ASHL_Operation { get; set; }
            public int ASHL_CreatedBy { get; set; }
            public DateTime ASHL_CreatedOn { get; set; }
            public int ASHL_CompID { get; set; }
            public int ASHL_YearID { get; set; }
            public string ASHL_IPAddress { get; set; }
        }

        //DownloadNotesExcel
        public class ExcelFileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //DownloadNotesPdf
        public class PdfFileDownloadResult
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        //SaveOrUpdate
        public class FirstScheduleNoteDto
        {
            public int SNF_ID { get; set; }
            public int SNF_CustId { get; set; }
            public string SNF_Description { get; set; }
            public string SNF_Category { get; set; }
            public decimal SNF_CYear_Amount { get; set; }
            public decimal SNF_PYear_Amount { get; set; }
            public int SNF_YearID { get; set; }
            public int SNF_CompID { get; set; }
            public string SNF_Status { get; set; }
            public string SNF_DelFlag { get; set; }
            public DateTime SNF_CRON { get; set; }
            public int SNF_CrBy { get; set; }
            public string SNF_IPAddress { get; set; }
        }

        // --PreDefinied Notes //
        //SaveAuthorisedShareCapital(Particulars)
        public class AuthorisedShareCapitalDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty; 
            public decimal CurrentYearAmount { get; set; }        
            public decimal PreviousYearAmount { get; set; }         
            public int FinancialYearId { get; set; }  
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;   
        }

        //SaveIssuedSubscribedandFullyPaidupShareCapital
        public class IssuedSubscribedandFullyPaidupShareCapitalAsyncDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYearAmount { get; set; }
            public decimal PreviousYearAmount { get; set; }
            public int FinancialYearId { get; set; }
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;
        }

        //Save(A)Issued
        public class IssuedDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYearAmount { get; set; }
            public decimal PreviousYearAmount { get; set; }
            public int FinancialYearId { get; set; }
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;
        }

        //Save(B)SubscribedandPaid-up
        public class SubscribedandPaidupDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYearAmount { get; set; }
            public decimal PreviousYearAmount { get; set; }
            public int FinancialYearId { get; set; }
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;
        }

        //SaveCallsUnpaid
        public class CallsUnpaidDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYearAmount { get; set; }
            public decimal PreviousYearAmount { get; set; }
            public int FinancialYearId { get; set; }
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;
        }

        //SaveForfeitedShares
        public class ForfeitedSharesDto
        {
            public int Id { get; set; } // SNF_ID (0 for new, >0 for update)
            public int CustomerId { get; set; } // SNF_CustId
            public string Description { get; set; } = string.Empty;
            public decimal CurrentYearAmount { get; set; }
            public decimal PreviousYearAmount { get; set; }
            public int FinancialYearId { get; set; }
            public int CompanyId { get; set; } // SNF_CompId
            public int UserId { get; set; } // SNF_CRBy
            public string IpAddress { get; set; } = string.Empty;
        }

        //Save(i)EquityShares
        public class EquitySharesDto
        {
            public int SNS_ID { get; set; }
            public int SNS_CustId { get; set; }
            public string SNS_Description { get; set; }

            public decimal SNS_CYear_BegShares { get; set; }
            public decimal SNS_CYear_BegAmount { get; set; }
            public decimal SNS_PYear_BegShares { get; set; }
            public decimal SNS_PYear_BegAmount { get; set; }

            public decimal SNS_CYear_AddShares { get; set; }
            public decimal SNS_CYear_AddAmount { get; set; }
            public decimal SNS_PYear_AddShares { get; set; }
            public decimal SNS_PYear_AddAmount { get; set; }

            public decimal SNS_CYear_EndShares { get; set; }
            public decimal SNS_CYear_EndAmount { get; set; }
            public decimal SNS_PYear_EndShares { get; set; }
            public decimal SNS_PYear_EndAmount { get; set; }

            public int SNS_YearID { get; set; }
            public int SNS_CompID { get; set; }

            public int SNS_CrBy { get; set; }
            public string SNS_IPAddress { get; set; }
        }

        //Save(ii)PreferenceShares
        public class PreferenceSharesDto
        {
            public int SNS_ID { get; set; }
            public int SNS_CustId { get; set; }
            public string SNS_Description { get; set; }

            public decimal SNS_CYear_BegShares { get; set; }
            public decimal SNS_CYear_BegAmount { get; set; }
            public decimal SNS_PYear_BegShares { get; set; }
            public decimal SNS_PYear_BegAmount { get; set; }

            public decimal SNS_CYear_AddShares { get; set; }
            public decimal SNS_CYear_AddAmount { get; set; }
            public decimal SNS_PYear_AddShares { get; set; }
            public decimal SNS_PYear_AddAmount { get; set; }

            public decimal SNS_CYear_EndShares { get; set; }
            public decimal SNS_CYear_EndAmount { get; set; }
            public decimal SNS_PYear_EndShares { get; set; }
            public decimal SNS_PYear_EndAmount { get; set; }

            public int SNS_YearID { get; set; }
            public int SNS_CompID { get; set; }

            public int SNS_CrBy { get; set; }
            public string SNS_IPAddress { get; set; }
        }

        //Save(iii)EquityShares
        public class iiiEquitySharesDto
        {
            public int SNS_ID { get; set; }
            public int SNS_CustId { get; set; }
            public string SNS_Description { get; set; }

            public decimal SNS_CYear_BegShares { get; set; }
            public decimal SNS_CYear_BegAmount { get; set; }
            public decimal SNS_PYear_BegShares { get; set; }
            public decimal SNS_PYear_BegAmount { get; set; }

            public decimal SNS_CYear_AddShares { get; set; }
            public decimal SNS_CYear_AddAmount { get; set; }
            public decimal SNS_PYear_AddShares { get; set; }
            public decimal SNS_PYear_AddAmount { get; set; }

            public decimal SNS_CYear_EndShares { get; set; }
            public decimal SNS_CYear_EndAmount { get; set; }
            public decimal SNS_PYear_EndShares { get; set; }
            public decimal SNS_PYear_EndAmount { get; set; }

            public int SNS_YearID { get; set; }
            public int SNS_CompID { get; set; }

            public int SNS_CrBy { get; set; }
            public string SNS_IPAddress { get; set; }
        }

        //Save(iv)PreferenceShares
        public class ivPreferenceSharesDto
        {
            public int SNS_ID { get; set; }
            public int SNS_CustId { get; set; }
            public string SNS_Description { get; set; }

            public decimal SNS_CYear_BegShares { get; set; }
            public decimal SNS_CYear_BegAmount { get; set; }
            public decimal SNS_PYear_BegShares { get; set; }
            public decimal SNS_PYear_BegAmount { get; set; }

            public decimal SNS_CYear_AddShares { get; set; }
            public decimal SNS_CYear_AddAmount { get; set; }
            public decimal SNS_PYear_AddShares { get; set; }
            public decimal SNS_PYear_AddAmount { get; set; }

            public decimal SNS_CYear_EndShares { get; set; }
            public decimal SNS_CYear_EndAmount { get; set; }
            public decimal SNS_PYear_EndShares { get; set; }
            public decimal SNS_PYear_EndAmount { get; set; }

            public int SNS_YearID { get; set; }
            public int SNS_CompID { get; set; }

            public int SNS_CrBy { get; set; }
            public string SNS_IPAddress { get; set; }
        }

        //Save(b)EquityShareCapital
        public class EquityShareCapitalDto
        {
            public int Id { get; set; }                 // SNT_ID (0 for new, >0 for update)
            public int CustomerId { get; set; }         // SNT_CustId
            public string Description { get; set; } = string.Empty; // SNT_Description
            public int CYearShares { get; set; }        // SNT_CYear_Shares
            public decimal CYearAmount { get; set; }    // SNT_CYear_Amount
            public int PYearShares { get; set; }        // SNT_PYear_Shares
            public decimal PYearAmount { get; set; }    // SNT_PYear_Amount
            public int FinancialYearId { get; set; }    // SNT_YearId
            public int CompanyId { get; set; }          // SNT_CompId
            public int UserId { get; set; }             // SNT_CRBY
            public string IpAddress { get; set; } = string.Empty; // SNT_IPAddress
        }

        //Save(b)PreferenceShareCapital
        public class PreferenceShareCapitalDto
        {
            public int Id { get; set; }                 // SNT_ID (0 for new, >0 for update)
            public int CustomerId { get; set; }         // SNT_CustId
            public string Description { get; set; } = string.Empty; // SNT_Description
            public int CYearShares { get; set; }        // SNT_CYear_Shares
            public decimal CYearAmount { get; set; }    // SNT_CYear_Amount
            public int PYearShares { get; set; }        // SNT_PYear_Shares
            public decimal PYearAmount { get; set; }    // SNT_PYear_Amount
            public int FinancialYearId { get; set; }    // SNT_YearId
            public int CompanyId { get; set; }          // SNT_CompId
            public int UserId { get; set; }             // SNT_CRBY
            public string IpAddress { get; set; } = string.Empty; // SNT_IPAddress
        }

        //Save(c)Terms/rights attached to equity shares
        public class TermsToEquityShareeDto
        {
            public int Id { get; set; }              // SND_ID (not really used since we always delete+insert)
            public int CustomerId { get; set; }      // SND_CustId
            public string Description { get; set; } = string.Empty; // SND_Description
            public int FinancialYearId { get; set; } // SND_YearId
            public int CompanyId { get; set; }       // SND_CompId
            public int UserId { get; set; }          // SND_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SND_IPAddress
        }

        //Save(d)Terms/Rights attached to preference shares
        public class TermsToPreferenceShareDto
        {
            public int Id { get; set; }              // SND_ID (not really used since we always delete+insert)
            public int CustomerId { get; set; }      // SND_CustId
            public string Description { get; set; } = string.Empty; // SND_Description
            public int FinancialYearId { get; set; } // SND_YearId
            public int CompanyId { get; set; }       // SND_CompId
            public int UserId { get; set; }          // SND_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SND_IPAddress
        }

        //Save(e)Nameofthesharholder
        public class NameofthesharholderDto
        {
            public int Id { get; set; }                 // SNT_ID (0 for new, >0 for update)
            public int CustomerId { get; set; }         // SNT_CustId
            public string Description { get; set; } = string.Empty; // SNT_Description
            public int CYearShares { get; set; }        // SNT_CYear_Shares
            public decimal CYearAmount { get; set; }    // SNT_CYear_Amount
            public int PYearShares { get; set; }        // SNT_PYear_Shares
            public decimal PYearAmount { get; set; }    // SNT_PYear_Amount
            public int FinancialYearId { get; set; }    // SNT_YearId
            public int CompanyId { get; set; }          // SNT_CompId
            public int UserId { get; set; }             // SNT_CRBY
            public string IpAddress { get; set; } = string.Empty; // SNT_IPAddress
        }

        //Save(e)PreferenceShares
        public class ePreferenceSharesDto
        {
            public int Id { get; set; }                 // SNT_ID (0 for new, >0 for update)
            public int CustomerId { get; set; }         // SNT_CustId
            public string Description { get; set; } = string.Empty; // SNT_Description
            public int CYearShares { get; set; }        // SNT_CYear_Shares
            public decimal CYearAmount { get; set; }    // SNT_CYear_Amount
            public int PYearShares { get; set; }        // SNT_PYear_Shares
            public decimal PYearAmount { get; set; }    // SNT_PYear_Amount
            public int FinancialYearId { get; set; }    // SNT_YearId
            public int CompanyId { get; set; }          // SNT_CompId
            public int UserId { get; set; }             // SNT_CRBY
            public string IpAddress { get; set; } = string.Empty; // SNT_IPAddress
        }

        //Save(f)SharesAllotted
        public class FSahresAllottedDto
        {
            public int Id { get; set; }              // SND_ID (not really used since we always delete+insert)
            public int CustomerId { get; set; }      // SND_CustId
            public string Description { get; set; } = string.Empty; // SND_Description
            public int FinancialYearId { get; set; } // SND_YearId
            public int CompanyId { get; set; }       // SND_CompId
            public int UserId { get; set; }          // SND_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SND_IPAddress
        }

        //SaveEquityShares(Promoter name)
        public class SaveEquitySharesPromoterNameDto
        {
            public int Id { get; set; }               // SNFT_ID
            public int CustomerId { get; set; }       // SNFT_CustId
            public string Description { get; set; } = string.Empty; // SNFT_Description
            public int NumShares { get; set; }        // SNFT_NumShares
            public decimal TotalShares { get; set; }  // SNFT_TotalShares
            public decimal ChangedShares { get; set; } // SNFT_ChangedShares
            public int FinancialYearId { get; set; }  // SNFT_YearId
            public int CompanyId { get; set; }        // SNFT_CompId
            public int UserId { get; set; }           // SNFT_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SNFT_IPAddress
        }

        //SavePreferenceShares(Promoter name)
        public class SavePreferenceSharesPromoterNameDto
        {
            public int Id { get; set; }               // SNFT_ID
            public int CustomerId { get; set; }       // SNFT_CustId
            public string Description { get; set; } = string.Empty; // SNFT_Description
            public int NumShares { get; set; }        // SNFT_NumShares
            public decimal TotalShares { get; set; }  // SNFT_TotalShares
            public decimal ChangedShares { get; set; } // SNFT_ChangedShares
            public int FinancialYearId { get; set; }  // SNFT_YearId
            public int CompanyId { get; set; }        // SNFT_CompId
            public int UserId { get; set; }           // SNFT_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SNFT_IPAddress
        }

        //SaveFootNote
        public class FootNoteDto
        {
            public int Id { get; set; }              // SND_ID (not really used since we always delete+insert)
            public int CustomerId { get; set; }      // SND_CustId
            public string Description { get; set; } = string.Empty; // SND_Description
            public int FinancialYearId { get; set; } // SND_YearId
            public int CompanyId { get; set; }       // SND_CompId
            public int UserId { get; set; }          // SND_CRBY
            public string IpAddress { get; set; } = string.Empty;   // SND_IPAddress
        }

        //GetFirstNote
        public class FirstNoteDto
        {
            public int SNF_ID { get; set; }              // Primary Key
            public int SNF_CustId { get; set; }          // Customer ID
            public string SNF_Description { get; set; }  // Description
            public decimal SNF_CYear_Amount { get; set; } // Current Year Amount
            public decimal SNF_PYear_Amount { get; set; } // Previous Year Amount
        }

        //GetThirdNote
        public class ThirdNoteDto
        {
            public int SNT_ID { get; set; }              // Primary Key
            public int SNT_CustId { get; set; }          // Customer ID
            public string SNT_Description { get; set; }  // Description
            public decimal SNT_CYear_Amount { get; set; } // Current Year Amount
            public decimal SNT_PYear_Amount { get; set; } // Previous Year Amount
        }

        //GetFourthNote
        public class FourthNoteDto
        {
            public int SNFT_ID { get; set; }             // Primary Key
            public int SNFT_CustId { get; set; }         // Customer ID
            public string SNFT_Description { get; set; } // Description
            public int SNFT_NumShares { get; set; }      // Number of Shares
            public decimal SNFT_TotalShares { get; set; } // Total Shares
            public decimal SNFT_ChangedShares { get; set; } // Changed Shares
        }

        //DeleteFirstNote
        public class DeleteFirstNoteDto
        {
            public int Id { get; set; }          // SNF_ID
            public int CustomerId { get; set; }  // SNF_CustId
            public int CompId { get; set; }      // SNF_CompId
            public int YearId { get; set; }      // SNF_YearId
        }

        //DeleteThirdNote
        public class DeleteThirdNoteDto
        {
            public int Id { get; set; }          // SNT_ID
            public int CustomerId { get; set; }  // SNT_CustId
            public int CompId { get; set; }      // SNT_CompId
            public int YearId { get; set; }      // SNT_YearId
        }

        //DeleteFourthNote
        public class DeleteFourthNoteDto
        {
            public int Id { get; set; }          // SNFT_ID
            public int CustomerId { get; set; }  // SNFT_CustId
            public int CompId { get; set; }      // SNFT_CompId
            public int YearId { get; set; }      // SNFT_YearId
        }
    }
}
