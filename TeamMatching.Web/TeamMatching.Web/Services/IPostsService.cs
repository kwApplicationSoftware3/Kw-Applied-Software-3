using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    // 모집글 관련 비즈니스 로직 인터페이스
    public interface IPostsService
    {
        // 글 작성 처리
        Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, int authorId);
        // 글 목록 조회
        Task<GetPostsResponse> GetPostsAsync();
        // 특정 게시글의 상세 정보 조회
        Task<GetPostDetailResponse> GetPostDetailAsync(int postId, int? currentUserId);
        // 특정 모집글에 들어온 지원서 목록 조회
        Task<GetApplicationsResponse> GetApplicationsByPostIdAsync(int postId);
        // 모집글 삭제
        Task<DeletePostResponse> DeletePostAsync(int postId, int currentUserId);
        // 모집글 지원서 제출
        Task<ApplyPostResponse> ApplyPostAsync(int postId, int userId, ApplyPostRequest request);
        // 기존 모집글 수정
        Task<UpdatePostResponse> UpdatePostAsync(int postId, int userId, UpdatePostRequest request);
        // 팀원 수락/거절
        Task<AcceptMemberResponse> AcceptMemberAsync(int postId, int currentUserId, AcceptMemberRequest request);
    }
}
