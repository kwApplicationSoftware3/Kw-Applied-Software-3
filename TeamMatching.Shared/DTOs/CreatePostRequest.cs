using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.DTOs
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "프로젝트 제목을 입력해주세요.")]
        public string Title { get; set; } = string.Empty; // 제목
        
        [Required(ErrorMessage = "상세 설명을 입력해주세요.")]
        public string Content { get; set; } = string.Empty; // 글 상세 내용
        
        [Required(ErrorMessage = "미리보기를 입력해주세요.")]
        public string Summary {  get; set; } = string.Empty; // 글 내용 미리보기

        public string? Category { get; set; } // 프로젝트 분야

        public int MaxMembers { get; set; } = 2; // 모집 인원

        public List<int> SelectedTagIds { get; set; } = new List<int>(); // 필요 기술 스택
    }
}
