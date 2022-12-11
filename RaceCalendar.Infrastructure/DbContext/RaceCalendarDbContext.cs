using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaceCalendar.Domain.Models.Authentication;

namespace RaceCalendar.Infrastructure.DbContext
{
    public class RaceCalendarDbContext : IdentityDbContext<User>
    {
        public RaceCalendarDbContext() { }

        public RaceCalendarDbContext(DbContextOptions<RaceCalendarDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users.Users");
            builder.Entity<IdentityRole>().ToTable("Users.Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("Users.UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("Users.UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("Users.UserClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("Users.UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("Users.RoleClaims");
        }
    }
}
