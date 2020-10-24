using System;
using System.Linq;
using Assembly_Lib;
using Xunit;
using Microsoft.Win32;

namespace TestProject
{
    public class AssemblyLibTests
    {
        private const string PATH = "E:\\University\\СПП\\lab-3\\Tracer.dll";
        private const string INCORRECT_PATH = "something";
        private const string INCORRECT_FILE_PATH = "E:\\University\\СПП\\lab-3\\WpfApp\\obj\\Debug\netcoreapp3.1\\WpfApp.dll";
        
        [Fact]
        public void ShouldReturnNullForInvalidPath()
        {
            Assert.Null(AssemblyLib.GetAssemblyInfo(INCORRECT_PATH));
        }
        
        [Fact]
        public void ShouldReturnNullForInvalidFile()
        {
            Assert.Null(AssemblyLib.GetAssemblyInfo(INCORRECT_FILE_PATH));
        }
        
        [Fact]
        public void NumberOfNamespacesShouldBeCorrect()
        {
            Assert.Equal(1, AssemblyLib.GetAssemblyInfo(PATH)?.NamespaceInfos?.Count);
        }
        
        [Fact]
        public void NumberOfMethodsShouldBeCorrect()
        {
            Assert.Equal(3, AssemblyLib.GetAssemblyInfo(PATH)?.NamespaceInfos.Values.First().DataTypeInfos.First().MethodInfos.Length);
        }
        
        [Fact]
        public void NumberOfFieldsShouldBeCorrect()
        {
            Assert.Equal(0, AssemblyLib.GetAssemblyInfo(PATH)?.NamespaceInfos.Values.First().DataTypeInfos.First().FieldInfos.Length);
        }
        
        [Fact]
        public void NumberOfPropertiesShouldBeCorrect()
        {
            Assert.Equal(0, AssemblyLib.GetAssemblyInfo(PATH)?.NamespaceInfos?.Values.First().DataTypeInfos.First().PropertyInfos.Length);
        }
        
    }
}