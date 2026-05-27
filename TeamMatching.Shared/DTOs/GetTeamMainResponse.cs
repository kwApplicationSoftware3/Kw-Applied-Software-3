using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums; // main 브랜치에 선언된 TeamRole Enum (Leader, Member) 활용

namespace TeamMatching.Shared.DTOs
{
    public class TeamPostListItemDto
    {
        // 팀 게시글의 고유 번호
        public int TeamPostId { get; set; }
        // 게시글 제목
        public string Title { get; set; } = string.Empty;
        //게시글 등록 시간 
        public DateTime CreatedAt { get; set; }
    }
    public class TeamMemberRoleDto
    {
        // 팀원의 사용자 고유 식별 번호 (main 브랜치의 UserId 기반)
        public int TeamMemberId { get; set; }
        // 팀원 닉네임
        public string Nickname { get; set; } = string.Empty ;
        // 팀 내부 권한 등급 (TeamRole.Leader = 0, TeamRole.Member = 1)
        // 또한 이 상태값에 따라 [팀 관리 설정], [공지 작성] 같은 방장 전용 버튼의 활성화/비활성화를 제어 가능  
        public TeamRole Role { get; set; }
        // 팀 매칭 과정에서 확정된 개별 구체적 개발 직무 (예: "백엔드", "프론트엔드", "UI/UX 디자이너")
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
        //서버 비즈니스 로직 처리 성공 여부 플래그 
        public bool IsSuccess { get; set; }
        // 서버의 가공 결과 안내 또는 검증 실패 사유 텍스트
        public string Message { get; set; } = string.Empty;

        /// 현재 소속되어 작업 중인 프로젝트 팀 명칭 (예: "응소실 3조")
        public string TeamName { get; set; } = string.Empty;

        // 팀 내부 게시판에 등록된 최신 글 리스트 배열 (백엔드 단에서 최신 등록순으로 정렬 후 전송됨)
        public List<TeamPostListItemDto> TeamPosts { get; set; } = new();

        // 현재 팀에 매칭되어 소속된 팀원들의 목록 현황 정보 배열       
        public List<TeamMemberRoleDto> TeamMemberRoles { get; set; } = new();

        // 팀원별 가능 시간 종합 정보
        public List<TeamScheduleSlotDto> TotalSchedules { get; set; } = new();

        // 내(현재 접속 유저)가 선택한 가능 시간 목록
        public List<DateTime> MySchedules { get; set; } = new();
    }
}
