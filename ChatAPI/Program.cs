using System.Reflection;
using System.Text;
using ChatAPI.Abstracts;
using ChatAPI.Concrete;
using ChatAPI.Data;
using ChatAPI.Hubs;
using ChatAPI.Jwt;
using ChatAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLayer.Repository.UnitOfWorks;

var builder = WebApplication.CreateBuilder(args);

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
#region MyRegion

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
builder.Services.AddSignalR();

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
    app.MapControllers();
    endpoints.MapHub<AuthHub>("/auth");
    endpoints.MapHub<ChatHub>("/chat");

});



app.Run();

