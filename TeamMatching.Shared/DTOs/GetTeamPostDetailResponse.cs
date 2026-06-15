using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class TeamPostCommentDto
    {
        // 닉네임
        public string Nickname { get; set; } = string.Empty;
        // 댓글 내용
        public string Content { get; set; } = string.Empty;
        // 댓글이 작성된 일시
        public DateTime CreatedAt { get; set; }
    }
 
    public class GetTeamPostDetailResponse
    {
        // 성공 여부
        public bool IsSuccess { get; set; }
        // 결과 메시지
        public string Message { get; set; } = string.Empty;
        // 상세 정보
        public string Title { get; set; } = string.Empty;
        // 게시글 내용
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } // 작성 시간
        public DateTime? UpdatedAt { get; set; } // 수정 시간

        // 본인 작성 여부
        public bool IsMyPost { get; set; }
        // 댓글 목록
        public List<TeamPostCommentDto> Comments { get; set; } = new();
        // 작성자 닉네임
        public string NickName { get; set; } = string.Empty;
    }
}
