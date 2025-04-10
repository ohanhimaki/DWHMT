namespace DataWarehouseModelingTool.Web.Models;

public class TargetTableDefinition
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ColumnMapping> ColumnMappings { get; set; } = new List<ColumnMapping>();
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
    public SourceColumnReference Source { get; set; }
    public string TargetColumnName { get; set; }
    public string? TargetDataType { get; set; }
    public bool IsKey { get; set; } = false; 
    public bool AllowNulls { get; set; } = true;
    public bool TrimWhitespace { get; set; } = false;

    
}

public class TargetTableRelationship
{
    public string SourceTableName { get; set; } // Name of one TargetTableDefinition
    public string SourceColumnName { get; set; } // Name of a column in SourceTableName
    public string TargetTableName { get; set; } // Name of the other TargetTableDefinition
    public string TargetColumnName { get; set; } // Name of a column in TargetTableName
    public string RelationshipType { get; set; } // "one-to-one", "one-to-many", "many-to-many"
}