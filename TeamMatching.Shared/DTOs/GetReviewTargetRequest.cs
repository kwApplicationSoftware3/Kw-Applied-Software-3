using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Shared.DTOs
{
    public class GetReviewTargetRequest
    {
        public int TeamId { get; set; } // 평가할 팀 ID
    }
}
