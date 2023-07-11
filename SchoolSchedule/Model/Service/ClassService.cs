using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.Model.Repositories;

namespace SchoolSchedule.Model.Service
{
    internal class ClassService
    {
        public IBaseRepository<Class> ClassRepository { get; }

        public ClassService(SchoolScheduleContext context)
        {
            ClassRepository = new ClassRepository(context);
        }

    }
}
