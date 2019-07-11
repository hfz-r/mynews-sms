using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.DTOs;

namespace StockManagementSystem.Api.Controllers.Generics
{
    public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Get domain instance from abstract BaseDto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private object GetDomainInstance(Type dto)
        {
            if (dto.BaseType != typeof(BaseDto))
                throw new Exception("Generics only applied to 'BaseDto' subclass");

            var className = dto.Name.Remove(dto.Name.Length - 3);

            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
                .First(t => string.Equals(t.Name, className, StringComparison.Ordinal));

            return type != null ? Activator.CreateInstance(type) : null;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var currentAssembly = typeof(GenericTypeControllerFeatureProvider).Assembly;
            var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any());

            foreach (var candidate in candidates)
            {
                var domainCandidate = GetDomainInstance(candidate);

                feature.Controllers.Add(typeof(BaseGenericController<,>)
                    .MakeGenericType(candidate, domainCandidate.GetType()).GetTypeInfo());
            }
        }
    }
}