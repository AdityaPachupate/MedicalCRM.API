using MediatR;

namespace CRM.API.Features.Medicines.UpdateMedicine
{
    public record UpdateMedicineRequest(Guid Id, string? Name, decimal? Price, bool? IsActive);
    public record UpdateMedicineResponse(Guid Id, string Name, decimal Price, bool IsActive, DateTime CreatedAt);
    public record UpdateMedicineCommand(UpdateMedicineRequest Request) : IRequest<UpdateMedicineResponse>;
}
