using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Utils.EntityFramework.Dynamic
{
    [Table("test")]
    public class tnp
    {
        [Key]
        [Column("test")]
        public string Name { get; set; }

        public tnp()
        {
            
        }
    }


    public class tnpDbContextDynamic : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbSet<tnp> _1_5_table_eng { get; set; }


        public tnpDbContextDynamic()
        {
        }

        public tnpDbContextDynamic(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public tnpDbContextDynamic(DbContextOptions options, ILoggerFactory loggerFactory) : base(options)
        {
            _loggerFactory = loggerFactory;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            var connectionString = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}
