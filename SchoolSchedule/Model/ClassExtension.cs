using System.Collections.ObjectModel;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model
{
    internal static class ClassExtension
    {
        private static readonly SchoolScheduleContext _context = new();
        public static async void Create(this ObservableCollection<Class> classes, Class newClass)
        {
            IBaseRepository <Class> classRepository = new BaseRepository<Class>(_context);
            await classRepository.CreateAsync(newClass);
        }

        public static async void Remove(this ObservableCollection<Class> classes, Class selectedClass)
        {
            IBaseRepository<Class> classRepository = new BaseRepository<Class>(_context);
            await classRepository.RemoveAsync(selectedClass);
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
