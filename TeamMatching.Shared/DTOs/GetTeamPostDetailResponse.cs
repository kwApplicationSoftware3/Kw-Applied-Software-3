using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class TeamPostCommentDto
    {
        // 댓글 작성자의 활동 닉네임 (예: "강은우", "문인호")
        public string Nickname { get; set; } = string.Empty;
        // 댓글 본문 내용 (예: "찬성합니다.", "반대합니다.")
        public string Content { get; set; } = string.Empty;
        // 댓글이 작성된 일시
        public DateTime CreatedAt { get; set; }
    }
 
    public class GetTeamPostDetailResponse
    {
        // API 요청 처리 성공 여부
        public bool IsSuccess { get; set; }
        // 처리 결과 메시지 ("게시글 상세 정보를 성공적으로 불러왔습니다.")
        public string Message { get; set; } = string.Empty;
        // 핵심 상세 정보 데이터 오브젝트
        public string Title { get; set; } = string.Empty;
        // 게시글 본문 상세 내용
        public string Content { get; set; } = string.Empty;
        // 현재 로그인하여 글을 읽고 있는 세션 유저가 이 글의 원래 작성자인지 여부 플래그
        public bool IsMyPost { get; set; }
        // 이 게시글에 등록된 전체 댓글 리스트 목록 배열
        public List<TeamPostCommentDto> Comments { get; set; } = new();
    }
}
