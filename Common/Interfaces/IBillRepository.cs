using CRM.API.Domain;

namespace CRM.API.Common.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> CreateBillWithItemsAsync(
            Bill bill, 
            IEnumerable<(Guid MedicineId, int Quantity)> medicineItems, 
            CancellationToken ct);
            
        Task<List<Bill>> GetLeadBillsAsync(Guid leadId, CancellationToken ct);
        
        Task UpdateBillAsync(Bill bill, CancellationToken ct);

        /// <summary>
        /// Unlinks a Bill from an Enrollment (e.g. when enrollment is deleted)
        /// but preserves the lead/debt record.
        /// </summary>
        Task DetachBillFromEnrollmentAsync(Guid enrollmentId, CancellationToken ct);

        /// <summary>
        /// Restores a link between an Enrollment and its Bill.
        /// </summary>
        Task ReattachBillToEnrollmentAsync(Guid enrollmentId, CancellationToken ct);

        /// <summary>
        /// Adds new medicine items to an existing bill and recalculates totals.
        /// </summary>
        Task AddMedicineToBillAsync(Guid billId, IEnumerable<(Guid MedicineId, int Quantity)> items, CancellationToken ct);
        
        void RecalculateTotals(Bill bill);
    }
}
