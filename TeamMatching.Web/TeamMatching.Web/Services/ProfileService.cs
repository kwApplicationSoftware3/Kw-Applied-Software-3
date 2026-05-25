using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Enums;
using TeamMatching.Web.Data;

namespace TeamMatching.Web.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;

        public ProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetProfileResponse> GetProfileAsync(int UserId)
        {
            try
            {
                if (UserId <= 0)
                {
                    return new GetProfileResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 유저 존재 확인
                var user = await _context.Users
                    .Include(u => u.UserTags)
                    .Include(u => u.TeamMemberships)         // 내 팀 정보 조인
                    .ThenInclude(tm => tm.Team)              // 팀 기본 정보 조인
                    .ThenInclude(t => t.Post)                // 팀과 연결된 모집글(상태 확인용) 조인
                    .FirstOrDefaultAsync(u => u.Id == UserId);
                if (user == null)
                {
                    return new GetProfileResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }
                return new GetProfileResponse
                {
                    IsSuccess = true,
                    Message = "내 정보를 성공적으로 불러왔습니다.",
                    Nickname = user.Nickname,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Bio= user.Bio,
                    ReliabilityScore = user.ReliabilityScore,
                    ContributionScore = user.ContributionScore,
                    CommunicationScore = user.CommunicationScore,
                    UserTagIds = user.UserTags.Select(ut => ut.TagId).ToList(),
                    MyTeams = user.TeamMemberships.Select(tm => new ProfileTeamDto
                    {
                        TeamName = tm.Team.TeamName,
                        Role = tm.Role,
                        Position = tm.Position, 
                        Status = tm.Team.Post.Status
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GetProfileResponse { IsSuccess = false, Message = $"내 정보를 불러오던 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
        public async Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request, int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return new UpdateProfileResponse { IsSuccess = false, Message = "사용자 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users
                    .Include(u => u.UserTags)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new UpdateProfileResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                {
                    return new UpdateProfileResponse { IsSuccess = false, Message = "기존 비밀번호가 일치하지 않습니다." };
                }

                user.Nickname = request.Nickname;
                user.Bio = request.Bio;
                user.ProfileImageUrl = request.ProfileImageUrl;
                if (!string.IsNullOrEmpty(request.NewPassword))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                }
                user.UpdatedAt = DateTime.Now;

                if (request.SelectedTagIds != null && request.SelectedTagIds.Any())
                {
                    user.UserTags.Clear();

                    //DB에 실제 존재하는 Tag인지 확인 후 추가
                    var validTagIds = await _context.Tags
                        .Where(t => request.SelectedTagIds.Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    foreach (var tagId in validTagIds)
                    {
                        user.UserTags.Add(new Shared.Entities.UserTag { TagId = tagId });
                    }
                }

                await _context.SaveChangesAsync();

                return new UpdateProfileResponse
                {
                    IsSuccess = true,
                    Message = "내 프로필 수정이 완료되었습니다.",
                };

            }
            catch (Exception ex)
            {
                return new UpdateProfileResponse { IsSuccess = false, Message = $"내 프로필 수정 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
    }
}
