using static TracePca.Dto.ProfileSetting.ProfileSettingDto;

namespace TracePca.Interface.ProfileSetting
{
    public interface ProfileSettingInterface
    {
        //GetUserProfile
        Task<IEnumerable<TracePaGetUserProfileDto>> GetUserProfileAsync(int iUserId);

        //PutChangePassword
        //Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(TracePaChangePasswordDto dto);
        Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(TracePaChangePasswordDto dto);

        //GetLicenseInformation
        Task<IEnumerable<TracePaLicenseInformationDto>> GetLicenseInformationAsync(string sEmailId, string sCustomerCode);

        //UpdateUserProfile
        Task<int> UpdateUserProfileAsync(UpdateUserProfileDto dto);
    }
}
