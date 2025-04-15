namespace DataWarehouseModelingTool.Web.Models;

public class TargetTableDefinition
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ColumnMapping> ColumnMappings { get; set; } = new List<ColumnMapping>();
    
    public List<Guid> RequiredDataIds { get; set; } = new List<Guid>();

    public void AddColumnFromSourceColumns(List<SourceColumnInfo> listOfSourceColumns)
    {
        
        var sourceColumnReferences = new List<SourceColumnReference>();
        foreach (var sourceColumn in listOfSourceColumns)
        {
            var sourceColumnReference = new SourceColumnReference
            {
                TableName = sourceColumn.TableName,
                ColumnName = sourceColumn.ColumnName
            };

            if (!sourceColumnReferences.Contains(sourceColumnReference))
            {
                sourceColumnReferences.Add(sourceColumnReference);
            }
        }
        var sourceColumnName = (listOfSourceColumns.Aggregate("", (current, sourceColumn) => current + $"{sourceColumn.TableName}.{sourceColumn.ColumnName}, ")).TrimEnd(',', ' ');
        var columnMapping = new ColumnMapping
        {
            SourceColumns = sourceColumnReferences,
            TargetColumnName = sourceColumnName,
            TargetDataType = null, // Set default or leave null
            IsKey = false,
            AllowNulls = false,
            TrimWhitespace = false // Default value
        };

        ColumnMappings.Add(columnMapping);
    }
}

public class SourceColumnReference
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SourceColumnReference other = (SourceColumnReference)obj;
        return TableName == other.TableName && ColumnName == other.ColumnName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TableName, ColumnName);
    }
}

public class ColumnMapping
{
    public List<SourceColumnReference> SourceColumns { get; set; }

    public string SourceColumnNames => SourceColumns.Aggregate("",
        (current, sourceColumn) => current + $"{sourceColumn.TableName}.{sourceColumn.ColumnName}, ");
    public string TargetColumnName { get; set; }
    public string? TargetDataType { get; set; }
    public bool IsKey { get; set; } = false; 
    public bool AllowNulls { get; set; } = true;
    public bool TrimWhitespace { get; set; } = false;
    public string Description { get; set; } = string.Empty; // For user notes

    
}

public class TargetTableRelationship
{
    public string SourceTableName { get; set; } // Name of one TargetTableDefinition
    public string SourceColumnName { get; set; } // Name of a column in SourceTableName
    public string TargetTableName { get; set; } // Name of the other TargetTableDefinition
    public string TargetColumnName { get; set; } // Name of a column in TargetTableName
    public string RelationshipType { get; set; } // "one-to-one", "one-to-many", "many-to-many"
}