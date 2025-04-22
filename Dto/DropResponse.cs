namespace TracePca.Dto
{
    public class DropResponse
    {
       
        public IEnumerable<DropDownDto> Divisions { get; set; }
        public IEnumerable<DropDownDto> Departments { get; set; }
        public IEnumerable<DropDownDto> Locations { get; set; }
        public IEnumerable<DropDownDto> Headers { get; set; }
        public IEnumerable<DropDownDto> SubHeaders { get; set; }

        public IEnumerable<DropDownDto> Bay { get; set; }

        public IEnumerable<DropDownDto> Items { get; set; }
    }
}
