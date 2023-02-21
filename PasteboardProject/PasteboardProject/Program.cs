using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    
    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();
    
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
