using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assembly_Lib;
using WpfApp.Annotations;

namespace WpfApp
{
    public class Dll : INotifyPropertyChanged
    {
        private string _path;
        private string _dllName;
        private AssemblyInfo _assemblyInfo;
        
        public string Path
        {
            get { return _path; }
            
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
            
        }

        public string Name
        {
            get { return _dllName; }
            
            set
            {
                _dllName = value;
                OnPropertyChanged("Name");
            }
        } 
        
        public Node AssemblyInfo 
        { 
            get 
            {
                return AssemblyLib.BuildTree(_assemblyInfo);
            } 
        }
        
        public Dll(string path, string name, AssemblyInfo assemblyInfo)
        {
            _path = path;
            _dllName = name;
            _assemblyInfo = assemblyInfo;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}