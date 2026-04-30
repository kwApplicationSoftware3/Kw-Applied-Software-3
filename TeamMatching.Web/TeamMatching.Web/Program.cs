using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamMatching.Web.Client.Pages;
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

// 의존성 주입(DI) 등록
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostsService, PostsService>();


var JWTString = Environment.GetEnvironmentVariable("JWT_SECRET");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TeamMatchingWeb";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TeamMatchingUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // 1. 보안팀 고용
.AddJwtBearer(options => // 2. JWT 스캐너 지급 및 규칙 세팅
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // 위조 검사 켜기
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTString)), // 이 키로 검사해!
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
