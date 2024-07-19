using System.Diagnostics;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal class StackFrameInfo(StackFrame stackFrame)
    {
        public string? Path { get; } = stackFrame.GetFileName();
        public int Line { get; } = stackFrame.GetFileLineNumber();
    }
}
