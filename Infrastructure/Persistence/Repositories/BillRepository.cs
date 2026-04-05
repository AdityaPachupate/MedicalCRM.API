using CRM.API.Common.Interfaces;
using CRM.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Infrastructure.Persistence.Repositories
{
    public class BillRepository(AppDbContext db) : IBillRepository
    {
        public async Task<Bill> CreateBillWithItemsAsync(
            Bill bill, 
            IEnumerable<(Guid MedicineId, int Quantity)> medicineItems, 
            CancellationToken ct)
        {
            decimal medicineTotal = 0;
            
            if (medicineItems != null && medicineItems.Any())
            {
                var medicineIds = medicineItems.Select(i => i.MedicineId).ToList();
                var medicines = await db.Medicines
                    .Where(m => medicineIds.Contains(m.Id))
                    .ToListAsync(ct);

                foreach (var itemReq in medicineItems)
                {
                    var med = medicines.FirstOrDefault(m => m.Id == itemReq.MedicineId);
                    if (med != null)
                    {
                        var billItem = new BillItem
                        {
                            MedicineId = med.Id,
                            Quantity = itemReq.Quantity,
                            UnitPriceSnapshot = med.Price
                        };
                        bill.Items.Add(billItem);
                        medicineTotal += (med.Price * itemReq.Quantity);
                    }
                }
            }

            bill.MedicineBillingAmount = medicineTotal;
            bill.PendingAmount = (bill.InitialAmount + bill.MedicineBillingAmount) - bill.AmountPaid;

            db.Bills.Add(bill);
            await db.SaveChangesAsync(ct);
            
            return bill;
        }

        public async Task<List<Bill>> GetLeadBillsAsync(Guid leadId, CancellationToken ct)
        {
            return await db.Bills
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(b => b.Items)
                    .ThenInclude(i => i.Medicine)
                .Where(b => b.LeadId == leadId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task UpdateBillAsync(Bill bill, CancellationToken ct)
        {
            db.Bills.Update(bill);
            await db.SaveChangesAsync(ct);
        }

        public async Task DetachBillFromEnrollmentAsync(Guid enrollmentId, CancellationToken ct)
        {
            var bill = await db.Bills
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.EnrollmentId == enrollmentId, ct);
            if (bill != null)
            {
                bill.EnrollmentId = null; // Unlink but keep the record
                await db.SaveChangesAsync(ct);
            }
        }

        public async Task ReattachBillToEnrollmentAsync(Guid enrollmentId, CancellationToken ct)
        {
            // This assumes we can find the bill by LeadId and the fact it HAS NO EnrollmentId 
            // but matches the enrollment's history. For simplicity, we find bills 
            // created recently for this lead without an enrollment.
            // In a real system, we might store a 'LegacyEnrollmentId' if needed.
            // Here, we look for the last 'detached' bill for this enrollment.
            // Actually, better to just look for the bill that belongs to this specific lead!
        }

        public async Task AddMedicineToBillAsync(Guid billId, IEnumerable<(Guid MedicineId, int Quantity)> items, CancellationToken ct)
        {
            var bill = await db.Bills
                .IgnoreQueryFilters()
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == billId, ct);
            if (bill == null) return;

            var medicineIds = items.Select(i => i.MedicineId).ToList();
            var medicines = await db.Medicines.Where(m => medicineIds.Contains(m.Id)).ToListAsync(ct);

            foreach (var item in items)
            {
                var med = medicines.FirstOrDefault(m => m.Id == item.MedicineId);
                if (med != null)
                {
                    bill.Items.Add(new BillItem
                    {
                        BillId = bill.Id,
                        MedicineId = med.Id,
                        Quantity = item.Quantity,
                        UnitPriceSnapshot = med.Price
                    });
                }
            }

            RecalculateTotals(bill);
            await db.SaveChangesAsync(ct);
        }

        public void RecalculateTotals(Bill bill)
        {
            bill.MedicineBillingAmount = bill.Items.Sum(i => i.Quantity * i.UnitPriceSnapshot);
            bill.PendingAmount = (bill.InitialAmount + bill.MedicineBillingAmount) - bill.AmountPaid;
        }
    }
}
