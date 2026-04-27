using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.DTOs
{
    /// <summary>
    /// 회원가입 요청 DTO
    /// </summary>
    public class RegisterRequest
    {
        [Required(ErrorMessage = "이메일은 필수 입력 항목입니다.")]
        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수 입력 항목입니다.")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상이어야 합니다.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "닉네임은 필수 입력 항목입니다.")]
        public string Nickname { get; set; } = string.Empty;

        public string? Department { get; set; } // 학과

        public string? StudentId { get; set; } // 학번

        public string? Bio { get; set; } // 자기소개

        public List<int> SelectedTagIds { get; set; } = new List<int>(); // 선택한 기술 스택 태그 ID 목록
    }
}
