using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;

namespace TeamMatching.Web.Controllers
{
    /// <summary>
    /// 인증 관련 API 컨트롤러
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // api/posts
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService PostsService)
        {
            _postsService = PostsService;
        }
        /// <summary>
        /// 게시글 목록 조회 엔드포인트
        /// </summary>
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
        /// <summary>
        /// 게시글 작성 엔드포인트
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreatePostResponse>> CreatePost([FromBody] CreatePostRequest request)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreatePostResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
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
        /// <summary>
        /// 모집글 상세 조회 API
        /// URL: GET /api/posts/{id} (예: /api/posts/105)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPostDetailResponse>> GetPostDetail(int id)
        {
            // 1. 기존 로그인/작성 구조처럼 Claims에서 유저 ID를 안전하게 추출 시도
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            int? currentUserId = null;

            // 토큰이 유효하여 ID가 존재하면 currentUserId에 값을 넣음 (비로그인이면 null 유지)
            if (idClaim != null && int.TryParse(idClaim.Value, out var parsedId))
            {
                currentUserId = parsedId;
            }

            // 2. 서비스 호출
            var result = await _postsService.GetPostDetailAsync(id, currentUserId);

            // 3. 응답 반환
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return NotFound(result); // 데이터를 못 찾은 경우 HTTP 404 표준 반환
        }
        
        // 특정 모집글의 지원자 목록 조회 API
        [HttpGet("{postId}/applications")]
        public async Task<ActionResult<GetApplicationsResponse>> GetApplications(int postId)
        {
            // 서비스 호출
            var result = await _postsService.GetApplicationsByPostIdAsync(postId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 목록이 비어있거나 오류가 난 경우에도 규격에 맞게 반환
            return BadRequest(result);
        }
        
        /// 모집글 삭제 API
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletePostResponse>> DeletePost(int id)
        {
            // 1. JWT 토큰에서 로그인한 유저의 고유 ID를 꺼냅니다. (CreatePost와 동일한 방식)
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

            // 토큰 정보가 이상하거나 ID를 읽을 수 없으면 튕겨냅니다.
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new DeletePostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            // 2. 서비스 로직 호출 (글 번호와 로그인한 사람의 번호를 같이 넘겨 권한을 체크하게 함)
            var result = await _postsService.DeletePostAsync(id, currentUserId);

            // 3. 응답 반환
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 권한이 없거나 글이 없는 경우 400 에러를 반환합니다.
            return BadRequest(result);
        }
        // 모집글 지원하기 API
        [Authorize]
        [HttpPost("{id}/apply")]
        public async Task<ActionResult<ApplyPostResponse>> ApplyPost(int id, [FromBody] ApplyPostRequest request)
        {
            // 1. 모델 유효성 검사
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApplyPostResponse { IsSuccess = false, Message = "메시지를 입력해주세요." });
            }

            // 2. 토큰에서 현재 로그인한 유저 ID 추출 (기존 방식 유지)
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new ApplyPostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            // 3. 서비스 호출
            var result = await _postsService.ApplyPostAsync(id, userId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        // 모집글 수정 API
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdatePostResponse>> UpdatePost(int id, [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdatePostResponse { IsSuccess = false, Message = "입력 데이터가 올바르지 않습니다." });
            }

            // 토큰에서 유저 ID 추출 (기존 방식 유지)
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
    }
}
