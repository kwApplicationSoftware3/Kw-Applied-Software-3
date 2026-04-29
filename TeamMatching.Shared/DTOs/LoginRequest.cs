using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "이메일을 입력해주세요.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        public string Password { get; set; }
    }
}
