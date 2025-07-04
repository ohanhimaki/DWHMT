using Blazored.LocalStorage;
using DataWarehouseModelingTool.Web.Models;
using DataWarehouseModelingTool.Web.Pages;
using Microsoft.JSInterop;
using System.Text.Json;
using static DataWarehouseModelingTool.Web.Pages.TableReferences;

namespace DataWarehouseModelingTool.Web.Services
{
    public class TableReferenceService : TableReferenceData
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;
        private const string ElementsKey = "Elements";
        public event Action OnChange;

        public TableReferenceService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
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

        /*
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
        */

        /*
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
        */
        public async Task<bool> CheckIfLocalStorageIsEmpty()
        {
            var storedElements = await _localStorage.GetItemAsync<string>(ElementsKey);
            if (storedElements == null)
            {
                Console.WriteLine("LocalStorage is empty");
                return true;
            }

            return false;
        }


        public async Task SaveData()
        {
            await _localStorage.SetItemAsync(ElementsKey, elements);
        }

        public async Task Initialize()
        {
            if (IsInitialized) return;
            var storedElements = await _localStorage.GetItemAsync<string>(ElementsKey);
            if (!string.IsNullOrEmpty(storedElements))
            {
                elements = JsonSerializer.Deserialize<List<Element>>(storedElements) ?? new List<Element>();
                foreach (var targetElement in elements) 
                {
                    foreach (var parentId in targetElement.parents) 
                    {
                        var sourceElement = elements.FirstOrDefault(x => x.id == parentId);

                        //switching references and referenced helps but elements are the wrong way around
                        sourceElement.references.Add(new Reference
                        {
                            id = sourceElement.id,
                            name = sourceElement.name,
                            type = sourceElement.type,
                            description = sourceElement.description,
                            sourceElement = sourceElement,
                            targetElement = targetElement
                        });

                        targetElement.referenced.Add(new Reference
                        {
                            id = targetElement.id,
                            name = targetElement.name,
                            type = targetElement.type,
                            description = targetElement.description,
                            sourceElement = sourceElement,
                            targetElement = targetElement
                        });
                    }
                }
            }
            IsInitialized = true;
            OnChange?.Invoke();
        }

        public async Task ClearProjectState()
        {
            elements.Clear();
            await _localStorage.RemoveItemAsync(ElementsKey);
            IsInitialized = false;
            IsLoading = false;

        }
    }


    public class TableReferenceData
    {
        public List<Element> elements { get; set; } = new List<Element>();

    }



}
