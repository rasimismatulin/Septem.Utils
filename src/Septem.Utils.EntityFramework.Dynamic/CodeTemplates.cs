using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Septem.Utils.EntityFramework.Dynamic
{

    public static class CodeTemplates
    {
        public static string EntityPropertyCodeTemplate(string type, string name) => 
            $@"    {(name == "Uid" ? "[Key]" : $"[Column(\"{name}\")]")}public {type} {NormalizeName(name)} {{ get; set; }}{Environment.NewLine}";

        public static string NormalizeName(string name)
        {
            //var invalidSymbols = new[] { " ", ";", ".", ",", "-", "%", "?", "(", ")", "$", "₼", "{", "}", "/" };
            //foreach (var invalidSymbol in invalidSymbols)
            //{
            //    name = name.Replace(invalidSymbol, "_");
            //}

            return name;
        }

        public static string EntityCodeTemplate(string nameSpace, string schemaName, string tableName, string entityName, string columnsCode) => 
$@"using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {nameSpace};

[Table(""{tableName}"", Schema = ""{schemaName}"")]
public class {NormalizeName(entityName)}
{{
{columnsCode}
}}
";

        public static string DbSetCodeTemplate(string dbSetType, string dbSetName) => 
            $@"    public DbSet<{NormalizeName(dbSetType)}> {NormalizeName(dbSetName)} {{ get; set; }}{Environment.NewLine}";

        public static string DbContextCodeTemplate(string nameSpace, string dbContextName, string dbSetsCode) => $@"
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace {nameSpace};

public class {dbContextName} : DbContext
{{
    private readonly ILoggerFactory _loggerFactory;
    
{dbSetsCode}

    public {dbContextName}()
    {{
    }}

    public {dbContextName}(ILoggerFactory loggerFactory)
    {{
        _loggerFactory = loggerFactory;
    }}

    public {dbContextName}(DbContextOptions options, ILoggerFactory loggerFactory) : base(options)
    {{
        _loggerFactory = loggerFactory;
        AppContext.SetSwitch(""Npgsql.EnableLegacyTimestampBehavior"", true);
    }}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {{
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(""appsettings.json"")
            .Build()
            .GetConnectionString(""DefaultConnection"");

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }}
}}
";
    }
}
