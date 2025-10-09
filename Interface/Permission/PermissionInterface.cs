using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TracePca.Dto.AssetRegister;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFiling;
using TracePca.Dto.DigitalFilling;
using TracePca.Service;
using static TracePca.Service.DigitalFilling.Cabinet;

namespace TracePca.Interface.Permission
{
    public interface PermissionInterface
	{
		 
		Task<IEnumerable<PermissionDto>> LoadPermissionDetailsAsync(int ModuleID, string PermissionType, int PermissionID, int CompID);

		Task<IEnumerable<PermissionDto>> LoadPermissionModuleAsync(int CompID);

		Task<IEnumerable<PermissionDto>> LoadPermissionRoleAsync(string sPermissionType, int CompID);

		Task<string> SaveOrUpdatePermissionAsync(int ModuleID, string PermissionType, int UsrOrGrpId, string sOpPkID, int UserID, int compID);

	}
}
