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
    /// <summary>
    /// 인증 관련 비즈니스 로직 구현체ㅡ인글작성
    /// </summary>
    public class PostsService : IPostsService
    {
        private readonly ApplicationDbContext _context;

        public PostsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, int authorId)
        {
            try
            {
                if (authorId <= 0)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보가 유효하지 않습니다." };
                }
                
                // 1. 작성자 존재 확인
                var author = await _context.Users.FindAsync(authorId);
                if (author == null)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보를 찾을 수 없습니다." };
                }

                // 2. 게시글 엔티티 생성
                var post = new Post
                {
                    AuthorId = authorId, 
                    Title = request.Title!,
                    Content = request.Content!,
                    Summary = request.Summary!,
                    Category = request.Category,
                    MaxMembers = request.MaxMembers,
                };

                // 3. 선택한 태그(기술 스택) 매핑
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
        public async Task<GetPostsResponse> GetPostsAsync()
        {
            try
            {
                // DB에서 글 목록을 최신순으로 가져와서 DTO 형태로 변환
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
        /// <summary>
        /// 게시글 상세 조회 서비스 로직
        /// </summary>
        public async Task<GetPostDetailResponse> GetPostDetailAsync(int postId, int? currentUserId)
        {
            try
            {
                // 1. 게시글과 연관된 태그 정보를 DB에서 함께 불러옵니다.
                var post = await _context.Posts
                    .Include(p => p.PostTags)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                // 2. 글이 없으면 실패 응답 반환
                if (post == null)
                {
                    return new GetPostDetailResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }

                // 3. 브랜치 구조에 맞게 평탄화된 DTO에 데이터를 담아 반환
                return new GetPostDetailResponse
                {
                    IsSuccess = true,
                    Message = "모집 글 상세 정보를 성공적으로 불러왔습니다.",
                    Title = post.Title,
                    Content = post.Content,
                    CurrentMembers = post.CurrentMembers,
                    MaxMembers = post.MaxMembers,
                    // PostTags 배열에서 TagId 숫자만 뽑아서 리스트로 변환합니다.
                    SelectedTagIds = post.PostTags.Select(pt => pt.TagId).ToList(),
                    // 현재 접속한 유저 ID가 있고, 그 ID가 글 작성자 ID와 같다면 true
                    IsMyPost = currentUserId.HasValue && post.AuthorId == currentUserId.Value
                };
            }
            catch (Exception ex)
            {
                return new GetPostDetailResponse { IsSuccess = false, Message = $"상세 정보 조회 중 오류 발생: {ex.Message}" };
            }
        }
        // ... 상단에 using Microsoft.EntityFrameworkCore; 확인 ...

        public async Task<GetApplicationsResponse> GetApplicationsByPostIdAsync(int postId)
        {
            try
            {
                // 1. Applicant 대신 이미 만들어두신 User 속성을 사용합니다!
                var applications = await _context.Applications
                    .Where(a => a.PostId == postId)
                    .Include(a => a.User) // <--- 여기를 a.User로 수정!
                        .ThenInclude(u => u!.UserTags)
                    .OrderByDescending(a => a.CreatedAt) // 최신 지원순 정렬
                    .ToListAsync();

                // 2. DTO로 변환할 때도 a.User에서 정보를 꺼내옵니다.
                var appList = applications.Select(a => new ApplicationItemDto
                {
                    ApplicationId = a.Id,
                    // Applicant가 아닌 User에서 닉네임을 가져옵니다.
                    Nickname = a.User?.Nickname ?? "알 수 없는 사용자",
                    Message = a.Message ?? string.Empty,
                    // User의 태그 리스트에서 ID만 추출
                    SelectedTagIds = a.User?.UserTags.Select(ut => ut.TagId).ToList() ?? new List<int>(),
                    CreatedAt = a.CreatedAt,
                    Status = a.Status
                }).ToList();

                return new GetApplicationsResponse
                {
                    IsSuccess = true,
                    Message = "지원자 목록을 성공적으로 불러왔습니다.",
                    Applications = appList // Flat하게 리스트 주입
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
        // 모집글 삭제 서비스 로직
        public async Task<DeletePostResponse> DeletePostAsync(int postId, int currentUserId)
        {
            try
            {
                // 1. 데이터베이스에서 삭제할 게시글을 찾습니다.
                var post = await _context.Posts.FindAsync(postId);

                // 2. 예외 처리: 글이 이미 없거나 찾을 수 없는 경우
                if (post == null)
                {
                    return new DeletePostResponse { IsSuccess = false, Message = "존재하지 않거나 이미 삭제된 게시글입니다." };
                }

                // 3. 권한 검사: 현재 접속한 유저(currentUserId)와 글의 원래 작성자(AuthorId)가 다르면 거절합니다.
                if (post.AuthorId != currentUserId)
                {
                    return new DeletePostResponse { IsSuccess = false, Message = "본인이 작성한 게시글만 삭제할 수 있습니다." };
                }

                // 4. 권한이 확인되면 DB에서 삭제하고 변경사항을 저장합니다.
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
        // 모집글 지원 서비스 로직
        public async Task<ApplyPostResponse> ApplyPostAsync(int postId, int userId, ApplyPostRequest request)
        {
            try
            {
                // 1. 게시글이 실제로 존재하는지 확인
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return new ApplyPostResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }

                // 2. (선택사항) 이미 지원했는지 체크하는 로직을 넣으면 중복 지원을 방지할 수 있습니다.
                var alreadyApplied = await _context.Applications
                    .AnyAsync(a => a.PostId == postId && a.UserId == userId);

                if (alreadyApplied)
                {
                    return new ApplyPostResponse { IsSuccess = false, Message = "이미 이 프로젝트에 지원하셨습니다." };
                }

                // 3. 지원서 엔티티 생성 및 저장
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
        // 모집글 수정 서비스 로직
        public async Task<UpdatePostResponse> UpdatePostAsync(int postId, int userId, UpdatePostRequest request)
        {
            try
            {
                // 1. 수정할 글을 기존 태그 정보와 함께 불러옵니다.
                var post = await _context.Posts
                    .Include(p => p.PostTags)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return new UpdatePostResponse { IsSuccess = false, Message = "존재하지 않는 게시글입니다." };
                }

                // 2. 권한 체크: 본인 글인지 확인
                if (post.AuthorId != userId)
                {
                    return new UpdatePostResponse { IsSuccess = false, Message = "본인이 작성한 글만 수정할 수 있습니다." };
                }

                // 3. 일반 정보 업데이트
                post.Title = request.Title;
                post.Content = request.Content;
                post.Summary = request.Summary;
                post.Category = request.Category;
                post.MaxMembers = request.MaxMembers;
                post.UpdatedAt = DateTime.Now;

                // 4. 태그 정보 업데이트 (기존 태그 삭제 후 새 태그 추가)
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
                    // 현재 모집 인원 증가
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

