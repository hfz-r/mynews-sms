using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IShiftControlApiService
    {
        ShiftControlMaster GetShiftControlById(int id);
        IList<ShiftControlMaster> GetShiftControls(int limit = 50, int page = 1, int sinceId = 0);
        int GetShiftControlsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}