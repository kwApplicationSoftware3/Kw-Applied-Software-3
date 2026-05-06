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
    /// <summary>
    /// 내 활동 관련 로직 구현체
    /// </summary>
    public class MyActivitiesService : IMyActivitiesService
    {
        private readonly ApplicationDbContext _context;

        public MyActivitiesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetMyActivitiesResponse> GetMyActivitiesAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new GetMyActivitiesResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new GetMyActivitiesResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                // 2. 내 글 목록
                List<MyPostDto> myPosts = new List<MyPostDto>();
                foreach(var post in user.MyPosts)
                {
                    myPosts.Add(new MyPostDto { PostId = post.Id, Title = post.Title, Status = post.Status, Applications = post.Applications.Count, CreatedAt = post.CreatedAt });
                }

                // 3. 내 지원서 목록
                List<MyApplicationDto> myApplications = new List<MyApplicationDto>();
                foreach(var application in user.MyApplications)
                {
                    myApplications.Add(new MyApplicationDto { PostId = application.PostId, Nickname = application.Post.Author.Nickname, 
                        Title = application.Post.Title, Status = application.Status, CreatedAt = application.CreatedAt });
                }

                // 4. 내 팀 목록
                List<ActivityTeamDto> myTeams = new List<ActivityTeamDto>();
                foreach (var team in user.MyTeams)
                {
                    myTeams.Add(new ActivityTeamDto { TeamId = team.Id, TeamName = team.TeamName, PostId = team.PostId });
                }

                return new GetMyActivitiesResponse { IsSuccess = true, Message = "내 활동을 성공적으로 불러왔습니다.", MyPosts = myPosts, MyApplications = myApplications, MyTeams = myTeams };
            }
            catch (Exception ex)
            {
                return new GetMyActivitiesResponse { IsSuccess = false, Message = $"내 활동을 불러오던 중 오류가 발생했습니다: {ex.Message}" };
            }
        }


        public async Task<GetReviewTargetResponse> GetReviewTargetAsync(GetReviewTargetRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                // 2. 평가할 팀 확인
                var team = await _context.Teams.FindAsync(request.TeamId);
                if (team == null)
                {
                    return new GetReviewTargetResponse { IsSuccess = false, Message = "팀 정보를 찾을 수 없습니다." };
                }
                // 2. 내 글 목록
                List<TeamMemberDto> members = new List<TeamMemberDto>();
                foreach (var member in team.TeamMembers)
                {
                    if (member.UserId == user.Id)
                        continue;

                    members.Add(new TeamMemberDto { UserId = member.UserId, Nickname = member.User.Nickname, Role = member.Role.ToString() });
                }
                

                return new GetReviewTargetResponse { IsSuccess = true, Message = "팀원 평가입니다.", PostId = team.PostId, Members = members };
            }
            catch (Exception ex)
            {
                return new GetReviewTargetResponse { IsSuccess = false, Message = $"팀원 평가 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

     
        public async Task<SubmitReviewResponse> SubmitReviewAsync(SubmitReviewRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                // 2. 중복 평가인지 확인
                var targetUserIds = request.Reviews.Select(r => r.UserId).ToList();
                bool hasDuplicateReview = await _context.Reviews.AnyAsync(r => r.PostId == request.PostId && r.ReviewerId == userId && targetUserIds.Contains(r.RevieweeId));

                if (hasDuplicateReview)
                {
                    return new SubmitReviewResponse { IsSuccess = false, Message = "이미 평가를 완료한 팀입니다." };
                }
                
                // 3. Reivews에 추가
                foreach (var review in request.Reviews)
                {
                    _context.Reviews.Add(new Review { PostId = request.PostId, ReviewerId = userId, RevieweeId = review.UserId, ReliabilityScore = review.ReliabilityScore, 
                        ContributionScore = review.ContributionScore, CommunicationScore = review.CommunicationScore});

                }

                await _context.SaveChangesAsync();
               // 4. 유저 평균 점수 업데이트
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

        
        public async Task<ClosePostResponse> ClosePostAsync(ClosePostRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                var post = await _context.Posts.FindAsync(request.PostId);

                if (post == null)
                {
                    return new ClosePostResponse { IsSuccess = false, Message = "모집 글 정보를 찾을 수 없습니다." };
                }

                // 2. 모집 상태 종료로 변경
                post.Status = PostStatus.Completed;

                await _context.SaveChangesAsync();

                return new ClosePostResponse { IsSuccess = true, Message = "모집이 종료되었습니다." };
            }
            catch (Exception ex)
            {
                return new ClosePostResponse { IsSuccess = false, Message = $"모집 종료 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
        
    }
}



