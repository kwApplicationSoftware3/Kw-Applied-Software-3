using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamMatching.Shared.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; } // 태그 고유 번호

        [Required]
        public string Name { get; set; } = string.Empty; // 기술 스택 명칭 (예: C#, Java)

        // 관계 설정
        public ICollection<UserTag> UserTags { get; set; } = new List<UserTag>(); // 이 기술을 가진 유저들
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>(); // 이 기술이 필요한 글들
    }
}
