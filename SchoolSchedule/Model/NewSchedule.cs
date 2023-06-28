using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model
{
    internal static class NewSchedule
    {
        private static SchoolScheduleContext _context;
        static NewSchedule()
        {
            _context = new SchoolScheduleContext();
        }

        public static void Add
            (int weekDay, int classId, byte lessonNumber, byte teacherId, string lessonName)
        {
            _context.Schedules.Add(new Schedule
            {
                WeekDay = weekDay,
                ClassId = classId,
                LessonNumber = lessonNumber,
                TeacherId = teacherId,
                LessonName = lessonName
            });
            _context.SaveChanges();
        }

        public static void Remove
            (int weekDay, int classId, byte lessonNumber, byte teacherId, string lessonName)
        {
            _context.Schedules.Remove(new Schedule
            {
                WeekDay = weekDay,
                ClassId = classId,
                LessonNumber = lessonNumber,
                TeacherId = teacherId,
                LessonName = lessonName
            });
            _context.SaveChanges();
        }
    }
}
