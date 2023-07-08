using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using System.Collections.Generic;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.DB.Repositories;

namespace SchoolSchedule.Model.Extensions
{
    internal static class ScheduleExtension
    {
        private static readonly SchoolScheduleContext _context;
        private static readonly IBaseRepository<Schedule> _scheduleRepository;

        static ScheduleExtension()
        {
            _context = new();
            _scheduleRepository = new ScheduleRepository(_context);
        }

        public static void AddSchedule(this List<Schedule> schedules, Schedule schedule)
        {
            _scheduleRepository.CreateAsync(schedule);
        }

        public static void RemoveSchedule(this List<Schedule> schedules, Schedule schedule)
        {
            _scheduleRepository.RemoveAsync(schedule);
        }
    }
}
