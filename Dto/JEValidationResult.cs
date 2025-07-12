namespace TracePca.Dto
{
    public class JEValidationResult
    {
        public string TransactionNo { get; set; }
        public bool IsValid { get; set; }
        public List<string> MatchedRules { get; set; } = new(); // Initialize to avoid nulls
        public string ValidationMessage { get; set; }
    }
}
