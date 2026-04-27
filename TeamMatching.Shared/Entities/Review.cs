using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamMatching.Shared.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; } // 리뷰 고유 번호

        [Required]
        public int PostId { get; set; } // 프로젝트(모집글) ID
        
        [ForeignKey("PostId")]
        public Post? Post { get; set; } // 프로젝트 정보

        [Required]
        public int ReviewerId { get; set; } // 평가를 작성한 유저 ID
        
        [ForeignKey("ReviewerId")]
        public User? Reviewer { get; set; } // 평가자 정보

        [Required]
        public int RevieweeId { get; set; } // 평가를 받는 유저 ID
        
        [ForeignKey("RevieweeId")]
        public User? Reviewee { get; set; } // 피평가자 정보

        [Range(1, 5)]
        public int ReliabilityScore { get; set; } // 신뢰도 점수 (1~5)

        [Range(1, 5)]
        public int ContributionScore { get; set; } // 기여도 점수 (1~5)

        [Range(1, 5)]
        public int CommunicationScore { get; set; } // 소통 점수 (1~5)

        public string? Comment { get; set; } // 상세 평가 내용

        public bool IsHidden { get; set; } = false; // 관리자 숨김 여부

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 평가 작성 일시
    }
}
