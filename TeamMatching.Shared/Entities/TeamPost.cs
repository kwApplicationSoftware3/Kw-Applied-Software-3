using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.Entities
{
    public class TeamPost
    {
        [Key]
        public int Id { get; set; } // 팀 게시글 고유 번호

        [Required]
        public int TeamId { get; set; } // 게시글이 소속된 팀의 외래키

        [ForeignKey("TeamId")]
        public Team? Team { get; set; } // 소속 팀과의 관계 설정을 위한 네비게이션 프로퍼티

        [Required]
        public int AuthorId { get; set; } // 작성자 ID

        [ForeignKey("AuthorId")]
        public User? Author { get; set; } // 작성자 정보

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty; // 게시글 제목

        [Required]
        public string Content { get; set; } = string.Empty; // 게시글 본문 상세 내용

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 게시글 작성 일시

        public DateTime? UpdatedAt { get; set; } // 수정 일시

        // 관계 설정
        public ICollection<TeamPostComment> TeamPostComments { get; set; } = new List<TeamPostComment>(); // 게시글에 달린 댓글 목록
    }
}
