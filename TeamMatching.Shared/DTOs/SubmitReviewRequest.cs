using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Shared.DTOs
{
    public class ReviewDto
    {
        [Required]
        public int UserId { get; set; } // 팀원 ID

        [Range(1, 5, ErrorMessage = "점수는 1~5점 사이여야 합니다.")]
        public float ReliabilityScore { get; set; } // 신뢰도 점수

        [Range(1, 5, ErrorMessage = "점수는 1~5점 사이여야 합니다.")]
        public float ContributionScore { get; set; } // 기여도 점수

        [Range(1, 5, ErrorMessage = "점수는 1~5점 사이여야 합니다.")]
        public float CommunicationScore { get; set; } // 소통 점수
    }
    public class SubmitReviewRequest
    {
        [Required]
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>(); // 평가 목록
    }
}
