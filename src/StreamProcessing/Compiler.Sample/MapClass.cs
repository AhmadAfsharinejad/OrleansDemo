﻿using System;
using System.Collections.Generic;

namespace Compiler.Sample;

public  class MapClass
{
    public static Dictionary<string, object> Map(Dictionary<string, object> input)
    {
        input["a"] = DateTime.Now;
        input["b"] += "RND";
        return input;
    }
}