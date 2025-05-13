using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public interface IMeetingRepository
    {
        Task<List<Meeting>> GetMeetingsAsync(Project project);
        Task<Meeting> GetMeetingByIdAsync(int id);
        Task AddMeetingAsync(Meeting meeting);
        Task UpdateMeetingAsync(Meeting meeting);
        Task DeleteMeetingAsync(int id);
    }
    public class MeetingRepository : IMeetingRepository
    {
        private readonly AppDbContext _context;

        public MeetingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Meeting>> GetMeetingsAsync(Project project)
        {
            return await _context.Meetings.Where(x => x.Project == project).ToListAsync();
        }

        public async Task<Meeting> GetMeetingByIdAsync(int id)
        {
            return await _context.Meetings.FindAsync(id);
        }

        public async Task AddMeetingAsync(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMeetingAsync(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMeetingAsync(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
