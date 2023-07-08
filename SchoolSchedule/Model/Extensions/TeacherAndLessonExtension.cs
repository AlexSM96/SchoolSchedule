using System.Collections.ObjectModel;
using System.Linq;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.DB.Interfaces;
using SchoolSchedule.DB.Repositories;
using SchoolSchedule.DB.Repositories.Base;

namespace SchoolSchedule.Model.Extensions
{
    internal static class TeacherAndLessonExtension
    {
        private static readonly SchoolScheduleContext _context;
        private static readonly IBaseRepository<TeacherAndLesson> _teacherAndLessonRepository;
        private static readonly IBaseRepository<Teacher> _teacherRepository;
        private static readonly IBaseRepository<Lesson> _lessonRepository;

        static TeacherAndLessonExtension()
        {
            _context = new SchoolScheduleContext();
            _teacherAndLessonRepository = new TeacherAndLeesonRepository(_context);
            _teacherRepository = new TeacherRepository(_context);
            _lessonRepository = new LessonRepository(_context);
        }

        public static async void Create
            (this ObservableCollection<TeacherAndLesson> teacherAndLessons, Teacher teacher, Lesson lesson)
        {
            var newTeacher = await _teacherRepository.CreateAsync(teacher);
            var isExistLesson = _lessonRepository.GetAll().Contains(lesson);
            if (isExistLesson)
            {
                await _teacherAndLessonRepository.CreateAsync(new TeacherAndLesson()
                {
                    Teacher = newTeacher,
                    LessonName = lesson.LessonName,
                });
            }
            else
            {
                var newLesson = await _lessonRepository.CreateAsync(lesson);
                await _teacherAndLessonRepository.CreateAsync(new TeacherAndLesson()
                {
                    Teacher = newTeacher,
                    LessonName = newLesson.LessonName,
                });
            }

        }

        public static async void RemoveTeacher(this ObservableCollection<TeacherAndLesson> teacherAndLessons, TeacherAndLesson teacherAndLesson)
        {
            await _teacherAndLessonRepository.RemoveAsync(teacherAndLesson);
        }

    }
}
