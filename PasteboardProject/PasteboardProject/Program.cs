using Microsoft.EntityFrameworkCore;
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
    builder.Services.AddTransient<IRepository, PasteboardRepositoryPostgres>();
    builder.Services.AddControllersWithViews();
    builder.Services.AddAuthentication("Bearer").AddJwtBearer();
    builder.Services.AddAuthorization();
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

    app.UseAuthorization();
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

