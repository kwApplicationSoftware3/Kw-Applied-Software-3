using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdatePostRequest
    {
        [Required(ErrorMessage = "제목을 입력해주세요.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "상세 내용을 입력해주세요.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "요약 내용을 입력해주세요.")]
        public string Summary { get; set; } = string.Empty;

        public string? Category { get; set; } //프로젝트 분야
        public int MaxMembers { get; set; }

        public List<int> SelectedTagIds { get; set; } = new(); // 수정된 기술 스택 ID 목록
    }
}
