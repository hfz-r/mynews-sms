using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.Reports;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Factories
{
    public class ReportModelFactory : IReportModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<Transaction> _transRepository;

        public ReportModelFactory(
            IBaseModelFactory baseModelFactory,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IRepository<Transaction> transRepository)
        {
            _baseModelFactory = baseModelFactory;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _transRepository = transRepository;
        }

        public async Task<ReportContainerModel> PrepareReportContainerModel(ReportContainerModel reportContainerModel)
        {
            if (reportContainerModel == null)
                throw new ArgumentNullException(nameof(reportContainerModel));

            //prepare nested models
            await PrepareSignedInLogSearchModel(reportContainerModel.ListSignedIn);
            await PrepareTransActivitySearchModel(reportContainerModel.ListTransActivity);

            return reportContainerModel;
        }

        public async Task<SignedInLogSearchModel> PrepareSignedInLogSearchModel(SignedInLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<TransActivitySearchModel> PrepareTransActivitySearchModel(TransActivitySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare selectlist
            await _baseModelFactory.PrepareBranches(searchModel.Branches);

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<SignedInLogListModel> PrepareSignedInLogListModel(SignedInLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var startLoginDateValue = searchModel.LastLoginFrom == null
                ? null
                : (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.LastLoginFrom.Value,
                    _dateTimeHelper.CurrentTimeZone);
            var endLoginDateValue = searchModel.LastLoginTo == null
                ? null
                : (DateTime?) _dateTimeHelper
                    .ConvertToUtcTime(searchModel.LastLoginTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var users = await _userService.GetUsersAsync(
                lastLoginFrom: startLoginDateValue,
                lastLoginTo: endLoginDateValue,
                ipAddress: searchModel.LastIpAddress,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new SignedInLogListModel
            {
                Data = users.Where(user => user.LastLoginDateUtc != null)
                    .Select(user =>
                    {
                        var signedInModel = user.ToModel<SignedInLogModel>();
                        signedInModel.UserId = user.Id;
                      
                        if (user.LastLoginDateUtc.HasValue)
                            signedInModel.LastLoginDate = _dateTimeHelper.ConvertToUserTime(user.LastLoginDateUtc.Value, 
                                DateTimeKind.Utc);

                        return signedInModel;
                    }),
                Total = users.TotalCount
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
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<IEnumerable<TransActivityModel>> PrepareListTransActivity(TransActivitySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var query = _transRepository.Table;

            if (searchModel.CreatedOnFrom.HasValue)
                query = query.Where(item => searchModel.CreatedOnFrom.Value <= item.CreatedOnUtc);
            if (searchModel.CreatedOnTo.HasValue)
                query = query.Where(item => searchModel.CreatedOnTo.Value >= item.CreatedOnUtc);
            if (searchModel.BranchId.HasValue && searchModel.BranchId.Value > 0)
                query = query.Where(item => searchModel.BranchId.Value == item.P_BranchNo);

            var model = (await query.ToListAsync()).Select(q =>
            {
                var m = q.ToModel<TransActivityModel>();
                m.Branch = q.Branch.Name;
                m.CreatedOn = q.CreatedOnUtc;

                return m;
            });

            return model;
        }
    }
}