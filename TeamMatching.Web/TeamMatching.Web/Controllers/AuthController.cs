using Microsoft.AspNetCore.Mvc;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;
using System.Threading.Tasks;

namespace TeamMatching.Web.Controllers
{
    /// <summary>
    /// 인증 관련 API 컨트롤러
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 회원가입 엔드포인트
        /// </summary>
        [HttpPost("signup")]
        public async Task<ActionResult<RegisterResponse>> SignUp(RegisterRequest request)
        {
            // 모델 유효성 검사 (DTO의 [Required] 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }

            var result = await _authService.RegisterAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// 로그인 엔드포인트
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { IsSuccess = false, Message = "입력 데이터 형식이 올바르지 않습니다." });
            }

            var result = await _authService.LoginAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 인증 실패는 401 반환이 자연스럽습니다.
            return Unauthorized(result);
        }
    }
}
