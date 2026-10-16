using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base (options)
        {
        }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Race> Races => Set<Race>(); 
    public DbSet<Participant > Participants => Set<Participant>();
    public DbSet<Organizer> Organizers => Set<Organizer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Registration> Registrations => Set<Registration>();

    public DbSet<Result> Results => Set<Result>();
    public DbSet<RaceCategory> RaceCategories => Set<RaceCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RaceCategory>()
        .HasKey(rc => new { rc.RaceID, rc.CategoryID });

        modelBuilder.Entity<RaceCategory>()
            .ToTable("RaceCategories");

        modelBuilder.Entity<User>()
            .Property(u => u.Contact_Info)
            .HasColumnName("ContactInfo");

            modelBuilder.Entity<User>()
            .Property(u => u.User_Type)
            .HasColumnName("UserType");
    }

}



