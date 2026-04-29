using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.DTOs
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "프로젝트 제목을 입력해주세요.")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "상세 설명을 입력해주세요.")]
        public string Content { get; set; } = string.Empty;

        public string? Category { get; set; }

        public int MaxMembers { get; set; } = 2;

        public List<int> SelectedTagIds { get; set; } = new List<int>();
    }
}
