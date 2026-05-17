using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.Entities
{
    //팀 내부 전용 게시판   
    public class TeamPost
    {
        [Key]
        public int Id { get; set; } // 데이터베이스 고유 번호 (PK - DTO의 teamPostId로 변환됨)

        [Required]
        public int TeamId { get; set; } // 게시글이 소속된 팀의 외래키 (FK)

        [ForeignKey("TeamId")]
        public Team? Team { get; set; } // 소속 팀과의 관계 설정을 위한 네비게이션 프로퍼티

        //본인 글 여부(isMyPost)를 판단하기 위해 작성자 유저 ID를 추가합니다.
        [Required]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User? Author { get; set; }
        
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
