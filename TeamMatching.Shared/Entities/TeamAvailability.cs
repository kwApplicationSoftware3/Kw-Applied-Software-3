using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamMatching.Shared.Entities
{
    public class  TeamAvailability
    {
        [Key]
        public int Id { get; set; } // 가능 시간 고유 번호

        [Required]
        public int TeamId { get; set; } // 소속 팀 ID

        [ForeignKey("TeamId")]
        public Team? Team { get; set; } // 팀 정보

        [Required]
        public int MemberId { get; set; } // 팀원 ID

        [ForeignKey("MemberId")]
        public TeamMember? Member { get; set; } // 팀원 정보

        [Required]
        public DateTime SlotStart { get; set; } // 시간 정보

        public DateTime CreatedAt { get; set; } // 생성 시간
    }
}
