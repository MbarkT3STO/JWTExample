using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTExample.Entities;
using JWTExample.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTExample.EFCore;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Seed some Roles
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Seed some Roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
        );

        // Seed some Users
        var passwordHash = new PasswordHasher<AppUser>();

        builder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = "1",
                UserName = "mbark",
                NormalizedUserName = "MBARK",
                Email = "mbark@localhost.com",
                NormalizedEmail = "MBARK@LOCALHOST.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHash.HashPassword(null, "P@ssw0rd"),
                SecurityStamp = string.Empty
            },
            new AppUser
            {
                Id = "2",
                UserName = "user",
                NormalizedUserName = "USER",
                Email = "user@localhost.com",
                NormalizedEmail = "USER@LOCALHOST.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHash.HashPassword(null, "P@ssw0rd"),
                SecurityStamp = string.Empty
            }
        );

        // Seed some UserRoles
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "1", UserId = "1" },
            new IdentityUserRole<string> { RoleId = "2", UserId = "2" }
        );

        // Seed some 10 Students
        builder.Entity<Student>().HasData(
            new Student { Id = 1, FirstName = "John", LastName = "Doe", Email = "student1@mail.com" },
            new Student { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "student2@mail.com" },
            new Student { Id = 3, FirstName = "Mary", LastName = "Doe", Email = "student3@mail.com" }
        );

        base.OnModelCreating(builder);
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
}