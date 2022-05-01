using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ChatAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,IdentityRole<int>,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");

                entity.Property(s => s.Text).IsRequired().HasMaxLength(500);

            
            });

            builder.Entity<Room>(entity =>
            {
                entity.ToTable("Rooms");

                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(s => s.Admin)
                    .WithMany(u => u.Rooms)
                    .IsRequired();
            });


            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
