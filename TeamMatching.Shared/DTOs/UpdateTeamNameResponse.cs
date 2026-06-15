using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamNameResponse
    {
        // 성공 여부
        public bool IsSuccess { get; set; }
        // 결과 메시지
        public string Message { get; set; } = string.Empty;
    }
}
