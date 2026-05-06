using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace TeamMatching.Web.Controllers
{
    /// <summary>
    /// 내 프로필 관련 API 컨트롤러
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // api/profile
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService ProfileService)
        {
            _profileService = ProfileService;
        }

        //
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<GetProfileResponse>> GetProfile()
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new GetProfileResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new GetProfileResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _profileService.GetProfileAsync(userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new GetProfileResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // Claims에서 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new CreatePostResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
            }

            var result = await _profileService.UpdateProfileAsync(request, userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
