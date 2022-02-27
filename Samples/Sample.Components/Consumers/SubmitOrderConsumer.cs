using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.LogDebug($"SubmitOrderConsumer: {context.Message.CustomerNumber}");
            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                await context.RespondAsync<OrderSubmissionRejected>(new
                {

                    Reason = "CustomerNumber was invalid",
                    InVar.Timestamp,
                    OrderId = context.Message.OrderId,
                    CustomerNumber = context.Message.CustomerNumber
                });

                return;
            }
            await context.RespondAsync<OrderSubmissionAccepted>(new
            {
                InVar.Timestamp,
                OrderId = context.Message.OrderId,
                CustomerNumber = context.Message.CustomerNumber
            });
        }
    }
}
