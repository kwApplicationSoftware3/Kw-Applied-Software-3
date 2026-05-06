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
        public string Nickname { get; set; } = string.Empty;

        public string? Bio { get; set; } // 자기소개

        public List<int> SelectedTagIds { get; set; } = new List<int>(); // 선택한 기술 스택 태그 ID 목록

        public string profileImageUrl { get; set; } = string.Empty; // 프로필 이미지 경로
    }
}
