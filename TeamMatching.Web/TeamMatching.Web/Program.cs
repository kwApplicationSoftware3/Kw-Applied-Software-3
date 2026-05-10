using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamMatching.Web.Client.Services;
using TeamMatching.Web.Components;
using TeamMatching.Web.Data;
using TeamMatching.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers(); // API 컨트롤러 기능 활성화

// HttpClient 등록 (서버 측에서도 CustomAuthenticationStateProvider 등에서 필요로 함)
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:7141") 
});

// 인증 상태 관리 서비스 등록 (서버/SSR용)
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// 의존성 주입(DI) 등록
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMyActivitiesService, MyActivitiesService>();

var JWTString = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "YourSuperSecretKeyForJWTAuth2026!TeamMatching";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TeamMatchingWeb";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TeamMatchingUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTString)),
        ValidateLifetime = true, // 만료 시간 검사 켜기
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience
    };
});

// Get Connection String from Environment Variable
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers(); // API 컨트롤러 매핑 추가

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TeamMatching.Web.Client._Imports).Assembly);

app.Run();
