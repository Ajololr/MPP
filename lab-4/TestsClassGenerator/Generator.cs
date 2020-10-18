using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsClassGenerator
{
    public class TestClassGenerator
    {
        public string GenerateTestClasses(string sourceCode)
        {
            string result = "";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            List<MethodDeclarationSyntax> methods = syntaxTree.GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>().ToList().Where(method => method.Modifiers.ToList().Any(token => token.Text == "public")).ToList();
            foreach(var method in methods)
            { 
                Console.WriteLine(method.Identifier);
            }
            return result;
        }
    }
}