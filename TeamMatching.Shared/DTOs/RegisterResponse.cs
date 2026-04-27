namespace TeamMatching.Shared.DTOs
{
    /// <summary>
    /// 회원가입 응답 DTO
    /// </summary>
    public class RegisterResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부
        public string Message { get; set; } = string.Empty; // 결과 메시지
    }
}
