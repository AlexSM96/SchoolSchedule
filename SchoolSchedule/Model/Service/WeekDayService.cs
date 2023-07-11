using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.Model.Repositories;

namespace SchoolSchedule.Model.Service
{
    internal class WeekDayService
    {
        public IBaseRepository<WeekDay> WeekDayRepository { get; }

        public WeekDayService(SchoolScheduleContext context)
        {
            WeekDayRepository = new WeekDayRepository(context);
        }
    }
}
