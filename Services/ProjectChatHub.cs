using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace final_qualifying_work.Services
{
    [Authorize]
    public class ProjectChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ProjectChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task JoinProjectChat(int projectId)
        {
            var userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Проверяем, что пользователь является участником проекта
            var isMember = await _context.ProjectUsers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (isMember)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
                await Clients.Caller.SendAsync("JoinedProjectChat", projectId);
            }
        }

        public async Task SendMessageToProject(int projectId, string message)
        {
            var userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            // Проверяем, что пользователь является участником проекта
            var isMember = await _context.ProjectUsers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (!isMember)
            {
                return;
            }

            // Получаем чат проекта
            var chat = await _context.ProjectChats
                .FirstOrDefaultAsync(c => c.ProjectId == projectId);

            if (chat == null)
            {
                return;
            }

            // Сохраняем в БД
            var newMessage = new ProjectMessage
            {
                ChatId = chat.Id,
                UserId = userId,
                MessageText = message,
                SentAt = DateTime.UtcNow
            };

            _context.ProjectMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            // Отправляем сообщение всем участникам проекта
            await Clients.Group($"project-{projectId}").SendAsync("ReceiveProjectMessage", projectId, userId, user.Username, message, DateTime.UtcNow);
        }
    }
}
