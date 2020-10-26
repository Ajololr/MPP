using Xunit;
using System.Collections.Generic;
using System.IO;
using TestsClassGenerator;
using TestClass = TestsClassGenerator.TestClass;

namespace TestProject
{
    public class UnitTest1
    {
        private TestClassGenerator _generator = new TestClassGenerator();
        private static string _emptyClassInput = "E:\\University\\СПП\\lab-1\\ClassLibrary1\\Class1.cs";
        private static string[] _emptyClassesInput = { 
            "E:\\University\\СПП\\lab-1\\ClassLibrary1\\Class1.cs", 
        };
        private static string _correctPath = "E:\\University\\СПП\\lab-2\\Faker\\Faker.cs";
        private static string[] _correctClassesInput = { 
            "E:\\University\\СПП\\lab-2\\Faker\\Faker.cs", 
            "E:\\University\\СПП\\lab-1\\Tracer\\TracerImpl.cs"
        };
        
        [Fact]
        public void ShouldReturnEmptyListForClassWithNoMethods()
        {
            using (StreamReader SourceReader = File.OpenText(_emptyClassInput))
            {
                List<TestClass> result = _generator.GenerateTestClasses(SourceReader.ReadToEnd());
                Assert.Empty(result);
            }
        }
        
        [Fact]
        public void ShouldReturnEmptyListForSeveralClassesWithNoMethods()
        {
            foreach (var path in _emptyClassesInput)
            {
                using (StreamReader SourceReader = File.OpenText(path))
                {
                    List<TestClass> result = _generator.GenerateTestClasses(SourceReader.ReadToEnd());
                    Assert.Empty(result);
                }
            }
        }
        
        [Fact]
        public void ShouldReturnCorrectListForSeveralClasses()
        {
            int result = 0;
            foreach (var path in _correctClassesInput)
            {
                using (StreamReader SourceReader = File.OpenText(path))
                {
                    result += _generator.GenerateTestClasses(SourceReader.ReadToEnd()).Count;
                }
            }
            Assert.Equal(2, result);
        }
        
        [Fact]
        public void ShouldReturnTestClassDataForCorrectInput()
        {
            using (StreamReader SourceReader = File.OpenText(_correctPath))
            {
                Assert.NotEmpty(_generator.GenerateTestClasses(SourceReader.ReadToEnd()));
            }
        }
    }
}