using TeamMatching.Shared.DTOs;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 인증 관련 비즈니스 로직 인터페이스
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 회원가입 처리
        /// </summary>
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
