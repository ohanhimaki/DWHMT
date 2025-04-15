namespace DataWarehouseModelingTool.Web.Models;

public static class TargetTableHelpers
{

    public static void MergeSourceColumns(this List<SourceColumnReference>? sourceColumns,
        SourceColumnInfo sourceColumnInfo)
    {
        if (sourceColumns == null)
        {
            return;
        }
        
        var sourceColumnReference = new SourceColumnReference
        {
            TableName = sourceColumnInfo.TableName,
            ColumnName = sourceColumnInfo.ColumnName
        };
        if (!sourceColumns.Contains(sourceColumnReference))
        {
            sourceColumns.Add(sourceColumnReference);
        }
        
    }
    
}