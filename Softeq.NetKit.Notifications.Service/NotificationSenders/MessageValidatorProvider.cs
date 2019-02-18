// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders
{
    internal class MessageValidatorProvider : IMessageValidatorProvider
    {
        private static readonly ConcurrentDictionary<Type, IValidator> InstanceCache = new ConcurrentDictionary<Type, IValidator>();
        private static readonly IList<Type> ValidatorsTypeList;

        static MessageValidatorProvider()
        {
            ValidatorsTypeList = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && x.GetInterfaces()
                    .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .ToList();
        }

        public IValidator GetValidator<TModel>(TModel model)
        {
            var modelType = model.GetType();
            var validator = InstanceCache.GetOrAdd(modelType, type =>
            {
                var validatorType = ValidatorsTypeList.FirstOrDefault(x =>
                {
                    var genericType = x.BaseType.GetGenericArguments().FirstOrDefault();
                    return genericType == modelType;
                });
                if (validatorType == null)
                {
                    throw new InvalidOperationException($"Unsupported notification message type {modelType}");
                }

                var instance = Activator.CreateInstance(validatorType);
                return instance as IValidator;
            });

            return validator;
        }
    }
}
