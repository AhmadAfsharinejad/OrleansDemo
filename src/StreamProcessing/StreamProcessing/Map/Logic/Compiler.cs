using System.Dynamic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StreamProcessing.Map.Exceptions;
using StreamProcessing.Map.Interfaces;

namespace StreamProcessing.Map.Logic;

internal sealed class Compiler : ICompiler
{
    public Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>
        CreateFunction(string code, string classFullName, string functionName)
    {
        var compilation = CreateCompile(code);

        var assembly = CreateAssembly(compilation);

        return CreateDelegate(classFullName, functionName, assembly);
    }

    private static Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>
        CreateDelegate(string classFullName, string functionName, Assembly assembly)
    {
        var type = assembly.GetType(classFullName);
        if (type is null) throw new InvalidClassException($"Can't Find '{classFullName}'.");

        var methodInfo = type.GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);
        if (methodInfo is null) throw new MissingMethodException($"Can't Find '{functionName}' function.");

        return (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)
            methodInfo.CreateDelegate(typeof(Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>));
    }

    private static Assembly CreateAssembly(Compilation compilation)
    {
        using var memoryStream = new MemoryStream();
        var compile = compilation.Emit(memoryStream);
        if (!compile.Success)
        {
            var errors = compile.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => x.GetMessage()).ToList();
            throw new CompileException($"Compile Error:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return Assembly.Load(memoryStream.ToArray());
    }

    private static CSharpCompilation CreateCompile(string code)
    {
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);

        return CSharpCompilation.Create(Guid.NewGuid().ToString())
            .WithOptions(compilationOptions)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Dictionary<,>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DateTime).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Path).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DynamicObject).GetTypeInfo().Assembly.Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
    }
}