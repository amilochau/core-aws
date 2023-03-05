using Milochau.Core.Aws.ReferenceProjects.LambdaFunction;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public class RuntimeApiClient
    {
        private readonly InternalRuntimeApiClient internalClient;
        private readonly IConsoleLoggerWriter consoleLoggerRedirector = new LogLevelLoggerWriter();

        public RuntimeApiClient(HttpClient httpClient)
        {
            internalClient = new InternalRuntimeApiClient(httpClient);
        }

        public async Task<InvocationRequest> GetNextInvocationAsync()
        {
            SwaggerResponse<Stream?> response = await internalClient.NextAsync();

            var headers = new RuntimeApiHeaders(response.Headers);
            consoleLoggerRedirector.SetCurrentAwsRequestId(headers.AwsRequestId);

            var lambdaContext = new LambdaContext(headers, consoleLoggerRedirector);
            return new InvocationRequest(response.Result, lambdaContext);
        }

        public Task ReportInvocationErrorAsync(string awsRequestId, Exception exception)
        {
            var exceptionInfo = new ExceptionInfo(exception);

            string exceptionInfoJson = JsonSerializer.Serialize(exception, ApplicationJsonSerializerContext.Default.Exception);

            return internalClient.ErrorWithXRayCauseAsync(awsRequestId, exceptionInfo.ErrorType, exceptionInfoJson);
        }

        public async Task SendResponseAsync(string awsRequestId, Stream? outputStream)
        {
            await internalClient.ResponseAsync(awsRequestId, outputStream);
        }
    }
}
