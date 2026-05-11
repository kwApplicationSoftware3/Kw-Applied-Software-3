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
            return await Http.GetFromJsonAsync<GetProfileResponse>("api/profile");
        }

        public async Task<UpdateProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await SetAuthorizationHeader();
            var response = await Http.PutAsJsonAsync("api/profile", request);
            return await response.Content.ReadFromJsonAsync<UpdateProfileResponse>();
        }
    }
}
