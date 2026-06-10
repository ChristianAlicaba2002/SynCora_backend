using System;
using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.Models;

namespace syncora_server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<FollowRequest> FollowRequests { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FollowRequest>()
            .HasOne(fr => fr.Sender)
            .WithMany(u => u.SentFollowRequests)
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FollowRequest>()
            .HasOne(fr => fr.Receiver)
            .WithMany(u => u.ReceivedFollowRequests)
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FollowRequest>()
            .HasIndex(fr => new { fr.SenderId, fr.ReceiverId })
            .IsUnique()
            .HasFilter($"[{nameof(FollowRequest.Status)}] = {(int)FollowRequestStatus.Pending}");

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FollowingId })
            .IsUnique();
    }
}