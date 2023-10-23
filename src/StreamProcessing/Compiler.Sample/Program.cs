// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

Console.WriteLine("Starting");


var code = File.ReadAllText("Code.txt");

var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);

var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString())
    .WithOptions(compilationOptions)
    .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(DateTime).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Dictionary<,>).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(typeof(DynamicObject).GetTypeInfo().Assembly.Location))
    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
    
using var memoryStream = new MemoryStream();
var compile = compilation.Emit(memoryStream);
var ss = compile.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => x.GetMessage()).ToList();
if (!compile.Success) throw new Exception("Compile Error.");
memoryStream.Seek(0, SeekOrigin.Begin);
var assembly = Assembly.Load(memoryStream.ToArray());

var type = assembly.GetType("Compiler.Sample.MapClass");
if (type is null) throw new Exception("Can't Find 'MapClass'.");
var methodInfo = type.GetMethod("Map", BindingFlags.Public | BindingFlags.Static);
if (methodInfo is null) throw new Exception("Can't Find 'Map' function.");
var method = (Func<Dictionary<string, object>, Dictionary<string, object>>)
    methodInfo.CreateDelegate(typeof(Func<Dictionary<string, object>, Dictionary<string, object>>));



var input = new Dictionary<string, object> { { "a", 1 }, { "b", "bb" } };
var result = method(input);

Console.WriteLine(result);






