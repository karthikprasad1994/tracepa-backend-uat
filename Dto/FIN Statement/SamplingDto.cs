namespace TracePca.Dto.FIN_Statement
{
    public class SamplingDto
    {

        //GetSystemSampling
        public class SystemstemSamplingDTO
        {
            public int AJTB_ID { get; set; }
            public string? AJTB_DescName { get; set; }
            public int AJTB_Debit { get; set; }
            public int AJTB_Credit { get; set; }
            public string AJTB_SyStatus { get; set; }
            public int RowNum { get; set; }
            public string? TransDate { get; set; }

        }

        //GetStatifiedSamping
        public class StratifiedSamplingDTO
        {
            public int AJTB_ID { get; set; }
            public string AJTB_DescName { get; set; }
            public decimal AJTB_Debit { get; set; }
            public decimal AJTB_Credit { get; set; }
            public string AJTB_SfStatus { get; set; }
            public DateTime TransDate { get; set; }
            public int RowNum { get; set; }
        }

        //UpdateSystemSamplingStatus
        public class UpdateSystemSamplingStatusDto
        {
            public int Id { get; set; }
            public string Status { get; set; }
        }

        //UpdateStatifiedSampingStatus
        public class UpdateStatifiedSampingStatusDto
        {
            public int Id { get; set; }
            public string Status { get; set; }
        }
    }
}

