using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Validators
{
    public class BaseDtoValidator<T> : AbstractValidator<T> where T : BaseDto, new()
    {
        private Dictionary<string, object> _requestValuesDictionary;

        public BaseDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary)
        {
            HttpContextAccessor = httpContextAccessor;
            JsonHelper = jsonHelper;
            HttpMethod = new HttpMethod(HttpContextAccessor.HttpContext.Request.Method);

            if (requestJsonDictionary == null ||
                requestJsonDictionary.Count > 0 && !requestJsonDictionary.ContainsKey("id"))
                HttpMethod = HttpMethod.Post;

            if (requestJsonDictionary != null && requestJsonDictionary.Count > 0)
                _requestValuesDictionary = requestJsonDictionary;

            SetRequiredIdRule();
        }


        protected IHttpContextAccessor HttpContextAccessor { get; private set; }

        protected Dictionary<string, object> RequestJsonDictionary =>
            _requestValuesDictionary ??
            (_requestValuesDictionary = GetRequestJsonDictionaryDictionaryFromHttpContext());

        protected IJsonHelper JsonHelper { get; private set; }

        public HttpMethod HttpMethod { get; set; }

        protected void MergeValidationResult(CustomContext validationContext, ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                foreach (var validationFailure in validationResult.Errors)
                    validationContext.AddFailure(validationFailure);
            }
        }

        protected Dictionary<string, object> GetRequestJsonDictionaryCollectionItemDictionary<TDto>(
            string collectionKey, TDto dto) where TDto : BaseDto
        {
            var collectionItems = (List<object>) RequestJsonDictionary[collectionKey];
            var collectionItemDictionary = collectionItems.FirstOrDefault(x =>
                ((Dictionary<string, object>) x).ContainsKey("id") &&
                (int) (long) ((Dictionary<string, object>) x)["id"] == dto.Id) as Dictionary<string, object>;

            return collectionItemDictionary;
        }

        protected void SetGreaterThanZeroCreateOrUpdateRule(Expression<Func<T, int?>> expression, string errorMessage,
            string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
                SetGreaterThanZeroRule(expression, errorMessage);
        }

        protected void SetGreaterThanZeroRule(Expression<Func<T, int?>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .Must(id => id > 0);
        }

        protected void SetNotNullOrEmptyCreateOrUpdateRule(Expression<Func<T, string>> expression, string errorMessage,
            string requestValueKey)
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey(requestValueKey))
                SetNotNullOrEmptyRule(expression, errorMessage);
        }

        protected void SetNotNullOrEmptyRule(Expression<Func<T, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                .NotNull()
                .NotEmpty()
                .WithMessage(errorMessage);
        }

        #region Private Methods

        private Dictionary<string, object> GetRequestJsonDictionaryDictionaryFromHttpContext()
        {
            var requestJsonDictionary =
                JsonHelper.GetRequestJsonDictionaryFromStream(HttpContextAccessor.HttpContext.Request.Body, true);
            var rootPropertyName = JsonHelper.GetRootPropertyName<T>();

            if (requestJsonDictionary.ContainsKey(rootPropertyName))
            {
                requestJsonDictionary = (Dictionary<string, object>) requestJsonDictionary[rootPropertyName];
            }

            return requestJsonDictionary;
        }

        private void SetRequiredIdRule()
        {
            if (HttpMethod == HttpMethod.Put)
                SetGreaterThanZeroCreateOrUpdateRule(x => x.Id, "invalid id", "id");
        }

        #endregion
    }
}