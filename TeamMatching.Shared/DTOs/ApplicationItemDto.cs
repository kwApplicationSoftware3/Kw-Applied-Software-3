using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class ApplicationItemDto
    {
        public int ApplicationId { get; set; }             // 지원서 고유 ID
        public string Nickname { get; set; } = string.Empty; // 지원자 닉네임
        public string Message { get; set; } = string.Empty;  // 지원 메시지 (자기소개 등)
        public List<int> SelectedTagIds { get; set; } = new(); // 지원자가 보유한 기술 스택 ID 목록
        public DateTime CreatedAt { get; set; }              // 지원 일시
    }
}
