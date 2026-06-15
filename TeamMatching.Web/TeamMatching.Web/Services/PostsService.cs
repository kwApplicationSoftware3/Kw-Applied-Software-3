using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Web.Data;
using TeamMatching.Shared.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    // 모집글 관련 비지니스 로직 구현체
    public class PostsService : IPostsService
    {
        private readonly ApplicationDbContext _context;

        public PostsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 글 작성 처리
        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, int authorId)
        {
            try
            {
                if (authorId <= 0)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보가 유효하지 않습니다." };
                }
                
                // 작성자 확인
                var author = await _context.Users.FindAsync(authorId);
                if (author == null)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보를 찾을 수 없습니다." };
                }

                // 게시글 데이터 초기화
                var post = new Post
                {
                    AuthorId = authorId,
                    Title = request.Title!,
                    Content = request.Content!,
                    Summary = request.Summary!,
                    Category = request.Category,
                    MaxMembers = request.MaxMembers,
                    CurrentMembers = 1, // 팀장 인원 카운트 반영
                };

                // 선택 태그 매핑
                if (request.SelectedTagIds != null && request.SelectedTagIds.Any())
                {
                    foreach (var tagId in request.SelectedTagIds)
                    {
                        post.PostTags.Add(new PostTag { TagId = tagId });
                    }
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return new CreatePostResponse { IsSuccess = true, Message = "게시글이 생성되었습니다." };
            }
            catch (Exception ex)
            {
                return new CreatePostResponse { IsSuccess = false, Message = $"게시글 생성 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

        // 글 목록 조회
        public async Task<GetPostsResponse> GetPostsAsync()
        {
            try
            {
                // 게시글 목록 조회 및 포맷팅
                var posts = await _context.Posts
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PostListItemDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Summary = p.Summary,
                        CurrentMembers = p.CurrentMembers,
                        MaxMembers = p.MaxMembers,
                        CreatedAt = p.CreatedAt,
                        Tags = p.PostTags.Select(pt => pt.Tag!.Name).ToList(),
                        Status = p.Status
                    })
                    .ToListAsync();

                return new GetPostsResponse
                {
                    IsSuccess = true,
                    Message = "모집 글 목록을 성공적으로 불러왔습니다.",
                    Data = posts
                };
            }
            catch (Exception ex)
            {
                return new GetPostsResponse
                {
                    IsSuccess = false,
                    Message = $"게시글 목록 조회 중 오류가 발생했습니다: {ex.Message}",
                    Data = new List<PostListItemDto>()
                };
            }
        }

        // 특정 게시글의 상세 정보 조회
        public async Task<GetPostDetailResponse> GetPostDetailAsync(int postId, int? currentUserId)
        {
            try
            {
                // 게시글 및 태그 데이터 조회
                var post = await _context.Posts
                    .Include(p => p.PostTags)
                    .Include(p => p.Applications)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                // 결과 반환
                if (post == null)
                {
                    return new GetPostDetailResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }
                
                List<TeamMemberRolePositionDto>? teamMembers = null;
                if (post.Status == PostStatus.InProgress || post.Status == PostStatus.Completed)
                {
                    var team = await _context.Teams
                        .Include(t => t.TeamMembers)
                        .ThenInclude(tm => tm.User)
                        .FirstOrDefaultAsync(t => t.PostId == postId);

                    if (team != null)
                    {
                        teamMembers = team.TeamMembers.Select(tm => new TeamMemberRolePositionDto
                        {
                            TeamMemberNickname = tm.User != null ? tm.User.Nickname : "알 수 없는 사용자",
                            TeamMemberRole = tm.Role,
                            TeamMemberPosition = tm.Position
                        }).ToList();
                    }
                }

                // 데이터 래핑 반환
                return new GetPostDetailResponse
                {
                    IsSuccess = true,
                    Message = "모집 글 상세 정보를 성공적으로 불러왔습니다.",
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    Category = post.Category,
                    CurrentMembers = post.CurrentMembers,
                    MaxMembers = post.MaxMembers,
                    // 태그 식별자 추출
                    SelectedTagIds = post.PostTags.Select(pt => pt.TagId).ToList(),
                    // 본인 작성 여부 매핑
                    IsMyPost = currentUserId.HasValue && post.AuthorId == currentUserId.Value,
                    IsClosed = post.Status != PostStatus.Recruiting,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    ApplicationCount = post.Applications.Count,
                    TeamMembers = teamMembers
                };
            }
            catch (Exception ex)
            {
                return new GetPostDetailResponse { IsSuccess = false, Message = $"상세 정보 조회 중 오류 발생: {ex.Message}" };
            }
        }

        // 특정 모집글에 들어온 지원서 목록 조회
        public async Task<GetApplicationsResponse> GetApplicationsByPostIdAsync(int postId)
        {
            try
            {
                // 사용자 속성 활용
                var applications = await _context.Applications
                    .Where(a => a.PostId == postId)
                    .Include(a => a.User) // 유저 정보 조인
                        .ThenInclude(u => u!.UserTags)
                    .OrderByDescending(a => a.CreatedAt) // 최신 등록순 정렬
                    .ToListAsync();

                // 엔티티 정보 포맷팅
                var appList = applications.Select(a => new ApplicationItemDto
                {
                    ApplicationId = a.Id,
                    // 닉네임 추출
                    Nickname = a.User?.Nickname ?? "알 수 없는 사용자",
                    Bio = a.User?.Bio,
                    Message = a.Message ?? string.Empty,
                    // 태그 식별자 분리
                    SelectedTagIds = a.User?.UserTags.Select(ut => ut.TagId).ToList() ?? new List<int>(),
                    CreatedAt = a.CreatedAt,
                    Status = a.Status,
                    ReliabilityScore = a.User?.ReliabilityScore ?? 0,
                    ContributionScore = a.User?.ContributionScore ?? 0,
                    CommunicationScore = a.User?.CommunicationScore ?? 0
                }).ToList();

                return new GetApplicationsResponse
                {
                    IsSuccess = true,
                    Message = "지원자 목록을 성공적으로 불러왔습니다.",
                    Applications = appList
                };
            }
            catch (Exception ex)
            {
                return new GetApplicationsResponse
                {
                    IsSuccess = false,
                    Message = $"지원자 목록 조회 중 오류 발생: {ex.Message}"
                };
            }
        }

        // 모집글 삭제
        public async Task<DeletePostResponse> DeletePostAsync(int postId, int currentUserId)
        {
            try
            {
                // 삭제 대상 게시글 조회
                var post = await _context.Posts.FindAsync(postId);

                // 삭제 예외 검증
                if (post == null)
                {
                    return new DeletePostResponse { IsSuccess = false, Message = "존재하지 않거나 이미 삭제된 게시글입니다." };
                }

                // 삭제 권한 검사
                if (post.AuthorId != currentUserId)
                {
                    return new DeletePostResponse { IsSuccess = false, Message = "본인이 작성한 게시글만 삭제할 수 있습니다." };
                }

                // 데이터 삭제 반영
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                return new DeletePostResponse
                {
                    IsSuccess = true,
                    Message = "모집글이 삭제되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new DeletePostResponse { IsSuccess = false, Message = $"게시글 삭제 중 오류가 발생했습니다: {ex.Message}" };
            }
        }

        // 모집글 지원서 제출
        public async Task<ApplyPostResponse> ApplyPostAsync(int postId, int userId, ApplyPostRequest request)
        {
            try
            {
                // 게시글 존재 여부 검사
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return new ApplyPostResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }

                // 중복 지원 여부 점검
                var alreadyApplied = await _context.Applications
                    .AnyAsync(a => a.PostId == postId && a.UserId == userId);

                if (alreadyApplied)
                {
                    return new ApplyPostResponse { IsSuccess = false, Message = "이미 이 프로젝트에 지원하셨습니다." };
                }

                // 지원서 데이터 저장
                var application = new Application
                {
                    PostId = postId,
                    UserId = userId,
                    Message = request.Message,
                    Status = TeamMatching.Shared.Enums.ApplicationStatus.Pending, // 대기 상태로 시작
                    CreatedAt = DateTime.Now
                };

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                return new ApplyPostResponse
                {
                    IsSuccess = true,
                    Message = "지원서 제출이 완료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new ApplyPostResponse { IsSuccess = false, Message = $"지원 중 오류 발생: {ex.Message}" };
            }
        }

        // 기존 모집글 수정
        public async Task<UpdatePostResponse> UpdatePostAsync(int postId, int userId, UpdatePostRequest request)
        {
            try
            {
                // 수정 대상 게시글 조회
                var post = await _context.Posts
                    .Include(p => p.PostTags)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return new UpdatePostResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }

                // 수정 권한 검증
                if (post.AuthorId != userId)
                {
                    return new UpdatePostResponse { IsSuccess = false, Message = "본인이 작성한 글만 수정할 수 있습니다." };
                }

                // 게시글 내용 갱신
                post.Title = request.Title;
                post.Content = request.Content;
                post.Summary = request.Summary;
                post.Category = request.Category;
                post.MaxMembers = request.MaxMembers;
                post.UpdatedAt = DateTime.Now;

                // 태그 갱신
                _context.PostTags.RemoveRange(post.PostTags);

                if (request.SelectedTagIds != null)
                {
                    foreach (var tagId in request.SelectedTagIds)
                    {
                        post.PostTags.Add(new PostTag { PostId = postId, TagId = tagId });
                    }
                }

                await _context.SaveChangesAsync();

                return new UpdatePostResponse
                {
                    IsSuccess = true,
                    Message = "모집 글이 수정되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new UpdatePostResponse { IsSuccess = false, Message = $"수정 중 오류 발생: {ex.Message}" };
            }
        }

        // 팀원 수락/거절
        public async Task<AcceptMemberResponse> AcceptMemberAsync(int postId, int currentUserId, AcceptMemberRequest request)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Post)
                    .FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.PostId == postId);

                if (application == null)
                {
                    return new AcceptMemberResponse { IsSuccess = false, Message = "지원서를 찾을 수 없습니다." };
                }

                if (application.Post == null || application.Post.AuthorId != currentUserId)
                {
                    return new AcceptMemberResponse { IsSuccess = false, Message = "본인이 작성한 글의 지원서만 처리할 수 있습니다." };
                }
                
                if (application.Status != ApplicationStatus.Pending)
                {
                    return new AcceptMemberResponse { IsSuccess = false, Message = "이미 처리된 지원서입니다." };
                }

                if (request.Status == ApplicationStatus.Accepted)
                {
                    // 모집 인원 증분 반영
                    if (application.Post != null)
                    {
                        if (application.Post.CurrentMembers >= application.Post.MaxMembers)
                        {
                            return new AcceptMemberResponse { IsSuccess = false, Message = "모집 인원이 가득 찼습니다." };
                        }
                        application.Post.CurrentMembers++;
                    }
                }

                application.Status = request.Status;
                await _context.SaveChangesAsync();
                
                string actionStr = request.Status == ApplicationStatus.Accepted ? "수락" : "거절";
                return new AcceptMemberResponse { IsSuccess = true, Message = $"지원서를 {actionStr}했습니다." };
            }
            catch (Exception ex)
            {
                return new AcceptMemberResponse { IsSuccess = false, Message = $"지원서 수락/거절 중 오류 발생: {ex.Message}" };
            }
        }
    }
}

