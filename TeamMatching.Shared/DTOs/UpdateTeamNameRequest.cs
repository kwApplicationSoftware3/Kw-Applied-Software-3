using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamNameRequest
    {
        [Required(ErrorMessage = "변경할 팀 이름을 입력해주세요.")]
        [MaxLength(50, ErrorMessage = "팀 이름은 50자를 초과할 수 없습니다.")]
        public string TeamName { get; set; } = string.Empty;
    }
}
