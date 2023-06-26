using Microsoft.IdentityModel.Tokens;
using SchoolSchedule.Commands;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.Model;
using SchoolSchedule.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SchoolSchedule.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private MainContext _context;

        public ObservableCollection<Class> Classes { get; }

        public ObservableCollection<TeacherAndLesson> TeachersAndLessons { get; }

        public ObservableCollection<object> RequestA { get; }

        public ObservableCollection<object> RequestB { get; set; }

        public ObservableCollection<object> RequestC { get; }

        public ObservableCollection<object> RequestD { get; }

        public ObservableCollection<WeekDay> WeekDay { get; set; }

        #region Title

        private string _title = "Расписание";

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        #endregion

        #region SelectedClass
        private Class _selectedClass = new();
        public Class SelectedClass
        {
            get { return _selectedClass; }
            set { Set(ref _selectedClass, value); }
        }
        #endregion

        #region TecherID and Name

        private string _teacherName = "";

        public string TeacherName
        {
            get { return _teacherName; }
            set { Set(ref _teacherName, value); }
        }

        private byte _teacherId = default;

        public byte TeacherId
        {
            get { return _teacherId; }
            set { Set(ref _teacherId, value); }
        }

        #endregion

        #region Lesson and ClassRoom
        private string _lessonName = "";

        public string LessonName
        {
            get { return _lessonName; }
            set { Set(ref _lessonName, value); }
        }

        private string _classRoom = "";

        public string ClassRoom
        {
            get { return _classRoom; }
            set { Set(ref _classRoom, value); }
        }

        #endregion

        #region ClassID and Name
        private int _classId;

        public int ClassId
        {
            get { return _classId; }
            set { Set(ref _classId, value); }
        }

        private string _className = "";

        public string ClassName
        {
            get { return _className; }
            set { Set(ref _className, value); }
        }

        #endregion

        #region SelectedWeekDay

        private WeekDay _selectedDay = new();

        public WeekDay SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                Set(ref _selectedDay, value);
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            _context = new();

            #region Commands
            AddNewTeacherAndLessons = new RelayCommand
                (OnAddNewTeacherAndLessonsExecuted, CanAddNewTeacherAndLessonsExecute);
            RemoveTeacherAndLessons = new RelayCommand
                (OnRemoveTeacherAndLessonsExecuted, CanRemoveTeacherAndLessonsExecute);
            AddNewClass = new RelayCommand(OnAddNewClassExecuted, CanAddNewClassExecute);
            RemoveClass = new RelayCommand(OnRemoveClassExecuted, CanRemoveClassExecute);
            FindClassRoom = new RelayCommand(OnFindClassRoomExecuted, CanFindClassRoomExecute);
            #endregion

            Classes = new ObservableCollection<Class>(_context.GetTableClasses());
            SelectedClass.Schedules = _context.GetTableSchedule();
            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>(_context
                .GetTableTeacherAndLessons());

            var requestA = (from teacher in TeachersAndLessons
                            group teacher.LessonName by teacher.Teacher.FullName into x
                            select new { Teacher = x.Key, LessonsCount = x.Count() })
                            .OrderByDescending(x => x.LessonsCount)
                            .Take(1);

            RequestA = new ObservableCollection<object>(requestA);
            WeekDay = new ObservableCollection<WeekDay>(_context.GetTableWeekDay());

            var requestC = from schedule in _context.GetTableSchedule()
                           group schedule by schedule.Class.ClassName into x
                           select new
                           {
                               Class = x.Key,
                               LessonCount = x.Select(x=>x.LessonName).Count(),
                               UniqTeachersCount = x.Select(x=>x.TeacherId).Distinct().Count(),
                           };
            RequestC = new ObservableCollection<object>(requestC);
        }

        #region AddNewTeacherAndLessonsCommand
        public ICommand AddNewTeacherAndLessons { get; }

        private bool CanAddNewTeacherAndLessonsExecute(object parameter)
            => _teacherName.IsNullOrEmpty() || _lessonName.IsNullOrEmpty()
            || _classRoom.IsNullOrEmpty() ? false : true;

        private void OnAddNewTeacherAndLessonsExecuted(object parameter)
        {
            if (TeacherName.IsNullOrEmpty()) return;
            NewTeacherAndLesson.Add(_teacherId, _teacherName, _lessonName, _classRoom);
        }

        #endregion

        #region RemoveTeacherAndLessonsCommand
        public ICommand RemoveTeacherAndLessons { get; }

        private bool CanRemoveTeacherAndLessonsExecute(object parameter)
            => _teacherId <= 0 && _lessonName == string.Empty ? false : true;

        private void OnRemoveTeacherAndLessonsExecuted(object parameter)
        {
            if (_lessonName.IsNullOrEmpty())
            {
                NewTeacherAndLesson.RemoveTeacher(_teacherId);
            }
            if (_teacherId == 0)
            {
                NewTeacherAndLesson.RemoveLesson(_lessonName);
            }
            if (_teacherId >= 0 && !_lessonName.IsNullOrEmpty())
            {
                NewTeacherAndLesson.Remove(_teacherId, _lessonName);
            }
        }
        #endregion

        #region AddNewClassCommand
        public ICommand AddNewClass { get; }

        private bool CanAddNewClassExecute(object parameter)
            => _className.IsNullOrEmpty() ? false : true;

        private void OnAddNewClassExecuted(object parameter)
        {
            if (!_className.IsNullOrEmpty())
            {
                NewClass.Add(_className);
            }
        }
        #endregion

        #region RemoveClassCommand
        public ICommand RemoveClass { get; }
        private bool CanRemoveClassExecute(object parameter) => _classId <= 0 ? false : true;

        private void OnRemoveClassExecuted(object parameter)
        {
            NewClass.Remove(_classId);
        }
        #endregion

        #region FindClassRoomCommand
        public ICommand FindClassRoom { get; }
        private bool CanFindClassRoomExecute(object parameter) => true;

        private void OnFindClassRoomExecuted(object parameter)
        {
            if (SelectedDay == null) return;
            var audience = (from schedule in _context.GetTableSchedule()
                            where schedule.WeekDay == SelectedDay.WeekDay1
                            group schedule by schedule.TeacherAndLesson.LessonNameNavigation.ClassRoom
                            into x
                            select new { ClassRoom = x.Key, CountTake = x.Count() })
                           .OrderBy(x => x.CountTake)
                           .ThenBy(x => x.ClassRoom)
                           .Take(1);
            RequestB = new ObservableCollection<object>(audience);
            OnPropertyChanged(nameof(RequestB));
        }

        #endregion
    }
}
