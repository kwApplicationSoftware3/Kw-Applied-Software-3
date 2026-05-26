using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Client.Services
{
    public class TeamClientService : BaseService
    {
        public TeamClientService(HttpClient http, IJSRuntime js) : base(http, js) { }

        // 팀 메인화면 데이터 조회
        public async Task<GetTeamMainResponse?> GetTeamMainAsync(int teamId)
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync($"api/teams/{teamId}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetTeamMainResponse>();
        }

        // 팀명 변경
        public async Task<UpdateTeamNameResponse?> UpdateTeamNameAsync(int teamId, UpdateTeamNameRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PutAsJsonAsync($"api/teams/{teamId}", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<UpdateTeamNameResponse>();
        }

        // 팀 게시글 상세 조회
        public async Task<GetTeamPostDetailResponse?> GetTeamPostDetailAsync(int teamId, int postId)
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync($"api/teams/{teamId}/posts/{postId}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetTeamPostDetailResponse>();
        }

        // 팀 게시글 작성
        public async Task<CreateTeamPostResponse?> CreateTeamPostAsync(int teamId, CreateTeamPostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/teams/{teamId}/posts", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<CreateTeamPostResponse>();
        }

        // 팀 게시글 수정
        public async Task<UpdateTeamPostResponse?> UpdateTeamPostAsync(int teamId, int postId, UpdateTeamPostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PutAsJsonAsync($"api/teams/{teamId}/posts/{postId}", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<UpdateTeamPostResponse>();
        }

        // 팀 게시글 삭제
        public async Task<DeleteTeamPostResponse?> DeleteTeamPostAsync(int teamId, int postId)
        {
            await SetAuthorizationHeader();
            var response = await Http.DeleteAsync($"api/teams/{teamId}/posts/{postId}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<DeleteTeamPostResponse>();
        }

        // 댓글 작성
        public async Task<CreateTeamPostCommentResponse?> CreateCommentAsync(int teamId, int postId, CreateTeamPostCommentRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/teams/{teamId}/posts/{postId}", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<CreateTeamPostCommentResponse>();
        }

        // 팀원 역할 변경
        public async Task<UpdateTeamRolesResponse?> UpdateTeamRolesAsync(int teamId, UpdateTeamRolesRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/teams/{teamId}/team-role-update", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<UpdateTeamRolesResponse>();
        }

        // 프로젝트 종료
        public async Task<EndProjectResponse?> EndProjectAsync(int teamId)
        {
            await SetAuthorizationHeader();
            var response = await Http.DeleteAsync($"api/teams/{teamId}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<EndProjectResponse>();
        }
    }
}
