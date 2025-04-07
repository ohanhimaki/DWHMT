# Data Warehouse Modeling App: Documentation and Plan

## I. Introduction

This document outlines the plan and basic documentation for a web application designed to assist users in modeling data warehouse layers, specifically transitioning from a "source" (initially provided by the user) to a "target" layer. The application will allow users to input source table metadata, define target tables by mapping source columns, and export the target schema in various formats.

## II. User Workflow

1.  **Source Data Input:**
    * Upon opening the application with a fresh project, the user is presented with an input area.
    * The user is expected to paste tabular data with the following columns (in any order, but the application should guide the user on the expected format):
        * `TableName` (Name of the source table)
        * `ColumnName` (Name of the column in the source table)
        * `ExampleData` (A sample value or a comma-separated list of sample values for the column)
        * `RowCount` (Total number of rows in the source table)
        * `UniqueCount` (Number of unique values in the source column)
        * `NullCount` (Number of null values in the source column)
    * The data is expected to be tab-separated, with each row representing a column in a source table. The first row should be headers.
    * The application will parse this data to create an internal representation of the source tables and their columns with associated metadata.

2.  **Source Table and Column Listing:**
    * After successful parsing, the application will display a list of the identified source tables.
    * For each source table, users can view its columns and the associated metadata (example data, row count, unique count, null count).
    * The UI should allow users to easily browse and understand the structure and characteristics of their source data.

3.  **Target Table Definition and Mapping:**
    * Users can create new "target" tables.
    * For each target table, users can define columns.
    * The core functionality here is the ability to map source columns to target columns. This will involve:
        * Selecting a source table and a source column.
        * Specifying a name for the target column (which can be the same or different from the source column name).
        * Adding optional comments or descriptions for the target column.
        * Potentially defining data type transformations (this can be a future enhancement).
    * The UI should provide a clear way to visualize these mappings.

4.  **Target Export:**
    * **Excel Export:** Users should be able to export the defined target table schemas (table names, column names, and potentially descriptions/comments) to an Excel file.
    * **Direct SQL Queries:** The application should be able to generate basic SQL `CREATE TABLE` statements for the defined target tables, potentially including column names and basic data types (which might be inferred or user-defined in a future enhancement).

5.  **Project Export/Import (JSON):**
    * **Export:** Users can export the entire project as a JSON file. This file should contain:
        * The raw source data (as it was input).
        * The internal representation of the source tables and their metadata.
        * The definitions of the target tables, including their columns and the mappings to the source columns.
        * Any additional project-level settings or metadata.
    * **Import (Future Enhancement):** While not explicitly in the initial usage, the plan should consider the ability to import a previously exported JSON project to resume work.

## III. Technical Plan (Blazor with MudBlazor)

Given your choice of Blazor with MudBlazor, here's a high-level technical plan:

1.  **Project Setup:** Ensure you have a Blazor WebAssembly or Blazor Server project set up with the MudBlazor NuGet package installed and configured.

2.  **Data Models (C# Classes):**
    * **`SourceColumnInfo`:** Represents a single column from the input source data with properties for `TableName`, `ColumnName`, `ExampleData` (likely a `string`), `RowCount` (int), `UniqueCount` (int), and `NullCount` (int).
    * **`SourceTable`:** Represents a source table, containing a `TableName` and a `List<SourceColumnInfo>`.
    * **`TargetColumn`:** Represents a column in a target table with properties for `TargetTableName`, `TargetColumnName`, `SourceTableName` (nullable), `SourceColumnName` (nullable), and `Comment` (string).
    * **`TargetTable`:** Represents a target table with a `TargetTableName` and a `List<TargetColumn>`.
    * **`DataWarehouseProject`:** A container class to hold `List<SourceTable>` and `List<TargetTable>`, and potentially project-level metadata.

3.  **Blazor Components and Pages:**
    * **`Pages/InputSourceData.razor`:**
        * UI: `MudTextField` for pasting. `MudButton` to process. `MudAlert` for error messages.
        * Logic: Parse tab-separated data into `List<SourceColumnInfo>`, group into `List<SourceTable>`, handle errors, navigate to the next step.
    * **`Pages/ViewSourceData.razor`:**
        * UI: `MudList` or `MudExpansionPanel` to display `SourceTable` names. Inside, a `MudTable` to show `SourceColumnInfo` properties.
    * **`Pages/DefineTargetTables.razor`:**
        * UI: Add new `TargetTable` button. For each `TargetTable`: `MudTextField` for name, button to add `TargetColumn`. For each `TargetColumn`: dropdown/selection for source mapping, `MudTextField` for target name and comment.
        * Logic: Maintain `List<TargetTable>`, methods to add tables/columns, handle source-to-target mapping.
    * **`Pages/ExportTarget.razor`:**
        * UI: Buttons for "Export to Excel" and "Export to SQL".
        * Logic: Use a library for Excel generation. Generate `CREATE TABLE` statements as text.
    * **Potentially a main project page (`/`)** to guide the user through the workflow.

4.  **State Management:** For a simple application, manage state within components. For more complex, consider Fluxor or a scoped service.

5.  **JSON Serialization/Deserialization:** Use `System.Text.Json` for project export and import of the `DataWarehouseProject` object.

## IV. Detailed Feature Breakdown

**1. Source Data Input (`InputSourceData.razor`)**

* UI: `MudTextField` for pasting. `MudButton` to process. `MudAlert` for error messages.
* Logic:
    * Split pasted text by newline.
    * Read the first line as headers.
    * For each subsequent line, split by tab.
    * Map values to `SourceColumnInfo` properties based on headers. Handle errors.
    * Group `SourceColumnInfo` by `TableName` into `SourceTable`.
    * Store `List<SourceTable>` in state.
    * Navigate to `ViewSourceData.razor`.

**2. View Source Data (`ViewSourceData.razor`)**

* UI: `MudList` or `MudExpansionPanel` for `SourceTable` names. `MudTable` inside to show `SourceColumnInfo` (ColumnName, ExampleData, RowCount, UniqueCount, NullCount).

**3. Define Target Tables (`DefineTargetTables.razor`)**

* UI:
    * Button to add `TargetTable`.
    * For each `TargetTable`: `MudTextField` for `TargetTableName`, button to add `TargetColumn`.
    * For each `TargetColumn`: Dropdown/selection for source mapping (`SourceTable`, `SourceColumnName`), `MudTextField` for `TargetColumnName`, `MudTextField` for `Comment`.
* Logic: Maintain `List<TargetTable>`, methods to add tables/columns, handle mapping.

**4. Export Target (`ExportTarget.razor`)**

* UI: "Export to Excel" and "Export to SQL" buttons.
* Logic:
    * **Excel:** Iterate `List<TargetTable>`, create worksheets, write headers and `TargetColumn` data using a library.
    * **SQL:** Iterate `List<TargetTable>`, generate `CREATE TABLE` statements with `TargetTableName` and `TargetColumnName` (basic data types initially).

**5. Project Export/Import (`MainLayout.razor` or a dedicated page)**

* UI: "Export Project (JSON)" and (future) "Import Project (JSON)" buttons.
* Logic:
    * **Export:** Create `DataWarehouseProject` object, serialize to JSON using `System.Text.Json`, offer download.
    * **Import (Future):** Allow upload, deserialize JSON to `DataWarehouseProject`, populate state.

## V. Future Enhancements

* Data Type Mapping
* More Advanced Transformations
* Visual Mapping Interface
* Database Connection and Metadata Browsing
* Gold Layer Modeling
* Reverse Engineering
* Collaboration

## VI. Conclusion

This plan provides a solid starting point for developing your data warehouse modeling application. By focusing on the core user workflow and iteratively adding features, you can build a valuable tool for your needs. Remember to prioritize user experience and provide clear feedback throughout the application. Good luck!
