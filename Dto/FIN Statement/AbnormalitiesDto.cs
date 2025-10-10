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

        //Type1
        public class AbnormalTransactions1Dto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal CreditAmt { get; set; }
            public decimal DebitAmt { get; set; }
            public decimal? AvgCreditAmt { get; set; }
            public decimal? AvgDebitAmt { get; set; }
            public string AbnormalType { get; set; }
        }

        //Type2
        public class AbnormalTransactions2Dto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal CreditAmt { get; set; }
            public decimal DebitAmt { get; set; }
            public decimal? AvgCreditAmt { get; set; }
            public decimal? AvgDebitAmt { get; set; }
            public string AbnormalType { get; set; }
        }

        //Type3
        public class AbnormalTransactions3Dto
        {
            public string AJTB_DescName { get; set; }
            public decimal CreditAmt { get; set; }
            public decimal DebitAmt { get; set; }

            public decimal? AvgCreditAmt { get; set; }
            public decimal? AvgDebitAmt { get; set; }
            public decimal? AvgCreditAmtRatio { get; set; }
            public decimal? AvgDebitAmtRatio { get; set; }

            public int? DuplicateCount { get; set; }
            public decimal? CreditPercentage { get; set; }
            public decimal? DebitPercentage { get; set; }
        }

    }
}
