using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Client.Services
{
    public class ActivityClientService : BaseService
    {
        public ActivityClientService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<GetMyActivitiesResponse?> GetMyActivitiesAsync()
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync("api/my-activities");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetMyActivitiesResponse>();
        }

        public async Task<ClosePostResponse?> ClosePostAsync(ClosePostRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync("api/my-activities", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<ClosePostResponse>();
        }

        public async Task<GetReviewTargetResponse?> GetReviewTargetAsync(int teamId)
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync($"api/my-activities/review/{teamId}");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetReviewTargetResponse>();
        }

        public async Task<SubmitReviewResponse?> SubmitReviewAsync(int teamId, SubmitReviewRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PostAsJsonAsync($"api/my-activities/review/{teamId}", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<SubmitReviewResponse>();
        }
    }
}
