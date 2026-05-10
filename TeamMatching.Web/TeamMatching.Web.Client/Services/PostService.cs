using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Web.Client.Services
{
    public class PostService : BaseService
    {
        public PostService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<GetPostsResponse?> GetPostsAsync()
        {
            return await Http.GetFromJsonAsync<GetPostsResponse>("api/posts");
        }

        public async Task<GetPostDetailResponse?> GetPostDetailAsync(int id)
        {
            await SetAuthorizationHeader(); // 로그인 상태면 내 지원 현황 등을 알기 위해 헤더 설정
            return await Http.GetFromJsonAsync<GetPostDetailResponse>($"api/posts/{id}");
        }

        public async Task<CreatePostResponse?> CreatePostAsync(CreatePostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync("api/posts", request);
            return await response.Content.ReadFromJsonAsync<CreatePostResponse>();
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await Http.GetFromJsonAsync<List<Tag>>("api/tags") ?? new();
        }

        public async Task<ApplyPostResponse?> ApplyPostAsync(int postId, ApplyPostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/posts/{postId}/apply", request);
            return await response.Content.ReadFromJsonAsync<ApplyPostResponse>();
        }
    }
}
