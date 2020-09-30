using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assembly_Lib;
using Microsoft.Win32;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dll files(*.dll)|*.dll";
            openFileDialog.ShowDialog();
            string filename = openFileDialog.FileName;
            if (filename == "") return;
            AssemblyInfo assemblyInfo = AssemblyLib.GetAssemblyInfo(filename);
            Console.WriteLine(assemblyInfo);
        }
        
    }
}