using System.Collections.Generic;
using System.Text.Json;
using Blazored.LocalStorage;
using DataWarehouseModelingTool.Web.Models;
using DataWarehouseModelingTool.Web.Pages;
using Microsoft.JSInterop;

public class AppStateService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private const string SourceTablesKey = "SourceTables";

    public AppStateService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
    }

    public List<SourceTable> SourceTables { get; set; } = new List<SourceTable>();

    public async Task SaveSourceTablesToLocalStorage()
    {
        await _localStorage.SetItemAsync(SourceTablesKey, JsonSerializer.Serialize(SourceTables));
    }

    public async Task ClearSourceTables()
    {
        // remove from app cache and localstorage
        SourceTables.Clear();
        await _localStorage.RemoveItemAsync(SourceTablesKey);
        
    }

public async Task DownloadStateAsJson()
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new { SourceTables = SourceTables });
            await _jsRuntime.InvokeVoidAsync("downloadFile", "application/json", "project_state.json", jsonString);
            Console.WriteLine("Project state downloaded as JSON.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading state as JSON: {ex.Message}");
        }
    }

    public async Task UploadStateFromJson(string jsonString)
    {
        try
        {
            if (!string.IsNullOrEmpty(jsonString))
            {
                var state = JsonSerializer.Deserialize<ProjectState>(jsonString);
                if (state?.SourceTables != null)
                {
                    SourceTables = state.SourceTables;
                    await SaveSourceTablesToLocalStorage(); // Optionally save to local storage after upload
                    Console.WriteLine("Project state uploaded from JSON.");
                }
                else
                {
                    Console.WriteLine("Uploaded JSON did not contain valid SourceTables data.");
                }
            }
            else
            {
                Console.WriteLine("No JSON string provided for upload.");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON for upload: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading state from JSON: {ex.Message}");
        }
    }

    public async Task LoadDataFromLocalStorage()
    {
        var storedSourceTables = await _localStorage.GetItemAsync<string>(SourceTablesKey);
        if (!string.IsNullOrEmpty(storedSourceTables))
        {
            SourceTables = JsonSerializer.Deserialize<List<SourceTable>>(storedSourceTables) ?? new List<SourceTable>();
        }
    }
}
