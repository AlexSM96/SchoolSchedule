using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.Model.Repositories;

namespace SchoolSchedule.Model.Service
{
    internal class TeacherAndLessonService
    {
        public IBaseRepository<TeacherAndLesson> TeacherAndLessonRepository { get; }

        public TeacherAndLessonService(SchoolScheduleContext context)
        {
            TeacherAndLessonRepository = new TeacherAndLessonRepository(context);
        }
    }
}
