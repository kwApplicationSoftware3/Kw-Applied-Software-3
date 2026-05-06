using TeamMatching.Shared.DTOs;
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
                var user = await _context.Users.FindAsync(UserId);
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
                    MyTeams = new List<ProfileTeamDto>()
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
                    return new UpdateProfileResponse { IsSuccess = false, Message = "로그인 정보가 유효하지 않습니다." };
                }

                // 1. 사용자 존재 확인
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new UpdateProfileResponse { IsSuccess = false, Message = "사용자 정보를 찾을 수 없습니다." };
                }
                
                user.Nickname = request.Nickname;
                user.Bio = request.Bio;
                user.UpdatedAt = DateTime.Now;
                if (request.SelectedTagIds != null)
                {
                    user.UserTags.Clear();
                    foreach(var tagId in request.SelectedTagIds)
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
