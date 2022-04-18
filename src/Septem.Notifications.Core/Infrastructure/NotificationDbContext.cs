using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure;

internal class NotificationDbContext : DbContext
{
    public DbSet<NotificationEntity> Notifications { get; set; }

    public DbSet<NotificationReceiverEntity> NotificationReceivers { get; set; }

    public DbSet<NotificationTokenEntity> NotificationTokens { get; set; }

    public DbSet<NotificationMessageEntity> NotificationMessages { get; set; }


    public NotificationDbContext()
    {

    }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {

    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        using (var enumerator = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk =>
               {
                   if (!fk.IsOwnership)
                       return fk.DeleteBehavior == DeleteBehavior.Cascade;
                   return false;
               }).GetEnumerator())
        {
            while (enumerator.MoveNext())
                if (enumerator.Current != null)
                    enumerator.Current.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(modelBuilder);
    }
}