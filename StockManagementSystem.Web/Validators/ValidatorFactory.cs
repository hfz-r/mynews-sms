using System;
using FluentValidation;
using FluentValidation.Attributes;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Web.Validators
{
    public class ValidatorFactory : AttributedValidatorFactory
    {
        public override IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            var validatorAttribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
            if (validatorAttribute == null || validatorAttribute.ValidatorType == null)
                return null;

            //create instance of the validator
            var instance = EngineContext.Current.ResolveUnregistered(validatorAttribute.ValidatorType);

            return instance as IValidator;
        }
    }
}