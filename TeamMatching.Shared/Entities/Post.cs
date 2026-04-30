using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; } // 모집글 고유 번호

        [Required]
        public int AuthorId { get; set; } // 작성자 ID
        
        [ForeignKey("AuthorId")]
        public User? Author { get; set; } // 작성자 정보

        [Required]
        public string Title { get; set; } = string.Empty; // 글 제목

        [Required]
        public string Content { get; set; } = string.Empty; // 글 내용
        
        [Required]
        public string Summary { get; set; } = string.Empty ; // 글 내용 요약

        public string? Category { get; set; } // 프로젝트 카테고리 (웹, 앱 등)

        public PostStatus Status { get; set; } = PostStatus.Recruiting; // 모집 상태 (모집중/진행중/완료)

        public int MaxMembers { get; set; } = 2; // 모집 정원

        public int CurrentMembers { get; set; } = 1; // 현재 모집 인원수

        public string? ContactUrl { get; set; } // 오픈카톡 등 연락 링크

        public int ViewCount { get; set; } = 0; // 조회수

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 작성 일시
        public DateTime? UpdatedAt { get; set; } // 수정 일시

        // 관계 설정
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>(); // 필요 기술 스택
        public ICollection<Application> Applications { get; set; } = new List<Application>(); // 지원서 목록
    }
}
