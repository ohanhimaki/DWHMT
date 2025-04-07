using System.Collections.Generic;
using System.Text.Json;
using Blazored.LocalStorage;
using DataWarehouseModelingTool.Web.Models;
using DataWarehouseModelingTool.Web.Pages;

public class AppStateService
{
    private readonly ILocalStorageService _localStorage;
    private const string SourceTablesKey = "SourceTables";

    public AppStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public List<SourceTable> SourceTables { get; set; } = new List<SourceTable>();

    public async Task SaveSourceTablesToLocalStorage()
    {
        await _localStorage.SetItemAsync(SourceTablesKey, JsonSerializer.Serialize(SourceTables));
    }

    public async Task LoadSourceTablesFromLocalStorage()
    {
        var storedSourceTables = await _localStorage.GetItemAsync<string>(SourceTablesKey);
        if (!string.IsNullOrEmpty(storedSourceTables))
        {
            SourceTables = JsonSerializer.Deserialize<List<SourceTable>>(storedSourceTables) ?? new List<SourceTable>();
        }
    }

    public void ClearSourceTables()
    {
        SourceTables.Clear();
    }
}
