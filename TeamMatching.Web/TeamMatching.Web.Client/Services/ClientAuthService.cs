using System.Net.Http.Json;
using Microsoft.JSInterop;
using TeamMatching.Shared.DTOs;

namespace TeamMatching.Web.Client.Services
{
    public class ClientAuthService : BaseService
    {
        public ClientAuthService(HttpClient http, IJSRuntime js) : base(http, js) { }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await Http.PostAsJsonAsync("api/auth/login", request);
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
        {
            var response = await Http.PostAsJsonAsync("api/auth/signup", request);
            return await response.Content.ReadFromJsonAsync<RegisterResponse>();
        }
    }
}
