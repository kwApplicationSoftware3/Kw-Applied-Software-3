using Microsoft.JSInterop;

namespace TeamMatching.Web.Services
{
    public class TokenService : ITokenService
    {
        private const string TokenKey = "authToken";
        private readonly IJSRuntime _js;

        public TokenService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }

        public async Task RemoveTokenAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }
}
