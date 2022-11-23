using System.Collections.Generic;

namespace Septem.Utils.EntityFramework.Dynamic;

public class TableModel
{
    public string Name { get; set; }

    public ICollection<ColumnModel> Columns { get; set; }

    public TableModel()
    {
        Columns = new List<ColumnModel>();
    }

    public TableModel(string name)
    {
        Name = name;
        Columns = new List<ColumnModel>();
    }

    public TableModel AddColumn(string name, string type)
    {
        Columns.Add(new ColumnModel(name, type));
        return this;
    }

    public TableModel AddColumns(ICollection<ColumnModel> columns)
    {
        foreach (var columnModel in columns)
            Columns.Add(columnModel);
        return this;
    }
}