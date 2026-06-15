using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class ApplicationItemDto
    {
        public int ApplicationId { get; set; }             // 지원서 고유 ID
        public string Nickname { get; set; } = string.Empty; // 지원자 닉네임
        public string? Bio { get; set; }                     // 지원자 기본 자기소개
        public string Message { get; set; } = string.Empty;  // 지원 메시지
        public List<int> SelectedTagIds { get; set; } = new(); // 보유 기술 스택
        public DateTime CreatedAt { get; set; }              // 지원 일시
        public ApplicationStatus Status { get; set; }        // 지원 상태
        public float ReliabilityScore { get; set; }          // 신뢰도 점수
        public float ContributionScore { get; set; }         // 기여도 점수
        public float CommunicationScore { get; set; }        // 소통 점수
    }

    public class GetApplicationsResponse
    {
        public bool IsSuccess { get; set; }                     // 성공 여부
        public string Message { get; set; } = string.Empty;     // 결과 메시지
        public List<ApplicationItemDto> Applications { get; set; } = new();
    }
}
