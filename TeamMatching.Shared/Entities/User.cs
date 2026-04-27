using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; } // 유저 고유 번호

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // 이메일 (아이디)

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // 암호화된 비밀번호

        [Required]
        public string Nickname { get; set; } = string.Empty; // 활동 닉네임

        public string? Department { get; set; } // 소속 학과
        
        public string? StudentId { get; set; } // 학번

        public string? ProfileImageUrl { get; set; } // 프로필 이미지 경로

        public string? Bio { get; set; } // 자기소개 한 줄

        public float ReliabilityScore { get; set; } = 0; // 평균 신뢰도 점수
        public float ContributionScore { get; set; } = 0; // 평균 기여도 점수
        public float CommunicationScore { get; set; } = 0; // 평균 소통 점수

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 가입 일시
        public DateTime? UpdatedAt { get; set; } // 정보 수정 일시

        // 관계 설정
        public ICollection<UserTag> UserTags { get; set; } = new List<UserTag>(); // 보유 기술 스택
        public ICollection<Post> MyPosts { get; set; } = new List<Post>(); // 내가 작성한 글
    }
}
