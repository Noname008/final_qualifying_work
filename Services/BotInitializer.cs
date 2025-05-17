using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public static class BotInitializer
    {
        public static async Task Initialize(AppDbContext context)
        {
            var botExists = await context.Users.AnyAsync(u => u.Id == 1);

            if (!botExists)
            {
                var bot = new User
                {
                    Id = 1,
                    Username = "Bot",
                    Email = "bot@system.com",
                    Password = "f",
                };

                context.Users.Add(bot);
                await context.SaveChangesAsync();
            }
        }
    }
}
