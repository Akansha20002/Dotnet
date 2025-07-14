using AutoMapper;
using CourseTech.Core.DTOs.Payment;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class PaymentMapProfile : Profile
    {
        public PaymentMapProfile()
        {
            CreateMap<Payment, PaymentDTO>();
            CreateMap<Payment, PaymentHistoryDTO>();
            CreateMap<PaymentRequestDTO, Payment>();
          
        }
    }
}