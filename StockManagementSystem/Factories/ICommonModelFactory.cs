using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Models.Common;

namespace StockManagementSystem.Factories
{
    public interface ICommonModelFactory
    {
        Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModel();

        Task<HeaderLinksModel> PrepareHeaderLinksModel();

        Task<LogoModel> PrepareLogoModel();

        Task<SystemInfoModel> PrepareSystemInfoModel(SystemInfoModel model);

        Task<IList<SystemWarningModel>> PrepareSystemWarningModels();

        string PrepareRobotsTextFile();
    }
}