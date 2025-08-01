using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Dto.Masters;
using static TracePca.Service.DigitalFilling.Cabinet;

namespace TracePca.Interface.Master
{
    public interface ErrorLogInterface
	{


 
        Task <string> LogErrorAsync(string accessCode, string message, string className, string functionName, ErrorLogDto dto);
		 
	}
}
