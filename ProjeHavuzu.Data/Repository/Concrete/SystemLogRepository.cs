using Microsoft.EntityFrameworkCore;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.DTOs.Common;
using ProjeHavuzu.Data.DTOs.LogDto;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class SystemLogRepository : RepositoryBase<SystemLog>, ISystemLogRepository
    {
        private readonly ApplicationContext _context;

        public SystemLogRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DataTableResponse<UnifiedLogListDto>> GetLogsServerSideAsync(DataTableRequest request, UnifiedLogFilterDto filter)
        {
            var query = _context.Set<SystemLog>().AsNoTracking()
                .Where(x => !x.IsDeleted);

            // Filter (UnifiedLogFilterDto)
            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(x => x.CreatedDate >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(x => x.CreatedDate <= filter.EndDate.Value.AddDays(1).AddTicks(-1));

                if (filter.UserId.HasValue && filter.UserId != Guid.Empty)
                    query = query.Where(x => x.CreatedBy == filter.UserId.Value);

                if (!string.IsNullOrEmpty(filter.ControllerSearch))
                    query = query.Where(x => x.Controller.Contains(filter.ControllerSearch));

                if (!string.IsNullOrEmpty(filter.LogLevelFilter))
                    query = query.Where(x => x.LogType == filter.LogLevelFilter);

                if (!string.IsNullOrEmpty(filter.LogTypeFilter))
                {
                    if (filter.LogTypeFilter == "System")
                        query = query.Where(x => x.CreatedBy == null || x.Exception != null);
                    else if (filter.LogTypeFilter == "User")
                        query = query.Where(x => x.CreatedBy != null && x.Exception == null);
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    var searchText = filter.SearchText.ToLower();
                    query = query.Where(x =>
                        x.Detail.ToLower().Contains(searchText) ||
                        x.Url.ToLower().Contains(searchText) ||
                        (x.Action != null && x.Action.ToLower().Contains(searchText)) ||
                        (x.Exception != null && x.Exception.ToLower().Contains(searchText)));
                }
            }

            // Search from DataTables (global search)
            if (request.Search != null && !string.IsNullOrEmpty(request.Search.Value))
            {
                var searchValue = request.Search.Value.ToLower();
                query = query.Where(x =>
                    x.Detail.ToLower().Contains(searchValue) ||
                    x.Url.ToLower().Contains(searchValue) ||
                    (x.Action != null && x.Action.ToLower().Contains(searchValue)) ||
                    (x.Controller != null && x.Controller.ToLower().Contains(searchValue)) ||
                    (x.Exception != null && x.Exception.ToLower().Contains(searchValue))
                );
            }

            var totalRecords = await _context.Set<SystemLog>().CountAsync(x => !x.IsDeleted);
            var filteredRecords = await query.CountAsync();

            // Order
            if (request.Order != null && request.Order.Any())
            {
                var order = request.Order.First();
                var isDesc = order.Dir == "desc";
                var colName = request.Columns.Count > order.Column ? request.Columns[order.Column].Data : "createdDate";

                query = colName switch
                {
                    "controller" => isDesc ? query.OrderByDescending(x => x.Controller) : query.OrderBy(x => x.Controller),
                    "action" => isDesc ? query.OrderByDescending(x => x.Action) : query.OrderBy(x => x.Action),
                    "logLevel" => isDesc ? query.OrderByDescending(x => x.LogType) : query.OrderBy(x => x.LogType),
                    "createdDate" => isDesc ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate),
                    _ => query.OrderByDescending(x => x.CreatedDate)
                };
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }

            // Pagination & Projection
            var dataQuery = from log in query
                            join u in _context.Users.AsNoTracking() on log.CreatedBy equals u.Id into uJoin
                            from user in uJoin.DefaultIfEmpty()
                            select new UnifiedLogListDto
                            {
                                Id = log.Id,
                                Controller = log.Controller,
                                Action = log.Action,
                                HttpMethod = log.HttpMethod,
                                Url = log.Url,
                                IpAddress = log.IpAddress,
                                UserAgent = log.UserAgent,
                                Detail = log.Detail,
                                LogLevel = log.LogType,
                                Exception = log.Exception,
                                CreatedDate = log.CreatedDate,
                                UserId = log.CreatedBy,
                                UserName = user != null ? user.FirstName + " " + user.LastName : "Sistem",
                                // LogSource logic duplicated here for projection
                                LogSource = (log.Exception != null || log.CreatedBy == null) ? "System" : "User"
                            };

            var data = await dataQuery
                .Skip(request.Start)
                .Take(request.Length)
                .ToListAsync();

            return new DataTableResponse<UnifiedLogListDto>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }
    }
}
