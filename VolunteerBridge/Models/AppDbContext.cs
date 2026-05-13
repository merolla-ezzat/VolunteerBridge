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
        public DbSet<ChatMessage> ChatMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ServiceRequest>()
      .Property(s => s.EstimatedHours)
      .HasPrecision(4, 1);   
            modelBuilder.Entity<User>()
                .Property(u => u.AverageRating)
                .HasPrecision(3, 2);  
            // Modified by Yousef: enforce unique user emails at the database level to match the controller validation.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
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
            // chat message relationships: each message has one sender and one receiver, both are users. Deleting a user should not delete messages, but should prevent orphaned references.
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.ChatMessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ChatMessagesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
