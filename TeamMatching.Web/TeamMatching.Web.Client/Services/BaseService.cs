using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace TeamMatching.Web.Client.Services
{
    /// <summary>
    /// API 통신의 기본 기능을 제공하는 베이스 서비스
    /// </summary>
    public abstract class BaseService
    {
        protected readonly HttpClient Http;
        protected readonly IJSRuntime JS;

        protected BaseService(HttpClient http, IJSRuntime js)
        {
            Http = http;
            JS = js;
        }

        /// <summary>
        /// localStorage에서 토큰을 읽어 Authorization 헤더를 설정합니다.
        /// </summary>
        protected async Task SetAuthorizationHeader()
        {
            var token = await JS.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Http.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// 401 Unauthorized 응답 시 토큰을 삭제하고 로그인 페이지로 이동합니다.
        /// </summary>
        protected async Task<bool> HandleUnauthorizedAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await JS.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await JS.InvokeVoidAsync("eval", "window.location.href='/login'");
                return true;
            }
            return false;
        }
    }
}
