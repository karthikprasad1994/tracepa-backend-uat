namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleAccountingRatioDto
    {

        //GetAccoutingRatio1
        public class Ratio1Dto
        {
            public decimal LiabilitiesDc1 { get; set; }
            public decimal LiabilitiesDP1 { get; set; }
            public decimal AssetsDc1 { get; set; }
            public decimal AssetsDP1 { get; set; }
        }

        //GetAccoutingRatio2
        public class Ratio2Dto
        {
            public decimal Current_Reporting_Period { get; set; }
            public decimal Previous_Reporting_Period { get; set; }
            public decimal Change { get; set; }
        }
    }
}
