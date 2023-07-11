using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model.Repositories
{
    internal class ScheduleRepository : BaseRepository<Schedule>
    {
        public ScheduleRepository(SchoolScheduleContext context) : base(context)
        {
        }
    }
}
