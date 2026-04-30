namespace TeamMatching.Shared.DTOs
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부
        
        public string Message { get; set; } = string.Empty; // 결과 메시지

        public string? AccessToken { get; set; } // JWT 액세스 토큰

        public DateTime? ExpiresAt { get; set; } // 액세스 토큰 만료 시간 (UTC)
    }
}
