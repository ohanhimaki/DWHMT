namespace DataWarehouseModelingTool.Web.Models;

public class ReportDefinition
{
    public string ReportName { get; set; }
    public string ReportDescription { get; set; }
    public List<RequiredData> RequiredData { get; set; } = new List<RequiredData>();
    
}

public class RequiredData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
}