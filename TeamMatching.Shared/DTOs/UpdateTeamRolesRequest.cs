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
        // 팀원 식별자
        [Required]
        public int TeamMemberId { get; set; }
        // 권한
        [Required]
        public TeamRole Role { get; set; }
        // 담당 직무
        public string? Position { get; set; }
    }
    public class UpdateTeamRolesRequest
    {
        // 변경 팀원 목록
        [Required]
        public List<UpdateTeamRoleDto> TeamRoles { get; set; } = new();
    }
}
