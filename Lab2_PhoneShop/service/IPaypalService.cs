using Lab2_PhoneShop.DTOs;
using static Lab2_PhoneShop.DTOs.PaymentRespone;

namespace BookCart.Service
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentUrl(PaymentInformation model, HttpContext context);
        Task<PaymentResponse> PaymentExecute(IQueryCollection collections);
    }
}
