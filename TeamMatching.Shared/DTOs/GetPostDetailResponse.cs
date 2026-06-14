using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class TeamMemberRolePositionDto
    {
        public string TeamMemberNickname { get; set; } = string.Empty; // 팀원 닉네임
        public TeamRole TeamMemberRole { get; set; }
        public string? TeamMemberPosition { get; set; }
    }

    public class GetPostDetailResponse
    {
        public bool IsSuccess { get; set; }                    // 성공 여부
        public string Message { get; set; } = string.Empty;    // 결과 메시지
        public string Title { get; set; } = string.Empty;      // 글 제목
        public int CurrentMembers { get; set; }                // 현재 참여 인원수
        public int MaxMembers { get; set; }                    // 최대 모집 인원수
        public List<int> SelectedTagIds { get; set; } = new(); // 선택된 기술 스택(태그) ID 목록
        public string Content { get; set; } = string.Empty;    // 글 본문 상세 내용
        public string Summary { get; set; } = string.Empty;    // 모집글 요약 (수정 폼 미리채움용)
        public string? Category { get; set; }                  // 프로젝트 분야 (수정 폼 미리채움용)
        public DateTime CreatedAt { get; set; } // 글 작성 일자
        public DateTime? UpdatedAt { get; set; } // 글 수정 일자
        public int ApplicationCount { get; set; } // 지원서 개수
        public bool IsMyPost { get; set; }                     // 접속한 유저가 이 글의 작성자인지 여부 (true면 수정/삭제 버튼 노출)
        public bool IsClosed { get; set; }                     // 모집 마감 여부
        public List<TeamMemberRolePositionDto>? TeamMembers { get; set; } // 팀이 결성된 경우 팀원 정보
    }
}
