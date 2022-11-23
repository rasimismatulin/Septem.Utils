using System;
using System.Linq;

namespace Septem.Utils.Helpers.Dynamic;

public class DynamicQueryable : IDynamicQueryable
{
    private readonly IDisposable _context;
    public IQueryable Query { get; set; }
    public string OrderByType { get; set; }
    public string OrderBy { get; set; }


    public DynamicQueryable()
    {
        
    }

    public DynamicQueryable(IQueryable query, IDisposable context)
    {
        _context = context;
        Query = query;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}