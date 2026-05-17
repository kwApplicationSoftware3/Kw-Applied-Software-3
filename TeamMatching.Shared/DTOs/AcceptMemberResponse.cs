using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class AcceptMemberResponse
    {
        public bool IsSuccess { get; set; }// 성공 여부
        public string Message { get; set; } = string.Empty; // 결과 메시지
    }
}
