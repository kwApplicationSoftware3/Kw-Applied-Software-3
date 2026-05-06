using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 내 프로필 관련 비즈니스 로직 인터페이스
    /// </summary>
    public interface IProfileService
    {
        Task<GetProfileResponse> GetProfileAsync(int UserId);
        Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request,int UserId);
    }
}
