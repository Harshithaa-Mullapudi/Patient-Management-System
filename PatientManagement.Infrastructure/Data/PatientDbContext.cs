using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<PatientProfile> PatientProfiles { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Email)
                .IsRequired();

            entity.HasIndex(p => p.Email)
                .IsUnique();

            entity.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            entity.Property(p => p.DateOfBirth)
                .HasColumnType("date"); 
        });

        // PatientProfile configuration (One-to-One)
        modelBuilder.Entity<PatientProfile>(entity =>
        {
            entity.ToTable("PatientProfiles");

            entity.HasKey(pp => pp.Id);

            entity.HasOne(pp => pp.Patient)
                .WithOne(p => p.PatientProfile)
                .HasForeignKey<PatientProfile>(pp => pp.PatientId);

            entity.Property(pp => pp.Address)
                .HasMaxLength(200);

            entity.Property(pp => pp.City)
                .HasMaxLength(100);

            entity.Property(pp => pp.State)
                .HasMaxLength(100);

            entity.Property(pp => pp.PostalCode)
                .HasMaxLength(10);
        });

        // Appointment configuration (One-to-Many)
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");

            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            entity.Property(a => a.DoctorName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Reason)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();
        }); 
    }
}