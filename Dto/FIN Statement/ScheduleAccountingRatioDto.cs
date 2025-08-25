namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleAccountingRatioDto
    {

        //GetAccoutingRatio
        public class AssetsLiabilitiesDto
        {
            public decimal LiabilitiesDc1 { get; set; }
            public decimal LiabilitiesDP1 { get; set; }
            public decimal AssetsDc1 { get; set; }
            public decimal AssetsDP1 { get; set; }
        }

    }
}
