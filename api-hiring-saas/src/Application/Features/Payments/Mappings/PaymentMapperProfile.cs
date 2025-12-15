using AutoMapper;
using Domain.Entities.Payments;
using DTO.Payments;

namespace Application.Features.Payments.Mappings;

public class PaymentMapperProfile : Profile
{
    public PaymentMapperProfile()
    {
        CreateMap<PaymentTransaction, PaymentTransactionResponse>();
    }
}