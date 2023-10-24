using System;
using System.Collections.Generic;

namespace Compiler.Sample;

public class MapClass
{
    public static IReadOnlyDictionary<string, object> Map(IReadOnlyDictionary<string, object> input)
    {
        var output = new Dictionary<string, object>(input);
        output["a"] = DateTime.Now;
        output["b"] += "RND";
        return output;
    }
}