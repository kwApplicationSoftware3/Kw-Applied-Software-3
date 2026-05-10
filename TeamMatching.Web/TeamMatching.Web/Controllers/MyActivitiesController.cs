using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace TeamMatching.Web.Controllers
{
    /// <summary>
    /// 내 활동 관련 API 컨트롤러
    /// </summary>
    [ApiController]
    [Route("api/my-activities")] // api/my-activities
    public class MyActivitiesController : ControllerBase
    {
        private readonly IMyActivitiesService _myActivitiesService;

        public MyActivitiesController(IMyActivitiesService MyActivitiesService)
        {
            _myActivitiesService = MyActivitiesService;
        }

        // 내 활동 관리 엔드포인트
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<GetMyActivitiesResponse>> GetMyActivities()
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new GetMyActivitiesResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new GetMyActivitiesResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _myActivitiesService.GetMyActivitiesAsync(userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 모집 종료 엔드포인트
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ClosePostResponse>> ClosePost([FromBody] ClosePostRequest request)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ClosePostResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new ClosePostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _myActivitiesService.ClosePostAsync(request, userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 팀원 평가 엔드포인트
        [Authorize]
        [HttpGet("review/{teamId}")]
        public async Task<ActionResult<GetReviewTargetResponse>> GetReviewTarget(int teamId)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new GetReviewTargetResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new GetReviewTargetResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _myActivitiesService.GetReviewTargetAsync(teamId, userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 팀원 평가 제출 엔드포인트
        [Authorize]
        [HttpPost("review/{teamId}")]
        public async Task<ActionResult<SubmitReviewResponse>> SubmitReview(int teamId, [FromBody] SubmitReviewRequest request)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new SubmitReviewResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new SubmitReviewResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }
           
            var result = await _myActivitiesService.SubmitReviewAsync(teamId, request, userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
