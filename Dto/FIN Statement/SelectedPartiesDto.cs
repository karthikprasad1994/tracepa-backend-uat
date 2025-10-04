namespace TracePca.Dto.FIN_Statement
{
    public class SelectedPartiesDto
    {

        //GetSelectedParties
        public class LoadTrailBalanceDto
        {
            public string ATBU_Description { get; set; }
            public int ATBU_Id { get; set; }
        }

        //UpdateSelectedPartiesStatus
        public class UpdateTrailBalanceStatusDto
        {
            public int Id { get; set; }
            public int CustId { get; set; }
            public int FinancialYearId { get; set; }
            public int BranchId { get; set; }
            public string Status { get; set; }
        }

    }
}
