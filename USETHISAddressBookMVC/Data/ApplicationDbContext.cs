using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using USETHISAddressBookMVC.Models;

namespace USETHISAddressBookMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<USETHISAddressBookMVC.Models.Category>? Category { get; set; }
        public DbSet<USETHISAddressBookMVC.Models.Contact>? Contact { get; set; }
    }
}