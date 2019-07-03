using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Helpers;
using System.Net.Http;
using FluentValidation;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Validators
{
    public class UserDtoValidator : BaseDtoValidator<UserDto>
    {
        private readonly IUserRolesHelper _userRolesHelper;
        private readonly IStoreMappingHelper _storeHelper;

        public UserDtoValidator(
            IHttpContextAccessor httpContextAccessor,
            IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary,
            IUserRolesHelper userRolesHelper,
            IStoreMappingHelper storeHelper) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _userRolesHelper = userRolesHelper;
            _storeHelper = storeHelper;

            SetEmailRule();
            SetPasswordRule();
            SetRoleIdsRule();
            SetStoreIdsRule();
        }

        private void SetEmailRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(u => u.Email, "Invalid email.", "email");
        }

        private void SetPasswordRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Password, "Invalid password.", "password");
        }

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
                                _userRolesHelper.IsInGuestsRole(roles) &&
                                _userRolesHelper.IsInRegisteredRole(roles);

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
                IList<Store> stores = null;

                RuleFor(x => x.StoreIds)
                    .NotNull()
                    .Must(r => r.Count > 0)
                    .WithMessage("store_ids required")
                    .DependentRules(() => RuleFor(dto => dto.StoreIds)
                        .Must(storeIds =>
                        {
                            if (stores == null)
                                stores = _storeHelper.GetValidStores(storeIds);

                            return stores.Any();
                        })
                        .WithMessage("invalid store_ids")
                    );
            }
        }
    }
}