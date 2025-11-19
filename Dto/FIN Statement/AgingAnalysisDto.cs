namespace TracePca.Dto.FIN_Statement
{
    public class AgingAnalysisDto
    {

        //GetAnalysisBasedOnMonthForTradePayables
        public class TradePayablesDto
        {
            public int ID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal Debit_Before6M { get; set; }
            public decimal Debit_After6M { get; set; }
            public decimal Credit_Before6M { get; set; }
            public decimal Credit_After6M { get; set; }
            public decimal Total_Debit { get; set; }
            public decimal Total_Credit { get; set; }
        }

        //GetAnalysisBasedOnMonthForTradeReceivables
        public class TradeReceiveablesDto
        {
            public int ID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal Debit_Before6M { get; set; }
            public decimal Debit_After6M { get; set; }
            public decimal Credit_Before6M { get; set; }
            public decimal Credit_After6M { get; set; }
            public decimal Total_Debit { get; set; }
            public decimal Total_Credit { get; set; }
        }

        //GetAnalysisBasedOnMonthForTradePayablesById
        public class TradePayablesByIdDto
        {
            public int ID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal Debit_Before6M { get; set; }
            public decimal Debit_After6M { get; set; }
            public decimal Credit_Before6M { get; set; }
            public decimal Credit_After6M { get; set; }
            public decimal Total_Debit { get; set; }
            public decimal Total_Credit { get; set; }
        }

        //GetAnalysisBasedOnMonthForTradeReceivablesById
        public class TradeReceiveablesByIdDto
        {
            public int ID { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal Debit_Before6M { get; set; }
            public decimal Debit_After6M { get; set; }
            public decimal Credit_Before6M { get; set; }
            public decimal Credit_After6M { get; set; }
            public decimal Total_Debit { get; set; }
            public decimal Total_Credit { get; set; }
        }
    }
}
