using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Client.Services
{
    public class ActivityService : BaseService
    {
        public ActivityService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<GetMyActivitiesResponse?> GetMyActivitiesAsync()
        {
            await SetAuthorizationHeader();
            return await Http.GetFromJsonAsync<GetMyActivitiesResponse>("api/my-activities");
        }

        public async Task<ClosePostResponse?> ClosePostAsync(ClosePostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync("api/my-activities", request);
            return await response.Content.ReadFromJsonAsync<ClosePostResponse>();
        }

        public async Task<GetReviewTargetResponse?> GetReviewTargetAsync(int teamId)
        {
            await SetAuthorizationHeader();
            return await Http.GetFromJsonAsync<GetReviewTargetResponse>($"api/my-activities/review/{teamId}");
        }

        public async Task<SubmitReviewResponse?> SubmitReviewAsync(int teamId, SubmitReviewRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/my-activities/review/{teamId}", request);
            return await response.Content.ReadFromJsonAsync<SubmitReviewResponse>();
        }
    }
}
