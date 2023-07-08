using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.DB.Repositories;
using System.Collections.ObjectModel;

namespace SchoolSchedule.Model.Extensions
{
    internal static class ClassExtension
    {
        private static readonly SchoolScheduleContext _context;
        private static readonly IBaseRepository<Class> _classRepository;

        static ClassExtension()
        {
            _context = new();
            _classRepository = new ClassRepository(_context);
        }

        public static async void Create(this ObservableCollection<Class> classes, Class newClass)
        {
            await _classRepository.CreateAsync(newClass);
        }

        public static async void RemoveSelectedClass(this ObservableCollection<Class> classes, Class selectedClass)
        {
            await _classRepository.RemoveAsync(selectedClass);
        }
    }
}
