using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Domain
{
    public class Bill
    {
        public Guid Id { get; set; }
        public Guid? EnrollmentId { get; set; }
        public Guid? RejoinRecordId { get; set; }
        public decimal PackageAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal MedicineBillingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Enrollment? Enrollment { get; set; }
        public RejoinRecord? RejoinRecord { get; set; }


    }
}