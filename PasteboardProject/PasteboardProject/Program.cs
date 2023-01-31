using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NLog.Fluent;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Middlewares;
using PasteboardProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection).UseLowerCaseNamingConvention());
builder.Services.AddTransient<IRepository, PasteboardRepositoryPostgres>();
builder.Services.AddControllersWithViews();

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