using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamNameResponse
    {
        //팀명 변경 성공 여부 플래그
        public bool IsSuccess { get; set; }
        //처리 결과 안내 또는 에러 메시지
        public string Message { get; set; } = string.Empty;
    }
}
