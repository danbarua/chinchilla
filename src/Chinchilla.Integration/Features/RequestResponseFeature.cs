﻿using Chinchilla.Integration.Features.Consumers;
using Chinchilla.Integration.Features.Messages;
using NUnit.Framework;

namespace Chinchilla.Integration.Features
{
    [TestFixture]
    public class RequestResponseFeature : WithApi
    {
        [Test]
        public void ShouldCreateRequestResponseWithRequester()
        {
            using (var bus = Depot.Connect("localhost/integration"))
            {
                using (var requester = bus.CreateRequester<CapitalizeMessage, CapitalizedMessage>())
                {
                    bus.Subscribe(new CapitalizeMessageConsumer());

                    CapitalizedMessage capitalized = null;

                    requester.Request(
                        new CapitalizeMessage("where am i?"),
                        response => { capitalized = response; });

                    WaitForDelivery();

                    Assert.That(capitalized, Is.Not.Null);
                    Assert.That(capitalized.Result, Is.EqualTo("WHERE AM I?"));
                }
            }
        }

        [Test]
        public void ShouldCreateRequestResponseOnBus()
        {
            using (var bus = Depot.Connect("localhost/integration"))
            {
                bus.Subscribe(new CapitalizeMessageConsumer());

                CapitalizedMessage capitalized = null;
                bus.Request<CapitalizeMessage, CapitalizedMessage>(
                    new CapitalizeMessage("where am i?"),
                    response => { capitalized = response; });

                WaitForDelivery();

                Assert.That(capitalized, Is.Not.Null);
                Assert.That(capitalized.Result, Is.EqualTo("WHERE AM I?"));
            }
        }
    }
}
