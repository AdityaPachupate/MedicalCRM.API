using CRM.API.Common.Enums;

namespace CRM.API.Domain
{
    public class Lead
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public LeadStatus Status { get; set; }
        public string Source { get; set; } = string.Empty;   // LookupValue.Code
        public string Reason { get; set; } = string.Empty;   // LookupValue.Code
        public DateTime CreatedAt { get; set; }
        public Guid? ClinicId { get; set; }                   // Future multi-clinic support
        public List<FollowUp> FollowUps { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<RejoinRecord> RejoinRecords { get; set; } = new();
    }
}