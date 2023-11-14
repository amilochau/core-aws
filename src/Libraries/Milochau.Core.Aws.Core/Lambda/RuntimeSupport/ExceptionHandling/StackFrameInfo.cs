using System.Diagnostics;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal class StackFrameInfo
    {
        public StackFrameInfo(StackFrame stackFrame)
        {
            Path = stackFrame.GetFileName();
            Line = stackFrame.GetFileLineNumber();
        }

        public string? Path { get; }
        public int Line { get; }
    }
}
