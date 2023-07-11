using Microsoft.EntityFrameworkCore;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.Model.Repositories;
using System.Collections.Generic;

namespace SchoolSchedule.Model.Service
{
    internal class ScheduleService
    {
        private readonly SchoolScheduleContext _context;

        public ScheduleService(SchoolScheduleContext context)
        {
            _context = context;
            ScheduleRepository = new ScheduleRepository(context);
        }
        public IBaseRepository<Schedule> ScheduleRepository { get; }

        public IEnumerable<Schedule> GetFullSchedule()
        {
            var schedule = ScheduleRepository.GetAll()
                .Include(x => x.TeacherAndLesson.LessonNameNavigation)
                .Include(x => x.TeacherAndLesson.Teacher)
                .Include(x => x.LessonNumberNavigation)
                .Include(x => x.WeekDayNavigation);
            return schedule;
        }

    }
}
