using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TeamMatching.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HttpClient 등록
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 인증 상태 관리 서비스 등록
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// 클라이언트 서비스 등록
builder.Services.AddScoped<ClientAuthService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ActivityService>();

await builder.Build().RunAsync();
