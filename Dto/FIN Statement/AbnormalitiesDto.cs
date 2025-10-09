namespace TracePca.Dto.FIN_Statement
{
    public class AbnormalitiesDto
    {

        //GetAbnormalTransactions
        //public class AbnormalTransactionsDto
        //{
        //    public string AJTB_DescName { get; set; }
        //    public decimal creditAmt { get; set; }
        //    public decimal DebitAmt { get; set; }
        //    public decimal AvgCreditAmt { get; set; }
        //    public decimal AvgDebitAmt { get; set; }
        //    public decimal AvgCreditAmtRatio { get; set; }
        //    public decimal AvgDebitAmtRatio { get; set; }
        //}

        public class AbnormalTransactionsDto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_DescName { get; set; } = string.Empty;
            public decimal CreditAmt { get; set; }
            public decimal DebitAmt { get; set; }
            public decimal AvgCreditAmt { get; set; }
            public decimal AvgDebitAmt { get; set; }
            public string AbnormalType { get; set; } = string.Empty;
        }


    }
}
