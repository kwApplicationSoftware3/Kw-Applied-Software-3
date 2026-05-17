using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class AcceptMemberRequest
    {
        [Required]
        public int ApplicationId { get; set; } // 지원서 ID
        
        [Required]
        [AllowedValues(ApplicationStatus.Accepted, ApplicationStatus.Rejected, ErrorMessage = "수락 혹은 거절 상태만 전달할 수 있습니다.")]
        public ApplicationStatus Status { get; set; } // 수락/거절
    }
}
