using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Web.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // 1. 이메일로 사용자 조회
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return new LoginResponse { IsSuccess = false, Message = "가입되지 않은 이메일입니다." };
                }

                // 2. 비밀번호 검증 (현재는 단순 비교: 실무에서는 해싱 비교 필요)
                if (user.PasswordHash != request.Password)
                {
                    return new LoginResponse { IsSuccess = false, Message = "비밀번호가 일치하지 않습니다." };
                }

                // 3. JWT 생성
                var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
                if (string.IsNullOrEmpty(secret))
                {
                    return new LoginResponse { IsSuccess = false, Message = "서버 JWT 시크릿이 설정되어 있지 않습니다." };
                }

                var key = Encoding.UTF8.GetBytes(secret);
                var tokenHandler = new JwtSecurityTokenHandler();

                int accessExpMin = int.TryParse(Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXP_MIN"), out var parsedMin) ? parsedMin : 1440;

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Nickname ?? string.Empty)
                };

                var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddMinutes(accessExpMin);

                var jwt = new JwtSecurityToken(
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                var accessToken = tokenHandler.WriteToken(jwt);

                return new LoginResponse
                {
                    IsSuccess = true,
                    Message = "로그인에 성공했습니다.",
                    AccessToken = accessToken,
                    ExpiresAt = expires
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse { IsSuccess = false, Message = $"로그인 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
    }
}
