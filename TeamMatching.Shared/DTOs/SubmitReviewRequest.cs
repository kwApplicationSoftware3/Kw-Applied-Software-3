using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Shared.DTOs
{
    public class ReviewDto
    {
        public int UserId { get; set; } // 팀원 ID

        public float ReliabilityScore { get; set; } // 신뢰도 점수

        public float ContributionScore { get; set; } // 기여도 점수

        public float CommunicationScore { get; set; } // 소통 점수
    }
    public class SubmitReviewRequest
    {
        public int PostId { get; set; } // 모집글 ID
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>(); // 팀원 평가 리스트
    }
}
