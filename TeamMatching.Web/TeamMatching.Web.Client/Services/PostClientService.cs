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
            var response = await Http.GetAsync($"api/posts/{id}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetPostDetailResponse>();
        }

        public async Task<CreatePostResponse?> CreatePostAsync(CreatePostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync("api/posts", request);
            if (await HandleUnauthorizedAsync(response)) return null;
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
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<ApplyPostResponse>();
        }

        public async Task<GetApplicationsResponse?> GetApplicationsAsync(int postId)
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync($"api/posts/{postId}/applications");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetApplicationsResponse>();
        }

        public async Task<UpdatePostResponse?> UpdatePostAsync(int id, UpdatePostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PutAsJsonAsync($"api/posts/{id}", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<UpdatePostResponse>();
        }

        public async Task<DeletePostResponse?> DeletePostAsync(int id)
        {
            await SetAuthorizationHeader();
            var response = await Http.DeleteAsync($"api/posts/{id}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<DeletePostResponse>();
        }

        public async Task<AcceptMemberResponse?> AcceptMemberAsync(int postId, AcceptMemberRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/posts/{postId}/applications", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<AcceptMemberResponse>();
        }
    }
}
