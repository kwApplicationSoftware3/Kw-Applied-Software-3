using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 인증 관련 비즈니스 로직 인터페이스
    /// </summary>
    public interface IPostsService
    {
        /// <summary>
        /// 글 작성 처리
        /// </summary>
        Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, int authorId);
        Task<GetPostsResponse> GetPostsAsync();

        // 특정 게시글의 상세 정보를 조회
        Task<GetPostDetailResponse> GetPostDetailAsync(int postId, int? currentUserId);
        // 특정 모집글에 들어온 지원서 목록을 조회
        Task<GetApplicationsResponse> GetApplicationsByPostIdAsync(int postId);
   
        // 특정 모집글을 삭제 (작성자 본인만 삭제 가능)
        Task<DeletePostResponse> DeletePostAsync(int postId, int currentUserId);
        // 특정 모집글에 지원서를 제출합니다.
        Task<ApplyPostResponse> ApplyPostAsync(int postId, int userId, ApplyPostRequest request);
        // 기존 모집글을 수정합니다. (작성자 본인만 가능)
        Task<UpdatePostResponse> UpdatePostAsync(int postId, int userId, UpdatePostRequest request);
    }
}
