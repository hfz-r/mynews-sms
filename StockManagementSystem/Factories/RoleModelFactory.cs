using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Factories
{
    public class RoleModelFactory : IRoleModelFactory
    {
        private readonly IRoleService _roleService;

        public RoleModelFactory(
            IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<RoleSearchModel> PrepareRoleSearchModelAsync(RoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<RoleListModel> PrepareRoleListModelAync(RoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var roles = await _roleService.GetRoles();

            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

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
                model.Data = model.Data.Filter(filter.Logic, filter.Filters);
            }

            return model;
        }
    }
}