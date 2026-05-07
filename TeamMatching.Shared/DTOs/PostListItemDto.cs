using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class PostListItemDto
    {
        /// 게시글 목록 조회 시 반환되는 게시글 아이템 정보
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public int CurrentMembers { get; set; }
        public int MaxMembers { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
