using MediatR;

namespace CRM.API.Features.Medicines.CreateMedicine
{
    public record CreateMedicineRequest(string Name, decimal Price);
    public record CreateMedicineResponse(Guid Id, string Name, decimal Price, DateTime CreatedAt);
    public record CreateMedicineCommand(CreateMedicineRequest Request) : IRequest<CreateMedicineResponse>;
}
