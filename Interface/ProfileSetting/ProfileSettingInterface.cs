using static TracePca.Dto.ProfileSetting.ProfileSettingDto;

namespace TracePca.Interface.ProfileSetting
{
    public interface ProfileSettingInterface
    {
        //GetUserProfile
        Task<IEnumerable<TracePaGetUserProfileDto>> GetUserProfileAsync(int iUserId);

        //PutChangePassword
        Task<IEnumerable<TracePaChangePasswordDto>> PutChangePasswordAsync(string LoginName, int UserId, TracePaChangePasswordDto dto);

        //GetLicenseInformation
        Task<IEnumerable<TracePaLicenseInformationDto>> GetLicenseInformationAsync(int iCustomerId);
    }
}
