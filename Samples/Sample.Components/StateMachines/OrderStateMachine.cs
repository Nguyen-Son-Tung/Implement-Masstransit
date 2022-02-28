using Automatonymous;
using MassTransit.RedisIntegration;
using MassTransit.Saga;
using Sample.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x => {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new
                        {
                            OrderId = context.Message.OrderId
                        });
                    }
                }));
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                .TransitionTo(Submitted));

            During(Submitted,
                Ignore(OrderSubmitted));

            DuringAny(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.SubmitAt ??= context.Data.Timestamp;
                    context.Instance.CustomerNumber ??= context.Data.CustomerNumber;

                }));

            DuringAny(
                When(OrderStatusRequested)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState,
                    CustomerNumber = x.Instance.CustomerNumber
                })));

        }
        public State Submitted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
        public Event<OrderSubmitted> OrderSubmitted { get; private set; }

    }

    [Obsolete]
    public class OrderState : SagaStateMachineInstance, IVersionedSaga
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? SubmitAt { get; set; }
        public int Version { get; set; }
    }
}
