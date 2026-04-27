using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.Entities
{
    public class Application
    {
        [Key]
        public int Id { get; set; } // 지원 고유 번호

        [Required]
        public int PostId { get; set; } // 지원한 모집글 ID
        
        [ForeignKey("PostId")]
        public Post? Post { get; set; } // 모집글 정보

        [Required]
        public int UserId { get; set; } // 지원자 ID
        
        [ForeignKey("UserId")]
        public User? User { get; set; } // 지원자 정보

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending; // 지원 상태 (대기/수락/거절)

        public string? Message { get; set; } // 지원 한마디 (자기소개 등)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 지원 일시
    }
}
