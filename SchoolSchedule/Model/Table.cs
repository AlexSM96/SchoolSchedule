using Microsoft.EntityFrameworkCore;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.DB.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SchoolSchedule.Model
{
    internal class Table
    {
        private readonly SchoolScheduleContext _context;
        private readonly IBaseRepository<Schedule> _scheduleRepository;
        private readonly IBaseRepository<Class> _classRepository;
        private readonly IBaseRepository<TeacherAndLesson> _teacherAndLessonRepository;
        private readonly IBaseRepository<WeekDay> _weekDayRepository;


        public Table()
        {
            _context = new SchoolScheduleContext();
            _scheduleRepository = new ScheduleRepository(_context);
            _classRepository = new ClassRepository(_context);
            _teacherAndLessonRepository = new TeacherAndLeesonRepository(_context);
            _weekDayRepository = new WeekDayRepository(_context);
        }

        public List<Class> GetTableClasses()
        {
            return _classRepository.GetAll()
                .OrderBy(x=> x.ClassName)
                .Include(x =>x.Schedules)
                .ToList();
        }

        public List<Schedule> GetTableSchedule()
        {
            return _scheduleRepository.GetAll()
                .OrderBy(x => x.Class.ClassName)
                .Include(x => x.TeacherAndLesson.LessonNameNavigation)
                .Include(x=>x.WeekDayNavigation)
                .Include(x=>x.LessonNumberNavigation)
                .ToList();
        }

        public List<TeacherAndLesson> GetTableTeacherAndLessons()
        {
            return _teacherAndLessonRepository.GetAll()
                .Include(x => x.Teacher)
                .ToList();
        }

        public List<WeekDay> GetTableWeekDay()
        {
           return _weekDayRepository.GetAll()
               .OrderBy(x=>x.WeekDay1)
               .ToList();  
        }
         

        public List<Schedule> GetDataD(string selectedClass)
        {
            return _scheduleRepository.GetAll()
                .Where(x => x.Class.ClassName.Contains(selectedClass))
                .ToList();
        }
    }
}
