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
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            var message = context.Message;
            _logger.LogDebug($"SubmitOrderConsumer: {context.Message.CustomerNumber}");

            #region For Request Client
            /*
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
            */
            #endregion

            _logger.LogInformation("Received a message from endpoint");
            _logger.LogInformation($"With data {message.OrderId} - {message.CustomerNumber}");
            return Task.CompletedTask;

        }
    }
}
