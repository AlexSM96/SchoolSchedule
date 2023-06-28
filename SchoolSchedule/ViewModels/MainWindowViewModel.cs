using Microsoft.IdentityModel.Tokens;
using SchoolSchedule.Commands;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.Model;
using SchoolSchedule.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SchoolSchedule.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private Table _table;

        public ObservableCollection<Class> Classes { get; set; }

        public ObservableCollection<TeacherAndLesson> TeachersAndLessons { get; }

        public ObservableCollection<object> RequestA { get; }

        public ObservableCollection<object> RequestB { get; set; }

        public ObservableCollection<object> RequestC { get; }

        public ObservableCollection<object> RequestD { get; set; }

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
        private readonly CollectionViewSource _selectedClassSchedule = new();
        public ICollectionView SelectedClassSchedule => _selectedClassSchedule?.View;

        private Class _selectedClass = new();
        public Class SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                if (!Set(ref _selectedClass, value)) return;
                _selectedClassSchedule.Source = value?.Schedules;
                OnPropertyChanged(nameof(SelectedClassSchedule));
            }
        }
        #endregion

        #region TecherId
        private byte _teacherId = default;

        public byte TeacherId
        {
            get { return _teacherId; }
            set { Set(ref _teacherId, value); }
        }
        #endregion

        #region TeacherName
        private string _teacherName = "";

        public string TeacherName
        {
            get { return _teacherName; }
            set { Set(ref _teacherName, value); }
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
            set
            {
                Set(ref _className, value);
            }
        }

        #endregion

        #region SelectedWeekDay

        public ObservableCollection<WeekDay> WeekDays { get; set; } = new();

        private WeekDay _selectedDay = new();

        public WeekDay SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                if (!Set(ref _selectedDay, value)) return;
                WeekDays.Add(value);
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            _table = new();

            #region Commands
            AddNewTeacherAndLessons = new RelayCommand
                (OnAddNewTeacherAndLessonsExecuted, CanAddNewTeacherAndLessonsExecute);

            RemoveTeacherAndLessons = new RelayCommand
                (OnRemoveTeacherAndLessonsExecuted, CanRemoveTeacherAndLessonsExecute);

            AddNewClass = new RelayCommand
                (OnAddNewClassExecuted, CanAddNewClassExecute);

            RemoveClass = new RelayCommand
                (OnRemoveClassExecuted, CanRemoveClassExecute);

            FindClassRoom = new RelayCommand
                (OnFindClassRoomExecuted, CanFindClassRoomExecute);

            FindClassTeacher = new RelayCommand
                (OnFindClassTeacherExecuted, CanFindClassTeacherExecute);

            SaveSchedule = new RelayCommand(OnSaveScheduleExecuted, CanSaveScheduleExecute);

            #endregion

            Classes = new ObservableCollection<Class>(_table.GetTableClasses());
            SelectedClass.Schedules = _table.GetTableSchedule();
            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>(_table
                .GetTableTeacherAndLessons());
            WeekDay = new ObservableCollection<WeekDay>(_table.GetTableWeekDay());
            RequestA = new ObservableCollection<object>(GetDataA());
            RequestC = new ObservableCollection<object>(GetDataC());
        }

        private IEnumerable<RequestA> GetDataA()
        {
            var requestA = (from teacher in TeachersAndLessons
                            group teacher.LessonName by teacher.Teacher.FullName into x
                            select new { Teacher = x.Key, LessonsCount = x.Count() })
                            .OrderByDescending(x => x.LessonsCount)
                            .Take(1);
            var result = requestA.Select(x => new RequestA
            {
                Teacher = x.Teacher,
                LessonsCount = x.LessonsCount
            });
            return result;
        }

        private IEnumerable<RequestB> GetDataB()
        {
            var audience = (from schedule in _table.GetTableSchedule()
                            where WeekDays.Contains(schedule.WeekDayNavigation)
                            group schedule by schedule.TeacherAndLesson.LessonNameNavigation.ClassRoom
                            into x
                            select new { ClassRoom = x.Key, CountTake = x.Count() })
                           .OrderBy(x => x.CountTake).ThenBy(x => x.ClassRoom)
                           .Take(1);
            var result = audience.Select(x => new RequestB
            {
                ClassRoom = x.ClassRoom,
                CountTake = x.CountTake
            });
            return result;
        }

        private IEnumerable<RequestC> GetDataC()
        {
            var requestC = from schedule in _table.GetTableSchedule()
                           group schedule by schedule.Class.ClassName into x
                           select new
                           {
                               Class = x.Key,
                               LessonCount = x.Select(x => x.LessonName).Count(),
                               UniqTeachersCount = x.Select(x => x.TeacherId).Distinct().Count(),
                           };
            var result = requestC.Select(x => new RequestC
            {
                Class = x.Class,
                LessonCount = x.LessonCount,
                UniqTeachersCount = x.UniqTeachersCount
            });
            return result;
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
                Classes = new ObservableCollection<Class>(_table.GetTableClasses());
                OnPropertyChanged(nameof(Classes));
            }
        }
        #endregion

        #region RemoveClassCommand
        public ICommand RemoveClass { get; }
        private bool CanRemoveClassExecute(object parameter) => _classId <= 0 ? false : true;

        private void OnRemoveClassExecuted(object parameter)
        {
            NewClass.Remove(_classId);
            Classes = new ObservableCollection<Class>(_table.GetTableClasses());
            OnPropertyChanged(nameof(Classes));
        }
        #endregion

        #region FindClassRoomCommand
        public ICommand FindClassRoom { get; }
        private bool CanFindClassRoomExecute(object parameter) => true;

        private void OnFindClassRoomExecuted(object parameter)
        {
            if (WeekDays.Count() == 0) return;
            RequestB = new ObservableCollection<object>(GetDataB());
            OnPropertyChanged(nameof(RequestB));
            WeekDays.Clear();
        }

        #endregion

        #region FindClassTeacherCommand

        public ICommand FindClassTeacher { get; }
        private bool CanFindClassTeacherExecute(object parameter) => true;

        private void OnFindClassTeacherExecuted(object parameter)
        {
            if (ClassName.IsNullOrEmpty()) return;
            var dataD = _table.GetDataD(ClassName)
                .GroupBy(x => x.LessonName);
            RequestD = new ObservableCollection<object>(dataD);
            OnPropertyChanged(nameof(RequestD));
        }
        #endregion

        #region SaveScheduleCommand
        public ICommand SaveSchedule { get; }

        private bool CanSaveScheduleExecute(object parameter) => true;

        private void OnSaveScheduleExecuted(object parameter)
        {
            if (parameter is not DataGrid grid) return;
            try
            {
                var currentItems = grid.Items.CurrentItem;
                if (currentItems is not Schedule schedule) return;
                var collection = new ObservableCollection<Schedule> { schedule };

                foreach (var item in collection)
                {
                    NewSchedule.Add(item.WeekDay, item.ClassId,
                        item.LessonNumber, item.TeacherId, item.LessonName);
                }
                OnPropertyChanged(nameof(collection));
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception?.InnerException?.Message);
            }
        }
        #endregion
    }
}
