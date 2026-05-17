using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamMatching.Shared.DTOs;
using TeamMatching.Web.Services;

namespace TeamMatching.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // 최종 오픈되는 기본 백엔드 API 루트 도메인 주소: /api/teams
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }
        // 팀 메인 화면 대시보드 데이터 종합 조회 API 엔드포인트
        [Authorize]
        [HttpGet("{teamId}")]
        public async Task<ActionResult<GetTeamMainResponse>> GetTeamMain(int teamId)
        {
            // 1. main 브랜치의 JWT 시스템 Claims 구조를 그대로 준수하여, 요청 헤더의 암호화된 토큰 속에서 로그인 세션 유저의 고유 ID(UserId)를 안전하게 디코딩 파싱해 추출합니다.
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new GetTeamMainResponse
                {
                    IsSuccess = false,
                    Message = "인증 자격 증명(토큰) 정보가 누락되었거나 만료되었습니다. 보안을 위해 다시 로그인해 주세요."
                });
            }

            // 2. 파싱해 낸 호출 유저 ID와 타겟 팀 ID를 비즈니스 서비스 레이어로 넘겨 소속 권한 및 데이터 취합 정렬 쿼리 수행
            var result = await _teamsService.GetTeamMainAsync(teamId, currentUserId);

            // 3. 비즈니스 로직 및 접근 검증 절차가 퍼펙트하게 통과하여 가공 처리가 끝난 경우
            if (result.IsSuccess)
            {
                return Ok(result); // HTTP 200 성공 코드 상태와 함께 명세서 규격의 직렬화 JSON 반환
            }

            // 타인의 팀 공간 무단 해킹 진입 차단 또는 팀 미조회 등의 실패 검증 상황 케어
            return BadRequest(result); // HTTP 400 에러 상태 코드와 함께 예외 사유가 담긴 JSON 객체 반환
        }
        // 팀명 변경 API 엔드포인트
        [Authorize]
        [HttpPut("{teamId}/name")]
        public async Task<ActionResult<UpdateTeamNameResponse>> UpdateTeamName(int teamId, [FromBody] UpdateTeamNameRequest request)
        {
            // 1. DTO 어노테이션 기반 입력 서식 자동 검증 (글자수 제한 등)
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdateTeamNameResponse { IsSuccess = false, Message = "팀 이름을 올바르게 입력해 주세요." });
            }

            // 2. main 브랜치 공통 보안 정책: 인증 토큰 내부 Claims 속성에서 요청 세션의 유저 고유 ID 파싱 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamNameResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰 자격 증명이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 3. 비즈니스 서비스 레이어로 매개변수 이관 및 권한/저장 실행
            var result = await _teamsService.UpdateTeamNameAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result); // 규격화된 성공 응답 JSON 모델 직렬화 반환
            }

            return BadRequest(result); // 권한 거부 사유가 명시된 실패 JSON 모델 반환
        }
        // 팀 내부 게시글 작성 API 엔드포인트
        [Authorize]
        [HttpPost("{teamId}/posts")]
        public async Task<ActionResult<CreateTeamPostResponse>> CreateTeamPost(int teamId, [FromBody] CreateTeamPostRequest request)
        {
            // 1. 모델 유효성 검증 (제목 100자 이하, 필수값 누락 등 체크)
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreateTeamPostResponse { IsSuccess = false, Message = "입력한 게시글 데이터 형식이 올바르지 않습니다." });
            }

            // 2. JWT 토큰에서 현재 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new CreateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다."
                });
            }

            // 3. 비즈니스 서비스 호출 (권한 검증 및 DB 저장)
            var result = await _teamsService.CreateTeamPostAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 권한이 없거나 저장에 실패한 경우
            return BadRequest(result);
        }
        [Authorize]
        [HttpPut("{teamId}/posts/{postId}")]
        public async Task<ActionResult<UpdateTeamPostResponse>> UpdateTeamPost(int teamId, int postId, [FromBody] UpdateTeamPostRequest request)
        {
            // 1. 모델 유효성 검증
            if (!ModelState.IsValid)
            {
                return BadRequest(new UpdateTeamPostResponse { IsSuccess = false, Message = "입력한 게시글 데이터 형식이 올바르지 않습니다." });
            }

            // 2. JWT 토큰에서 현재 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 3. 비즈니스 서비스 호출 (권한 검증 및 DB 수정)
            var result = await _teamsService.UpdateTeamPostAsync(teamId, postId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete("{teamId}/posts/{postId}")]
        public async Task<ActionResult<DeleteTeamPostResponse>> DeleteTeamPost(int teamId, int postId)
        {
            // 1. JWT 토큰에서 현재 사용자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new DeleteTeamPostResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 2. 비즈니스 서비스 호출 (권한 검증 및 DB 데이터 삭제 처리)
            var result = await _teamsService.DeleteTeamPostAsync(teamId, postId, currentUserId);

            // 3. 정상 처리 응답
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 4. 실패 처리 응답
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("{teamId}/posts/{postId}")]
        public async Task<ActionResult<GetTeamPostDetailResponse>> GetTeamPostDetail(int teamId, int postId)
        {
            // 1. main 브랜치 표준 Claims 체계를 가동하여 요청 헤더 토큰 세션에서 현재 호출 유저 ID 디코딩 후 확보
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new GetTeamPostDetailResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰 정보가 불완전합니다. 로그아웃 후 다시 재진입해 주세요."
                });
            }

            // 2. 서비스 비즈니스 가공단으로 식별 인자 값 전송 이관
            var result = await _teamsService.GetTeamPostDetailAsync(teamId, postId, currentUserId);

            // 3. 비즈니스 단 검증 절차가 성공적으로 수행되어 통과한 경우
            if (result.IsSuccess)
            {
                return Ok(result); // 규격화 직렬화 모델 응답 패키지 전송
            }

            // 부정한 경로 우회 접근자 필터링 결과 반환
            return BadRequest(result);
        }
        [Authorize]
        [HttpPost("{teamId}/posts/{postId}/comments")]
        public async Task<ActionResult<CreateTeamPostCommentResponse>> CreateTeamPostComment(int teamId, int postId, [FromBody] CreateTeamPostCommentRequest request)
        {
            // 1. 모델 유효성 검증 (빈 문자열, 500자 초과 방지)
            if (!ModelState.IsValid)
            {
                return BadRequest(new CreateTeamPostCommentResponse { IsSuccess = false, Message = "댓글 내용을 올바르게 입력해주세요." });
            }

            // 2. JWT 토큰에서 작성자(현재 로그인 유저) ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new CreateTeamPostCommentResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 3. 비즈니스 서비스 호출 (권한 검증 및 DB 저장)
            var result = await _teamsService.CreateTeamPostCommentAsync(teamId, postId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete("{teamId}")]
        public async Task<ActionResult<EndProjectResponse>> EndProject(int teamId)
        {
            // 1. JWT 토큰에서 현재 로그인 유저 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new EndProjectResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 2. 비즈니스 서비스 호출 (팀장 권한 검증 및 마감/종료 처리)
            var result = await _teamsService.EndProjectAsync(teamId, currentUserId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 권한 부족이나 잘못된 접근일 경우 400 Bad Request 반환
            return BadRequest(result);
        }
        [Authorize]
        [HttpPost("{teamId}/roles")]
        public async Task<ActionResult<UpdateTeamRolesResponse>> UpdateTeamRoles(int teamId, [FromBody] UpdateTeamRolesRequest request)
        {
            // 1. 모델 유효성 및 빈 배열 전송 방어
            if (!ModelState.IsValid || request.TeamRoles == null || request.TeamRoles.Count == 0)
            {
                return BadRequest(new UpdateTeamRolesResponse { IsSuccess = false, Message = "변경할 팀원 역할 데이터가 올바르지 않습니다." });
            }

            // 2. JWT 토큰에서 요청자 ID 추출
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var currentUserId))
            {
                return Unauthorized(new UpdateTeamRolesResponse
                {
                    IsSuccess = false,
                    Message = "인증 토큰이 유효하지 않습니다. 다시 로그인해 주세요."
                });
            }

            // 3. 비즈니스 서비스 호출 (팀장 권한 검증 및 DB 일괄 업데이트)
            var result = await _teamsService.UpdateTeamRolesAsync(teamId, currentUserId, request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            // 권한이 없거나 처리 중 실패한 경우 400 에러 반환
            return BadRequest(result);
        }
    }
}