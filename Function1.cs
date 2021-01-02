using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace LoadSimulator
{
    public static class SimulateLoad
    {
        [FunctionName("Function1")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await context.CallActivityAsync("SimulateLoad_SimulateTimeConsumingProcess", "");
        }

        [FunctionName("SimulateLoad_SimulateTimeConsumingProcess")]
        public static void SimulateTimeConsumingProcess([ActivityTrigger] string _, ILogger log)
        {
            log.LogInformation($"Starting time consuming process");
            Thread.Sleep(20 * 1000);
            log.LogInformation($"Finished time consuming process");
        }

        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}