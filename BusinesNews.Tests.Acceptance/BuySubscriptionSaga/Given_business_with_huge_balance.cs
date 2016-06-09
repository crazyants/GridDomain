﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.AccountAggregate.Events;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.OfferAggregate;
using BusinessNews.Node;
using BusinessNews.ReadModel;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Configuration;
using Microsoft.Practices.Unity;
using NMoneys;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace BusinesNews.Tests.Acceptance.BuySubscriptionSaga
{
    [TestFixture]
    class Given_business_with_huge_balance: NodeCommandsTest
    {
        public Given_business_with_huge_balance() : base(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig())
        {
        }

        [TestFixtureSetUp]
        public void Given_business_with_money()
        {
            var accountId = Guid.NewGuid();
            var businessId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();

            var registerNewBusinessCommand = new RegisterNewBusinessCommand(businessId, "test business", accountId);
            ExecuteAndWaitFor<BusinessCreatedEvent>(registerNewBusinessCommand);

            var createAccountCommand = new CreateAccountCommand(accountId, businessId);
            ExecuteAndWaitFor<BusinessBalanceCreatedProjectedNotification>(createAccountCommand);

            var replenishAccountByCardCommand = new ReplenishAccountByCardCommand(accountId, new Money(1000), null);
            ExecuteAndWaitFor<AccountBalanceReplenishEvent>(replenishAccountByCardCommand);

            var orderSubscriptionCommand = new OrderSubscriptionCommand(businessId, VIPSubscription.ID, subscriptionId);
       
            ExecuteAndWaitFor<SubscriptionOrderCompletedEvent>(orderSubscriptionCommand);
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(500);
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();
            BusinessNews.Node.CompositionRoot.Init(container, new LocalDbConfiguration());
            return new GridDomainNode(container, new BusinessNewsRouting(), TransportMode.Standalone, Sys);
        }
    }
}