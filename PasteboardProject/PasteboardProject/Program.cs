using Microsoft.EntityFrameworkCore;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection).UseLowerCaseNamingConvention());
builder.Services.AddTransient<IRepository, PasteboardRepositoryPostgres>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pasteboard}/{action=ShowPasteboard}/{id?}");
app.MapControllerRoute(
    name: "short",
    pattern: "{id}",
    defaults: new { controller = "Pasteboard", action = "ShowPasteboard" });

app.Run();