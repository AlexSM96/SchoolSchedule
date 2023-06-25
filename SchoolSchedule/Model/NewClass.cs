using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;

namespace SchoolSchedule.Model
{
    internal static class NewClass
    {
        private static SchoolScheduleContext _context;
        static NewClass()
        {
            _context = new SchoolScheduleContext();
        }

        public static void Add(string className)
        {
            var newClass = _context.Classes.Add(new Class
            {
                ClassName = className
            });
            _context.SaveChanges();
        }

        public static void Remove(int classId)
        {
            var newClass = _context.Classes.Remove(new Class
            {
                ClassId = classId
            });
            _context.SaveChanges();
        }
    }
}
