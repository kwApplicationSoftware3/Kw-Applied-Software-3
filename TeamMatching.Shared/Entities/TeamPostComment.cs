using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.Entities
{
    public class TeamPostComment
    {
        [Key]
        public int Id { get; set; } // 댓글 고유 번호

        [Required]
        public int TeamPostId { get; set; } // 댓글이 달린 게시글 ID

        [ForeignKey("TeamPostId")]
        public TeamPost? TeamPost { get; set; }

        [Required]
        public int UserId { get; set; } // 댓글 작성자 유저 ID

        [ForeignKey("UserId")]
        public User? User { get; set; } // 작성자 상세 정보 (닉네임 추출용)

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty; // 댓글 내용

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 댓글 작성 일시
    }
}
