namespace TracePca.Dto.Authentication
{
    public class UpdateCustomerModulesDto
    {
        public int CustomerId { get; set; }

        public List<int> ModuleIds { get; set; } = new List<int>();

        // These lists must match by index (PkIds[i] -> ModuleIds[i])

    }

}

  

