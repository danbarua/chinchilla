﻿using System;
using Machine.Fakes;
using Machine.Specifications;

namespace Chinchilla.Specifications
{
    public class ErrorQueueFaultStrategySpecification
    {
        [Subject(typeof(ErrorQueueFaultStrategy))]
        public class when_handling_failed_delivery : with_strategy
        {
            Establish context = () =>
                delivery = An<IDelivery>();

            Because of = () =>
                Subject.Handle(delivery, new Exception());

            It should_accept_delivery = () =>
                delivery.WasToldTo(d => d.Accept());

            static IDelivery delivery;
        }

        [Subject(typeof(ErrorQueueFaultStrategy))]
        public class when_building_error : with_strategy
        {
            Establish context = () =>
            {
                delivery = An<IDelivery>();
                delivery.WhenToldTo(d => d.RoutingKey).Return("delivery-routing-key");
            };

            Because of = () =>
                fault = Subject.BuildFault(delivery, new Exception("ermagherd"));

            It should_have_routing_key_for_original_message = () =>
                fault.RoutingKey.ShouldEqual("delivery-routing-key");

            static IDelivery delivery;

            static Fault fault;
        }

        public class with_strategy : WithFakes
        {
            Establish context = () =>
            {
                var publisher = An<IPublisher<Fault>>();

                var bus = An<IBus>();
                bus.WhenToldTo(b => b.CreatePublisher<Fault>(
                    Param.IsAny<Action<IPublisherBuilder>>())).Return(publisher);

                Subject = new ErrorQueueFaultStrategy(bus);
            };

            protected static ErrorQueueFaultStrategy Subject;

            protected static IPublisher<Fault> publisher;
        }
    }
}