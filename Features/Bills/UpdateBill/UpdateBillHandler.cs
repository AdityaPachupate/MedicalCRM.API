using CRM.API.Common.Interfaces;
using MediatR;

namespace CRM.API.Features.Bills.UpdateBill;

public class UpdateBillHandler(IBillRepository billRepository) : IRequestHandler<UpdateBillCommand, UpdateBillResponse>
{
    public async Task<UpdateBillResponse> Handle(UpdateBillCommand command, CancellationToken ct)
    {
        var request = command.Request;
        
        await billRepository.UpdateBillWithItemsAsync(
            command.Id,
            request.InitialAmount,
            request.AmountPaid,
            request.Items?.Select(i => (i.MedicineId, i.Quantity)),
            ct
        );

        return new UpdateBillResponse(true);
    }
}
