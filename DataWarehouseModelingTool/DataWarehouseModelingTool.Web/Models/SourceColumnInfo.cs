namespace DataWarehouseModelingTool.Web.Models;

public class SourceColumnInfo
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string ExampleData { get; set; }
    public int RowCount { get; set; }
    public int UniqueCount { get; set; }
    public int NullCount { get; set; }
}
