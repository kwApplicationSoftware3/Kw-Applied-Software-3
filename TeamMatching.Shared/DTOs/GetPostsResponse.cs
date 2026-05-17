using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

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
        public List<string> Tags { get; set; } = new();
        public PostStatus Status { get; set; }
    }

    public class GetPostsResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<PostListItemDto> Data { get; set; } = new List<PostListItemDto>();
    }
}
