using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;

namespace SchoolSchedule.Model
{
    internal static class NewTeacherAndLesson
    {
        private static SchoolScheduleContext _context;
        static NewTeacherAndLesson()
        {
            _context = new SchoolScheduleContext();
        }

        public static void  Add
            (byte teacherId, string teacherName, string lessonName, string classRoom)
        {
            var newTeacher = _context.Teachers.Add(new Teacher
            {
                FullName = teacherName
            });
            var newLesson = _context.Lessons.Add(new Lesson
            {
                LessonName = lessonName,
                ClassRoom = classRoom
            });
            var newTeacherAndLessons = _context.TeacherAndLessons.Add(new TeacherAndLesson
            {
                Teacher = newTeacher.Entity,
                LessonName = lessonName
            });
            _context.SaveChanges();
        }

        public static void Remove(byte teacherId, string lessonName)
        {
            var teacherAndLessons = _context.TeacherAndLessons.Remove(new TeacherAndLesson
            {
                TeacherId = teacherId,
                LessonName = lessonName,
            });
            _context.SaveChanges();
        }

        public static void RemoveTeacher(byte teacherId)
        {
            var newTeacher = _context.Teachers.Remove(new Teacher
            {
                Id = teacherId
            });
            _context.SaveChanges();
        }

        public static void RemoveLesson(string lessonName)
        {
            var newLesson = _context.Lessons.Remove(new Lesson
            {
                LessonName = lessonName
            });
            _context.SaveChanges();
        }
    }
}
