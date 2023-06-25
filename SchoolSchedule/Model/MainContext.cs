using Microsoft.EntityFrameworkCore;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SchoolSchedule.Model
{
    internal class MainContext
    {
        private SchoolScheduleContext _context;
        public MainContext()
        {
            _context = new SchoolScheduleContext();
        }

        public List<Class> GetClasses()
        {
            IQueryable<Class> classes = _context.Classes.OrderBy(x => x.ClassName);
            return classes.ToList();
        }

        public List<Schedule> GetSchedule()
        {
            IQueryable<Schedule> schedulies = _context.Schedules.OrderBy(x => x.Class.ClassName);
            return schedulies
                .Include(x => x.TeacherAndLesson.LessonNameNavigation)
                .Include(x => x.TeacherAndLesson)
                .Include(x => x.LessonNumberNavigation)
                .Include(x => x.WeekDayNavigation)
                .ToList();
        }

        public List<TeacherAndLesson> GetTeacherAndLessons()
        {
            IQueryable<TeacherAndLesson> teachers = _context.TeacherAndLessons;
            return teachers
                .Include("Teacher")
                .ToList();
        }
    }
}
