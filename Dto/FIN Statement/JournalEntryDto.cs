namespace TracePca.Dto.FIN_Statement
{
    public class JournalEntryDto
    {

        //GetCustomersName
        public class CustDto
        {
            public int Cust_Id { get; set; }
            public string? Cust_Name { get; set; }
        }

        //GetFinancialYear
        public class FinancialYearDto
        {
            public int YearId { get; set; }
            public string? Id { get; set; }
        }

        //GetDuration
        public class CustDurationDto
        {
            public int Cust_DurtnId { get; set; }
        }

        //GetBranchName
        public class CustBranchDto
        {
            public int Branchid { get; set; }
            public string? BranchName { get; set; }
        }

        //GetJournalEntryInformation
        public class JournalEntryInformationDto
        {
            public int Id { get; set; }
            public string TransactionNo { get; set; }
            public int BranchID { get; set; }
            public string BillNo { get; set; }
            public string BillDate { get; set; }
            public string Party { get; set; }
            public int PartyID { get; set; }
            public string BillType { get; set; }
            public string DebDescription { get; set; }
            public decimal Debit { get; set; }
            public string CredDescription { get; set; }
            public decimal Credit { get; set; }
            public string Status { get; set; }
        }
    }
}
