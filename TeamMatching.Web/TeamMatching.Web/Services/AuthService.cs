using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 인증 관련 비즈니스 로직 구현체
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // 1. 이메일 중복 체크
                var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);
                if (existingUser)
                {
                    return new RegisterResponse { IsSuccess = false, Message = "이미 가입된 이메일입니다." };
                }

                // 2. 유저 엔티티 생성 (비밀번호 해싱은 추후 보안 강화 시 적용)
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = request.Password, // 실무에서는 반드시 암호화 필요
                    Nickname = request.Nickname,
                    Department = request.Department,
                    StudentId = request.StudentId,
                    Bio = request.Bio,
                    CreatedAt = DateTime.Now
                };

                // 3. 선택한 태그(기술 스택) 매핑
                if (request.SelectedTagIds != null && request.SelectedTagIds.Any())
                {
                    foreach (var tagId in request.SelectedTagIds)
                    {
                        user.UserTags.Add(new UserTag { TagId = tagId });
                    }
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new RegisterResponse { IsSuccess = true, Message = "회원가입이 완료되었습니다." };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { IsSuccess = false, Message = $"회원가입 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
    }
}
