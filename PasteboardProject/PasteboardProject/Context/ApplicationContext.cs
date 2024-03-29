using Microsoft.EntityFrameworkCore;
using PasteboardProject.Models;

namespace PasteboardProject.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Pasteboard> Pasteboards { get; set; }
    public DbSet<PasteboardField> PasteboardFields { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PasteboardVisitor> PasteboardVisitors { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }
}