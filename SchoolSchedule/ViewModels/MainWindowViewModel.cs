using Microsoft.IdentityModel.Tokens;
using SchoolSchedule.Commands;
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
        private readonly Table _tables;

        public ObservableCollection<Class> Classes { get; set; }

        public ObservableCollection<TeacherAndLesson> TeachersAndLessons { get; }

        public ObservableCollection<WeekDay> WeekDay { get; set; }

        public ObservableCollection<RequestA> RequestA { get; }

        public ObservableCollection<RequestB> RequestB { get; set; }

        public ObservableCollection<RequestC> RequestC { get; }

        public ObservableCollection<object> RequestD { get; set; }

        public List<Schedule> Schedules { get; set; }

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
            _tables = new();

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

            SaveSchedule = new RelayCommand
                (OnSaveScheduleExecuted, CanSaveScheduleExecute);

            DeleteSchedule = new RelayCommand
                (OnDeleteScheduleExecuted, CanDeleteScheduleExecute);

            #endregion

            Classes = new ObservableCollection<Class>(_tables.GetTableClasses());
            SelectedClass.Schedules = _tables.GetTableSchedule();
            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>(_tables
                .GetTableTeacherAndLessons());
            WeekDay = new ObservableCollection<WeekDay>(_tables.GetTableWeekDay());
            RequestA = new ObservableCollection<RequestA>(GetDataA());
            RequestC = new ObservableCollection<RequestC>(GetDataC());
            Schedules = new List<Schedule>(_tables.GetTableSchedule());

        }

        private IEnumerable<RequestA> GetDataA()
        {
            var request = TeachersAndLessons
                .GroupBy(x => x.Teacher.FullName)
                .Select(x => new RequestA { Teacher = x.Key, LessonsCount = x.Count() })
                .OrderByDescending(x => x.LessonsCount)
                .Take(1);
            return request;
        }

        private IEnumerable<RequestB> GetDataB()
        {
            var audience = _tables
                .GetTableSchedule()
                .Where(x => WeekDays.Contains(x.WeekDayNavigation))
                .GroupBy(x => x.TeacherAndLesson.LessonNameNavigation.ClassRoom)
                .Select(x => new RequestB { ClassRoom = x.Key, CountTake = x.Count() })
                .OrderBy(x => x.CountTake)
                .ThenBy(x => x.ClassRoom)
                .Take(1);
            return audience;
        }

        private IEnumerable<RequestC> GetDataC()
        {
            var request = _tables.GetTableSchedule()
                .GroupBy(x => x.Class.ClassName)
                .Select(x => new RequestC
                {
                    Class = x.Key,
                    LessonCount = x.Select(x => x.LessonName).Count(),
                    UniqTeachersCount = x.Select(x => x.TeacherId)
                        .Distinct()
                        .Count()
                });

            return request;
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
            => _teacherId > 0 || _lessonName != string.Empty;

        private void OnRemoveTeacherAndLessonsExecuted(object parameter)
        {
            if (_lessonName.IsNullOrEmpty())
            {
                NewTeacherAndLesson.RemoveTeacher(_teacherId);
            }
            if (_teacherId <= 0)
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
                ClassExtension.Add(_className);
                Classes = new ObservableCollection<Class>(_tables.GetTableClasses());
                OnPropertyChanged(nameof(Classes));
            }
        }
        #endregion

        #region RemoveClassCommand
        public ICommand RemoveClass { get; }
        private bool CanRemoveClassExecute(object parameter) => _classId > 0;

        private void OnRemoveClassExecuted(object parameter)
        {
            ClassExtension.Remove(_classId);
            Classes = new ObservableCollection<Class>(_tables.GetTableClasses());
            OnPropertyChanged(nameof(Classes));
        }
        #endregion

        #region FindClassRoomCommand
        public ICommand FindClassRoom { get; }
        private bool CanFindClassRoomExecute(object parameter) => true;

        private void OnFindClassRoomExecuted(object parameter)
        {
            if (!WeekDays.Any()) return;
            RequestB = new ObservableCollection<RequestB>(GetDataB());
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
            var dataD = _tables.GetDataD(ClassName)
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
            try
            {
                foreach (Schedule item in SelectedClass.Schedules.ToList())
                {
                    if (!_tables.GetTableSchedule().Contains(item))
                    {
                        var newSchedule = _tables.GetTableSchedule();
                        newSchedule.AddSchedule(item);
                    }
                }
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
        #endregion

        #region DeletescheduleCommand

        public ICommand DeleteSchedule { get; }
        private bool CanDeleteScheduleExecute(object parameter) => true;

        private void OnDeleteScheduleExecuted(object parameter)
        {
            if (parameter is not DataGrid grid) return;
            try
            {
                var scheduleRows = grid.SelectedItem;
                if (scheduleRows is not Schedule row) return;
                if (SelectedClass.Schedules.Contains(row))
                {
                    _tables.GetTableSchedule().RemoveSchedule(row);
                    _selectedClassSchedule.View.Refresh();
                }
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        #endregion
    }
}
