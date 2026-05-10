using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class ApplyPostRequest
    {
        [Required(ErrorMessage = "지원 메시지를 입력해주세요.")]
        public string Message { get; set; } = string.Empty; // "C# 프로젝트 벡엔드 지원합니다!"
    }
}
