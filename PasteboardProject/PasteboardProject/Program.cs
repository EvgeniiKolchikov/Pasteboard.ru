using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Middlewares;
using PasteboardProject.Repositories;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var connection = builder.Configuration.GetConnectionString("PostgresConnection");
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseNpgsql(connection).UseLowerCaseNamingConvention());
    builder.Services.AddTransient<IPasteboardRepository, PasteboardRepositoryPostgres>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddControllersWithViews();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // указывает, будет ли валидироваться издатель при валидации токена
                ValidateIssuer = true,
                // строка, представляющая издателя
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                // будет ли валидироваться потребитель токена
                ValidateAudience = true,
                // установка потребителя токена
                ValidAudience = builder.Configuration["Jwt:Audience"],
                // будет ли валидироваться время существования
                ValidateLifetime = true,
                // установка ключа безопасности
                IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                // валидация ключа безопасности
                ValidateIssuerSigningKey = true,
            };
        });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    
    
    
    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    });
    
    app.UseRouting();
    
    app.Use(async (context, next) =>
    {
        var token = context.Request.Cookies[".AspNetCore.Cookies"];
        if (!string.IsNullOrEmpty(token))
            context.Request.Headers.Add("Authorization", "Bearer " + token);
 
        await next();
    });
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseMiddleware<ResponseTimeMiddleware>();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Stopped exception");
}

finally
{
    logger.Debug("main stopped");
    LogManager.Shutdown();
}
