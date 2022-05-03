using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ChatAPI.Abstracts;
using ChatAPI.Concrete;
using ChatAPI.Data;
using ChatAPI.Hubs;
using ChatAPI.Jwt;
using ChatAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLayer.Repository.UnitOfWorks;

var builder = WebApplication.CreateBuilder(args);
string CustomCookieScheme = nameof(CustomCookieScheme);
string CustomTokenScheme = nameof(CustomTokenScheme);
// Add services to the container.
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IService<>),typeof(Service<>));
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));


builder.Services.AddDbContext<ApplicationDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).GetName().Name);
    });
});
builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();
//authen schema
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, CustomCookie>(Static.CustomCookieScheme, _ =>
    {

    })
    .AddJwtBearer(Static.CustomTokenScheme, o =>
    {
        o.Events = new()
        {
            OnMessageReceived = (context) =>
            {
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/protected")
                    || path.StartsWithSegments("/token"))
                {
                    var accessToken = context.Request.Query["access_token"];

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        // context.Token = accessToken;

                        var claims = new Claim[]
                        {
                            new("user_id", accessToken),
                            new("token", "token_claim"),
                        };
                        var identity = new ClaimsIdentity(claims,Static.CustomTokenScheme);
                        context.Principal = new(identity);
                        context.Success();
                    }
                }

                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("Cookie", pb => pb
        .AddAuthenticationSchemes(Static.CustomCookieScheme)
        .RequireAuthenticatedUser());

    c.AddPolicy("Token", pb => pb
        // schema get's ignored in signalr
        .AddAuthenticationSchemes(Static.CustomTokenScheme)
        .RequireClaim("token")
        .RequireAuthenticatedUser());
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

#region MyRegion

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidIssuer = "https://github.com/huseyinafsin",
//            ValidAudience = "https://github.com/huseyinafsin",
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey =
//                new SymmetricSecurityKey(
//                    Encoding.UTF8.GetBytes("MOGQodmQdS1DoPoSMkjB2tq4A7gr2ZMCmFso5swounToBXZCXfmXk6FdPvaHQ2l3")),
//            ClockSkew = TimeSpan.Zero
//        };
//    });

//.AddJwtBearer(options =>
//{

//    options.Authority = "https://github.com/huseyinafsin";
//    options.Events = new JwtBearerEvents()
//    {
//        OnMessageReceived = context =>
//        {
//            var accessToken = context.Request.Query["access_token"];
//            var path = context.HttpContext.Request.Path;
//            if (!string.IsNullOrEmpty(accessToken) &&
//                (path.StartsWithSegments("/chat")))
//            {
//                // Read the token out of the query string
//                context.Token = accessToken;
//            }
//            return Task.CompletedTask;
//        }
//    };
//});

#endregion
builder.Services.AddRazorPages();

builder.Services.AddCors(options
    => options.AddDefaultPolicy(policy =>
        policy
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(x => true)));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/protected", o =>
    {
        // o.Transports = HttpTransportType.LongPolling;
    });

    endpoints.Map("/get-cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        ctx.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(30)
        });
        return ctx.Response.WriteAsync("");
    });

    endpoints.Map("/token", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Token");

    endpoints.Map("/cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Cookie");
});

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapRazorPages();
//    app.MapControllers();
//    endpoints.MapHub<AuthHub>("/auth");
//    endpoints.MapHub<ChatHub>("/chat");

//});



app.Run();


public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
    }
}

public class CustomCookie : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public CustomCookie(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Cookies.TryGetValue("signalr-auth-cookie", out var cookie))
        {
            var claims = new Claim[]
            {
                new("user_id", cookie),
                new("cookie", "cookie_claim"),
            };
            var identity = new ClaimsIdentity(claims,Static.CustomCookieScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, new(), Static.CustomCookieScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("signalr-auth-cookie not found"));
    }
}
public static class Static
{
  public  const  string CustomCookieScheme = nameof(CustomCookieScheme);
  public  const string CustomTokenScheme = nameof(CustomTokenScheme);
}
