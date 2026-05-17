using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Services
{
    // 접근 권한 소속 검증을 수행하고 팀 메인 대시보드 화면 구성에 필요한 전체 패키지 데이터를 집약 가공하여 반환
    public interface ITeamsService
    {
        Task<GetTeamMainResponse> GetTeamMainAsync(int teamId, int currentUserId);
        Task<UpdateTeamNameResponse> UpdateTeamNameAsync(int teamId, int currentUserId, UpdateTeamNameRequest request);
        Task<CreateTeamPostResponse> CreateTeamPostAsync(int teamId, int currentUserId, CreateTeamPostRequest request);
        Task<UpdateTeamPostResponse> UpdateTeamPostAsync(int teamId, int postId, int currentUserId, UpdateTeamPostRequest request);
        Task<DeleteTeamPostResponse> DeleteTeamPostAsync(int teamId, int postId, int currentUserId);
        Task<GetTeamPostDetailResponse> GetTeamPostDetailAsync(int teamId, int postId, int currentUserId);
        Task<CreateTeamPostCommentResponse> CreateTeamPostCommentAsync(int teamId, int postId, int currentUserId, CreateTeamPostCommentRequest request);
        Task<EndProjectResponse> EndProjectAsync(int teamId, int currentUserId);
        Task<UpdateTeamRolesResponse> UpdateTeamRolesAsync(int teamId, int currentUserId, UpdateTeamRolesRequest request);
    }
}


