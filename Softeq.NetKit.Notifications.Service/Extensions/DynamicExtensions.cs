// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Service.Extensions
{
    public static class DynamicExtensions
    {
        public static object ToStatic(Type instanceType, IDictionary<string, object> properties)
        {
            object entity = Activator.CreateInstance(instanceType);
            PopulateProperties(entity, properties);
            return entity;
        }

        public static T ToStatic<T>(IDictionary<string, object> properties)
        {
            T entity = Activator.CreateInstance<T>();
            PopulateProperties(entity, properties);
            return entity;
        }

        private static void PopulateProperties(object instance, IDictionary<string, object> properties)
        {
            var instanceType = instance.GetType();
            foreach (var entry in properties)
            {
                var propertyInfo = instanceType.GetProperty(entry.Key);
                if (propertyInfo != null)
                {
                    var convertedValue = new object();
                    var type = propertyInfo.PropertyType;
                    if (type == typeof(Guid))
                    {
                        convertedValue = new Guid(entry.Value.ToString());
                    }
                    if (type == typeof(string))
                    {
                        convertedValue = entry.Value;
                    }
                    propertyInfo.SetValue(instance, convertedValue, null);
                }
            }
        }
    }
}
