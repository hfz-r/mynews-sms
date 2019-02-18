namespace StockManagementSystem.Core.Domain.Security
{
    /// <summary>
    /// Represents an entity which supports permission/ACL
    /// </summary>
    public interface IAclSupported
    {
        bool SubjectToAcl { get; set; }
    }
}