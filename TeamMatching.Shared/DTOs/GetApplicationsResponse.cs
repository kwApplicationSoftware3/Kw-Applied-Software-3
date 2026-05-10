using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class GetApplicationsResponse
    {
        public bool IsSuccess { get; set; }                     // 성공 여부
        public string Message { get; set; } = string.Empty;     // 결과 메시지
        public List<ApplicationItemDto> Applications { get; set; } = new();
    }
}
