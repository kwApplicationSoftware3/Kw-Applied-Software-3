using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Web.Client.Services
{
    public class PostClientService : BaseService
    {
        public PostClientService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<GetPostsResponse?> GetPostsAsync()
        {
            return await Http.GetFromJsonAsync<GetPostsResponse>("api/posts");
        }

        public async Task<GetPostDetailResponse?> GetPostDetailAsync(int id)
        {
            await SetAuthorizationHeader();
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
            return await Http.GetFromJsonAsync<List<Tag>>("api/tags") ?? [];
        }

        public async Task<ApplyPostResponse?> ApplyPostAsync(int postId, ApplyPostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/posts/{postId}/apply", request);
            return await response.Content.ReadFromJsonAsync<ApplyPostResponse>();
        }

        public async Task<GetApplicationsResponse?> GetApplicationsAsync(int postId)
        {
            await SetAuthorizationHeader();
            return await Http.GetFromJsonAsync<GetApplicationsResponse>($"api/posts/{postId}/applications");
        }
    }
}
