using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    // 인증 관련 비즈니스 로직 인터페이스
    public interface IAuthService
    {
        // 회원가입 처리
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        // 로그인 처리
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
