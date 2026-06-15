using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;

namespace TeamMatching.Web.Controllers
{
    // 인증 관련 API 컨트롤러
    [ApiController]
    [Route("api/[controller]")] // api/posts
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService PostsService)
        {
            _postsService = PostsService;
        }
        
        // 글 목록 조회 엔드포인트
        [HttpGet]
        public async Task<ActionResult<GetPostsResponse>> GetPosts()
        {
            var result = await _postsService.GetPostsAsync();

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        // 글 작성 처리 엔드포인트
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreatePostResponse>> CreatePost([FromBody] CreatePostRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreatePostResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var authorId))
            {
                return Unauthorized(new CreatePostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _postsService.CreatePostAsync(request, authorId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 특정 게시글의 상세 정보 조회 엔드포인트
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPostDetailResponse>> GetPostDetail(int id)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            int? currentUserId = null;

            // 식별자 할당
            if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
            {
                currentUserId = parsedId;
            }

            // 비즈니스 로직 실행
            var result = await _postsService.GetPostDetailAsync(id, currentUserId);

            // 결과 반환
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return NotFound(result); // 예외 응답 반환
        }

        // 특정 모집글에 들어온 지원서 목록 조회 엔드포인트
        [HttpGet("{postId}/applications")]
        public async Task<ActionResult<GetApplicationsResponse>> GetApplications(int postId)
        {
            // 비즈니스 로직 실행
            var result = await _postsService.GetApplicationsByPostIdAsync(postId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 예외 응답 규격화 반환
            return BadRequest(result);
        }

        // 모집글 삭제 엔드포인트
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletePostResponse>> DeletePost(int id)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            // 인증 실패 반환
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new DeletePostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            // 비즈니스 로직 실행
            var result = await _postsService.DeletePostAsync(id, currentUserId);

            // 결과 반환
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 모집글 지원서 제출 엔드포인트
        [Authorize]
        [HttpPost("{id}/apply")]
        public async Task<ActionResult<ApplyPostResponse>> ApplyPost(int id, [FromBody] ApplyPostRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApplyPostResponse { IsSuccess = false, Message = "메시지를 입력해주세요." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new ApplyPostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            // 비즈니스 로직 실행
            var result = await _postsService.ApplyPostAsync(id, userId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 기존 모집글 수정 엔드포인트
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdatePostResponse>> UpdatePost(int id, [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdatePostResponse { IsSuccess = false, Message = "입력 데이터가 올바르지 않습니다." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new UpdatePostResponse { IsSuccess = false, Message = "인증 정보가 유효하지 않습니다." });
            }

            var result = await _postsService.UpdatePostAsync(id, userId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 팀원 수락/거절 엔드포인트
        [Authorize]
        [HttpPost("{id}/applications")]
        public async Task<ActionResult<AcceptMemberResponse>> AcceptMember(int id, [FromBody] AcceptMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AcceptMemberResponse { IsSuccess = false, Message = "입력 데이터가 올바르지 않습니다." });
            }

            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new AcceptMemberResponse { IsSuccess = false, Message = "인증 정보가 유효하지 않습니다." });
            }

            var result = await _postsService.AcceptMemberAsync(id, userId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
