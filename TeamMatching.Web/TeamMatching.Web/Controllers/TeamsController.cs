using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Components;
using TeamMatching.Web.Services;

namespace TeamMatching.Web.Controllers
{
    // 팀 관련 API 컨트롤러
    [ApiController]
    [Route("api/[controller]")] // /api/teams
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        // 팀 메인화면 조회 엔트포인트
        [Authorize]
        [HttpGet("{teamId}")]
        public async Task<ActionResult<GetTeamMainResponse>> GetTeamMain(int teamId)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new GetTeamMainResponse
                {
                    IsSuccess = false,
                    Message = "인증 자격 증명(토큰) 정보가 누락되었거나 만료되었습니다. 보안을 위해 다시 로그인해 주세요."
                });
            }

            // 팀 데이터 조회 쿼리 실행
            var result = await _teamsService.GetTeamMainAsync(teamId, currentUserId);

            // 요청 처리 성공 시
            if (result.IsSuccess)
            {
                return Ok(result); // 성공 응답 반환
            }

            // 접근 권한 예외 방어
            return BadRequest(result); // 예외 응답 반환
        }

        // 팀명 변경 엔트포인트
        [Authorize]
        [HttpPut("{teamId}")]
        public async Task<ActionResult<UpdateTeamNameResponse>> UpdateTeamName(int teamId, [FromBody] UpdateTeamNameRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdateTeamNameResponse { IsSuccess = false, Message = "팀 이름을 올바르게 입력해 주세요." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamNameResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰 자격 증명이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.UpdateTeamNameAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result); // 성공 응답 반환
            }

            return BadRequest(result); // 에러 응답 반환
        }

        // 팀 내부 게시글 작성 엔트포인트
        [Authorize]
        [HttpPost("{teamId}/posts")]
        public async Task<ActionResult<CreateTeamPostResponse>> CreateTeamPost(int teamId, [FromBody] CreateTeamPostRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreateTeamPostResponse { IsSuccess = false, Message = "입력한 게시글 데이터 형식이 올바르지 않습니다." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new CreateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.CreateTeamPostAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 팀 게시글 수정 엔트포인트
        [Authorize]
        [HttpPut("{teamId}/posts/{postId}")]
        public async Task<ActionResult<UpdateTeamPostResponse>> UpdateTeamPost(int teamId, int postId, [FromBody] UpdateTeamPostRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdateTeamPostResponse { IsSuccess = false, Message = "입력한 게시글 데이터 형식이 올바르지 않습니다." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.UpdateTeamPostAsync(teamId, postId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 팀 게시글 삭제 엔트포인트
        [Authorize]
        [HttpDelete("{teamId}/posts/{postId}")]
        public async Task<ActionResult<DeleteTeamPostResponse>> DeleteTeamPost(int teamId, int postId)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new DeleteTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.DeleteTeamPostAsync(teamId, postId, currentUserId);

            // 결과 반환
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 팀 게시글 조회 엔트포인트
        [Authorize]
        [HttpGet("{teamId}/posts/{postId}")]
        public async Task<ActionResult<GetTeamPostDetailResponse>> GetTeamPostDetail(int teamId, int postId)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new GetTeamPostDetailResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰 정보가 불완전합니다. 로그아웃 후 다시 재진입해 주세요."
                });
            }

            // 상세 데이터 조회
            var result = await _teamsService.GetTeamPostDetailAsync(teamId, postId, currentUserId);

            // 요청 처리 성공 시
            if (result.IsSuccess)
            {
                return Ok(result); // 성공 응답 반환
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 댓글 작성 엔트포인트
        [Authorize]
        [HttpPost("{teamId}/posts/{postId}")]
        public async Task<ActionResult<CreateTeamPostCommentResponse>> CreateTeamPostComment(int teamId, int postId, [FromBody] CreateTeamPostCommentRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreateTeamPostCommentResponse { IsSuccess = false, Message = "댓글 내용을 올바르게 입력해주세요." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new CreateTeamPostCommentResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.CreateTeamPostCommentAsync(teamId, postId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // 프로젝트 종료 엔트포인트
        [Authorize]
        [HttpDelete("{teamId}")]
        public async Task<ActionResult<EndProjectResponse>> EndProject(int teamId)
        {
            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new EndProjectResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.EndProjectAsync(teamId, currentUserId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 팀원 역할 변경 엔트포인트
        [Authorize]
        [HttpPost("{teamId}/team-role-update")]
        public async Task<ActionResult<UpdateTeamRolesResponse>> UpdateTeamRoles(int teamId, [FromBody] UpdateTeamRolesRequest request)
        {
            // 모델 유효성 검증
            if (!ModelState.IsValid || request.TeamRoles == null || request.TeamRoles.Count == 0)
            {
                return BadRequest(new UpdateTeamRolesResponse { IsSuccess = false, Message = "변경할 팀원 역할 데이터가 올바르지 않습니다." });
            }

            // 사용자 식별자 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamRolesResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 비즈니스 로직 실행
            var result = await _teamsService.UpdateTeamRolesAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 결과 반환
            return BadRequest(result);
        }

        // 팀원 가능 시간 설정 엔트포인트
        [Authorize]
        [HttpPost("{teamId}/timetable")]
        public async Task<ActionResult<SetAvailableTimesResponse>> SetAvailableTimes(int teamId, [FromBody] SetAvailableTimesRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new SetAvailableTimesResponse { IsSuccess = false, Message = "입력한 가능 시간 데이터 형식이 올바르지 않습니다." });
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new SetAvailableTimesResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            var result = await _teamsService.SetAvailableTimesAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}