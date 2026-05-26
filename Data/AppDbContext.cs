using System;
using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.Models;

namespace syncora_server.Data;

public class AppDbContext: DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

   public DbSet<User> Users { get; set; }
}