using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public delegate Task<InvocationResponse> LambdaBootstrapHandler(InvocationRequest invocation);
    public delegate Task<bool> LambdaBootstrapInitializer();

    public class LambdaBootstrap
    {
        private readonly LambdaBootstrapHandler? handler;
        private readonly RuntimeApiClient client;

        public LambdaBootstrap(HttpClient httpClient, LambdaBootstrapHandler? handler)
        {
            this.handler = handler;
            client = new RuntimeApiClient(httpClient);
        }

        public async Task RunAsync()
        {
            while (true)
            {
                using (var invocation = await client.GetNextInvocationAsync())
                {
                    InvocationResponse? response = null;
                    bool invokeSucceeded = false;

                    try
                    {
                        response = await handler!(invocation);
                        invokeSucceeded = true;
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine(exception);
                        await client.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, exception);
                    }

                    if (invokeSucceeded)
                    {
                        try
                        {
                            await client.SendResponseAsync(invocation.LambdaContext.AwsRequestId, response?.OutputStream);
                        }
                        finally
                        {
                            if (response != null && response.DisposeOutputStream)
                            {
                                response.OutputStream?.Dispose();
                            }
                        }
                    }
                }
            }
        }
    }

}
