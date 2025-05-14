using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace final_qualifying_work.Pages.User
{
    public class ProjectIndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectIndexModel(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<ProjectDto> UserProjects { get; set; }
        public List<ProjectMessageDto> Messages { get; set; }
        public List<ProjectMemberDto> ProjectMembers { get; set; }
        public ProjectDto CurrentProject { get; set; }
        public int CurrentUserId { get; set; }

        public class ProjectDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class ProjectMessageDto
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string MessageText { get; set; }
            public DateTime SentAt { get; set; }
        }

        public class ProjectMemberDto
        {
            public string Username { get; set; }
            public bool IsProjectOwner { get; set; }
        }

        public async Task OnGetAsync(int? projectId)
        {
            CurrentUserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // ѕолучаем проекты, где пользователь €вл€етс€ участником
            UserProjects = await _context.ProjectUsers
                .Where(pm => pm.UserId == CurrentUserId)
                .Select(pm => new ProjectDto
                {
                    Id = pm.Project.Id,
                    Name = pm.Project.Title
                })
                .ToListAsync();

            if (projectId.HasValue && UserProjects.Any(p => p.Id == projectId.Value))
            {
                CurrentProject = await _context.Projects
                    .Where(p => p.Id == projectId.Value)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Title
                    })
                    .FirstOrDefaultAsync();

                // ѕолучаем сообщени€ чата проекта
                Messages = await _context.ProjectMessages
                    .Where(m => m.Chat.ProjectId == projectId.Value)
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ProjectMessageDto
                    {
                        UserId = m.UserId,
                        Username = m.User.Username,
                        MessageText = m.MessageText,
                        SentAt = m.SentAt
                    })
                    .ToListAsync();

                // ѕолучаем участников проекта
                ProjectMembers = await _context.ProjectUsers
                    .Where(pm => pm.ProjectId == projectId.Value)
                    .Select(pm => new ProjectMemberDto
                    {
                        Username = pm.User.Username,
                        IsProjectOwner = pm.Status
                    })
                    .ToListAsync();

                // —оздаем чат, если его еще нет
                var chatExists = await _context.ProjectChats.AnyAsync(c => c.ProjectId == projectId.Value);
                if (!chatExists)
                {
                    _context.ProjectChats.Add(new ProjectChat
                    {
                        ProjectId = projectId.Value,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(int projectId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("—ообщение не может быть пустым");
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // ѕровер€ем, что пользователь €вл€етс€ участником проекта
            var isMember = await _context.ProjectUsers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            // ѕолучаем или создаем чат проекта
            var chat = await _context.ProjectChats
                .FirstOrDefaultAsync(c => c.ProjectId == projectId);

            if (chat == null)
            {
                chat = new ProjectChat
                {
                    ProjectId = projectId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ProjectChats.Add(chat);
                await _context.SaveChangesAsync();
            }

            var newMessage = new ProjectMessage
            {
                ChatId = chat.Id,
                UserId = userId,
                MessageText = message,
                SentAt = DateTime.UtcNow
            };

            _context.ProjectMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }
    }
}
