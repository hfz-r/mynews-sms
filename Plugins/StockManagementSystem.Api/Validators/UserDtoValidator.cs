using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Helpers;
using System.Net.Http;
using FluentValidation;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Validators
{
    public class UserDtoValidator : BaseDtoValidator<UserDto>
    {
        private readonly IUserRolesHelper _userRolesHelper;

        public UserDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary, IUserRolesHelper userRolesHelper) : base(
            httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _userRolesHelper = userRolesHelper;

            SetEmailRule();
            SetPasswordRule();
            SetRoleIdsRule();
            SetStoreIdsRule();

            //TODO: role validation
            //TODO: store validation
            //SetRoleRule();
        }

        private void SetEmailRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(u => u.Email, "Invalid email.", "email");
        }

        private void SetPasswordRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Password, "Invalid password.", "password");
        }

        //private void SetRoleRule()
        //{
        //    var key = "roles";
        //    if (RequestJsonDictionary.ContainsKey(key))
        //    {
        //        RuleForEach(u => u.Roles)
        //            .Custom((roleDto, context) =>
        //            {
        //                var roleJsonDictionary = GetRequestJsonDictionaryCollectionItemDictionary(key, roleDto);

        //                var validator = new RoleDtoValidator(HttpContextAccessor, JsonHelper, roleJsonDictionary);

        //                if (roleDto.Id == 0)
        //                    validator.HttpMethod = HttpMethod.Post;

        //                var validationResult = validator.Validate(roleDto);

        //                MergeValidationResult(context, validationResult);
        //            });
        //    }
        //}

        private void SetRoleIdsRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("role_ids"))
            {
                IList<Role> roles = null;

                RuleFor(x => x.RoleIds)
                    .NotNull()
                    .Must(r => r.Count > 0)
                    .WithMessage("role_ids required")
                    .DependentRules(() => RuleFor(dto => dto.RoleIds)
                        .Must(roleIds =>
                        {
                            if (roles == null)
                                roles = _userRolesHelper.GetValidRoles(roleIds);

                            var isInGuestAndRegisterRoles =
                                _userRolesHelper.IsInGuestsRole(roles) && _userRolesHelper.IsInRegisteredRole(roles);

                            return !isInGuestAndRegisterRoles;
                        })
                        .WithMessage("must not be in guest and register roles simultaneously")
                        .DependentRules(() => RuleFor(dto => dto.RoleIds)
                            .Must(roleIds =>
                            {
                                if (roles == null)
                                    roles = _userRolesHelper.GetValidRoles(roleIds);

                                var isInGuestOrRegisterRoles =
                                    _userRolesHelper.IsInGuestsRole(roles) ||
                                    _userRolesHelper.IsInRegisteredRole(roles);

                                return isInGuestOrRegisterRoles;
                            })
                            .WithMessage("must be in guest or register role")
                        )
                    );
            }
        }

        private void SetStoreIdsRule()
        {
            if (HttpMethod == HttpMethod.Post || RequestJsonDictionary.ContainsKey("store_ids"))
            {
                RuleFor(x => x.StoreIds)
                    .NotNull()
                    .Must(r => r.Count > 0)
                    .WithMessage("store_ids required");
            }
        }
    }
}