using DataWarehouseModelingTool.Web.Models;

namespace DataWarehouseModelingTool.Web.Services;

public static class AppStateHelpers
{
    
    public static async Task MergeSourceTables(this AppStateService appStateService, List<SourceTable> processedTables)
    {
        // Merge the processed tables into the existing SourceTables list
        foreach (var processedTable in processedTables)
        {
            var existingTable = appStateService.SourceTables.FirstOrDefault(t => t.TableName == processedTable.TableName);
            if (existingTable != null)
            {
                foreach (var processedTableColumn in processedTable.Columns)
                {
                    var existingColumn = existingTable.Columns.FirstOrDefault(c => c.ColumnName == processedTableColumn.ColumnName);
                    if (existingColumn != null)
                    {
                        existingColumn.Merge(processedTableColumn);
                    }
                    else
                    {
                        // Add new column to the existing table
                        existingTable.Columns.Add(processedTableColumn);
                    }
                    
                }

            }
            else
            {
                // Add new table
                appStateService.SourceTables.Add(processedTable);
            }
        }

        await appStateService.SaveSourceTablesToLocalStorage();
    }
    
}