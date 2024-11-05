using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AflinxFunction
{
    public class MobileNotification
    {
        [FunctionName("MobileNotification")]
        public void Run([ServiceBusTrigger("mobilenotification", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            dynamic data = JsonConvert.DeserializeObject(myQueueItem);

            string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");
            if (data.userId != null)
            {
                foreach (var userId in data.userId)
                {
                    data.touserId = Convert.ToString(userId);

                    UriBuilder builder = new UriBuilder(NotificationAPIUrl+ "api/notificationque/sendnotification");

                    var request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                    if (data != null)
                    {
                        var content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");

                        request.Content = content;

                    }

                    //request.Headers.Add("Content-Type", "application/json");

                    var httpClient = new HttpClient();
                    HttpResponseMessage ResponseMessage = httpClient.Send(request);
                    log.LogInformation($"Email: {Convert.ToString(ResponseMessage.Content)}");
                }
            }
            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
