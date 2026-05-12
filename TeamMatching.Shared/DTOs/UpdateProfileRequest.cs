using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "닉네임은 필수 입력 항목입니다.")]
        public string Nickname { get; set; } = string.Empty;

        public string? Bio { get; set; } // 자기소개

        public List<int> SelectedTagIds { get; set; } = new List<int>(); // 선택한 기술 스택 태그 ID 목록

        public string? ProfileImageUrl { get; set; } // 프로필 이미지 경로

        public string? NewPassword {  get; set; } // 새로운 비밀번호

        [Required(ErrorMessage = "기존 비밀번호를 입력해주세요.")]
        public string OldPassword { get; set; } = string.Empty; // 기존 비밀번호
    }
}