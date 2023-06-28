using BillingApp.Controllers;
using BillingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace BillingApp.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task SendCorrectPaymentGateway_ReturnReceipt()
        {
            var controller = CreateController(HttpStatusCode.OK);

            var order = new Order
            {
                OrderNumber = "OrderNumber",
                UserId = 0,
                PayableAmount = 100,
                PaymentGateway = 1,
                OptionalDescription = "OptionalDescription",
            };

            var result = await controller.ProcessOrder(order) as ObjectResult;
            var receipt = result?.Value as Receipt;

            Assert.AreEqual((int)HttpStatusCode.OK, result?.StatusCode);
            Assert.IsNotNull(receipt);
            Assert.AreEqual(order.OrderNumber, receipt.OrderNumber);
            Assert.AreEqual(order.PayableAmount, receipt.PayableAmount);
            Assert.AreEqual(true, receipt.PaymentSuccessful);
        }

        [TestMethod]
        public async Task SendCorrectPaymentGateway_ReturnBadRequest()
        {
            var controller = CreateController(HttpStatusCode.BadRequest);

            var order = new Order
            {
                OrderNumber = "OrderNumber",
                UserId = 0,
                PayableAmount = 100,
                PaymentGateway = 1,
                OptionalDescription = "OptionalDescription",
            };

            var result = await controller.ProcessOrder(order) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result?.StatusCode);
            Assert.AreEqual("Order payment is not successful", result?.Value);
        }

        [TestMethod]
        public async Task SendWrongPaymentGateway_ReturnBadRequest()
        {
            var controller = CreateController(HttpStatusCode.OK);

            var order = new Order
            {
                OrderNumber = "OrderNumber",
                UserId = 0,
                PayableAmount = 100,
                PaymentGateway = 0,
                OptionalDescription = "OptionalDescription",
            };

            var result = await controller.ProcessOrder(order) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result?.StatusCode);
            Assert.AreEqual("Wrong payment gateway", result?.Value);
        }

        private BillingApiController CreateController(HttpStatusCode httpStatusCode)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

            return new BillingApiController(client, config);
        }
    }
}