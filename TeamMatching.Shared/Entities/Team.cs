using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamMatching.Shared.Entities
{
    public class Team
    {
        [Key]
        public int Id { get; set; } // 팀 고유 번호

        [Required]
        public int PostId { get; set; } // 모집글 ID (이 글을 통해 결성됨)
        
        [ForeignKey("PostId")]
        public Post? Post { get; set; } // 원본 모집글 정보

        [Required]
        public string TeamName { get; set; } = string.Empty; // 확정된 팀 이름

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 팀 생성 일시

        // 관계 설정
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>(); // 소속 팀원 목록
    }
}
