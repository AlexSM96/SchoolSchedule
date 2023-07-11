using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model.Repositories
{
    internal class TeacherAndLessonRepository : BaseRepository<TeacherAndLesson>
    {
        public TeacherAndLessonRepository(SchoolScheduleContext context) : base(context)
        {
        }
    }
}
