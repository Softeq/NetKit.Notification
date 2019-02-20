using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Components.EventBus;
using Softeq.NetKit.Components.EventBus.Abstract;
using Softeq.NetKit.Components.EventBus.Managers;
using Softeq.NetKit.Components.EventBus.Service;
using Softeq.NetKit.Components.EventBus.Service.Connection;
using Softeq.NetKit.Notifications.Service.EventBus.Events;
using Softeq.NetKit.Notifications.Service.EventBus.Handlers;
using System;

namespace Softeq.NetKit.Notifications.Service.EventBus
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var config = context.Resolve<IConfiguration>();
                return new ServiceBusPersisterConnectionConfiguration
                {
                    ConnectionString = config["EventBus:ServiceBus:ConnectionString"],
                    QueueConfiguration = new ServiceBusPersisterQueueConnectionConfiguration
                    {
                        QueueName = config["EventBus:ServiceBus:QueueName"]
                    },
                    TopicConfiguration = new ServiceBusPersisterTopicConnectionConfiguration
                    {
                        SubscriptionName = config["EventBus:ServiceBus:SubscriptionName"],
                        TopicName = config["EventBus:ServiceBus:TopicName"]
                    }
                };
            }).SingleInstance();

            builder.RegisterType<ServiceBusPersisterConnection>()
                .As<IServiceBusPersisterConnection>();

            builder.RegisterType<EventBusSubscriptionsManager>()
                .As<IEventBusSubscriptionsManager>();

            builder.RegisterType<EventBusService>()
                .As<IEventBusSubscriber>();

            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();

                return new MessageQueueConfiguration
                {
                    TimeToLiveInMinutes = Convert.ToInt32(config["EventBus:MessageTimeToLiveInMinutes"])
                };
            }).SingleInstance();

            builder.RegisterType<ArticleCreatedNotificationEventHandler>();

            builder.RegisterType<EventBusRegistrationStartable>().
                As<IStartable>()
                .SingleInstance();

            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();

                return new EventPublishConfiguration(config["EventBus:ServiceBus:EventPublisherId"]);
            })
            .As<EventPublishConfiguration>();
        }

        private class EventBusRegistrationStartable : IStartable
        {
            private readonly IEventBusSubscriber _subscriber;

            public EventBusRegistrationStartable(IEventBusSubscriber subscriber)
            {
                _subscriber = subscriber;
            }

            public void Start()
            {
                _subscriber.RegisterQueueListener();
                _subscriber.SubscribeAsync<ArticleCreatedNotificationEvent, ArticleCreatedNotificationEventHandler>().Wait();
            }
        }
    }
}
