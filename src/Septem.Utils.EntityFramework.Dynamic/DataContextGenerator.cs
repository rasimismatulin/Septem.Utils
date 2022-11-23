using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Septem.Utils.Helpers.Dynamic;

namespace Septem.Utils.EntityFramework.Dynamic;

public class DataContextGenerator
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private static readonly DataContextHashContainer DataContextHashContainer;
    private static AssemblyLoadContext _assemblyLoadContext;
    private static DatabaseModel _model;
    private static Assembly _assembly;

    static DataContextGenerator()
    {
        DataContextHashContainer = new DataContextHashContainer();
    }

    public DataContextGenerator(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<DataContextGenerator>();
    }

    public bool ValidateHash(string tableName, ICollection<KeyValuePair<string, string>> columns) =>
        DataContextHashContainer.ValidateHash(tableName, columns);

    public void Generate(DatabaseModel model)
    {
        var entitiesCode = new List<string>();
        var dbSetsCode = new StringBuilder();
        foreach (var table in model.Tables)
        {
            var columnsCode = new StringBuilder();
            foreach (var column in table.Columns)
            {
                var columnCode = CodeTemplates.EntityPropertyCodeTemplate(column.Type, column.Name);
                columnsCode.Append(columnCode);
            }
            var entityCode = CodeTemplates.EntityCodeTemplate(model.RootNameSpace, model.SchemaName, table.Name, table.Name, columnsCode.ToString());
            entitiesCode.Add(entityCode);
            var dbSetCode = CodeTemplates.DbSetCodeTemplate(table.Name, table.Name);
            dbSetsCode.Append(dbSetCode);
        }

        var dbContextCode = CodeTemplates.DbContextCodeTemplate(model.RootNameSpace, model.DataContextName, dbSetsCode.ToString());
        entitiesCode.Add(dbContextCode);
        using var peStream = new MemoryStream();


        var result = GenerateCode(entitiesCode).Emit(peStream);
        if (result.Success)
        {
            DataContextHashContainer.SaveHash(model);
            _model = model;

            if (_assemblyLoadContext != default)
            {
                _assemblyLoadContext.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            _assemblyLoadContext = new AssemblyLoadContext("DynamicDataContext", isCollectible: true);
            var cnt = AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.Contains("DynamicData"));
            _logger.LogInformation($"DynamicDataContext assembly count: {cnt}");
            peStream.Seek(0, SeekOrigin.Begin);
            _assembly = _assemblyLoadContext.LoadFromStream(peStream);
        }
        else
        {
            //File.WriteAllLines("C:\\temp\\generated.cs", entitiesCode);
            _logger.LogError("Error on generation data context. Source:");
            foreach (var code in entitiesCode)
                _logger.LogError(code);

            foreach (var resultDiagnostic in result.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error))
            {
                var location = resultDiagnostic.Location.ToString();
                var diagnostic = resultDiagnostic.ToString();
                _logger.LogError($"Location: {location}; Message: {diagnostic}");
            }
        }
    }


    public IDynamicQueryable GetQuery(string tableName)
    {
        if (_assembly == null)
            _logger.LogError($"Assembly is null!");

        var type = _assembly.GetType($"{_model.RootNameSpace}.{_model.DataContextName}");
        _ = type ?? throw new Exception("DataContext type not found");
        var constr = type.GetConstructor(new[] { typeof(ILoggerFactory) });
        _ = constr ?? throw new Exception("DataContext ctor not found");
        var dynamicContext = (DbContext)constr.Invoke(new object[] { _loggerFactory });
        var query = dynamicContext.Query($"{_model.RootNameSpace}.{CodeTemplates.NormalizeName(tableName)}");
        return new DynamicQueryable(query, dynamicContext);
    }


    private static CSharpCompilation GenerateCode(IEnumerable<string> sourceFiles)
    {
        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
        var parsedSyntaxTrees = sourceFiles.Select(f => SyntaxFactory.ParseSyntaxTree(f, options));
        return CSharpCompilation.Create($"DynamicDataContext.dll",
            parsedSyntaxTrees,
            references: CompilationReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }

    private static IEnumerable<MetadataReference> CompilationReferences()
    {
        var refs = new List<MetadataReference>();
        var ass = Assembly.GetExecutingAssembly();
        var referencedAssemblies = ass.GetReferencedAssemblies();
        refs.AddRange(referencedAssemblies.Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

        refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(System.Data.Common.DbConnection).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location));

        return refs;
    }

    public Type GetRuntimeType(string tableName)
    {
        var type = _assembly.GetType($"{_model.RootNameSpace}.{tableName}");
        return type;
    }
}