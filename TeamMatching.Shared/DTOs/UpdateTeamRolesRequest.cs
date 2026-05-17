using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class UpdateTeamRoleDto
    {
        // 역할을 변경할 대상 팀원의 식별 번호 (main 브랜치 기준 유저 ID와 매핑됨)
        [Required]
        public int TeamMemberId { get; set; }
        // 변경할 권한 (Leader 또는 Member)
        [Required]
        public TeamRole Role { get; set; }
        // 변경할 직무 포지션 (예: "백엔드", "프론트엔드")
        public string? Position { get; set; }
    }
    public class UpdateTeamRolesRequest
    {
        // 역할이 변경된 팀원들의 목록 배열    
        [Required]
        public List<UpdateTeamRoleDto> TeamRoles { get; set; } = new();
    }
}
