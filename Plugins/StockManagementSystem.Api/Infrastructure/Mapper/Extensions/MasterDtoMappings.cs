using StockManagementSystem.Api.DTOs.Master;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class MasterDtoMappings
    {
        public static ASNDetailMasterDto ToDto(this ASNDetailMaster entity)
        {
            return entity.MapTo<ASNDetailMaster, ASNDetailMasterDto>();
        }

        public static ASNHeaderMasterDto ToDto(this ASNHeaderMaster entity)
        {
            return entity.MapTo<ASNHeaderMaster, ASNHeaderMasterDto>();
        }

        public static BarcodeMasterDto ToDto(this BarcodeMaster entity)
        {
            return entity.MapTo<BarcodeMaster, BarcodeMasterDto>();
        }

        public static MainCategoryMasterDto ToDto(this MainCategoryMaster entity)
        {
            return entity.MapTo<MainCategoryMaster, MainCategoryMasterDto>();
        }

        public static OrderBranchMasterDto ToDto(this OrderBranchMaster entity)
        {
            return entity.MapTo<OrderBranchMaster, OrderBranchMasterDto>();
        }

        public static SalesMasterDto ToDto(this SalesMaster entity)
        {
            return entity.MapTo<SalesMaster, SalesMasterDto>();
        }

        public static ShelfLocationMasterDto ToDto(this ShelfLocationMaster entity)
        {
            return entity.MapTo<ShelfLocationMaster, ShelfLocationMasterDto>();
        }

        public static StockTakeControlMasterDto ToDto(this StockTakeControlMaster entity)
        {
            return entity.MapTo<StockTakeControlMaster, StockTakeControlMasterDto>();
        }

        public static StockTakeRightMasterDto ToDto(this StockTakeRightMaster entity)
        {
            return entity.MapTo<StockTakeRightMaster, StockTakeRightMasterDto>();
        }

        public static StockTakeControlOutletMasterDto ToDto(this StockTakeControlOutletMaster entity)
        {
            return entity.MapTo<StockTakeControlOutletMaster, StockTakeControlOutletMasterDto>();
        }

        public static StockSupplierMasterDto ToDto(this StockSupplierMaster entity)
        {
            return entity.MapTo<StockSupplierMaster, StockSupplierMasterDto>();
        }

        public static ShiftControlMasterDto ToDto(this ShiftControlMaster entity)
        {
            return entity.MapTo<ShiftControlMaster, ShiftControlMasterDto>();
        }

        public static SubCategoryMasterDto ToDto(this SubCategoryMaster entity)
        {
            return entity.MapTo<SubCategoryMaster, SubCategoryMasterDto>();
        }

        public static SupplierMasterDto ToDto(this SupplierMaster entity)
        {
            return entity.MapTo<SupplierMaster, SupplierMasterDto>();
        }
    }
}