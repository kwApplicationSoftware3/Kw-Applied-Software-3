using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class TeamMemberDto
    {
        public int UserId { get; set; } // 팀원 ID
        public string Nickname { get; set; } = string.Empty; // 팀원 닉네임
        public TeamRole Role { get; set; } = TeamRole.Member;// 팀 내 역할
        public string? Position { get; set; } // 팀 내 구체적 역할
    }
    public class GetReviewTargetResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부

        public string Message { get; set; } = string.Empty; // 결과 메시지

        public int PostId { get; set; } // 모집글 ID

        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>(); // 팀원 정보
    }
}
