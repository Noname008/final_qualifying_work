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
        private readonly AiService _aiService;
        private readonly User bot;

        public ProjectChatHub(AppDbContext context, AiService aiService)
        {
            _context = context;
            bot = _context.Users.Find(1)!;
            _aiService = aiService;
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
            // Отправляем сообщение всем участникам проекта
            _context.ProjectMessages.Add(newMessage);
            await _context.SaveChangesAsync();
            await Clients.Group($"project-{projectId}").SendAsync("ReceiveProjectMessage", projectId, userId, user.Username, message, DateTime.UtcNow);

            if (message.Contains("@bot"))
            {
                string response = await ProcessBotCommand(message);
                await Clients.Group($"project-{projectId}").SendAsync("ReceiveProjectMessage", projectId, bot.Id, bot.Username, response, DateTime.UtcNow);
                var botMessage = new ProjectMessage
                {
                    ChatId = chat.Id,
                    UserId = bot.Id,
                    MessageText = response,
                    SentAt = DateTime.UtcNow
                };
                _context.ProjectMessages.Add(botMessage);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> ProcessBotCommand(string message)
        {
            return await _aiService.Process(message.Replace("@bot", "").Trim());
            if (message.Contains("help"))
                return "Доступные команды: @bot help - справка, @bot time - текущее время";

            if (message.Contains("time"))
                return $"Текущее время: {DateTime.UtcNow.ToString("HH:mm UTC")}";

            return "Неизвестная команда. Напишите '@bot help' для списка команд";
        }
    }
}
