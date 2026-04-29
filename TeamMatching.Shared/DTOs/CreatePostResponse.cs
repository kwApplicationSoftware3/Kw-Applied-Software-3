namespace TeamMatching.Shared.DTOs
{
    public class CreatePostResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부
        
        public string Message { get; set; } = string.Empty; // 결과 메시지
    }
}
