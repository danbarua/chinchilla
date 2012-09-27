using System.Collections.Generic;

namespace Chinchilla.Configuration
{
    public interface ISubscriptionConfiguration : IEndpointConfiguration
    {
        IEnumerable<string> QueueNames { get; }

        uint PrefetchSize { get; }

        ushort PrefetchCount { get; }

        IDeliveryStrategy BuildDeliveryStrategy(IDeliveryProcessor deliveryProcessor);

        IFaultStrategy BuildFaultStrategy(IBus bus);
    }
}