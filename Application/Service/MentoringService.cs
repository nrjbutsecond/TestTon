using Application.DTOs.Mentoring;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Mentor;
using Domain.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MentoringService : IMentoringService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MentoringService> _logger;

        public MentoringService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MentoringService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ============================================
        // SESSION MANAGEMENT
        // ============================================

        public async Task<MentoringSessionDetailDto> CreateSessionAsync(
            int mentorId, CreateMentoringSessionDto dto)
        {
            try
            {
                var mentor = await _unitOfWork.Users.GetByIdAsync(mentorId);
                if (mentor == null || mentor.Role != UserRoles.MentoringStaff)
                {
                    throw new UnauthorizedAccessException("User is not a mentor");
                }

                var sessionEndDate = dto.SessionDate.AddMinutes(dto.Duration);

                // Check conflicts with existing sessions
                var hasConflict = await _unitOfWork.MentoringSessions
                    .HasConflictingSessionAsync(mentorId, dto.SessionDate, sessionEndDate);

                if (hasConflict)
                {
                    throw new InvalidOperationException("Session conflicts with existing session");
                }

                // Check conflicts with blocked times
                var hasBlockedConflict = await _unitOfWork.MentorBlockedTimes
                    .HasOverlapAsync(mentorId, dto.SessionDate, sessionEndDate);

                if (hasBlockedConflict)
                {
                    throw new InvalidOperationException("Session conflicts with blocked time");
                }

                var session = _mapper.Map<MentoringRecord>(dto);
                session.MentorId = mentorId;
                session.CreatedBy = mentorId.ToString();
                session.CreatedAt = DateTime.UtcNow;

                var createdSession = await _unitOfWork.MentoringSessions.AddAsync(session);

                if (dto.SessionType != "OneOnOne" && dto.ParticipantIds != null && dto.ParticipantIds.Any())
                {
                    foreach (var participantId in dto.ParticipantIds.Take(dto.MaxParticipants))
                    {
                        var participant = new MentoringSessionParticipant
                        {
                            MentoringRecordId = createdSession.Id,
                            UserId = participantId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = mentorId.ToString()
                        };
                        await _unitOfWork.MentoringParticipants.AddAsync(participant);
                    }
                    createdSession.CurrentParticipants = dto.ParticipantIds.Count;
                    _unitOfWork.MentoringSessions.Update(createdSession);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session created: {SessionId} by mentor: {MentorId}",
                    createdSession.Id, mentorId);

                return _mapper.Map<MentoringSessionDetailDto>(
                    await _unitOfWork.MentoringSessions.GetSessionWithDetailsAsync(createdSession.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mentoring session for mentor: {MentorId}", mentorId);
                throw;
            }
        }

        public async Task<MentoringSessionDetailDto> UpdateSessionAsync(
            int sessionId, int mentorId, UpdateMentoringSessionDto dto)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null || session.IsDeleted)
                {
                    throw new KeyNotFoundException("Session not found");
                }

                if (session.MentorId != mentorId)
                {
                    throw new UnauthorizedAccessException("Only the session mentor can update this session");
                }

                if (session.Status == MentoringSessionStatus.Completed ||
                    session.Status == MentoringSessionStatus.Cancelled)
                {
                    throw new InvalidOperationException("Cannot update completed or cancelled sessions");
                }

                var newEndDate = dto.SessionDate.AddMinutes(dto.Duration);

                if (session.SessionDate != dto.SessionDate || session.Duration != dto.Duration)
                {
                    var hasConflict = await _unitOfWork.MentoringSessions
                        .HasConflictingSessionAsync(mentorId, dto.SessionDate, newEndDate, sessionId);

                    if (hasConflict)
                    {
                        throw new InvalidOperationException("Session conflicts with existing session");
                    }
                }

                _mapper.Map(dto, session);
                session.UpdatedAt = DateTime.UtcNow;
                session.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentoringSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session updated: {SessionId}", sessionId);

                return _mapper.Map<MentoringSessionDetailDto>(
                    await _unitOfWork.MentoringSessions.GetSessionWithDetailsAsync(sessionId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mentoring session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> DeleteSessionAsync(int sessionId, int mentorId)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null || session.IsDeleted)
                {
                    return false;
                }

                if (session.MentorId != mentorId)
                {
                    throw new UnauthorizedAccessException("Only the session mentor can delete this session");
                }

                session.IsDeleted = true;
                session.DeletedAt = DateTime.UtcNow;
                session.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentoringSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session deleted: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mentoring session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<MentoringSessionDetailDto?> GetSessionByIdAsync(int sessionId)
        {
            var session = await _unitOfWork.MentoringSessions.GetSessionWithDetailsAsync(sessionId);
            return session != null ? _mapper.Map<MentoringSessionDetailDto>(session) : null;
        }

        public async Task<IEnumerable<MentoringSessionListDto>> GetUpcomingSessionsAsync(int userId)
        {
            var sessions = await _unitOfWork.MentoringSessions.GetUpcomingSessionsAsync(userId);
            return _mapper.Map<IEnumerable<MentoringSessionListDto>>(sessions);
        }

        public async Task<bool> StartSessionAsync(int sessionId, int mentorId)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null || session.MentorId != mentorId)
                {
                    return false;
                }

                if (session.Status != MentoringSessionStatus.Scheduled)
                {
                    throw new InvalidOperationException("Only scheduled sessions can be started");
                }

                session.Status = MentoringSessionStatus.InProgress;
                session.UpdatedAt = DateTime.UtcNow;
                session.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentoringSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session started: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> CompleteSessionAsync(
            int sessionId, int mentorId, CompleteMentoringSessionDto dto)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null || session.MentorId != mentorId)
                {
                    return false;
                }

                session.Status = MentoringSessionStatus.Completed;
                session.SessionNotes = dto.SessionNotes;
                session.ActionItems = dto.ActionItems != null && dto.ActionItems.Any()
                    ? System.Text.Json.JsonSerializer.Serialize(dto.ActionItems)
                    : null;
                session.MenteeProgress = dto.MenteeProgress;
                session.NextSessionDate = dto.NextSessionDate;
                session.UpdatedAt = DateTime.UtcNow;
                session.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentoringSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session completed: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> CancelSessionAsync(
            int sessionId, int userId, CancelMentoringSessionDto dto)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null)
                {
                    return false;
                }

                if (session.MentorId != userId && session.MenteeId != userId)
                {
                    throw new UnauthorizedAccessException("Only participants can cancel this session");
                }

                if (session.Status == MentoringSessionStatus.Completed)
                {
                    throw new InvalidOperationException("Cannot cancel completed sessions");
                }

                session.Status = MentoringSessionStatus.Cancelled;
                session.CancellationReason = dto.Reason;
                session.CancelledAt = DateTime.UtcNow;
                session.CancelledBy = userId;
                session.UpdatedAt = DateTime.UtcNow;
                session.UpdatedBy = userId.ToString();

                _unitOfWork.MentoringSessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mentoring session cancelled: {SessionId} by user: {UserId}",
                    sessionId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> RateSessionAsync(
            int sessionId, int menteeId, RateMentoringSessionDto dto)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null)
                {
                    return false;
                }

                var isParticipant = session.MenteeId == menteeId ||
                    await _unitOfWork.MentoringParticipants.GetBySessionAndUserAsync(sessionId, menteeId) != null;

                if (!isParticipant)
                {
                    throw new UnauthorizedAccessException("Only participants can rate this session");
                }

                if (session.Status != MentoringSessionStatus.Completed)
                {
                    throw new InvalidOperationException("Only completed sessions can be rated");
                }

                if (session.SessionType == SessionType.OneOnOne)
                {
                    session.Rating = dto.Rating;
                    session.MenteeFeedback = dto.Feedback;
                    session.UpdatedAt = DateTime.UtcNow;
                    session.UpdatedBy = menteeId.ToString();
                    _unitOfWork.MentoringSessions.Update(session);
                }
                else
                {
                    var participant = await _unitOfWork.MentoringParticipants
                        .GetBySessionAndUserAsync(sessionId, menteeId);

                    if (participant != null)
                    {
                        participant.Rating = dto.Rating;
                        participant.Feedback = dto.Feedback;
                        participant.UpdatedAt = DateTime.UtcNow;
                        participant.UpdatedBy = menteeId.ToString();
                        _unitOfWork.MentoringParticipants.Update(participant);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session rated: {SessionId} by user: {UserId}",
                    sessionId, menteeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rating session: {SessionId}", sessionId);
                throw;
            }
        }

        // ============================================
        // CALENDAR VIEWS
        // ============================================

        public async Task<IEnumerable<CalendarEventDto>> GetMentorCalendarAsync(
            int mentorId, DateTime startDate, DateTime endDate)
        {
            var events = new List<CalendarEventDto>();

            var sessions = await _unitOfWork.MentoringSessions
                .GetSessionsByMentorAndDateRangeAsync(mentorId, startDate, endDate);
            events.AddRange(_mapper.Map<IEnumerable<CalendarEventDto>>(sessions));

            var blockedTimes = await _unitOfWork.MentorBlockedTimes
                .GetByMentorAndDateRangeAsync(mentorId, startDate, endDate);
            events.AddRange(_mapper.Map<IEnumerable<CalendarEventDto>>(blockedTimes));

            return events.OrderBy(e => e.Start);
        }

        public async Task<IEnumerable<CalendarEventDto>> GetMenteeCalendarAsync(
            int menteeId, DateTime startDate, DateTime endDate)
        {
            var sessions = await _unitOfWork.MentoringSessions
                .GetSessionsByMenteeAndDateRangeAsync(menteeId, startDate, endDate);

            return _mapper.Map<IEnumerable<CalendarEventDto>>(sessions)
                .OrderBy(e => e.Start);
        }

        // ============================================
        // PARTICIPANT MANAGEMENT
        // ============================================

        public async Task<bool> AddParticipantAsync(int sessionId, int mentorId, int userId)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetSessionWithDetailsAsync(sessionId);
                if (session == null || session.MentorId != mentorId)
                {
                    return false;
                }

                if (session.CurrentParticipants >= session.MaxParticipants)
                {
                    throw new InvalidOperationException("Session is full");
                }

                var existingParticipant = await _unitOfWork.MentoringParticipants
                    .GetBySessionAndUserAsync(sessionId, userId);

                if (existingParticipant != null)
                {
                    throw new InvalidOperationException("User is already a participant");
                }

                var participant = new MentoringSessionParticipant
                {
                    MentoringRecordId = sessionId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = mentorId.ToString()
                };

                await _unitOfWork.MentoringParticipants.AddAsync(participant);

                session.CurrentParticipants++;
                _unitOfWork.MentoringSessions.Update(session);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Participant added to session: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding participant to session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> RemoveParticipantAsync(int sessionId, int mentorId, int userId)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetSessionWithDetailsAsync(sessionId);
                if (session == null || session.MentorId != mentorId)
                {
                    return false;
                }

                var participant = await _unitOfWork.MentoringParticipants
                    .GetBySessionAndUserAsync(sessionId, userId);

                if (participant == null)
                {
                    return false;
                }

                participant.IsDeleted = true;
                participant.DeletedAt = DateTime.UtcNow;
                _unitOfWork.MentoringParticipants.Update(participant);

                session.CurrentParticipants = Math.Max(0, session.CurrentParticipants - 1);
                _unitOfWork.MentoringSessions.Update(session);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Participant removed from session: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing participant from session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> JoinSessionAsync(int sessionId, int userId)
        {
            try
            {
                var participant = await _unitOfWork.MentoringParticipants
                    .GetBySessionAndUserAsync(sessionId, userId);

                if (participant == null)
                {
                    throw new InvalidOperationException("User is not a participant in this session");
                }

                participant.HasJoined = true;
                participant.JoinedAt = DateTime.UtcNow;
                participant.UpdatedAt = DateTime.UtcNow;
                participant.UpdatedBy = userId.ToString();

                _unitOfWork.MentoringParticipants.Update(participant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User joined session: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining session: {SessionId}", sessionId);
                throw;
            }
        }

        // ============================================
        // AVAILABILITY MANAGEMENT
        // ============================================

        public async Task<IEnumerable<MentorAvailabilityDto>> GetMentorAvailabilityAsync(int mentorId)
        {
            var availabilities = await _unitOfWork.MentorAvailabilities
                .GetByMentorIdAsync(mentorId);
            return _mapper.Map<IEnumerable<MentorAvailabilityDto>>(availabilities);
        }

        public async Task<MentorAvailabilityDto> AddAvailabilityAsync(
            int mentorId, CreateAvailabilityDto dto)
        {
            try
            {
                var availability = _mapper.Map<MentorAvailability>(dto);
                availability.MentorId = mentorId;
                availability.CreatedAt = DateTime.UtcNow;
                availability.CreatedBy = mentorId.ToString();

                var created = await _unitOfWork.MentorAvailabilities.AddAsync(availability);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Availability added for mentor: {MentorId}", mentorId);
                return _mapper.Map<MentorAvailabilityDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding availability for mentor: {MentorId}", mentorId);
                throw;
            }
        }

        public async Task<bool> UpdateAvailabilityAsync(
            int availabilityId, int mentorId, CreateAvailabilityDto dto)
        {
            try
            {
                var availability = await _unitOfWork.MentorAvailabilities
                    .GetByIdAndMentorAsync(availabilityId, mentorId);

                if (availability == null)
                {
                    return false;
                }

                availability.DayOfWeek = Enum.Parse<DayOfWeek>(dto.DayOfWeek);
                availability.StartTime = TimeSpan.Parse(dto.StartTime);
                availability.EndTime = TimeSpan.Parse(dto.EndTime);
                availability.IsRecurring = dto.IsRecurring;
                availability.RecurringEndDate = dto.RecurringEndDate;
                availability.SpecificDate = dto.SpecificDate;
                availability.UpdatedAt = DateTime.UtcNow;
                availability.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentorAvailabilities.Update(availability);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Availability updated: {AvailabilityId}", availabilityId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability: {AvailabilityId}", availabilityId);
                throw;
            }
        }

        public async Task<bool> DeleteAvailabilityAsync(int availabilityId, int mentorId)
        {
            try
            {
                var availability = await _unitOfWork.MentorAvailabilities
                    .GetByIdAndMentorAsync(availabilityId, mentorId);

                if (availability == null)
                {
                    return false;
                }

                availability.IsDeleted = true;
                availability.DeletedAt = DateTime.UtcNow;
                _unitOfWork.MentorAvailabilities.Update(availability);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Availability deleted: {AvailabilityId}", availabilityId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting availability: {AvailabilityId}", availabilityId);
                throw;
            }
        }

        // ============================================
        // BLOCKED TIME MANAGEMENT
        // ============================================

        public async Task<IEnumerable<MentorBlockedTimeDto>> GetBlockedTimesAsync(
            int mentorId, DateTime startDate, DateTime endDate)
        {
            var blockedTimes = await _unitOfWork.MentorBlockedTimes
                .GetByMentorAndDateRangeAsync(mentorId, startDate, endDate);
            return _mapper.Map<IEnumerable<MentorBlockedTimeDto>>(blockedTimes);
        }

        public async Task<MentorBlockedTimeDto> AddBlockedTimeAsync(
            int mentorId, CreateBlockedTimeDto dto)
        {
            try
            {
                var blockedTime = _mapper.Map<MentorBlockedTime>(dto);
                blockedTime.MentorId = mentorId;
                blockedTime.CreatedAt = DateTime.UtcNow;
                blockedTime.CreatedBy = mentorId.ToString();

                var created = await _unitOfWork.MentorBlockedTimes.AddAsync(blockedTime);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Blocked time added for mentor: {MentorId}", mentorId);
                return _mapper.Map<MentorBlockedTimeDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding blocked time for mentor: {MentorId}", mentorId);
                throw;
            }
        }

        public async Task<bool> UpdateBlockedTimeAsync(
            int blockedTimeId, int mentorId, CreateBlockedTimeDto dto)
        {
            try
            {
                var blockedTime = await _unitOfWork.MentorBlockedTimes
                    .GetByIdAndMentorAsync(blockedTimeId, mentorId);

                if (blockedTime == null)
                {
                    return false;
                }

                blockedTime.StartDateTime = dto.StartDateTime;
                blockedTime.EndDateTime = dto.EndDateTime;
                blockedTime.Reason = dto.Reason;
                blockedTime.IsAllDay = dto.IsAllDay;
                blockedTime.UpdatedAt = DateTime.UtcNow;
                blockedTime.UpdatedBy = mentorId.ToString();

                _unitOfWork.MentorBlockedTimes.Update(blockedTime);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Blocked time updated: {BlockedTimeId}", blockedTimeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blocked time: {BlockedTimeId}", blockedTimeId);
                throw;
            }
        }

        public async Task<bool> DeleteBlockedTimeAsync(int blockedTimeId, int mentorId)
        {
            try
            {
                var blockedTime = await _unitOfWork.MentorBlockedTimes
                    .GetByIdAndMentorAsync(blockedTimeId, mentorId);

                if (blockedTime == null)
                {
                    return false;
                }

                blockedTime.IsDeleted = true;
                blockedTime.DeletedAt = DateTime.UtcNow;
                _unitOfWork.MentorBlockedTimes.Update(blockedTime);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Blocked time deleted: {BlockedTimeId}", blockedTimeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blocked time: {BlockedTimeId}", blockedTimeId);
                throw;
            }
        }

        // ============================================
        // AVAILABLE TIME SLOTS
        // ============================================

        public async Task<IEnumerable<AvailableTimeSlotsDto>> GetAvailableTimeSlotsAsync(
            int mentorId, DateTime startDate, int daysCount, int sessionDuration)
        {
            var result = new List<AvailableTimeSlotsDto>();
            var endDate = startDate.AddDays(daysCount);

            var availabilities = await _unitOfWork.MentorAvailabilities
                .GetByMentorIdAsync(mentorId);

            var sessions = await _unitOfWork.MentoringSessions
                .GetSessionsByMentorAndDateRangeAsync(mentorId, startDate, endDate);

            var blockedTimes = await _unitOfWork.MentorBlockedTimes
                .GetByMentorAndDateRangeAsync(mentorId, startDate, endDate);

            for (int i = 0; i < daysCount; i++)
            {
                var currentDate = startDate.AddDays(i);
                var dayOfWeek = currentDate.DayOfWeek;

                var dayAvailability = availabilities
                    .Where(a => a.DayOfWeek == dayOfWeek && a.IsRecurring)
                    .ToList();

                if (!dayAvailability.Any())
                    continue;

                var timeSlots = new List<TimeSlotDto>();

                foreach (var availability in dayAvailability)
                {
                    var slotStart = availability.StartTime;
                    var slotEnd = availability.EndTime;

                    while (slotStart.Add(TimeSpan.FromMinutes(sessionDuration)) <= slotEnd)
                    {
                        var slotStartDateTime = currentDate.Date.Add(slotStart);
                        var slotEndDateTime = slotStartDateTime.AddMinutes(sessionDuration);

                        var isAvailable = true;
                        string? reason = null;

                        if (sessions.Any(s =>
                            s.SessionDate < slotEndDateTime &&
                            s.SessionEndDate > slotStartDateTime))
                        {
                            isAvailable = false;
                            reason = "Booked";
                        }

                        if (blockedTimes.Any(b =>
                            b.StartDateTime < slotEndDateTime &&
                            b.EndDateTime > slotStartDateTime))
                        {
                            isAvailable = false;
                            reason = "Blocked";
                        }

                        timeSlots.Add(new TimeSlotDto
                        {
                            StartTime = slotStart.ToString(@"hh\:mm"),
                            EndTime = slotStart.Add(TimeSpan.FromMinutes(sessionDuration)).ToString(@"hh\:mm"),
                            IsAvailable = isAvailable,
                            Reason = reason
                        });

                        slotStart = slotStart.Add(TimeSpan.FromMinutes(30));
                    }
                }

                if (timeSlots.Any())
                {
                    result.Add(new AvailableTimeSlotsDto
                    {
                        Date = currentDate,
                        TimeSlots = timeSlots
                    });
                }
            }

            return result;
        }

        // ============================================
        // STATISTICS
        // ============================================

        public async Task<MentorStatisticsDto> GetMentorStatisticsAsync(int mentorId)
        {
            var totalSessions = await _unitOfWork.MentoringSessions
                .GetTotalSessionCountAsync(mentorId);

            var completedSessions = await _unitOfWork.MentoringSessions
                .GetCompletedSessionCountAsync(mentorId);

            var upcomingSessions = await _unitOfWork.MentoringSessions
                .GetUpcomingSessionCountAsync(mentorId);

            var averageRating = await _unitOfWork.MentoringSessions
                .GetAverageRatingAsync(mentorId);

            var totalMentees = await _unitOfWork.MentoringSessions
                .GetTotalMenteesCountAsync(mentorId);

            return new MentorStatisticsDto
            {
                TotalSessions = totalSessions,
                CompletedSessions = completedSessions,
                UpcomingSessions = upcomingSessions,
                AverageRating = Math.Round(averageRating, 2),
                TotalMentees = totalMentees
            };
        }

        // ============================================
        // ATTACHMENT MANAGEMENT
        // ============================================

        public async Task<bool> AddAttachmentAsync(
            int sessionId, int userId, string fileName, string fileUrl, long fileSize)
        {
            try
            {
                var session = await _unitOfWork.MentoringSessions.GetByIdAsync(sessionId);
                if (session == null)
                {
                    return false;
                }

                var isAuthorized = session.MentorId == userId ||
                    session.MenteeId == userId ||
                    await _unitOfWork.MentoringParticipants.GetBySessionAndUserAsync(sessionId, userId) != null;

                if (!isAuthorized)
                {
                    throw new UnauthorizedAccessException("User is not authorized to add attachments");
                }

                var attachment = new MentoringSessionAttachment
                {
                    MentoringRecordId = sessionId,
                    FileName = fileName,
                    FileUrl = fileUrl,
                    FileSize = fileSize,
                    FileType = System.IO.Path.GetExtension(fileName),
                    UploadedBy = userId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString()
                };

                await _unitOfWork.MentoringAttachments.AddAsync(attachment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Attachment added to session: {SessionId}", sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment to session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> RemoveAttachmentAsync(int attachmentId, int userId)
        {
            try
            {
                var attachment = await _unitOfWork.MentoringAttachments
                    .GetByIdWithSessionAsync(attachmentId);

                if (attachment == null)
                {
                    return false;
                }

                if (attachment.MentoringRecord.MentorId != userId &&
                    attachment.UploadedBy != userId.ToString())
                {
                    throw new UnauthorizedAccessException("User is not authorized to remove this attachment");
                }

                attachment.IsDeleted = true;
                attachment.DeletedAt = DateTime.UtcNow;
                _unitOfWork.MentoringAttachments.Update(attachment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Attachment removed: {AttachmentId}", attachmentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing attachment: {AttachmentId}", attachmentId);
                throw;
            }
        }
    }
}