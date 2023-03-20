using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PasteboardProject.Interfaces;

namespace PasteboardProject.Services;

public class TokenService
{
    private const int ExpireTokenTime = 60;
    private const int ExpireCookieTime = 60;
    private static string GenerateToken(ITokenGenerated user, IConfiguration configuration)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        if (user.Email == "admin@pasteboard.ru")
        {
            claims.Add(new Claim(ClaimTypes.Role,"Administrator"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role,"User"));
        }
        
        var keybytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
        var jwt = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(ExpireTokenTime)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keybytes),
                SecurityAlgorithms.HmacSha256));
            
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    public static void AddTokenToCookie(ITokenGenerated user, IConfiguration configuration, HttpContext context)
    {
        DeleteTokenFromCookie(context);
        var token = GenerateToken(user, configuration);
        context.Response.Cookies.Append(".AspNetCore.PasteboardCookie", token, 
            new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(ExpireCookieTime)
            });
    }
    public static void DeleteTokenFromCookie(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey(".AspNetCore.PasteboardCookie"))
        {
            context.Response.Cookies.Delete(".AspNetCore.PasteboardCookie");
        }
    }
}