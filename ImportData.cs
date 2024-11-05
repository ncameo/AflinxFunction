using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AflinxFunction
{
    public static class ImportData
    {
        [FunctionName("ImportData")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var outputs = new List<string>();
            dynamic data = context.GetInput<dynamic>();

           
            int total=data.Count;
            log.LogInformation("Started orchestration with ID = '{total}'.", total);
            int count =1;
            foreach (dynamic input in data)
            {
                string nd = JsonConvert.SerializeObject(input);
                log.LogInformation("Started orchestration with ID = '{nd}'.", nd);
                if (count == 1)
                {
                  
                    string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");
                    UriBuilder builder = new UriBuilder(NotificationAPIUrl + "api/FileProcessing/updatefilestatus?id="+Convert.ToString(input.DataFileId) + "&Status=InProgress");

                    var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
                 

                    var httpClient = new HttpClient();
                    HttpResponseMessage ResponseMessage = httpClient.Send(request);
                }
                outputs.Add(await context.CallActivityAsync<object>(nameof(CreateUser), input));
                
               
                if (count == total)
                {
                    string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");
                    UriBuilder builder = new UriBuilder(NotificationAPIUrl + "api/FileProcessing/updatefilestatus?id=" + Convert.ToString(input.DataFileId) + "&Status=Completed");

                    var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);


                    var httpClient = new HttpClient();
                    HttpResponseMessage ResponseMessage = httpClient.Send(request);
                }
                count=count+1;
            }
            // Replace "hello" with the name of your Durable Activity Function.
         
           // outputs.Add(await context.CallActivityAsync<string>(nameof(CreateUser), "Seattle"));
           // outputs.Add(await context.CallActivityAsync<string>(nameof(CreateUser), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(CreateUser))]
        public static string CreateUser([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            dynamic data = context.GetInput<dynamic>();
            string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");
            UriBuilder builder = new UriBuilder(NotificationAPIUrl + "api/FileProcessing/uploaduser");

            var request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            if (data != null)
            {
               
                var content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");

                request.Content = content;

            }

            //request.Headers.Add("Content-Type", "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage ResponseMessage = httpClient.Send(request);
            string id = Convert.ToString(data.Id);
            log.LogInformation(id);
            log.LogInformation("Saying hello to {name}.");
            return $"Hello !";
        }

        [FunctionName("ImportData_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {

            dynamic data = await req.Content.ReadAsAsync<dynamic>();
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ImportData", data);
            //string nd = JsonConvert.SerializeObject(data);
           // log.LogInformation("Started orchestration with ID = '{nd}'.", nd);
            //int total = data.Count;
            //log.LogInformation("Started orchestration with ID = '{total}'.", total);
            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}