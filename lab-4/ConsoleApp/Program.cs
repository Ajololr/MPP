using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks.Dataflow;
using TestsClassGenerator;

class Dataflow
{
   private const string DestPath = "E:\\University\\СПП\\lab-4\\Output\\";
   private const int MaxDegreeOfParallelismLoad = 1;
   private const int MaxDegreeOfParallelismGenerate = 1;
   private const int MaxDegreeOfParallelismSave = 1;
   private static string[] _input = new[] { 
      "E:\\University\\СПП\\lab-2\\Faker\\Faker.cs", 
      "E:\\University\\СПП\\lab-1\\Tracer\\TracerImpl.cs",
      "E:\\University\\СПП\\lab-1\\Tracer\\JsonSerializer.cs",
      "E:\\University\\СПП\\lab-1\\Tracer\\XmlSerializer.cs",
      "E:\\University\\СПП\\lab-1\\App\\Program.cs",
      "E:\\University\\СПП\\lab-3\\WpfApp\\ApplicationViewModel.cs",
      "E:\\University\\СПП\\lab-3\\Assembly Lib\\Class1.cs"
   };

   static void Main()
   {
      var sw = new Stopwatch();
      sw.Start();
      
      var loadSourceFileToMemory = new TransformBlock<string, string>(async path =>
      {
         Console.WriteLine("Loading to memory '{0}'...", path);

         using (StreamReader SourceReader = File.OpenText(path))
         {
            return await SourceReader.ReadToEndAsync();
         }
      }, new ExecutionDataflowBlockOptions
      {
         MaxDegreeOfParallelism = MaxDegreeOfParallelismLoad
      });

      var generateTestClass = new TransformManyBlock<string, TestClass>(async text =>
      {
         Console.WriteLine("Generating test class...");
         
         TestClassGenerator classGenerator = new TestClassGenerator();

         return classGenerator.GenerateTestClasses(text);
      }, new ExecutionDataflowBlockOptions
      {
         MaxDegreeOfParallelism = MaxDegreeOfParallelismGenerate
      });

      var saveTestClassToFile = new ActionBlock<TestClass>(async testClass =>
      {
         using (StreamWriter DestinationWriter = File.CreateText(DestPath + testClass.FileName))
         {
            Console.WriteLine("Saving '{0}' class to file...", testClass.FileName);
            await DestinationWriter.WriteAsync(testClass.Source);
            Console.WriteLine("Saved!");
         }
      }, new ExecutionDataflowBlockOptions
      {
         MaxDegreeOfParallelism = MaxDegreeOfParallelismSave
      });

      var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

      loadSourceFileToMemory.LinkTo(generateTestClass, linkOptions);
      generateTestClass.LinkTo(saveTestClassToFile, linkOptions);

      foreach (var path in _input)
      {
         loadSourceFileToMemory.Post(path);
      }

      loadSourceFileToMemory.Complete();

      saveTestClassToFile.Completion.Wait();
      sw.Stop();
      Console.WriteLine(sw.ElapsedMilliseconds);
   }

   public void Foo()
   {
      
   }
   
   private void Bar()
   {
      
   }
}

class Test
{
   
   public void Foo()
   {
      
   }
   
   private void Bar()
   {
      
   }
}
