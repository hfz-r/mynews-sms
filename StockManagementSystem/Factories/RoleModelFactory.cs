using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Factories
{
    public class RoleModelFactory : IRoleModelFactory
    {
        private readonly IUserService _userService;

        public RoleModelFactory(IUserService userService)
        {
            _userService = userService;
        }

        public Task<RoleSearchModel> PrepareRoleSearchModel(RoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<RoleListModel> PrepareRoleListModel(RoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var roles = _userService.GetRoles(true);

            var model = new RoleListModel
            {
                Data = roles.PaginationByRequestModel(searchModel).Select(role =>
                {
                    var roleModel = role.ToModel<RoleModel>();
                    return roleModel;
                }),
                Total = roles.Count
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}