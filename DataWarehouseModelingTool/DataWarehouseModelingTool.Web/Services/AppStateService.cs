using System.Collections.Generic;
using System.Text.Json;
using Blazored.LocalStorage;
using DataWarehouseModelingTool.Web.Models;
using DataWarehouseModelingTool.Web.Pages;
using Microsoft.JSInterop;

public class AppStateData
{
    public List<SourceTable> SourceTables { get; set; } = new List<SourceTable>();
    public List<TargetTableDefinition> TargetTables { get; set; } = new List<TargetTableDefinition>();
    public List<ReportDefinition> ReportDefinitions { get; set; } = new List<ReportDefinition>();
    public List<TargetTableRelationship> TargetTableRelationships { get; set; } = new List<TargetTableRelationship>();
}
public class AppStateService : AppStateData
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private const string SourceTablesKey = "SourceTables";
    private const string TargetTablesKey = "TargetTables";
    private const string RelationsKey = "Relations";
    private const string ReportDefinitionsKey = "ReportDefinitions";

    public AppStateService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
        Initialize();

    }

    public bool IsLoading { get; set; } = false;
    public bool IsInitialized { get; set; } = false;
    // public List<SourceTable> SourceTables { get; set; } = new List<SourceTable>();
    // public List<TargetTableDefinition> TargetTables { get; set; } = new List<TargetTableDefinition>();
    // public List<ReportDefinition> ReportDefinitions { get; set; } = new List<ReportDefinition>();
    //
    // public List<TargetTableRelationship> TargetTableRelationships { get; set; } = new List<TargetTableRelationship>();

    public async Task SaveSourceTablesToLocalStorage()
    {
        await _localStorage.SetItemAsync(SourceTablesKey, JsonSerializer.Serialize(SourceTables));
    }


public async Task DownloadStateAsJson()
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new { SourceTables = SourceTables, TargetTables, ReportDefinitions, TargetTableRelationships });
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
                var state = JsonSerializer.Deserialize<AppStateData>(jsonString);
                if (state?.SourceTables != null)
                {
                    SourceTables = state.SourceTables;
                    TargetTables = state.TargetTables;
                    ReportDefinitions = state.ReportDefinitions;
                    TargetTableRelationships = state.TargetTableRelationships;
                    
                    await SaveSourceTablesToLocalStorage(); // Optionally save to local storage after upload
                    await SaveTargetTables(); // Optionally save target tables after upload
                    
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
    }

    
    public async Task SaveTargetTables()
    {
        await _localStorage.SetItemAsync(TargetTablesKey, TargetTables);
        await _localStorage.SetItemAsync(RelationsKey, TargetTableRelationships);
    }
    public async Task SaveReportDefinitions()
    {
        await _localStorage.SetItemAsync(ReportDefinitionsKey, ReportDefinitions);
    }

    public async Task Initialize()
    {
        if(IsInitialized) return;
        
            
        var storedSourceTables = await _localStorage.GetItemAsync<string>(SourceTablesKey);
        if (!string.IsNullOrEmpty(storedSourceTables))
        {
            SourceTables = JsonSerializer.Deserialize<List<SourceTable>>(storedSourceTables) ?? new List<SourceTable>();
        }
        var storedTargetTables = await _localStorage.GetItemAsync<string>("TargetTables");
        if (!string.IsNullOrEmpty(storedTargetTables))
        {
            TargetTables = JsonSerializer.Deserialize<List<TargetTableDefinition>>(storedTargetTables) ?? new List<TargetTableDefinition>();
        }
        var storedTargetTableRelationships = await _localStorage.GetItemAsync<string>("TargetTableRelationships");
        if (!string.IsNullOrEmpty(storedTargetTableRelationships))
        {
            TargetTableRelationships = JsonSerializer.Deserialize<List<TargetTableRelationship>>(storedTargetTableRelationships) ?? new List<TargetTableRelationship>();
        }
        var storedReportDefinitions = await _localStorage.GetItemAsync<string>(ReportDefinitionsKey);
        if (!string.IsNullOrEmpty(storedReportDefinitions))
        {
            ReportDefinitions = JsonSerializer.Deserialize<List<ReportDefinition>>(storedReportDefinitions) ?? new List<ReportDefinition>();
        }
        IsInitialized = true;
    }

    public async Task ClearProjectState()
    {
        SourceTables.Clear();
        await _localStorage.RemoveItemAsync(SourceTablesKey);
        TargetTables.Clear();
        await _localStorage.RemoveItemAsync(TargetTablesKey);
        TargetTableRelationships.Clear();
        await _localStorage.RemoveItemAsync(RelationsKey);
        ReportDefinitions.Clear();
        await _localStorage.RemoveItemAsync(ReportDefinitionsKey);
        IsInitialized = false;
        IsLoading = false;
        
    }
}
