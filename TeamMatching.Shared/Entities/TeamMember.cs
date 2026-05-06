using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.Entities
{
    public class TeamMember
    {
        [Key]
        public int Id { get; set; } // 팀 멤버 고유 번호

        [Required]
        public int TeamId { get; set; } // 소속 팀 ID
        
        [ForeignKey("TeamId")]
        public Team? Team { get; set; } // 팀 정보

        [Required]
        public int UserId { get; set; } // 팀원 ID
        
        [ForeignKey("UserId")]
        public User? User { get; set; } // 팀원 유저 정보

        public TeamRole Role { get; set; } = TeamRole.Member; // 팀 내 역할 (팀장/팀원)

        [MaxLength(30)]
        public string? Position { get; set; } // 팀 내 구체적 역할 (백엔드, 프론트엔드, 디자이너 등)
    }
}
