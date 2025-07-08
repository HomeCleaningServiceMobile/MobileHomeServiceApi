using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MHS.Common;
using MHS.Common.Enums;
using MHS.Repository.Models;

namespace MHS.Repository.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServicePackage> ServicePackages { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<WorkSchedule> WorkSchedules { get; set; }
    public DbSet<StaffSkill> StaffSkills { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }
    public DbSet<BookingImage> BookingImages { get; set; }
    public DbSet<StaffReport> StaffReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables with custom names if needed
        // ApplicationUser configurations
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(p => p.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)System.Enum.Parse(typeof(UserRole), v)
                )
                .HasDefaultValue(UserRole.Customer);
                
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserStatus)System.Enum.Parse(typeof(UserStatus), v)
                )
                .HasDefaultValue(UserStatus.Active);
        });
        
        // Customer configurations
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Staff configurations
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EmployeeId).IsUnique();
        });

        // Admin configurations
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EmployeeId).IsUnique();
        });

        // Service configurations
        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
            
            entity.Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (ServiceType)System.Enum.Parse(typeof(ServiceType), v)
                )
                .HasDefaultValue(ServiceType.HouseCleaning);
        });

        // ServicePackage configurations
        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // Booking configurations
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => e.BookingNumber).IsUnique();
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(10, 2);
            entity.Property(e => e.AddressLatitude).HasPrecision(10, 8);
            entity.Property(e => e.AddressLongitude).HasPrecision(11, 8);
            
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (BookingStatus)System.Enum.Parse(typeof(BookingStatus), v)
                )
                .HasDefaultValue(BookingStatus.Pending);
            
            // Configure relationships to avoid cascade conflicts
            entity.HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(b => b.Staff)
                .WithMany()
                .HasForeignKey(b => b.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment configurations
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.PaymentNumber).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.RefundAmount).HasPrecision(10, 2);
            
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentStatus)System.Enum.Parse(typeof(PaymentStatus), v)
                )
                .HasDefaultValue(PaymentStatus.Pending);
                
            entity.Property(p => p.Method)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentMethod)System.Enum.Parse(typeof(PaymentMethod), v)
                )
                .HasDefaultValue(PaymentMethod.Cash);
            
            entity.HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Review configurations - Fix cascade paths
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(r => r.Staff)
                .WithMany()
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StaffSkill configurations
        modelBuilder.Entity<StaffSkill>(entity =>
        {
            entity.HasIndex(e => new { e.StaffId, e.ServiceId }).IsUnique();
            
            entity.HasOne(ss => ss.Staff)
                .WithMany(s => s.StaffSkills)
                .HasForeignKey(ss => ss.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ss => ss.Service)
                .WithMany(s => s.StaffSkills)
                .HasForeignKey(ss => ss.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CustomerAddress configurations
        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.Property(e => e.Latitude).HasPrecision(10, 8);
            entity.Property(e => e.Longitude).HasPrecision(11, 8);
            
            entity.HasOne(ca => ca.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerPaymentMethod configurations
        modelBuilder.Entity<CustomerPaymentMethod>(entity =>
        {
            entity.HasOne(cpm => cpm.Customer)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(cpm => cpm.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BookingImage configurations
        modelBuilder.Entity<BookingImage>(entity =>
        {
            entity.HasOne(bi => bi.Booking)
                .WithMany(b => b.BookingImages)
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkSchedule configurations
        modelBuilder.Entity<WorkSchedule>(entity =>
        {
            entity.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (WorkScheduleStatus)System.Enum.Parse(typeof(WorkScheduleStatus), v)
                )
                .HasDefaultValue(WorkScheduleStatus.Available);
            
            entity.HasOne(ws => ws.Staff)
                .WithMany()
                .HasForeignKey(ws => ws.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ws => ws.Booking)
                .WithMany()
                .HasForeignKey(ws => ws.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification configurations
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (NotificationType)System.Enum.Parse(typeof(NotificationType), v)
                )
                .HasDefaultValue(NotificationType.SystemAlert);
            
            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Staff location configurations
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(e => e.CurrentLatitude).HasPrecision(10, 8);
            entity.Property(e => e.CurrentLongitude).HasPrecision(11, 8);
        });

        // Configure soft delete filter (User filter already set above)
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Staff>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Admin>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServicePackage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Booking>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WorkSchedule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StaffSkill>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CustomerAddress>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CustomerPaymentMethod>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookingImage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StaffReport>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = now;
            }
        }
    }
} 