using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model.Repositories
{
    internal class ClassRepository : BaseRepository<Class>
    {
        public ClassRepository(SchoolScheduleContext context) : base(context)
        {
        }

    }
}
