using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class CreateTeamPostCommentRequest
    {
        // 작성할 댓글의 본문 내용>
        [Required(ErrorMessage = "댓글 내용을 입력해주세요.")]
        [MaxLength(500, ErrorMessage = "댓글은 최대 500자까지 작성 가능합니다.")]
        public string Content { get; set; } = string.Empty;
    }
}
