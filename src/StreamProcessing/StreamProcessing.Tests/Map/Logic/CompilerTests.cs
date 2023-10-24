using System;
using System.Collections.Generic;
using FluentAssertions;
using StreamProcessing.Map.Exceptions;
using StreamProcessing.Map.Interfaces;
using StreamProcessing.Map.Logic;
using Xunit;

namespace StreamProcessing.Tests.Map.Logic;

public class CompilerTests
{
    private readonly ICompiler _sut;

    public CompilerTests()
    {
        _sut = new Compiler();
    }

    [Fact]
    public void CreateFunction_ShouldReturnExpected_WhenOk()
    {
        //Arrange
        var code = @"using System;
using System.Collections.Generic;
namespace Compiler.Sample;
public class MapClass
{
    public static IReadOnlyDictionary<string, object> Map(IReadOnlyDictionary<string, object> input)
    {
        var output = new Dictionary<string, object>(input);
        output[""a""] += ""RND"";
        return output;
    }
}";
        var fullClassName = "Compiler.Sample.MapClass";
        var functionName = "Map";
        
        var input = new Dictionary<string, object>
        {
            {"a",  "1"}
        };
        var expected = new Dictionary<string, object>
        {
            {"a",  "1RND"}
        };
        
        //Act
        var func = _sut.CreateFunction(code, fullClassName, functionName);
        var actual = func(input);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void CreateFunction_ShouldThrowException_WhenClassNameIsInvalid()
    {
        //Arrange
        var code = @"using System;
using System.Collections.Generic;
namespace Compiler.Sample;
public class MapClass
{
    public static IReadOnlyDictionary<string, object> Map(IReadOnlyDictionary<string, object> input)
    {
        var output = new Dictionary<string, object>(input);
        output[""a""] += ""RND"";
        return output;
    }
}";
        var fullClassName = "Sample.MapClass";
        var functionName = "Map";

        //Act
        var act = () => _sut.CreateFunction(code, fullClassName, functionName);

        //Assert
        act.Should().Throw<InvalidClassException>();
    }
    
    [Fact]
    public void CreateFunction_ShouldThrowException_WhenFunctionNameIsInvalid()
    {
        //Arrange
        var code = @"using System;
using System.Collections.Generic;
namespace Compiler.Sample;
public class MapClass
{
    public static IReadOnlyDictionary<string, object> Map(IReadOnlyDictionary<string, object> input)
    {
        var output = new Dictionary<string, object>(input);
        output[""a""] += ""RND"";
        return output;
    }
}";
        var fullClassName = "Compiler.Sample.MapClass";
        var functionName = "2Map2";
        
        //Act
        var act = () => _sut.CreateFunction(code, fullClassName, functionName);

        //Assert
        act.Should().Throw<MissingMethodException>();
    }
    
    [Fact]
    public void CreateFunction_ShouldThrowException_WhenCompileIsInvalid()
    {
        //Arrange
        var code = @"using System;
using System.Collections.Generic;
namespace Compiler.Sample;
public class MapClass
{
    public static IReadOnlyDictionary<string, object> Map(IReadOnlyDictionary<string, object> input)
    {
        Invalid syntax
        var output = new Dictionary<string, object>(input);
        output[""a""] += ""RND"";
        return output;
    }
}";
        var fullClassName = "Compiler.Sample.MapClass";
        var functionName = "Map2";
      
        //Act
        var act = () => _sut.CreateFunction(code, fullClassName, functionName);

        //Assert
        act.Should().Throw<CompileException>();
    }
}