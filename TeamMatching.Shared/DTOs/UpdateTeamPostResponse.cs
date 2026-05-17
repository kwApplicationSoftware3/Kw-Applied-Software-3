using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamPostResponse
    {
        // 수정 성공 여부
        public bool IsSuccess { get; set; }
        // 처리 결과 메시지 ("게시글이 수정되었습니다.")
        public string Message { get; set; } = string.Empty;
    }
}
