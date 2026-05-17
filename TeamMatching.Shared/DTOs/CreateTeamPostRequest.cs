using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class CreateTeamPostRequest
    {
        //게시글 제목
        [Required(ErrorMessage = "게시글 제목을 입력해주세요.")]
        [MaxLength(100, ErrorMessage = "제목은 100자를 넘을 수 없습니다.")]
        public string Title { get; set; } = string.Empty;

        //게시즐 상세 본문
        [Required(ErrorMessage = "게시글 내용을 입력해주세요.")]
        public string Content { get; set; } = string.Empty;
    }
}
