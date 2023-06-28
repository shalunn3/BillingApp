using BillingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BillingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillingApiController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private List<PaymentGateway> _paymentGateway;

        public BillingApiController(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
            _paymentGateway = _config.GetSection("PaymentGateway").Get<List<PaymentGateway>>();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(Order order)
        {
            var paymentGateway = GetPaymentGateway(order.PaymentGateway);

            if (paymentGateway == null)
            {
                return BadRequest("Wrong payment gateway");
            }

            try
            {
                var response = await _client.PostAsJsonAsync(paymentGateway.Url, order);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Order payment is not successful");
                }
            }
            catch
            {
                return BadRequest("Payment gateway is not working");
            }

            var receipt = new Receipt()
            {
                OrderNumber = order.OrderNumber,
                PayableAmount = order.PayableAmount,
                PaymentSuccessful = true,
            };

            return Ok(receipt);
        }

        private PaymentGateway? GetPaymentGateway(int id)
        {
            return _paymentGateway.FirstOrDefault(p => p.Id == id);
        }
    }
}