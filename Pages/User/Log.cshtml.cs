using final_qualifying_work.Data;
using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class LogModel : PageModel
    {
        private readonly ILogService _logService;
        private readonly IProjectUserRepository _projectUserRepository;
        private const int DefaultPageSize = 10;

        public LogModel(IProjectUserRepository projectUserRepository, ILogService logService)
        {
            _projectUserRepository = projectUserRepository;
            _logService = logService;
        }

        public PaginatedList<LogProject> ProjectLogs { get; set; }
        public SelectList UserRoles { get; set; }
        public SelectList PageSizes { get; set; }
        public string CurrentSort { get; set; }
        public string DateSort { get; set; }
        public int ProjectId { get; set; }

        public async Task OnGetAsync(
            int projectId,
            LogProjectType? filterType,
            string filterRole,
            string sortOrder,
            int? pageIndex,
            int? pageSize)
        {
            // Инициализация списков для фильтров
            
            UserRoles = new SelectList(new List<string> { "All", "Admin", "User" });
            PageSizes = new SelectList(new[] { 5, 10, 20, 50 }, DefaultPageSize);
            ProjectId = projectId;

            // Настройка сортировки
            CurrentSort = sortOrder;
            DateSort = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            IQueryable<LogProject> logsQuery = _logService.GetLogs(projectId);

            // Применение фильтров
            if (filterType.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.LogType == filterType.Value);
            }

            if (!string.IsNullOrEmpty(filterRole) && filterRole != "All")
            {
                logsQuery = logsQuery.Where(l => _projectUserRepository.GetProjectUserAsync(projectId,l.Id).Result.Role.ToString() == filterRole);
            }

            // Применение сортировки
            logsQuery = sortOrder switch
            {
                "date_desc" => logsQuery.OrderByDescending(l => l.CreatedDate),
                _ => logsQuery.OrderBy(l => l.CreatedDate)
            };

            // Пагинация
            var actualPageSize = pageSize ?? DefaultPageSize;
            ProjectLogs = await PaginatedList<LogProject>.CreateAsync(
                logsQuery.AsNoTracking(),
                pageIndex ?? 1,
                actualPageSize);
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
