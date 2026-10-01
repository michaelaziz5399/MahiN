#region Using Directives
using MahiN.DataBase.Data;
using MahiN.DataBase.Models;
using MahiN.Mapper;
using MahiN.Scripts;
using MahiN.Services.BookingService;
using MahiN.Services.FileService;
using MahiN.Services.MatchService;
using MahiN.Services.PlaygroundService;
using MahiN.Services.RoleService;
using MahiN.Services.SubscriptionService;
using MahiN.Services.SuspensionService;
using MahiN.Services.TeamService;
using MahiN.Services.TournamentService;
using MahiN.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
#endregion

var builder = WebApplication.CreateBuilder(args);

#region Data Connection
var myConnection = builder.Configuration.GetConnectionString("MahiNDb")
  ?? throw new InvalidOperationException("Connection string 'MahiNDb' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(a => { a.UseSqlServer(myConnection); });
#endregion

#region Identity Configuration
builder.Services.AddIdentity<AppUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>();
#endregion

#region Authentication & JWT Bearer Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
    };
});
#endregion
// Add services to the container.
#region Services 
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IPlaygroundRepository, PlaygroundRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISuspensionRepository, SuspensionRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IUserService, UserService>();
// Add Hosted Services
builder.Services.AddHostedService<BookingAutoCompletedService>();
builder.Services.AddHostedService<SubscriptionExpirationService>();
builder.Services.AddHostedService<SuspensionCleanupHostedService>();
builder.Services.AddHttpContextAccessor();
#endregion

#region Mapper
// mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(UserProfile).Assembly);
    cfg.AddMaps(typeof(PlaygroundProfile).Assembly);
    cfg.AddMaps(typeof(BookingProfile).Assembly);
});

#endregion

builder.Services.AddControllers();

#region Add OpenAPI & CORS
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                    "https://st-goerge-club.vercel.app", // رابط موقع الفرونت 
                    "http://localhost:3000"          // React Localhost
               )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
#endregion

var app = builder.Build();

#region Medlewares
app.MapOpenApi();
app.MapScalarApiReference();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion