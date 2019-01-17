using System;
using FluentValidation.Validators;

namespace StockManagementSystem.Web.Validators
{
    public class DecimalPropertyValidator : PropertyValidator
    {
        private readonly decimal _maxValue;

        public DecimalPropertyValidator(decimal maxValue) : base("Decimal value is out of range")
        {
            this._maxValue = maxValue;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (decimal.TryParse(context.PropertyValue.ToString(), out decimal value))
                return Math.Round(value, 3) < _maxValue;

            return false;
        }
    }
}