using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SchoolSchedule.ViewModels.Base
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertName = null)
        {
            if(field.Equals(value)) return false;
            field = value;
            OnPropertyChanged(propertName); 
            return true;
        }
    }
}
