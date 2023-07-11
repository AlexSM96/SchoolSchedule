using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolSchedule.Commands;
using SchoolSchedule.DB.Database.Context;
using SchoolSchedule.DB.Database.Entities;
using SchoolSchedule.Model;
using SchoolSchedule.Model.Service;
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
        private SchoolScheduleContext _context;

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

        #region TeacherName
        private string _teacherName = string.Empty;

        public string TeacherName
        {
            get { return _teacherName; }
            set { Set(ref _teacherName, value); }
        }
        #endregion

        #region LessonName and ClassRoom
        private string _lessonName = string.Empty;

        public string LessonName
        {
            get { return _lessonName; }
            set { Set(ref _lessonName, value); }
        }

        private string _classRoom = string.Empty;

        public string ClassRoom
        {
            get { return _classRoom; }
            set { Set(ref _classRoom, value); }
        }

        #endregion

        #region ClassName
        private string _className = "";

        public string ClassName
        {
            get { return _className; }
            set { Set(ref _className, value); }
        }
        #endregion

        #region SelectedWeekDay

        public ObservableCollection<WeekDay> SelectedWeekDays { get; set; } = new();

        private WeekDay _selectedDay = new();
        private ScheduleService _scheduleService;
        private ClassService _classService;
        private WeekDayService _weekDayService;
        private TeacherAndLessonService _teacherAndLessonService;

        public WeekDay SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                if (!Set(ref _selectedDay, value)) return;
                SelectedWeekDays.Add(value);
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            InitRepositories();

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

            Classes = new ObservableCollection<Class>(_classService?.ClassRepository.GetAll());
            SelectedClass.Schedules = _scheduleService?.GetFullSchedule().ToList();
            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>(_teacherAndLessonService?.TeacherAndLessonRepository?.GetAll());
            WeekDay = new ObservableCollection<WeekDay>(_weekDayService?.WeekDayRepository?.GetAll());
            RequestA = new ObservableCollection<RequestA>(GetDataA());
            RequestC = new ObservableCollection<RequestC>(GetDataC());
            Schedules = new List<Schedule>(_scheduleService?.ScheduleRepository?.GetAll());
        }


        public ObservableCollection<Class> Classes { get; set; }

        public ObservableCollection<TeacherAndLesson> TeachersAndLessons { get; set; }

        public ObservableCollection<WeekDay> WeekDay { get; set; }

        public ObservableCollection<RequestA> RequestA { get; }

        public ObservableCollection<RequestB> RequestB { get; set; }

        public ObservableCollection<RequestC> RequestC { get; }

        public ObservableCollection<object> RequestD { get; set; }

        public List<Schedule> Schedules { get; set; }

        private void InitRepositories()
        {
            _context = new();
            _scheduleService = new ScheduleService(_context);
            _classService = new ClassService(_context);
            _weekDayService = new WeekDayService(_context);
            _teacherAndLessonService = new TeacherAndLessonService(_context);
        }

        private IEnumerable<RequestA> GetDataA()
        {
            var request = _teacherAndLessonService.TeacherAndLessonRepository.GetAll()
                .Include(x => x.Teacher)
                .GroupBy(x => x.Teacher.FullName)
                .Select(x => new RequestA { Teacher = x.Key, LessonsCount = x.Count() })
                .OrderByDescending(x => x.LessonsCount)
                .Take(1);
            return request;
        }

        private IEnumerable<RequestB> GetDataB()
        {
            var audience = _scheduleService.ScheduleRepository.GetAll()
                .Where(x => SelectedWeekDays.Contains(x.WeekDayNavigation))
                .GroupBy(x => x.TeacherAndLesson.LessonNameNavigation.ClassRoom)
                .Select(x => new RequestB { ClassRoom = x.Key, CountTake = x.Count() })
                .OrderBy(x => x.CountTake)
                .ThenBy(x => x.ClassRoom)
                .Take(1);
            return audience.ToList();
        }

        private IEnumerable<RequestC> GetDataC()
        {
            var request = _scheduleService.ScheduleRepository.GetAll()
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
            => !_teacherName.IsNullOrEmpty() && !_lessonName.IsNullOrEmpty()
                                             && !_classRoom.IsNullOrEmpty();

        private async void OnAddNewTeacherAndLessonsExecuted(object parameter)
        {
            await _teacherAndLessonService.TeacherAndLessonRepository
                .CreateAsync(new TeacherAndLesson
                {
                    Teacher = new Teacher { FullName = TeacherName },
                    LessonNameNavigation = new Lesson
                    {
                        LessonName = LessonName,
                        ClassRoom = ClassRoom
                    },
                    LessonName = LessonName
                });

            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>
                (_teacherAndLessonService.TeacherAndLessonRepository.GetAll());
            OnPropertyChanged(nameof(TeachersAndLessons));
        }
        #endregion

        #region RemoveTeacherAndLessonsCommand
        public ICommand RemoveTeacherAndLessons { get; }

        private bool CanRemoveTeacherAndLessonsExecute(object parameter)
            => parameter is DataGrid { SelectedItem: TeacherAndLesson };

        private async void OnRemoveTeacherAndLessonsExecuted(object parameter)
        {
            if (parameter is not DataGrid grid) return;
            var selectedItem = grid.SelectedItem;
            if (selectedItem is not TeacherAndLesson teacherAndLesson) return;
            await _teacherAndLessonService.TeacherAndLessonRepository.RemoveAsync(teacherAndLesson);
            TeachersAndLessons = new ObservableCollection<TeacherAndLesson>
                (_teacherAndLessonService.TeacherAndLessonRepository.GetAll());
            OnPropertyChanged(nameof(TeachersAndLessons));
        }
        #endregion

        #region AddNewClassCommand
        public ICommand AddNewClass { get; }

        private bool CanAddNewClassExecute(object parameter)
            => !_className.IsNullOrEmpty();

        private async void OnAddNewClassExecuted(object parameter)
        {
            var newClass = new Class { ClassName = _className };
            await _classService.ClassRepository.CreateAsync(newClass);
            Classes = new ObservableCollection<Class>(_classService.ClassRepository.GetAll());
            OnPropertyChanged(nameof(Classes));
        }
        #endregion

        #region RemoveClassCommand
        public ICommand RemoveClass { get; }
        private bool CanRemoveClassExecute(object parameter)
            => parameter is DataGrid { SelectedItem: Class };

        private async void OnRemoveClassExecuted(object parameter)
        {
            if (parameter is not DataGrid { SelectedItem: Class selectedClass }) return;
            await _classService.ClassRepository.RemoveAsync(selectedClass);
            Classes = new ObservableCollection<Class>(_classService.ClassRepository.GetAll());
            OnPropertyChanged(nameof(Classes));
        }
        #endregion

        #region FindClassRoomCommand
        public ICommand FindClassRoom { get; }

        private bool CanFindClassRoomExecute(object parameter) => true;

        private void OnFindClassRoomExecuted(object parameter)
        {
            if (!SelectedWeekDays.Any()) return;
            RequestB = new ObservableCollection<RequestB>(GetDataB());
            OnPropertyChanged(nameof(RequestB));
            SelectedWeekDays.Clear();
        }

        #endregion

        #region FindClassTeacherCommand

        public ICommand FindClassTeacher { get; }
        private bool CanFindClassTeacherExecute(object parameter) => parameter is TextBox;

        private void OnFindClassTeacherExecuted(object parameter)
        {
            if (parameter is not TextBox textBox) return;
            if (string.IsNullOrWhiteSpace(textBox.Text)) return;
            var dataD = _scheduleService.ScheduleRepository.GetAll()
                .Include(x => x.TeacherAndLesson.Teacher)
                .Where(x => x.Class.ClassName.Contains(textBox.Text))
                .GroupBy(x => x.LessonName);
            RequestD = new ObservableCollection<object>(dataD);
            OnPropertyChanged(nameof(RequestD));
        }
        #endregion

        #region SaveScheduleCommand
        public ICommand SaveSchedule { get; }

        private bool CanSaveScheduleExecute(object parameter) => true;

        private async void OnSaveScheduleExecuted(object parameter)
        {
            try
            {
                foreach (Schedule selectedRow in SelectedClass.Schedules.ToList())
                {
                    bool rowIsExist = _scheduleService.ScheduleRepository
                        .GetAll()
                        .Contains(selectedRow);
                    if (!rowIsExist)
                    {
                        await _scheduleService.ScheduleRepository.CreateAsync(selectedRow);
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
        private bool CanDeleteScheduleExecute(object parameter)
            => parameter is DataGrid { SelectedItem: Schedule };

        private async void OnDeleteScheduleExecuted(object parameter)
        {
            try
            {
                var selectedRow = parameter as Schedule;
                if (SelectedClass.Schedules.Contains(selectedRow))
                {
                    await _scheduleService.ScheduleRepository.RemoveAsync(selectedRow);
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
