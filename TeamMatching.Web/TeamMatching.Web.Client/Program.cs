using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HttpClient 등록: 서버 API와 통신하기 위해 필수입니다.
// BaseAddress는 앱이 실행되는 현재 서버 주소로 자동 설정됩니다.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
