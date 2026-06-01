using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Shared.Enums;
using TeamMatching.Web.Data; // main 브랜치의 ApplicationDbContext 주입

namespace TeamMatching.Web.Services
{
    public class TeamsService : ITeamsService
    {
        private readonly ApplicationDbContext _context;

        public TeamsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetTeamMainResponse> GetTeamMainAsync(int teamId, int currentUserId)
        {
            try
            {
                // 1. main 브랜치의 Team 엔티티 규격에 정확히 맞추어 소속 팀원(TeamMembers) 컬렉션을 함께 즉시 로드(Include)합니다.
                var team = await _context.Teams
                    .Include(t => t.TeamMembers)
                         .ThenInclude(tm => tm.User)
                    .Include(t => t.TeamPosts)
                    .FirstOrDefaultAsync(t => t.Id == teamId);

                // 2. 예외 방어선: 데이터베이스에 전달된 팀 ID 정보 자체가 검색되지 않는 비정상적인 상황 케어
                if (team == null)
                {
                    return new GetTeamMainResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 프로젝트가 종료되어 파기된 팀 공간입니다."
                    };
                }

                // 3. 보안 통제 가드: 브라우저 주소창에 타인의 팀 번호(ID)를 임의 개조하여 무단 침입을 시도하려는 행위를 원천 체크 차단합니다.
                // main 브랜치의 TeamMember 구조 내부 'UserId' 컬럼과 현재 요청자의 ID를 완벽히 대조 검증합니다.
                bool isAuthorizedMember = team.TeamMembers.Any(tm => tm.UserId == currentUserId);
                if (!isAuthorizedMember)
                {
                    return new GetTeamMainResponse
                    {
                        IsSuccess = false,
                        Message = "귀하는 이 팀의 공식 소속 팀원이 아닙니다. 메인화면 열람 권한이 원천 차단되었습니다."
                    };
                }

                // 4. 팀원 리스트 데이터 모델 바인딩 가공 (main 브랜치 TeamMember 엔티티 스펙인 UserId, Role, Position과 100% 매칭)
                var membersList = team.TeamMembers.Select(tm => new TeamMemberRoleDto
                {
                    TeamMemberId = tm.UserId, // 프론트엔드가 프로필 조회 등으로 연동하기 편하도록 UserId값을 팀원 식별자로 바인딩
                    Nickname = tm.User.Nickname, // 닉네임
                    Role = tm.Role,           // main 브랜치의 팀장/팀원 권한 이넘 코드 매핑
                    Position = tm.Position    // main 브랜치의 담당 직무 상세 텍스트(백엔드, 프론트엔드 등) 매핑
                }).ToList();

                // 5. 신규 확장한 팀 내부 게시판 전용 테이블(TeamPosts)에서 해당 팀으로 작성된 글을 필터링하여 수집합니다.
                var postsList = team.TeamPosts
                    .OrderByDescending(tp => tp.CreatedAt) // 가장 최근에 올린 중요한 공지사항이나 일정 피드가 UI상단에 배치되도록 정렬
                    .Select(tp => new TeamPostListItemDto
                    {
                        TeamPostId = tp.Id, // 게시글 상세 열람용 ID 매핑
                        Title = tp.Title,   // 공지 글 제목 매핑
                        CreatedAt = tp.CreatedAt // 작성 일시 매핑
                    })
                    .ToList();

                // 6. 팀원들의 가능 시간을 종합합니다.
                var availabilities = await _context.TeamAvailabilities
                    .Include(ta => ta.Member)
                        .ThenInclude(tm => tm.User)
                    .Where(ta => ta.TeamId == teamId)
                    .ToListAsync();

                // 6-1. 내 가능 시간만 따로 추출
                var mySchedules = availabilities
                    .Where(ta => ta.Member.UserId == currentUserId)
                    .Select(ta => ta.SlotStart)
                    .ToList();

                // 6-2. 전체 시간표 종합
                var totalSchedules = availabilities
                    .GroupBy(ta => ta.SlotStart)
                    .Select(g => new TeamScheduleSlotDto
                    {
                        AvailableTime = g.Key,
                        Count = g.Count(),
                        AvailableMemberNames = g.Select(ta => ta.Member.User.Nickname).ToList()
                    })
                    .OrderBy(dto => dto.AvailableTime)
                    .ToList();

                // 7. 명세서에 기재된 최종 포맷 양식인 단일 "data" 래퍼 구조체 내부에 모든 서브 리스트를 탑재하여 최종 반환 진행
                return new GetTeamMainResponse
                {
                    IsSuccess = true,
                    Message = "팀 메인화면을 불러왔습니다.",
                    TeamName = team.TeamName, // main 브랜치의 실제 원본 DB 필드명인 'TeamName' 매핑 완비
                    TeamPosts = postsList,
                    TeamMemberRoles = membersList,
                    TotalSchedules = totalSchedules,
                    MySchedules = mySchedules
                };
            }
            catch (Exception ex)
            {
                // 데이터베이스 타임아웃 등 예기치 못한 시스템 오류 상황 추적 및 예외 로그 바인딩 처리
                return new GetTeamMainResponse
                {
                    IsSuccess = false,
                    Message = $"팀 대시보드 쿼리 연동 중 시스템 서버 에러가 발생했습니다: {ex.Message}"
                };
            }
        }
        // 팀 이름 변경 비즈니스 로직 처리
        public async Task<UpdateTeamNameResponse> UpdateTeamNameAsync(int teamId, int currentUserId, UpdateTeamNameRequest request)
        {
            try
            {
                // 1. 권한 검증: 현재 요청을 보낸 사용자가 이 팀의 팀장(Leader)인지 단일 쿼리로 확인합니다.
                bool isLeader = await _context.TeamMembers
                    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId && tm.Role == TeamRole.Leader);

                if (!isLeader)
                {
                    return new UpdateTeamNameResponse
                    {
                        IsSuccess = false,
                        Message = "팀 이름은 팀장만 변경할 수 있는권한입니다." 
                    };
                }

                // 2. 팀 정보를 로드합니다.
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);

                // 3. 직무 권한 검증: 팀 소속이 맞더라도 '팀장(Leader)'이 아니라면 이름 수정을 거부합니다.
                if (team == null)
                {
                    return new UpdateTeamNameResponse { IsSuccess = false, Message = "존재하지 않는 팀입니다." };
                }

                // 4. 안전하게 검증이 완료되었으므로 main 브랜치 Team.cs의 실제 필드명인 'TeamName'에 새 값을 덮어씁니다.
                team.TeamName = request.TeamName;
                await _context.SaveChangesAsync();

                return new UpdateTeamNameResponse
                {
                    IsSuccess = true,
                    Message = "팀명 변경이 완료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new UpdateTeamNameResponse
                {
                    IsSuccess = false,
                    Message = $"팀 이름 변경 중 데이터베이스 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        // 팀 게시글 작성 비즈니스 로직        
        public async Task<CreateTeamPostResponse> CreateTeamPostAsync(int teamId, int currentUserId, CreateTeamPostRequest request)
        {
            try
            {
                // 1. 소속 검증: 해당 팀이 존재하고, 현재 유저가 그 팀의 멤버(TeamMember)로 등록되어 있는지 체크합니다.
                bool isMember = await _context.TeamMembers
                    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (!isMember)
                {
                    return new CreateTeamPostResponse
                    {
                        IsSuccess = false,
                        Message = "해당 팀의 소속원이 아니므로 게시글을 작성할 권한이 없습니다."
                    };
                }

                // 2. 새로운 팀 게시글 엔티티(TeamPost)를 생성합니다.
                var newPost = new TeamPost
                {
                    TeamId = teamId,
                    AuthorId = currentUserId, // 추가: 작성자 ID 저장
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.Now
                };

                // 3. DB에 추가하고 변경사항을 저장(Commit)합니다.
                _context.TeamPosts.Add(newPost);
                await _context.SaveChangesAsync();

                return new CreateTeamPostResponse
                {
                    IsSuccess = true,
                    Message = "게시글 작성이 완료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new CreateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = $"게시글 저장 중 서버 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        public async Task<UpdateTeamPostResponse> UpdateTeamPostAsync(int teamId, int postId, int currentUserId, UpdateTeamPostRequest request)
        {
            try
            {
                // 1. 대상 게시글 조회: 파라미터로 받은 팀 ID와 게시글 ID가 모두 일치하는 글을 찾습니다.
                var post = await _context.TeamPosts
                    .FirstOrDefaultAsync(p => p.Id == postId && p.TeamId == teamId);

                if (post == null)
                {
                    return new UpdateTeamPostResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 이미 삭제된 게시글입니다."
                    };
                }
                
                // 작성자인지 확인
                if (post.AuthorId != currentUserId)
                {
                    return new UpdateTeamPostResponse { IsSuccess = false, Message = "본인이 작성한 글만 수정할 수 있습니다." };
                }

                // 3. 내용 업데이트 및 DB 저장
                post.Title = request.Title;
                post.Content = request.Content;
                post.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new UpdateTeamPostResponse
                {
                    IsSuccess = true,
                    Message = "게시글이 수정되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new UpdateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = $"게시글 수정 중 데이터베이스 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        public async Task<DeleteTeamPostResponse> DeleteTeamPostAsync(int teamId, int postId, int currentUserId)
        {
            try
            {
                // 1. 삭제할 게시글 대상 조회
                var post = await _context.TeamPosts
                    .FirstOrDefaultAsync(p => p.Id == postId && p.TeamId == teamId);

                // 3. 예외 처리: 글이 이미 없거나 파라미터가 잘못된 경우
                if (post == null)
                {
                    return new DeleteTeamPostResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 이미 삭제된 게시글입니다."
                    };
                }

                // 작성자인지 확인
                if (post.AuthorId != currentUserId)
                {
                    return new DeleteTeamPostResponse { IsSuccess = false, Message = "본인이 작성한 글만 삭제할 수 있습니다." };
                }


                // 4. DB에서 데이터 삭제 및 변경사항 저장
                _context.TeamPosts.Remove(post);
                await _context.SaveChangesAsync();

                return new DeleteTeamPostResponse
                {
                    IsSuccess = true,
                    Message = "게시글이 삭제되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new DeleteTeamPostResponse
                {
                    IsSuccess = false,
                    Message = $"게시글 삭제 중 서버 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        public async Task<GetTeamPostDetailResponse> GetTeamPostDetailAsync(int teamId, int postId, int currentUserId)
        {
            try
            {
                // 1. 소속 보안 검증: 현재 글을 읽으려는 유저가 이 팀의 정식 멤버(TeamMember)인지 DB 전수조사 수행
                bool isMember = await _context.TeamMembers
                    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (!isMember)
                {
                    return new GetTeamPostDetailResponse
                    {
                        IsSuccess = false,
                        Message = "소속 팀원만 외부 비공개 게시글을 열람할 수 있습니다."
                    };
                }

                // 2. 타겟 게시글 단건 정밀 쿼리 조회
                var post = await _context.TeamPosts
                    .Include(tp => tp.TeamPostComments)
                        .ThenInclude(tpc => tpc.User)
                    .Include(tp => tp.Author)
                    .FirstOrDefaultAsync(p => p.Id == postId && p.TeamId == teamId);

                if (post == null)
                {
                    return new GetTeamPostDetailResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 작성자에 의해 삭제된 게시글입니다."
                    };
                }

                // 3. 해당 게시글에 기재된 댓글 목록(TeamPostComments)을 작성자(User) 정보와 조인(Include)하여 조회 정렬
                var commentsList = post.TeamPostComments
                    .OrderBy(tpc => tpc.CreatedAt)
                    .Select(tpc => new TeamPostCommentDto
                {
                    Nickname = tpc.User != null ? tpc.User.Nickname : "알 수 없는 사용자",
                    Content = tpc.Content,
                    CreatedAt = tpc.CreatedAt
                }).ToList();
             

                // 4. 명세서 청사진 규격 객체 조합 단계를 실행하여 리턴
                return new GetTeamPostDetailResponse
                {
                    IsSuccess = true,
                    Message = "게시글 상세 정보를 성공적으로 불러왔습니다.",
                    Title = post.Title,
                    Content = post.Content,
                    // 현재 조회 요청자 세션 유저 ID와 글 엔티티에 기록된 UserId 소유권자가 완벽 일치하면 true 반환
                    IsMyPost = post.AuthorId == currentUserId,
                    Comments = commentsList,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    NickName = post.Author != null ? post.Author.Nickname ?? "알 수 없는 사용자" : "알 수 없는 사용자"
                };
            }
            catch (Exception ex)
            {
                return new GetTeamPostDetailResponse
                {
                    IsSuccess = false,
                    Message = $"상세 정보를 불러오는 중 치명적인 서버 오류 발생: {ex.Message}"
                };
            }
        }
        public async Task<CreateTeamPostCommentResponse> CreateTeamPostCommentAsync(int teamId, int postId, int currentUserId, CreateTeamPostCommentRequest request)
        {
            try
            {
                // 1. 소속 검증: 현재 유저가 해당 팀의 멤버가 맞는지 확인
                bool isMember = await _context.TeamMembers
                    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (!isMember)
                {
                    return new CreateTeamPostCommentResponse
                    {
                        IsSuccess = false,
                        Message = "팀 소속원이 아니므로 댓글을 작성할 권한이 없습니다."
                    };
                }

                // 2. 게시글 유효성 검증: 댓글을 달려는 게시글이 해당 팀에 실제로 존재하는지 확인
                bool postExists = await _context.TeamPosts
                    .AnyAsync(p => p.Id == postId && p.TeamId == teamId);

                if (!postExists)
                {
                    return new CreateTeamPostCommentResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 삭제된 게시글입니다."
                    };
                }

                // 3. 댓글 엔티티(TeamPostComment) 생성 및 데이터 매핑
                var newComment = new TeamPostComment
                {
                    TeamPostId = postId,       // 어떤 게시글에 달린 댓글인지
                    UserId = currentUserId,    // 누가 작성했는지
                    Content = request.Content, // 댓글 본문
                    CreatedAt = DateTime.Now
                };

                // 4. DB에 추가하고 변경사항 저장
                _context.TeamPostComments.Add(newComment);
                await _context.SaveChangesAsync();

                return new CreateTeamPostCommentResponse
                {
                    IsSuccess = true,
                    Message = "댓글 작성이 완료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new CreateTeamPostCommentResponse
                {
                    IsSuccess = false,
                    Message = $"댓글 저장 중 시스템 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        public async Task<EndProjectResponse> EndProjectAsync(int teamId, int currentUserId)
        {
            try
            {
                // 1. 권한 검증: 현재 유저가 해당 팀의 '팀장(Leader)'인지 확인
                var memberInfo = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (memberInfo == null || memberInfo.Role != TeamRole.Leader)
                {
                    return new EndProjectResponse
                    {
                        IsSuccess = false,
                        Message = "프로젝트 종료 권한이 없습니다. 팀장만 종료할 수 있습니다."
                    };
                }

                // 2. 대상 팀(프로젝트) 존재 여부 확인 및 원본 모집글 로드
                var team = await _context.Teams
                    .Include(t => t.Post)
                    .FirstOrDefaultAsync(t => t.Id == teamId);
                    
                if (team == null)
                {
                    return new EndProjectResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않는 팀 또는 프로젝트입니다."
                    };
                }

                // 3. 프로젝트 상태를 Completed로 변경
                if (team.Post != null)
                {
                    team.Post.Status = PostStatus.Completed;
                }

                await _context.SaveChangesAsync();

                return new EndProjectResponse
                {
                    IsSuccess = true,
                    Message = "프로젝트가 종료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new EndProjectResponse
                {
                    IsSuccess = false,
                    Message = $"프로젝트 종료 처리 중 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
        public async Task<UpdateTeamRolesResponse> UpdateTeamRolesAsync(int teamId, int currentUserId, UpdateTeamRolesRequest request)
        {
            try
            {
                // 1. 보안 검증: 현재 변경을 시도하는 유저가 이 팀의 소속원인지, 그리고 '팀장(Leader)'이 맞는지 철저히 체크합니다.
                var currentUserMemberInfo = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (currentUserMemberInfo == null || currentUserMemberInfo.Role != TeamRole.Leader)
                {
                    return new UpdateTeamRolesResponse
                    {
                        IsSuccess = false,
                        Message = "팀원 역할을 변경할 권한이 없습니다. (팀장만 변경 가능)"
                    };
                }

                // 2. 해당 팀에 속한 모든 팀원들의 데이터를 DB에서 한 번에 가져옵니다.
                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.TeamId == teamId)
                    .ToListAsync();

                // 3. 프론트엔드에서 넘어온 변경 리스트를 순회하며 DB 데이터를 갱신합니다.
                foreach (var roleUpdate in request.TeamRoles)
                {
                    // 앞서 팀 메인화면 조회(GetTeamMain) 기능에서 TeamMemberId를 UserId로 매핑하여 내려줬으므로, 여기서도 UserId로 매칭합니다.
                    var targetMember = teamMembers.FirstOrDefault(tm => tm.UserId == roleUpdate.TeamMemberId);

                    if (targetMember != null)
                    {
                        targetMember.Role = roleUpdate.Role; // 리더/멤버 권한 변경
                        targetMember.Position = roleUpdate.Position; // 백엔드/프론트엔드 등 직무 변경
                    }
                }

                // [중요] 역할 변경 후 팀장이 0명이 되는지 방어
                if (!teamMembers.Any(tm => tm.Role == TeamRole.Leader))
                {
                    return new UpdateTeamRolesResponse
                    {
                        IsSuccess = false,
                        Message = "최소 1명 이상의 팀장이 팀에 남아있어야 합니다."
                    };
                }

                // 4. 모든 변경 사항을 트랜잭션으로 묶어 DB에 안전하게 커밋합니다.
                await _context.SaveChangesAsync();

                return new UpdateTeamRolesResponse
                {
                    IsSuccess = true,
                    Message = "역할 변경이 완료되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new UpdateTeamRolesResponse
                {
                    IsSuccess = false,
                    Message = $"역할 변경 중 서버 오류가 발생했습니다: {ex.Message}"
                };
            }
        }

        public async Task<SetAvailableTimesResponse> SetAvailableTimesAsync(int teamId, int currentUserId, SetAvailableTimesRequest request)
        {
            try
            {
                var teamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId);

                if (teamMember == null)
                {
                    return new SetAvailableTimesResponse
                    {
                        IsSuccess = false,
                        Message = "해당 팀의 소속원이 아니므로 가능 시간을 설정할 권한이 없습니다."
                    };
                }

                var existingAvailabilities = await _context.TeamAvailabilities
                    .Where(ta => ta.TeamId == teamId && ta.MemberId == teamMember.Id)
                    .ToListAsync();
                
                if (existingAvailabilities.Any())
                {
                    _context.TeamAvailabilities.RemoveRange(existingAvailabilities);
                }

                if (request.AvailableTimes != null && request.AvailableTimes.Any())
                {
                    var newAvailabilities = request.AvailableTimes.Select(time => new TeamAvailability
                    {
                        TeamId = teamId,
                        MemberId = teamMember.Id,
                        SlotStart = time,
                        CreatedAt = DateTime.Now
                    }).ToList();

                    _context.TeamAvailabilities.AddRange(newAvailabilities);
                }

                await _context.SaveChangesAsync();

                return new SetAvailableTimesResponse
                {
                    IsSuccess = true,
                    Message = "가능 시간이 성공적으로 설정되었습니다."
                };
            }
            catch (Exception ex)
            {
                return new SetAvailableTimesResponse
                {
                    IsSuccess = false,
                    Message = $"가능 시간 설정 중 서버 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
    }
}