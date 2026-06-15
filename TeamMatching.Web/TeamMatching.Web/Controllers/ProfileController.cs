using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace TeamMatching.Web.Controllers
{
    // 내 프로필 관련 API 컨트롤러
    [ApiController]
    [Route("api/[controller]")] // api/profile
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService ProfileService)
        {
            _profileService = ProfileService;
        }
        
        // 프로필 불러오기 엔드포인트
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<GetProfileResponse>> GetProfile()
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new GetProfileResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // 사용자 식별자 추출
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

        // 프로필 업데이트 엔드포인트
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdateProfileResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized(new UpdateProfileResponse { IsSuccess = false, Message = "사용자 인증 정보가 유효하지 않습니다." });
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
