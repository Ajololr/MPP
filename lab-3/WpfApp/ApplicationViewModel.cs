using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assembly_Lib;
using Microsoft.Win32;
using WpfApp.Annotations;

namespace WpfApp
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Dll _selectedDll;
        
        public ObservableCollection<Dll> Dlls { get; set; }
        public Dll SelectedDll
        {
            get { return _selectedDll; }
            set
            {
                _selectedDll = value;
                OnPropertyChanged("SelectedDll");
            }
        }
        
        public ApplicationViewModel()
        {
            Dlls = new ObservableCollection<Dll>();
        }
        
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                       {
                           OpenFileDialog openFileDialog = new OpenFileDialog();
                           openFileDialog.Filter = "Dll files(*.dll)|*.dll";
                           openFileDialog.ShowDialog();
                           string filename = openFileDialog.FileName;
                           if (filename == "") return;
                           AssemblyInfo assemblyInfo = AssemblyLib.GetAssemblyInfo(filename);
                           
                           Dll dll = new Dll(openFileDialog.FileName, openFileDialog.SafeFileName, assemblyInfo);
                           Dlls.Insert(0, dll);
                           SelectedDll = dll;
                       }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}