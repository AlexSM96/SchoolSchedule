using Microsoft.EntityFrameworkCore;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SchoolSchedule.Model
{
    internal class Table
    {
        private SchoolScheduleContext _context;
        public Table()
        {
            _context = new SchoolScheduleContext();
        }

        public List<Class> GetTableClasses()
        {
            IQueryable<Class> classes = _context.Classes
                .OrderBy(x => x.ClassName);
            return classes
                .Include(x=>x.Schedules)
                .ToList();
        }

        public List<Schedule> GetTableSchedule()
        {
            IQueryable<Schedule> schedulies = _context.Schedules
                .OrderBy(x => x.Class.ClassName);
            return schedulies
                .Include(x => x.TeacherAndLesson.LessonNameNavigation)
                .Include(x => x.TeacherAndLesson)
                .Include(x => x.LessonNumberNavigation)
                .Include(x => x.WeekDayNavigation)
                .ToList();
        }

        public List<TeacherAndLesson> GetTableTeacherAndLessons()
        {
            IQueryable<TeacherAndLesson> teachers = _context.TeacherAndLessons;
            return teachers
                .Include("Teacher")
                .ToList();
        }

        public List<WeekDay> GetTableWeekDay()
        {
            IQueryable<WeekDay> weekDay = _context.WeekDays
                .OrderBy(x=>x.WeekDay1);
            return weekDay.ToList();
        }

        public List<Schedule> GetDataD(string selectedClass)
        {
            IQueryable<Schedule> schedulies = _context.Schedules
                .Where(x => x.Class.ClassName.Contains(selectedClass));
            return schedulies.ToList();
        }
    }
}
