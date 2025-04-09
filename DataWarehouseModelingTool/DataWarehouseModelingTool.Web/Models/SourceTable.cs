namespace DataWarehouseModelingTool.Web.Models;

public class SourceTable
{
    public string TableName { get; set; }
    public List<SourceColumnInfo> Columns { get; set; } = new List<SourceColumnInfo>();
    public string Comment { get; set; } 
}