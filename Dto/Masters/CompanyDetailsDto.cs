using TracePca.Dto.Audit;

namespace TracePca.Dto.Master
{
    public class CompanyDetailsListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CompanyDetailsDto
    {
        public int Company_ID { get; set; }
        public string Company_Code { get; set; }
        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_State { get; set; }
        public string Company_Country { get; set; }
        public string Company_PinCode { get; set; }
        public string Company_EmailID { get; set; }
        public string Company_Establishment_Date { get; set; }
        public string Company_ContactPerson { get; set; }
        public string Company_MobileNo { get; set; }
        public string Company_ContactEmailID { get; set; }
        public string Company_TelephoneNo { get; set; }
        public string Company_WebSite { get; set; }
        public string Company_ContactNo1 { get; set; }
        public string Company_ContactNo2 { get; set; }
        public string Company_HolderName { get; set; }
        public string Company_AccountNo { get; set; }
        public string Company_Bankname { get; set; }
        public string Company_Branch { get; set; }
        public string Company_Conditions { get; set; }
        public string Company_Paymentterms { get; set; }
        public int UserId { get; set; }
        public int CompId { get; set; }
        public string IpAddress { get; set; }
    }
}
