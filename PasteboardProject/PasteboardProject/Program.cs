using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NLog.Fluent;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Middlewares;
using PasteboardProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionPostgres = builder.Configuration.GetConnectionString("PostgresConnection");
var connectionMsSql = builder.Configuration.GetConnectionString("SqlConnection");
// builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionMsSql));
// builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionPostgres).UseLowerCaseNamingConvention());
//  builder.Services.AddTransient(typeof(IRepository),typeof(PasteboardRepositoryPostgres));
//  builder.Services.AddTransient(typeof(IRepository),typeof(PasteboardRepositorySql));
//  builder.Services.AddTransient(typeof(IRepository),typeof(PasteboardRepositoryJson));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ContextFactory>();
builder.Services.AddScoped<PasteboardRepositoryPostgres>()
    .AddScoped<IRepository, PasteboardRepositoryPostgres>(r => r.GetService<PasteboardRepositoryPostgres>());
 builder.Services.AddScoped<PasteboardRepositoryJson>()
     .AddScoped<IRepository, PasteboardRepositoryJson>(r => r.GetService<PasteboardRepositoryJson>());


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

app.UseMiddleware<ResponseTimeMiddleware>();

app.MapControllers();

app.Run();