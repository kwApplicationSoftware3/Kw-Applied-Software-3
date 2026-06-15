using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class TeamPostListItemDto
    {
        // 팀 게시글의 고유 번호
        public int TeamPostId { get; set; }
        // 게시글 제목
        public string Title { get; set; } = string.Empty;
        // 등록 시간
        public DateTime CreatedAt { get; set; }
    }
    public class TeamMemberRoleDto
    {
        // 사용자 식별자
        public int TeamMemberId { get; set; }
        // 팀원 닉네임
        public string Nickname { get; set; } = string.Empty ;
        // 역할
        public TeamRole Role { get; set; }
        // 담당 직무
        public string? Position { get; set; }
    }
    
    public class TeamScheduleSlotDto
    {
        public DateTime AvailableTime { get; set; }
        public int Count { get; set; } // 선택한 멤버 수
        public List<string> AvailableMemberNames { get; set; } = new(); // 선택한 멤버들의 닉네임
    }

    public class GetTeamMainResponse
    {
        // 성공 여부
        public bool IsSuccess { get; set; }
        // 결과 메시지
        public string Message { get; set; } = string.Empty;

        /// 현재 소속되어 작업 중인 프로젝트 팀 명칭 (예: "응소실 3조")
        public string TeamName { get; set; } = string.Empty;

        // 최신 게시글 목록
        public List<TeamPostListItemDto> TeamPosts { get; set; } = new();

        // 팀원 목록
        public List<TeamMemberRoleDto> TeamMemberRoles { get; set; } = new();

        // 시간표 정보
        public List<TeamScheduleSlotDto> TotalSchedules { get; set; } = new();

        // 내가 선택한 가능 시간 목록
        public List<DateTime> MySchedules { get; set; } = new();
    }
}
