using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    // 내 프로필 관련 비즈니스 로직 인터페이스
    public interface IProfileService
    {
        // 프로필 불러오기
        Task<GetProfileResponse> GetProfileAsync(int UserId);
        // 프로필 업데이트
        Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request,int UserId);
    }
}
