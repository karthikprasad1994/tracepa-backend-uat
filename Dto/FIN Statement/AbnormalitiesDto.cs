namespace TracePca.Dto.FIN_Statement
{
    public class AbnormalitiesDto
    {

        //GetAbnormalTransactions
        public class AbnormalTransactionsDto
        {
            public string AJTB_DescName { get; set; }
            public decimal creditAmt { get; set; }
            public decimal DebitAmt { get; set; }
            public decimal AvgCreditAmt { get; set; }
            public decimal AvgDebitAmt { get; set; }
            public decimal AvgCreditAmtRatio { get; set; }
            public decimal AvgDebitAmtRatio { get; set; }
        }
    }
}
