using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly ILogger<OrderController> _logger;
        readonly ISendEndpointProvider _endpointProvider;
        readonly IRequestClient<CheckOrder> _checkOrderRequestClient;

        public OrderController(ILogger<OrderController> logger
            , ISendEndpointProvider endpointProvider
            , IRequestClient<CheckOrder> checkOrderRequestClient)
        {
            _logger = logger;
            _endpointProvider = endpointProvider;
            _checkOrderRequestClient = checkOrderRequestClient;
        }

        [HttpGet]
        public async Task<ActionResult<OrderStatus>> Get(Guid orderId)
        {
            var (result, notFound) = await _checkOrderRequestClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                OrderId = orderId
            });

            if (result.IsCompletedSuccessfully)
            {
                var res = await result;
                return Ok(res.Message);
            }
            var info = await notFound;
            return NotFound(info.Message);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Guid id, string customerNumber)
        {
            
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _endpointProvider.GetSendEndpoint(
                new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = id,
                InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return NoContent();
        }
    }
}
