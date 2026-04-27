namespace TeamMatching.Shared.Entities
{
    public class UserTag
    {
        public int UserId { get; set; } // 유저 ID
        public User? User { get; set; } // 유저 정보

        public int TagId { get; set; } // 태그(기술) ID
        public Tag? Tag { get; set; } // 태그 정보
    }
}
