using Application.DTOs.Mentoring;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentoringController : ControllerBase
    {
        private readonly IMentoringService _mentoringService;

        public MentoringController(IMentoringService mentoringService)
        {
            _mentoringService = mentoringService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        #region Session Management

        /// <summary>
        /// Create a new mentoring session (Mentor only)
        /// </summary>
        [HttpPost("sessions")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult<MentoringSessionDetailDto>> CreateSession(
            [FromBody] CreateMentoringSessionDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var session = await _mentoringService.CreateSessionAsync(mentorId, dto);
                return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing mentoring session (Mentor only)
        /// </summary>
        [HttpPut("sessions/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult<MentoringSessionDetailDto>> UpdateSession(
            int id, [FromBody] UpdateMentoringSessionDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var session = await _mentoringService.UpdateSessionAsync(id, mentorId, dto);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Delete a mentoring session (Mentor only)
        /// </summary>
        [HttpDelete("sessions/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> DeleteSession(int id)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.DeleteSessionAsync(id, mentorId);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Get session details by ID
        /// </summary>
        [HttpGet("sessions/{id}")]
        public async Task<ActionResult<MentoringSessionDetailDto>> GetSessionById(int id)
        {
            var session = await _mentoringService.GetSessionByIdAsync(id);

            if (session == null)
                return NotFound(new { message = "Session not found" });

            return Ok(session);
        }

        /// <summary>
        /// Get upcoming sessions for current user
        /// </summary>
        [HttpGet("sessions/upcoming")]
        public async Task<ActionResult<IEnumerable<MentoringSessionListDto>>> GetUpcomingSessions()
        {
            var userId = GetCurrentUserId();
            var sessions = await _mentoringService.GetUpcomingSessionsAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Start a mentoring session (Mentor only)
        /// </summary>
        [HttpPost("sessions/{id}/start")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> StartSession(int id)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.StartSessionAsync(id, mentorId);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Session started successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Complete a mentoring session with notes (Mentor only)
        /// </summary>
        [HttpPost("sessions/{id}/complete")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> CompleteSession(
            int id, [FromBody] CompleteMentoringSessionDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.CompleteSessionAsync(id, mentorId, dto);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Session completed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel a mentoring session (Mentor or Mentee)
        /// </summary>
        [HttpPost("sessions/{id}/cancel")]
        public async Task<ActionResult> CancelSession(
            int id, [FromBody] CancelMentoringSessionDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mentoringService.CancelSessionAsync(id, userId, dto);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Session cancelled successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Rate a completed session (Mentee only)
        /// </summary>
        [HttpPost("sessions/{id}/rate")]
        public async Task<ActionResult> RateSession(
            int id, [FromBody] RateMentoringSessionDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mentoringService.RateSessionAsync(id, userId, dto);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Session rated successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Calendar Views

        /// <summary>
        /// Get mentor's calendar events for a date range
        /// </summary>
        [HttpGet("calendar/mentor")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetMentorCalendar(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var mentorId = GetCurrentUserId();
            var events = await _mentoringService.GetMentorCalendarAsync(mentorId, startDate, endDate);
            return Ok(events);
        }

        /// <summary>
        /// Get mentee's calendar events for a date range
        /// </summary>
        [HttpGet("calendar/mentee")]
        public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetMenteeCalendar(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var menteeId = GetCurrentUserId();
            var events = await _mentoringService.GetMenteeCalendarAsync(menteeId, startDate, endDate);
            return Ok(events);
        }

        /// <summary>
        /// Get available time slots for booking with a mentor
        /// </summary>
        [HttpGet("mentors/{mentorId}/available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableTimeSlotsDto>>> GetAvailableTimeSlots(
            int mentorId,
            [FromQuery] DateTime startDate,
            [FromQuery] int daysCount = 7,
            [FromQuery] int sessionDuration = 60)
        {
            var slots = await _mentoringService.GetAvailableTimeSlotsAsync(
                mentorId, startDate, daysCount, sessionDuration);
            return Ok(slots);
        }

        #endregion

        #region Participant Management

        /// <summary>
        /// Add participant to a group session (Mentor only)
        /// </summary>
        [HttpPost("sessions/{sessionId}/participants/{userId}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> AddParticipant(int sessionId, int userId)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.AddParticipantAsync(sessionId, mentorId, userId);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Participant added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove participant from a group session (Mentor only)
        /// </summary>
        [HttpDelete("sessions/{sessionId}/participants/{userId}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> RemoveParticipant(int sessionId, int userId)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.RemoveParticipantAsync(sessionId, mentorId, userId);

                if (!result)
                    return NotFound(new { message = "Participant not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Join a session (mark as joined)
        /// </summary>
        [HttpPost("sessions/{sessionId}/join")]
        public async Task<ActionResult> JoinSession(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mentoringService.JoinSessionAsync(sessionId, userId);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Joined session successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Availability Management

        /// <summary>
        /// Get mentor's availability schedule
        /// </summary>
        [HttpGet("mentors/{mentorId}/availability")]
        public async Task<ActionResult<IEnumerable<MentorAvailabilityDto>>> GetMentorAvailability(
            int mentorId)
        {
            var availabilities = await _mentoringService.GetMentorAvailabilityAsync(mentorId);
            return Ok(availabilities);
        }

        /// <summary>
        /// Add availability slot (Mentor only)
        /// </summary>
        [HttpPost("availability")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult<MentorAvailabilityDto>> AddAvailability(
            [FromBody] CreateAvailabilityDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var availability = await _mentoringService.AddAvailabilityAsync(mentorId, dto);
                return CreatedAtAction(nameof(GetMentorAvailability),
                    new { mentorId }, availability);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update availability slot (Mentor only)
        /// </summary>
        [HttpPut("availability/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> UpdateAvailability(
            int id, [FromBody] CreateAvailabilityDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.UpdateAvailabilityAsync(id, mentorId, dto);

                if (!result)
                    return NotFound(new { message = "Availability not found" });

                return Ok(new { message = "Availability updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete availability slot (Mentor only)
        /// </summary>
        [HttpDelete("availability/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> DeleteAvailability(int id)
        {
            var mentorId = GetCurrentUserId();
            var result = await _mentoringService.DeleteAvailabilityAsync(id, mentorId);

            if (!result)
                return NotFound(new { message = "Availability not found" });

            return NoContent();
        }

        #endregion

        #region Blocked Time Management

        /// <summary>
        /// Get mentor's blocked times
        /// </summary>
        [HttpGet("mentors/{mentorId}/blocked-times")]
        public async Task<ActionResult<IEnumerable<MentorBlockedTimeDto>>> GetBlockedTimes(
            int mentorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var blockedTimes = await _mentoringService.GetBlockedTimesAsync(
                mentorId, startDate, endDate);
            return Ok(blockedTimes);
        }

        /// <summary>
        /// Add blocked time (Mentor only)
        /// </summary>
        [HttpPost("blocked-times")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult<MentorBlockedTimeDto>> AddBlockedTime(
            [FromBody] CreateBlockedTimeDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var blockedTime = await _mentoringService.AddBlockedTimeAsync(mentorId, dto);
                return Ok(blockedTime);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update blocked time (Mentor only)
        /// </summary>
        [HttpPut("blocked-times/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> UpdateBlockedTime(
            int id, [FromBody] CreateBlockedTimeDto dto)
        {
            try
            {
                var mentorId = GetCurrentUserId();
                var result = await _mentoringService.UpdateBlockedTimeAsync(id, mentorId, dto);

                if (!result)
                    return NotFound(new { message = "Blocked time not found" });

                return Ok(new { message = "Blocked time updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete blocked time (Mentor only)
        /// </summary>
        [HttpDelete("blocked-times/{id}")]
        [Authorize(Roles = "Mentoring Staff")]
        public async Task<ActionResult> DeleteBlockedTime(int id)
        {
            var mentorId = GetCurrentUserId();
            var result = await _mentoringService.DeleteBlockedTimeAsync(id, mentorId);

            if (!result)
                return NotFound(new { message = "Blocked time not found" });

            return NoContent();
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get mentor statistics
        /// </summary>
        [HttpGet("mentors/{mentorId}/statistics")]
        public async Task<ActionResult<MentorStatisticsDto>> GetMentorStatistics(int mentorId)
        {
            var statistics = await _mentoringService.GetMentorStatisticsAsync(mentorId);
            return Ok(statistics);
        }

        #endregion

        #region Attachments

        /// <summary>
        /// Add attachment to session
        /// </summary>
        [HttpPost("sessions/{sessionId}/attachments")]
        public async Task<ActionResult> AddAttachment(
            int sessionId,
            [FromForm] string fileName,
            [FromForm] string fileUrl,
            [FromForm] long fileSize)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mentoringService.AddAttachmentAsync(
                    sessionId, userId, fileName, fileUrl, fileSize);

                if (!result)
                    return NotFound(new { message = "Session not found" });

                return Ok(new { message = "Attachment added successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Remove attachment from session
        /// </summary>
        [HttpDelete("attachments/{attachmentId}")]
        public async Task<ActionResult> RemoveAttachment(int attachmentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _mentoringService.RemoveAttachmentAsync(attachmentId, userId);

                if (!result)
                    return NotFound(new { message = "Attachment not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        #endregion
    }
}