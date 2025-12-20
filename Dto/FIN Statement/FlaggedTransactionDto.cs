namespace TracePca.Dto.FIN_Statement
{
    public class FlaggedTransactionDto
    {

        //GetDiferenceAmountStatus
        public class GetDiferenceAmountStatusDto
        {
            public int ATBU_ID { get; set; }
            public string ATBU_STATUS { get; set; }
            public string ATBU_Description { get; set; }
            public decimal ATBU_Closing_Debit_Amount { get; set; }
            public decimal ATBU_Closing_Credit_Amount { get; set; }
            public decimal PrevATBU_Closing_Debit_Amount { get; set; }
            public decimal PrevATBU_Closing_Credit_Amount { get; set; }

        }

        //GetAbnormalEntriesAEStatus 
        public class GetGetAbnormalEntriesSeqReferenceNumDto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_AEStatus { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
        }

        //GetSelectedPartiesSeqReferenceNum
        public class GetSelectedPartiesSeqReferenceNumDto
        {
            public int AJTB_ID { get; set; }
            public int AJTB_SeqReferenceNum { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
        }

        //GetSystemSamplingStatus
        public class GetSystemSamplingStatusDto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_SyStatus { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
        }

        //GetSystemSamplingStatus
        public class GetStatifiedSampingStatusDto
        {
            public int AJTB_ID { get; set; }
            public string AJTB_SfStatus { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
        }
    }
}
