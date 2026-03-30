using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Domain
{
    public class RejoinRecord
    {
        public Guid Id { get; set; }
        public Guid LeadId { get; set; }
        public Guid PackageId { get; set; }
        public decimal PackageCostSnapshot { get; set; }
        public int PackageDurationSnapshot { get; set; }          // Validated: must be 5, 10, or 20
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Lead Lead { get; set; } = null!;
        public Package Package { get; set; } = null!;
        public Bill? Bill { get; set; }
    }
}