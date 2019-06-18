![Azure DevOps builds](https://dev.azure.com/SofteqDevelopment/NetKit/_apis/build/status/Notifications/Notifications-CI-Build)

# Softeq.NetKit.NotificationService

Softeq.NetKit.NotificationService is a RESTful microservice that allows to quickly bring notification support to a developing solution.
Service supports the following notification types: 
1. Push via Azure Notification Hub
2. Email via SendGrid
3. SMS via Twilio

API is written in ```Asp.Net Core 2.0``` and secured with ```OAuth2``` protocol. 
```Swashbuckle``` is enabled to provide API consumers with the documentation.

API has an integration with [Softeq.NetKit.EventDriverCommunication] (https://github.com/Softeq/EventDrivenCommunication) to enable sending notification requests via Message Bus.

# Getting Started

## Explore 

Service exposes the following APIs:

1. ```/settings``` API to initialize new user settings, read and manage;
2. ```/notifications``` API to send new notifications;
3. ```/notifications/history``` API to view and delete notification history assoicated to a particular user;
4. ```/notifications/push/subscription``` API to subscribe or unsubscribe user's mobile devices to receive Push notifications;

## Configure

1. Configure Data Storage: 
    The microservice supports multiple storages: 
        SQL - implemented in ```Softeq.NetKit.Notifications.Store.Sql```
        NoSQL via Azure CosmosDB - implemented in  ```Softeq.NetKit.Notifications.Store.CosmosDB```

    When desired data storage is chosen, 
    * Add a reference to a data store implementation to  ```Softeq.NetKit.Notifications.Web``` project
    * Add Automapper configuration to Startup.cs

    ```csharp
        services.AddAutoMapper(typeof(Service.ContainerModule).Assembly,
                    typeof(Store.Sql.ContainerModule).Assembly);
    ```

    * Register Autofac module in DI container

    ```csharp
        builder.RegisterModule(new Store.Sql.ContainerModule());
    ```

2. Update ```appsettings.json``` by configuring storage connection strings, API keys

## Develop

1. Add new event to ```Softeq.NetKit.Notifications.Domain.Models.Notification.NotificationEvent``` enum
```csharp
    public enum NotificationEvent
    {
        MyNewCustomEvent = 0
    }
```

2. Modify ```Softeq.NetKit.Notifications.Service.NotificationSenders.NotificationEventConfiguration``` by specifying what notification type will support newly-added event
```csharp
    public static Dictionary<NotificationType, IList<NotificationEventConfiguration>> Config = new Dictionary<NotificationType, IList<NotificationEventConfiguration>>
        {
            {
                NotificationType.Email, new List<NotificationEventConfiguration>
                {
                    new NotificationEventConfiguration(NotificationEvent.MyNewCustomEvent, true)
                }
            },
            {
                NotificationType.SMS, new List<NotificationEventConfiguration>
                {
					new NotificationEventConfiguration(NotificationEvent.MyNewCustomEvent)
                }
            },
            {
                NotificationType.Push, new List<NotificationEventConfiguration>
                {
                    new NotificationEventConfiguration(NotificationEvent.MyNewCustomEvent)
                }
            }
        };
```

3. Implement Notification message per supported notification type under ```/NotificationSenders/NOTIFICATION_TYPE/Messages```
    * For Push notification
    ```csharp 
        public class MyNewCustomEventPushMessage : PushNotificationMessage
        {
            [JsonProperty("someId")]
            public Guid SomeId { get; set; }

            public MyNewCustomEventPushMessage()
            {
                NotificationType = (int)NotificationEvent.MyNewCustomEvent;
                BodyLocalizationKey = "MyNewCustomEvent_body";
                TitleLocalizationKey = "MyNewCustomEvent_title";
            }
        }
    ```

    * For Email notification
    ```csharp 
        public class MyNewCustomEventEmailModel : IEmailTemplateModel
        {
            public Guid SomeId { get; set; }
        }

        public class MyNewCustomEventEmailMessage : BaseEmailNotification<MyNewCustomEventEmailModel>
        {
            public MyNewCustomEventEmailMessage(string toEmail, string toName, MyNewCustomEventEmailModel model) : base(new RecipientDto(toEmail, toName))
            {
                TemplateModel = model;
            }
        }
    ```
	
	* For Sms notification
    ```csharp 
        public class MyNewCustomEventSmsModel : ISmsNotification
        {
            public string CustomField { get; set; }
        }

        public class MyNewCustomEventSmsMessage : BaseSmsNotification
        {
            public MyNewCustomEventSmsMessage()
            {

            }
        }
    ```

3. Implement Validator per supported Notificationn Type under ```/NotificationSenders/NOTIFICATION_TYPE/Validators```
    * For Email notification
    ```csharp 
        internal class MyNewCustomEventEmailMessageValidator : BaseEmailValidator<MyNewCustomEventEmailMessage>
        {
            public MyNewCustomEventEmailMessageValidator()
            {
                RuleFor(x => x.TemplateModel.SomeId).NotEqual(Guid.Empty);
            }
        }
    ```

    * For Push notification
    ```csharp 
        internal class MyNewCustomEventPushMessageValidator : BasePushMessageValidator<MyNewCustomEventPushMessage>
        {
            public MyNewCustomEventPushMessageValidator()
            {
                RuleFor(x => x.SomeId).NotEqual(Guid.Empty);
            }
        }
    ```
	
	* For Sms notification
    ```csharp 
        internal class MyNewCustomEventSmsMessageValidator : BaseSmsValidator<MyNewCustomEventSmsMessage>
        {
            public MyNewCustomEventSmsMessageValidator()
            {
                RuleFor(x => x.CustomField).NotEqual(string.Empty());
            }
        }
    ```

4. Update MessageFactory under ```/NotificationSenders/NOTIFICATION_TYPE/***MessageFactory.cs```

5. Add localization info under ```/NotificationSenders/NOTIFICATION_TYPE/Resources```

## About

This project is maintained by [Softeq Development Corp.](https://www.softeq.com/)
We specialize in .NET core applications.

 - [Facebook](https://web.facebook.com/Softeq.by/)
 - [Instagram](https://www.instagram.com/softeq/)
 - [Twitter](https://twitter.com/Softeq)
 - [Vk](https://vk.com/club21079655).

## Contributing

We welcome any contributions.

## License

The Query Utils project is available for free use, as described by the [LICENSE](/LICENSE) (MIT).
