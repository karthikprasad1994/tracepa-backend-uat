namespace TracePca.Dto
{
    public class AccJETransaction
    {
      
            public string Acc_JE_TransactionNo { get; set; }
            public string AJTB_TranscNo { get; set; }
            public string AJTB_Operation { get; set; }
            public string AJTB_Description { get; set; }
            public string AJTB_Comments { get; set; }
            public string AJTB_Narration { get; set; }
           public string AJTB_Remarks { get; set; }            // ✅ Add this
         public DateTime? AJTB_CreatedOn { get; set; }
          
          public decimal? AJTB_Amount { get; set; }
        public decimal? AJTB_Credit { get; set; }

        public decimal? AJTB_Debit { get; set; }

        public string? AJTB_DescName { get; set; }


    }

    }

 