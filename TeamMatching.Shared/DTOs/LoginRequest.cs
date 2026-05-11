using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.DTOs
{
    /// <summary>
    /// 로그인 요청 정보를 담는 DTO
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "이메일은 필수 입력 항목입니다.")]
        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수 입력 항목입니다.")]
        public string Password { get; set; } = string.Empty;
    }
}
