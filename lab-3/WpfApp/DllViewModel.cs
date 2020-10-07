using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp.Annotations;

namespace WpfApp
{
    public class DllViewModel : INotifyPropertyChanged
    {
        private Dll _dll;
        
        public DllViewModel(Dll dll)
        {
            _dll = dll;
        }
 
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}