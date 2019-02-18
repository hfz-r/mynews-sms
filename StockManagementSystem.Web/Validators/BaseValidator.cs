using System.Linq;
using System.Linq.Dynamic.Core;
using FluentValidation;
using StockManagementSystem.Core;
using StockManagementSystem.Data;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Web.Validators
{
    public abstract class BaseValidator<TModel> : AbstractValidator<TModel> where TModel : class 
    {
        /// <summary>
        /// Sets validation rule(s) to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="filterStringPropertyNames">Properties to skip</param>
        protected void SetDatabaseValidationRules<TEntity>(IDbContext dbContext, params string[] filterStringPropertyNames) 
            where TEntity : BaseEntity
        {
            SetStringPropertiesMaxLength<TEntity>(dbContext, filterStringPropertyNames);
            SetDecimalMaxValue<TEntity>(dbContext);
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength<TEntity>(IDbContext dbContext, params string[] filterPropertyNames)
            where TEntity : BaseEntity
        {
            if (dbContext == null)
                return;

            //filter model properties for which need to get max lengths
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(string) && !filterPropertyNames.Contains(property.Name))
                .Select(property => property.Name).ToList();

            //get max length of these properties
            var propertyMaxLengths = dbContext.GetColumnsMaxLength<TEntity>()
                .Where(property => modelPropertyNames.Contains(property.Name) && property.MaxLength.HasValue);

            //create expressions for the validation rules
            var maxLengthExpressions = propertyMaxLengths.Select(property => new
            {
                MaxLength = property.MaxLength.Value,
                Expression = DynamicExpressionParser.ParseLambda<TModel, string>(null, false, property.Name)
            }).ToList();

            //define string length validation rules
            foreach (var expression in maxLengthExpressions)
            {
                RuleFor(expression.Expression).Length(0, expression.MaxLength);
            }
        }

        /// <summary>
        /// Sets max value validation rule(s) to decimal properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="dbContext">Database context</param>
        protected virtual void SetDecimalMaxValue<TEntity>(IDbContext dbContext) where TEntity : BaseEntity
        {
            if (dbContext == null)
                return;

            //filter model properties for which need to get max values
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(decimal))
                .Select(property => property.Name).ToList();

            //get max values of these properties
            var decimalPropertyMaxValues = dbContext.GetDecimalColumnsMaxValue<TEntity>()
                .Where(property => modelPropertyNames.Contains(property.Name) && property.MaxValue.HasValue);

            //create expressions for the validation rules
            var maxValueExpressions = decimalPropertyMaxValues.Select(property => new
            {
                MaxValue = property.MaxValue.Value,
                Expression = DynamicExpressionParser.ParseLambda<TModel, decimal>(null, false, property.Name)
            }).ToList();

            //define decimal validation rules
            foreach (var expression in maxValueExpressions)
            {
                RuleFor(expression.Expression).IsDecimal(expression.MaxValue)
                    .WithMessage($"The value is out of range. Maximum value is {expression.MaxValue - 1}.99");
            }
        }
    }
}