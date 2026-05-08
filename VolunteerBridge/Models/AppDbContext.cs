using Microsoft.EntityFrameworkCore;

namespace VolunteerBridge.Models
{
    public class AppDbContext :DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Acceptance> Acceptances { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<PointTransaction> pointTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=Maryam\SQLEXPRESS;Database=VolunteerBridge;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ServiceRequest>()
      .Property(s => s.EstimatedHours)
      .HasPrecision(4, 1);   
            modelBuilder.Entity<User>()
                .Property(u => u.AverageRating)
                .HasPrecision(3, 2);  
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.Requester)
                .WithMany(u => u.ServiceRequests)
                .HasForeignKey(r => r.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Acceptance>()
                .HasOne(a => a.Request)
                .WithMany(r => r.Acceptances)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Acceptance>()
                .HasOne(a => a.Volunteer)
                .WithMany(u => u.Acceptances)
                .HasForeignKey(a => a.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Acceptance)
                .WithMany(a => a.Ratings)
                .HasForeignKey(r => r.AcceptanceId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.FromUser)
                .WithMany(u => u.RatingsGiven)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.ToUser)
                .WithMany(u => u.RatingsReceived)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PointTransaction>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PointTransactions)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PointTransaction>()
                .HasOne(pt => pt.Acceptance)
                .WithMany(a => a.PointTransactions)
                .HasForeignKey(pt => pt.AcceptanceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
