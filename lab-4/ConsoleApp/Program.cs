using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using TestsClassGenerator;

class Dataflow
{
   private const string DestPath = "E:\\University\\СПП\\lab-4\\Output\\output.txt";
   
   static void Main()
   {
      var loadSourceFileToMemory = new TransformBlock<string, string>(async path =>
      {
         Console.WriteLine("Loading to memory '{0}'...", path);

         using (StreamReader SourceReader = File.OpenText(path))
         {
            return await SourceReader.ReadToEndAsync();
         }
      });

      var generateTestClass = new TransformBlock<string, string>(async text =>
      {
         Console.WriteLine("Generating test class...");
         
         TestClassGenerator classGenerator = new TestClassGenerator();
         classGenerator.GenerateTestClasses(text);
         
         return text;
      });

      var saveTestClassToFile = new ActionBlock<string>(async text =>
      {
         using (StreamWriter DestinationWriter = File.CreateText(DestPath))
         {
            Console.WriteLine("Saving class to file...");
            await DestinationWriter.WriteAsync(text);
         }
      });

      var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

      loadSourceFileToMemory.LinkTo(generateTestClass, linkOptions);
      generateTestClass.LinkTo(saveTestClassToFile, linkOptions);

      loadSourceFileToMemory.Post("E:\\University\\СПП\\lab-1\\Tracer\\TracerImpl.cs");

      loadSourceFileToMemory.Complete();

      saveTestClassToFile.Completion.Wait();
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
   
   public void FooBar()
   {
      
   }
   
   private void BarFoo()
   {
      
   }
}
