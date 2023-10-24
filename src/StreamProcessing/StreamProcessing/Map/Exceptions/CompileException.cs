namespace StreamProcessing.Map.Exceptions;

public class CompileException : Exception
{
    public CompileException()
    {
    }

    public CompileException(string message)
        : base(message)
    {
    }

    public CompileException(string message, Exception inner)
        : base(message, inner)
    {
    }
}