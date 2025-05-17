using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ILogService _logService;
        private readonly ProjectService _projectRepository;

        public CalendarModel(IMeetingRepository meetingRepository, ProjectService projectRepository, ILogService logService)
        {
            _meetingRepository = meetingRepository;
            _projectRepository = projectRepository;
            _logService = logService;
        }

        public Project Project { get; set; }

        public async Task<IActionResult> OnGetAsync(int projectId)
        {
            Project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (Project == null)
            {
                return NotFound();
            }
            return Page();
            
        }

        public async Task<IActionResult> OnGetMeetingsAsync(int projectId)
        {
            Project = await _projectRepository.GetProjectByIdAsync(projectId);
            var meetings = await _meetingRepository.GetMeetingsAsync(Project);
            var events = meetings.Select(m => new
            {
                id = m.Id,
                title = m.Title,
                start = m.StartTime.ToString("o"),
                end = m.EndTime.ToString("o"),
                location = m.Location,
                isOnline = m.IsOnline,
                url = m.MeetingUrl,
                description = m.Description,
                participants = m.Participants
            });
            return new JsonResult(events);
        }

        public async Task<IActionResult> OnGetMeetingDetailsAsync(int id)
        {
            var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            return new JsonResult(meeting);
        }

        public async Task<IActionResult> OnPostSaveMeetingAsync([FromBody] MeetingDto meetingDto)
        {
            try
            {
                Meeting meeting;
                if (meetingDto.Id > 0)
                {
                    meeting = await _meetingRepository.GetMeetingByIdAsync(meetingDto.Id);
                    if (meeting == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    meeting = new Meeting();
                }
                meeting.Title = meetingDto.Title;
                meeting.Description = meetingDto.Description;
                meeting.StartTime = DateTime.Parse(meetingDto.StartTime);
                meeting.EndTime = DateTime.Parse(meetingDto.EndTime);
                meeting.Location = meetingDto.Location;
                meeting.IsOnline = meetingDto.IsOnline;
                meeting.MeetingUrl = meetingDto.MeetingUrl;
                meeting.Participants = meetingDto.Participants;
                meeting.ProjectId = meetingDto.projectId;

                if (meetingDto.Id > 0)
                {
                    await _logService.AddLog(meeting.ProjectId, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.UpdateMeeting, meeting.Title);
                    await _meetingRepository.UpdateMeetingAsync(meeting);
                }
                else
                {
                    meeting.OrganizerId = User.Identity.Name; // или ваш механизм получения текущего пользователя
                    await _logService.AddLog(meeting.ProjectId, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.AddMeeting, meeting.Title);
                    await _meetingRepository.AddMeetingAsync(meeting);
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> OnDeleteDeleteMeetingAsync(int id)
        {
            try
            {
                var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
                await _logService.AddLog(meeting.ProjectId, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.DeleteMeeting, meeting.Title);
                await _meetingRepository.DeleteMeetingAsync(id);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        public class MeetingDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Location { get; set; }
            public bool IsOnline { get; set; }
            public string MeetingUrl { get; set; }
            public string Participants { get; set; }
            public int projectId { get; set; }
        }
    }
}
