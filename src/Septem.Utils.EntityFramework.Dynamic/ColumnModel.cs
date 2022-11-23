namespace Septem.Utils.EntityFramework.Dynamic;

public class ColumnModel
{
    public string Name { get; set; }

    public string Type { get; set; }

    public ColumnModel()
    {
        
    }

    public ColumnModel(string name, string type)
    {
        Name = name;
        Type = type;
    }
}