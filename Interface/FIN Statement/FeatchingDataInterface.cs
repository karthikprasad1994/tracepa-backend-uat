namespace TracePca.Interface.FIN_Statement
{
    public interface FeatchingDataInterface
    {
        //Trdm
        Task<string> ExportTrdmFullDatabaseAsync();

        //Tr25_44
        Task<string> ExportTr25_44FullDatabaseAsync();

        //Customer Registraction
        Task<string> ExportCustomerRegistrationFullDatabaseAsync();
    }
}
