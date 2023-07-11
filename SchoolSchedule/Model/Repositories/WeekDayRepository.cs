using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model.Repositories
{
    internal class WeekDayRepository : BaseRepository<WeekDay>
    {
        public WeekDayRepository(SchoolScheduleContext context) : base(context)
        {
        }
    }
}
