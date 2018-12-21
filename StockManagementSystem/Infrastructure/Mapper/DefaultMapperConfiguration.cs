using AutoMapper;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Infrastructure.Mapper
{
    public class DefaultMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public DefaultMapperConfiguration()
        {
            CreateRoleMaps();
            
            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseModel
                if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(BaseModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseModel.CustomProperties), options => options.Ignore());
                }
            });
        }

        /// <summary>
        /// Create roles maps
        /// </summary>
        protected virtual void CreateRoleMaps()
        {
            CreateMap<Role, RoleModel>();
            CreateMap<RoleModel, Role>()
                .ForMember(entity => entity.ConcurrencyStamp, options => options.Ignore())
                .ForMember(entity => entity.NormalizedName, options => options.Ignore())
                .ForMember(entity => entity.Claims, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore());
        }

        public int Order => 0;
    }
}