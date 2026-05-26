using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Client.Services
{
    public class ProfileClientService : BaseService
    {
        public ProfileClientService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<GetProfileResponse?> GetProfileAsync()
        {
            await SetAuthorizationHeader();
            var response = await Http.GetAsync("api/profile");
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<GetProfileResponse>();
        }

        public async Task<UpdateProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PutAsJsonAsync("api/profile", request);
            if (await HandleUnauthorizedAsync(response)) return null;
            return await response.Content.ReadFromJsonAsync<UpdateProfileResponse>();
        }
    }
}
