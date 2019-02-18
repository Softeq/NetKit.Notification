// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Notifications.Domain.DataStores;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Softeq.NetKit.Notifications.Service.Services.Caching
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();

                return new InMemoryCachingOptions
                {
                    UserSettingsExpiresInHours = Convert.ToInt32(config["Caching:UserSettingsExpiresInHours"])
                };
            }).SingleInstance();

            builder.Register((cnt, parameters) => new InMemoryCachedSettingsDataStore(cnt.ResolveNamed<ISettingsDataStore>(typeof(ISettingsDataStore).Name, 
                    parameters), cnt.Resolve<IMemoryCache>(), 
                    cnt.Resolve<InMemoryCachingOptions>()))
                .As<ISettingsDataStore>();
            builder.RegisterType<InMemoryCachedSettingsDataStore>().Named<ISettingsDataStore>(typeof(InMemoryCachedSettingsDataStore).Name);
        }
    }
}
