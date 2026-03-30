using CRM.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.API.Infrastructure.Persistence.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.PackageAmount).HasColumnType("decimal(10,2)");
        builder.Property(b => b.AdvanceAmount).HasColumnType("decimal(10,2)");
        builder.Property(b => b.PendingAmount).HasColumnType("decimal(10,2)");
        builder.Property(b => b.MedicineBillingAmount).HasColumnType("decimal(10,2)");
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("now()");
    }
}