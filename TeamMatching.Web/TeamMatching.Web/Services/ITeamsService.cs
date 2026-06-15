using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Services
{
    // 팀 관련 비지니스 로직 인터페이스
    public interface ITeamsService
    {
        // 팀 메인화면 조회
        Task<GetTeamMainResponse> GetTeamMainAsync(int teamId, int currentUserId);
        // 팀명 변경
        Task<UpdateTeamNameResponse> UpdateTeamNameAsync(int teamId, int currentUserId, UpdateTeamNameRequest request);
        // 팀 내부 게시글 작성
        Task<CreateTeamPostResponse> CreateTeamPostAsync(int teamId, int currentUserId, CreateTeamPostRequest request);
        // 팀 게시글 수정
        Task<UpdateTeamPostResponse> UpdateTeamPostAsync(int teamId, int postId, int currentUserId, UpdateTeamPostRequest request);
        // 팀 게시글 삭제
        Task<DeleteTeamPostResponse> DeleteTeamPostAsync(int teamId, int postId, int currentUserId);
        // 팀 게시글 조회
        Task<GetTeamPostDetailResponse> GetTeamPostDetailAsync(int teamId, int postId, int currentUserId);
        // 댓글 작성
        Task<CreateTeamPostCommentResponse> CreateTeamPostCommentAsync(int teamId, int postId, int currentUserId, CreateTeamPostCommentRequest request);
        // 프로젝트 종료
        Task<EndProjectResponse> EndProjectAsync(int teamId, int currentUserId);
        // 팀원 역할 변경
        Task<UpdateTeamRolesResponse> UpdateTeamRolesAsync(int teamId, int currentUserId, UpdateTeamRolesRequest request);
        // 팀원 가능 시간 설정
        Task<SetAvailableTimesResponse> SetAvailableTimesAsync(int teamId, int currentUserId, SetAvailableTimesRequest request);
    }
}


