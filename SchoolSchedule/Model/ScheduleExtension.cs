using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using System.Collections.Generic;

namespace SchoolSchedule.Model
{
    internal static class ScheduleExtension
    {
        private static readonly SchoolScheduleContext _context = new();
       
        public static void AddSchedule(this List<Schedule> schedules, Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
        }

        public static void RemoveSchedule(this List<Schedule> schedules, Schedule schedule)
        {
            _context.Schedules.Remove(schedule);
            _context.SaveChanges();
        }
    }
}
