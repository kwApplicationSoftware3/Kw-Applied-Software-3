using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 내 활동 관련 비즈니스 로직 인터페이스
    /// </summary>
    public interface IMyActivitiesService
    {
        // 내 활동 관리
        Task<GetMyActivitiesResponse> GetMyActivitiesAsync(int userId);

        // 팀원평가
        Task<GetReviewTargetResponse> GetReviewTargetAsync(int teamId, int userId);
        
        // 팀원평가 제출
        Task<SubmitReviewResponse> SubmitReviewAsync(int teamId, SubmitReviewRequest request, int userId);
        
        // 모집글 모집 종료
        Task<ClosePostResponse> ClosePostAsync(ClosePostRequest request, int userId);

    }
}