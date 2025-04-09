namespace DataWarehouseModelingTool.Web.Models;

public class SourceColumnInfo
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string ExampleData { get; set; }
    public int RowCount { get; set; }
    public int UniqueCount { get; set; }
    public int NullCount { get; set; }
    
    // settings:
    
    public string? Comment { get; set; } // For user notes
    public bool IsSourceSystemKey { get; set; } = false; // To mark source system keys
    public bool IsPrimaryKey { get; set; } = false; // To mark potential primary keys in source

    public void Merge(SourceColumnInfo processedTableColumn)
    {
        if (processedTableColumn == null) return;

        ExampleData = processedTableColumn.ExampleData;
        RowCount = processedTableColumn.RowCount;
        UniqueCount = processedTableColumn.UniqueCount;
        NullCount = processedTableColumn.NullCount;
        
    }
}
