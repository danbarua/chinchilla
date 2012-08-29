﻿using Chinchilla.Topologies;
using Machine.Fakes;
using Machine.Specifications;

namespace Chinchilla.Specifications
{
    public class SubscriptionConfigurationSpecification
    {
        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_default_topology : WithSubject<SubscriptionConfiguration>
        {
            Because of = () =>
                messageTopology = Subject.BuildTopology(new Endpoint("endpointName", "messageType"));

            It should_build_topology = () =>
                messageTopology.ShouldNotBeNull();

            static IMessageTopology messageTopology;
        }

        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_custom_topology : WithSubject<SubscriptionConfiguration>
        {
            Establish context = () =>
            {
                builder = An<IMessageTopologyBuilder>();
                Subject.SetTopology(builder);
            };

            Because of = () =>
                Subject.BuildTopology(new Endpoint("endpointName", "messageType"));

            It should_build_default_topology = () =>
                builder.WasToldTo(b => b.Build(Param.IsAny<IEndpoint>()));

            static IMessageTopologyBuilder builder;
        }

        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_default_consumer_strategy : WithSubject<SubscriptionConfiguration>
        {
            Because of = () =>
                strategy = Subject.BuildDeliveryStrategy(An<IDeliveryProcessor>());

            It should_build_immediate_strategy = () =>
                strategy.ShouldBeOfType<ImmediateDeliveryStrategy>();

            static IDeliveryStrategy strategy;
        }

        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_configured_consumer_strategy : WithSubject<SubscriptionConfiguration>
        {
            Establish context = () =>
                Subject.DeliverUsing<WorkerPoolDeliveryStrategy>(t => t.NumWorkers = 5);

            Because of = () =>
                strategy = Subject.BuildDeliveryStrategy(An<IDeliveryProcessor>());

            It should_create_strategy_of_correct_type = () =>
                strategy.ShouldBeOfType<WorkerPoolDeliveryStrategy>();

            It should_configured_strategy = () =>
                ((WorkerPoolDeliveryStrategy)strategy).NumWorkers.ShouldEqual(5);

            static IDeliveryStrategy strategy;
        }

        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_default_delivery_failure_strategy : WithSubject<SubscriptionConfiguration>
        {
            Because of = () =>
                strategy = Subject.BuildDeliveryFailureStrategy();

            It should_build_immediate_strategy = () =>
                strategy.ShouldBeOfType<ErrorQueueDeliveryFailureStrategy>();

            static IDeliveryFailureStrategy strategy;
        }

        [Subject(typeof(SubscriptionConfiguration))]
        public class when_building_configured_delivery_failure_strategy : WithSubject<SubscriptionConfiguration>
        {
            Establish context = () =>
                Subject.DeliverFailuresUsing<IgnoreDeliveryFailureStrategy>();

            Because of = () =>
                strategy = Subject.BuildDeliveryFailureStrategy();

            It should_create_strategy_of_correct_type = () =>
                strategy.ShouldBeOfType<IgnoreDeliveryFailureStrategy>();

            static IDeliveryFailureStrategy strategy;
        }
    }
}
