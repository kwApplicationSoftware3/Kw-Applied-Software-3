using System.ComponentModel.DataAnnotations;
using TeamMatching.Shared.Entities;
using TeamMatching.Shared.Enums;

namespace TeamMatching.Shared.DTOs
{
    public class MyPostDto
    {
        public int PostId { get; set; }

        public string Title { get; set; } = string.Empty;

        public PostStatus Status { get; set; }

        public int Applications { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class MyApplicationDto
    {
        public int PostId { get; set; }

        public string Nickname { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ActivityTeamDto
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public int PostId { get; set; }
    }

    public class GetMyActivitiesResponse
    {
        public bool IsSuccess { get; set; } // 성공 여부

        public string Message { get; set; } = string.Empty; // 결과 메시지

        public List<MyPostDto> MyPosts { get; set; } = new List<MyPostDto>(); // 내 작성글 목록

        public List<MyApplicationDto> MyApplications { get; set; } = new List<MyApplicationDto>(); // 내 지원서 목록

        public List<ActivityTeamDto> MyTeams { get; set; } = new List<ActivityTeamDto>(); // 내 팀 목록
    }
}
