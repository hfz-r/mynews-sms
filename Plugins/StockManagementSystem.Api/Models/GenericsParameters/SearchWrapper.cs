using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Core;

namespace StockManagementSystem.Api.Models.GenericsParameters
{
    public class SearchWrapper<TDto, TEntity> 
        where TDto : BaseDto 
        where TEntity : BaseEntity
    {
        public Search<TDto> ToCount(IQueryable<TEntity> query)
        {
            return new Search<TDto> {Count = query.Count()};
        }

        public Search<TDto> ToList(IQueryable<TEntity> query, int page, int limit, Func<IList<TEntity>, IList<TDto>> toDto)
        {
            var entity = new ApiList<TEntity>(query, page - 1, limit);

            var dto = toDto(entity);

            return new Search<TDto> {List = dto};
        }
    }

    /// <summary>
    /// Search model
    /// </summary>
    /// <typeparam name="TDto">Dto class</typeparam>
    public class Search<TDto> where TDto : BaseDto
    {
        public int Count { get; set; }
        public IList<TDto> List { get; set; }
    }

}