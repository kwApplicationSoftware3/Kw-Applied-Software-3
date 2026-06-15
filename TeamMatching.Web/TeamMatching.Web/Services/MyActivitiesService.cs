using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Shared.Enums;
using TeamMatching.Web.Data;

namespace TeamMatching.Web.Services
{
    // 내 활동 관련 비지니스 로직 구현체
    public class MyActivitiesService : IMyActivitiesService
    {
        private readonly ApplicationDbContext _context;

        public MyActivitiesService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 내 활동 관리
        public async Task<GetMyActivitiesResponse> GetMyActivitiesAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new GetMyActivitiesResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 사용자 확인
                var user = await _context.Users
                    .Include(u => u.MyPosts).ThenInclude(p => p.Applications)
                    .Include(u => u.MyApplications).ThenInclude(a => a.Post).ThenInclude(p => p.Author)
                    .Include(u => u.TeamMemberships).ThenInclude(tm => tm.Team).ThenInclude(t => t.Post) // Post.Status 확인용
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new GetMyActivitiesResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }
                
                // 등록 게시글 목록 조회
                List<MyPostDto> myPosts = new List<MyPostDto>();
                foreach(var post in user.MyPosts)
                {
                    myPosts.Add(new MyPostDto { PostId = post.Id, Title = post.Title, Status = post.Status, Applications = post.Applications.Count, 
                        CreatedAt = post.CreatedAt });
                }

                // 제출 지원서 목록 조회
                List<MyApplicationDto> myApplications = new List<MyApplicationDto>();
                foreach(var application in user.MyApplications)
                {
                    myApplications.Add(new MyApplicationDto { PostId = application.PostId, Nickname = application.Post.Author.Nickname, 
                        Title = application.Post.Title, Status = application.Status, CreatedAt = application.CreatedAt });
                }
                // 소속 팀 목록 조회
                var myReviewedPostIds = await _context.Reviews
                    .Where(r => r.ReviewerId == userId)
                    .Select(r => r.PostId)
                    .Distinct()
                    .ToListAsync();
                // 소속 팀 목록 조회
                List<ActivityTeamDto> myTeams = new List<ActivityTeamDto>();
                foreach (var membership in user.TeamMemberships)
                {
                    bool isEnded = membership.Team.Post?.Status == PostStatus.Completed;
                    bool isEvaluated = myReviewedPostIds.Contains(membership.Team.PostId);
                    myTeams.Add(new ActivityTeamDto { TeamId = membership.TeamId, TeamName = membership.Team.TeamName, PostId = membership.Team.PostId, IsEnded = isEnded, IsEvaluated = isEvaluated });
                }

                return new GetMyActivitiesResponse { IsSuccess = true, Message = "내 활동을 성공적으로 불러왔습니다.", MyPosts = myPosts, MyApplications = myApplications, MyTeams = myTeams };
            }
            catch (Exception ex)
            {
                return new GetMyActivitiesResponse { IsSuccess = false, Message = $"내 활동을 불러오던 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

        // 팀원평가
        public async Task<GetReviewTargetResponse> GetReviewTargetAsync(int teamId, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 사용자 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                // 대상 팀 확인
                var team = await _context.Teams
                    .Include(t => t.TeamMembers)
                    .ThenInclude(tm => tm.User) // 사용자 정보 병합 조회
                    .FirstOrDefaultAsync(t => t.Id == teamId);
                if (team == null)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "팀 정보를 찾을 수 없습니다." };
                }

                if (!team.TeamMembers.Any(tm => tm.UserId == userId))
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "해당 팀을 평가할 권한이 없습니다." };
                }

                List<TeamMemberDto> members = new List<TeamMemberDto>();
                foreach (var member in team.TeamMembers)
                {
                    if (member.UserId == user.Id)
                        continue;

                    members.Add(new TeamMemberDto { UserId = member.UserId, Nickname = member.User.Nickname, Role = member.Role, Position = member.Position });
                }
                

                return new GetReviewTargetResponse { IsSuccess = true, Message = "팀원 평가입니다.", PostId = team.PostId, Members = members };
            }
            catch (Exception ex)
            {
                return new GetReviewTargetResponse { IsSuccess = false, Message = $"팀원 평가 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

        // 팀원평가 제출
        public async Task<SubmitReviewResponse> SubmitReviewAsync(int teamId, SubmitReviewRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 사용자 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                var team = await _context.Teams.FindAsync(teamId);
                if (team == null)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "팀 정보를 찾을 수 없습니다." };
                }
                int currentPostId = team.PostId;

                // 평가 유효성 검증
                var targetUserIds = request.Reviews.Select(r => r.UserId).ToList();
                if (targetUserIds.Count != targetUserIds.Distinct().Count())
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "요청 데이터 내에 중복된 평가 대상이 있습니다." };
                }

                if (targetUserIds.Contains(userId))
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "자기 자신은 평가할 수 없습니다." };
                }

                bool hasDuplicateReview = await _context.Reviews.AnyAsync(r => r.PostId == currentPostId && r.ReviewerId == userId && targetUserIds.Contains(r.RevieweeId));
                if (hasDuplicateReview)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "이미 평가를 완료한 팀입니다." };
                }

                var validTeamMembers = await _context.TeamMembers
                    .Where(tm => tm.Team.PostId == currentPostId)
                    .Select(tm => tm.UserId)
                    .ToListAsync();

                // 외부 인원 포함 여부 검증
                bool containsInvalidUser = targetUserIds.Except(validTeamMembers).Any();
                if (containsInvalidUser)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "해당 팀의 팀원이 아닌 사용자가 평가 대상에 포함되어 있습니다." };
                }

                // 평가 데이터 저장
                foreach (var review in request.Reviews)
                {
                    _context.Reviews.Add(new Review { PostId = currentPostId, ReviewerId = userId, RevieweeId = review.UserId, ReliabilityScore = review.ReliabilityScore, 
                        ContributionScore = review.ContributionScore, CommunicationScore = review.CommunicationScore});

                }

               // 유저 평가 점수 갱신
                var usersToUpdate = await _context.Users.Include(u => u.ReceivedReviews).Where(u => targetUserIds.Contains(u.Id)).ToListAsync();
                foreach(var member in usersToUpdate)
                {
                    if(member.ReceivedReviews.Any())
                    {
                        member.ReliabilityScore = member.ReceivedReviews.Average(u => u.ReliabilityScore);
                        member.ContributionScore = member.ReceivedReviews.Average(u =>u.ContributionScore);
                        member.CommunicationScore = member.ReceivedReviews.Average(u => u.CommunicationScore);
                    }
                }
                await _context.SaveChangesAsync();

                return new SubmitReviewResponse { IsSuccess = true, Message = "팀원 평가가 완료되었습니다." };
            }
            catch (Exception ex)
            {
                return new SubmitReviewResponse { IsSuccess = false, Message = $"팀원 평가 제출 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

        // 모집글 모집 종료
        public async Task<ClosePostResponse> ClosePostAsync(ClosePostRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 사용자 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                var post = await _context.Posts
                    .Include(p => p.Applications)
                    .FirstOrDefaultAsync(p => p.Id == request.PostId);

                if (post == null)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "모집 글 정보를 찾을 수 없습니다." };
                }

                // 글 작성자 본인만 모집을 마감할 수 있도록 검증
                if (post.AuthorId != userId)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "모집글을 마감할 권한이 없습니다." };
                }

                // 모집 상태 갱신
                post.Status = PostStatus.InProgress;

                var newTeam = new Team
                {
                    PostId = post.Id,
                    TeamName = post.Title,
                    CreatedAt = DateTime.Now,
                    TeamMembers = new List<TeamMember>()
                };

                newTeam.TeamMembers.Add(new TeamMember
                {
                    UserId = post.AuthorId,
                    Role = TeamRole.Leader,
                    // 초기 직무 값 설정
                });

                var acceptedApplicants = post.Applications
                    .Where(a => a.Status == ApplicationStatus.Accepted)
                    .ToList();

                foreach (var applicant in acceptedApplicants)
                {
                    newTeam.TeamMembers.Add(new TeamMember
                    {
                        UserId = applicant.UserId,
                        Role = TeamRole.Member,
                        // 초기 직무 값 설정
                    });
                }

                // 대기 지원서 일괄 거절
                var pendingApplications = post.Applications
                    .Where(a => a.Status == ApplicationStatus.Pending)
                    .ToList();

                foreach (var app in pendingApplications)
                {
                    app.Status = ApplicationStatus.Rejected;
                }

                _context.Teams.Add(newTeam);

                await _context.SaveChangesAsync();

                return new ClosePostResponse { IsSuccess = true, Message = "모집이 종료되었습니다." };
            }
            catch (Exception ex)
            {
                return new ClosePostResponse { IsSuccess = false, Message = $"팀 생성 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
        
    }
}



