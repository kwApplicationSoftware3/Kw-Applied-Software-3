using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Shared.DTOs
{
    public class ProfileTeamDto
    {
        public string TeamName { get; set; }

        public string Role { get; set; }
    }
    public class GetProfileResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부

        public string Message { get; set; } = string.Empty; // 결과 메시지

        [Required]
        public string Nickname { get; set; } = string.Empty; // 활동 닉네임

        public string? ProfileImageUrl { get; set; } // 프로필 이미지 경로

        public string? Bio { get; set; } // 자기소개 한 줄

        public float ReliabilityScore { get; set; } = 0; // 평균 신뢰도 점수
        public float ContributionScore { get; set; } = 0; // 평균 기여도 점수
        public float CommunicationScore { get; set; } = 0; // 평균 소통 점수

        public List<int> UserTagIds { get; set; } = new List<int>(); // 보유 기술 스택
        public List<ProfileTeamDto> MyTeams { get; set; } = new List<ProfileTeamDto>(); // 최근 참여 프로젝트
    }
}
