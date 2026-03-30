namespace CRM.API.Domain
{
    public class LookupValue
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;    // "LeadSource" | "LeadReason"
        public string Code { get; set; } = string.Empty;         // "WalkIn" — used in API requests
        public string DisplayName { get; set; } = string.Empty;  // "Walk-In" — shown in UI
        public bool IsActive { get; set; } = true;               // Soft delete
        public DateTime CreatedAt { get; set; }
    }
}