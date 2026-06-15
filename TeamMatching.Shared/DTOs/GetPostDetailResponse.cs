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
        public List<int> SelectedTagIds { get; set; } = new(); // 선택된 기술 스택 ID 목록
        public string Content { get; set; } = string.Empty;    // 게시글 내용
        public string Summary { get; set; } = string.Empty;    // 요약
        public string? Category { get; set; }                  // 분야
        public DateTime CreatedAt { get; set; } // 작성 시간
        public DateTime? UpdatedAt { get; set; } // 수정 시간
        public int ApplicationCount { get; set; } // 지원자 수
        public bool IsMyPost { get; set; }                     // 본인 작성 여부
        public bool IsClosed { get; set; }                     // 모집 마감 여부
        public List<TeamMemberRolePositionDto>? TeamMembers { get; set; } // 팀원 정보
    }
}
