using System;
using System.Reflection;
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
            .AddJsonFile("appsettings.notifications.designtime.json")
            .Build()
            .GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                    property.SetValueConverter(new DateTimeOffsetToUtcConverter());
                if (property.ClrType == typeof(DateTimeOffset?))
                    property.SetValueConverter(new NullableDateTimeOffsetToUtcConverter());
            }
        }
        base.OnModelCreating(modelBuilder);
    }
}