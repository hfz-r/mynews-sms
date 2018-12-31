namespace StockManagementSystem.Core.Domain.Security
{
    /// <summary>
    /// Represents an entity which supports Permissions
    /// </summary>
    public interface IAclSupported
    {
        bool SubjectToAcl { get; set; }
    }
}