using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class GetPostDetailResponse
    {
        public bool IsSuccess { get; set; }                    // 성공 여부
        public string Message { get; set; } = string.Empty;    // 결과 메시지
        public string Title { get; set; } = string.Empty;      // 글 제목
        public int CurrentMembers { get; set; }                // 현재 참여 인원수
        public int MaxMembers { get; set; }                    // 최대 모집 인원수
        public List<int> SelectedTagIds { get; set; } = new(); // 선택된 기술 스택(태그) ID 목록
        public string Content { get; set; } = string.Empty;    // 글 본문 상세 내용
        public bool IsMyPost { get; set; }                     // 접속한 유저가 이 글의 작성자인지 여부 (true면 수정/삭제 버튼 노출)
    }
}
