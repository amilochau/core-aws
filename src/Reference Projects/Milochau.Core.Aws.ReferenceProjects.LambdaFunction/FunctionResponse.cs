using System;
using System.Collections;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class FunctionResponse
    {
        public IDictionary EnvironmentVariables { get; set; } = Environment.GetEnvironmentVariables();
    }
}
