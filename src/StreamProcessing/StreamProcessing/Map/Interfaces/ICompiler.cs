namespace StreamProcessing.Map.Interfaces;

internal interface ICompiler
{
    Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> CreateFunction(string code, string classFullName, string functionName);
}