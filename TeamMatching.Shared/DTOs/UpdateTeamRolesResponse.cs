using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamRolesResponse
    {
        // 처리 성공 여부
        public bool IsSuccess { get; set; }
        // 처리 결과 메시지 ("역할 변경이 완료되었습니다.")
        public string Message { get; set; } = string.Empty;
    }
}
