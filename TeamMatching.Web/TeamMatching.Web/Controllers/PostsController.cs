using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;
using Microsoft.AspNetCore.Authorization;

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
    }
}
