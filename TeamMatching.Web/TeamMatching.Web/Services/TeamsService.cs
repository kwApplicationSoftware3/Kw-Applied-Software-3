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
    // 팀 관련 비지니스 로직 구현체
    public class TeamsService : ITeamsService
    {
        private readonly ApplicationDbContext _context;

        public TeamsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 팀 메인화면 조회
        public async Task<GetTeamMainResponse> GetTeamMainAsync(int teamId, int currentUserId)
        {
            try
            {
                // 소속 팀원 데이터 로딩
                var team = await _context.Teams
                    .Include(t => t.TeamMembers)
                         .ThenInclude(tm => tm.User)
                    .Include(t => t.TeamPosts)
                    .FirstOrDefaultAsync(t => t.Id == teamId);

                // 데이터 접근 예외 방어
                if (team == null)
                {
                    return new GetTeamMainResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 프로젝트가 종료되어 파기된 팀 공간입니다."
                    };
                }

                // 허가되지 않은 접근 차단
                bool isAuthorizedMember = team.TeamMembers.Any(tm => tm.UserId == currentUserId);
                if (!isAuthorizedMember)
                {
                    return new GetTeamMainResponse
                    {
                        IsSuccess = false,
                        Message = "귀하는 이 팀의 공식 소속 팀원이 아닙니다. 메인화면 열람 권한이 없습니다."
                    };
                }

                // 팀원 데이터 구조화
                var membersList = team.TeamMembers.Select(tm => new TeamMemberRoleDto
                {
                    TeamMemberId = tm.UserId, // 팀원 식별자 맵핑
                    Nickname = tm.User.Nickname, // 닉네임
                    Role = tm.Role,           // 역할 정보 매핑
                    Position = tm.Position    // 직무 정보 매핑
                }).ToList();

                // 팀 전용 게시글 수집
                var postsList = team.TeamPosts
                    .OrderByDescending(tp => tp.CreatedAt) // 게시글 최신순 정렬
                    .Select(tp => new TeamPostListItemDto
                    {
                        TeamPostId = tp.Id, // 게시글 식별자 할당
                        Title = tp.Title,   // 글 제목 할당
                        CreatedAt = tp.CreatedAt // 작성 시간 할당
                    })
                    .ToList();

                // 팀원 스케줄 종합
                var availabilities = await _context.TeamAvailabilities
                    .Include(ta => ta.Member)
                        .ThenInclude(tm => tm.User)
                    .Where(ta => ta.TeamId == teamId)
                    .ToListAsync();

                // 자신 스케줄 추출
                var mySchedules = availabilities
                    .Where(ta => ta.Member.UserId == currentUserId)
                    .Select(ta => ta.SlotStart)
                    .ToList();

                // 전체 스케줄 합산
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

                // 모든 데이터 객체 래핑 및 반환
                return new GetTeamMainResponse
                {
                    IsSuccess = true,
                    Message = "팀 메인화면을 불러왔습니다.",
                    TeamName = team.TeamName, // 팀명 매핑 적용
                    TeamPosts = postsList,
                    TeamMemberRoles = membersList,
                    TotalSchedules = totalSchedules,
                    MySchedules = mySchedules
                };
            }
            catch (Exception ex)
            {
                // 시스템 오류 반환
                return new GetTeamMainResponse
                {
                    IsSuccess = false,
                    Message = $"팀 대시보드 쿼리 연동 중 시스템 서버 에러가 발생했습니다: {ex.Message}"
                };
            }
        }

        // 팀명 변경
        public async Task<UpdateTeamNameResponse> UpdateTeamNameAsync(int teamId, int currentUserId, UpdateTeamNameRequest request)
        {
            try
            {
                // 팀장 권한 검증 쿼리
                bool isLeader = await _context.TeamMembers
                    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == currentUserId && tm.Role == TeamRole.Leader);

                if (!isLeader)
                {
                    return new UpdateTeamNameResponse
                    {
                        IsSuccess = false,
                        Message = "팀 이름은 팀장만 변경할 수 있습니다." 
                    };
                }

                // 팀 데이터 로드
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);

                // 권한 불충분 예외 처리
                if (team == null)
                {
                    return new UpdateTeamNameResponse { IsSuccess = false, Message = "존재하지 않는 팀입니다." };
                }

                // 팀 이름 갱신 저장
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
        
        // 팀 내부 게시글 작성
        public async Task<CreateTeamPostResponse> CreateTeamPostAsync(int teamId, int currentUserId, CreateTeamPostRequest request)
        {
            try
            {
                // 소속 멤버 여부 체크
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

                // 팀 게시물 데이터 생성
                var newPost = new TeamPost
                {
                    TeamId = teamId,
                    AuthorId = currentUserId, // 작성자 식별자 포함
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.Now
                };

                // 변경 사항 저장
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

        // 팀 게시글 수정
        public async Task<UpdateTeamPostResponse> UpdateTeamPostAsync(int teamId, int postId, int currentUserId, UpdateTeamPostRequest request)
        {
            try
            {
                // 수정 대상 게시물 탐색
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
                
                // 작성자 일치 여부 확인
                if (post.AuthorId != currentUserId)
                {
                    return new UpdateTeamPostResponse { IsSuccess = false, Message = "본인이 작성한 글만 수정할 수 있습니다." };
                }

                // 게시글 갱신 내용 저장
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

        // 팀 게시글 삭제
        public async Task<DeleteTeamPostResponse> DeleteTeamPostAsync(int teamId, int postId, int currentUserId)
        {
            try
            {
                // 삭제 대상 게시물 탐색
                var post = await _context.TeamPosts
                    .FirstOrDefaultAsync(p => p.Id == postId && p.TeamId == teamId);

                // 예외 조건 검사
                if (post == null)
                {
                    return new DeleteTeamPostResponse
                    {
                        IsSuccess = false,
                        Message = "존재하지 않거나 이미 삭제된 게시글입니다."
                    };
                }

                // 작성자 일치 여부 확인
                if (post.AuthorId != currentUserId)
                {
                    return new DeleteTeamPostResponse { IsSuccess = false, Message = "본인이 작성한 글만 삭제할 수 있습니다." };
                }


                // 게시글 삭제 반영
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

        // 팀 게시글 조회
        public async Task<GetTeamPostDetailResponse> GetTeamPostDetailAsync(int teamId, int postId, int currentUserId)
        {
            try
            {
                // 팀 소속 권한 확인
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

                // 대상 게시물 조회
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

                // 댓글 및 작성자 정보 조회 및 정렬
                var commentsList = post.TeamPostComments
                    .OrderBy(tpc => tpc.CreatedAt)
                    .Select(tpc => new TeamPostCommentDto
                {
                    Nickname = tpc.User != null ? tpc.User.Nickname : "알 수 없는 사용자",
                    Content = tpc.Content,
                    CreatedAt = tpc.CreatedAt
                }).ToList();
             

                // 응답 객체 포맷 구성 반환
                return new GetTeamPostDetailResponse
                {
                    IsSuccess = true,
                    Message = "게시글 상세 정보를 성공적으로 불러왔습니다.",
                    Title = post.Title,
                    Content = post.Content,
                    // 게시글 소유권 판별
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

        // 댓글 작성
        public async Task<CreateTeamPostCommentResponse> CreateTeamPostCommentAsync(int teamId, int postId, int currentUserId, CreateTeamPostCommentRequest request)
        {
            try
            {
                // 유저 소속 확인
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

                // 대상 게시글 존재 확인
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

                // 댓글 데이터 인스턴스화
                var newComment = new TeamPostComment
                {
                    TeamPostId = postId,       // 대상 게시물 할당
                    UserId = currentUserId,    // 작성 유저 할당
                    Content = request.Content, // 댓글 내용 할당
                    CreatedAt = DateTime.Now
                };

                // DB에 추가하고 변경사항 저장
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

        // 프로젝트 종료
        public async Task<EndProjectResponse> EndProjectAsync(int teamId, int currentUserId)
        {
            try
            {
                // 현재 유저가 해당 팀의 팀장인지 확인
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

                // 종료 대상 팀 및 게시물 로드
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

                // 프로젝트 상태 갱신
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

        // 팀원 역할 변경
        public async Task<UpdateTeamRolesResponse> UpdateTeamRolesAsync(int teamId, int currentUserId, UpdateTeamRolesRequest request)
        {
            try
            {
                // 팀장 권한 검증
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

                // 전체 팀원 목록 조회
                var teamMembers = await _context.TeamMembers
                    .Where(tm => tm.TeamId == teamId)
                    .ToListAsync();

                // 변경 요청 역할 갱신 처리
                foreach (var roleUpdate in request.TeamRoles)
                {
                    // 대상 팀원 식별자 매칭
                    var targetMember = teamMembers.FirstOrDefault(tm => tm.UserId == roleUpdate.TeamMemberId);

                    if (targetMember != null)
                    {
                        targetMember.Role = roleUpdate.Role; // 역할 등급 변경
                        targetMember.Position = roleUpdate.Position; // 담당 직무 변경
                    }
                }

                // 최소 팀장 인원 유지 검증
                if (!teamMembers.Any(tm => tm.Role == TeamRole.Leader))
                {
                    return new UpdateTeamRolesResponse
                    {
                        IsSuccess = false,
                        Message = "최소 1명 이상의 팀장이 팀에 남아있어야 합니다."
                    };
                }

                // 전체 변경사항 반영
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

        // 팀원 가능 시간 설정
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