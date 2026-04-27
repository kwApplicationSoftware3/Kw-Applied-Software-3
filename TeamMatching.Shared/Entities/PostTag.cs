namespace TeamMatching.Shared.Entities
{
    public class PostTag
    {
        public int PostId { get; set; } // 모집글 ID
        public Post? Post { get; set; } // 모집글 정보

        public int TagId { get; set; } // 태그(기술) ID
        public Tag? Tag { get; set; } // 태그 정보
    }
}
